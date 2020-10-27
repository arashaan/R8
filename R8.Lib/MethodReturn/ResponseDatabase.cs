using R8.Lib.Enums;
using R8.Lib.Localization;

namespace R8.Lib.MethodReturn
{
    public class ResponseDatabase : Response, IResponseDatabase
    {
        public ResponseDatabase()
        {
        }

        public ResponseDatabase(ILocalizer localizer) : base(localizer)
        {
        }

        public ResponseDatabase(Flags status) : base(status)
        {
        }

        public ResponseDatabase(Flags status, ValidatableResultCollection errors) : base(status, errors)
        {
        }

        public DatabaseSaveState? Save { get; set; }

        public override bool Success => CheckSuccess(Status, Save);

        public static bool CheckSuccess(Flags status, DatabaseSaveState? saveState)
        {
            if (status == Flags.Success && saveState == null)
                return true;

            return status == Flags.Success &&
                   (saveState == DatabaseSaveState.Saved ||
                    saveState == DatabaseSaveState.NotSaved ||
                    saveState == DatabaseSaveState.SavedWithErrors);
        }
    }
}