using MovieShop.Core.Entites;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MovieShop.Core.ServiceInterfaces
{
    public interface IMovieService
    {
        Task<IEnumerable<Movie>> GetHighestGrossingMovies();
        Task<Movie> GetMovieByIdAsync(int id);
    }
}
