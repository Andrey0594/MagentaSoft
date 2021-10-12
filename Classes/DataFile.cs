using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;


namespace MagentaSoft.Classes
{
    public class DataFile
    {
        public UrlTable Data;
        public string MyFile;
        public string Hash;

        public DataFile(string file)
        {
            if (File.Exists(file))
            {
                MyFile = file;
                GetHash();
                DataBase db = new DataBase();
                if (db.IsExistHash(Hash))
                {
                    Data = db.GetData(Hash);
                    if (Data == null)
                    {
                        GetDataFromFile();
                        if (Data.Data != null)
                            db.InsertDataTable(Data.Data, Hash);
                    }
                   
                }
                else
                {
                    GetDataFromFile();
                    if(Data.Data!=null)
                        db.InsertDataTable(Data.Data,Hash);
                }
            }
            else
            {
                throw new FileNotFoundException("Файл не найден");
            }
        }

        private void GetHash()
        {
            if (MyFile != null && File.Exists(MyFile))
            {
                byte[] arr = File.ReadAllBytes(MyFile);
                HashAlgorithm sha1 = new SHA256CryptoServiceProvider();
                byte[] shaHash = sha1.ComputeHash(arr);
                Hash = "";
                foreach (byte item in shaHash)
                {
                    Hash += Convert.ToString(item, 16);
                }
            }
            else
                throw new FileNotFoundException();
        }

        public void GetDataFromFile()
        {
            string data = File.ReadAllText(MyFile, Encoding.Default);
            if (!string.IsNullOrEmpty(data))
            {
                Data = new UrlTable();
                Data.FillData(data);
            }
        }

    }
}
