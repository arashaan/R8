using R8.Lib.Enums;
using R8.Lib.Localization;

namespace R8.Lib.MethodReturn
{
    /// <summary>
    /// An base class for saving changes of DbContext entity.
    /// </summary>
    /// <typeparam name="TStatus">A type that representing status type.</typeparam>
    public abstract class ResponseBaseDatabase<TStatus> : ResponseBase<TStatus>, IResponseBaseDatabase<TStatus>
    {
        /// <summary>
        /// An base class for saving changes of DbContext entity.
        /// </summary>
        public ResponseBaseDatabase()
        {
        }

        /// <summary>
        /// An base class for saving changes of DbContext entity.
        /// </summary>
        /// <param name="localizer">A working instance of <see cref="ILocalizer"/>.</param>
        public ResponseBaseDatabase(ILocalizer localizer) : base(localizer)
        {
        }

        /// <summary>
        /// An base class for saving changes of DbContext entity.
        /// </summary>
        /// <param name="status">A value that representing status.</param>
        public ResponseBaseDatabase(TStatus status) : base(status)
        {
        }

        /// <summary>
        /// An base class for saving changes of DbContext entity.
        /// </summary>
        /// <param name="status">A value that representing status.</param>
        /// <param name="errors">A collection of <see cref="ValidatableResultCollection"/> that representing errors.</param>
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