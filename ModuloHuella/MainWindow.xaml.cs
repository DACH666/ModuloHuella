using AForge.Video;
using AForge.Video.DirectShow;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;


namespace ModuloHuella
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int iRet = FPutils.FP_SUCCESS;
        int index = 0;
        byte[] image = new byte[FPutils.FP_IMAGE_WIDTH * FPutils.FP_IMAGE_HEIGHT];
        byte[] bmp = new byte[FPutils.FP_IMAGE_WIDTH * FPutils.FP_IMAGE_HEIGHT + FPutils.FP_BMP_HEADER];
        byte[] tempArray = new byte[FPutils.FP_FTP_MAX * 1000];
        int nTemp = 0;
        static FPMsg.FpMessageHandler FpMessageHandler = null;
   
        private bool _isCapturing;
        private byte[] huellaCapturada;
        private byte[] fotoCapturada;
        private VideoCaptureDevice videoSource;
        private FilterInfoCollection videoDevices;




        public MainWindow()
        {
            InitializeComponent();
            FpMessageHandler = new FPMsg.FpMessageHandler(FpMessageCallback);
            FPutils.FPModule_InstallMessageHandler(FpMessageHandler);
            GC.KeepAlive(FpMessageHandler);
            this.videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            this.videoSource = new VideoCaptureDevice(this.videoDevices[0].MonikerString);
            this.videoSource.NewFrame += new NewFrameEventHandler(this.video_NewFrame);
        }

        private void CapturaHuella_Click(object sender, RoutedEventArgs e)
        {
            FpMessageCallback(FPMsg.FP_MSG_TYPE_T.FP_MSG_PRESS_FINGER, IntPtr.Zero);
            iRet = FPutils.FPModule_OpenDevice();
            if (iRet == FPutils.FP_SUCCESS)
            {
                byte[] data = new byte[FPutils.FP_FTP_MAX];

                FPutils.FPModule_SetCollectTimes(15);
                iRet = FPutils.FPModule_FpEnroll(data);
                if (iRet == FPutils.FP_SUCCESS)
                {

                    Buffer.BlockCopy(data, 0, tempArray, nTemp * FPutils.FP_FTP_MAX, FPutils.FP_FTP_MAX);
                    //MessageBox.Show("FpEnroll success ,Score" + FPutils.FPModule_GetQuality(data) + "ID:" + nTemp + "Cnt:" + (nTemp + 1));
                    SaveTemp("temp.dat", data);
                    nTemp++;
                    huellaCapturada = data;
                    TextoHuella.Content = "Huella capturada con exito.";
                    ProgressBarCaptura.Value = 100;
                }
                else
                {
                    MessageBox.Show("Error al capturar huella");

                }
            }
        }
        private void InsertFinferprintToDatabase(byte[] fingerprintData)
        {
            string connectionString = "Data Source=HuellaDb.db";
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                string query = "INSERT INTO Cliente (Huella) VALUES (@Huella)";
                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Huella", fingerprintData);
                    int result = command.ExecuteNonQuery();
                    if (result > 0)
                    {
                        ProgressBarCaptura.Value = 100;
                    }
                    else
                    {
                        MessageBox.Show("Error al agregar la huellaS.");
                    }
                }
            }
        }

        public void CapturaFoto_Click(object sender, RoutedEventArgs e)

        {
            if (videoSource != null && videoSource.IsRunning)
            {
                videoSource.SignalToStop();
                videoSource.WaitForStop();
                TextoHuella.Content = "Foto capturada con exito!";
                string filePath = "captured_image.jpg";
                SaveCapturedImage(filePath);
            }
        }

        private void SaveCapturedImage(string filePath)
        {
            if (webcamImage.Source is BitmapImage bitmapImage)
            {
                var encoder = new JpegBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bitmapImage));

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    encoder.Save(fileStream);
                }
            }
        }


        private void SaveTemp(string path, byte[] szTmp)
        {
            FileStream file = new FileStream(path, FileMode.Create);
            //将byte数组写入文件中
            file.Write(szTmp, 0, szTmp.Length);
            //所有流类型都要关闭流，否则会出现内存泄露问题
            file.Close();
        }
        void FpMessageCallback(FPMsg.FP_MSG_TYPE_T enMsgType, IntPtr pMsgData)
        {
            switch (enMsgType)
            {
                case FPMsg.FP_MSG_TYPE_T.FP_MSG_RISE_FINGER:
                    TextoHuella.Content = "Coloca y levanta tu dedo.";
                    ProgressBarCaptura.Value = 0;
                    break;

                case FPMsg.FP_MSG_TYPE_T.FP_MSG_PRESS_FINGER:
                    TextoHuella.Content = "Coloca tu dedo.";
                    ProgressBarCaptura.Value = 0;
                    break;

                case FPMsg.FP_MSG_TYPE_T.FP_MSG_ENROLL_TIME:
                    int[] time = new int[1];
                    Marshal.Copy(pMsgData, time, 0, 1);
                    //TextoHuella.Content = "Enroll Time: " + time[0];
                    index = time[0];
                    break;

                case FPMsg.FP_MSG_TYPE_T.FP_MSG_CAPTURED_IMAGE:
                    object obj = Marshal.PtrToStructure(pMsgData, typeof(FPutils.FP_IMAGE_DATA));
                    var pInfos = (FPutils.FP_IMAGE_DATA)obj;

                    Marshal.Copy(pInfos.pbyImage, image, 0, pInfos.dwWidth * pInfos.dwHeight);
                    FPutils.ImgBufferToBmpBuffer(image, pInfos.dwWidth, pInfos.dwHeight, bmp);

                    using (MemoryStream ms = new MemoryStream(bmp, true))
                    {
                        ms.Position = 0;
                        BitmapImage bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.StreamSource = ms;
                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
                        bitmap.EndInit();
                        HuellaI.Source = bitmap;

                        string filePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"Enroll{index}.bmp");
                        using (FileStream fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                        {
                            ms.WriteTo(fileStream);
                        }
                    }
                    break;
            }

            Application.Current.Dispatcher.Invoke(() => { }, System.Windows.Threading.DispatcherPriority.Background);
        }

        public void video_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            BitmapImage bitmapImage;

            using (var memoryStream = new MemoryStream())
            {
                eventArgs.Frame.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Bmp);
                memoryStream.Position = 0;

                bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memoryStream;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze();
            }

            Dispatcher.Invoke(() =>
            {
                webcamImage.Source = bitmapImage;
            });
        }
        public BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                bitmap.Save((Stream)memoryStream, ImageFormat.Bmp);
                memoryStream.Position = 0L;
                BitmapImage imageSource = new BitmapImage();
                imageSource.BeginInit();
                imageSource.StreamSource = (Stream)memoryStream;
                imageSource.CacheOption = BitmapCacheOption.OnLoad;
                imageSource.EndInit();
                return imageSource;
            }
        }
        private void TextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {

        }

        private void Button_Enviar(object sender, RoutedEventArgs e)
        {
            if (huellaCapturada == null)
            {
                MessageBox.Show("Por favor, capture la huella antes de enviar.");
                return;

            }
            string nombre = Nombre.Text;
            string apellidoP = ApellidoP.Text;
            string apellidoM = ApellidoM.Text;
            string correo = Correo.Text;
            string celular = Celular.Text;
            string filePath = "captured_image.jpg";

            byte[] imageBytes = File.ReadAllBytes(filePath);

            string connectionString = "Data Source=HuellaDb.db;";
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                var getIdCommand = connection.CreateCommand();
                getIdCommand.CommandText =
                   @"SELECT IFNULL(MAX(IdCliente), 0) + 1 FROM Cliente";

                int newIdCliente;

                try
                {

                    newIdCliente = Convert.ToInt32(getIdCommand.ExecuteScalar());
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Hubo un error al generar el nuevo Id: {ex.Message}");
                    return;
                }

                var command = connection.CreateCommand();
                command.CommandText =
                    @"INSERT INTO Cliente (IdCliente, Nombre, ApellidoP, ApellidoM, Correo, Celular, Huella, Foto)
                    VALUES (@IdCliente, @Nombre, @ApellidoP, @ApellidoM, @Correo, @Celular, @Huella, @Foto)
                ";
                command.Parameters.AddWithValue("@IdCliente", newIdCliente);
                command.Parameters.AddWithValue("@Nombre", nombre);
                command.Parameters.AddWithValue("@ApellidoP", apellidoP);
                command.Parameters.AddWithValue("@ApellidoM", apellidoM);
                command.Parameters.AddWithValue("@Correo", correo);
                command.Parameters.AddWithValue("@Celular", celular);
                command.Parameters.AddWithValue("@Huella", huellaCapturada);
                command.Parameters.AddWithValue("@Foto", imageBytes);

                try
                {
                    command.ExecuteNonQuery();
                    MessageBox.Show("Registro exitoso");
                    SaveImageToDatabase(filePath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Hubo un error al registrar: {ex.Message}");
                }
            }
        }


        private void ToggleButton_Checked(object sender, RoutedEventArgs e)
        {

            StartCapture();
        }
        private void ToggleButton_UnChecked(object sender, RoutedEventArgs e)
        {
            StopCapture();
        }


        private void StartCapture()
        {
            videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            if (videoDevices.Count == 0)
            {
                MessageBox.Show("No video devices found.");
                return;
            }

            videoSource = new VideoCaptureDevice(videoDevices[0].MonikerString);
            videoSource.NewFrame += new NewFrameEventHandler(video_NewFrame);
            videoSource.Start();
        }


        private void StopCapture()
        {
            
                if (videoSource != null && videoSource.IsRunning)
                {
                    videoSource.SignalToStop();
                    videoSource.WaitForStop();
                }
        }


        public BitmapImage LoadBitmap(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                return bitmapImage;
            }
        }

        private void SaveImageToDatabase(string filePath)
        {

        }

        private void Button_CapturaFoto(object sender, RoutedEventArgs e)
        {

        }

        private void ProgressBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

        }

        protected override void OnClosed(EventArgs e)
        {
            if (videoSource != null && videoSource.IsRunning)
            {
                videoSource.SignalToStop();
                videoSource.WaitForStop();
            }
            base.OnClosed(e);
        }


    }
}



