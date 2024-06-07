using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Core.Entities
{
    public class Projection
    {
        public int ProjectionId { get; set; }
        public string Image { get; set; }
        public string Text { get; set; }
        public string? predicted_label { get; set; }

        // Foreign key property
        public string AppUserId { get; set; }

        // Navigation property for the many-to-one relationship with AppUser
        public AppUser AppUser { get; set; }

        public override string ToString()
        {
            return Text;
        }
    }
}
