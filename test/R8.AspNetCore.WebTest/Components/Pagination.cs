using Microsoft.AspNetCore.Mvc;
using R8.AspNetCore.Routing;

namespace R8.AspNetCore.WebTest.Components
{
    public class Pagination : ViewComponent
    {
        public IViewComponentResult Invoke() => this.InvokePagination("~/", "List");
    }
}