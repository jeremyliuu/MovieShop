using Microsoft.EntityFrameworkCore;
using MovieShop.Core.Entites;
using MovieShop.Core.RepositoryInterfaces;
using MovieShop.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MovieShop.Infrastructure.Repositories
{
    public class CastRepository : EfRepository<Cast>, ICastRepository
    {
        public CastRepository(MovieShopDbContext dbContext): base(dbContext)
        {

        }

        public override async Task<Cast> GetByIdAsync(int id)
        {
            var cast = await _dbContext.Casts.Where(c => c.Id == id).FirstOrDefaultAsync();
            return cast;
        }

        public async Task<IEnumerable<Movie>> GetMoviesByCast(int id)
        {
            var movies = await _dbContext.MovieCasts.Where(mc => mc.CastId == id).Select(mc => mc.Movie).ToListAsync();
            return movies;
        }
    }
}
