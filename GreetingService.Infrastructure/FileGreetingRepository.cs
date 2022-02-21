using GreetingService.Core;
using GreetingService.Core.Entities;
using System.Text.Json;

namespace GreetingService.Infrastructure
{
    public class FileGreetingRepository : IGreetingRepository
    {
        private readonly string _filePath = "greetings.json";
        private readonly JsonSerializerOptions _jsonSerializerOptions = new() { WriteIndented = true, };        

        public FileGreetingRepository(string filePath)
        {
            if (!File.Exists(filePath))
                File.WriteAllText(filePath, "[]");

            _filePath = filePath;
        }

        public async Task CreateAsync(Greeting greeting)
        {
            var content = File.ReadAllText(_filePath);
            var greetings = JsonSerializer.Deserialize<IList<Greeting>>(content);

            if (greetings.Any(x => x.Id == greeting.Id))
                throw new Exception($"Greeting with id: {greeting.Id} already exists");

            greetings.Add(greeting);

            File.WriteAllText(_filePath, JsonSerializer.Serialize(greetings, _jsonSerializerOptions));
        }



        public async Task<Greeting> GetAsync(Guid id)
        {
            var content = File.ReadAllText(_filePath);
            var greetings = JsonSerializer.Deserialize<IList<Greeting>>(content);
            return greetings?.FirstOrDefault(x => x.Id == id);
        }

        public async Task<IEnumerable<Greeting>> GetAsync()
        {
            var content = File.ReadAllText(_filePath);
            var greetings = JsonSerializer.Deserialize<IList<Greeting>>(content);
            return greetings;
        }

        public async Task UpdateAsync(Greeting greeting)
        {
            var content = File.ReadAllText(_filePath);
            var greetings = JsonSerializer.Deserialize<IList<Greeting>>(content);
            var existingGreeting =  greetings.FirstOrDefault(x => x.Id == greeting.Id);

            if (existingGreeting == null)
                throw new Exception($"Greeting with id: {greeting.Id} not found");

            existingGreeting.To = greeting.To;
            existingGreeting.From = greeting.From;
            existingGreeting.Message = greeting.Message;

            File.WriteAllText(_filePath, JsonSerializer.Serialize(greetings, _jsonSerializerOptions));
        }

        public async Task DeleteRecordAsync(Guid id)
        {
            var content = File.ReadAllText(_filePath);
            var greetings = JsonSerializer.Deserialize<IList<Greeting>>(content);
            var greetingToDelete = greetings.FirstOrDefault(x => x.Id == id);

            if (greetingToDelete == null)
                throw new Exception($"Greeting with id: {id} not found");

            greetings.Remove(greetingToDelete);
            File.WriteAllText(_filePath, JsonSerializer.Serialize(greetings, _jsonSerializerOptions));
        }

        public async Task<IEnumerable<Greeting>> GetAsync(string from, string to)
        {
            var greetings = await GetAsync();

            if (!string.IsNullOrWhiteSpace(from))
                greetings = greetings.Where(x => x.From.Equals(from, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrWhiteSpace(to))
                greetings = greetings.Where(x => x.To.Equals(to, StringComparison.OrdinalIgnoreCase));

            return greetings;
        }
    }
}
