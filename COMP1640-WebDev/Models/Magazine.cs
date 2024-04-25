using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace COMP1640_WebDev.Models
{
    public class Magazine
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        [Required(ErrorMessage ="Title of Magazine can not be null")]
        [StringLength(255)]
        public string? Title {  get; set; } 
        public string? Description { get; set; }
        [Required(ErrorMessage = "Please select cover image for magazine")]
        public byte[]? CoverImage { get; set; }


        [Required (ErrorMessage ="Please select Faculty")]
		[Display(Name = "Faculty")]
        [ForeignKey("Faculty")]
        public required string FacultyId { get; set; } 
        public Faculty? Faculty { get; set; }

		[Required(ErrorMessage = "Please select Semester")]
		[Display(Name = "AcademicYear")]
		[ForeignKey("AcademicYear")]
		public required string AcademicYearId { get; set; }
		public AcademicYear? AcademicYear { get; set; }
        //Cai nay de khi get detail magazine thi co list contributon


    }
}
