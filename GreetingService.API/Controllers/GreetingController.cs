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


        

        [HttpGet("basic")]
        public IEnumerable<Greeting > Get()
        {
            return _greetingRepository.Get();
        }


        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Greeting))]      
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<Greeting> Get(Guid id)
        {
            try
            {
                return _greetingRepository.Get(id);
            }
            catch (Exception)
            {

                return NotFound();
            }
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public void Update(Greeting greeting)
        {
            _greetingRepository.Update(greeting);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public void Create(Greeting greeting)
        {
            try
            {
                _greetingRepository.Create(greeting);
            }
            catch (Exception)
            {

                throw;
            }
            
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public void Delete(Guid id)
        {
            try
            {
                _greetingRepository.DeleteRecord(id);
            }
            catch (Exception)
            {

                throw;
            }
        }





        //// GET: GreetingController
        //public ActionResult Index()
        //{
        //    return View();
        //}

        //// GET: GreetingController/Details/5
        //public ActionResult Details(int id)
        //{
        //    return View();
        //}

        //// GET: GreetingController/Create
        //public ActionResult Create()
        //{
        //    return View();
        //}

        // POST: GreetingController/Create
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create(IFormCollection collection)
        //{
        //    try
        //    {
        //        return RedirectToAction(nameof(Index));
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}

        //// GET: GreetingController/Edit/5
        //public ActionResult Edit(int id)
        //{
        //    return View();
        //}

        // POST: GreetingController/Edit/5
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit(int id, IFormCollection collection)
        //{
        //    try
        //    {
        //        return RedirectToAction(nameof(Index));
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}

        // GET: GreetingController/Delete/5
        //public ActionResult Delete(int id)
        //{
        //    return View();
        //}

        // POST: GreetingController/Delete/5
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Delete(int id, IFormCollection collection)
        //{
        //    try
        //    {
        //        return RedirectToAction(nameof(Index));
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}
    }
}
