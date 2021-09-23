using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;

using System.Text;
using System.Web;

namespace TrOCR.Helper
{
    public static class GoogleTranslate
    {
        public static string Translate(string text, string from, string to)
        {
            var query = HttpUtility.UrlEncode(text)?.Replace("+", "%20");
            var url = "https://translate.google.cn/translate_a/single";
            var param= $"client=at&sl={from}&tl={to}&dt=t&q={query}";
            try
            {
                string data = HttpPost(url,param);
                dynamic jsonr = Json2Object<dynamic>(data);
                string result = "";
                foreach (var s in jsonr[0])
                {
                    result += (s[0] + " ");
                }
                return result;
            }
            catch (Exception ex)
            {

                return ex.Message;
            }


        }

        public static string HttpPost(string url, string paramData = "")
        {
            string result = string.Empty;
            try
            {
                HttpWebRequest wbRequest = (HttpWebRequest)WebRequest.Create(url);
                wbRequest.Method = "POST";
                wbRequest.ContentType = "application/x-www-form-urlencoded";
                wbRequest.Referer = "https://translate.google.cn/";
                wbRequest.Host = "translate.google.com";
                wbRequest.Headers.Add("Accept-Encoding", "gzip, deflate, br");
                wbRequest.Headers.Add("Accept-Language", "zh-Hans-CN, zh-Hans; q=0.5");
                wbRequest.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9";
                wbRequest.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/93.0.4577.63 Safari/537.36";
                wbRequest.ContentLength = Encoding.UTF8.GetByteCount(paramData);
                using (Stream requestStream = wbRequest.GetRequestStream())
                {
                    using (StreamWriter swrite = new StreamWriter(requestStream))
                    {
                        swrite.Write(paramData);
                    }
                }
                using (HttpWebResponse response = (HttpWebResponse)wbRequest.GetResponse())
                {
                    string ContentEncoding = response.ContentEncoding.ToLower();
                    Stream stream = null;
                    if (ContentEncoding.Contains("gzip"))
                    {
                        stream = new GZipStream(response.GetResponseStream(), CompressionMode.Decompress);
                    }
                    else if (ContentEncoding.Contains("deflate"))
                    {
                        stream = new DeflateStream(response.GetResponseStream(), CompressionMode.Decompress);
                    }
                    else
                    {
                        stream = response.GetResponseStream();
                    }
                    using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                    {
                        result = reader.ReadToEnd();
                    }
                }
            }
            catch (Exception ex)
            { }

            return result;
        }

        public static T Json2Object<T>(string strJson)
        {
            T result;
            using (System.IO.MemoryStream memoryStream = new System.IO.MemoryStream(System.Text.Encoding.UTF8.GetBytes(strJson)))
            {
                System.Runtime.Serialization.Json.DataContractJsonSerializer dataContractJsonSerializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(T));
                try
                {
                    result = (T)((object)dataContractJsonSerializer.ReadObject(memoryStream));
                }
                catch (System.Exception ex)
                {
                    result = default(T);
                }
            }
            return result;
        }
    }
}
