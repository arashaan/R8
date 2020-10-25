using Newtonsoft.Json;

using R8.Lib.Enums;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace R8.Lib.MethodReturn
{
    /// <summary>
    /// Represents a collection of responses with sub-results
    /// </summary>
    public class ResponseCollection : IDictionary<int, IResponseDatabase>, IResponseBase
    {
        /// <summary>
        /// Represents a collection of responses with sub-results
        /// </summary>
        public ResponseCollection()
        {
        }

        /// <summary>
        /// Represents a collection of responses with sub-results
        /// </summary>
        /// <param name="status">Set default <see cref="Flags"/> to collection instance</param>
        public ResponseCollection(Flags status)
        {
            this.Add(new ResponseDatabase(status));
        }

        /// <summary>
        /// Represents a collection of responses with sub-results
        /// </summary>
        /// <param name="response">Add single instance of <see cref="IResponseDatabase"/></param>
        public ResponseCollection(IResponseDatabase response)
        {
            this.Add(response);
        }

        /// <summary>
        /// Represents a collection of responses with sub-results
        /// </summary>
        /// <param name="collection">Add collection of <see cref="IResponseDatabase"/> to instance</param>
        public ResponseCollection(IEnumerable<IResponseDatabase> collection)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            this.AddRange(collection);
        }

        /// <summary>
        /// List of results with type of <see cref="IResponseDatabase"/>
        /// </summary>
        [JsonIgnore]
        public Dictionary<int, IResponseDatabase> Results { get; set; }

        /// <summary>
        /// Gets or sets Save State into DbContext
        /// </summary>
        public DatabaseSaveState? Save { get; set; }

        /// <summary>
        /// An instance of <see cref="IResponseDatabase"/> as Head of the Collection that it is first iterate of <see cref="Results"/>
        /// </summary>
        public IResponseDatabase? Head => Results[0];

        /// <summary>
        /// Add an item to the <see cref="ICollection{T}"/>
        /// </summary>
        /// <param name="model">The object to add to <see cref="ICollection{T}"/></param>
        public void Add(IResponseDatabase model)
        {
            Results ??= new Dictionary<int, IResponseDatabase>();
            Results.Add(Results.Count, model);
        }

        public void Add(KeyValuePair<int, IResponseDatabase> item)
        {
            Results ??= new Dictionary<int, IResponseDatabase>();
            var (key, value) = item;
            Results.Add(key, value);
        }

        public void Clear()
        {
            Results.Clear();
        }

        public bool Contains(KeyValuePair<int, IResponseDatabase> item)
        {
            return Results.Contains(item);
        }

        public void CopyTo(KeyValuePair<int, IResponseDatabase>[] array, int arrayIndex)
        {
            return;
        }

        public bool Remove(KeyValuePair<int, IResponseDatabase> item)
        {
            return Results.Remove(item.Key);
        }

        public int Count => Results.Count;
        public bool IsReadOnly => false;

        public void AddRange(IEnumerable<IResponseDatabase> collection)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            Results ??= new Dictionary<int, IResponseDatabase>();
            foreach (var response in collection)
                Results.Add(Results.Count, response);
        }

        public void Add<T>(Flags status) where T : class
        {
            this.Add(new Response<T>(status));
        }

        public bool Success
        {
            get
            {
                var baseCondition = Results != null && Results.Any() && Results.All(x => x.Value.Success);
                if (Save != null)
                    return baseCondition;

                var dbCondition = Save == DatabaseSaveState.Saved ||
                                  Save == DatabaseSaveState.NotSaved ||
                                  Save == DatabaseSaveState.SavedWithErrors;
                return baseCondition && dbCondition;
            }
        }

        public static explicit operator ResponseCollection(Flags status)
        {
            return new ResponseCollection(status);
        }

        public static implicit operator bool(ResponseCollection response)
        {
            return response.Success;
        }

        public ValidatableResultCollection Errors =>
            (ValidatableResultCollection)Results?.SelectMany(x => x.Value.Errors).ToList();

        public void Add(int key, IResponseDatabase value)
        {
            Results.Add(key, value);
        }

        public bool ContainsKey(int key)
        {
            return Results.ContainsKey(key);
        }

        public bool Remove(int key)
        {
            return Results.Remove(key);
        }

        public bool TryGetValue(int key, out IResponseDatabase value)
        {
            return Results.TryGetValue(key, out value);
        }

        public IResponseDatabase this[int index]
        {
            get => Results[index];
            set => Results[index] = value;
        }

        public ICollection<int> Keys => Results.Select(x => x.Key).ToList();
        public ICollection<IResponseDatabase> Values => Results.Select(x => x.Value).ToList();

        IEnumerator<KeyValuePair<int, IResponseDatabase>> IEnumerable<KeyValuePair<int, IResponseDatabase>>.GetEnumerator()
        {
            return Results.GetEnumerator();
        }

        public IEnumerator GetEnumerator()
        {
            return Results.GetEnumerator();
        }
    }
}