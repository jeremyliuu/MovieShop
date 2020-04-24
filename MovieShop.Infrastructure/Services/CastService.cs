using MovieShop.Core.Entites;
using MovieShop.Core.RepositoryInterfaces;
using MovieShop.Core.ServiceInterfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MovieShop.Infrastructure.Services
{
    public class CastService: ICastService
    {
        private readonly ICastRepository _castRepository;
        public CastService(ICastRepository castRepository)
        {
            _castRepository = castRepository;
        }

        public async Task<Cast> GetCastDetails(int id)
        {
            return await _castRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Movie>> GetMoviesByCast(int id)
        {
            return await _castRepository.GetMoviesByCast(id);
        }
    }
}
