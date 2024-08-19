using System.Collections.Generic;
using System.IO;
using System;
using System.Text;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Data;
using System.Windows;
using System.Text.RegularExpressions;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using Microsoft.Data.Sqlite;
using System.Reflection.Emit;

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
        static FPMsg.FpMessageHandler FpMessageHandler = null;//防止Invoke异常
        public MainWindow()
        { 
            FpMessageHandler = new FPMsg.FpMessageHandler(FpMessageCallback);
            FPutils.FPModule_InstallMessageHandler(FpMessageHandler);
            GC.KeepAlive(FpMessageHandler);
        }

        private void Button_CapturaHuella(object sender, RoutedEventArgs e)
        {
            iRet = FPutils.FPModule_OpenDevice();
            if (iRet == FPutils.FP_SUCCESS)
            {
                CapturarFoto(sender, e);
                byte[] data = new byte[FPutils.FP_FTP_MAX];

                FPutils.FPModule_SetCollectTimes(5);
                iRet = FPutils.FPModule_FpEnroll(data);
                if (iRet == FPutils.FP_SUCCESS)
                {

                    Buffer.BlockCopy(data, 0, tempArray, nTemp * FPutils.FP_FTP_MAX, FPutils.FP_FTP_MAX);
                    //MessageBox.Show("FpEnroll success ,Score" + FPutils.FPModule_GetQuality(data) + "ID:" + nTemp + "Cnt:" + (nTemp + 1));
                    SaveTemp("temp.dat", data);
                    nTemp++;
                    InsertFinferprintToDatabase(data);
                }
                else
                {
                    MessageBox.Show("FpEnroll failed");
                }

             /*   BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.StreamSource = ms;
                bitmap.EndInit();
                bitmap.Freeze();

                if (bitmap != null)
                {
                    HuellaI.Source = bitmap;
                }
                else
                {
                    MessageBox.Show("Error al mostrar la imagen");
                }

            }
            else
            {
                MessageBox.Show("OpenDevice failed");*/
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
                        MessageBox.Show("La Huella a sido agregada satisfactoriamente.");
                    }
                    else
                    {
                        MessageBox.Show("Error al agregar la huellaS.");
                    }
                }
            }
        }

        private void CapturarFoto(object sender, RoutedEventArgs e)

        {
            int w = 0, h = 0;
            iRet = FPutils.FPModule_CaptureImage(image, ref w, ref h);
            if (iRet == FPutils.FP_SUCCESS)
            {
                // Escribir imagen en archivo
                // Convertir imagen binaria de frotamiento al formato BMP   
                FPutils.ImgBufferToBmpBuffer(image, w, h, bmp);

                // escribir archivo
                System.IO.MemoryStream ms = new MemoryStream(bmp, true);
                ms.Position = 0;
                ms.Seek(0, SeekOrigin.Begin);
                byte[] imagebytes = ms.ToArray();

                /* Insertar la imagen a la bd

                string connectionString = "Data Source=HuellaDb.db";
                using (var connection = new SqliteConnection(connectionString))
                {
                    connection.Open();
                    string query = "INSERT INTO Cliente (Huella) VALUES (@Huella)";
                    using (var command = new SqliteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("Huella", imagebytes);
                        command.ExecuteNonQuery();
                   */

                // Mostrado en la interfaz
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.StreamSource = ms;
                bitmap.EndInit();
                bitmap.Freeze();

                if (bitmap != null)
                {
                    HuellaI.Source = bitmap;
                }
                else
                {
                    MessageBox.Show("Error al mostrar la imagen");
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

            if (enMsgType == FPMsg.FP_MSG_TYPE_T.FP_MSG_RISE_FINGER)
            {
                TextoHuella.Content = ("Levanta y coloca tu dedo");
            }
            else if (enMsgType == FPMsg.FP_MSG_TYPE_T.FP_MSG_PRESS_FINGER)
            {
               TextoHuella.Content = ("Coloca tu dedo");
            }
            else if (enMsgType == FPMsg.FP_MSG_TYPE_T.FP_MSG_ENROLL_TIME)
            {
                int[] time = new int[1];
                Marshal.Copy(pMsgData, time, 0, 1);
                index = time[0];
            }
            else if (enMsgType == FPMsg.FP_MSG_TYPE_T.FP_MSG_CAPTURED_IMAGE)
            {
                object obj = Marshal.PtrToStructure(pMsgData, typeof(FPutils.FP_IMAGE_DATA));
                var pInfos = (FPutils.FP_IMAGE_DATA)obj;

                Marshal.Copy(pInfos.pbyImage, image, 0, pInfos.dwWidth * pInfos.dwHeight);
                FPutils.ImgBufferToBmpBuffer(image, pInfos.dwWidth, pInfos.dwHeight, bmp);

                // 写入文件
                System.IO.MemoryStream ms = new MemoryStream(bmp, true);
                ms.Position = 0;
                Image _Img = Image.FromStream(ms);
                _Img.Save(Path.Combine(Environment.CurrentDirectory, "Enroll" + index + ".bmp"));

                // 在界面显示
                CapturarFoto(null, null);
               /* BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.StreamSource = ms;
                bitmap.EndInit();
                bitmap.Freeze();

                if (bitmap != null)
                {
                    HuellaI.Source = bitmap;
                }
                else
                {
                    MessageBox.Show("Error al mostrar la imagen");
                }*/
            }
        }

        private void TextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {

        }
        private void ToggleButton_Foto(object sender, RoutedEventArgs e)
        {

        }
        private void Button_Enviar(object sender, RoutedEventArgs e)
        {



        }

        private void Button_Captura(object sender, RoutedEventArgs e)
        {

        }

    }
}
    
