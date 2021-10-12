
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using MagentaSoft.Properties;

namespace MagentaSoft.Classes
{
    public class DataBase
    {
        private readonly string _connectionString = Application.StartupPath + "\\MagentaDB.db";
        public SQLiteConnection Connection;
        public DataBase()
        {
            CreateConnection();
        }
        private void CreateConnection()
        {
            if (!File.Exists(_connectionString))
            {
                File.Create(_connectionString);
                Connection = new SQLiteConnection("Data Source=" + _connectionString);
                CreateDb();
                //create db
            }
            Connection = new SQLiteConnection("Data Source=" + _connectionString);
            try
            {
                CreateDb();
                Connection.Open();
                Connection.Close();
            }
            catch
            {
                Connection = null;
            }

        }
        private void CreateDb()
        {
            SQLiteCommand command = new SQLiteCommand("CREATE TABLE if not exists [Files]([Id] INTEGER PRIMARY KEY AUTOINCREMENT,[HashData] VARCHAR); " +
                                                      "CREATE TABLE if not exists[Records]([Id] INTEGER,[Url] VARCHAR,[Status] INTEGER,[IdFile] INTEGER REFERENCES[Files]([Id]) ON DELETE CASCADE ON UPDATE CASCADE); ", Connection);
            Connection.Open();
            command.ExecuteNonQuery();
            Connection.Close();
        }
        public UrlTable GetData(string hash)
        {
            UrlTable table = null;
            CreateConnection();
            if (Connection != null)
            {
                try
                {
                    SQLiteCommand command =
                        new SQLiteCommand(
                            $"select records.id as id, url, status from files, records where files.id=records.idFile and hashData='{hash}'",
                            Connection);
                    Connection.Open();
                    SQLiteDataReader reader = command.ExecuteReader();
                    table = new UrlTable();
                    table.FillData(reader);
                    reader.Close();
                    reader.Dispose();
                }
                catch
                {
                    return null;
                }
                finally
                {
                    Connection.Close();
                }
            }
            return table;
        }
        private bool IsEqualImage(Bitmap img1, Bitmap img2)
        {
            if (img1.Width == img2.Width && img1.Height == img2.Height)
            {
                for (int i = 0; i < img1.Width; i += 4)
                {
                    for (int j = 0; j < img1.Height; j += 4)
                    {
                        if (img1.GetPixel(i, j) != img2.GetPixel(i, j))
                            return false;
                    }
                }

                return true;
            }

            return false;
        }
        public void InsertDataTable(DataTable table, string hash)
        {
            CreateConnection();
            if (Connection != null)
            {
                SQLiteCommand command = new SQLiteCommand("begin", Connection);
                Connection.Open();
                command.ExecuteNonQuery();
                command =
                    new SQLiteCommand($"insert into files (hashdata)values('{hash}');", Connection);
                command.ExecuteNonQuery();
                foreach (DataRow tableRow in table.Rows)
                {
                    int status = 0;
                    if (IsEqualImage((Bitmap)tableRow["StatusColumn"], Resources.ok))
                        status = 1;
                    else if (IsEqualImage((Bitmap)tableRow["StatusColumn"], Resources.error))
                    {
                        status = -1;
                    }
                    command = new SQLiteCommand($"insert into records (id, url, status, idFile) values('{tableRow["IdColumn"]}', '{tableRow["UrlColumn"]}',{status},(select id from files order by id desc limit 1))", Connection);

                    command.ExecuteNonQuery();

                }
                command = new SQLiteCommand("end", Connection);
                command.ExecuteNonQuery();
                Connection.Close();
            }
        }
        public void UpdateStatus(int status, int idRecord, string hash)
        {
            CreateConnection();
            if (Connection != null)
            {
                SQLiteCommand command = new SQLiteCommand($"update records set status={status} where id={idRecord} and idFile=(select id from files where hashdata='{hash}' order by id desc limit 1)", Connection);
                Connection.Open();
                command.ExecuteNonQuery();
                Connection.Close();
            }
        }

        public void ClearDb()
        {
            CreateConnection();
            if (Connection != null)
            {
                SQLiteCommand command = new SQLiteCommand("delete from files;" +
                                                          "delete from records", Connection);
                Connection.Open();
                command.ExecuteNonQuery();
                Connection.Close();
            }

        }
        public bool IsExistHash(string hash)
        {
            bool flag= false;
            CreateConnection();
            if (Connection != null)
            {
                SQLiteCommand command = new SQLiteCommand($"select * from Files where HashData='{hash}'", Connection);
                Connection.Open();
                SQLiteDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                    flag = true;
                reader.Close();
                reader.Dispose();
                Connection.Close();
            }
            return flag;
        }


    }
}
