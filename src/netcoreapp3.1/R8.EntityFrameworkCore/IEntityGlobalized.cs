using R8.Lib.Localization;

namespace R8.EntityFrameworkCore
{
    /// <summary>
    /// An <see cref="IEntityGlobalized"/> interface that representing globalized information about specific entity.
    /// </summary>
    public interface IEntityGlobalized
    {
        /// <summary>
        /// Gets of sets serialized type of <see cref="Name"/> as JSON.
        /// </summary>
        public string NameJson { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="LocalizerContainer"/> that stores globalized values and specified cultures.
        /// </summary>
        public LocalizerContainer Name
        {
            get => LocalizerContainer.Deserialize(NameJson);
            set => NameJson = value.Serialize();
        }
    }
}