using AutoMapper;
using MovieShop.Core.ApiModels.Request;
using MovieShop.Core.ApiModels.Response;
using MovieShop.Core.Entites;
using MovieShop.Core.Helpers;
using MovieShop.Core.RepositoryInterfaces;
using MovieShop.Core.ServiceInterfaces;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using MovieShop.Core.Exceptions;
using System.Linq;

namespace MovieShop.Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ICryptoService _cryptoService;
        private readonly IAsyncRepository<Purchase> _purchaseRepository;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAsyncRepository<Favorite> _favoriteRepository;
        private readonly IMovieService _movieService;
        private readonly IAsyncRepository<Review> _reviewRepository;
        public UserService(IUserRepository userRepository, ICryptoService cryptoService, IAsyncRepository<Purchase> purchaseRepository, 
            IMapper mapper, ICurrentUserService currentUserService, IAsyncRepository<Favorite> favoriteRepository, 
            IMovieService movieService, IAsyncRepository<Review> reviewRepository)

        {
            _purchaseRepository = purchaseRepository;
            _userRepository = userRepository;
            _cryptoService = cryptoService;
            _mapper = mapper;
            _currentUserService = currentUserService;
            _favoriteRepository = favoriteRepository;
            _movieService = movieService;
            _reviewRepository = reviewRepository;
        }

        public async Task AddFavorite(FavoriteRequestModel favoriteRequest)
        {
            if (_currentUserService.UserId != favoriteRequest.UserId)
            {
                throw new HttpException(HttpStatusCode.Unauthorized, "You are not Authorized to Add Favorite");
            }

            if (await FavoriteExists(favoriteRequest.UserId, favoriteRequest.MovieId))
            {
                throw new ConflictException("Movie already Favorited");
            }

            var favorite = _mapper.Map<Favorite>(favoriteRequest);
            await _favoriteRepository.AddAsync(favorite);
        }

        public async Task<bool> FavoriteExists(int id, int movieId)
        {
            return await _favoriteRepository.GetExistsAsync(f => f.MovieId == movieId &&
                                                                 f.UserId == id);
        }

        public async Task<UserRegisterResponseModel> CreateUser(UserRegisterRequestModel requestModel)
        {
            // 1. Call GetUserByEmail  with  requestModel.Email to check if the email exists in the User Table or not
            // if user/email exists return Email already exists and throw an Conflict exception
            // if email does not exists then we can proceed in creating the User record
            // 1. var salt =  Genreate a random salt
            // 2. var hashedPassword =  we take requestModel.Password and add Salt from above step and Hash them to generate Unique Hash
            // 3. Save Email, Salt, hashedPassword along with other details that user sent like FirstName, LastName etc
            // 4. return the UserRegisterResponseModel object with newly craeted Id for the User
            var dbUser = await _userRepository.GetUserByEmail(requestModel.Email);
            if (dbUser != null)
            {
                throw new Exception("Email already exists");
            }
            var salt = _cryptoService.CreateSalt();
            var hashedPassword = _cryptoService.HashPassword(requestModel.Password, salt);
            var user = new User
            {
                Email = requestModel.Email,
                Salt = salt,
                HashedPassword = hashedPassword,
                FirstName = requestModel.FirstName,
                LastName = requestModel.LastName
            };
            var createdUser = await _userRepository.AddAsync(user);
            var response = new UserRegisterResponseModel
            {
                Id = createdUser.Id,
                Email = requestModel.Email,
                FirstName = requestModel.FirstName,
                LastName = requestModel.LastName
            };
            return response;
        }

        public async Task<FavoriteResponseModel> GetAllFavoritesForUser(int id)
        {
            if (_currentUserService.UserId != id)
                throw new HttpException(HttpStatusCode.Unauthorized, "You are not Authorized to View Favorites");

            var favoritedMovies = await _favoriteRepository.ListAllWithIncludesAsync(
                f => f.UserId == _currentUserService.UserId,
                f => f.Movie);
            return _mapper.Map<FavoriteResponseModel>(favoritedMovies);
        }

        public async Task<PurchaseResponseModel> GetAllPurchasedMoviesByUser(int id)
        {
            if (_currentUserService.UserId != id)
                throw new HttpException(HttpStatusCode.Unauthorized, "You are not Authorized to View Purchases");

            var purchasedMovies = await _purchaseRepository.ListAllWithIncludesAsync(
                p => p.UserId == _currentUserService.UserId,
                p => p.Movie);
            return _mapper.Map<PurchaseResponseModel>(purchasedMovies);
        }

        public Task<PagedResultSet<User>> GetAllUsersByPagination(int pageSize = 20, int page = 0, string lastName = "")
        {
            throw new NotImplementedException();
        }

        public async Task<User> GetUserByEmail(string email)
        {
            return await _userRepository.GetUserByEmail(email);
        }

        public async Task PurchaseMovie(PurchaseRequestModel purchaseRequest)
        {
            if (_currentUserService.UserId != purchaseRequest.UserId)
                throw new HttpException(HttpStatusCode.Unauthorized, "You are not Authorized to purchase");

            if (await IsMoviePurchased(purchaseRequest))
            {
                throw new ConflictException("Movie already Favorited");
            }
            // Get Movie Price from Movie Table
            var movie = await _movieService.GetMovieByIdAsync(purchaseRequest.MovieId);
            purchaseRequest.TotalPrice = movie.Price;

            var purchase = _mapper.Map<Purchase>(purchaseRequest);
            await _purchaseRepository.AddAsync(purchase);
        }

        public async Task<bool> IsMoviePurchased(PurchaseRequestModel purchaseRequest)
        {
            return await _purchaseRepository.GetExistsAsync(p =>
                p.UserId == purchaseRequest.UserId && p.MovieId == purchaseRequest.MovieId);
        }

        public async Task RemoveFavorite(FavoriteRequestModel favoriteRequest)
        {
            var favorite =
                await _favoriteRepository.ListAsync(f => f.UserId == favoriteRequest.UserId &&
                                                         f.MovieId == favoriteRequest.MovieId);
            await _favoriteRepository.DeleteAsync(favorite.First());
        }

        public async Task<User> ValidateUser(string email, string password)
        {
            // 1 Go to databse and get the whole record for this email, so that the object inlcudes salt and hashedpassword
            var user = await _userRepository.GetUserByEmail(email);
            if (user == null)
            {
                // User did not even registered in our Database
                return null;
            }
            // if  User Registered
            // hash the password with user entered password and database Salt
            // then compare the hashes
            var hashedPassword = _cryptoService.HashPassword(password, user.Salt);
            if (hashedPassword == user.HashedPassword)
            {
                return user;
            }
            else return null;
        }

        public async Task AddMovieReview(ReviewRequestModel reviewRequest)
        {
            if (_currentUserService.UserId != reviewRequest.UserId)
                throw new HttpException(HttpStatusCode.Unauthorized, "You are not Authorized to Review");
            var review = _mapper.Map<Review>(reviewRequest);

            await _reviewRepository.AddAsync(review);
        }

        public async Task UpdateMovieReview(ReviewRequestModel reviewRequest)
        {
            if (_currentUserService.UserId != reviewRequest.UserId)
                throw new HttpException(HttpStatusCode.Unauthorized, "You are not Authorized to Review");
            var review = _mapper.Map<Review>(reviewRequest);

            await _reviewRepository.UpdateAsync(review);
        }

        public async Task DeleteMovieReview(int userId, int movieId)
        {
            if (_currentUserService.UserId != userId)
                throw new HttpException(HttpStatusCode.Unauthorized, "You are not Authorized to Delete Review");
            var review = await _reviewRepository.ListAsync(r => r.UserId == userId && r.MovieId == movieId);
            await _reviewRepository.DeleteAsync(review.First());
        }

        public async Task<ReviewResponseModel> GetAllReviewsByUser(int id)
        {
            if (_currentUserService.UserId != id)
                throw new HttpException(HttpStatusCode.Unauthorized, "You are not Authorized to View Reviews");

            var userReviews = await _reviewRepository.ListAllWithIncludesAsync(r => r.UserId == id, r => r.Movie);
            return _mapper.Map<ReviewResponseModel>(userReviews);
        }
    }
}
