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
    public class ResponseCollection<T> : IDictionary<int, T>, IResponseTrack where T : IResponseBaseDatabase
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
        /// <param name="responseBase">Add single instance of <see cref="IResponseBaseDatabase"/></param>
        public ResponseCollection(T responseBase)
        {
            this.Add(responseBase);
        }

        /// <summary>
        /// Represents a collection of responses with sub-results
        /// </summary>
        /// <param name="collection">Add collection of <see cref="IResponseBaseDatabase"/> to instance</param>
        public ResponseCollection(IEnumerable<T> collection)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            this.AddRange(collection);
        }

        /// <summary>
        /// List of results with type of <see cref="T"/>
        /// </summary>
        [JsonIgnore]
        public Dictionary<int, T> Results { get; set; }

        /// <summary>
        /// Gets or sets Save State into DbContext
        /// </summary>
        public DatabaseSaveState? Save { get; set; }

        /// <summary>
        /// An instance of <see cref="IResponseBaseDatabase"/> as Head of the Collection that it is first iterate of <see cref="Results"/>
        /// </summary>
        public T Head => Results.FirstOrDefault().Value;

        /// <summary>
        /// Add an item to the <see cref="ICollection{T}"/>
        /// </summary>
        /// <param name="model">The object to add to <see cref="ICollection{T}"/></param>
        public void Add(T model)
        {
            Results ??= new Dictionary<int, T>();
            Results.Add(Results.Count, model);
        }

        public void Add(KeyValuePair<int, T> item)
        {
            Results ??= new Dictionary<int, T>();
            var (key, value) = item;
            Results.Add(key, value);
        }

        public void Clear()
        {
            Results.Clear();
        }

        public bool Contains(KeyValuePair<int, T> item)
        {
            return Results.Contains(item);
        }

        public void CopyTo(KeyValuePair<int, T>[] array, int arrayIndex)
        {
            return;
        }

        public bool Remove(KeyValuePair<int, T> item)
        {
            return Results.Remove(item.Key);
        }

        public int Count => Results.Count;
        public bool IsReadOnly => false;

        public void AddRange(IEnumerable<T> collection)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            Results ??= new Dictionary<int, T>();
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

        public static implicit operator bool(ResponseCollection<T> response)
        {
            return response.Success;
        }

        public ValidatableResultCollection Errors =>
            (ValidatableResultCollection)Results?.SelectMany(x => x.Value.Errors).ToList();

        public void Add(int key, T value)
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

        public bool TryGetValue(int key, out T value)
        {
            return Results.TryGetValue(key, out value);
        }

        public T this[int index]
        {
            get => Results[index];
            set => Results[index] = value;
        }

        public ICollection<int> Keys => Results.Select(x => x.Key).ToList();
        public ICollection<T> Values => Results.Select(x => x.Value).ToList();

        IEnumerator<KeyValuePair<int, T>> IEnumerable<KeyValuePair<int, T>>.GetEnumerator()
        {
            return Results.GetEnumerator();
        }

        public IEnumerator GetEnumerator()
        {
            return Results.GetEnumerator();
        }
    }

    /// <summary>
    /// Represents a collection of responses with sub-results
    /// </summary>
    public class ResponseCollection : IDictionary<int, IResponseBaseDatabase>, IResponseTrack
    {
        /// <summary>
        /// Represents a collection of responses with sub-results
        /// </summary>
        public ResponseCollection()
        {
        }

        ///// <summary>
        ///// Represents a collection of responses with sub-results
        ///// </summary>
        ///// <param name="status">Set default <see cref="Flags"/> to collection instance</param>
        //public ResponseCollection(object status)
        //{
        //    this.Add(new ResponseBaseDatabase(status));
        //}

        /// <summary>
        /// Represents a collection of responses with sub-results
        /// </summary>
        /// <param name="responseBase">Add single instance of <see cref="IResponseBaseDatabase"/></param>
        public ResponseCollection(IResponseBaseDatabase responseBase)
        {
            this.Add(responseBase);
        }

        /// <summary>
        /// Represents a collection of responses with sub-results
        /// </summary>
        /// <param name="collection">Add collection of <see cref="IResponseBaseDatabase"/> to instance</param>
        public ResponseCollection(IEnumerable<IResponseBaseDatabase> collection)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            this.AddRange(collection);
        }

        /// <summary>
        /// List of results with type of <see cref="IResponseBaseDatabase"/>
        /// </summary>
        [JsonIgnore]
        public Dictionary<int, IResponseBaseDatabase> Results { get; set; }

        /// <summary>
        /// Gets or sets Save State into DbContext
        /// </summary>
        public DatabaseSaveState? Save { get; set; }

        /// <summary>
        /// An instance of <see cref="IResponseBaseDatabase"/> as Head of the Collection that it is first iterate of <see cref="Results"/>
        /// </summary>
        public IResponseBaseDatabase? Head => Results[0];

        /// <summary>
        /// Add an item to the <see cref="ICollection{T}"/>
        /// </summary>
        /// <param name="model">The object to add to <see cref="ICollection{T}"/></param>
        public void Add(IResponseBaseDatabase model)
        {
            Results ??= new Dictionary<int, IResponseBaseDatabase>();
            Results.Add(Results.Count, model);
        }

        public void Add(KeyValuePair<int, IResponseBaseDatabase> item)
        {
            Results ??= new Dictionary<int, IResponseBaseDatabase>();
            var (key, value) = item;
            Results.Add(key, value);
        }

        public void Clear()
        {
            Results.Clear();
        }

        public bool Contains(KeyValuePair<int, IResponseBaseDatabase> item)
        {
            return Results.Contains(item);
        }

        public void CopyTo(KeyValuePair<int, IResponseBaseDatabase>[] array, int arrayIndex)
        {
            return;
        }

        public bool Remove(KeyValuePair<int, IResponseBaseDatabase> item)
        {
            return Results.Remove(item.Key);
        }

        public int Count => Results.Count;
        public bool IsReadOnly => false;

        public void AddRange(IEnumerable<IResponseBaseDatabase> collection)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            Results ??= new Dictionary<int, IResponseBaseDatabase>();
            foreach (var response in collection)
                Results.Add(Results.Count, response);
        }

        // public void Add<T>(object status) where T : class
        // {
        //     this.Add(new ResponseBase<T>(status));
        // }

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

        public static implicit operator bool(ResponseCollection response)
        {
            return response.Success;
        }

        public ValidatableResultCollection Errors =>
            (ValidatableResultCollection)Results?.SelectMany(x => x.Value.Errors).ToList();

        public void Add(int key, IResponseBaseDatabase value)
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

        public bool TryGetValue(int key, out IResponseBaseDatabase value)
        {
            return Results.TryGetValue(key, out value);
        }

        public IResponseBaseDatabase this[int index]
        {
            get => Results[index];
            set => Results[index] = value;
        }

        public ICollection<int> Keys => Results.Select(x => x.Key).ToList();
        public ICollection<IResponseBaseDatabase> Values => Results.Select(x => x.Value).ToList();

        IEnumerator<KeyValuePair<int, IResponseBaseDatabase>> IEnumerable<KeyValuePair<int, IResponseBaseDatabase>>.GetEnumerator()
        {
            return Results.GetEnumerator();
        }

        public IEnumerator GetEnumerator()
        {
            return Results.GetEnumerator();
        }
    }
}