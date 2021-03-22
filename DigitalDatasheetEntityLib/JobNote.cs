using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DigitalDatasheetEntityLib
{
    public class JobNote
    {
        #region Primary Key Vars
        [Required]
        public string WorkOrderNumber { get; set; }
        [Required]
        public string TestCondition { get; set; }
        [Required]
        public string TestPerformedOn { get; set; }
        [Required]
        public string User { get; set; }
        [Required]
        [Column(TypeName = "TEXT")]
        public DateTime DateAdded { get; set; }
        #endregion Primary Key Vars

        [Required]
        [Column(TypeName = "TEXT")]
        public DateTime DateUpdated { get; set; }
        [Required]
        public string Note { get; set; }
        [Column(TypeName = "INTEGER")]
        public bool Completed { get; set; }
    }
}
