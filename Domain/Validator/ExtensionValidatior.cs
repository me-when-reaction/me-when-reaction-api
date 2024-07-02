using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace MeWhenAPI.Domain.Validator
{
    public static class ExtensionValidatior
    {
        public static IRuleBuilderOptions<T, TElement> In<T, TElement>(this IRuleBuilder<T, TElement> ruleBuilder, ICollection<TElement> allowed) =>
            ruleBuilder
                .Must(allowed.Contains)
                .WithMessage("Allowed value for {PropertyName} is only " + $"{string.Join(", ", allowed)}");
        
    }
}