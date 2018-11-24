using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rexxar.Identity.Data
{
    public class RexxarDbContext : IdentityDbContext<RexxarUser, RexxarRole, Guid>
    {
        public RexxarDbContext(DbContextOptions<RexxarDbContext> options) : base(options)
        {
        }
    }
}
