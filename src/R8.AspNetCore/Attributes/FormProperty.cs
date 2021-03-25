using System;

namespace R8.AspNetCore.Attributes
{
    /// <summary>
    /// An attribute to set alias name instead of property name, for using in <c>HttpContext.GetForm()</c>
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class FormProperty : Attribute
    {
        /// <summary>
        /// Gets or sets the name of the property.
        /// </summary>
        /// <value>The name of the property.</value>
        public string PropertyName { get; set; }
    }
}