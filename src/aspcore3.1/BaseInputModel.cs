using Microsoft.AspNetCore.Mvc;

using R8.Lib;

namespace R8.AspNetCore
{
    public interface IBaseInput
    {
        bool IsNew { get; }
    }

    public interface IBaseInput<TId> : IBaseInput
    {
        TId Id { get; set; }
    }

    public abstract class BaseInput<TModel> : BaseInput<TModel, string> where TModel : class
    {
    }

    public abstract class BaseInput<TModel, TId> : ValidatableObject<TModel>, IBaseInput<TId> where TModel : class
    {
        public bool IsNew => Id == null;

        [HiddenInput]
        public TId Id { get; set; }
    }
}