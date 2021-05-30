using Newtonsoft.Json;

using R8.Lib.JsonExtensions;

using System.Collections.Generic;
using System.Linq;

namespace R8.EntityFrameworkCore.Audits
{
    /// <summary>
    /// Represents a strongly typed list of <see cref="IAudit"/> that can be accessed by index. Provides methods to search, sort, and manipulate lists.
    /// </summary>
    public class AuditCollection : List<Audit>
    {
        /// <summary>
        /// Represents a strongly typed list of <see cref="Audit"/> that can be accessed by index. Provides methods to search, sort, and manipulate lists.
        /// </summary>
        public AuditCollection()
        {
        }

        /// <summary>
        /// Represents a strongly typed list of <see cref="Audit"/> that can be accessed by index. Provides methods to search, sort, and manipulate lists.
        /// </summary>
        public AuditCollection(IEnumerable<Audit> collection) : base(collection)
        {
        }

        public override string ToString()
        {
            var toString = $"{this.Count} Audits.";
            if (Last == null) return toString;

            var lastText = $"Last: {Last.Flag} - {Last.DateTime}";
            return $"{toString}. {lastText}";
        }

        /// <summary>
        /// Gets first item of current instance in type of <see cref="Audit"/> that represents creation data.
        /// </summary>
        public Audit Created => this.OrderByDescending(x => x.DateTime).FirstOrDefault(x => x.Flag == AuditFlags.Created);

        private static JsonSerializerSettings SerializerSettings
        {
            get
            {
                var settings = new JsonSerializerSettings
                {
                    Error = (serializer, err) => err.ErrorContext.Handled = true,
                    DefaultValueHandling = DefaultValueHandling.Ignore,
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    NullValueHandling = NullValueHandling.Ignore,
                    ObjectCreationHandling = ObjectCreationHandling.Replace,
                    ContractResolver = new NullToEmptyContractResolver(),
                    Formatting = Formatting.None,
                    PreserveReferencesHandling = PreserveReferencesHandling.None,
                    TypeNameHandling = TypeNameHandling.Auto
                };
                return settings;
            }
        }

        /// <summary>
        /// Returns an automated method to serialize current instance to JSON.
        /// </summary>
        /// <returns>A <see cref="string"/> for JSON.</returns>
        public string Serialize()
        {
            return JsonConvert.SerializeObject(this, SerializerSettings);
        }

        public static explicit operator AuditCollection(string json)
        {
            return Deserialize(json);
        }

        public static implicit operator string(AuditCollection collection)
        {
            return collection.Serialize();
        }

        /// <summary>
        /// Returns a deserialized instance of <see cref="AuditCollection"/> according to given json.
        /// </summary>
        /// <param name="json">A <see cref="string"/> value that representing JSON data.</param>
        /// <returns>A <see cref="AuditCollection"/> object.</returns>
        public static AuditCollection Deserialize(string json)
        {
            if (string.IsNullOrEmpty(json))
                return new AuditCollection();

            AuditCollection obj;
            try
            {
                obj = JsonConvert.DeserializeObject<AuditCollection>(json, SerializerSettings) ?? new AuditCollection();
            }
            catch
            {
                obj = new AuditCollection();
            }

            return obj;
        }

        /// <summary>
        /// Gets last item of current instance in type of <see cref="Audit"/>.
        /// </summary>
        public Audit Last => this.OrderByDescending(x => x.DateTime).FirstOrDefault();
    }
}