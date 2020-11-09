using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace R8.EntityFrameworkCore
{
    /// <summary>
    /// Represents a strongly typed list of <see cref="IAudit"/> that can be accessed by index. Provides methods to search, sort, and manipulate lists.
    /// </summary>
    public class AuditCollection : List<IAudit>
    {
        public override string ToString()
        {
            var toString = $"{this.Count} Audits.";
            if (Last == null) return toString;

            var lastText = $"Last: {Last.Flag} - {Last.DateTime}";
            return $"{toString}. {lastText}";
        }

        /// <summary>
        /// Gets first item of current instance in type of <see cref="IAudit"/> that represents creation data.
        /// </summary>
        public IAudit Created => this.First(x => x.Flag == AuditFlags.Created);

        private static JsonSerializerSettings SerializerSettings
        {
            get
            {
                var settings = Lib.JsonExtensions.CustomJsonSerializerSettings.Settings;
                settings.DefaultValueHandling = DefaultValueHandling.Ignore;

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
        /// Gets last item of current instance in type of <see cref="IAudit"/>.
        /// </summary>
        public IAudit Last => this.OrderByDescending(x => x.DateTime).First();
    }
}