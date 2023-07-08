﻿using FluentValidation;
using FluentValidation.Results;
using MessageSilo.Features.Connection;
using MessageSilo.Features.EntityManager;
using MessageSilo.Shared.DataAccess;
using MessageSilo.Shared.Enums;
using MessageSilo.Shared.Models;
using Orleans;
using System.Security.Claims;

namespace MessageSilo.API.Controllers
{
    public class ConnectionsController : CRUDController<ConnectionSettingsDTO, ConnectionState, IConnectionGrain>
    {
        protected override EntityKind GetKind() => EntityKind.Connection;

        public ConnectionsController(
            ILogger<CRUDController<ConnectionSettingsDTO, ConnectionState, IConnectionGrain>> logger,
            IClusterClient client,
            IHttpContextAccessor httpContextAccessor,
            IEntityRepository repo) :
            base(logger, client, httpContextAccessor, repo)
        {
        }

        protected override Task<List<ValidationFailure>?> update(ConnectionSettingsDTO dto)
        {
            var user = httpContextAccessor?.HttpContext?.User;
            var role = user?.FindFirst(ClaimTypes.Role)?.Value;

            if (role is not null && role == "obapp-user")
                dto.IsTemporary = true;

            return base.update(dto);
        }
    }
}
