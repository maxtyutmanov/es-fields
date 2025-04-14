using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WorkflowApi.Models;
using WorkflowApi.Services;

namespace WorkflowApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WorkflowController : ControllerBase
    {
        private readonly IWorkflowService _workflowService;

        public WorkflowController(IWorkflowService workflowService)
        {
            _workflowService = workflowService;
        }

        [HttpPost]
        public async Task<ActionResult<Workflow>> Create([FromBody] Workflow workflow)
        {
            try
            {
                var result = await _workflowService.CreateAsync(workflow);
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Workflow>> GetById(string id)
        {
            try
            {
                var workflow = await _workflowService.GetByIdAsync(id);
                if (workflow == null)
                {
                    return NotFound();
                }
                return Ok(workflow);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Workflow>>> GetAll()
        {
            try
            {
                var workflows = await _workflowService.GetAllAsync();
                return Ok(workflows);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Workflow>> Update(string id, [FromBody] Workflow workflow)
        {
            try
            {
                var result = await _workflowService.UpdateAsync(id, workflow);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            try
            {
                var result = await _workflowService.DeleteAsync(id);
                if (!result)
                {
                    return NotFound();
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
} 