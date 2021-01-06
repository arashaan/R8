using Microsoft.AspNetCore.Mvc;

using R8.Lib;

namespace R8.AspNetCore
{
    public interface IInputBase
    {
        bool IsNew { get; }
    }

    public interface IInputBase<TId> : IInputBase
    {
        TId Id { get; set; }
    }

    public abstract class InputBase<TModel> : InputBase<TModel, string> where TModel : class
    {
    }

    public abstract class InputBase<TModel, TId> : ValidatableObject<TModel>, IInputBase<TId> where TModel : class
    {
        public virtual bool IsNew => Id == null;

        [HiddenInput]
        public virtual TId Id { get; set; }
    }
}