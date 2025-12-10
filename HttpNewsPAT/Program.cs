using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace HttpNewsPAT
{
    class Program
    {
        static void Main(string[] args)
        {
            // Создаём запрос для получения данных на странице
            WebRequest request = WebRequest.Create("http://news.permaviat.ru/main");
            // Выполняем запрос, записывая результат в переменную response
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            // Выводим статус ответа в консоль
            Console.WriteLine(response.StatusDescription);
            // Создаём поток для чтения данных ответа
            Stream dataStream = response.GetResponseStream();
            // Инициализируем поток для чтения данных
            StreamReader reader = new StreamReader(dataStream);
            // Читаем ответ
            string responseFromServer = reader.ReadToEnd();
            // Вводим ответ в консоль
            Console.WriteLine(responseFromServer);
            // Закрываем потоки и соединение
            reader.Close();
            dataStream.Close();
            response.Close();
            Console.Read();
        }

        public static void ParsingHtml(string htmlCode)
        {
            var Html = new HtmlDocument();
            Html.LoadHtml(htmlCode);

            var Document = Html.DocumentNode;
            IEnumerable<HtmlNode> DivNews = Document.Descendants("div").Where(x => x.HasClass("news"));

            foreach (var DivNew in DivNews)
            {
                var src = DivNew.ChildNodes[1].GetAttributeValue("src", "none");
                var name = DivNew.ChildNodes[3].InnerHtml;
                var description = DivNew.ChildNodes[5].InnerHtml;

                Console.WriteLine($"{name} \nИзображение: {src} \nОписание: {description}");
            }
        }
        public static string GetContent(Cookie token)
        {
            string Content = null;
            string url = "https://news.permaviat.ru/main";
            Debug.WriteLine($"Выполняем запрос: {url}");

            HttpWebRequest Request = (HttpWebRequest)WebRequest.Create(url);
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
            var html = new HtmlDocument();
            html.LoadHtml(htmlCode);

            var Document = html.DocumentNode;
            IEnumerable<HtmlNode> DivsNews = Document.Descendants("div").Where(n => n.HasClass("news"));

            foreach (HtmlNode DivNews in DivsNews)
            {
                var src = DivNews.ChildNodes[1].GetAttributeValue("src", "none");
                var name = DivNews.ChildNodes[3].InnerText;
                var description = DivNews.ChildNodes[5].InnerText;

                Console.WriteLine(name + "\n" + "Изображение: " + src + "\n" + "Описание: " + description + "\n");
            }
        }
    }
}
