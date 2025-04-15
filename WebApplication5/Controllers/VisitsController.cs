using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication5.Dto;
using WebApplication5.Models;
using WebApplication5.Repository;
using WebApplication5.Services;

namespace WebApplication5.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VisitController : ControllerBase
    {
        private readonly IVisitRepository _visitRepository;
        private readonly INotificationRepository _notificationRepository;
        private readonly NotificationService _notificationService;

        public VisitController(
            IVisitRepository visitRepository,
            INotificationRepository notificationRepository,
            NotificationService notificationService)
        {
            _visitRepository = visitRepository;
            _notificationRepository = notificationRepository;
            _notificationService = notificationService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateVisit([FromBody] VisitDto visitDto)
        {
            var visit = new Visit
            {
                CommercialCref = visitDto.CommercialCref,
                TiersId = visitDto.TiersId,
                VisitDate = visitDto.VisitDate,
                IsCompleted = false
            };

            var createdVisit = await _visitRepository.CreateAsync(visit);

            await _notificationService.SendNotificationAsync(
                visit.CommercialCref,
                $"New visit assigned to client ID {visit.TiersId} on {visit.VisitDate:D}"
            );

            return CreatedAtAction(nameof(GetVisit), new { id = createdVisit.Id }, createdVisit);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetVisit(int id)
        {
            var visit = await _visitRepository.GetByIdAsync(id);
            if (visit == null)
                return NotFound();

            return Ok(visit);
        }

        [HttpGet("commercial/{commercialCref}")]
        public async Task<IActionResult> GetVisitsByCommercial(string commercialCref)
        {
            var visits = await _visitRepository.GetByCommercialAsync(commercialCref);
            return Ok(visits);
        }

        [HttpPut("{id}/complete")]
        public async Task<IActionResult> CompleteVisit(int id, [FromBody] string? notes)
        {
            var visit = await _visitRepository.GetByIdAsync(id);
            if (visit == null)
                return NotFound();

            visit.IsCompleted = true;
            visit.Notes = notes;
            await _visitRepository.UpdateAsync(visit);

            return Ok(visit);
        }
    }
}
