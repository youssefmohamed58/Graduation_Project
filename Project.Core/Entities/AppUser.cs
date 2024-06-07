using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Project.Core.Entities
{
    public class AppUser : IdentityUser
    {
        public string FullName {  get; set; }

        public int Code { get; set; }

        public int ConfirmCode { get; set; }

        public string? Token { get; set; }

        public string? PictureUrl { get; set; }
        public ICollection<Projection> Projections { get; set; }
    }
}
