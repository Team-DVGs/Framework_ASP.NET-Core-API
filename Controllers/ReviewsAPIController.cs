using AutoMapper;
using Do_an_mon_hoc.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Do_an_mon_hoc.Controllers
{
    [Route("api")]
    [ApiController]
    public class ReviewsApiController : ControllerBase
    {
        private MiniMarketContext _context { get; }
        private IMapper _mapper { get; }

        private ILogger<ReviewsApiController> _logger;

        public ReviewsApiController(MiniMarketContext context, IMapper mapper, ILogger<ReviewsApiController> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        //Lay danh gia san pham
        [HttpGet("sanpham/{productId}/danhgia")]
        public async Task<ActionResult<IEnumerable<ReviewDTO_Get>>> GetReviewsForProduct(int productId)
        {
            var reviews = await _context.Reviews
                .Where(r => r.ProductId == productId)
                .Include(c => c.User)
                .ToListAsync();

            if (reviews == null || !reviews.Any())
            {
                return NotFound(); // Return 404 if there are no reviews for the specified product
            }

            // Map the entities to DTOs
            var reviewDtos = _mapper.Map<IEnumerable<ReviewDTO_Get>>(reviews);

            return Ok(reviewDtos);
        }

        //them danh gia
        [HttpPost("sanpham/{productId}/themdanhgia")]
        public async Task<ActionResult<ReviewDTO_Add>> PostReview(int productId, [FromBody] ReviewDTO_Add reviewDto)
        {
            var product = await _context.Products.FindAsync(productId);

            if (product == null)
            {
                return NotFound(new
                {
                    status = "error",
                    message = "Product not found",
                    error = new
                    {
                        // You can provide additional details about the error if needed
                    }
                });
            }

            try
            {
                // Map the DTO to the entity
                var review = _mapper.Map<Review>(reviewDto);
                review.ProductId = productId;

                // Save the review to the database
                _context.Reviews.Add(review);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    status = "success",
                    message = "Review added successfully",
                    data = new
                    {
                        // You can include additional data if needed
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = "error",
                    message = "Failed to add review",
                    error = new
                    {
                        message = ex.Message,
                        // You can include additional details about the error if needed
                    }
                });
            }
        }

    }
}
