using AutoMapper;
using MovieShop.Core.ApiModels.Response;
using MovieShop.Core.Entites;
using MovieShop.Core.Helpers;
using MovieShop.Core.RepositoryInterfaces;
using MovieShop.Core.ServiceInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MovieShop.Infrastructure.Services
{
    public class MovieService: IMovieService
    {
        private readonly IMovieRepository _movieRepository;
        private readonly IMapper _mapper;
        public MovieService(IMovieRepository movieRepository, IMapper mapper)
        {
            _movieRepository = movieRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<Movie>> GetHighestGrossingMovies()
        {
            return await _movieRepository.GetTopGrossingMovies();
        }

        public async Task<IEnumerable<Movie>> GetMovieByGenre(int id)
        {
            return await _movieRepository.GetMoviesByGenreId(id);
        }

        public async Task<MovieDetailsResponseModel> GetMovieByIdAsync(int id)
        {
            var movie = await _movieRepository.GetByIdAsync(id);
            var response = _mapper.Map<MovieDetailsResponseModel>(movie);
            return response;
        }

        public async Task<PagedResultSet<MovieResponseModel>> GetMoviesByPagination(int pageSize = 20, int page = 0, string title = "")
        {
            // check if title parameter is null or empty, if not then construct a Expression with Contains method
            // contains method will transalate to SQL like
            Expression<Func<Movie, bool>> filterExpression = null;
            if (!string.IsNullOrEmpty(title))
            {
                filterExpression = movie => title != null && movie.Title.Contains(title);
            }
            //  // we are gonna call GetPagedData method from repository;
            // pass the order by column, here we are ordering our result by movie title
            // pass the above filter expression
            var pagedMovies = await _movieRepository.GetPagedData(page, pageSize, movies => movies.OrderBy(m => m.Title), filterExpression);
            // once you get movies from repository , convert them in to MovieResponseModel List
            var pagedMovieResponseModel = new List<MovieResponseModel>();
            foreach (var movie in pagedMovies)
            {
                pagedMovieResponseModel.Add(new MovieResponseModel
                {
                    Id = movie.Id,
                    Title = movie.Title,
                    PosterUrl = movie.PosterUrl,
                    ReleaseDate = movie.ReleaseDate.Value
                });
            }
            // Pass the List of MovieResponseModel to our PagedResultSet class so that it can display the data along with page numbers
            var movies = new PagedResultSet<MovieResponseModel>(pagedMovieResponseModel, page, pageSize, pagedMovies.TotalCount);
            return movies;
        }
    }
}
