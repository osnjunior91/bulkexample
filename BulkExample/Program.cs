using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;

namespace BulkExample
{
    class Program
    {
        private const string Url = "https://free-nba.p.rapidapi.com/games/?per_page=100";
        private static HttpClient _httpClient;
        private static HttpClient HttpClient => _httpClient ?? (_httpClient = new HttpClient());
        private static List<Partida> partidas = new List<Partida>();
        static void Main(string[] args)
        {
            Console.WriteLine("Carregando os dados...");

            HttpClient.DefaultRequestHeaders.Add("x-rapidapi-host", "free-nba.p.rapidapi.com");
            HttpClient.DefaultRequestHeaders.Add("x-rapidapi-key", "********************");
            HttpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            var response = HttpClient.GetAsync(Url).Result;
            var result = JsonConvert.DeserializeObject<Result>(response.Content.ReadAsStringAsync().Result);
            int pagina = 1;
            int total = result.Dados.TotalPages;
            partidas.AddRange(result.Partidas);
            //for (int i = pagina; i <= total; i++)
            //{
            //    response = HttpClient.GetAsync($"{Url}&page={i}").Result;
            //    result = JsonConvert.DeserializeObject<Result>(response.Content.ReadAsStringAsync().Result);
            //    partidas.AddRange(result.Partidas);
            //}

            Console.ReadKey();
        }
    }
}
