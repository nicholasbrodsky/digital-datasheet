using System.ComponentModel.DataAnnotations;

namespace DigitalDatasheetEntityLib
{
    public class JobData
    {
        #region Primary Key Vars
        [Required]
        public string WorkOrderNumber { get; set; }
        [Required]
        public string TestCondition { get; set; }
        [Required]
        public string TestPerformedOn { get; set; }
        [Required]
        public string StructureTitle { get; set; }
        [Required]
        public string SerialNumber { get; set; }
        [Required]
        public string Location { get; set; }
        #endregion Primary Key Vars
        #region Measurements Vars
        public string HoleCuPlating { get; set; }
        public string ExternalConductor { get; set; }
        public string SurfaceCladCu { get; set; }
        //public string SelectivePlate { get; set; }
        public string WrapCu { get; set; }
        public string CapCu { get; set; }
        public string InternalCladCu { get; set; }
        public string MinEtchback { get; set; }
        public string MaxEtchback { get; set; }
        public string InternalAnnularRing { get; set; }
        public string ExternalAnnularRing { get; set; }
        public string Dielectric { get; set; }
        public string Wicking { get; set; }
        #endregion Measurement Vars
        #region Observation Vars
        public string InnerlayerSeparation { get; set; }
        public string PlatingCrack { get; set; }
        public string PlatingVoid { get; set; }
        //public string FoilCrack { get; set; }
        public string DelamBlisters { get; set; }
        public string LaminateVoidCrack { get; set; }
        public string AcceptReject { get; set; }
        #endregion Observation Vars
        #region Placement Vars
        public int Row { get; set; }
        public int StructureOrder { get; set; }
        #endregion Placement Vars
    }
}