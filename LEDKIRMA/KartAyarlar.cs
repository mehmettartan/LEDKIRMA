using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;

namespace LEDKIRMA
{
    public partial class KartAyarlar : Form
    {
        private BacgroundWorker bacgroundWorker;
        string[] ports;
        bool comBusy;
        private SerialPort _serialPort;

        //bool btnAkımVolt;

        private static KartAyarlar kartAyarInstance;
        public KartAyarlar()
        {
            InitializeComponent();

            // Arka plan işçisini başlat
            bacgroundWorker = new BacgroundWorker();

            // KonveyorKontrolleri delegesine özel kontrol işlevini atayın
            bacgroundWorker.KartAyarlarıKOntrolleri = KartAyarlarıKontrolleriMetodu;

            this.txtBxAkVolt1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtBxBslemeVolt_KeyDown);

            // Seri portu yapılandır
            _serialPort = new SerialPort
            {
                BaudRate = 2400,
                Parity = Parity.None,
                DataBits = 8,
                StopBits = StopBits.One,
                Handshake = Handshake.None
            };
        }

        private void KartAyarlarıKontrolleriMetodu()
        {
            ReadVolt();
            ReadCurrent();
        }

        private void ReadVolt()
        {
        if (_serialPort.IsOpen)
        {
            _serialPort.Close();
        }

        _serialPort.Open();
        _serialPort.DiscardInBuffer();  // Read buffer'ı temizle

        // Voltaj sorgulama komutu gönder
        string command = "V\r";
        _serialPort.Write(command);

        // Cevabı oku
        string response = _serialPort.ReadLine(); // \n karakterine kadar okur

        // Eğer cevap V ile başlıyorsa V karakterini çıkart
        if (response.StartsWith("V"))
        {
            response = response.Substring(1).Trim();
        }

        // UI iş parçacığında TextBox'a yaz
        this.Invoke(new MethodInvoker(() =>
        {
            txtBxAkVolt3.Text = response;
        }));

        _serialPort.Close();

        }
        private void ReadCurrent()
        {
            if (comBusy == false)
            {
                if (_serialPort.IsOpen)
                {
                    _serialPort.Close();
                }

                _serialPort.Open();
                _serialPort.DiscardInBuffer();  // Read buffer'ı temizle

                // Voltaj sorgulama komutu gönder
                string command = "A\r";
                _serialPort.Write(command);

                // Cevabı oku
                string response = _serialPort.ReadLine(); // \n karakterine kadar okur

                // Eğer cevap V ile başlıyorsa V karakterini çıkart
                if (response.StartsWith("A"))
                {
                    response = response.Substring(1).Trim();
                }

                // UI iş parçacığında TextBox'a yaz
                this.Invoke(new MethodInvoker(() =>
                {
                    txtBxAkVolt6.Text = response;
                }));

                _serialPort.Close();
            }

            else
            {
                MessageBox.Show("COMPort Meşgul");
            }

        }


        public static KartAyarlar GetInstance()
        {
            // Eğer form henüz oluşturulmadıysa veya kapatıldıysa, yeni bir örneği oluşturun
            if (kartAyarInstance == null || kartAyarInstance.IsDisposed)
            {
                kartAyarInstance = new KartAyarlar();
            }
            return kartAyarInstance;
        }

        private void btnYeniMdl_Click(object sender, EventArgs e)
        {
            txtBxMdlAd.Text = "";
            txtBxPcbBoy.Text = "";
            txtBxPcbAdt.Text = "";
            txtBxKartAl.Text = "";
            txtBxKartTst.Text = "";
            txtBxBslemeVolt.Text = "";
            txtBxBslemeAkm.Text = "";
            txtBxAkVolt4.Text = "";
            txtBxAkVolt5.Text = "";
            txtBxAkVolt1.Text = "";
            txtBxAkVolt2.Text = "";
            txtBxAkVolt6.Text = "";
            txtBxAkVolt3.Text = "";
            btnAkmVoltTest.Enabled = false;
            btnFeasaTest.Enabled = false;
        }

