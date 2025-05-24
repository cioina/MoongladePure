﻿using MoongladePure.Caching.Filters;
using MoongladePure.Menus;
using MoongladePure.Web.Attributes;

namespace MoongladePure.Web.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class MenuController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    [TypeFilter(typeof(ClearBlogCache), Arguments = new object[] { CacheDivision.General, "menu" })]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    public async Task<IActionResult> Create(EditMenuRequest request)
    {
        var response = await mediator.Send(new CreateMenuCommand(request));
        return Ok(response);
    }

    [HttpDelete("{id:guid}")]
    [TypeFilter(typeof(ClearBlogCache), Arguments = new object[] { CacheDivision.General, "menu" })]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete([NotEmpty] Guid id)
    {
        await mediator.Send(new DeleteMenuCommand(id));
        return NoContent();
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(EditMenuRequest), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Edit([NotEmpty] Guid id)
    {
        var menu = await mediator.Send(new GetMenuQuery(id));
        if (null == menu) return NotFound();

        var model = new EditMenuRequest
        {
            Id = menu.Id,
            DisplayOrder = menu.DisplayOrder,
            Icon = menu.Icon,
            Title = menu.Title,
            Url = menu.Url,
            IsOpenInNewTab = menu.IsOpenInNewTab,
            SubMenus = menu.SubMenus.Select(p => new EditSubMenuRequest
            {
                Id = p.Id,
                Title = p.Title,
                Url = p.Url,
                IsOpenInNewTab = p.IsOpenInNewTab
            }).ToArray()
        };

        return Ok(model);
    }

    [HttpPut("edit")]
    [TypeFilter(typeof(ClearBlogCache), Arguments = new object[] { CacheDivision.General, "menu" })]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Edit(EditMenuRequest request)
    {
        await mediator.Send(new UpdateMenuCommand(request));
        return NoContent();
    }
}