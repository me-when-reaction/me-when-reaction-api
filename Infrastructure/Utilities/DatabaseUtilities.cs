using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MeWhenAPI.Domain.Model;

namespace MeWhenAPI.Infrastructure.Utilities
{
    public static class DatabaseUtilities
    {
        public static IQueryable<TModel> IsNotDeletedAnd<TModel>(this IQueryable<TModel> query, Expression<Func<TModel, bool>> predicate) where TModel : BaseModel
            => query.Where(x => !x.Deleted).Where(predicate);
    }
}