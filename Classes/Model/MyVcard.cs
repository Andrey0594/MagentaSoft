using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace MagentaSoft.Classes.Model
{
    public class MyVcard
    {
        public int VcardLen;
        public string VcardHex;

        public string FormatFio;
        public string Company;
        public string Post;
        public string Address;
        public string WorkPhone;
        public string Phone;
        public string Email;
        public string WorkEmail;
        public string Site;
        public string WorkSite;
        public string Birthday;
        public string Facebook;
        public string Instagram;
        public string Telegram;
        public string Skype;
        public string Whatsup;
        public string Vk;
        public string LinkedIn;
        public string Youtube;
        public string TikTok;
        public string Twitter;
        public string ClubHouse;
        public string Gis;
        public string Note;
        public string Vcf;

        public MyVcard()
        {
            VcardLen = 0;
            VcardHex = "";
            FormatFio = null;
            Company = null;
            Post = null;
            Address = null;
            WorkPhone = null;
            Phone = null;
            Email = null;
            WorkEmail = null;
            Site = null;
            WorkSite = null;
            Birthday = null;
            Facebook = null;
            Instagram = null;
            Telegram = null;
            Skype = null;
            Whatsup = null;
            Vk = null;
            LinkedIn = null;
            Youtube = null;
            TikTok = null;
            Twitter = null;
            ClubHouse = null;
            Gis = null;
            Note = null;
            Vcf = null;
        }
        public MyVcard(string url, string site)
        {
            VcardLen = 0;
            VcardHex = "";
            using (WebClient client = new WebClient())
            {

                string html = client.DownloadString(url);
                GetVcf(html, client, site);
                Regex reg = new Regex(@"FN[^:]*:([^\n]*)");
                FormatFio = GetField(reg, Vcf);
                reg = new Regex(@"ORG[^:]*:([^\n]*)");
                Company = GetField(reg, Vcf);
                reg = new Regex(@"TITLE[^:]*:([^\n]*)");
                Post = GetField(reg, Vcf);
                reg = new Regex(@"ADR[^:]*:([^\n]*)");
                Address = GetField(reg, Vcf).Trim(';');
                reg = new Regex(@"TEL;TYPE=work:([^\n]*)");
                WorkPhone = GetField(reg, Vcf);
                reg = new Regex(@"TEL;TYPE=cell:([^\n]*)");
                Phone = GetField(reg, Vcf);
                reg = new Regex(@"EMAIL;TYPE=INTERNET,PREF:([^\n]*)");
                Email = GetField(reg, Vcf);
                reg = new Regex(@"EMAIL;TYPE=INTERNET:([^\n]*)");
                WorkEmail = GetField(reg, Vcf);
                reg = new Regex(@"URL:([^\n]*)");
                Site = GetField(reg, Vcf);
                reg = new Regex(@"URL;type=WORK;[^:]*:([^\n]*)");
                WorkSite = GetField(reg, Vcf);
                reg = new Regex(@"BDAY:([^\n]*)");
                Birthday = GetField(reg, Vcf);
                reg = new Regex(@"X-SOCIALPROFILE;type=facebook;x-user=([^:]*):");
                Facebook = GetField(reg, Vcf);
                reg = new Regex(@"X-SOCIALPROFILE;type=Instagram;x-user=([^:]*):");
                Instagram = GetField(reg, Vcf);
                reg = new Regex(@"X-SOCIALPROFILE;type=Telegram;x-user=([^:]*):");
                Telegram = GetField(reg, Vcf);
                reg = new Regex(@"IMPP;X-SERVICE-TYPE=Skype:skype:([^\n]*)");
                Skype = GetField(reg, Vcf);
                reg = new Regex(@"X-SOCIALPROFILE;type=whatsup;x-user=([^:]*):");
                Whatsup = GetField(reg, Vcf);
                reg = new Regex(@"X-SOCIALPROFILE;type=vk;x-user=([^:]*):");
                Vk = GetField(reg, Vcf);
                reg = new Regex(@"X-SOCIALPROFILE;type=linkedin;x-user=([^:]*):");
                LinkedIn = GetField(reg, Vcf);
                reg = new Regex(@"X-SOCIALPROFILE;type=youtube;x-user=([^:]*):");
                Youtube = GetField(reg, Vcf);
                reg = new Regex(@"X-SOCIALPROFILE;type=tiktok;x-user=([^:]*):");
                TikTok = GetField(reg, Vcf);
                reg = new Regex(@"X-SOCIALPROFILE;type=twitter;x-user=([^:]*):");
                Twitter = GetField(reg, Vcf);
                reg = new Regex(@"X-SOCIALPROFILE;type=clubhouse;x-user=([^:]*):");
                ClubHouse = GetField(reg, Vcf);
                reg = new Regex(@"X-SOCIALPROFILE;type=2gis;x-user=2ГИС:([^\n]*)");
                Gis = GetField(reg, Vcf);
                reg = new Regex(@"NOTE[^:]*:([^\n]*)");
                Note = GetField(reg, Vcf);
                GetVcardLength();
                GetHexUrl();

            }
        }

        public string GetVCard()
        {
            StringBuilder sBuilder = new StringBuilder();
            sBuilder.AppendLine("BEGIN:VCARD");
            sBuilder.AppendLine("VERSION:3.0");
            sBuilder.AppendLine("PRODID:nfcc");

            if (!string.IsNullOrEmpty(FormatFio))
            {
                sBuilder.AppendLine($"N;CHARSET=UTF-8:{FormatFio.Replace(" ", ";")}");
                sBuilder.AppendLine($"FN;CHARSET=UTF-8:{FormatFio}");
            }
            if (!string.IsNullOrEmpty(Company))
                sBuilder.AppendLine($"ORG;CHARSET=UTF-8:{Company}");
            if (!string.IsNullOrEmpty(Post))
                sBuilder.AppendLine($"TITLE;CHARSET=UTF-8:{Post}");
            if (!string.IsNullOrEmpty(Address))
                sBuilder.AppendLine($"ADR;TYPE=dom,work;CHARSET=UTF-8:;;{Address};;;;");
            if (!string.IsNullOrEmpty(WorkPhone))
                sBuilder.AppendLine($"TEL;TYPE=work:{WorkPhone}");
            if (!string.IsNullOrEmpty(Phone))
                sBuilder.AppendLine($"TEL;TYPE=cell:{Phone}");
            if (!string.IsNullOrEmpty(Email))
                sBuilder.AppendLine($"EMAIL;TYPE=INTERNET,PREF:{Email}");
            if (!string.IsNullOrEmpty(WorkEmail))
                sBuilder.AppendLine($"EMAIL;TYPE=INTERNET:{WorkEmail}");
            if (!string.IsNullOrEmpty(Site))
                sBuilder.AppendLine($"URL:{Site}");
            if (!string.IsNullOrEmpty(WorkSite))
                sBuilder.AppendLine($"URL;type=WORK;CHARSET=UTF-8:{WorkSite}");
            if (!string.IsNullOrEmpty(Birthday))
                sBuilder.AppendLine($"BDAY:{Birthday}");
            if (!string.IsNullOrEmpty(Facebook))
                sBuilder.AppendLine($"X-SOCIALPROFILE;type=facebook;x-user={Facebook}:https://facebook.com/{Facebook.TrimStart('@')}/");
            if (!string.IsNullOrEmpty(Instagram))
                sBuilder.AppendLine($"X-SOCIALPROFILE;type=Instagram;x-user={Instagram}:https://instagram.com/{Instagram.TrimStart('@')}/");
            if (!string.IsNullOrEmpty(Telegram))
                sBuilder.AppendLine($"X-SOCIALPROFILE;type=Telegram;x-user={Telegram}:https://tele.click/{Telegram.TrimStart('@')}");
            if (!string.IsNullOrEmpty(Skype))
                sBuilder.AppendLine($"IMPP;X-SERVICE-TYPE=Skype:skype:{Skype}");
            if (!string.IsNullOrEmpty(Whatsup))
                sBuilder.AppendLine($"X-SOCIALPROFILE;type=whatsup;x-user={Whatsup}:https://wa.me/{Whatsup.TrimStart('@')}");
            if (!string.IsNullOrEmpty(Vk))
                sBuilder.AppendLine($"X-SOCIALPROFILE;type=vk;x-user={Vk}:https://vk.com/{Vk.TrimStart('@')}");
            if (!string.IsNullOrEmpty(LinkedIn))
                sBuilder.AppendLine($"X-SOCIALPROFILE;type=linkedin;x-user={LinkedIn}:https://linkedin.com/in/{LinkedIn.TrimStart('@')}");
            if (!string.IsNullOrEmpty(Youtube))
                sBuilder.AppendLine($"X-SOCIALPROFILE;type=youtube;x-user={Youtube}:https://youtube.ru/{Youtube.TrimStart('@')}");
            if (!string.IsNullOrEmpty(TikTok))
                sBuilder.AppendLine($"X-SOCIALPROFILE;type=tiktok;x-user={TikTok}:https://tiktok.com/{TikTok}/");
            if (!string.IsNullOrEmpty(Twitter))
                sBuilder.AppendLine($"X-SOCIALPROFILE;type=twitter;x-user={Twitter}:https://twitter.com/{Twitter.TrimStart('@')}");
            if (!string.IsNullOrEmpty(ClubHouse))
                sBuilder.AppendLine($"X-SOCIALPROFILE;type=clubhouse;x-user={ClubHouse}:https://clubber.one/{ClubHouse.TrimStart('@')}");
            if (!string.IsNullOrEmpty(Gis))
                sBuilder.AppendLine($"X-SOCIALPROFILE;type=2gis;x-user=2ГИС:{Gis}");
            if (!string.IsNullOrEmpty(Note))
                sBuilder.AppendLine($"NOTE;CHARSET=UTF-8:{Note}");
            sBuilder.AppendLine("END:VCARD");
            return sBuilder.ToString();
        }
        public List<string[]> GetStruct()
        {
            List<string[]> resultList = new List<string[]>();

            List<string> param = this.GetType().GetFields().Select(t => t.Name).ToList();
            GetType().GetFields().Where(t => t.Name != "Vcf" && t.Name != "VcardLen" && t.Name != "VcardHex").ToList().ForEach(t =>
            {
                string[] temp = new[] { t.Name, t.GetValue(this)?.ToString() };
                resultList.Add(temp);
            });
            return resultList;

        }
        private void GetVcardLength()
        {
            VcardLen = 0;
            if (!string.IsNullOrEmpty(Vcf))
                VcardLen = Encoding.UTF8.GetBytes(Vcf).Length;
        }

        private void GetHexUrl()
        {
            VcardHex = "";
            if (!string.IsNullOrEmpty(Vcf))
            {
                byte[] arr = Encoding.UTF8.GetBytes(Vcf);
                foreach (byte item in arr)
                {
                    string temp = Convert.ToString(item, 16);
                    if (temp.Length % 2 == 1)
                        temp = "0" + temp;
                    VcardHex += temp;
                }
            }
        }

        private string GetField(Regex reg, string data)
        {
            string field = "";
            MatchCollection matches = reg.Matches(data);
            if (matches.Count > 0 && matches[0].Groups.Count > 1)
            {
                field = matches[0].Groups[1].Value;
            }
            return field;
        }
        private void CreateFile()
        {
            if (!Directory.Exists(Application.StartupPath + "\\TempFiles"))
                Directory.CreateDirectory(Application.StartupPath + "\\TempFiles");
            if (File.Exists(Application.StartupPath + "\\TempFiles\\tempFile.vcf"))
                File.Delete(Application.StartupPath + "\\TempFiles\\tempFile.vcf");
        }
        private void GetVcf(string data, WebClient client, string site)
        {
            CreateFile();
            Regex reg = new Regex($"https://{site}/vcf/[^\">]*");
            MatchCollection matches = reg.Matches(data);
            if (matches.Count > 0)
            {
                if (matches[0].Groups.Count == 1)
                {
                    client.DownloadFile(matches[0].Groups[0].Value,
                        Application.StartupPath + "\\TempFiles\\tempFile.vcf");
                    Vcf = File.ReadAllText(Application.StartupPath + "\\TempFiles\\tempFile.vcf");
                }
            }
        }
    }
}
