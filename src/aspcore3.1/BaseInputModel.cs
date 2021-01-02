using Microsoft.AspNetCore.Mvc;

using R8.Lib;

namespace R8.AspNetCore
{
    public interface IBaseInputModel
    {
        bool IsNew { get; }
    }

    public interface IBaseInputModel<TId> : IBaseInputModel
    {
        TId Id { get; set; }
    }

    public abstract class BaseInputModel<TModel> : BaseInputModel<TModel, string> where TModel : class
    {
    }

    public abstract class BaseInputModel<TModel, TId> : ValidatableObject<TModel>, IBaseInputModel<TId> where TModel : class
    {
        public bool IsNew => Id == null;

        [HiddenInput]
        public TId Id { get; set; }
    }
}