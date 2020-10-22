using Newtonsoft.Json;

using R8.Lib.Enums;

namespace R8.Lib.MethodReturn
{
    public class ResponseDatabase : IResponseDatabase
    {
        public ResponseDatabase()
        {
        }

        public ResponseDatabase(Flags status)
        {
            Status = status;
        }

        public ResponseDatabase(Flags status, ValidatableResultCollection errors) : this(status)
        {
            Errors = errors;
        }

        [JsonIgnore]
        public DatabaseSaveState? Save { get; set; }

        public bool Success => CheckSuccess(Status, Save);

        public Flags Status { get; set; }
        public string Message => this.GetMessage();
        public ValidatableResultCollection Errors { get; set; }
        public ILocalizer Localizer { get; set; }

        public static explicit operator ResponseDatabase(Flags status)
        {
            var response = new ResponseDatabase(status);
            return response;
        }

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