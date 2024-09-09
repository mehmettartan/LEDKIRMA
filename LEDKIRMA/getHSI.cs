using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace LEDKIRMA
{
    class getHSI
    {
        [DllImport("feasacom64.dll")]
        private static extern int FeasaCom_Open(int CommPort, string Baudrate);

        [DllImport("feasacom64.dll")]
        private static extern int FeasaCom_Close(int CommPort);

        [DllImport("feasacom64.dll")]
        private static extern int FeasaCom_Send(int CommPort, string Command, StringBuilder ResponseText);

        [DllImport("feasacom64.dll")]
        private static extern int FeasaCom_SetResponseTimeout(uint Timeout);

        public bool CaptureAndRead(int portNumber, int numFibers, out string[] hues, out string[] saturations, out string[] intensities)
        {
            hues = new string[numFibers];
            saturations = new string[numFibers];
            intensities = new string[numFibers];

            StringBuilder buffer = new StringBuilder(100);

            //Increase maximum timeout to avoid issues due to long capture times
            FeasaCom_SetResponseTimeout(8000); //8000ms

            //Open port
            if (FeasaCom_Open(portNumber, "115200") == 1)
            {
                //No error

                //Send command to capture
                int resp = FeasaCom_Send(portNumber, "CAPTURE", buffer);
                if (resp == -1)
                {
                    MessageBox.Show("Error! unable to send the command!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    FeasaCom_Close(portNumber);
                    return false;
                }
                else if (resp == 0)
                {
                    MessageBox.Show("Timeout detected!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    FeasaCom_Close(portNumber);
                    return false;
                }

                for (int i = 1; i <= numFibers; i++)
                {
                    //Send command to get HSI for each fiber
                    string fiberNumber = i.ToString("D2");
                    string command = "GETHSI" + fiberNumber;
                    resp = FeasaCom_Send(portNumber, command, buffer);
                    if (resp == -1)
                    {
                        MessageBox.Show("Error! unable to send the command!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        FeasaCom_Close(portNumber);
                        return false;
                    }
                    else if (resp == 0)
                    {
                        MessageBox.Show("Timeout detected!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        FeasaCom_Close(portNumber);
                        return false;
                    }

                    //Split the response using the space as separator
                    string[] auxlist = buffer.ToString().Split(' ');
                    hues[i - 1] = auxlist[0]; // Hue
                    saturations[i - 1] = auxlist[1]; // Saturation
                    intensities[i - 1] = auxlist[2]; // Intensity
                }

                //Close the port
                FeasaCom_Close(portNumber);

                return true;
            }
            else
            {
                //Error: unable to open the selected port
                MessageBox.Show("Unable to open the port", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }
        }
    }
}
