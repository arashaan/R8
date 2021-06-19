using System;
using System.Collections.Generic;
using System.Text;
using R8.EntityFrameworkCore.EntityBases;

namespace R8.EntityFrameworkCore.Test.FakeDatabase.FakeEntities
{
    public class Role : EntityLocalized<Role>
    {
        public virtual ICollection<User> Users { get; set; }
    }
}