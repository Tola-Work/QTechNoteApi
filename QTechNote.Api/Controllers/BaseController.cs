using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QTechNote.Api.Logic;
using QTechNote.Models.DTOs;

namespace QTechNote.Api.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
[Authorize]
public abstract class BaseController<TEntity, TLogic> : ControllerBase
    where TEntity : class
    where TLogic : BaseLogic<TEntity>
{
    protected readonly TLogic _logic;

    protected BaseController(TLogic logic)
    {
        _logic = logic;
    }

    [HttpGet]
    public virtual async Task<ActionResult<PaginationResponseDto<TEntity>>> GetAll([FromQuery] PaginationDto pagination)
    {
        var entities = await _logic.GetAllAsync();
        return Ok(entities);
    }

    [HttpGet("{id}")]
    public virtual async Task<ActionResult<TEntity>> GetById(int id)
    {
        var entity = await _logic.GetByIdAsync(id);
        if (entity == null) return NotFound();
        return Ok(entity);
    }

    [HttpPost]
    public virtual async Task<ActionResult<TEntity>> Create([FromBody] TEntity entity)
    {
        var created = await _logic.CreateAsync(entity);
        return CreatedAtAction(nameof(GetById), new { id = GetEntityId(created) }, created);
    }

    [HttpPut("{id}")]
    public virtual async Task<ActionResult<TEntity>> Update(int id, TEntity entity)
    {
        var updated = await _logic.UpdateAsync(id, entity);
        if (updated == null) return NotFound();
        return Ok(updated);
    }

    [HttpDelete("{id}")]
    public virtual async Task<ActionResult> Delete(int id)
    {
        var result = await _logic.DeleteAsync(id);
        if (!result) return NotFound();
        return NoContent();
    }

    protected abstract int GetEntityId(TEntity entity);
}