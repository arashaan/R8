using System.Globalization;
using System.Linq;

using Microsoft.EntityFrameworkCore;

using R8.Lib;
using R8.Lib.Localization;

namespace R8.EntityFrameworkCore
{
    public static class EntityLocalizedExtensions
    {
        /// <summary>
        /// Filters a sequence of values based on <c><see cref="localizedName"/></c> and <c><see cref="canonicalName"/></c> in given <see cref="IQueryable{TResult}"/>.
        /// </summary>
        /// <typeparam name="TSource">A generic type of <see cref="EntityLocalized"/></typeparam>
        /// <param name="source">A <see cref="IQueryable{T}"/> that representing translating query.</param>
        /// <param name="canonicalName">A <see cref="string"/> value that representing canonical name that already stored in entity.</param>
        /// <param name="localizedName">A <see cref="string"/> value that representing localized name that already stored in <see cref="LocalizerContainer"/> in entity.</param>
        /// <returns>Continues chains of <see cref="IQueryable"/> plus adding current filter.</returns>
        public static IQueryable<TSource> WhereHas<TSource>(this IQueryable<TSource> source, string canonicalName, string localizedName) where TSource : EntityLocalized
        {
            var twoLetter = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
            return source.Where(x =>
                x.CanonicalName.Equals(canonicalName) ||
                EF.Functions.Like(x.NameJson, $"%\"{twoLetter}\":\"{localizedName}\"%"));
        }
    }
}