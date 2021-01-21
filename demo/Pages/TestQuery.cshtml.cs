using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using R8.AspNetCore3_1.Demo.Services.Routing;

namespace R8.AspNetCore3_1.Demo.Pages
{
    public class TestQueryModel : PageModel
    {
        public TestQueryModel()
        {
            PageTitle = "TEST ACTIVE";
        }

        public void OnGet()
        {
        }
    }
}