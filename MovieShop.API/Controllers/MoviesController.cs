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
    public class MoviesController : ControllerBase
    {
        private readonly IMovieService _movieService;
        public MoviesController(IMovieService movieService)
        {
            _movieService = movieService;
        }

        // (1) (2) (3) (4)
        // Get Movies by Pagination and can also include search
        // http://localhost:54232/api/movies?pageIndex=1&pagesize=25&title=m 80
        // take Query String Parameters from the URL
        // dbContext.Movies --> get movies 1-25 where title like'%m%' skip(0) take 25
        // Select * from Movies where title like '%m%' offset 0 fetch next rows 25 order by title; Page 1
        // Select * from Movies where title like '%m%' offset 25 fetch next rows 25 order by title; Pge 2
        [HttpGet]
        [Route("")]
        public async Task<ActionResult> GetMoviesByPagination([FromQuery] int pageSize = 20, [FromQuery] int pageIndex = 1, string title = "")
        {
            var movies = await _movieService.GetMoviesByPagination(pageSize, pageIndex, title);
            return Ok(movies);
        }

        [HttpGet]
        [Route("toprevenue")]
        public async Task<IActionResult> GetTopRevenueMovies()
        {
            var movies = await _movieService.GetHighestGrossingMovies();
            return Ok(movies);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetMovieById(int id)
        {
            var movie = await _movieService.GetMovieByIdAsync(id);
            return Ok(movie);
        }

        [HttpGet]
        [Route("genresmovie/{id}")]

        public async Task<IActionResult> GetMovieByGenre(int id)
        {
            var movies = await _movieService.GetMovieByGenre(id);
            return Ok(movies);
        }

    }

}