using System;
using System.Text;

namespace MagentaSoft.Classes.Model
{
    public class MyData
    {
        public MyUrl Url;
        public MyVcard Card;
        public int OnlyUrlLength;
        public int OnlyVcardLength;
        public int UrlBeforeCardLength;
        public int CardBeforeUrlLength;


        public MyData(string url)
        {
            Url = new MyUrl(url);
            if (url.ToLower().Contains("magenta") || url.ToLower().Contains("nfcc"))
            {
                string site = url.ToLower().Contains("magenta") ? "magenta.su" : "nfcc.ru";
                try
                {
                    Card = new MyVcard(url, site);
                }
                catch 
                {
                    Card = new MyVcard();
                    throw new ArgumentException("Данная ссылка не существует");
                }
                
            }
            else
            {
                
            }
            GetLength();

        }

        public void GetLength()
        {
            OnlyUrlLength = GetHexOnlyUrl().Length / 2;
            if(Card?.Vcf != null)
            {
                OnlyVcardLength = GetHexOnlyVcard().Length / 2;
                UrlBeforeCardLength = GetHexUrlBeforeCard().Length / 2;
                CardBeforeUrlLength = GetHexVcardBeforeUrl().Length / 2;
            }
            else
            {
                OnlyVcardLength = UrlBeforeCardLength = CardBeforeUrlLength = 0;
            }
        }



        private string GetHexData(string data)
        {
            string result = "";
            byte[] arr = Encoding.UTF8.GetBytes(data);
            foreach (byte item in arr)
            {
                string temp = Convert.ToString(item, 16);
                if (temp.Length % 2 == 1)
                    temp = "0" + temp;
                result += temp;
            }



            return result.ToUpper();
        }
        private string GetBeginOnlyUrl(int urlLength, string type)
        {
            if (urlLength < 256)
            {
                int totalLenght = urlLength + 5;
                string hexTotalLength = Convert.ToString(totalLenght, 16);
                if (hexTotalLength.Length % 2 == 1)
                    hexTotalLength = "0" + hexTotalLength;
                string hexLength = Convert.ToString(urlLength + 1, 16);
                if (hexLength.Length % 2 == 1)
                    hexLength = "0" + hexLength;
                return ("000003" + hexTotalLength + "D101" + hexLength + "55" + type).ToUpper();
            }
            else
            {
                int totalLenght = urlLength + 7;
                string hexTotalLength = Convert.ToString(totalLenght, 16);
                if (hexTotalLength.Length % 2 == 1)
                    hexTotalLength = "0" + hexTotalLength;
                string hexLength = Convert.ToString(urlLength + 1, 16);
                if (hexLength.Length % 2 == 1)
                    hexLength = "0" + hexLength;
                return ("03FF" + hexTotalLength + "C1010000" + hexLength + "55" + type).ToUpper();
            }
            //return "";
        }
        public string GetHexOnlyUrl()
        {
            string tempUrl = Url.Url.TrimStart('h', 't', 'p', 's', ':', '/', 'w', '.');
            string type = "03";
            if (Url.Url.ToLower().Contains("https"))
                type = "04";
            string begin = GetBeginOnlyUrl(tempUrl.Length, type);
            string hexUrl = begin + GetHexData(tempUrl) + "FE";
            while (hexUrl.Length % 32 > 0)
            {
                hexUrl += "00";
            }
            return hexUrl;
        }
        private string GetBeginOnlyVcard(int cardLenght)
        {
            if (cardLenght < 256)
            {
                int totalLenght = cardLenght + 13;
                string hexTotalLength = Convert.ToString(totalLenght, 16);
                if (hexTotalLength.Length % 2 == 1)
                    hexTotalLength = "0" + hexTotalLength;
                string hexLength = Convert.ToString(cardLenght, 16);
                if (hexLength.Length % 2 == 1)
                    hexLength = "0" + hexLength;

                return ("000003" + hexTotalLength + "D20A" + hexLength + "746578742F7663617264").ToUpper();
            }
            else
            {
                int totalLenght = cardLenght + 16;
                string hexTotalLength = Convert.ToString(totalLenght, 16);
                if (hexTotalLength.Length % 2 == 1)
                    hexTotalLength = "0" + hexTotalLength;
                string hexLength = Convert.ToString(cardLenght, 16);
                if (hexLength.Length % 2 == 1)
                    hexLength = "0" + hexLength;
                return ("03ff" + hexTotalLength + "C20A0000" + hexLength + "746578742F7663617264").ToUpper();
            }
        }
        public string GetHexOnlyVcard()
        {
            string hexCard = GetHexData(Card.Vcf);
            int len = hexCard.Length / 2;
            string begin = GetBeginOnlyVcard(len);
            hexCard = begin + hexCard + "FE";
            while (hexCard.Length % 32 > 0)
            {
                hexCard += "00";
            }
            return hexCard;
        }



