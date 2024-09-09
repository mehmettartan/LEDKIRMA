using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace LEDKIRMA
{
    class getWLength
    {
        [DllImport("feasacom64.dll")]
        private static extern int FeasaCom_Open(int CommPort, string Baudrate);

        [DllImport("feasacom64.dll")]
        private static extern int FeasaCom_Close(int CommPort);

        [DllImport("feasacom64.dll")]
        private static extern int FeasaCom_Send(int CommPort, string Command, StringBuilder ResponseText);

        [DllImport("feasacom64.dll")]
        private static extern int FeasaCom_SetResponseTimeout(uint Timeout);
        public bool CaptureAndRead(int portNumber, int numFibers, out string[] wavelengths)
        {
            wavelengths = new string[numFibers]; // Wavelength değerlerini tutacak dizi

            StringBuilder buffer = new StringBuilder(100); // Cevapları tutacak buffer

            // Bağlantı süresini artırmak için maksimum zaman aşımını artır
            FeasaCom_SetResponseTimeout(8000); //8000ms

            // Bağlantıyı aç
            if (FeasaCom_Open(portNumber, "115200") == 1)
            {
                // Hata yok

                // Yakalama komutunu gönder
                int resp = FeasaCom_Send(portNumber, "CAPTURE", buffer);
                if (resp == -1)
                {
                    MessageBox.Show("Hata! Komut gönderilemedi!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    FeasaCom_Close(portNumber);
                    return false;
                }
                else if (resp == 0)
                {
                    MessageBox.Show("Zaman aşımı algılandı!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    FeasaCom_Close(portNumber);
                    return false;
                }

                for (int i = 1; i <= numFibers; i++)
                {
                    // Her fiber için wavelength değerini almak için komut gönder
                    string fiberNumber = i.ToString("D2");
                    string command = "GETWAVELENGTH" + fiberNumber;
                    resp = FeasaCom_Send(portNumber, command, buffer);
                    if (resp == -1)
                    {
                        MessageBox.Show("Hata! Komut gönderilemedi!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        FeasaCom_Close(portNumber);
                        return false;
                    }
                    else if (resp == 0)
                    {
                        MessageBox.Show("Zaman aşımı algılandı!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        FeasaCom_Close(portNumber);
                        return false;
                    }

                    // Cevabı boşluk karakteriyle ayırarak parçala
                    string[] auxlist = buffer.ToString().Split(' ');
                    wavelengths[i - 1] = auxlist[0]; // Wavelength değeri
                }

                // Bağlantıyı kapat
                FeasaCom_Close(portNumber);

                return true;
            }
            else
            {
                // Seçilen bağlantı noktasını açamama hatası
                MessageBox.Show("Bağlantı noktası açılamıyor", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }
        }
    }
}
