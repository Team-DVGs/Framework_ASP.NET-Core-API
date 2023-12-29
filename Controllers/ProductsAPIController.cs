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
    public class ProductsApiController : ControllerBase
    {
        private MiniMarketContext _context { get; }
        private IMapper _mapper { get; }

        private ILogger<ProductsApiController> _logger;

        public ProductsApiController(MiniMarketContext context, IMapper mapper, ILogger<ProductsApiController> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        //read
        [HttpGet]
        [Route("sanpham")]

        public async Task<ActionResult<IEnumerable<ProductDto_Get>>> GetProducts()
        {
            Console.WriteLine("Entering GetProducts");

            var products = await _context.Products
                .Include(p => p.Brand)
                .Include(p=> p.Category)
                .ToListAsync();

            var convertedProducts = _mapper.Map<IEnumerable<ProductDto_Get>>(products);


            return Ok(convertedProducts);
        }


        /*
        public ActionResult<IEnumerable<ProductDto_Get>> GetProducts()
        {
            var products = _context.Products
                .Include(p => p.Brand) // Include the Brand entity
                .Select(p => new ProductDto_Get
                {
                    Id = p.Id,
                    Thumbnail = p.Thumbnail,
                    Name = p.Name,
                    Price = p.Price,
                    DiscountPercent = p.DiscountPercent,
                    DiscountPrice = p.DiscountPrice,
                    Description = p.Description,
                    BrandId = p.BrandId,
                    Rating = p.Rating,
                  
                    BrandName = (p.Brand == null ? null : p.Brand.Name )
                    // Map other properties as needed
                })
                .ToList();

            return Ok(products);
        }
        */
        //Lay 5 san pham lien quan
        [HttpGet("sanpham/{productId}/lienquan")]
        public async Task<ActionResult<IEnumerable<ProductDto_Get>>> GetRelatedProducts(int productId)
        {

            var product = await _context.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == productId);

            if (product == null)
            {
                return NotFound(); // Return 404 if the specified product is not found
            }

            var relatedProducts = await _context.Products
            .Include(p => p.Brand)
            .Include(p => p.Category)
            .Where(p => p.CategoryId == product.CategoryId && p.Id != productId)
            .Take(5)
            .ToListAsync();

            var convertedProducts = _mapper.Map<IEnumerable<ProductDto_Get>>(relatedProducts);


            return Ok(convertedProducts);
        }


        //lay san pham theo danh muc
        [HttpGet("danhmuc/{categoryId}")]
        public async Task<ActionResult<IEnumerable<ProductDto_Get>>> GetProductsFromCategory(int categoryId)
        {

            var product = await _context.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .Where(p => p.CategoryId == categoryId)
                .ToListAsync();

            if (product == null)
            {
                return NotFound(); // Return 404 if the specified product is not found
            }


            var convertedProducts = _mapper.Map<IEnumerable<ProductDto_Get>>(product);


            return Ok(convertedProducts);
        }
        //lay chi tiet 1 san pham
        [HttpGet("sanpham/{productId}")]
        public async Task<ActionResult<IEnumerable<ProductDto_GetProductDetail>>> GetDetailProducts(int productId)
        {

            var product = await _context.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .Include(p => p.Galleries)
                .FirstOrDefaultAsync(p => p.Id == productId);

            if (product == null)
            {
                return NotFound(); // Return 404 if the specified product is not found
            }



            var detailProductDto = _mapper.Map<ProductDto_GetProductDetail>(product);


            return Ok(detailProductDto);
        }

        [HttpGet("danhmuc/{id}")]
        public async Task<ActionResult<IEnumerable<ProductDto_Get>>> GetProductsByFilter(
        int id,
        [FromQuery] int page = 0,
        [FromQuery] int categoryId = 1,
        [FromQuery] string sort = "tenaz",
        [FromQuery] int brand = 1,
        [FromQuery] string range = "0-500000")
        {
            try
            {
                var query = _context.Products
                    .Include(p => p.Brand)
                    .Include(p => p.Category)
                    .Where(p => categoryId == 0 || p.CategoryId == categoryId)
                    .Where(p => brand == 0 || p.BrandId == brand);

                // Filter by range locally, after fetching from the database
                var products = await query.ToListAsync();

                products = products
                    .Where(p => IsInRange(p.RegPrice, range))
                    .ToList();

                switch (sort)
                {
                    case "tenaz":
                        products = products.OrderBy(p => p.Name).ToList();
                        break;
                    case "tenza":
                        products = products.OrderByDescending(p => p.Name).ToList();
                        break;
                    case "giathap":
                        products = products.OrderBy(p => p.RegPrice).ToList();
                        break;
                    case "giacao":
                        products = products.OrderByDescending(p => p.RegPrice).ToList();
                        break;
                    default:
                        products = products.OrderBy(p => p.Id).ToList();
                        break;
                }

                var pageSize = 16;
                var skip = page * pageSize;
                var paginatedProducts = products.Skip(skip).Take(pageSize).ToList();

                var productDtos = paginatedProducts.Select(p => _mapper.Map<ProductDto_Get>(p)).ToList();

                return Ok(productDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = "error",
                    message = "Failed to get products",
                    error = new
                    {
                        message = ex.Message,
                        // You can include additional details about the error if needed
                    }
                });
            }
        }

        [HttpGet("sanpham/banchay")]
        public ActionResult<IEnumerable<ProductDto_GetBanChay>> GetBestSellingProducts()
        {
            try
            {
                var responseTypes = new List<ProductDto_GetBanChay>();

                // Define types and queries
                var types = new[] { "Noi bat", "Pho bien", "Hang moi" };
                var queries = new[] { "noibat", "phobien", "hangmoi" };

                for (int i = 0; i < types.Length; i++)
                {
                    var type = types[i];
                    var query = queries[i];

                    var products = _context.Products
                        .OrderBy(p => Guid.NewGuid()) // Shuffle the products randomly
                        .Take(6) // Get 6 random products for each type
                        .ToList();

                    var productDtos = _mapper.Map<IEnumerable<ProductDto_Get>>(products);

                    var responseType = new ProductDto_GetBanChay
                    {
                        type = type,
                        query = query,
                        products = productDtos
                    };

                    responseTypes.Add(responseType);
                }

                return Ok(responseTypes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = "error",
                    message = "Failed to get best-selling products",
                    error = new
                    {
                        message = ex.Message,
                        // You can include additional details about the error if needed
                    }
                });
            }
        }



        [HttpGet("numbersofpages")]
        public async Task<ActionResult<int>> GetNumberOfPages(
        [FromQuery] int categoryId = 0,
        [FromQuery] int brand = 0,
        [FromQuery] string range = "0-50")
        {
            try
            {
                var query = _context.Products
                    .Where(p => categoryId == 0 || p.CategoryId == categoryId)
                    .Where(p => brand == 0 || p.BrandId == brand)
                    .Where(p => IsInRange(p.RegPrice, range));

                var pageSize = 16;
                var totalProducts = await query.CountAsync();
                var totalPages = (int)Math.Ceiling((double)totalProducts / pageSize);

                return Ok(new { n = totalPages });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = "error",
                    message = "Failed to get the number of pages",
                    error = new
                    {
                        message = ex.Message,
                        // You can include additional details about the error if needed
                    }
                });
            }
        }


        [HttpGet("danhmucall/{id}")]
        public async Task<ActionResult<IEnumerable<ProductDto_Get>>> GetAllProductsByFilter(
        int id,
        [FromQuery] string keyword = "*",
        [FromQuery] int page = 0,
        [FromQuery] string sort = "az",
        [FromQuery] string range = "0-50"
        )
        {
            try
            {
                IQueryable<Product> query = _context.Products
                    .Include(p => p.Category)
                    .Include(p=> p.Brand)
                    .Where(p => p.CategoryId == id);

                // Keyword search
                if (keyword != "*")
                {
                    query = query.Where(p => p.Name.Contains(keyword) || p.Description.Contains(keyword));
                }

                // Sorting
                switch (sort)
                {
                    case "az":
                        query = query.OrderBy(p => p.Name);
                        break;
                        // Add other sorting options as needed
                }

                // Range filtering
                if (range != "0-50")
                {
                    var rangeValues = range.Split('-');
                    if (rangeValues.Length == 2 && int.TryParse(rangeValues[0], out int min) && int.TryParse(rangeValues[1], out int max))
                    {
                        query = query.Where(p => p.RegPrice >= min && p.RegPrice < max);
                    }
                }

                // Pagination
                var pageSize = 16;
                var skip = page * pageSize;
                var products = await query.Skip(skip).Take(pageSize).ToListAsync();

                var productDtos = _mapper.Map<List<ProductDto_Get>>(products);

                return Ok(productDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = "error",
                    message = "Failed to get products by category",
                    error = new
                    {
                        message = ex.Message,
                        // You can include additional details about the error if needed
                    }
                });
            }
        }


        [HttpPost]
        [Route("Add")]
        public async Task<IActionResult> AddProduct([FromBody] ProductDto_Add productDto)
        {
            
                // Map the DTO to the entity
                var newProduct = _mapper.Map<Product>(productDto);

                // Add the new product to the context
                await _context.Products.AddAsync(newProduct);

                // Save changes to the database
                await _context.SaveChangesAsync();

                // Map the added product to the DTO and return it
                return Ok("OK");
            
        }

        [HttpPut]
        [Route("Update/{id}")]
        public async Task<ActionResult<ProductDto_Get>> UpdateProduct(int id, [FromBody] ProductDto_Update productDto)
        {
            
                // Retrieve the existing product from the context
                var existingProduct = await _context.Products
                    .Include(p => p.Brand)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (existingProduct == null)
                {
                    return NotFound("Product not found");
                }

                // Update the existing product with the data from the DTO
                _mapper.Map(productDto, existingProduct);

                // Save changes to the database
                await _context.SaveChangesAsync();

                // Map the updated product to the DTO and return it
                var updatedProductDto = _mapper.Map<ProductDto_Get>(existingProduct);
                return Ok(updatedProductDto);
            
            
        }


        [HttpDelete]
        [Route("Delete/{id}")]
        public async Task<ActionResult> DeleteProduct(int id)
        {
            try
            {
                // Retrieve the product to be deleted from the context
                var productToDelete = await _context.Products
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (productToDelete == null)
                {
                    return NotFound("Product not found");
                }

                // Remove the product from the context
                _context.Products.Remove(productToDelete);

                // Save changes to the database
                await _context.SaveChangesAsync();

                return NoContent(); // 204 No Content
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting a product.");
                return StatusCode(500, "Internal server error");
            }
        }


        private bool IsInRange(double? price, string range)
        {
            var rangeValues = range.Split('-');
            if (rangeValues.Length == 2 && int.TryParse(rangeValues[0], out int min) && int.TryParse(rangeValues[1], out int max))
            {
                return price >= min && price < max;
            }
            return false;
        }
    }


}
