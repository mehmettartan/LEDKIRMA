using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LEDKIRMA
{
    public partial class FeasaAyarlar : Form
    {
        //private BacgroundWorker bacgroundWorker;
        List<int> hatalıIndeksler = new List<int>();

        private static FeasaAyarlar feasaAyarInstance;
        int portNumber;
        int selectedPort;
        private iniFile inifile;


        List<string> receivedDataList = new List<string>();
        string command;

        bool continueRGB = false;
        bool continueCIEXY = false;
        bool continueCIEUV = false;
        bool continueSAT = false;
        bool continueHue = false;
        bool continueIntensity = false;
        bool continueCCT = false;
        bool continuewL = false;

        //FeasaAnalyser feasaAnalyser = new FeasaAnalyser("COM27");

        public event EventHandler<RgbButtonEventArgs> RgbButtonStateChanged;
        public event EventHandler<CIExyButtonEventArgs> CIExyButtonStateChanged;
        public event EventHandler<CIEuvButtonEventArgs> CIEuvButtonStateChanged;
        public event EventHandler<SatButtonEventArgs> SatButtonStateChanged;
        public event EventHandler<HueButtonEventArgs> HueButtonStateChanged;
        public event EventHandler<IntenstyButtonEventArgs> IntenstyButtonStateChanged;
        public event EventHandler<CCTButtonEventArgs> CCTButtonStateChanged;
        public event EventHandler<wLButtonEventArgs> wLButtonStateChanged;

        public static string LedSayi;

        public FeasaAyarlar()
        {
            InitializeComponent();

            inifile = new iniFile("settings.ini");

            // Arka plan işçisini başlat
            //bacgroundWorker = new BacgroundWorker();

            // KonveyorKontrolleri delegesine özel kontrol işlevini atayın
            //bacgroundWorker.FeasaAyarlarıKOntrolleri = FeasaAyarlarıKontrolleriMetodu;
            this.FormClosing += FeasaAyarlar_FormClosing; // FormClosing olayını ekleyin
        }

        private void FeasaAyarlarıKontrolleriMetodu()
        {

        }

        public static FeasaAyarlar GetInstance()
        {
            // Eğer form henüz oluşturulmadıysa veya kapatıldıysa, yeni bir örneği oluşturun
            if (feasaAyarInstance == null || feasaAyarInstance.IsDisposed)
            {
                feasaAyarInstance = new FeasaAyarlar();
            }
            return feasaAyarInstance;
        }

        private void FeasaAnalyser_DataReceivedEvent(object sender, string data)
        {
            kullaniciSayisi = Convert.ToInt32(txtBxLedSayi.Text); 

            // Gelen veriyi listeye ekle
            receivedDataList.Add(data);

            // Eğer listedeki veri sayısı kullanıcı sayısına eşitse, textboxlara yazmaya başla
            if (receivedDataList.Count == kullaniciSayisi)
            {
                for (int i = 0; i < kullaniciSayisi; i++)
                {
                    string[] dataArray = receivedDataList[i].Split(new[] { ' ', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

                    string textBoxName = $"textBoxFeasa{(i * 12) + 1}"; // Her 12. TextBox'ın adını kullanarak adı oluştur
                    Control[] controls = this.Controls.Find(textBoxName, true); // TextBox'ı bul
                    if (controls.Length > 0 && controls[0] is TextBox textBox)
                    {
                        int index = 0; // Veri dizisindeki başlangıç indeksi
                        if (index < dataArray.Length) // Veri dizisi boyutundan büyükse
                        {
                            // TextBox'a veriyi yerleştir
                            textBox.Text = dataArray[index];
                        }
                    }
                }

                // Verileri aldıktan sonra, listeyi temizle
                receivedDataList.Clear();

                // Portu kapat
                //feasaAnalyser.ClosePort();
            }
        }

        private void txtBxLedSayi_TextChanged(object sender, EventArgs e)
        {
            txtBxLedSayi.BackColor = Color.WhiteSmoke;

            Anasayfa anasayfa = Anasayfa.GetInstance();
            anasayfa.LedButtonDurumClean();
        }

        private void txtBxLedSayi_Leave(object sender, EventArgs e)
        {
            UpdateTextBoxes(txtBxLedSayi.Text);
        }

        private void txtBxLedSayi_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                UpdateTextBoxes(txtBxLedSayi.Text);
            }
        }
        private void UpdateTextBoxes(string input)
        {
            LedSayi = input;


            if (int.TryParse(input, out int ledSayi))
            {
                if (ledSayi > 15)
                {
                    MessageBox.Show("Lütfen 15'ten büyük bir sayı girmeyiniz!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return; // Uyarı verildiği için fonksiyondan çık
                }

                btnRGB.Enabled = true;
                btnCIExy.Enabled = true;
                btnCIEuy.Enabled = true;
                btnSaturation.Enabled = true;
                btnHue.Enabled = true;
                btnIntensity.Enabled = true;
                btnCCT.Enabled = true;
                btnWaveLength.Enabled = true;

                btnRGB.BackColor = Color.FromArgb(31, 31, 31);
                btnCIExy.BackColor = Color.FromArgb(31, 31, 31);
                btnCIEuy.BackColor = Color.FromArgb(31, 31, 31);
                btnSaturation.BackColor = Color.FromArgb(31, 31, 31);
                btnHue.BackColor = Color.FromArgb(31, 31, 31);
                btnIntensity.BackColor = Color.FromArgb(31, 31, 31);
                btnCCT.BackColor = Color.FromArgb(31, 31, 31);
                btnWaveLength.BackColor = Color.FromArgb(31, 31, 31);


                // Veriyi DataStorage.Instance.Veri'ye ata
                //DataStorage.Instance.Veri = ledSayi.ToString();

                int textBoxSayisi = Convert.ToInt32(txtBxLedSayi.Text);

                // Tüm Led TextBox'larının durumunu temizleyin
                for (int i = 1; i <= 15; i++)
                {
                    string textBoxName = "txtBxLed" + i;
                    Control[] controls = this.Controls.Find(textBoxName, true);

                    if (controls.Length > 0 && controls[0] is TextBox textBox)
                    {
                        textBox.Enabled = false; // Tüm Led TextBox'larını devre dışı bırakın
                        textBox.BackColor = Color.FromArgb(31, 31, 31); // Tüm Led TextBox'ların arka plan rengini varsayılan rengi yapın
                    }
                }

                // Etkinleştirilecek Led TextBox'ları
                for (int i = 0; i < textBoxSayisi; i++)
                {
                    string textBoxName = "txtBxLed" + (i + 1); // TextBox'ların isimlerini oluşturun
                    Control[] controls = this.Controls.Find(textBoxName, true); // Oluşturulan isme göre TextBox'ı bulun

                    if (controls.Length > 0 && controls[0] is TextBox textBox) // TextBox bulundu mu ve TextBox mı kontrol ediliyor
                    {
                        textBox.Enabled = true; // TextBox'ı etkinleştirin
                        textBox.BackColor = Color.WhiteSmoke; // TextBox'ın arka plan rengini ayarlayın
                    }
                }

                // Tüm Led TextBox'larının durumunu temizleyin
                for (int i = 1; i <= 180; i++)
                {
                    string textBoxName = "textBoxFeasa" + i;
                    Control[] controls = this.Controls.Find(textBoxName, true);

                    if (controls.Length > 0 && controls[0] is TextBox textBox)
                    {
                        textBox.Enabled = false; // Tüm Led TextBox'larını devre dışı bırakın
                        textBox.BackColor = Color.FromArgb(31, 31, 31); // Tüm Led TextBox'ların arka plan rengini varsayılan rengi yapın
                    }
                }

                // Tüm Led TextBox'larının durumunu temizleyin
                for (int i = 1; i <= 24; i++)
                {
                    string textBoxName = "txtBxMinMax" + i;
                    Control[] controls = this.Controls.Find(textBoxName, true);

                    if (controls.Length > 0 && controls[0] is TextBox textBox)
                    {
                        textBox.Enabled = false; // Tüm Led TextBox'larını devre dışı bırakın
                        textBox.BackColor = Color.FromArgb(31, 31, 31); // Tüm Led TextBox'ların arka plan rengini varsayılan rengi yapın
                    }
                }

                for (int i = 0; i < textBoxSayisi * 12; i++)
                {
                    string textBoxName = "textBoxFeasa" + (i + 1); // TextBox'ların isimlerini oluşturun
                    Control[] controls = this.Controls.Find(textBoxName, true);

                    if (controls.Length > 0 && controls[0] is TextBox textBox) // TextBox bulundu mu ve TextBox mı kontrol ediliyor
                    {
                        textBox.Enabled = true; // TextBox'ı etkinleştirin
                        //textBox.BackColor = Color.WhiteSmoke; // TextBox'ın arka plan rengini ayarlayın
                    }
                }


            }

        }

        int kullaniciSayisi;
        int sutunSayisi = 12;
        private bool ilkTiklama = true;

        private void Edit(int start, int stop, int number)
        {
            for (int i = start; i < stop; i++)
            {
                for (int j = 0; j < number; j++)
                {
                    int currentIndex = 1 + i + (j * sutunSayisi);
                    string textBoxName = "textBoxFeasa" + currentIndex;
                    Control[] controls = this.Controls.Find(textBoxName, true);

                    if (controls.Length > 0 && controls[0] is TextBox textBox)
                    {
                        textBox.Enabled = true; // Etkinleştir
                        textBox.BackColor = Color.White; // İsteğe bağlı olarak arka plan rengini değiştirebilirsiniz
                    }
                }
            }
        }

        private void reEdit(int start, int stop, int number)
        {
            for (int i = start; i < stop; i++)
            {
                for (int j = 0; j < number; j++)
                {
                    int currentIndex = 1 + i + (j * sutunSayisi);
                    string textBoxName = "textBoxFeasa" + currentIndex;
                    Control[] controls = this.Controls.Find(textBoxName, true);

                    if (controls.Length > 0 && controls[0] is TextBox textBox)
                    {
                        textBox.Clear();
                        textBox.Enabled = true; // Etkinleştir
                        textBox.BackColor = Color.FromArgb(31,31,31); // İsteğe bağlı olarak arka plan rengini değiştirebilirsiniz
                    }
                }
            }
        }

        private void minMaxUpdate(int start, int end)
        {
            for (int i = start; i <= end; i++)
            {
                string textBoxName = "txtBxMinMax" + i;

                Control[] controls = this.Controls.Find(textBoxName, true);
                if (controls.Length > 0 && controls[0] is TextBox textBox)
                {
                    textBox.Enabled = true;
                    textBox.BackColor = Color.White;
                }
            }
        }

        private void disableTextBox(int start, int end)
        {
            for (int i = start; i <= end; i++)
            {
                string textBoxName = "txtBxMinMax" + i;

                Control[] controls = this.Controls.Find(textBoxName, true);
                if (controls.Length > 0 && controls[0] is TextBox textBox)
                {
                    textBox.Enabled = false;
                    textBox.BackColor = Color.FromArgb(31, 31, 31);
                }
            }
        }

        private void btnRGB_Click(object sender, EventArgs e)
        {
            if (ilkTiklama)
            {
                minMaxUpdate(1, 6);
                kullaniciSayisi = Convert.ToInt32(txtBxLedSayi.Text);

                btnRGB.BackColor = Color.LightCoral;

                for (int i = 1; i <= 6; i++)
                {
                    TextBox textBox = Controls.Find("txtBxMinMax" + i, true)[0] as TextBox;
                    if (textBox != null)
                    {
                        textBox.Text = inifile.Read("TextBox" + i, "TextBoxValues");
                    }
                }

                ilkTiklama = false;

                // Olayı tetikleyin
                RgbButtonStateChanged?.Invoke(this, new RgbButtonEventArgs(true));

                Edit(0, 3, kullaniciSayisi);

                continueRGB = true;

                //bacgroundWorker.Baslat();

            }
            else
            {
                btnRGB.BackColor = Color.FromArgb(31, 31, 31);

                reEdit(0, 3, kullaniciSayisi);

                ilkTiklama = true;
                for (int i = 1; i <= 6; i++)
                {
                    TextBox textBox = Controls.Find("txtBxMinMax" + i, true)[0] as TextBox;
                    if (textBox != null)
                    {
                        textBox.Clear();
                    }
                }
                disableTextBox(1, 6);

                continueRGB = false;

                // Olayı tetikleyin
                RgbButtonStateChanged?.Invoke(this, new RgbButtonEventArgs(false));
            }
        }

        private void RGBContinue()
        {
            // UI iş parçacığında çalışması gereken kodu UI iş parçacığına döndür
            if (InvokeRequired)
            {
                this.Invoke(new Action(RGBContinue));
                return;
            }

            portNumber = Convert.ToInt32(comboBox1.Items[comboBox1.SelectedIndex]);
            getRGB rgbAnalyzer = new getRGB(portNumber);

            if (rgbAnalyzer.Capture())
            {
                var rgbData = rgbAnalyzer.ReadRGBI(kullaniciSayisi);

                // Min ve Max değerleri TextBox'lardan al
                if (int.TryParse(txtBxMinMax1.Text, out int minValue) &&
                    int.TryParse(txtBxMinMax2.Text, out int maxValue))
                {
                    if (rgbData != null)
                    {
                        for (int i = 0; i < rgbData.Count && i < kullaniciSayisi; i++)
                        {
                            int rTextBoxNumber = 1 + (i * 12);
                            int gTextBoxNumber = 2 + (i * 12);
                            int bTextBoxNumber = 3 + (i * 12);
                            //int iTextBoxNumber = 4 + (i * 12);

                            var rTextBox = Controls.Find("textBoxFeasa" + rTextBoxNumber, true).FirstOrDefault() as TextBox;
                            var gTextBox = Controls.Find("textBoxFeasa" + gTextBoxNumber, true).FirstOrDefault() as TextBox;
                            var bTextBox = Controls.Find("textBoxFeasa" + bTextBoxNumber, true).FirstOrDefault() as TextBox;

                            if (rTextBox != null && gTextBox != null && bTextBox != null)
                            {
                                // Veriyi ayrıştır ve ilgili kısmı al
                                var dataParts = rgbData[i].Split(' ');

                                // RGB değerlerini al
                                if (int.TryParse(dataParts[0], out int rValue) && int.TryParse(dataParts[1], out int gValue) && int.TryParse(dataParts[2], out int bValue))
                                {
                                    // RGB değerlerinin minValue ile maxValue arasında olup olmadığını kontrol et
                                    if (rValue < minValue || rValue > maxValue || gValue < minValue || gValue > maxValue || bValue < minValue || bValue > maxValue)
                                    {
                                        MessageBox.Show($"RGB values out of range for user {i + 1}. Please check the input data.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    }
                                    else
                                    {
                                        rTextBox.Text = rValue.ToString(); // Red
                                        gTextBox.Text = gValue.ToString(); // Green
                                        bTextBox.Text = bValue.ToString(); // Blue
                                    }
                                }
                                else
                                {
                                    //MessageBox.Show($"Invalid RGB data format for user {i + 1}. Please check the input data.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                }
                            }
                        }
                        ///MessageBox.Show("Capture and read successful!");
                    }
                    else
                    {
                        //MessageBox.Show("Error reading data from the LED Analyser!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                }

            }
            else
            {
                //MessageBox.Show("Error capturing data from the LED Analyser!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

        }

        private void btnCIExy_Click(object sender, EventArgs e)
        {
            if (ilkTiklama)
            {
                minMaxUpdate(7, 10);
                kullaniciSayisi = Convert.ToInt32(txtBxLedSayi.Text);
                Edit(3, 5, kullaniciSayisi);

                btnCIExy.BackColor = Color.LightCoral;

                for (int i = 7; i <= 10; i++)
                {
                    TextBox textBox = Controls.Find("txtBxMinMax" + i, true)[0] as TextBox;
                    if (textBox != null)
                    {
                        textBox.Text = inifile.Read("TextBox" + i, "TextBoxValues");
                    }
                }

                ilkTiklama = false;

                continueCIEXY = true;

                //bacgroundWorker.Baslat();

                CIExyButtonStateChanged?.Invoke(this, new CIExyButtonEventArgs(true));


            }
            else
            {
                btnCIExy.BackColor = Color.FromArgb(31, 31, 31);
                reEdit(3, 5, kullaniciSayisi);

                ilkTiklama = true;
                for (int i = 7; i <= 10; i++)
                {
                    TextBox textBox = Controls.Find("txtBxMinMax" + i, true)[0] as TextBox;
                    if (textBox != null)
                    {
                        textBox.Clear();
                    }
                }
                disableTextBox(7, 10);

                continueCIEXY = false;

                //bacgroundWorker.Durdur();

                CIExyButtonStateChanged?.Invoke(this, new CIExyButtonEventArgs(false));
            }            
        }

        private void CIEXYContinue()
        {
            // UI iş parçacığında çalışması gereken kodu UI iş parçacığına döndür
            if (InvokeRequired)
            {
                this.Invoke(new Action(CIEXYContinue));
                return;
            }

            portNumber = Convert.ToInt32(comboBox1.Items[comboBox1.SelectedIndex]);
            getXY xyAnalyzer = new getXY(portNumber);

            if (xyAnalyzer.Capture())
            {
                var xyData = xyAnalyzer.ReadXY(kullaniciSayisi);

                if (xyData != null)
                {
                    for (int i = 0; i < xyData.Count && i < kullaniciSayisi; i++)
                    {
                        int xTextBoxNumber = 4 + (i * 12);
                        int yTextBoxNumber = 5 + (i * 12);


                        var xTextBox = Controls.Find("textBoxFeasa" + xTextBoxNumber, true).FirstOrDefault() as TextBox;
                        var yTextBox = Controls.Find("textBoxFeasa" + yTextBoxNumber, true).FirstOrDefault() as TextBox;

                        //var iTextBox = Controls.Find("textBoxFeasa" + iTextBoxNumber, true).FirstOrDefault() as TextBox;

                        if (xTextBox != null && yTextBox != null)
                        {
                            // Veriyi ayrıştır ve ilgili kısmı al
                            var dataParts = xyData[i].Split(' ');

                            xTextBox.Text = dataParts[0]; // Red
                            yTextBox.Text = dataParts[1]; // Green

                        }
                    }
                    //MessageBox.Show("Capture and read successful!");
                }
                else
                {
                    //MessageBox.Show("Error reading data from the LED Analyser!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
            else
            {
                //MessageBox.Show("Error capturing data from the LED Analyser!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void btnCIEuy_Click(object sender, EventArgs e)
        {
            if (ilkTiklama)
            {
                minMaxUpdate(11, 14);
                kullaniciSayisi = Convert.ToInt32(txtBxLedSayi.Text);
                Edit(5, 7, kullaniciSayisi);

                btnCIEuy.BackColor = Color.LightCoral;

                for (int i = 11; i <= 14; i++)
                {
                    TextBox textBox = Controls.Find("txtBxMinMax" + i, true)[0] as TextBox;
                    if (textBox != null)
                    {
                        textBox.Text = inifile.Read("TextBox" + i, "TextBoxValues");
                    }
                }

                ilkTiklama = false;

                continueCIEUV = true;

                CIEuvButtonStateChanged?.Invoke(this, new CIEuvButtonEventArgs(true));


            }
            else
            {
                btnCIEuy.BackColor = Color.FromArgb(31, 31, 31);
                reEdit(5, 7, kullaniciSayisi);

                ilkTiklama = true;

                for (int i = 11; i <= 14; i++)
                {
                    TextBox textBox = Controls.Find("txtBxMinMax" + i, true)[0] as TextBox;
                    if (textBox != null)
                    {
                        textBox.Clear();
                    }
                }
                disableTextBox(11, 14);

                continueCIEUV = false;

                CIEuvButtonStateChanged?.Invoke(this, new CIEuvButtonEventArgs(false));
            }
            
        }

        private void CIEUVContinue()
        {
            // UI iş parçacığında çalışması gereken kodu UI iş parçacığına döndür
            if (InvokeRequired)
            {
                this.Invoke(new Action(CIEUVContinue));
                return;
            }

            portNumber = Convert.ToInt32(comboBox1.Items[comboBox1.SelectedIndex]);
            getUV uvAnalyzer = new getUV(portNumber);

            string[] cıeuvs;

            List<int> hatalıIndeksler = new List<int>();

            if (uvAnalyzer.Capture())
            {
                var uvData = uvAnalyzer.ReadUV(kullaniciSayisi);

                if (uvData != null)
                {
                    for (int i = 0; i < uvData.Count && i < kullaniciSayisi; i++)
                    {
                        int uTextBoxNumber = 6 + (i * 12);
                        int vTextBoxNumber = 7 + (i * 12);


                        var uTextBox = Controls.Find("textBoxFeasa" + uTextBoxNumber, true).FirstOrDefault() as TextBox;
                        var vTextBox = Controls.Find("textBoxFeasa" + vTextBoxNumber, true).FirstOrDefault() as TextBox;

                        //var iTextBox = Controls.Find("textBoxFeasa" + iTextBoxNumber, true).FirstOrDefault() as TextBox;

                        if (uTextBox != null && vTextBox != null)
                        {
                            // Veriyi ayrıştır ve ilgili kısmı al
                            var dataParts = uvData[i].Split(' ');

                            uTextBox.Text = dataParts[0]; // Red
                            vTextBox.Text = dataParts[1]; // Green

                        }
                    }
                    //MessageBox.Show("Capture and read successful!");
                }
                else
                {
                    //MessageBox.Show("Error reading data from the LED Analyser!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
            else
            {
                //MessageBox.Show("Error capturing data from the LED Analyser!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void btnSaturation_Click(object sender, EventArgs e)
        {
            if (ilkTiklama)
            {
                minMaxUpdate(15, 16);
                kullaniciSayisi = Convert.ToInt32(txtBxLedSayi.Text);
                Edit(7, 8, kullaniciSayisi);

                btnSaturation.BackColor = Color.LightCoral;

                for (int i = 15; i <= 16; i++)
                {
                    TextBox textBox = Controls.Find("txtBxMinMax" + i, true)[0] as TextBox;
                    if (textBox != null)
                    {
                        textBox.Text = inifile.Read("TextBox" + i, "TextBoxValues");
                    }
                }

                ilkTiklama = false;
                continueSAT = true;

                SatButtonStateChanged?.Invoke(this, new SatButtonEventArgs(true, hatalıIndeksler));
            }
            else
            {
                btnSaturation.BackColor = Color.FromArgb(31, 31, 31);
                reEdit(7, 8, kullaniciSayisi);

                ilkTiklama = true;

                for (int i = 15; i <= 16; i++)
                {
                    TextBox textBox = Controls.Find("txtBxMinMax" + i, true)[0] as TextBox;
                    if (textBox != null)
                    {
                        textBox.Clear();
                    }
                }
                disableTextBox(15, 16);

                continueSAT = false;

                SatButtonStateChanged?.Invoke(this, new SatButtonEventArgs(false, hatalıIndeksler));
            }            
        }

        private void SATContinue()
        {
            // UI iş parçacığında çalışması gereken kodu UI iş parçacığına döndür
            if (InvokeRequired)
            {
                this.Invoke(new Action(SATContinue));
                return;
            }

            selectedPort = Convert.ToInt32(comboBox1.Items[comboBox1.SelectedIndex]);
            kullaniciSayisi = Convert.ToInt32(txtBxLedSayi.Text);

            getHSI sReader = new getHSI();
            string[] saturations;

            List<int> hatalıIndeksler = new List<int>();

            if (sReader.CaptureAndRead(selectedPort, kullaniciSayisi, out _, out saturations, out _))
            {
                if (int.TryParse(txtBxMinMax15.Text, out int minValue) &&
                    int.TryParse(txtBxMinMax16.Text, out int maxValue))
                {
                    // Saturation değerlerini kullan
                    for (int i = 0; i < saturations.Length; i++)
                    {
                        int textBoxIndex = 8 + i * 12; // Başlangıç indeksi hesapla
                        TextBox textBox = Controls.Find("textBoxFeasa" + textBoxIndex.ToString(), true)[0] as TextBox;

                        if (int.TryParse(saturations[i], out int saturation))
                        {
                            if (saturation < minValue || saturation > maxValue)
                            {
                                hatalıIndeksler.Add(i + 1); // Hatalı indeksi listeye ekle
                            }
                            else
                            {
                                hatalıIndeksler.Add(-1);
                                textBox.Text = saturations[i];
                            }
                        }
                        else
                        {
                            MessageBox.Show($"Geçersiz dalga boyu formatı kullanıcı {i + 1} için. Lütfen girdi verilerini kontrol edin.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }

                    // Hatalı indeksler varsa olayı tetikle
                    if (hatalıIndeksler.Count > 0)
                    {
                        SatButtonStateChanged?.Invoke(this, new SatButtonEventArgs(true, hatalıIndeksler));
                    }
                }

            }
        }

        private void btnHue_Click(object sender, EventArgs e)
        {
            if (ilkTiklama)
            {
                minMaxUpdate(17, 18);
                kullaniciSayisi = Convert.ToInt32(txtBxLedSayi.Text);
                Edit(8, 9, kullaniciSayisi);

                btnHue.BackColor = Color.LightCoral;

                for (int i = 17; i <= 18; i++)
                {
                    TextBox textBox = Controls.Find("txtBxMinMax" + i, true)[0] as TextBox;
                    if (textBox != null)
                    {
                        textBox.Text = inifile.Read("TextBox" + i, "TextBoxValues");
                    }
                }

                ilkTiklama = false;
                continueHue = true;

                HueButtonStateChanged?.Invoke(this, new HueButtonEventArgs(true, hatalıIndeksler));

            }
            else
            {
                btnHue.BackColor = Color.FromArgb(31, 31, 31);
                reEdit(8, 9, kullaniciSayisi);

                ilkTiklama = true;
                for (int i = 17; i <= 18; i++)
                {
                    TextBox textBox = Controls.Find("txtBxMinMax" + i, true)[0] as TextBox;
                    if (textBox != null)
                    {
                        textBox.Clear();
                    }
                }
                disableTextBox(17, 18);

                continueHue = false;

                HueButtonStateChanged?.Invoke(this, new HueButtonEventArgs(false, hatalıIndeksler));
            }
            
        }

        private void HueContinue()
        {
            // UI iş parçacığında çalışması gereken kodu UI iş parçacığına döndür
            if (InvokeRequired)
            {
                this.Invoke(new Action(HueContinue));
                return;
            }

            selectedPort = Convert.ToInt32(comboBox1.Items[comboBox1.SelectedIndex]);
            kullaniciSayisi = Convert.ToInt32(txtBxLedSayi.Text);

            getHSI hReader = new getHSI();
            string[] hues;

            List<int> hatalıIndeksler = new List<int>();

            if (hReader.CaptureAndRead(selectedPort, kullaniciSayisi, out hues, out _, out _))
            {
                if (int.TryParse(txtBxMinMax17.Text, out int minValue) &&
                    int.TryParse(txtBxMinMax18.Text, out int maxValue))
                {
                    // Hue değerlerini kullan
                    for (int i = 0; i < hues.Length; i++)
                    {
                        int textBoxIndex = 9 + i * 12; // Başlangıç indeksi hesapla
                        TextBox textBox = Controls.Find("textBoxFeasa" + textBoxIndex.ToString(), true)[0] as TextBox;

                        if (double.TryParse(hues[i], out double hue))
                        {
                            if (hue < minValue || hue > maxValue)
                            {
                                hatalıIndeksler.Add(i + 1); // Hatalı indeksi listeye ekle
                            }
                            else
                            {
                                hatalıIndeksler.Add(-1);
                                textBox.Text = hues[i];
                            }
                        }
                        else
                        {
                            MessageBox.Show($"Geçersiz dalga boyu formatı kullanıcı {i + 1} için. Lütfen girdi verilerini kontrol edin.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }

                    }

                    // Hatalı indeksler varsa olayı tetikle
                    if (hatalıIndeksler.Count > 0)
                    {
                        HueButtonStateChanged?.Invoke(this, new HueButtonEventArgs(true, hatalıIndeksler));
                    }
                }


            }
        }

        private void btnIntensity_Click(object sender, EventArgs e)
        {
            if (ilkTiklama)
            {
                minMaxUpdate(19, 20);
                kullaniciSayisi = Convert.ToInt32(txtBxLedSayi.Text);
                Edit(9, 10, kullaniciSayisi);

                btnIntensity.BackColor = Color.LightCoral;

                for (int i = 19; i <= 20; i++)
                {
                    TextBox textBox = Controls.Find("txtBxMinMax" + i, true)[0] as TextBox;
                    if (textBox != null)
                    {
                        textBox.Text = inifile.Read("TextBox" + i, "TextBoxValues");
                    }
                }

                ilkTiklama = false;

                continueIntensity = true;

                IntenstyButtonStateChanged?.Invoke(this, new IntenstyButtonEventArgs(true, hatalıIndeksler));


            }
            else
            {
                btnIntensity.BackColor = Color.FromArgb(31, 31, 31);
                reEdit(9, 10, kullaniciSayisi);

                ilkTiklama = true;
                for (int i = 19; i <= 20; i++)
                {
                    TextBox textBox = Controls.Find("txtBxMinMax" + i, true)[0] as TextBox;
                    if (textBox != null)
                    {
                        textBox.Clear();
                    }
                }
                disableTextBox(19, 20);
                continueIntensity = false;
                IntenstyButtonStateChanged?.Invoke(this, new IntenstyButtonEventArgs(false, hatalıIndeksler));
            }
            
        }

        private void IntensityContinue()
        {
            // UI iş parçacığında çalışması gereken kodu UI iş parçacığına döndür
            if (InvokeRequired)
            {
                this.Invoke(new Action(IntensityContinue));
                return;
            }

            selectedPort = Convert.ToInt32(comboBox1.Items[comboBox1.SelectedIndex]);
            kullaniciSayisi = Convert.ToInt32(txtBxLedSayi.Text);

            getHSI iReader = new getHSI();
            string[] intensities;

            List<int> hatalıIndeksler = new List<int>();

            if (iReader.CaptureAndRead(selectedPort, kullaniciSayisi, out _, out _, out intensities))
            {
                if (int.TryParse(txtBxMinMax19.Text, out int minValue) &&
                    int.TryParse(txtBxMinMax20.Text, out int maxValue))
                {
                    // Intensity değerlerini kullan
                    for (int i = 0; i < intensities.Length; i++)
                    {
                        int textBoxIndex = 10 + i * 12; // Başlangıç indeksi hesapla
                        TextBox textBox = Controls.Find("textBoxFeasa" + textBoxIndex.ToString(), true)[0] as TextBox;

                        if (int.TryParse(intensities[i], out int intensitie))
                        {
                            if (intensitie < minValue || intensitie > maxValue)
                            {
                                hatalıIndeksler.Add(i + 1); // Hatalı indeksi listeye ekle
                            }
                            else
                            {
                                hatalıIndeksler.Add(-1);
                                textBox.Text = intensities[i];
                            }
                        }
                        else
                        {
                            MessageBox.Show($"Geçersiz dalga boyu formatı kullanıcı {i + 1} için. Lütfen girdi verilerini kontrol edin.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }

                    }

                    // Hatalı indeksler varsa olayı tetikle
                    if (hatalıIndeksler.Count > 0)
                    {
                        IntenstyButtonStateChanged?.Invoke(this, new IntenstyButtonEventArgs(true, hatalıIndeksler));
                    }
                }

            }
        }

        private void btnCCT_Click(object sender, EventArgs e)
        {
            

            if (ilkTiklama)
            {
                minMaxUpdate(21, 22);              
                Edit(10, 11, kullaniciSayisi);
                btnCCT.BackColor = Color.LightCoral;
                for (int i = 21; i <= 22; i++)
                {
                    TextBox textBox = Controls.Find("txtBxMinMax" + i, true)[0] as TextBox;
                    if (textBox != null)
                    {
                        textBox.Text = inifile.Read("TextBox" + i, "TextBoxValues");
                    }
                }
                ilkTiklama = false;

                continueCCT = true;

                CCTButtonStateChanged?.Invoke(this, new CCTButtonEventArgs(true, hatalıIndeksler));
            }
            else
            {
                btnCCT.BackColor = Color.FromArgb(31, 31, 31);
                reEdit(10, 11, kullaniciSayisi);

                ilkTiklama = true;
                for (int i = 21; i <= 22; i++)
                {
                    TextBox textBox = Controls.Find("txtBxMinMax" + i, true)[0] as TextBox;
                    if (textBox != null)
                    {
                        textBox.Clear();
                    }
                }
                disableTextBox(21, 22);

                continueCCT = false;

                CCTButtonStateChanged?.Invoke(this, new CCTButtonEventArgs(false, hatalıIndeksler));
            }
        }

        private void CCTContinue()
        {
            // UI iş parçacığında çalışması gereken kodu UI iş parçacığına döndür
            if (InvokeRequired)
            {
                this.Invoke(new Action(CCTContinue));
                return;
            }

            selectedPort = Convert.ToInt32(comboBox1.Items[comboBox1.SelectedIndex]);
            kullaniciSayisi = Convert.ToInt32(txtBxLedSayi.Text);

            getCCT cctReader = new getCCT();
            string[] ccts;

            List<int> hatalıIndeksler = new List<int>();

            if (cctReader.CaptureAndRead(selectedPort, kullaniciSayisi, out ccts))
            {
                if (int.TryParse(txtBxMinMax21.Text, out int minValue) &&
                    int.TryParse(txtBxMinMax22.Text, out int maxValue))
                {
                    // CCT değerlerini kullan
                    for (int i = 0; i < ccts.Length; i++)
                    {
                        int textBoxIndex = 11 + i * 12; // Başlangıç indeksi hesapla
                        TextBox textBox = Controls.Find("textBoxFeasa" + textBoxIndex.ToString(), true)[0] as TextBox;

                        if (int.TryParse(ccts[i], out int cct))
                        {
                            if (cct < minValue || cct > maxValue)
                            {
                                hatalıIndeksler.Add(i + 1); // Hatalı indeksi listeye ekle
                            }
                            else
                            {
                                hatalıIndeksler.Add(-1);
                                textBox.Text = ccts[i];
                            }
                        }
                        else
                        {
                            MessageBox.Show($"Geçersiz dalga boyu formatı kullanıcı {i + 1} için. Lütfen girdi verilerini kontrol edin.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
    
                    }

                    // Hatalı indeksler varsa olayı tetikle
                    if (hatalıIndeksler.Count > 0)
                    {
                        CCTButtonStateChanged?.Invoke(this, new CCTButtonEventArgs(true, hatalıIndeksler));
                    }

                }
            }
        }

        private void btnWaveLength_Click(object sender, EventArgs e)
        {
            if (ilkTiklama)
            {
                minMaxUpdate(23, 24);
                kullaniciSayisi = Convert.ToInt32(txtBxLedSayi.Text);
                Edit(11, 12, kullaniciSayisi);

                btnWaveLength.BackColor = Color.LightCoral;

                for (int i = 23; i <= 24; i++)
                {
                    TextBox textBox = Controls.Find("txtBxMinMax" + i, true)[0] as TextBox;
                    if (textBox != null)
                    {
                        textBox.Text = inifile.Read("TextBox" + i, "TextBoxValues");
                    }
                }

                ilkTiklama = false;

                continuewL = true;

                wLButtonStateChanged?.Invoke(this, new wLButtonEventArgs(true, hatalıIndeksler));


            }
            else
            {
                btnWaveLength.BackColor = Color.FromArgb(31, 31, 31);
                reEdit(11, 12, kullaniciSayisi);

                ilkTiklama = true;
                for (int i = 23; i <= 24; i++)
                {
                    TextBox textBox = Controls.Find("txtBxMinMax" + i, true)[0] as TextBox;
                    if (textBox != null)
                    {
                        textBox.Clear();
                    }
                }
                disableTextBox(23, 24);

                continuewL = false;

                wLButtonStateChanged?.Invoke(this, new wLButtonEventArgs(false, hatalıIndeksler));
            }

        }

        private void wLContinue()
        {
            // UI iş parçacığında çalışması gereken kodu UI iş parçacığına döndür
            if (InvokeRequired)
            {
                 this.Invoke(new Action(wLContinue));
                return;
            }

            selectedPort = Convert.ToInt32(comboBox1.Items[comboBox1.SelectedIndex]);
            kullaniciSayisi = Convert.ToInt32(txtBxLedSayi.Text);

            getWLength wLReader = new getWLength();
            string[] wavelengths;

            List<int> hatalıIndeksler = new List<int>();

            if (wLReader.CaptureAndRead(selectedPort, kullaniciSayisi, out wavelengths))
            {
                // Min ve Max değerlerini TextBox'lardan al
                if (int.TryParse(txtBxMinMax23.Text, out int minValue) &&
                    int.TryParse(txtBxMinMax24.Text, out int maxValue))
                {
                    // Dalga boyu değerlerini kontrol et ve TextBox'lara yaz
                    for (int i = 0; i < wavelengths.Length; i++)
                    {
                        int textBoxIndex = 12 + i * 12; // Başlangıç indeksi hesapla
                        TextBox textBox = Controls.Find("textBoxFeasa" + textBoxIndex.ToString(), true)[0] as TextBox;

                        if (int.TryParse(wavelengths[i], out int wavelength))
                        {
                            if (wavelength < minValue || wavelength > maxValue)
                            {
                                hatalıIndeksler.Add(i + 1); // Hatalı indeksi listeye ekle
                            }
                            else
                            {
                                hatalıIndeksler.Add(-1);
                                textBox.Text = wavelengths[i];
                            }
                        }
                        else
                        {
                            MessageBox.Show($"Geçersiz dalga boyu formatı kullanıcı {i + 1} için. Lütfen girdi verilerini kontrol edin.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }

                    // Hatalı indeksler varsa olayı tetikle
                    if (hatalıIndeksler.Count > 0)
                    {
                        wLButtonStateChanged?.Invoke(this, new wLButtonEventArgs(true, hatalıIndeksler));
                    }
                }
                else
                {
                    MessageBox.Show("Min veya Max değerleri geçerli değil. Lütfen girdi verilerini kontrol edin.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                MessageBox.Show("Dalga boyu verileri okunamadı. Lütfen bağlantıyı kontrol edin.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        public class RgbButtonEventArgs : EventArgs
        {
            public bool IsActive { get; }

            public RgbButtonEventArgs(bool isActive)
            {
                IsActive = isActive;
            }
        }

        public class CIExyButtonEventArgs : EventArgs
        {
            public bool IsActive { get; }

            public CIExyButtonEventArgs(bool isActive)
            {
                IsActive = isActive;
            }
        }

        public class CIEuvButtonEventArgs : EventArgs
        {
            public bool IsActive { get; }

            public CIEuvButtonEventArgs(bool isActive)
            {
                IsActive = isActive;
            }
        }

        public class SatButtonEventArgs : EventArgs
        {
            public bool IsActive { get; }

            public List<int> HatalıIndexler { get; } // Hatalı indeksleri tutan özellik
            public SatButtonEventArgs(bool isActive, List<int> hatalıIndexler)
            {
                IsActive = isActive;
                HatalıIndexler = hatalıIndexler;
            }
        }

        public class HueButtonEventArgs : EventArgs
        {
            public bool IsActive { get; }

            public List<int> HatalıIndexler { get; } // Hatalı indeksleri tutan özellik

            public HueButtonEventArgs(bool isActive, List<int> hatalıIndexler)
            {
                IsActive = isActive;

                HatalıIndexler = hatalıIndexler;
            }
        }

        public class IntenstyButtonEventArgs : EventArgs
        {
            public bool IsActive { get; }

            public List<int> HatalıIndexler { get; } // Hatalı indeksleri tutan özellik

            public IntenstyButtonEventArgs(bool isActive, List<int> hatalıIndexler)
            {
                IsActive = isActive;

                HatalıIndexler = hatalıIndexler;
            }
        }

        public class CCTButtonEventArgs : EventArgs
        {
            public bool IsActive { get; }
            public List<int> HatalıIndexler { get; } // Hatalı indeksleri tutan özellik

            public CCTButtonEventArgs(bool isActive, List<int> hatalıIndexler)
            {
                IsActive = isActive;
                HatalıIndexler = hatalıIndexler;
            }
        }

        public class wLButtonEventArgs : EventArgs
        {
            public bool IsActive { get; }
            public List<int> HatalıIndexler { get; } // Hatalı indeksleri tutan özellik

            public wLButtonEventArgs(bool isActive, List<int> hatalıIndexler)
            {
                IsActive = isActive;
                HatalıIndexler = hatalıIndexler;
            }
        }

        private void btnAnasayfa_Click(object sender, EventArgs e)
        {
            Anasayfa anasayfa = Anasayfa.GetInstance(); // Mevcut form örneğini al
            anasayfa.LedButtonEdit();

            anasayfa.ShowForm(); // UrunAyr formunu göster

            //HideForm();
        }

        private void exit_Click(object sender, EventArgs e)
        {
            //bacgroundWorker.Durdur();

            Application.Exit();
        }

        private void ResetButtonColor(int butonNumarasi)
        {
            switch (butonNumarasi)
            {
                case 1:
                    btnRGB.BackColor = Color.FromArgb(31, 31, 31);
                    txtBxMinMax1.BackColor = Color.FromArgb(31, 31, 31);
                    txtBxMinMax2.BackColor = Color.FromArgb(31, 31, 31);
                    txtBxMinMax3.BackColor = Color.FromArgb(31, 31, 31);
                    txtBxMinMax4.BackColor = Color.FromArgb(31, 31, 31);
                    txtBxMinMax5.BackColor = Color.FromArgb(31, 31, 31);
                    txtBxMinMax6.BackColor = Color.FromArgb(31, 31, 31);
                    break;
                case 2:
                    btnCIExy.BackColor = Color.FromArgb(31, 31, 31);
                    break; 
                case 3:
                    btnCIEuy.BackColor = Color.FromArgb(31, 31, 31);
                    break;
                case 4:
                    btnSaturation.BackColor = Color.FromArgb(31, 31, 31);
                    break;
                case 5:
                    btnHue.BackColor = Color.FromArgb(31, 31, 31);
                    break;
                case 6:
                    btnIntensity.BackColor = Color.FromArgb(31, 31, 31);
                    break;
                case 7:
                    btnCCT.BackColor = Color.FromArgb(31, 31, 31);
                    break;
                case 8:
                    btnWaveLength.BackColor = Color.FromArgb(31, 31, 31);
                    break;
            }
        }

        private void button59_Click(object sender, EventArgs e)
        {
            KartAyarlar kartAyarlar = KartAyarlar.GetInstance(); // Mevcut form örneğini al
            kartAyarlar.ShowForm(); // UrunAyr formunu göster

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

        private void FeasaAyarlar_Load(object sender, EventArgs e)
        {
            int i;

            /*This command enumerates the existing ports to find out
            what are the serial ports existing on your computer and
            the devices connected to them. You need to execute this
            command everytime you plug or unplug a Feasa Device,
            once your application is running */
            //FeasaCom.EnumPorts();

            //List available ports
            for (i = 1; i <= 100; i++)
            {
                if (FeasaCom.IsPortAvailable(i) == 1) comboBox1.Items.Add(i.ToString());
            }
            //Select the first port of the list
            if (comboBox1.Items.Count > 0) comboBox1.SelectedIndex = 0;

        }

        private void SaveTextBoxValues()
        {
            for (int i = 1; i <= 24; i++)
            {
                TextBox textBox = Controls.Find("txtBxMinMax" + i, true)[0] as TextBox;
                if (textBox != null)
                {
                    inifile.Write("TextBox" + i, textBox.Text, "TextBoxValues");
                }
            }
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            SaveTextBoxValues();
            MessageBox.Show("Values saved to INI file.");
        }

        private void FeasaAyarlar_FormClosing(object sender, FormClosingEventArgs e)
        {
            //bacgroundWorker.Durdur();
        }

        private void button1_Click(object sender, EventArgs e)
        {
             if (continueRGB == true)
                 RGBContinue();
             if (continueCIEXY == true)
                 CIEXYContinue();
             if (continueCIEUV == true)
                 CIEUVContinue();
             if (continueSAT == true)
                 SATContinue();
             if (continueHue == true)
                 HueContinue();
             if (continueIntensity == true)
                 IntensityContinue();
             if (continueCCT == true)
                 CCTContinue();
             if (continuewL == true)
                 wLContinue();
        }
    }
}
