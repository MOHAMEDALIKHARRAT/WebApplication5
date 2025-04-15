using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication5.Dto;
using WebApplication5.Models;
using WebApplication5.Repository;

namespace WebApplication5.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class CommercialTasksController : ControllerBase
    {
        private readonly ICommercialTaskRepository _taskRepository;
        private readonly ILogger<CommercialTasksController> _logger;

        public CommercialTasksController(ICommercialTaskRepository taskRepository, ILogger<CommercialTasksController> logger)
        {
            _taskRepository = taskRepository;
            _logger = logger;
        }

        [HttpPost]
 
        public async Task<IActionResult> CreateTask([FromBody] CommercialTaskDto taskDto)
        {
            var task = new CommercialTask
            {
                CommercialId = taskDto.CommercialId,
                Type = Enum.Parse<TaskType>(taskDto.Type),
                Description = taskDto.Description,
                DueDate = taskDto.DueDate != null ? DateTime.Parse(taskDto.DueDate) : null
            };
            var createdTask = await _taskRepository.CreateTaskAsync(task);
            return CreatedAtAction(nameof(GetTask), new { id = createdTask.Id }, taskDto);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTask(int id)
        {
            var task = await _taskRepository.GetTaskByIdAsync(id);
            if (task == null) return NotFound();
            var taskDto = new CommercialTaskDto
            {
                CommercialId = task.CommercialId,
                Type = task.Type.ToString(),
                Description = task.Description,
                Status = task.Status.ToString(), // This is fine since Status is a property of CommercialTask
                DueDate = task.DueDate?.ToString("yyyy-MM-dd")
            };
            return Ok(taskDto);
        }

        [HttpGet("commercial/{commercialId}")]
        public async Task<IActionResult> GetTasksByCommercial(string commercialId)
        {
            var tasks = await _taskRepository.GetTasksByCommercialAsync(commercialId);
            var taskDtos = tasks.Select(t => new CommercialTaskDto
            {
                CommercialId = t.CommercialId,
                Type = t.Type.ToString(),
                Description = t.Description,
                Status = t.Status.ToString(), // This is fine since Status is a property of CommercialTask
                DueDate = t.DueDate?.ToString("yyyy-MM-dd")
            });
            return Ok(taskDtos);
        }

        [HttpPut("{id}")]

        public async Task<IActionResult> UpdateTask(int id, [FromBody] CommercialTaskUpdateDto updateDto)
        {
            var task = await _taskRepository.GetTaskByIdAsync(id);
            if (task == null) return NotFound();
            task.Status = Enum.Parse<WebApplication5.Models.TaskStatus>(updateDto.Status); // Fully qualified
            var updated = await _taskRepository.UpdateTaskAsync(task);
            return updated ? NoContent() : StatusCode(500);
        }
    }
}