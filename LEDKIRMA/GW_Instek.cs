using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;

namespace LEDKIRMA
{
    class GW_Instek
    {
        private SerialPort serialPort;

        public event EventHandler<string> DataReceivedEvent;

        public GW_Instek(string portName)
        {
            serialPort = new SerialPort(portName, 2400, Parity.None, 8, StopBits.One);
            serialPort.DataReceived += SerialPort_DataReceived;
            serialPort.Handshake = Handshake.None;
            serialPort.RtsEnable = true;
        }

        public void OpenPort()
        {
            try
            {
                if (!serialPort.IsOpen)
                {
                    serialPort.Open();
                    Console.WriteLine("Port opened.");
                }
                else
                {
                    Console.WriteLine("Port is already open.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error opening port: " + ex.Message);
            }
        }

        // Method to close the serial port
        public void ClosePort()
        {
            try
            {
                if (serialPort.IsOpen)
                {
                    serialPort.Close();
                    Console.WriteLine("Port closed.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error closing port: " + ex.Message);
            }
        }

        public void SendCommand(string command)
        {
            try
            {
                if (serialPort.IsOpen)
                {
                    serialPort.WriteLine(command);
                    Console.WriteLine("Command sent: " + command);
                }
                else
                {
                    Console.WriteLine("Serial port is not open.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error sending command: " + ex.Message);
            }
        }

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                string data = serialPort.ReadLine();

                DataReceivedEvent?.Invoke(this, data); // Olayı tetikleyin
            }
            catch (Exception ex)
            {

            }
        }
    }
}
