using GreetingService.API.Authentication;
using GreetingService.Core;
using GreetingService.Core.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace GreetingService.API.Controllers
{

    [ApiController]
    [BasicAuth]
    [Route("[controller]")]
 
    public class GreetingController : ControllerBase
    {

        private readonly IGreetingRepository _greetingRepository;


        public GreetingController(IGreetingRepository greetingRepository)
        {
            _greetingRepository = greetingRepository;
        }


        

        [HttpGet]
        public async Task<IEnumerable<Greeting>> GetAsync()
        {
            return await _greetingRepository.GetAsync();
        }


        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Greeting))]      
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Greeting>> GetAsync(Guid id)
        {
            try
            {
                return await _greetingRepository.GetAsync(id);
            }
            catch (Exception)
            {

                return NotFound();
            }
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> UpdateAsync(Greeting greeting)
        {
            try
            {
                await _greetingRepository.UpdateAsync(greeting);
                return Accepted();
            }
            catch (Exception)
            {
                Console.WriteLine($"Greeting with id:{greeting.Id} not found");
                return NotFound();
            }
        }


        // 
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult> CreateAsync(Greeting greeting)
        {
            try
            {
                await _greetingRepository.CreateAsync(greeting);
                return Accepted();
            }
            catch (Exception)
            {

                return NotFound();
            }
            
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Guid>> DeleterecordAsync(Guid id)
        {
            try
            {
                await _greetingRepository.DeleteRecordAsync(id);
                return Accepted(id);
            }
            catch (Exception)
            {

                return NotFound(id);
            }
        }
    }
}
