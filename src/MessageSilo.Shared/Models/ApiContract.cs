using FluentValidation.Results;
using Microsoft.AspNetCore.Http;

namespace MessageSilo.Shared.Models
{
    public class ApiContract<R> where R : class
    {
        public R? Data { get; set; }

        public List<ValidationFailure> Errors { get; set; } = new List<ValidationFailure>();

        public ApiContract(IHttpContextAccessor hca, int httpStatusCode, R? data = null, List<ValidationFailure>? errors = null)
        {
            Data = data;

            if (errors is not null)
                Errors = errors;

            hca.HttpContext.Response.StatusCode = httpStatusCode;
        }

        public ApiContract()
        {
        }
    }
}
