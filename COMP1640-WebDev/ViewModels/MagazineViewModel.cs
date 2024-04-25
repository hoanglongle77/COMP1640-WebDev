using COMP1640_WebDev.Models;
using System.ComponentModel.DataAnnotations;

namespace COMP1640_WebDev.ViewModels
{
	public class MagazineViewModel
	{
		public string? Id { get; set; }


		[Required(ErrorMessage = "Title can not be null")]
		public string Title { get; set; }
		[Required(ErrorMessage = "Description can not be null")]
		public string Description { get; set; }

		[Required(ErrorMessage = "FacultyId can not be null")]
		public string FacultyId { get; set; }
		[Required(ErrorMessage = "AcademicYearId can not be null")]
		public string AcademicYearId { get; set; }

		public IEnumerable<Faculty>? Falulties { get; set; }
		public IEnumerable<AcademicYear>? AcademicYears { get; set; } 
		[Display(Name = "File")]
		[Required(ErrorMessage = "FormFile can not be null")]

		public IFormFile? FormFile { get; set; }

		
	}
}
