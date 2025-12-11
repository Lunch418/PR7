using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Linq;
using System.Collections;
using System.Diagnostics;
using System.Text;

namespace SimpleNewsParser
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Cookie token = SingIn("user", "user");
            string Content = GetContent(token);
            ParsingHtml(Content);
            //WebRequest request = WebRequest.Create("https://news.permaviat.ru/main");
            //HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            //Console.WriteLine(response.StatusDescription);
            //using (Stream dataStream = response.GetResponseStream()) 
            //{
            //    using (StreamReader reader = new StreamReader(dataStream))
            //    {
            //        string responseFromServer = reader.ReadToEnd();
            //        Console.WriteLine(responseFromServer);
            //    }
            //}              


            ////reader.Close();
            ////dataStream.Close();
            ////response.Close();
            Console.Read();

        }
        public static Cookie SingIn(string login, string password)
        {
            Cookie token = null;
            string Url = "http://news.permaviat.ru/ajax/login.php";

            Debug.WriteLine($"Выполняем запрос: {Url}");

            HttpWebRequest Request = (HttpWebRequest)WebRequest.Create(Url);
            Request.Method = "POST";
            Request.ContentType = "application/x-www-form-urlencoded";
            Request.CookieContainer = new CookieContainer();

            byte[] Data = Encoding.ASCII.GetBytes($"Login={login}&password={password}");
            Request.ContentLength = Data.Length;

            using (Stream stream = Request.GetRequestStream())
            {
                stream.Write(Data, 0, Data.Length);
            }

            using (HttpWebResponse Response = (HttpWebResponse)Request.GetResponse())
            {
                Debug.WriteLine($"Статус выполнения: {Response.StatusCode}");

                string ResponseFromServer = new StreamReader(Response.GetResponseStream()).ReadToEnd();
                token = Response.Cookies["token"];
            }
            return token;
        }
        public static string GetContent(Cookie token)
        {
            string Content = null;
            string Url = "http://news.permaviat.ru/main";
            Debug.WriteLine($"Выполняем запрос: {Url}");

            HttpWebRequest Request = (HttpWebRequest)WebRequest.Create(Url);
            Request.CookieContainer = new CookieContainer();
            Request.CookieContainer.Add(token);

            using (HttpWebResponse Response = (HttpWebResponse)Request.GetResponse())
            {
                Debug.WriteLine($"Статус выполнения: {Response.StatusCode}");

                Content = new StreamReader(Response.GetResponseStream()).ReadToEnd();

            }
            return Content;
        }
        public static void ParsingHtml(string htmlCode)
        {
            var Html = new HtmlDocument();
            Html.LoadHtml(htmlCode);

            var Document = Html.DocumentNode;
            IEnumerable DivsNews = Document.Descendants(0).Where(n => n.HasClass("news"));
            foreach (HtmlNode DivNews in DivsNews)
            {
                var src = DivNews.ChildNodes[1].GetAttributeValue("src", "нет изображения");

                var name = DivNews.ChildNodes.Count > 3 ? DivNews.ChildNodes[3].InnerHtml : "нет названия";
                var description = DivNews.ChildNodes.Count > 5 ? DivNews.ChildNodes[5].InnerHtml : "нет описания";
                Console.WriteLine(name + "\n" + "Изображение" + src + "\n" + "Описание:" + description + "\n");
            }
        }
    }
}
