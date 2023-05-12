using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageSilo.Shared.Models
{
    public class ApiContract<R> where R : class
    {
        public R? Data { get; set; }

        public List<ValidationFailure>? Errors { get; set; } = new List<ValidationFailure>();

        public ApiContract(IHttpContextAccessor hca, int httpStatusCode, R? data = null, List<ValidationFailure>? errors = null)
        {
            Data = data;
            Errors = errors;

            hca.HttpContext.Response.StatusCode = httpStatusCode;
        }

        public ApiContract()
        {
        }
    }
}
