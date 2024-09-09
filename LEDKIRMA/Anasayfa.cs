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
{    public partial class Anasayfa : Form
    {
        //public Button TargetButton => btnTarget;

        private static Anasayfa anasayfaInstance;

        private FeasaAyarlar feasaAyarlar;

        int ledSayi;
        private bool ilkTiklama = true;
        public Anasayfa()
        {
            InitializeComponent();

            // Eğer daha önce bir Anasayfa örneği oluşturulmadıysa, bu formu anasayfaInstance değişkenine ata
            if (anasayfaInstance == null || anasayfaInstance.IsDisposed)
            {
                anasayfaInstance = this;
            }

            feasaAyarlar = FeasaAyarlar.GetInstance();
            feasaAyarlar.RgbButtonStateChanged += OnRgbButtonStateChanged;
            feasaAyarlar.CIExyButtonStateChanged += OnCIExyButtonStateChanged;
            feasaAyarlar.CIEuvButtonStateChanged += OnCIEuvButtonStateChanged;
            feasaAyarlar.SatButtonStateChanged += OnSatButtonStateChanged;
            feasaAyarlar.HueButtonStateChanged += OnHueButtonStateChanged;
            feasaAyarlar.IntenstyButtonStateChanged += OnIntenstyButtonStateChanged;
            feasaAyarlar.CCTButtonStateChanged += OnCCTButtonStateChanged;
            feasaAyarlar.wLButtonStateChanged += OnwLButtonStateChanged;

        }

        public static Anasayfa GetInstance()
        {
            // Eğer form henüz oluşturulmadıysa veya kapatıldıysa, yeni bir örneği oluşturun
            if (anasayfaInstance == null || anasayfaInstance.IsDisposed)
            {
                anasayfaInstance = new Anasayfa();
            }

            return anasayfaInstance;
        }

        private void Anasayfa_Load(object sender, EventArgs e)
        {

        }

        public void LedButtonEdit()
        {
            LedButtonClean();
            ledSayi = Convert.ToInt32(FeasaAyarlar.LedSayi);

            for (int i = 1; i <= ledSayi; i++)
            {
                string buttonName = "btnLed" + i;
                Control[] controls = this.Controls.Find(buttonName, true);

                if (controls.Length > 0 && controls[0] is Button)
                {
                    Button btn = (Button)controls[0];
                    btn.BackColor = Color.Snow;
                }
;
            }
        }

        public void LedButtonClean()
        {
            for (int i = 1; i <= 15; i++)
            {
                string buttonName = "btnLed" + i;
                Control[] controls = this.Controls.Find(buttonName, true);

                if (controls.Length > 0 && controls[0] is Button)
                {
                    Button btn = (Button)controls[0];
                    btn.BackColor = Color.FromArgb(31,31,31);
                }
;
            }
        }

        public void LedButtonDurumClean()
        {
            for (int i = 1; i <= 180; i++)
            {
                
                string buttonName = "btnLedDurum" + i;
                Control[] controls = this.Controls.Find(buttonName, true);

                if (controls.Length > 0 && controls[0] is Button)
                {
                    Button btn = (Button)controls[0];
                    btn.BackColor = Color.FromArgb(31, 31, 31);
                }
;
            }
        }

        public void OnRgbButtonStateChanged(object sender, FeasaAyarlar.RgbButtonEventArgs e)
        {
            if (e.IsActive)
            {
                ledSayi = Convert.ToInt32(FeasaAyarlar.LedSayi);
                EditButtons(0, 3, ledSayi);

                ilkTiklama = false;
            }
            else
            {
                reEditButtons(0, 3, ledSayi);

                ilkTiklama = true;
            }

        }

        public void OnCIExyButtonStateChanged(object sender, FeasaAyarlar.CIExyButtonEventArgs e)
        {
            if (e.IsActive)
            {
                ledSayi = Convert.ToInt32(FeasaAyarlar.LedSayi);
                EditButtons(3, 5, ledSayi);

                ilkTiklama = false;
            }
            else
            {
                reEditButtons(3, 5, ledSayi);

                ilkTiklama = true;
            }
        }

        public void OnCIEuvButtonStateChanged(object sender, FeasaAyarlar.CIEuvButtonEventArgs e)
        {
            if (e.IsActive)
            {
                ledSayi = Convert.ToInt32(FeasaAyarlar.LedSayi);
                EditButtons(5, 7, ledSayi);

                ilkTiklama = false;
            }
            else
            {
                reEditButtons(5, 7, ledSayi);

                ilkTiklama = true;
            }
        }

        public void OnSatButtonStateChanged(object sender, FeasaAyarlar.SatButtonEventArgs e)
        {
            if (e.IsActive)
            {
                ledSayi = Convert.ToInt32(FeasaAyarlar.LedSayi);
                EditButtons(7, 8, ledSayi);

                ilkTiklama = false;

                if (e.HatalıIndexler != null && e.HatalıIndexler.Count > 0)
                {
                    // Hatalı indeksler için tüm butonları kırmızı yap
                    foreach (int hatalıIndex in e.HatalıIndexler)
                    {
                        if (hatalıIndex != -1)
                        {
                            UpdateButtonColor((hatalıIndex * 12 - 4), Color.Red);
                        }

                    }
                }
            }
            else
            {
                reEditButtons(7, 8, ledSayi);

                ilkTiklama = true;
            }
        }

        public void OnHueButtonStateChanged(object sender, FeasaAyarlar.HueButtonEventArgs e)
        {
            if (e.IsActive)
            {
                ledSayi = Convert.ToInt32(FeasaAyarlar.LedSayi);
                EditButtons(8, 9, ledSayi);

                ilkTiklama = false;

                if (e.HatalıIndexler != null && e.HatalıIndexler.Count > 0)
                {
                    // Hatalı indeksler için tüm butonları kırmızı yap
                    foreach (int hatalıIndex in e.HatalıIndexler)
                    {
                        if (hatalıIndex != -1)
                        {
                            UpdateButtonColor((hatalıIndex * 12 - 3), Color.Red);
                        }

                    }
                }
            }
            else
            {
                reEditButtons(8, 9, ledSayi);

                ilkTiklama = true;
            }
        }

        public void OnIntenstyButtonStateChanged(object sender, FeasaAyarlar.IntenstyButtonEventArgs e)
        {
            if (e.IsActive)
            {
                ledSayi = Convert.ToInt32(FeasaAyarlar.LedSayi);
                EditButtons(9, 10, ledSayi);

                ilkTiklama = false;

                if (e.HatalıIndexler != null && e.HatalıIndexler.Count > 0)
                {
                    // Hatalı indeksler için tüm butonları kırmızı yap
                    foreach (int hatalıIndex in e.HatalıIndexler)
                    {
                        if (hatalıIndex != -1)
                        {
                            UpdateButtonColor((hatalıIndex * 12 - 2), Color.Red);
                        }

                    }
                }
            }
            else
            {
                reEditButtons(9, 10, ledSayi);

                ilkTiklama = true;
            }
        }

        public void OnCCTButtonStateChanged(object sender, FeasaAyarlar.CCTButtonEventArgs e)
        {
            if (e.IsActive)
            {
                ledSayi = Convert.ToInt32(FeasaAyarlar.LedSayi);
                EditButtons(10, 11, ledSayi);

                ilkTiklama = false;

                if (e.HatalıIndexler != null && e.HatalıIndexler.Count > 0)
                {
                    // Hatalı indeksler için tüm butonları kırmızı yap
                    foreach (int hatalıIndex in e.HatalıIndexler)
                    {
                        if (hatalıIndex != -1)
                        {
                            UpdateButtonColor((hatalıIndex * 12 - 1), Color.Red);
                        }

                    }
                }
            }
            else
            {
                reEditButtons(10, 11, ledSayi);

                ilkTiklama = true;
            }
        }

        public void OnwLButtonStateChanged(object sender, FeasaAyarlar.wLButtonEventArgs e)
        {
            if (e.IsActive)
            {
                ledSayi = Convert.ToInt32(FeasaAyarlar.LedSayi);
                EditButtons(11, 12, ledSayi);

                ilkTiklama = false;

                if (e.HatalıIndexler != null && e.HatalıIndexler.Count > 0)
                {
                    // Hatalı indeksler için tüm butonları kırmızı yap
                    foreach (int hatalıIndex in e.HatalıIndexler)
                    {
                        if (hatalıIndex != -1)
                        {
                            UpdateButtonColor(hatalıIndex * 12, Color.Red);
                        }

                    }
                }
            }
            else
            {
                reEditButtons(11, 12, ledSayi);

                ilkTiklama = true;
            }
        }

        int sutunSayisi = 12;
        private void EditButtons(int start, int stop, int number)
        {
            for (int i = start; i < stop; i++)
            {
                for (int j = 0; j < number; j++)
                {
                    int currentIndex = 1 + i + (j * sutunSayisi);
                    string buttonName = "btnLedDurum" + currentIndex;
                    Control[] controls = this.Controls.Find(buttonName, true);

                    if (controls.Length > 0 && controls[0] is Button button)
                    {
                        button.Enabled = true; // Butonu etkinleştir
                        button.BackColor = Color.GreenYellow; // İsteğe bağlı olarak arka plan rengini değiştirebilirsiniz
                    }
                }
            }
        }

        private void reEditButtons(int start, int stop, int number)
        {
            for (int i = start; i < stop; i++)
            {
                for (int j = 0; j < number; j++)
                {
                    int currentIndex = 1 + i + (j * sutunSayisi);
                    string buttonName = "btnLedDurum" + currentIndex;
                    Control[] controls = this.Controls.Find(buttonName, true);

                    if (controls.Length > 0 && controls[0] is Button button)
                    {
                        button.Enabled = true; // Butonu etkinleştir
                        button.BackColor = Color.FromArgb(31, 31, 31); // Arka plan rengini ayarlayın
                    }
                }
            }
        }

        private void UpdateButtonColor(int index, Color color)
        {
            
            string buttonName = "btnLedDurum" + index;
            Control[] controls = this.Controls.Find(buttonName, true);

            if (controls.Length > 0 && controls[0] is Button button)
            {
                button.BackColor = color; // Butonun arka plan rengini güncelle
            }
        }

        private void btnFeasaAyar_Click(object sender, EventArgs e)
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

        private void button59_Click(object sender, EventArgs e)
        {
            KartAyarlar kartAyarlar = KartAyarlar.GetInstance(); // Mevcut form örneğini al
            kartAyarlar.ShowForm(); // UrunAyr formunu göster

            HideForm();
        }
    }
}
