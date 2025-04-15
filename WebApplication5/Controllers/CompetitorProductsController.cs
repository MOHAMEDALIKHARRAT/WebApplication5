using Microsoft.AspNetCore.Mvc;
using WebApplication5.Dto;
using WebApplication5.Models;
using WebApplication5.Repository;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplication5.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompetitorProductsController : ControllerBase
    {
        private readonly ICompetitorProductRepository _competitorProductRepository;

        public CompetitorProductsController(ICompetitorProductRepository competitorProductRepository)
        {
            _competitorProductRepository = competitorProductRepository;
        }

        [HttpPost]
        public async Task<IActionResult> CreateCompetitorProduct([FromBody] CompetitorProductDto productDto)
        {
            var product = new CompetitorProduct
            {
                VisitId = productDto.VisitId,
                ProductName = productDto.ProductName,
                Price = productDto.Price,
                ImageUrl = productDto.ImageUrl,
                Notes = productDto.Notes
            };

            var createdProduct = await _competitorProductRepository.CreateAsync(product);
            return CreatedAtAction(nameof(CreateCompetitorProduct), new { id = createdProduct.Id }, createdProduct);
        }
    }
}
