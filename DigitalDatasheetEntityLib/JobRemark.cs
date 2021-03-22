using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DigitalDatasheetEntityLib
{
    public class JobRemark
    {
        #region Primary Key Vars
        [Required]
        public string WorkOrderNumber { get; set; }
        [Required]
        public string TestCondition { get; set; }
        [Required]
        public string TestPerformedOn { get; set; }
        [Required]
        public string Remark { get; set; }
        #endregion Primary Key Vars
        [Column(TypeName = "INTEGER")]
        public bool Reject { get; set; }
        public int Row { get; set; }
    }
}