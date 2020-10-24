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
    public class ResponseCollection : IList<IResponseDatabase>, IResponseBase
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
        public List<IResponseDatabase> Results { get; set; }

        public DatabaseSaveState? Save { get; set; }

        public void Add(IResponseDatabase model)
        {
            Results ??= new List<IResponseDatabase>();
            Results.Add(model);
        }

        public void Clear()
        {
            Results.Clear();
        }

        public bool Contains(IResponseDatabase item)
        {
            return Results.Contains(item);
        }

        public void CopyTo(IResponseDatabase[] array, int arrayIndex)
        {
            Results.CopyTo(array, arrayIndex);
        }

        public bool Remove(IResponseDatabase item)
        {
            return Results.Remove(item);
        }

        public int Count => Results.Count;
        public bool IsReadOnly => false;

        public void AddRange(IEnumerable<IResponseDatabase> collection)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            Results ??= new List<IResponseDatabase>();
            foreach (var response in collection)
            {
                Results.Add(response);
            }
        }

        public void Add<T>(Flags status) where T : class
        {
            this.Add(new Response<T>(status));
        }

        public bool Success
        {
            get
            {
                var baseCondition = Results != null && Results.Any() && Results.All(x => x.Success);
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
            (ValidatableResultCollection)Results?.SelectMany(x => x.Errors).ToList();

        public IEnumerator<IResponseDatabase> GetEnumerator()
        {
            return Results.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int IndexOf(IResponseDatabase item)
        {
            return Results.IndexOf(item);
        }

        public void Insert(int index, IResponseDatabase item)
        {
            Results.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            Results.RemoveAt(index);
        }

        public IResponseDatabase this[int index]
        {
            get => Results[index];
            set => Results[index] = value;
        }
    }
}