using R8.Lib.Localization;

namespace R8.EntityFrameworkCore
{
    /// <summary>
    /// An <see cref="IEntityLocalizedBase"/> interface that representing globalized information about specific entity.
    /// </summary>
    public interface IEntityLocalizedBase
    {
        /// <summary>
        /// Gets of sets serialized type of <see cref="Name"/> as JSON.
        /// </summary>
        /// <remarks>STRONGLY RECOMMENDED to edit <see cref="Name"/> value, instead of this property's value.</remarks>
        public string NameJson { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="LocalizerContainer"/> that stores globalized values and specified cultures.
        /// </summary>
        /// <remarks>This property will not been mapped into the entity.</remarks>
        public LocalizerContainer Name { get; set; }
    }
}