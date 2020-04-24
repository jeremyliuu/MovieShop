using MovieShop.Core.Entites;
using MovieShop.Core.RepositoryInterfaces;
using MovieShop.Core.ServiceInterfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MovieShop.Infrastructure.Services
{
    public class MovieService: IMovieService
    {
        private readonly IMovieRepository _movieRepository;
        public MovieService(IMovieRepository movieRepository)
        {
            _movieRepository = movieRepository;
        }

        public async Task<IEnumerable<Movie>> GetHighestGrossingMovies()
        {
            return await _movieRepository.GetTopGrossingMovies();
        }

        public async Task<Movie> GetMovieByIdAsync(int id)
        {
            return await _movieRepository.GetByIdAsync(id);
        }
    }
}
