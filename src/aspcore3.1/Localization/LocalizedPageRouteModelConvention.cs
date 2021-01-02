using System.Linq;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace R8.AspNetCore.Localization
{
    public class LocalizedPageRouteModelConvention : IPageRouteModelConvention
    {
        public void Apply(PageRouteModel model)
        {
            foreach (var selector in model.Selectors.ToList())
            {
                model.Selectors.Add(new SelectorModel
                {
                    AttributeRouteModel = new AttributeRouteModel
                    {
                        Order = -1,
                        Template = AttributeRouteModel
                            .CombineTemplates("{" + LanguageRouteConstraint.Key + "?}",
                                selector.AttributeRouteModel.Template),
                    }
                });
            }
        }
    }
}