namespace R8.EntityFrameworkCore
{
    public enum DatabaseSaveState
    {
        NotSavedYet = 0,
        NoNeedToSave = 1,
        NotSaved = 2,
        SavedWithErrors = 3,
        Saved = 4,
        SaveFailure = 5
    }
}