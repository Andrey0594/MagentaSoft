using System;
using System.Text;
using System.Windows.Forms;
using MagentaSoft.Classes;
using MagentaSoft.Classes.Model;

namespace MagentaSoft
{
    public partial class MainForm : Form
    {
        public DataBase Db;
        public DataFile File;
        public MyData Data;


        public MainForm()
        {
            InitializeComponent();
            Db = new DataBase();
        }

        private void UrlDGView_Click(object sender, EventArgs e)
        {

        }
        private void OpenFileItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog fDialog = new OpenFileDialog();
            fDialog.Multiselect = false;
            if (fDialog.ShowDialog() == DialogResult.OK)
            {
                File = new DataFile(fDialog.FileName);
                UrlDGView.DataSource = File.Data.Data;
                UrlDGView.Columns[0].Width = UrlDGView.Columns[1].Width = 50;
            }
        }

        private void UrlDGView_CurrentRowChanged(object sender, Telerik.WinControls.UI.CurrentRowChangedEventArgs e)
        {
            if (e.CurrentRow != null)
            {
                try
                {
                    ParametrsDGView.Rows.Clear();
                    Data = new MyData(e.CurrentRow.Cells["UrlColumn"].Value.ToString());

                    VcfTBox.Text = Data.Card.Vcf;
                    var temp = Data.Card.GetStruct();
                    foreach (string[] item in temp)
                    {
                        ParametrsDGView.Rows.Add(item[0], item[1]);
                    }
                    ParametrsDGView.Rows[0].IsCurrent = true;
                    WriteBtn.Enabled = true;
                    Data.GetLength();
                    UrlSizeValue.Text = Data.Url.UrlLen.ToString();
                    VCardSizeValue.Text = Data.Card.VcardLen.ToString();
                    OnlyUrlBtn_CheckedChanged(this, new EventArgs());
                    if (VcfTBox.Text != "")
                        OnlyCardBtn.Enabled = CardBeforeUrl.Enabled = UrlBeforeCardBtn.Enabled = true;
                    else
                    {
                        OnlyCardBtn.Enabled = CardBeforeUrl.Enabled = UrlBeforeCardBtn.Enabled = false;
                    }


                }
                catch (ArgumentException ex)
                {
                    VcfTBox.Text = ex.Message;
                    e.CurrentRow.Cells["StatusColumn"].Value = Properties.Resources.error;
                    Db.UpdateStatus(-1, int.Parse(e.CurrentRow.Cells["IdColumn"].Value.ToString()), File.Hash);
                    WriteBtn.Enabled = false;
                    OnlyCardBtn.Enabled = CardBeforeUrl.Enabled = UrlBeforeCardBtn.Enabled = false;
                }



            }
        }

        private void ClearDbItem_Click(object sender, EventArgs e)
        {
            Db?.ClearDb();
        }

        private void LogTBox_TextChanged(object sender, EventArgs e)
        {
            System.IO.File.WriteAllText(Application.StartupPath + "\\Log.txt", LogTBox.Text, Encoding.Default);
        }

        private void ParametrsDGView_CellValueChanged(object sender, Telerik.WinControls.UI.GridViewCellEventArgs e)
        {
            if (e.Row != null)
            {
                var value = e.Value == null ? "" : e.Value.ToString();
                string fieldColumn = e.Row.Cells["ParametrColumn"].Value.ToString();
                Data.Card.GetType().GetField(fieldColumn).SetValue(Data.Card, value);
                Data.Card.Vcf = Data.Card.GetVCard();
                VcfTBox.Text = Data.Card.Vcf;
                Data.GetLength();
                UrlSizeValue.Text = Data.Url.UrlLen.ToString();
                VCardSizeValue.Text = Data.Card.VcardLen.ToString();
                OnlyUrlBtn_CheckedChanged(this, new EventArgs());
            }

        }

        private void OnlyUrlBtn_CheckedChanged(object sender, EventArgs e)
        {
            if (OnlyUrlBtn.Checked)
            {
                SumSizeValue.Text = Data.OnlyUrlLength.ToString();
            }
            else if (OnlyCardBtn.Checked)
            {
                SumSizeValue.Text = Data.OnlyVcardLength.ToString();
            }
            else if (UrlBeforeCardBtn.Checked)
            {
                SumSizeValue.Text = Data.UrlBeforeCardLength.ToString();
            }
            else
            {
                SumSizeValue.Text = Data.CardBeforeUrlLength.ToString();
            }

            StatusLbl.Image = int.Parse(SumSizeValue.Text) <= int.Parse(MaxSizeValue.Text) ? Properties.Resources.ok : Properties.Resources.error;
        }
    }
}
