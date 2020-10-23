using System;
using System.Collections;
using Newtonsoft.Json;

using R8.Lib.Enums;

using System.Collections.Generic;
using System.Linq;

namespace R8.Lib.MethodReturn
{
    /// <summary>
    /// Represents a collection of responses with sub-results
    /// </summary>
    public class ResponseCollection : IList<IResponse>, IResponseBase
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
        /// <param name="response">Add single instance of <see cref="IResponse"/></param>
        public ResponseCollection(IResponse response)
        {
            this.Add(response);
        }

        /// <summary>
        /// Represents a collection of responses with sub-results
        /// </summary>
        /// <param name="collection">Add collection of <see cref="IResponse"/> to instance</param>
        public ResponseCollection(IEnumerable<IResponse> collection)
        {
            if (collection == null) 
                throw new ArgumentNullException(nameof(collection));
            
            this.AddRange(collection);
        }
        /// <summary>
        /// List of results with type of <see cref="IResponse"/>
        /// </summary>
        [JsonIgnore]
        public List<IResponse> Results { get; set; }
        public DatabaseSaveState? Save { get; set; }

        public void Add(IResponse model)
        {
            Results ??= new List<IResponse>();
            Results.Add(model);
        }

        public void Clear()
        {
            Results.Clear();
        }

        public bool Contains(IResponse item)
        {
            return Results.Contains(item);
        }

        public void CopyTo(IResponse[] array, int arrayIndex)
        {
            Results.CopyTo(array, arrayIndex);
        }

        public bool Remove(IResponse item)
        {
            return Results.Remove(item);
        }

        public int Count => Results.Count;
        public bool IsReadOnly => false;

        public void AddRange(IEnumerable<IResponse> collection)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            Results ??= new List<IResponse>();
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

        public IEnumerator<IResponse> GetEnumerator()
        {
            return Results.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int IndexOf(IResponse item)
        {
            return Results.IndexOf(item);
        }

        public void Insert(int index, IResponse item)
        {
            Results.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            Results.RemoveAt(index);
        }

        public IResponse this[int index]
        {
            get => Results[index];
            set => Results[index] = value;
        }
    }
}