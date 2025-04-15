using Microsoft.AspNetCore.Mvc;
using WebApplication5.Dto;
using WebApplication5.Models;
using WebApplication5.Repository;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplication5.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuotesController : ControllerBase
    {
        private readonly IQuoteRepository _quoteRepository;
        private readonly IArticleRepository _articleRepository;

        public QuotesController(IQuoteRepository quoteRepository, IArticleRepository articleRepository)
        {
            _quoteRepository = quoteRepository;
            _articleRepository = articleRepository;
        }

        [HttpPost]
        public async Task<IActionResult> CreateQuote([FromBody] QuoteDto quoteDto)
        {
            if (quoteDto.QuoteLines == null || !quoteDto.QuoteLines.Any())
                return BadRequest("At least one quote line is required.");

            var quote = new Quote
            {
                VisitId = quoteDto.VisitId,
                QuoteRef = quoteDto.QuoteRef,
                QuoteDate = quoteDto.QuoteDate,
                QuoteLines = new List<QuoteLine>()
            };

            // Validate stock (no deduction)
            decimal totalAmount = 0;
            var stockIssues = new List<string>();

            foreach (var lineDto in quoteDto.QuoteLines)
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

                var quoteLine = new QuoteLine
                {
                    ArticleId = lineDto.ArticleId,
                    Quantity = lineDto.Quantity,
                    UnitPrice = article.PrixVente
                };
                quote.QuoteLines.Add(quoteLine);
                totalAmount += lineDto.Quantity * quoteLine.UnitPrice;
            }

            if (stockIssues.Any())
                return BadRequest(new { Errors = stockIssues });

            quote.TotalAmount = totalAmount;
            var createdQuote = await _quoteRepository.CreateAsync(quote);
            return CreatedAtAction(nameof(GetQuote), new { id = createdQuote.Id }, createdQuote);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetQuote(int id)
        {
            var quote = await _quoteRepository.GetByIdAsync(id);
            if (quote == null)
                return NotFound();

            return Ok(quote);
        }
    }
}
