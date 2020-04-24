using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MovieShop.Core.ServiceInterfaces;

namespace MovieShop.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CastsController : ControllerBase
    {
        private readonly ICastService _castService;
        public CastsController(ICastService castService)
        {
            _castService = castService;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetCastDetailsById(int id)
        {
            var cast = await _castService.GetCastDetails(id);
            return Ok(cast);
        }

        [HttpGet]
        [Route("moviebycast/{id}")]
        public async Task<IActionResult> GetMoviesByCastId(int id)
        {
            var movies = await _castService.GetMoviesByCast(id);
            return Ok(movies);
        }
    }
}