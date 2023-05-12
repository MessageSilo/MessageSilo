using FluentValidation;
using MessageSilo.Features.Target;
using MessageSilo.Shared.DataAccess;
using MessageSilo.Shared.Enums;
using MessageSilo.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Orleans;

namespace MessageSilo.API.Controllers
{
    public class TargetsController : CRUDController<TargetDTO, TargetDTO, ITargetGrain>
    {
        protected override EntityKind GetKind() => EntityKind.Target;

        public TargetsController(ILogger<CRUDController<TargetDTO, TargetDTO, ITargetGrain>> logger, IClusterClient client, IHttpContextAccessor httpContextAccessor, IEntityRepository repo, IValidator<TargetDTO> validator) : base(logger, client, httpContextAccessor, repo, validator)
        {
        }
    }
}
