using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Swashbuckle.AspNetCore.Annotations;

namespace AddressAPI.Model
{
    [Table("Address")]
    public class Address
    {
        [Key]
        [Required]
        [SwaggerSchema(ReadOnly = true)]
        public int AddressId { get; set; }

        [StringLength(50)]
        public string Street { get; set; }

        public int HouseNumber { get; set; }

        [StringLength(3)]
        public string Annex { get; set; }

        [StringLength(7)]
        public string PostalCode { get; set; }

        public string City { get; set; }

        public string Country { get; set; }
    }
}
