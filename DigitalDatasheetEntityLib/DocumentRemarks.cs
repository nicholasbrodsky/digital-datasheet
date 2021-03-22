using System.ComponentModel.DataAnnotations;

namespace DigitalDatasheetEntityLib
{
    public class DocumentRemark
    {
        [Key]
        [Required]
        public int ID { get; set; }
        [Required]
        public string SectionTitle { get; set; }
        public string Remark { get; set; }
    }
}