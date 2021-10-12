using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using MagentaSoft.Properties;


namespace MagentaSoft.Classes
{
    public class UrlTable
    {
        public  DataTable Data;
        public UrlTable()
        {
            Data = new DataTable("main");
            Data.Columns.Add("StatusColumn", typeof(Bitmap));
            Data.Columns.Add("IdColumn", typeof(int));
            Data.Columns.Add("UrlColumn", typeof(string));
            Data.Columns[0].Caption = @"Статус";
            Data.Columns[1].Caption = @"№";
            Data.Columns[2].Caption = @"Url";
        }


        public void FillData(SQLiteDataReader reader)
        {
            Image waitImg = Resources.waiting__1_;
            Image errorImg = Resources.error;
            Image okImg = Resources.ok;
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    int status = int.Parse(reader["status"].ToString());
                    Image img;
                    if (status < 0)
                        img = errorImg;
                    else if (status == 0)
                        img = waitImg;
                    else
                        img = okImg;
                    Data.Rows.Add(img, int.Parse(reader["id"].ToString()), reader["url"].ToString());
                }
            }
        }

        public void FillData(string data)
        {
            Image img = Resources.waiting__1_;
            Regex reg = new Regex(@"https?://[^\s]*");
            MatchCollection matches = reg.Matches(data);
            int i = 1;
            foreach (Match match in matches)
            {
                if (Path.GetExtension(match.Value) == "")
                    Data.Rows.Add(img, i++, match.Value.Trim());
            }
        }
    }
}
