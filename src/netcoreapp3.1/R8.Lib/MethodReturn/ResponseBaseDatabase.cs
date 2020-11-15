using R8.Lib.Enums;
using R8.Lib.Localization;

namespace R8.Lib.MethodReturn
{
    public abstract class ResponseBaseDatabase : ResponseBase, IResponseBaseDatabase
    {
        protected ResponseBaseDatabase()
        {
        }

        protected ResponseBaseDatabase(ILocalizer localizer) : base(localizer)
        {
        }

        protected ResponseBaseDatabase(object status) : base(status)
        {
        }

        protected ResponseBaseDatabase(object status, ValidatableResultCollection errors) : base(status, errors)
        {
        }

        public DatabaseSaveState? Save { get; set; }

        public bool Success => IsSaveSuccessful(Save);

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