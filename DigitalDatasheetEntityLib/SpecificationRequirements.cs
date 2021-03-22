using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DigitalDatasheetEntityLib
{
    public class SpecificationRequirements
    {
        [Key]
        [Required]
        public string Specification { get; set; }
        public string HoleCuPlating { get; set; }
        public string WrapCu { get; set; }
        public string CapCu { get; set; }
        public string MinEtchback { get; set; }
        public string MaxEtchback { get; set; }
        public string InternalAnnularRing { get; set; }
        public string ExternalAnnularRing { get; set; }
        public string Dielectric { get; set; }
        public string Wicking { get; set; }
        [Column(TypeName = "INTEGER")]
        public bool WickingNote { get; set; }
        //public int SpecificationOrder { get; set; }
    }
}