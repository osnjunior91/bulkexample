using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization.Formatters.Binary;

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
            HttpClient.DefaultRequestHeaders.Add("x-rapidapi-key", "************************");
            HttpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            var response = HttpClient.GetAsync(Url).Result;
            var result = JsonConvert.DeserializeObject<Result>(response.Content.ReadAsStringAsync().Result);
            int pagina = 1;
            int total = result.Dados.TotalPages;
            partidas.AddRange(result.Partidas);

            for (int i = pagina; i <= total; i++)
            {
                response = HttpClient.GetAsync($"{Url}&page={i}").Result;
                result = JsonConvert.DeserializeObject<Result>(response.Content.ReadAsStringAsync().Result);
                partidas.AddRange(result.Partidas);
            }

            List<Partida> partidasBulk = partidas.Select(x => (Partida)x.Clone()).ToList();

            using (var context = new Context())
            {
                Console.WriteLine("\n\n******************** Entity Framework **************************");              
                var salvarEf  = context.SalvaPartidasEntity(partidas);               
                Console.WriteLine($"Foram gastos {salvarEf} segundos para salvar {partidas.Count} registros.");
                var deleteEf = context.RemoverTodosEntity(partidas);
                Console.WriteLine($"Foram gastos {deleteEf} segundos para excluir os registros.");
                Console.WriteLine("\n\n******************** Entity Framework com Bulk **************************");
                var salvarBulk = context.SalvaPartidasBulk(partidasBulk);
                Console.WriteLine($"Foram gastos {salvarBulk} segundos para salvar {partidasBulk.Count} registros.");
                partidasBulk = context.BuscarPartidas();
                var deleteBulk = context.RemoverTodosBulk(partidasBulk);
                Console.WriteLine($"Foram gastos {deleteBulk} segundos para excluir os registros.");
            }
            

            Console.ReadKey();
        }

        

    }
}
