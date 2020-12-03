using R8.Lib.Enums;
using R8.Lib.Localization;

namespace R8.Lib.MethodReturn
{
    public abstract class ResponseBaseDatabase<TStatus> : ResponseBase<TStatus>, IResponseBaseDatabase<TStatus>
    {
        public ResponseBaseDatabase()
        {
        }

        public ResponseBaseDatabase(ILocalizer localizer) : base(localizer)
        {
        }

        public ResponseBaseDatabase(TStatus status) : base(status)
        {
        }

        public ResponseBaseDatabase(TStatus status, ValidatableResultCollection errors) : base(status, errors)
        {
        }

        public virtual DatabaseSaveState? Save { get; set; }

        public virtual bool Success => IsSaveSuccessful(Save);

        public static bool IsSaveSuccessful(DatabaseSaveState? saveState)
        {
            if (saveState == null)
                return true;

            return saveState == DatabaseSaveState.Saved ||
                   saveState == DatabaseSaveState.NotSaved ||
                   saveState == DatabaseSaveState.SavedWithErrors;
        }
    }
}