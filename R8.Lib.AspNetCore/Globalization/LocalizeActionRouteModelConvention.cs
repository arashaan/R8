using System.Linq;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using R8.Lib.AspNetCore.Base;

namespace R8.Lib.AspNetCore.Globalization
{
    public class LocalizeActionRouteModelConvention : IApplicationModelConvention
    {
        private readonly AttributeRouteModel _routePrefix;

        public LocalizeActionRouteModelConvention()
        {
            _routePrefix = new AttributeRouteModel(new RouteAttribute(LanguageRouteConstraint.Key));
        }

        public void Apply(ApplicationModel application)
        {
            foreach (var selector in application.Controllers.SelectMany(c => c.Selectors))
            {
                selector.AttributeRouteModel = selector.AttributeRouteModel != null
                    ? AttributeRouteModel.CombineAttributeRouteModel(_routePrefix, selector.AttributeRouteModel)
                    : _routePrefix;
            }
        }
    }
}