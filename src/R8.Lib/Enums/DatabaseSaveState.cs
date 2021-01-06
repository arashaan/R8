namespace R8.Lib.Enums
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