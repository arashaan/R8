using System.ComponentModel;

namespace R8.Lib.AspNetCore.Base.Enums
{
    /// <summary>
    /// A enumerator constants the represents values for <c>autocomplete</c> tag in HTML
    /// </summary>
    public enum AutoCompletes
    {
        /// <summary>
        /// The middle name.
        /// </summary>
        [Description("additional-name")]
        AdditionalName,

        /// <summary>
        /// The first administrative level in the address. This is typically the province in which the address is located. In the United Flag, this would be the state. In Switzerland, the canton. In the United Kingdom, the post town.
        /// </summary>
        [Description("address-level1")]
        AddressLevel1,

        /// <summary>
        /// The second administrative level, in addresses with at least two of them. In countries with two administrative levels, this would typically be the city, town, village, or other locality in which the address is located.
        /// </summary>
        [Description("address-level2")]
        AddressLevel2,

        /// <summary>
        ///     The third administrative level, in addresses with at least three administrative levels.
        /// </summary>
        [Description("address-level3")]
        AddressLevel3,

        /// <summary>
        ///     The finest-grained administrative level, in addresses which have four levels.
        /// </summary>
        [Description("address-level4")]
        AddressLevel4,

        /// <summary>
        /// Each individual line of the street address. These should only be present if the "street-address" is also present.
        /// </summary>
        [Description("address-line1")]
        AddressLine1,

        /// <summary>
        /// Each individual line of the street address. These should only be present if the "street-address" is also present.
        /// </summary>
        [Description("address-line2")]
        AddressLine2,

        /// <summary>
        /// Each individual line of the street address. These should only be present if the "street-address" is also present.
        /// </summary>
        [Description("address-line3")]
        AddressLine3,

        /// <summary>
        ///     A birth date, as a full date.
        /// </summary>
        [Description("bday")]
        BirthDate,

        /// <summary>
        ///     The day of the month of a birth date.
        /// </summary>
        [Description("bday-day")]
        BirthDay,

        /// <summary>
        ///     The month of the year of a birth date.
        /// </summary>
        [Description("bday-month")]
        BirthMonth,

        /// <summary>
        ///     The year of a birth date.
        /// </summary>
        [Description("bday-year")]
        BirthYear,

        /// <summary>
        /// meaning the field is part of the billing address or contact information
        /// </summary>
        [Description("billing")]
        Billing,

        /// <summary>
        ///     A middle name as given on a payment instrument or credit card.
        /// </summary>
        [Description("cc-additional-name")]
        CreditCardAdditionalName,

        /// <summary>
        ///     The security code for the payment instrument; on credit cards, this is the 3-digit verification number on the back of the card.
        /// </summary>
        [Description("cc-csc")]
        CreditCardCsc,

        /// <summary>
        ///     A payment method expiration date, typically in the form "MM/YY" or "MM/YYYY".
        /// </summary>
        [Description("cc-exp")]
        CreditCardExpiryDate,

        /// <summary>
        ///     The month in which the payment method expires.
        /// </summary>
        [Description("cc-exp-month")]
        CreditCardExpiryMonth,

        /// <summary>
        ///     The year in which the payment method expires.
        /// </summary>
        [Description("cc-exp-year")]
        CreditCardExpiryYear,

        /// <summary>
        ///     A family name, as given on a credit card.
        /// </summary>
        [Description("cc-family-name")]
        CreditCardFamilyName,

        /// <summary>
        ///     A given (first) name as given on a payment instrument like a credit card.
        /// </summary>
        [Description("cc-given-name")]
        CreditCardGivenName,

        /// <summary>
        ///     The full name as printed on or associated with a payment instrument such as a credit card. Using a full name field is preferred, typically, over breaking the name into pieces.
        /// </summary>
        [Description("cc-name")]
        CreditCardName,

        /// <summary>
        ///     A credit card number or other number identifying a payment method, such as an account number.
        /// </summary>
        [Description("cc-number")]
        CreditCardNumber,

        /// <summary>
        ///     The type of payment instrument (such as "Visa" or "Master Card").
        /// </summary>
        [Description("cc-type")]
        CreditCardType,

        /// <summary>
        ///     A country code.
        /// </summary>
        [Description("country")]
        Country,

        /// <summary>
        ///     A country name.
        /// </summary>
        [Description("country-name")]
        CountryName,

        /// <summary>
        ///     The user's current password.
        /// </summary>
        [Description("current-password")]
        CurrentPassword,

        /// <summary>
        ///     An email address.
        /// </summary>
        [Description("email")]
        Email,

        /// <summary>
        ///     The family (or "last") name.
        /// </summary>
        [Description("family-name")]
        FamilyName,

        /// <summary>
        /// meaning the field describes a fax machine's contact details
        /// </summary>
        [Description("fax")]
        Fax,

        /// <summary>
        /// The given (or "first") name.
        /// </summary>
        [Description("given-name")]
        GivenName,

        /// <summary>
        /// meaning the field is for contacting someone at their residence
        /// </summary>
        [Description("home")]
        Home,

        /// <summary>
        /// The prefix or title, such as "Mrs.", "Mr.", "Miss", "Ms.", "Dr.", or "Mlle.".
        /// </summary>
        [Description("honorific-prefix")]
        HonorificPrefix,

