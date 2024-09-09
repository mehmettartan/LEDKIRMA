using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LEDKIRMA
{
    public class DataStorage
    {
        private static DataStorage instance;
        public string Veri { get; set; }
        public int whichButton { get; set; }
        public int start { get; set; }
        public int stop { get; set; }

        // Private yapıcı metot
        private DataStorage() { }

        // Singleton deseni ile tek bir örneği sağlayan metot
        public static DataStorage Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new DataStorage();
                }
                return instance;
            }
        }
    }
}
