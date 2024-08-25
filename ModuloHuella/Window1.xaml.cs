using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ModuloHuella
{
    /// <summary>
    /// Lógica de interacción para Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        int iRet = FPutils.FP_SUCCESS;


        public Window1()
        {
            iRet = FPutils.FPModule_OpenDevice();
            InitializeComponent();
        }

        private void checar_Click(object sender, RoutedEventArgs e)
        {
            // Paso 1: Obtener la huella digital a comparar
            byte[] huellaMatch = GetFingerprintDataToMatch(); // Tu método para obtener la huella digital actual

            // Paso 2: Obtener todas las huellas digitales de la base de datos junto con sus idcliente
            var huellas = getHuellasDb();

            // Paso 3: Iterar sobre cada huella digital y compararla
            foreach (var huella in huellas)
            {
                int matchResult = FPutils.FPModule_MatchTemplate(huella.Data, huellaMatch, 3);

                if (matchResult == FPutils.FP_SUCCESS)
                {
                    FPutils.FPModule_SetCollectTimes(3);
                    textBoxId.Text =(huella.IdCliente.ToString());
                    return;
                }
            }

            MessageBox.Show("Fingerprint match failed");

        }

        class datosHuella
        {
            public int IdCliente { get; set; }
            public byte[] Data { get; set; }
        }

        // Ejemplo de método para obtener todas las huellas digitales de la base de datos
        private List<datosHuella> getHuellasDb()
        {
            var huellas = new List<datosHuella>();

            string connectionString = "Data Source=HuellaDb.db;";
            string query = "SELECT idcliente, Huella FROM Cliente;";

            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                using (var command = new SqliteCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var huella = new datosHuella
                        {
                            IdCliente = reader.GetInt32(0),
                            Data = (byte[])reader.GetValue(1)
                        };
                        huellas.Add(huella);
                    }
                }
            }

            return huellas;
        }




        // Método de ejemplo para obtener la huella digital a comparar (puede variar según tu implementación)
        private byte[] GetFingerprintDataToMatch()
        {
            byte[] data = new byte[FPutils.FP_FTP_MAX];
            FPutils.FPModule_SetCollectTimes(1);
            int iRet = FPutils.FPModule_FpEnroll(data);

            if (iRet == FPutils.FP_SUCCESS)
            {
                // Aquí asumimos que el método FPModule_FpEnroll almacena la huella en 'data'
                // y que este es el formato correcto para la comparación
                return data;
            }
            else
            {
                // Manejar el caso de error
                MessageBox.Show("Error al obtener la huella digital del dispositivo.");
                return null;
            }


        }
     
            public void SetFingerprintImage(BitmapImage image)
            {
                fpImageBox.Source = image;
            }
        

        private void textBoxId_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
