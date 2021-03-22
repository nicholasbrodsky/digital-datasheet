using System.ComponentModel.DataAnnotations;

namespace DigitalDatasheetEntityLib
{
    public class JobRequirements
    {
        #region Primary Key Vars
        [Required]
        public string WorkOrderNumber { get; set; }
        [Required]
        public string TestCondition { get; set; }
        [Required]
        public string TestPerformedOn { get; set; }
        #endregion Primary Key Vars
        #region Requiremet Vars
        public string HoleCuPlating { get; set; }
        public string ExternalConductor { get; set; }
        public string SurfaceCladCu { get; set; }
        public string WrapCu { get; set; }
        public string CapCu { get; set; }
        public string InternalCladCu { get; set; }
        public string MinEtchback { get; set; }
        public string MaxEtchback { get; set; }
        public string InternalAnnularRing { get; set; }
        public string ExternalAnnularRing { get; set; }
        public string Dielectric { get; set; }
        public string Wicking { get; set; }
        #endregion Requirement Vars
    }
}