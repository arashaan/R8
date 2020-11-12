using Microsoft.AspNetCore.Mvc;

using R8.Lib;
using R8.Lib.Enums;
using R8.Lib.MethodReturn;

using System;
using System.Threading.Tasks;

namespace R8.AspNetCore
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
            Func<Task<Flags>> onAddition, Func<Task<Flags>> onUpdate)
            where TSource : class
        {
            var status = IsNew
                ? await onAddition.Invoke().ConfigureAwait(false)
                : await onUpdate.Invoke().ConfigureAwait(false);
            return new Response<TSource>(status);
        }

        public async Task<Response<TSource>> HandleAsync<TSource>(
            Func<Task<Flags>> onAddition, Func<Flags> onUpdate)
            where TSource : class
        {
            var status = IsNew
                ? await onAddition.Invoke().ConfigureAwait(false)
                : onUpdate.Invoke();
            return new Response<TSource>(status);
        }

        public async Task<Response<TSource>> HandleAsync<TSource>(
            Func<Flags> onAddition, Func<Task<Flags>> onUpdate)
            where TSource : class
        {
            var status = IsNew
                ? onAddition.Invoke()
                : await onUpdate.Invoke().ConfigureAwait(false);
            return new Response<TSource>(status);
        }

        public Response<TSource> Handle<TSource>(
            Func<Flags> onAddition, Func<Flags> onUpdate)
            where TSource : class
        {
            var status = IsNew
                ? onAddition.Invoke()
                : onUpdate.Invoke();
            return new Response<TSource>(status);
        }
    }
}