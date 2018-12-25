using System;
using System.Linq;
using System.Linq.Expressions;

namespace UpsCoolWeb.Data.Core
{
    public interface IQuery<TModel> : IQueryable<TModel>
    {
        IQuery<TResult> Select<TResult>(Expression<Func<TModel, TResult>> selector);
        IQuery<TModel> Where(Expression<Func<TModel, Boolean>> predicate);

        IQueryable<TView> To<TView>();
    }
}
