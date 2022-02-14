
using GreetingService.API.Client;
using System.Text.Json;
using System.Net.Http.Json;
using System.Xml;
using System.Xml.Serialization;
using System.Diagnostics;
using System.Text;

namespace  GreetingServiceAPIClient
{
    public class Program
    {
        private static HttpClient _httpClient = new();

        private const string _getGreetingsCommand = "get greetings";
        private const string _getGreetingCommand = "get greeting ";
        private const string _writeGreetingCommand = "write greeting ";
        private const string _updateGreetingCommand = "update greeting ";
        private const string _exportGreetingsCommand = "export greetings";
        private const string _repeatingCallsCommand = "repeat calls ";
        private static string _from = "Batman";
        private static string _to = "Superman";

        public static async Task Main(string[] args)
        {
            var authParam = Convert.ToBase64String(Encoding.UTF8.GetBytes("keen:summer2022"));
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", authParam);        //Always send this header for all requests from this HttpClient
                                                                                                                                                //_httpClient.BaseAddress = new Uri("http://localhost:5020/");
            _httpClient.BaseAddress = new Uri("https://segunfunstodev.azurewebsites.net/api/");
            //_httpClient.BaseAddress = new Uri("http://segun-api-app.azurewebsites.net/");
            //_httpClient.BaseAddress = new Uri("http://localhost:5284/");

            Console.WriteLine("Welcome to command line Greeting client");
            Console.WriteLine("Enter name of greeting sender:");

            var from = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(from))
                _from = from;

            Console.WriteLine("Enter name of greeting recipient:");
            var to = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(to))
                _to = to;

            while (true)
            {
                Console.WriteLine("Available commands:");
                Console.WriteLine(_getGreetingsCommand);
                Console.WriteLine($"{_getGreetingCommand} [id]");
                Console.WriteLine($"{_writeGreetingCommand} [message]");
                Console.WriteLine($"{_updateGreetingCommand} [id] [message]");
                Console.WriteLine($"{_repeatingCallsCommand} [Number]");

                Console.WriteLine(_exportGreetingsCommand);

                Console.WriteLine("\nWrite command and press [enter] to execute");

                var command = Console.ReadLine();

                if (string.IsNullOrEmpty(command))
                {
                    Console.WriteLine("Command cannot be empty\n");
                    continue;
                }

                if (command.Equals(_getGreetingsCommand, StringComparison.OrdinalIgnoreCase))
                {
                    await GetGreetingsAsync();
                }
                else if (command.StartsWith(_getGreetingCommand, StringComparison.OrdinalIgnoreCase))
                {
                    var idPart = command.Replace(_getGreetingCommand, "");
                    if (Guid.TryParse(idPart, out var id))
                    {
                        await GetGreetingAsync(id);
                    }
                    else
                    {
                        Console.WriteLine($"{idPart} is not a valid GUID\n");
                    }
                }
                else if (command.StartsWith(_writeGreetingCommand, StringComparison.OrdinalIgnoreCase))
                {
                    var message = command.Replace(_writeGreetingCommand, "");
                    await WriteGreetingAsync(message,from,to);
                }
                else if (command.StartsWith(_updateGreetingCommand, StringComparison.OrdinalIgnoreCase))
                {
                    var idAndMessagePart = command.Replace(_updateGreetingCommand, "") ?? "";
                    var idPart = idAndMessagePart.Split(" ").First();
                    var messagePart = idAndMessagePart.Replace(idPart, "").Trim();

                    if (Guid.TryParse(idPart, out var id))
                    {
                        await UpdateGreetingAsync(id, messagePart);
                    }
                    else
                    {
                        Console.WriteLine($"{idPart} is not a valid GUID");
                    }
                }
                else if (command.Equals(_exportGreetingsCommand, StringComparison.OrdinalIgnoreCase))
                {
                    await ExportGreetingsAsync();
                }
                else if(command.StartsWith(_repeatingCallsCommand))
                {
                    var countPart = command.Replace(_repeatingCallsCommand, "");

                    if (int.TryParse(countPart, out var count))
                    {
                        await RepeatCallsAsync(count);
                    }
                    else
                    {
                        Console.WriteLine($"Could not parse {countPart} as int");
                    }
                }
                else
                {
                    Console.WriteLine("Command not recognized\n");
                }
                Console.ReadLine();
            }
            
        }


            private static async Task<IList<Greeting>> GetGreetingsAsync()
        {
            //var URI = "http://localhost:5284/greeting/";
            var Result = await _httpClient.GetAsync("greeting");
            var greetingString = await Result.Content.ReadAsStringAsync();
            var greetings = JsonSerializer.Deserialize<IList<Greeting>>(greetingString);
            return greetings;
        }


        private static async Task<Greeting> GetGreetingAsync(Guid id)
        {
            //var URI = "http://localhost:5284/greeting/"+id;
            var Result = await _httpClient.GetAsync($"greeting/{id}");
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

            var response = await _httpClient.PostAsJsonAsync("greeting", greeting);
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
           
            var response = await _httpClient.PutAsJsonAsync("greeting", greeting);
            Console.WriteLine(response.StatusCode);
            var tt = await response.Content?.ReadAsStringAsync();
            Console.WriteLine(tt);

        }


        private static async Task DeleteGreetingAsync(Guid id)
        {
            var URI = "http://localhost:5284/greeting/" + id;
            var Result = await _httpClient.DeleteAsync($"greeting/{id}");
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
            var response = await _httpClient.GetAsync("/greeting");
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

        private static async Task RepeatCallsAsync(int count)
        {
            var greetings = await GetGreetingsAsync();
            var greeting = greetings.First();

            //init a jobs list
            var jobs = new List<int>();
            for (int i = 0; i < count; i++)
            {
                jobs.Add(i);
            }

            var stopwatch = Stopwatch.StartNew();           //use stopwatch to measure elapsed time just like a real world stopwatch

            //I cheat by running multiple calls in parallel for maximum throughput - we will be limited by our cpu, wifi, internet speeds
            //This is a bit advanced and the syntax is new with lamdas - don't worry if you don't understand all of it.
            //I always copy this from the internet and adapt to my needs
            //Running this in Visual Studio debugger is slow, try running .exe file directly from File Explorer or command line prompt
            await Parallel.ForEachAsync(jobs, new ParallelOptions { MaxDegreeOfParallelism = 50 }, async (job, token) =>
            {
                var start = stopwatch.ElapsedMilliseconds;
                var response = await _httpClient.GetAsync($"greeting/{greeting.id}");
                var end = stopwatch.ElapsedMilliseconds;

                Console.WriteLine($"Response: {response.StatusCode} - Call: {job} - latency: {end - start} ms - rate/s: {job / stopwatch.Elapsed.TotalSeconds}");
            });
        }
    }
}