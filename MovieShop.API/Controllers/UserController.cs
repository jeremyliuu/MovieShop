using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MovieShop.Core.ApiModels.Request;
using MovieShop.Core.ServiceInterfaces;

namespace MovieShop.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        // Methods which are secured and should be presented with JWT to process that method
        // 1. Purchase a Movie
        // 2. Add Favorite
        // 3. Delete a Favorite
        // 4. Add a Review
        // 5. Update/Delete a a review
        // 6. Get all movies Purchased by user
        // 7. Get all movies FAvorited by user
        // 8. Get all reviews done by  user
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }
        [Authorize]
        [HttpGet]
        [Route("{id:int}/purchases")]
        // api/user/1882/purchases
        public async Task<ActionResult> GetMoviesPurchasedByUser(int id)
        {
            var purchasedMovies = await _userService.GetAllPurchasedMoviesByUser(id);
            return Ok(purchasedMovies);
            //return Ok("successfully called Purchases ");
        }

        [Authorize]
        [HttpGet]
        [Route("{id:int}/favorites")]
        public async Task<ActionResult> GetFavoriteMovies(int id)
        {
            var favoritedMovies = await _userService.GetAllFavoritesForUser(id);
            return Ok(favoritedMovies);
        }

        [Authorize]
        [HttpPost("favorite")]
        public async Task<ActionResult> CreateFavorite([FromBody] FavoriteRequestModel favoriteRequest)
        {
            await _userService.AddFavorite(favoriteRequest);
            return Ok();
        }

        [Authorize]
        [HttpPost("unfavorite")]
        public async Task<ActionResult> DeleteFavorite([FromBody] FavoriteRequestModel favoriteRequest)
        {
            await _userService.RemoveFavorite(favoriteRequest);
            return Ok();
        }

        [Authorize]
        [HttpPost("purchase")]
        public async Task<ActionResult> CreatePurchase([FromBody] PurchaseRequestModel purchaseRequest)
        {
            await _userService.PurchaseMovie(purchaseRequest);
            return Ok();
        }

        [Authorize]
        [HttpPost("review")]
        public async Task<ActionResult> CreateReview([FromBody] ReviewRequestModel reviewRequest)
        {
            await _userService.AddMovieReview(reviewRequest);
            return Ok();
        }

        [Authorize]
        [HttpPut("review")]
        public async Task<ActionResult> UpdateReview([FromBody] ReviewRequestModel reviewRequest)
        {
            await _userService.UpdateMovieReview(reviewRequest);
            return Ok();
        }

        [Authorize]
        [HttpDelete("{userId:int}/movie/{movieId:int}")]
        public async Task<ActionResult> DeleteReview(int userId, int movieId)
        {
            await _userService.DeleteMovieReview(userId, movieId);
            return NoContent();
        }

        [Authorize]
        [HttpGet("{id:int}/reviews")]
        public async Task<ActionResult> GetUserReviewedMoviesAsync(int id)
        {
            var userMovies = await _userService.GetAllReviewsByUser(id);
            return Ok(userMovies);
        }
    }
}