using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CapstonePRS.Models
{
    [Index("PartNbr", IsUnique = true, Name = "UIDX_PartNbr")]
    public class Product
    {

        public int Id { get; set; }

        [StringLength(30)]
        public string PartNbr { get; set; }

        [StringLength(30)]
        public string Name { get; set; }

        [Column(TypeName = "decimal(11,2)")]
        public decimal Price { get; set; }

        [StringLength(30)]
        public string Unit { get; set; }

        [StringLength(255)]
        public string? PhotoPath { get; set; }

        public int VendorId { get; set; }
        public virtual Vendor? Vendor { get; set; }



    }
}
