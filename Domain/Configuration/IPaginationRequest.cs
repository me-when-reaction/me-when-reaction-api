using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using MeWhenAPI.Domain.Validator;

namespace MeWhenAPI.Domain.Configuration
{
    public class PaginationRequestPlain
    {
        public int PageSize { get; set; } = 10;
        public int CurrentPage { get; set; } = 1;
    }

    public class PaginationRequest<TResponse> : PaginationRequestPlain, IRequest<TResponse> { }

    public class PaginationRequestValidator<TRequest> : AbstractValidator<TRequest>
        where TRequest : PaginationRequestPlain
    {
        public PaginationRequestValidator()
        {
            RuleFor(x => x.PageSize).In([5, 10, 20, 50, 100]);
            RuleFor(x => x.CurrentPage).GreaterThanOrEqualTo(1);
        }
    }
}