using System;
using System.Windows.Forms;

namespace MagentaSoft
{
    public partial class InfoForm : Form
    {
        public string Url;
        public string VCard;
        public InfoForm(string url, string vcard)
        {
            Url = url;
            VCard = vcard;
            InitializeComponent();
        }

        private void InfoForm_Shown(object sender, System.EventArgs e)
        {
            textBox1.Text = "Ссылка: " + Url;
            richTextBox1.Text = "VCard:" + Environment.NewLine + Environment.NewLine + VCard;

        }
        public InfoForm()
        {
            InitializeComponent();
        }
    }
}
