using Microsoft.EntityFrameworkCore;

namespace R8.EntityFrameworkCore
{
    public enum DatabaseSaveState
    {
        ///// <summary>
        ///// Empty state will be shown when still Saving of changes not started.
        ///// </summary>
        //NotSavedYet = 0,

        ///// <summary>
        ///// When could not find any changes in Database models.
        ///// </summary>
        //NoNeedToSave = 1,

        /// <summary>
        /// When have entity changes, but database could not save none of changes.
        /// </summary>
        NotSaved = 2,

        /// <summary>
        /// When saved changes in Database, are less than expected changes.
        /// </summary>
        SavedPartially = 3,

        /// <summary>
        /// Saved successfully.
        /// </summary>
        Saved = 4,

        /// <summary>
        /// <para>When an error returned from Database server and DbContext unable to save changes.</para>
        /// <para>Commonly returned when has <see cref="DbUpdateConcurrencyException"/> or <see cref="DbUpdateException"/>.</para>
        /// </summary>
        SaveFailure = 5,

        ///// <summary>
        ///// When saved changes in Database, are more than expected changes.
        ///// </summary>
        //SavedMore = 6
    }
}