using R8.Lib.Attributes;

namespace R8.AspNetCore3_1.Demo.Services.Enums
{
    public enum Flags
    {
        [FlagShow]
        Failed = 0,

        [FlagShow]
        Aborted = 1,

        [FlagShow]
        Success = 100,

        #region Database

        UnableToSave = 98,
        NoNeedToSave = 99,

        #endregion Database

        #region User

        [FlagShow]
        WrongPassword = 101,

        [FlagShow]
        NeedToSignIn = 102,

        [FlagShow]
        SignedIn = 103,

        [FlagShow]
        Deactivated = 104,

        [FlagShow]
        UserNotFound = 105,

        [FlagShow]
        NeedValidEmail = 106,

        [FlagShow]
        EmailAlreadyRegistered = 107,

        [FlagShow]
        EmailAlreadyConfirmed = 108,

        [FlagShow]
        TryNewPassword = 109,

        [FlagShow]
        NeedEmailVerification = 110,

        [FlagShow]
        CredentialsNotValid = 111,

        #endregion User

        #region Unable

        [FlagShow]
        UnableToSendMail = 200,

        [FlagShow]
        UnableToUpload = 201,

        [FlagShow]
        Conflict = 202,

        #endregion Unable

        #region Language

        [FlagShow]
        LanguageNotSupported = 300,

        [FlagShow]
        UnableToChangeLanguage = 301,

        [FlagShow]
        UnableToLogin = 302,

        #endregion Language

        #region Forbidden

        [FlagShow]
        Forbidden = 400,

        [FlagShow]
        ForbiddenAndUnableToUpdateOrShow = 401,

        #endregion Forbidden

        #region Token

        [FlagShow]
        TokenIsNotValid = 500,

        [FlagShow]
        CodeIsNotValid = 501,

        [FlagShow]
        CodeExpired = 502,

        #endregion Token

        #region Model

        [FlagShow]
        RetryAfterReview = 600,

        // [FlagShow]
        // DuplicateModel = 601,

        [FlagShow]
        AlreadyExists = 602,

        [FlagShow]
        AlreadyDeleted = 603,

        [FlagShow]
        NotDeleted = 604,

        #endregion Model

        #region Null

        UserIsNull = 800,
        ModelIsNull = 801,
        ParamIsNull = 802,
        ValueIsNull = 803,
        EntityIsNull = 804,

        [FlagShow]
        ModelIsNotValid = 805,

        IdIsNotValid = 806,

        [FlagShow]
        ReachedMaximum = 807,

        #endregion Null

        NA = 999,
        UnexpectedError = 1000,
    }
}