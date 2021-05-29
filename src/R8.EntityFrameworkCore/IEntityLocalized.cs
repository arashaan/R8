using R8.Lib.Localization;

using System.Globalization;

namespace R8.EntityFrameworkCore
{
    /// <summary>
    /// An <see cref="IEntityLocalized"/> interface that representing some basic data about and also globalized information  specific entity.
    /// </summary>
    public interface IEntityLocalized : IEntityBase, IEntityLocalizedBase
    {
        /// <summary>
        /// Updates value of the <see cref="Name"/> property in <see cref="CultureInfo.TwoLetterISOLanguageName"/>.
        /// </summary>
        /// <param name="cultureTwoLetterIso"></param>
        /// <param name="value"></param>
        /// <returns>A <see cref="LocalizerContainer"/>.</returns>
        LocalizerContainer UpdateName(string cultureTwoLetterIso, string value);

        /// <summary>
        /// Updates value of the <see cref="Name"/> property in specified <see cref="CultureInfo"/>.
        /// </summary>
        /// <param name="culture"></param>
        /// <param name="value"></param>
        /// <returns>A <see cref="LocalizerContainer"/>.</returns>
        LocalizerContainer UpdateName(CultureInfo culture, string value);

        /// <summary>
        /// Updates value of the <see cref="Name"/> property in <see cref="CultureInfo.CurrentCulture"/>.
        /// </summary>
        /// <param name="value"></param>
        /// <returns>A <see cref="LocalizerContainer"/>.</returns>
        LocalizerContainer UpdateName(string value);
    }
}