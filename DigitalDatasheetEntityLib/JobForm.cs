using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DigitalDatasheetEntityLib
{
    public class JobForm
    {
        #region Primary Key Vars
        [Required]
        public string WorkOrderNumber { get; set; }
        [Required]
        public string TestCondition { get; set; }
        [Required]
        public string TestPerformedOn { get; set; }
        #endregion Primary Key Vars
        #region Job Form Vars
        public int? CustomerMountQty { get; set; }
        [Column(TypeName = "TEXT")]
        public DateTime? DateTested { get; set; }
        public string TestedBy { get; set; }
        public string CheckedBy { get; set; }
        public string PartNumber { get; set; }
        public string LotNumber { get; set; }
        public string Customer { get; set; }
        public string DateCode { get; set; }
        public string Specification1 { get; set; }
        public string Specification2 { get; set; }
        public string BoardType { get; set; }
        public string TestProcedure { get; set; }
        public int DrawingProvided { get; set; }
        public string EvaluatedBy { get; set; }
        [Column(TypeName = "TEXT")]
        public DateTime? DateEvaluated { get; set; }
        public string BakeTimeIn { get; set; }
        public string BakeTimeOut { get; set; }
        public int? TotalTime { get; set; }
        public int? TestTemp { get; set; }
        public int? SolderFloats { get; set; }
        [Column(TypeName = "TEXT")]
        public DateTime? LastSaved { get; set; }
        #endregion Job Form Vars
    }
}
