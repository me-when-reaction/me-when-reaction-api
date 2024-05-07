using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;

namespace MeWhen.Service.Pipe
{
    public class ValidatorPipeline<TRequest, TResponse>(IValidator<TRequest> Validator) : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest
    {
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var validate = await Validator.ValidateAsync(request, cancellationToken);
            if (!validate.IsValid) throw new ValidationException(validate.Errors);

            return await next();
        }
    }
}