using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Threading;
using System.Windows.Forms;

namespace LEDKIRMA
{
    class BacgroundWorker
    {
        private readonly BackgroundWorker worker;
        private bool running;

        // Her form için özel kontrol işlevlerini temsil eden delegeler
        public Action KartAyarlarıKOntrolleri { get; set; }
        public Action FeasaAyarlarıKOntrolleri { get; set; }

        public BacgroundWorker()
        {
            worker = new BackgroundWorker();
            worker.DoWork += KontrolleriYurut;
            running = false;
        }

        public void Baslat()
        {
            if (!running)
            {
                running = true;
                worker.RunWorkerAsync();
            }
        }

        public void Durdur()
        {
            running = false;
        }


        private void KontrolleriYurut(object sender, DoWorkEventArgs e)
        {
            while (running)
            {
                KartAyarlarıKOntrolleri?.Invoke();

                FeasaAyarlarıKOntrolleri?.Invoke();

                Thread.Sleep(500); // 1 saniye bekle
            }
        }
    }
}
