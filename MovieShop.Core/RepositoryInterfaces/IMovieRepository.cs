using MovieShop.Core.Entites;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MovieShop.Core.RepositoryInterfaces
{
    public interface IMovieRepository: IAsyncRepository<Movie>
    {
        Task<IEnumerable<Movie>> GetTopGrossingMovies();
    }
}
