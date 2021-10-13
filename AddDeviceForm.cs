using System;
using System.Windows.Forms;
using MagentaSoft.Classes;

namespace MagentaSoft
{
    public partial class AddDeviceForm : Form
    {
        public string Message="";
        public Device Device;

        private bool _flag;



        public AddDeviceForm()
        {
            InitializeComponent();
        }

        private void AddDeviceForm_Shown(object sender, System.EventArgs e)
        {
            Message = "";
            _flag = true;
            while (_flag)
            {
                Application.DoEvents();
                Device = new Device();
                if (!string.IsNullOrEmpty(Device.DeviceName))
                {
                    Message = $@"Подключено устройство-{Device.DeviceName}" + Environment.NewLine;
                    break;
                }
                
            }
            Close();
            
        }

        private void AddDeviceForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _flag = false;
        }
    }
}
