using Microsoft.AspNetCore.Mvc.ApplicationModels;

using R8.Lib.AspNetCore.Base;

using System.Linq;

namespace R8.Lib.AspNetCore.Globalization
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