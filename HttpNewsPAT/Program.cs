using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Linq;

namespace SimpleNewsParser
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("=== Парсер новостей Lenta.ru ===\n");

            try
            {
                string htmlCode = GetHtmlFromUrl("https://lenta.ru/rubrics/media");

                ParseLentaNews(htmlCode);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }

            Console.WriteLine("\nНажмите любую клавишу для выхода...");
            Console.ReadKey();
        }
        public static string GetHtmlFromUrl(string url)
        {
            Console.WriteLine($"Загружаем страницу: {url}");

            using (var client = new System.Net.Http.HttpClient())
            {
                client.DefaultRequestHeaders.Add("User-Agent",
                    "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");
                var response = client.GetAsync(url).Result;
                response.EnsureSuccessStatusCode();

                string htmlCode = response.Content.ReadAsStringAsync().Result;
                Console.WriteLine($"Страница загружена успешно!\n");
                return htmlCode;
            }
        }

        public static void ParseLentaNews(string htmlCode)
        {
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(htmlCode);

            var newsCards = htmlDoc.DocumentNode
                .SelectNodes("//section[contains(@class, 'rubric-page')]//a[contains(@class, 'card')]");

            if (newsCards == null || !newsCards.Any())
            {
                Console.WriteLine("Новости не найдены. Возможно, изменилась структура сайта.");
                return;
            }

            Console.WriteLine($"Найдено новостей: {newsCards.Count}\n");

            int count = 0;
            foreach (var card in newsCards.Take(10)) 
            {
                count++;
                Console.WriteLine($"Новость #{count}");
                Console.WriteLine(new string('-', 50));

                var titleNode = card.SelectSingleNode(".//h3[contains(@class, 'card-title')]") ??
                               card.SelectSingleNode(".//span[contains(@class, 'card-title')]");
                string title = titleNode?.InnerText?.Trim() ?? "Без заголовка";
                Console.WriteLine($"Заголовок: {title}");
                var descNode = card.SelectSingleNode(".//div[contains(@class, 'card-annotation')]");
                string description = descNode?.InnerText?.Trim() ?? "Без описания";
                if (description.Length > 150)
                    description = description.Substring(0, 150) + "...";
                Console.WriteLine($"Описание: {description}");

                var timeNode = card.SelectSingleNode(".//time[contains(@class, 'card-date')]");
                string date = timeNode?.InnerText?.Trim() ?? "Дата не указана";
                Console.WriteLine($"Дата: {date}");
                string link = "https://lenta.ru" + card.GetAttributeValue("href", "");
                Console.WriteLine($"Ссылка: {link}");

                Console.WriteLine(new string('-', 50) + "\n");
            }
        }
    }
}