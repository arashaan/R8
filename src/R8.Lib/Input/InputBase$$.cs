using System;
using System.Collections.Generic;
using System.Text;
using R8.Lib.Validatable;

namespace R8.Lib.Input
{
    public abstract class InputBase<TModel, TId> : ValidatableObject<TModel>, IInputBase<TId> where TModel : class
    {
        public virtual bool IsNew => Id == null;

        public virtual TId Id { get; set; }
    }
}