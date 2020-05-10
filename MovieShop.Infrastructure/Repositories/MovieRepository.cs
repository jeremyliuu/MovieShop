using Microsoft.EntityFrameworkCore;
using MovieShop.Core.ApiModels.Response;
using MovieShop.Core.Entites;
using MovieShop.Core.RepositoryInterfaces;
using MovieShop.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieShop.Infrastructure.Repositories
{
    public class MovieRepository : EfRepository<Movie>, IMovieRepository
    {
        public MovieRepository(MovieShopDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<IEnumerable<Movie>> GetTopGrossingMovies()
        {
            return await _dbContext.Movies.OrderByDescending(m => m.Revenue).Take(20).ToListAsync();
        }

        public override async Task<Movie> GetByIdAsync(int id)
        {
            var movie = await _dbContext.Movies
                                       .Include(m => m.MovieCasts).ThenInclude(m => m.Cast).Include(m => m.MovieGenres)
                                       .ThenInclude(m => m.Genre)
                                       .FirstOrDefaultAsync(m => m.Id == id);
            if (movie == null) return null;
            var movieRating = await _dbContext.Reviews.Where(r => r.MovieId == id).AverageAsync(r => r.Rating);
            if (movieRating > 0) movie.Rating = movieRating;
            
            return movie;
        }

        public async Task<IEnumerable<Movie>> GetMoviesByGenreId(int id)
        {
            var movies = await _dbContext.MovieGenres.Where(mg => mg.GenreId == id).Select(mg => mg.Movie).ToListAsync();
            return movies;
        }
    }
}
