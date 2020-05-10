using AutoMapper;
using MovieShop.Core.ApiModels.Request;
using MovieShop.Core.ApiModels.Response;
using MovieShop.Core.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MovieShop.Core.MappingProfiles
{
    public class MoviesMappingProfile: Profile
    {
        public MoviesMappingProfile()
        {
            CreateMap<IEnumerable<Purchase>, PurchaseResponseModel>()
               .ForMember(p => p.PurchasedMovies, opt => opt.MapFrom(src => GetPurchasedMovies(src)))
               .ForMember(p => p.UserId, opt => opt.MapFrom(src => src.FirstOrDefault().UserId));

            CreateMap<IEnumerable<Favorite>, FavoriteResponseModel>()
               .ForMember(p => p.FavoriteMovies, opt => opt.MapFrom(src => GetFavoriteMovies(src)))
               .ForMember(p => p.UserId, opt => opt.MapFrom(src => src.FirstOrDefault().UserId));

            CreateMap<IEnumerable<Review>, ReviewResponseModel>()
                .ForMember(r => r.MovieReviews, opt => opt.MapFrom(src => GetUserReviewedMovies(src)))
                .ForMember(r => r.UserId, opt => opt.MapFrom(src => src.FirstOrDefault().UserId));

            CreateMap<Movie, MovieDetailsResponseModel>()
               .ForMember(md => md.Casts, opt => opt.MapFrom(src => GetCasts(src.MovieCasts)))
               .ForMember(md => md.Genres, opt => opt.MapFrom(src => GetMovieGenres(src.MovieGenres)));

            CreateMap<FavoriteRequestModel, Favorite>();

            CreateMap<PurchaseRequestModel, Purchase>();

            CreateMap<ReviewRequestModel, Review>();

        }

        private List<Genre> GetMovieGenres(IEnumerable<MovieGenre> srcGenres)
        {
            var movieGenres = new List<Genre>();
            foreach (var genre in srcGenres)
            {
                movieGenres.Add(new Genre { Id = genre.GenreId, Name = genre.Genre.Name });
            }

            return movieGenres;
        }

        private List<PurchaseResponseModel.PurchasedMovieResponseModel> GetPurchasedMovies(
            IEnumerable<Purchase> purchases)
        {
            var purchaseResponse = new PurchaseResponseModel
            {
                PurchasedMovies = new List<PurchaseResponseModel.PurchasedMovieResponseModel>()
            };
            foreach (var purchase in purchases)
                purchaseResponse.PurchasedMovies.Add(new PurchaseResponseModel.PurchasedMovieResponseModel
                {
                    PosterUrl = purchase.Movie.PosterUrl,
                    PurchaseDateTime = purchase.PurchaseDateTime,
                    Id = purchase.MovieId,
                    Title = purchase.Movie.Title
                });

            return purchaseResponse.PurchasedMovies;
        }

        private List<FavoriteResponseModel.FavoriteMovieResponseModel> GetFavoriteMovies(
            IEnumerable<Favorite> favorites)
        {
            var favoriteResponse = new FavoriteResponseModel
            {
                FavoriteMovies = new List<FavoriteResponseModel.FavoriteMovieResponseModel>()
            };
            foreach (var favorite in favorites)
                favoriteResponse.FavoriteMovies.Add(new FavoriteResponseModel.FavoriteMovieResponseModel
                {
                    PosterUrl = favorite.Movie.PosterUrl,
                    Id = favorite.MovieId,
                    Title = favorite.Movie.Title
                });

            return favoriteResponse.FavoriteMovies;
        }

        private List<ReviewMovieResponseModel> GetUserReviewedMovies(IEnumerable<Review> reviews)
        {
            var reviewResponse = new ReviewResponseModel { MovieReviews = new List<ReviewMovieResponseModel>() };

            foreach (var review in reviews)
            {
                reviewResponse.MovieReviews.Add(new ReviewMovieResponseModel
                {
                    MovieId = review.MovieId,
                    Rating = review.Rating,
                    UserId = review.UserId,
                    ReviewText = review.ReviewText
                });
            }

            return reviewResponse.MovieReviews;
        }

        private static List<MovieDetailsResponseModel.CastResponseModel> GetCasts(IEnumerable<MovieCast> srcMovieCasts)
        {
            var movieCast = new List<MovieDetailsResponseModel.CastResponseModel>();
            foreach (var cast in srcMovieCasts)
                movieCast.Add(new MovieDetailsResponseModel.CastResponseModel
                {
                    Id = cast.CastId,
                    Gender = cast.Cast.Gender,
                    Name = cast.Cast.Name,
                    ProfilePath = cast.Cast.ProfilePath,
                    TmdbUrl = cast.Cast.TmdbUrl,
                    Character = cast.Character
                });

            return movieCast;
        }
    }
}