        /// <summary>
        ///     The suffix, such as "Jr.", "B.Sc.", "PhD.", "MBASW", or "IV".
        /// </summary>
        [Description("honorific-suffix")]
        HonorificSuffix,

        /// <summary>
        ///     A URL for an instant messaging protocol endpoint, such as "xmpp:username@example.net".
        /// </summary>
        [Description("impp")]
        Impp,

        /// <summary>
        ///     A preferred language, given as a valid BCP 47 language tag.
        /// </summary>
        [Description("language")]
        Language,

        /// <summary>
        /// meaning the field is for contacting someone regardless of location
        /// </summary>
        [Description("mobile")]
        Mobile,

        /// <summary>
        /// The field expects the value to be a person's full name. Using "name" rather than breaking the name down into its components is generally preferred because it avoids dealing with the wide diversity of human names and how they are structured; however, you can use the following autocomplete values if you do need to break the name down into its components
        /// </summary>
        [Description("name")]
        Name,

        /// <summary>
        /// A new password. When creating a new account or changing passwords, this should be used for an "Enter your new password" or "Confirm new password" field, as opposed to a general "Enter your current password" field that might be present. This may be used by the browser both to avoid accidentally filling in an existing password and to offer assistance in creating a secure password.
        /// </summary>
        [Description("new-password")]
        NewPassword,

        /// <summary>
        ///     A nickname or handle.
        /// </summary>
        [Description("nickname")]
        Nickname,

        /// <summary>
        /// A company or organization name, such as "Acme Widget Company" or "Girl Scouts of America".
        /// </summary>
        [Description("organization")]
        Organization,

        /// <summary>
        /// A job title, or the title a person has within an organization, such as "Senior Technical Writer", "President", or "Assistant Troop Leader".
        /// </summary>
        [Description("organization-title")]
        OrganizationTitle,

        /// <summary>
        /// meaning the field describes a pager's or beeper's contact details
        /// </summary>
        [Description("pager")]
        Pager,

        /// <summary>
        ///     The URL of an image representing the person, company, or contact information given in the other fields in the form.
        /// </summary>
        [Description("photo")]
        Photo,

        /// <summary>
        ///     A postal code (in the United Flag, this is the ZIP code).
        /// </summary>
        [Description("postal-code")]
        PostalCode,

        /// <summary>
        ///     A gender identity (such as "Female", "Fa'afafine", "Male"), as freeform text without newlines.
        /// </summary>
        [Description("sex")]
        Sex,

        /// <summary>
        /// meaning the field is part of the shipping address or contact information
        /// </summary>
        [Description("shipping")]
        Shipping,

        /// <summary>
        /// A street address. This can be multiple lines of text, and should fully identify the location of the address within its second administrative level (typically a city or town), but should not include the city name, ZIP or postal code, or country name.
        /// </summary>
        [Description("street-address")]
        StreetAddress,

        /// <summary>
        /// A full telephone number, including the country code. If you need to break the phone number up into its components, you can use these values for those fields:
        /// </summary>
        [Description("tel")]
        Telephone,

        /// <summary>
        ///     The area code, with any country-internal prefix applied if appropriate.
        /// </summary>
        [Description("tel-area-code")]
        TelephoneAreaCode,

        /// <summary>
        ///     The country code, such as "1" for the United Flag, Canada, and other areas in North America and parts of the Caribbean.
        /// </summary>
        [Description("tel-country-code")]
        TelephoneCountryCode,

        /// <summary>
        ///     A telephone extension code within the phone number, such as a room or suite number in a hotel or an office extension in a company.
        /// </summary>
        [Description("tel-extension")]
        TelephoneExtension,

        /// <summary>
        ///     The phone number without the country or area code. This can be split further into two parts, for phone numbers which have an exchange number and then a number within the exchange. For the phone number "555-6502", use "tel-local-prefix" for "555" and "tel-local-suffix" for "6502".
        /// </summary>
        [Description("tel-local")]
        TelephoneLocal,

        /// <summary>
        /// First part of the component of the telephone number that follows the area code, when that component is split into two components
        /// </summary>
        [Description("tel-local-prefix")]
        TelephoneLocalPrefix,

        /// <summary>
        /// Second part of the component of the telephone number that follows the area code, when that component is split into two components
        /// </summary>
        [Description("tel-local-suffix")]
        TelephoneLocalSuffix,

        /// <summary>
        ///     The entire phone number without the country code component, including a country-internal prefix. For the phone number "1-855-555-6502", this field's value would be "855-555-6502".
        /// </summary>
        [Description("tel-national")]
        TelephoneNational,

        /// <summary>
        ///     The amount, given in the currency specified by "transaction-currency", of the transaction, for a payment form.
        /// </summary>
        [Description("transaction-amount")]
        TransactionAmount,

        /// <summary>
        ///     The currency in which the transaction is to take place.
        /// </summary>
        [Description("transaction-currency")]
        TransactionCurrency,

        /// <summary>
        ///     A URL, such as a home page or company web site address as appropriate given the context of the other fields in the form.
        /// </summary>
        [Description("url")]
        Url,

        /// <summary>
        /// meaning the field is for contacting someone at their workplace
        /// </summary>
        [Description("work")]
        Work,

        /// <summary>
        ///     A username or account name.
        /// </summary>
        [Description("username")]
        Username,

        /// <summary>
        ///     A one-time code used for verifying user identity.
        /// </summary>
        [Description("one-time-code")]
        OneTimeCode
    }
}