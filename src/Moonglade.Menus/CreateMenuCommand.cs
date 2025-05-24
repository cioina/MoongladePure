﻿using MediatR;
using MoongladePure.Data.Entities;
using MoongladePure.Data.Infrastructure;

namespace MoongladePure.Menus;

public record CreateMenuCommand(EditMenuRequest Payload) : IRequest<Guid>;

public class CreateMenuCommandHandler(IRepository<MenuEntity> repo) : IRequestHandler<CreateMenuCommand, Guid>
{
    public async Task<Guid> Handle(CreateMenuCommand request, CancellationToken ct)
    {
        var uid = Guid.NewGuid();
        var menu = new MenuEntity
        {
            Id = uid,
            Title = request.Payload.Title.Trim(),
            DisplayOrder = request.Payload.DisplayOrder.GetValueOrDefault(),
            Icon = request.Payload.Icon,
            Url = request.Payload.Url,
            IsOpenInNewTab = request.Payload.IsOpenInNewTab
        };

        if (request.Payload.SubMenus is { Length: > 0 })
        {
            var sms = request.Payload.SubMenus.Select(p => new SubMenuEntity
            {
                Id = Guid.NewGuid(),
                IsOpenInNewTab = p.IsOpenInNewTab,
                Title = p.Title,
                Url = p.Url,
                MenuId = uid
            });

            menu.SubMenus = sms.ToList();
        }

        await repo.AddAsync(menu, ct);
        return uid;
    }
}