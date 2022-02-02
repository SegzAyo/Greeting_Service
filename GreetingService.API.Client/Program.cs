
using GreetingService.API.Client;
using System.Text.Json;
using System.Net.Http.Json;
using System.Xml;
using System.Xml.Serialization;

namespace  GreetingServiceAPIClient
{
    public class Program
    {
        private static HttpClient _httpClient = new();



        public static async Task Main(string[] args)
        {
            //var Id = new Guid("3fa85f64-5717-4562-b3fc-2d963f66af46");
            //var greeting = await GetGreetingAsync(Id);
            //Console.WriteLine(greeting.message);



            //var Id2 = new Guid("3fa85f64-5717-4562-b3fc-2d963f66af46"); 
            //        await UpdateGreetingAsync(Id2, "This is post 1");



            //await WriteGreetingAsync("New year greeting", "Joan", "H&M");

            ExportGreetingsAsync();


            //var Id3 = new Guid("1cf715f9-a3e6-4026-aed8-9b4b509f8220");
            //await DeleteGreetingAsync(Id3);


            //var greetings = await GetGreetingsAsync();

            //foreach (var g in greetings)
            //{
            //    Console.WriteLine(g.message);
            //    Console.WriteLine(g.from);
            //    Console.WriteLine(g.to);
            //    Console.WriteLine();

            //}


            Console.WriteLine("Done");
            Console.ReadLine();
        }


        private static async Task<IList<Greeting>> GetGreetingsAsync()
        {
            var URI = "http://localhost:5284/greeting/";
            var Result = await _httpClient.GetAsync(URI);
            var greetingString = await Result.Content.ReadAsStringAsync();
            var greetings = JsonSerializer.Deserialize<IList<Greeting>>(greetingString);
            return greetings;
        }
        private static async Task<Greeting> GetGreetingAsync(Guid id)
        {
            var URI = "http://localhost:5284/greeting/"+id;
            var Result = await _httpClient.GetAsync(URI);
            var greetingString = await Result.Content.ReadAsStringAsync();
            var greeting = JsonSerializer.Deserialize<Greeting>(greetingString);
            return greeting;

        }
        private static async Task WriteGreetingAsync(string message, string NFron, string NTo)
        {
            var URI = "http://localhost:5284/greeting/";
            var greeting = new Greeting
            {
                id = Guid.NewGuid(),
                message = message,
                from = NFron,
                to = NTo
            };

            var response = await _httpClient.PostAsJsonAsync(URI, greeting);
            Console.WriteLine(response.StatusCode);
            var tt = await response.Content?.ReadAsStringAsync();
            Console.WriteLine(tt);


        }
        private static async Task UpdateGreetingAsync(Guid id, string message)
        {
            var URI = "http://localhost:5284/greeting/";
            var greeting = new Greeting
            {
                id = id,
                message = message,
                from = "Joe",
                to = "John"
            };
           
            var response = await _httpClient.PutAsJsonAsync(URI, greeting);
            Console.WriteLine(response.StatusCode);
            var tt = await response.Content?.ReadAsStringAsync();
            Console.WriteLine(tt);

        }

        private static async Task DeleteGreetingAsync(Guid id)
        {
            var URI = "http://localhost:5284/greeting/" + id;
            var Result = await _httpClient.DeleteAsync(URI);
            if (Result.IsSuccessStatusCode)
            {
                Console.WriteLine(Result.StatusCode);
                Console.WriteLine("Greeting Deleted");
            }
            else
            {
                Console.WriteLine("No message matches the id");
                var tt = await Result.Content?.ReadAsStringAsync();
            }
            Console.WriteLine();
            
            //var greetingString = await Result.Content.ReadAsStringAsync();
            //var greeting = JsonSerializer.Deserialize<Greeting>(greetingString);
            

        }

        private static async Task ExportGreetingsAsync()
        {
            var response = await _httpClient.GetAsync("http://localhost:5284/greeting");
            try
            {
                response.EnsureSuccessStatusCode();
                var responseBody = await response.Content.ReadAsStringAsync();
                var greetings = JsonSerializer.Deserialize<List<Greeting>>(responseBody);

                var filename = "greetingExport.xml";
                var xmlWriterSettings = new XmlWriterSettings
                {
                    Indent = true,


                };
                using var xmlWriter = XmlWriter.Create(filename, xmlWriterSettings);
                var serializer = new XmlSerializer(typeof(List<Greeting>));
                serializer.Serialize(xmlWriter, greetings);

                Console.WriteLine($"Exported {greetings.Count()} greetings to {filename}\n");
            }
            catch (Exception)
            {

                Console.WriteLine("");
            }
                                                             
            
        }


        //////var URI = "http://localhost:5284/greeting/";
        //////var greeting = new Greeting
        //////{
        //////    Id = new Guid(Guid.NewGuid().ToString()),
        //////    message = "Hej 1",
        //////    from = "Joe",
        //////    to = "John"

        //////};
        //////var serialized = JsonSerializer.Serialize(greeting);
        //////var content = new StringContent(serialized);
        //////_httpClient.PostAsync(URI, content);
    }
}