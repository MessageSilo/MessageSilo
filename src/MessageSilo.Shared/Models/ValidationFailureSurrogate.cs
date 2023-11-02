using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageSilo.Shared.Models
{
    [GenerateSerializer]
    public struct ValidationFailureSurrogate
    {
        [Id(0)]
        public string PropertyName { get; set; }

        [Id(1)]
        public string ErrorMessage { get; set; }

        [Id(2)]
        public object AttemptedValue { get; set; }
    }

    [RegisterConverter]
    public sealed class ValidationFailureSurrogateConverter : IConverter<ValidationFailure, ValidationFailureSurrogate>
    {
        public ValidationFailure ConvertFromSurrogate(
            in ValidationFailureSurrogate surrogate) =>
            new(surrogate.PropertyName, surrogate.ErrorMessage, surrogate.AttemptedValue);

        public ValidationFailureSurrogate ConvertToSurrogate(
            in ValidationFailure value) =>
            new()
            {
                PropertyName = value.PropertyName,
                ErrorMessage = value.ErrorMessage,
                AttemptedValue = value.AttemptedValue
            };
    }
}
