using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using R8.Lib.MethodReturn;

namespace R8.Lib.AspNetCore
{
    public abstract class BaseInputModel<TModel> : BaseInputModel<TModel, string> where TModel : class
    {
    }

    public abstract class BaseInputModel<TModel, TId> : ValidatableObject<TModel> where TModel : class
    {
        public bool IsNew => Id == null;

        [HiddenInput]
        public TId Id { get; set; }

        public async Task<Response<TSource>> HandleAsync<TSource>(
        Func<Task<Response<TSource>>> onAddition, Func<Task<Response<TSource>>> onUpdate)
        where TSource : class
        {
            var status = IsNew
              ? await onAddition.Invoke().ConfigureAwait(false)
              : await onUpdate.Invoke().ConfigureAwait(false);

            return status;
        }

        public async Task<Response<TSource>> HandleAsync<TSource>(
            Func<Task<Response<TSource>>> onAddition, Func<Response<TSource>> onUpdate)
            where TSource : class
        {
            var status = IsNew
                ? await onAddition.Invoke().ConfigureAwait(false)
                : onUpdate.Invoke();

            return status;
        }

        public async Task<Response<TSource>> HandleAsync<TSource>(
            Func<Response<TSource>> onAddition, Func<Task<Response<TSource>>> onUpdate)
            where TSource : class
        {
            var status = IsNew
                ? onAddition.Invoke()
                : await onUpdate.Invoke().ConfigureAwait(false);

            return status;
        }

        public Response<TSource> Handle<TSource>(
            Func<Response<TSource>> onAddition, Func<Response<TSource>> onUpdate)
            where TSource : class
        {
            var status = IsNew
                ? onAddition.Invoke()
                : onUpdate.Invoke();

            return status;
        }
    }
}