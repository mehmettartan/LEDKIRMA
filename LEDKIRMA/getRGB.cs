using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace LEDKIRMA
{
    class getRGB
    {
        [DllImport("feasacom64.dll")]
        private static extern int FeasaCom_Open(int CommPort, string Baudrate);

        [DllImport("feasacom64.dll")]
        private static extern int FeasaCom_Close(int CommPort);

        [DllImport("feasacom64.dll")]
        private static extern int FeasaCom_Send(int CommPort, string Command, StringBuilder ResponseText);

        [DllImport("feasacom64.dll")]
        private static extern int FeasaCom_IsPortAvailable(int CommPort);

        [DllImport("feasacom64.dll")]
        private static extern int FeasaCom_SetResponseTimeout(uint Timeout);

        public int PortNumber { get; set; }

        public getRGB(int portNumber)
        {
            PortNumber = portNumber;
            FeasaCom_SetResponseTimeout(8000); // 8000ms
        }

        public bool Capture()
        {
            StringBuilder buffer = new StringBuilder(100);
            if (FeasaCom_Open(PortNumber, "115200") == 1)
            {
                int resp = FeasaCom_Send(PortNumber, "CAPTURE", buffer);
                FeasaCom_Close(PortNumber);
                return resp == 1;
            }
            return false;
        }

        public List<string> ReadRGBI(int numFibers)
        {
            List<string> results = new List<string>();
            StringBuilder buffer = new StringBuilder(100);

            if (FeasaCom_Open(PortNumber, "115200") == 1)
            {
                for (int sensor = 1; sensor <= numFibers; sensor++)
                {
                    int resp = FeasaCom_Send(PortNumber, "GETRGBI" + sensor.ToString("00"), buffer);
                    if (resp == -1 || resp == 0)
                    {
                        FeasaCom_Close(PortNumber);
                        return null;
                    }
                    results.Add(buffer.ToString());
                }
                FeasaCom_Close(PortNumber);
            }
            return results;
        }

        public static List<int> GetAvailablePorts()
        {
            List<int> availablePorts = new List<int>();
            for (int i = 1; i <= 100; i++)
            {
                if (FeasaCom_IsPortAvailable(i) == 1)
                {
                    availablePorts.Add(i);
                }
            }
            return availablePorts;
        }
    }
}
