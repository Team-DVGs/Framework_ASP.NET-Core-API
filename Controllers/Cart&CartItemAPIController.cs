using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Do_an_mon_hoc.Models;
using System.Linq;
using AutoMapper;
using Do_an_mon_hoc.Dto.Products;
using Do_an_mon_hoc.Models;


namespace Do_an_mon_hoc.Controllers
{
    [Route("api")]
    [ApiController]
    public class CartItemApiController : ControllerBase
    {
        private MiniMarketContext _context { get; }
        private IMapper _mapper { get; }

        private ILogger<CartItemApiController> _logger;

        public CartItemApiController(MiniMarketContext context, IMapper mapper, ILogger<CartItemApiController> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        //read
        [HttpGet]
        [Route("giohang/{cartId}")]
        public async Task<ActionResult<IEnumerable<CartDTO_Get>>> GetCartItems(int cartId)
        {

            var cartItems = await _context.Carts
                .Include(c => c.CartItems).ThenInclude(p => p.Product)
                .Where(p => p.Id == cartId)
                .ToListAsync();

            var convertedCartItems = _mapper.Map<IEnumerable<CartDTO_Get>>(cartItems);

            return Ok(convertedCartItems);
        }





        [HttpPost]
        [Route("sanpham/them")]
        public async Task<ActionResult<CartDTO_Get>> AddProductToCart([FromBody] CartItemDTO_Add cartItemDto)
        {
            try
            {
                // Map the DTO to the entity
                var newCartItem = _mapper.Map<CartItem>(cartItemDto);

                // Add the new product to the context
                await _context.CartItems.AddAsync(newCartItem);

                // Save changes to the database
                await _context.SaveChangesAsync();

                // Retrieve the updated cart including the newly added item
                var updatedCart = await _context.Carts
                    .Include(c => c.CartItems).ThenInclude(p => p.Product)
                    .Where(c => c.Id == newCartItem.CartId)
                    .FirstOrDefaultAsync();

                var cartDto = _mapper.Map<CartDTO_Get>(updatedCart);

                return Ok(cartDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = "error",
                    message = "Failed to add product to cart",
                    error = new
                    {
                        message = ex.Message,
                        // You can include additional details about the error if needed
                    }
                });
            }
        }

        [HttpPut]
        [Route("giohang/capnhat/{id}")]
        public async Task<ActionResult<CartDTO_Get>> UpdateCartItems(int id, [FromBody] CartItemDTO_Update cartItemUpdateDto)
        {

            // Retrieve the existing product from the context
            var existingCartItem = await _context.CartItems
                .FirstOrDefaultAsync(p => p.Id == id);

            if (existingCartItem == null)
            {
                return NotFound("Product not found");
            }

            // Update the existing product with the data from the DTO
            _mapper.Map(cartItemUpdateDto, existingCartItem);

            // Save changes to the database
            await _context.SaveChangesAsync();

            // Map the updated product to the DTO and return it
            var updatedCart = await _context.Carts
                    .Include(c => c.CartItems).ThenInclude(p => p.Product)
                    .Where(c => c.Id == existingCartItem.CartId)
                    .FirstOrDefaultAsync();

            var cartDto = _mapper.Map<CartDTO_Get>(updatedCart);

            return Ok(cartDto);


        }


        [HttpDelete]
        [Route("giohang/xoa/{cartItemId}")]
        public async Task<ActionResult<CartDTO_Get>> DeleteCartItem(int cartItemId)
        {
            try
            {
                // Find the cart item by id
                var cartItem = await _context.CartItems.FindAsync(cartItemId);

                

                if (cartItem == null)
                {
                    return NotFound(new
                    {
                        status = "error",
                        message = "Cart item not found",
                        error = new
                        {
                            // You can include additional details about the error if needed
                        }
                    });
                }
                int? id = cartItem.CartId;

                // Remove the cart item from the context
                _context.CartItems.Remove(cartItem);

                // Save changes to the database
                await _context.SaveChangesAsync();
                var updatedCart = await _context.Carts
                    .Include(c => c.CartItems).ThenInclude(p => p.Product)
                    .Where(c => c.Id == id)
                    .FirstOrDefaultAsync();

                var cartDto = _mapper.Map<CartDTO_Get>(updatedCart);

                return Ok(cartDto);


            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = "error",
                    message = "Failed to delete cart item",
                    error = new
                    {
                        message = ex.Message,
                        // You can include additional details about the error if needed
                    }
                });
            }
        }

        [HttpDelete]
        [Route("giohang/{cartId}/xoa")]
        public async Task<ActionResult<CartDTO_Get>> DeleteAllCartItems(int cartId)
        {
            try
            {
                // Find the cart by id
                var cart = await _context.Carts
                    .Include(c => c.CartItems)
                    .FirstOrDefaultAsync(c => c.Id == cartId);

                if (cart == null)
                {
                    return NotFound(new
                    {
                        status = "error",
                        message = "Cart not found",
                        error = new
                        {
                            // You can include additional details about the error if needed
                        }
                    });
                }

                
                // Remove all cart items from the context
                _context.CartItems.RemoveRange(cart.CartItems);

                // Save changes to the database
                await _context.SaveChangesAsync();

                var updatedCart = await _context.Carts
                    .Include(c => c.CartItems).ThenInclude(p => p.Product)
                    .Where(c => c.Id == cartId)
                    .FirstOrDefaultAsync();

                var cartDto = _mapper.Map<CartDTO_Get>(updatedCart);

                return Ok(cartDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = "error",
                    message = "Failed to delete all cart items",
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
