using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Covid.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Covid.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CovidController : ControllerBase
    {
        private readonly Models.CovidService _service;

        public CovidController(Models.CovidService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> SaveCovid(Models.Covid covid)
        {
            await _service.SaveCovid(covid);
            IQueryable<Models.Covid> covidList = _service.GetList();
            return Ok(covidList);
        }
        [HttpGet]
        public IActionResult InitializeCovid()
        {
            Random rnd = new Random();
            Enumerable.Range(1, 10).ToList().ForEach(async x =>
            {
                foreach (ECity item in Enum.GetValues(typeof(ECity)))
                {
                    var newCovid = new Models.Covid
                    {
                        City = item,
                        Count = rnd.Next(10, 1000),
                        CovidDate = DateTime.Now.AddDays(x),
                    };
                    _service.SaveCovid(newCovid).Wait();//await metodu 
                    System.Threading.Thread.Sleep(500);//0.5 sn  bekle

                }
            });
            return Ok("Covid 19 Dataları Veritabanına Kaydedildi.");

        }
    }
}
