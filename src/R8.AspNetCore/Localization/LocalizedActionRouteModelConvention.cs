using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

using System.Linq;

namespace R8.AspNetCore.Localization
{
    public class LocalizedActionRouteModelConvention : IApplicationModelConvention
    {
        private readonly AttributeRouteModel _routePrefix;

        public LocalizedActionRouteModelConvention()
        {
            _routePrefix = new AttributeRouteModel(new RouteAttribute(R8.AspNetCore.Localization.Constraints.LanguageKey));
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