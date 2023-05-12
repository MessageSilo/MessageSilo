using FluentValidation;
using MessageSilo.Shared.DataAccess;
using MessageSilo.Shared.Enums;
using MessageSilo.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Orleans;

namespace MessageSilo.API.Controllers
{
    [Route("api/v1/[controller]")]
    public abstract class CRUDController<DTO, STATE, GRAIN> : MessageSiloControllerBase
        where DTO : Entity
        where STATE : class
        where GRAIN : IEntityGrain<DTO, STATE>
    {
        protected readonly IEntityRepository repo;

        protected readonly IValidator<DTO> validator;

        protected abstract EntityKind GetKind();

        public CRUDController(ILogger<CRUDController<DTO, STATE, GRAIN>> logger, IClusterClient client, IHttpContextAccessor httpContextAccessor, IEntityRepository repo, IValidator<DTO> validator)
            : base(logger, httpContextAccessor, client)
        {
            this.repo = repo;
            this.validator = validator;
        }

        [HttpGet()]
        public async Task<ApiContract<IEnumerable<STATE>>> Index()
        {
            var result = new List<STATE>();

            var entities = await repo.Query(GetKind(), loggedInUserId);

            foreach (var c in entities)
            {
                var entity = client!.GetGrain<GRAIN>(c.Id);
                result.Add(await entity.GetState());
            }

            return new ApiContract<IEnumerable<STATE>>(httpContextAccessor, StatusCodes.Status200OK, data: result);
        }

        [HttpDelete(template: "{name}")]
        public async Task<ApiContract<STATE>> Delete(string name)
        {
            var id = $"{loggedInUserId}|{name}";
            var entity = client!.GetGrain<GRAIN>(id);
            await entity.Delete();
            await repo.Delete(loggedInUserId, new[] { name });

            return new ApiContract<STATE>(httpContextAccessor, StatusCodes.Status200OK);
        }

        [HttpGet(template: "{name}")]
        public async Task<ApiContract<STATE>> Show(string name)
        {
            var id = $"{loggedInUserId}|{name}";
            var entities = await repo.Query(GetKind(), loggedInUserId);

            if (!entities.Any(p => p.Id == id))
            {
                return new ApiContract<STATE>(httpContextAccessor, StatusCodes.Status404NotFound);
            }

            var entity = client!.GetGrain<GRAIN>(id);

            return new ApiContract<STATE>(httpContextAccessor, StatusCodes.Status200OK, data: await entity.GetState());
        }

        [HttpPut(template: "{name}")]
        public async Task<ApiContract<STATE>> Update(string name, [FromBody] DTO dto)
        {
            dto.PartitionKey = loggedInUserId;

            var validationResults = await validator.ValidateAsync(dto);

            if (!validationResults.IsValid)
                return new ApiContract<STATE>(httpContextAccessor, StatusCodes.Status400BadRequest, errors: validationResults.Errors);

            await repo.Add(new[]
            {
                    new Entity()
                    {
                        PartitionKey = loggedInUserId,
                        RowKey = name,
                        Kind = GetKind()
                    }
                });

            var entity = client!.GetGrain<GRAIN>(dto.Id);
            await entity.Update(dto);

            return new ApiContract<STATE>(httpContextAccessor, StatusCodes.Status200OK, data: await entity.GetState());
        }
    }
}
