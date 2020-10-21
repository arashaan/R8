using Microsoft.AspNetCore.Mvc.ApplicationModels;

using System.Linq;
using R8.Lib.AspNetCore.Base;

namespace R8.Lib.AspNetCore.Globalization
{
    public class LocalizedPageRouteModelConvention : IPageRouteModelConvention
    {
        // private readonly List<LocalRoute> _routes;
        //
        // public LocalizedPageRouteModelConvention()
        // {
        //     _routes ??= new List<LocalRoute>();
        //     _routes.Add(new LocalRoute
        //     {
        //         Page = "/Pages/Contact.cshtml",
        //         LocalizedPageNames = new List<string> { "iletisim" }
        //     });
        // }

        public void Apply(PageRouteModel model)
        {
            // var selectorCount = model.Selectors.Count;
            // var route = _routes.FirstOrDefault(p => p.Page == model.RelativePath);
            // if (route != null)
            // {
            //     foreach (var localizedPageName in route.LocalizedPageNames)
            //     {
            //         model.Selectors.Add(new SelectorModel
            //         {
            //             AttributeRouteModel = new AttributeRouteModel
            //             {
            //                 Template = localizedPageName
            //             }
            //         });
            //     }
            // }

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