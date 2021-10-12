using System;
using System.Text;

namespace MagentaSoft.Classes.Model
{
    public class MyUrl
    {
        public string Url;
        public int UrlLen;
        public string HexUrl;


        public MyUrl(string url)
        {
            Url = url;
            GetUrlLength();
            GetHexUrl();

        }


        private void GetUrlLength()
        {
            string cutUrl = Url.TrimStart('h', 't', 'p', 's', ':', '/','w');
            UrlLen= Encoding.UTF8.GetBytes(cutUrl).Length+1;
        }

        private void GetHexUrl()
        {
            HexUrl = "";
            byte[] arr = Encoding.UTF8.GetBytes(Url);
            foreach (byte item in arr)
            {
                string temp = Convert.ToString(item, 16);
                if (temp.Length % 2 == 1)
                    temp = "0" + temp;
                HexUrl += temp;
            }
        }

    }
}