        private string GetBeginUrl(int urlLength, int cardLength, string type, string begin)
        {
            if (urlLength < 256)
            {
                int totalLength = urlLength + cardLength + 21;
                string hexTotalLength = Convert.ToString(totalLength, 16);
                if (hexTotalLength.Length % 2 == 1)
                    hexTotalLength = "0" + hexTotalLength;
                string hexLength = Convert.ToString(urlLength + 1, 16);
                if (hexLength.Length % 2 == 1)
                    hexLength = "0" + hexLength;

                return (begin + hexTotalLength + "9101" + hexLength + "55" + type).ToUpper();
            }
            return "";
        }
        private string GetBeginCard(int cardLenght)
        {
            if (cardLenght < 256)
            {
                string hexLength = Convert.ToString(cardLenght, 16);
                if (hexLength.Length % 2 == 1)
                    hexLength = "0" + hexLength;

                return ("520A" + hexLength + "746578742F7663617264").ToUpper();
            }
            else
            {
                string hexLength = Convert.ToString(cardLenght, 16);
                if (hexLength.Length % 2 == 1)
                    hexLength = "0" + hexLength;
                return ("420A0000" + hexLength + "746578742F7663617264").ToUpper();
            }
        }
        public string GetHexUrlBeforeCard()
        {
            string hexData = "";
            string card = Card.Vcf.Replace("\r", "").Trim('\n');
            string tempUrl = Url.Url.TrimStart('h', 't', 'p', 's', ':', '/', 'w', '.');


            string HexUrl = GetHexData(tempUrl);
            string HexCard = GetHexData(card);

            int urlLength = HexUrl.Length / 2;
            int cardLength = HexCard.Length / 2;


            string type = "03";
            if (Url.Url.ToLower().Contains("https"))
                type = "04";



            string begin = "000003";
            if (urlLength + cardLength + 21 > 255)
            {
                begin = "03FF";
            }


            hexData += GetBeginUrl(urlLength, cardLength, type, begin) + GetHexData(tempUrl);

            hexData = (hexData + GetBeginCard(cardLength) + GetHexData(card) + "FE").ToUpper();
            while (hexData.Length % 32 > 0)
            {
                hexData += "00";
            }

            return hexData;
        }



        private string GetBeginCard(int cardLenght, int urlLength)
        {
            if (cardLenght < 256)
            {
                int totalLenght = 13 + cardLenght + 5 + urlLength;
                string hexTotalLength = Convert.ToString(totalLenght, 16);
                if (hexTotalLength.Length % 2 == 1)
                    hexTotalLength = "0" + hexTotalLength;
                string hexLength = Convert.ToString(cardLenght, 16);
                if (hexLength.Length % 2 == 1)
                    hexLength = "0" + hexLength;

                return ("000003" + hexTotalLength + "920A" + hexLength + "746578742F7663617264").ToUpper();
            }
            else
            {
                int totalLenght = cardLenght + urlLength + 1 + 20;
                string hexTotalLength = Convert.ToString(totalLenght, 16);
                if (hexTotalLength.Length % 2 == 1)
                    hexTotalLength = "0" + hexTotalLength;
                string hexLength = Convert.ToString(cardLenght, 16);
                if (hexLength.Length % 2 == 1)
                    hexLength = "0" + hexLength;


                return ("03ff" + hexTotalLength + "820A0000" + hexLength + "746578742F7663617264").ToUpper();
            }

            return "";
        }
        private string GetBeginUrl(int urlLength, string type)
        {
            if (urlLength < 256)
            {

                string hexLength = Convert.ToString(urlLength + 1, 16);
                if (hexLength.Length % 2 == 1)
                    hexLength = "0" + hexLength;
                return ("5101" + hexLength + "55" + type).ToUpper();
            }
            return "";
        }

        public string GetHexVcardBeforeUrl()
        {
            string hexData = "";
            string card = Card.Vcf.Replace("\r", "").Trim('\n');
            string tempUrl = Url.Url.TrimStart('h', 't', 'p', 's', ':', '/', 'w', '.');
            string type = "03";
            if (Url.Url.ToLower().Contains("https"))
                type = "04";
            string HexUrl = GetHexData(tempUrl);
            string HexCard = GetHexData(card);

            int urlLength = HexUrl.Length / 2;
            int cardLength = HexCard.Length / 2;

            hexData += GetBeginCard(cardLength, urlLength);
            hexData = (hexData + GetHexData(card) + GetBeginUrl(urlLength, type) + GetHexData(tempUrl) + "FE").ToUpper();
            while (hexData.Length % 32 > 0)
            {
                hexData += "00";
            }

            return hexData;
        }



    }
}
