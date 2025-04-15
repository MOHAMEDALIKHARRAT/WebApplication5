using Microsoft.AspNetCore.Mvc;
using WebApplication5.Dto;
using WebApplication5.Models;
using WebApplication5.Repository;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplication5.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecoveriesController : ControllerBase
    {
        private readonly IRecoveryRepository _recoveryRepository;

        public RecoveriesController(IRecoveryRepository recoveryRepository)
        {
            _recoveryRepository = recoveryRepository;
        }

        [HttpPost]
        public async Task<IActionResult> CreateRecovery([FromBody] RecoveryDto recoveryDto)
        {
            var recovery = new Recovery
            {
                VisitId = recoveryDto.VisitId,
                AmountCollected = recoveryDto.AmountCollected,
                CollectionDate = recoveryDto.CollectionDate,
                Notes = recoveryDto.Notes
            };

            var createdRecovery = await _recoveryRepository.CreateAsync(recovery);
            return CreatedAtAction(nameof(CreateRecovery), new { id = createdRecovery.Id }, createdRecovery);
        }
    }
}
