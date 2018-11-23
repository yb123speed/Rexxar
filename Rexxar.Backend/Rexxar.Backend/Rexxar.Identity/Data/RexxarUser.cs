using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rexxar.Identity.Data
{
    public class RexxarUser:IdentityUser<Guid>
    {
        public string Avatar { get; set; }
    }
}
