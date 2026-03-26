using CoachManagement_Api.Entity;
using CoachManagement_Api.Services.interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoachManagement_Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FormationController : ControllerBase
{
    private readonly IFormationService _service;
    public FormationController(IFormationService service)=>_service=service;
    [HttpGet] public async Task<IActionResult> GetAll()=>Ok(await _service.GetAllAsync());
    [HttpGet("{id:int}")] public async Task<IActionResult> GetById(int id){var row=await _service.GetByIdAsync(id);return row==null?NotFound():Ok(row);}    
    [HttpPost] public async Task<IActionResult> Create([FromBody] Formation body){if(!ModelState.IsValid)return BadRequest(ModelState);var created=await _service.CreateAsync(body);return CreatedAtAction(nameof(GetById),new{id=created!.id_formations},created);}    
    [HttpPut("{id:int}")] public async Task<IActionResult> Update(int id,[FromBody] Formation body){if(!ModelState.IsValid)return BadRequest(ModelState);var updated=await _service.UpdateAsync(id,body);return updated==null?NotFound():Ok(updated);}    
    [HttpDelete("{id:int}")] public async Task<IActionResult> Delete(int id){return await _service.DeleteAsync(id)?NoContent():NotFound();}
}
