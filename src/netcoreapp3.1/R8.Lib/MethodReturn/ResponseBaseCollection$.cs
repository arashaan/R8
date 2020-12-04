using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json;

using R8.Lib.Enums;

namespace R8.Lib.MethodReturn
{
    /// <summary>
    /// Represents a collection of responses with sub-results
    /// </summary>
    /// <typeparam name="TStatus">A type that representing status type.</typeparam>
    public class ResponseBaseCollection<TStatus> : IDictionary<int, IResponseBaseDatabase<TStatus>>, IResponseErrors
    {
        /// <summary>
        /// Represents a collection of responses with sub-results
        /// </summary>
        public ResponseBaseCollection()
        {
        }

        /// <summary>
        /// Represents a collection of responses with sub-results
        /// </summary>
        /// <param name="responseBase">Add single instance of <see cref="IResponseBaseDatabase"/></param>
        public ResponseBaseCollection(IResponseBaseDatabase<TStatus> responseBase)
        {
            this.Add(responseBase);
        }

        /// <summary>
        /// Represents a collection of responses with sub-results
        /// </summary>
        /// <param name="collection">Add collection of <see cref="IResponseBaseDatabase"/> to instance</param>
        public ResponseBaseCollection(IEnumerable<IResponseBaseDatabase<TStatus>> collection)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            this.AddRange(collection);
        }

        /// <summary>
        /// List of results with type of <see cref="IResponseBaseDatabase"/>
        /// </summary>
        [JsonIgnore]
        public virtual Dictionary<int, IResponseBaseDatabase<TStatus>> Results { get; set; }

        /// <summary>
        /// Gets or sets Save State into DbContext
        /// </summary>
        public virtual DatabaseSaveState? Save { get; set; }

        /// <summary>
        /// An instance of <see cref="IResponseBaseDatabase"/> as Head of the Collection that it is first iterate of <see cref="Results"/>
        /// </summary>
        public virtual IResponseBaseDatabase<TStatus>? Head => Results[0];

        /// <summary>
        /// Add an item to the <see cref="ICollection{T}"/>
        /// </summary>
        /// <param name="model">The object to add to <see cref="ICollection{T}"/></param>
        public void Add(IResponseBaseDatabase<TStatus> model)
        {
            Results ??= new Dictionary<int, IResponseBaseDatabase<TStatus>>();
            Results.Add(Results.Count, model);
        }

        public void Add(KeyValuePair<int, IResponseBaseDatabase<TStatus>> item)
        {
            Results ??= new Dictionary<int, IResponseBaseDatabase<TStatus>>();
            var (key, value) = item;
            Results.Add(key, value);
        }

        public void Clear()
        {
            Results.Clear();
        }

        public bool Contains(KeyValuePair<int, IResponseBaseDatabase<TStatus>> item)
        {
            return Results.Contains(item);
        }

        public void CopyTo(KeyValuePair<int, IResponseBaseDatabase<TStatus>>[] array, int arrayIndex)
        {
            return;
        }

        public bool Remove(KeyValuePair<int, IResponseBaseDatabase<TStatus>> item)
        {
            return Results.Remove(item.Key);
        }

        public int Count => Results.Count;
        public bool IsReadOnly => false;

        public void AddRange(IEnumerable<IResponseBaseDatabase<TStatus>> collection)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            Results ??= new Dictionary<int, IResponseBaseDatabase<TStatus>>();
            foreach (var response in collection)
                Results.Add(Results.Count, response);
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

        public static implicit operator bool(ResponseBaseCollection<TStatus> responseBase)
        {
            return responseBase.Success;
        }

        public ValidatableResultCollection Errors =>
            (ValidatableResultCollection)Results?.SelectMany(x => x.Value.Errors).ToList();

        public void Add(int key, IResponseBaseDatabase<TStatus> value)
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

        public bool TryGetValue(int key, out IResponseBaseDatabase<TStatus> value)
        {
            return Results.TryGetValue(key, out value);
        }

        public IResponseBaseDatabase<TStatus> this[int index]
        {
            get => Results[index];
            set => Results[index] = value;
        }

        public ICollection<int> Keys => Results.Select(x => x.Key).ToList();
        public ICollection<IResponseBaseDatabase<TStatus>> Values => Results.Select(x => x.Value).ToList();

        IEnumerator<KeyValuePair<int, IResponseBaseDatabase<TStatus>>> IEnumerable<KeyValuePair<int, IResponseBaseDatabase<TStatus>>>.GetEnumerator()
        {
            return Results.GetEnumerator();
        }

        public IEnumerator GetEnumerator()
        {
            return Results.GetEnumerator();
        }
    }
}