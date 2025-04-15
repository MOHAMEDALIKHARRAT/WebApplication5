using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication5.Data;
using WebApplication5.Dto;
using WebApplication5.Models;
using WebApplication5.Repository;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplication5.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IArticleRepository _articleRepository;
        private readonly AppDbContext _context;

        public OrdersController(
            IOrderRepository orderRepository,
            IArticleRepository articleRepository,
            AppDbContext context)
        {
            _orderRepository = orderRepository;
            _articleRepository = articleRepository;
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] OrderDto orderDto)
        {
            if (orderDto.OrderLines == null || !orderDto.OrderLines.Any())
                return BadRequest("At least one order line is required.");

            var order = new Order
            {
                VisitId = orderDto.VisitId,
                OrderRef = orderDto.OrderRef,
                OrderDate = orderDto.OrderDate,
                OrderLines = new List<OrderLine>()
            };

            // Validate stock and prepare order lines
            decimal totalAmount = 0;
            var stockIssues = new List<string>();
            var articlesToUpdate = new List<Article>();

            foreach (var lineDto in orderDto.OrderLines)
            {
                if (lineDto.Quantity <= 0)
                {
                    stockIssues.Add($"Quantity must be positive for Article ID {lineDto.ArticleId}");
                    continue;
                }

                var article = await _articleRepository.GetByIdAsync(lineDto.ArticleId);
                if (article == null)
                {
                    stockIssues.Add($"Article ID {lineDto.ArticleId} not found");
                    continue;
                }

                if (lineDto.Quantity > article.StockQuantity)
                {
                    stockIssues.Add($"Insufficient stock for Article ID {lineDto.ArticleId}: requested {lineDto.Quantity}, available {article.StockQuantity}");
                    continue;
                }

                var orderLine = new OrderLine
                {
                    ArticleId = lineDto.ArticleId,
                    Quantity = lineDto.Quantity,
                    UnitPrice = article.PrixVente
                };
                order.OrderLines.Add(orderLine);
                totalAmount += lineDto.Quantity * orderLine.UnitPrice;

                // Prepare stock deduction
                article.StockQuantity -= lineDto.Quantity;
                articlesToUpdate.Add(article);
            }

            if (stockIssues.Any())
                return BadRequest(new { Errors = stockIssues });

            order.TotalAmount = totalAmount;

            // Save order and update stock in a transaction
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                await _orderRepository.CreateAsync(order);
                foreach (var article in articlesToUpdate)
                {
                    _context.Articles.Update(article);
                }
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                return StatusCode(500, "An error occurred while processing the order.");
            }

            return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, order);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrder(int id)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null)
                return NotFound();

            return Ok(order);
        }
    }
}
