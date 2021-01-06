using System;
using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json;

namespace R8.Lib.MethodReturn
{
    ///// <summary>
    ///// Represents a collection of responses with sub-results
    ///// </summary>
    ///// <typeparam name="TStatus">A type that representing status type.</typeparam>
    //public class ResponseBaseCollection<TStatus> : IResponseErrors
    //{
    //    /// <summary>
    //    /// Represents a collection of responses with sub-results
    //    /// </summary>
    //    public ResponseBaseCollection()
    //    {
    //    }

    //    /// <summary>
    //    /// Represents a collection of responses with sub-results
    //    /// </summary>
    //    /// <param name="responseBase">Add single instance of <see cref="IResponseBaseDatabase{TStatus}"/></param>
    //    public ResponseBaseCollection(IResponseBaseDatabase<TStatus> responseBase)
    //    {
    //        this.Add(responseBase);
    //    }

    //    /// <summary>
    //    /// Represents a collection of responses with sub-results
    //    /// </summary>
    //    /// <param name="collection">Add collection of <see cref="IResponseBaseDatabase{TStatus}"/> to instance</param>
    //    public ResponseBaseCollection(IEnumerable<IResponseBaseDatabase<TStatus>> collection)
    //    {
    //        if (collection == null)
    //            throw new ArgumentNullException(nameof(collection));

    //        this.AddRange(collection);
    //    }

    //    /// <summary>
    //    /// List of results with type of <see cref="IResponseBaseDatabase{TStatus}"/>
    //    /// </summary>
    //    [JsonIgnore]
    //    public virtual List<IResponseBaseDatabase<TStatus>> Results { get; set; }

    //    /// <summary>
    //    /// An instance of <see cref="IResponseBaseDatabase{TStatus}"/> as Head of the Collection that it is first iterate of <see cref="Results"/>
    //    /// </summary>
    //    public virtual IResponseBaseDatabase<TStatus> Head => Results.FirstOrDefault();

    //    /// <summary>
    //    /// Add an item to the <see cref="ICollection{T}"/>
    //    /// </summary>
    //    /// <param name="model">The object to add to <see cref="ICollection{T}"/></param>
    //    public void Add(IResponseBaseDatabase<TStatus> model)
    //    {
    //        Results ??= new List<IResponseBaseDatabase<TStatus>>();
    //        Results.Add(model);
    //    }

    //    public void AddRange(IEnumerable<IResponseBaseDatabase<TStatus>> collection)
    //    {
    //        if (collection == null)
    //            throw new ArgumentNullException(nameof(collection));

    //        Results ??= new List<IResponseBaseDatabase<TStatus>>();
    //        foreach (var response in collection)
    //            Results.Add(response);
    //    }

    //    public bool Success => Results != null && Results.Any() && Results.All(x => x.Success);

    //    public static implicit operator bool(ResponseBaseCollection<TStatus> responseBase)
    //    {
    //        return responseBase.Success;
    //    }

    //    public ValidatableResultCollection Errors =>
    //        (ValidatableResultCollection)Results?.SelectMany(x => x.Errors).ToList();
    //}
}