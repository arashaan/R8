using R8.EntityFrameworkCore.Test.Enums;
using R8.EntityFrameworkCore.Wrappers;

namespace R8.EntityFrameworkCore.Test
{
    public class FakeWrapper<TModel> : WrapperBase<TModel, Flags> where TModel : class
    {
        public FakeWrapper(TModel entity) : base(entity)
        {
        }

        public FakeWrapper(Flags status) : base(status)
        {
        }

        public FakeWrapper(Flags status, TModel entity) : base(status, entity)
        {
        }

        public string Message
        {
            get
            {
                return this.Localizer != null ? this.Localizer[Status.ToString()] : Status.ToString();
            }
        }

        public override bool Success => Status == Flags.Success;
        public override Flags Status { get; set; }

        public static implicit operator FakeWrapper<TModel>(Flags flag)
        {
            return new FakeWrapper<TModel>(flag);
        }

        public static explicit operator Flags(FakeWrapper<TModel> src)
        {
            return src.Status;
        }
    }
}