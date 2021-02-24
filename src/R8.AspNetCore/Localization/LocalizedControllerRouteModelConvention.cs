using System.Linq;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace R8.AspNetCore.Localization
{
    public class LocalizedControllerRouteModelConvention : IControllerModelConvention
    {
        public void Apply(ControllerModel controller)
        {
            foreach (var selector in controller.Selectors.ToList())
            {
                controller.Selectors.Add(new SelectorModel
                {
                    AttributeRouteModel = new AttributeRouteModel
                    {
                        Order = -1,
                        Template = AttributeRouteModel
                            .CombineTemplates("{" + R8.AspNetCore.Localization.Constraints.LanguageKey + "?}",
                                selector.AttributeRouteModel.Template),
                    }
                });
            }
        }
    }
}