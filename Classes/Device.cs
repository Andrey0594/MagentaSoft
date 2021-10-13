using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Management;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace MagentaSoft.Classes
{
    public class Device
    {
        public string DeviceName;
        private SerialPort _port = null;
        public bool DetectCardFlag = true;


        public static Dictionary<String, String> GetPortNames()
        {

            Dictionary<String, String> ports = new Dictionary<String, String>();
            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("Select * From Win32_SerialPort");

                foreach (var obj in searcher.Get())
                {
                    var queryObj = (ManagementObject)obj;
                    if (queryObj != null && queryObj["DeviceID"].ToString().Contains("COM"))
                    {

                        String pidVid = queryObj["PNPDeviceID"].ToString().Replace("\\\\", "\\").Split('\\')[1];
                        ports.Add(pidVid, queryObj["DeviceID"].ToString());
                    }
                }
            }
            catch
            {
                //ignored
            }

            return ports;
        }


        public Device()
        {
            var temp = GetPortNames();
            for (int i = 0; i < temp.Keys.Count; i++)
            {
                try
                {
                    _port = new SerialPort(temp[temp.Keys.ToList()[i]], 115200, Parity.None, 8);
                    _port.Open();
                    _port.Write("ATI\r");
                    Application.DoEvents();
                    Thread.Sleep(200);
                    string message = _port.ReadExisting();
                    message = message.Replace("\r\n", "").Replace("\n", "").Replace("OK", "");
                    if (message == "\n")
                        message = "";
                    if (message.ToLower().Contains(@"odrfid"))
                    {
                        DeviceName = message;
                        break;
                    }

                }
                catch
                {
                    //iognored
                }
                finally
                {
                    _port.Close();
                }


            }
        }


        public string DetectNewCard()
        {
            string message = "";
            DetectCardFlag = true;
            IsDeviceEnabled();
            try
            {
                _port.Open();
                _port.Write("AT+SCAN1\r");
                while (DetectCardFlag)
                {
                    Application.DoEvents();
                    message = _port.ReadExisting().Replace("\r\n", "").Replace("\n", "");
                    if (message.Length == 8)
                    {
                        DetectCardFlag = false;

                    }
                }
                _port.Close();
            }
            catch
            {
                //ignored
            }
            return message;
        }


        public string FormatCard()
        {
            IsDeviceEnabled();
            string message = "";
            try
            {
                _port.Open();
               // message = _port.ReadExisting().Replace("\r\n", "\r").Trim('\n');
                _port.Write("AT+SCAN0\r");
               // message = _port.ReadExisting().Replace("\r\n", "\r").Trim('\n');
                _port.Write("AT+W1:140103E103E103E103E103E103E103E1\r");
                //Thread.Sleep(50);
                message = _port.ReadExisting().Replace("\r\n", "\r").Trim('\n');
                if (message.ToLower().Contains("error"))
                {
                    _port.Write("AT+KAD3F7D3F7D3F7\r");
                    _port.Write("AT+KBFFFFFFFFFFFF\r");
                    _port.Write("AT+W4:00000304D8000000FE00000000000000\r");
                    for (int i = 7; i < 64; i += 4)
                    {
                        _port.Write($"AT+W{i}:D3F7D3F7D3F77F078840FFFFFFFFFFFF\r");
                    }
                    message = _port.ReadExisting();
                }
                else
                {
                    _port.Write("AT+W2:03E103E103E103E103E103E103E103E1\r");
                    _port.Write("AT+W3:A0A1A2A3A4A5787788C1FFFFFFFFFFFF\r");
                    _port.Write("AT+W4:00000304D8000000FE00000000000000\r");
                    for (int i = 7; i < 64; i += 4)
                    {
                        _port.Write($"AT+W{i}:D3F7D3F7D3F77F078840FFFFFFFFFFFF\r");
                    }
                    message = _port.ReadExisting();
                }
            }
            catch
            {
                //ignored
            }
            finally
            {
                if (_port.IsOpen)
                    _port.Close();

            }
            if (message.ToLower().Contains("error"))
                return "При форматировании карты возникли ошибки";
            return "Карта отформатирована";


        }



        public string  WriteData(string data)
        {
            //IsDeviceEnabled();
            string message = "error";
            try
            {
                _port.Open();
                message = _port.ReadExisting().Replace("\r\n", "\r").Trim('\n');
                _port.Write("AT+SCAN0\r");
                message = _port.ReadExisting().Replace("\r\n", "\r").Trim('\n');
                //_port.Write("AT+KAD3F7D3F7D3F7\r");
                //_port.Write("AT+KBFFFFFFFFFFFF\r");
                message = _port.ReadExisting();
                int i = 0;
                int block = 4;
                while (i < data.Length)
                {
                    if ((block + 1) % 4 == 0)
                        block++;
                    string tempUrl = data.Substring(i, 32);
                    _port.Write($"AT+W{block}:{tempUrl}\r");
                    block++;
                    i += 32;
                }
                message = _port.ReadExisting();
            }
            catch
            {
                int a = 0;
                //ignored
            }
            finally
            {
                if (_port.IsOpen)
                    _port.Close();

            }
            if (message.ToLower().Contains("error"))
                return  "Во время записи возникли ошибки";
            return "Данные записаны на карту";
        }

        public void SetScan0()
        {
            _port.Open();
            _port.Write("AT+SCAN0\r");
            _port.Close();
        }
        public void SetScan1()
        {
            _port.Open();
            _port.Write("AT+SCAN1\r");
            _port.Close();
        }

        private void IsDeviceEnabled()
        {
            bool flag = false;
            if (_port != null)
            {
                try
                {
                    _port.Open();
                    _port.Write("ATI\r");
                    string message = "";
                    for (int i = 0; i < 10; i++)
                    {
                        Thread.Sleep(50);
                        message = _port.ReadExisting();
                        if (message.ToLower().Contains(@"odrfid"))
                        {
                            flag = true;
                            break;
                        }
                    }
                    _port.Close();
                }
                catch
                {
                    //ignored
                }
            }

            if (!flag)
            {
                AddDeviceForm frm = new AddDeviceForm();
                frm.ShowDialog(Application.OpenForms[0]);
                if (frm.Message == "")
                {
                    Application.OpenForms[0].Close();
                }
            }

        }
    }
}
