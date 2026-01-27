using CHMS.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CHMS.Domain.Entities
{
    public class HomestayAmenity
    {
        public Guid HomestayId { get; set; }
        public Guid AmenityId { get; set; }
        public virtual Homestay Homestay { get; set; } = null!;
        public virtual Amenity Amenity { get; set; } = null!;
    }
}