        private void btnMdlSec_Click(object sender, EventArgs e)
        {

            OpenFileDialog dosyaDialog = new OpenFileDialog();

            dosyaDialog.Filter = "MDL Dosyaları|*.mdl";

            DialogResult result = dosyaDialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                txtBxMdlAd.Text = dosyaDialog.FileName;
            }

        }

        private void btnAkmVoltTest_Click(object sender, EventArgs e)
        {
            if (btnAkmVoltTest.Text == "PASİF")
            {
                btnAkmVoltTest.Text = "AKTİF";
                btnAkmVoltTest.BackColor = Color.DarkOliveGreen;

                EnableTextBoxes(true);

                bacgroundWorker.Baslat();



            }
            else
            {
                btnAkmVoltTest.Text = "PASİF";
                btnAkmVoltTest.BackColor = Color.DarkRed;

                EnableTextBoxes(false);

                limitUpdate();

                bacgroundWorker.Durdur();
            }
        }

        private void EnableTextBoxes(bool enable)
        {
            for (int i = 1; i <= 6; i++)
            {
                // TextBox kontrolünü bul
                var textBox = this.Controls.Find("txtBxAkVolt" + i, true).FirstOrDefault() as TextBox;

                if (textBox != null)
                {
                    // Etkinleştir veya pasifleştir
                    textBox.Enabled = enable;
                    // Arka plan rengini ayarla
                    textBox.BackColor = enable ? Color.WhiteSmoke : Color.FromArgb(31,31,31);
                }
            }
        }

        private void limitUpdate()
        {

        }

        private void btnFeasaTest_Click(object sender, EventArgs e)
        {
            if (btnFeasaTest.Text == "PASİF")
            {
                btnFeasaTest.Text = "AKTİF";
                btnFeasaTest.BackColor = Color.DarkOliveGreen;
            }
            else
            {
                btnFeasaTest.Text = "PASİF";
                btnFeasaTest.BackColor = Color.DarkRed;
            }
        }

        private void buttonMotYurutmeUp_Click(object sender, EventArgs e)
        {

        }

        private void buttonMotYurutmeDown_Click(object sender, EventArgs e)
        {

        }

        private void buttonTestMotUp_Click(object sender, EventArgs e)
        {

        }

        private void buttonTestMotDown_Click(object sender, EventArgs e)
        {

        }

        private void buttonTestMotKartAl_Click(object sender, EventArgs e)
        {

        }

        private void buttonTestMotKartTest_Click(object sender, EventArgs e)
        {

        }

        private void buttonTestMotKartBırak_Click(object sender, EventArgs e)
        {

        }

        private void buttonAdetSıfırla_Click(object sender, EventArgs e)
        {

        }

        private void buttonManuelTest_Click(object sender, EventArgs e)
        {

        }

        private void btnMin_Click(object sender, EventArgs e)
        {

        }
        private void buttonKonvUp_Click(object sender, EventArgs e)
        {

        }

        private void buttonKonvDown_Click(object sender, EventArgs e)
        {

        }

        private void buttonDonmePistUp_Click(object sender, EventArgs e)
        {

        }

        private void buttonDonmePistDown_Click(object sender, EventArgs e)
        {

        }

        private void buttonKrmaPistUp_Click(object sender, EventArgs e)
        {

        }

        private void buttonKrmaPistDown_Click(object sender, EventArgs e)
        {

        }

        private void buttonTestPistUp_Click(object sender, EventArgs e)
        {

        }

        private void buttonTestPistDown_Click(object sender, EventArgs e)
        {

        }

        private void buttonYurutmeGrippUp_Click(object sender, EventArgs e)
        {

        }

        private void buttonYurutmeGrippDown_Click(object sender, EventArgs e)
        {

        }

        private void buttonTestGrippUp_Click(object sender, EventArgs e)
        {

        }

        private void buttonTestGrippDown_Click(object sender, EventArgs e)
        {

        }

        private void buttonBaskPistUp_Click(object sender, EventArgs e)
        {

        }

        private void buttonBaskPistDown_Click(object sender, EventArgs e)
        {

        }

        private void btnMdlLoad_Click(object sender, EventArgs e)
        {

        }

        private void button58_Click(object sender, EventArgs e)
        {
            Anasayfa anasayfa = Anasayfa.GetInstance(); // Mevcut form örneğini al
            anasayfa.ShowForm(); // aNASAYFA formunu göster

            HideForm();
        }

        private void button117_Click(object sender, EventArgs e)
        {
            FeasaAyarlar feasaAyarlar = FeasaAyarlar.GetInstance(); // Mevcut form örneğini al
            feasaAyarlar.ShowForm(); // UrunAyr formunu göster

            HideForm();
        }

        public void HideForm()
        {
            this.Hide(); // Formu gizle
        }

        // Formu tekrar göstermek için bir yöntem oluşturun
        public void ShowForm()
        {
            this.Show(); // Formu göster
        }
        private void exit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void label33_Click(object sender, EventArgs e)
        {

        }

        private void KartAyarlar_Load(object sender, EventArgs e)
        {
            // COM portlarını al
            ports = System.IO.Ports.SerialPort.GetPortNames();

            // ComboBox'ı temizle
            cbCOM.Items.Clear();

            // Her bir COM portunu ComboBox'a ekle
            foreach (string port in ports)
            {
                cbCOM.Items.Add(port);
            }

            // Eğer en az bir port varsa, ilkini seç
            if (cbCOM.Items.Count > 0)
            {
                cbCOM.SelectedIndex = 2;
            }
        }

        private void cbCOM_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbCOM.SelectedIndex >= 0)
            {
                string selectedPort = cbCOM.SelectedItem.ToString();
                _serialPort.PortName = selectedPort;
            }
        }

        private void txtBxBslemeVolt_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                //comBusy = true;
                // Enter tuşuna basıldı
                string value = txtBxBslemeVolt.Text;
                string command = $"SV {value}\r"; // Komut formatı: U<değer><CR><LF>

                // Komutu seri porta gönder
                SendCommandToSerialPort(command);
                //comBusy = false;

                // Enter tuşunu işlediğimiz için bu olayın başka bir işlem yapmasını engelle
                e.SuppressKeyPress = true;
            }
        }

        private void txtBxBslemeAkm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                //comBusy = true;
                // Enter tuşuna basıldı
                string value = txtBxBslemeAkm.Text;
                string command = $"SI {value}\r"; // Komut formatı: U<değer><CR><LF>

                // Komutu seri porta gönder
                SendCommandToSerialPort(command);

                //comBusy = false;

                // Enter tuşunu işlediğimiz için bu olayın başka bir işlem yapmasını engelle
                e.SuppressKeyPress = true;
            }
        }

        private void SendCommandToSerialPort(string command)
        {
            try
            {
                if (!_serialPort.IsOpen)
                {
                    _serialPort.Open();
                }

                _serialPort.WriteLine(command);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Komut gönderilemedi: " + ex.Message);
            }
        }
    }
}
