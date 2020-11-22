using R8.EntityFrameworkCore;

namespace R8.AspNetCore.Demo.Services.Database.Entities
{
    public class Translation : EntityLocalized<Translation>
    {
        public string Key { get; set; }
    }
}