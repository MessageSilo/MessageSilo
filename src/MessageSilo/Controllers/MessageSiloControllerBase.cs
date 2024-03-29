﻿using MessageSilo.Features.EntityManager;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MessageSilo.Controllers
{
    [Authorize]
    public abstract class MessageSiloControllerBase : ControllerBase
    {
        protected readonly ILogger<MessageSiloControllerBase> logger;

        protected readonly IClusterClient client;

        protected readonly IHttpContextAccessor httpContextAccessor;

        protected readonly string loggedInUserId;

        protected readonly IEntityManagerGrain entityManagerGrain;

        public MessageSiloControllerBase(
            ILogger<MessageSiloControllerBase> logger,
            IHttpContextAccessor httpContextAccessor,
            IClusterClient client)
        {
            this.logger = logger;
            this.client = client;
            this.httpContextAccessor = httpContextAccessor;

            var user = httpContextAccessor?.HttpContext?.User;
            loggedInUserId = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
            this.entityManagerGrain = client.GetGrain<IEntityManagerGrain>(loggedInUserId);
        }
    }
}
