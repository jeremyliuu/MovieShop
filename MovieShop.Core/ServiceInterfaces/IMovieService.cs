using MovieShop.Core.ApiModels.Response;
using MovieShop.Core.Entites;
using MovieShop.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MovieShop.Core.ServiceInterfaces
{
    public interface IMovieService
    {
        Task<IEnumerable<Movie>> GetHighestGrossingMovies();
        Task<MovieDetailsResponseModel> GetMovieByIdAsync(int id);
        Task<IEnumerable<Movie>> GetMovieByGenre(int id);
        Task<PagedResultSet<MovieResponseModel>> GetMoviesByPagination(int pageSize = 20, int page = 0, string title = "");
    }
}
