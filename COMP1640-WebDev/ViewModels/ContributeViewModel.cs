using COMP1640_WebDev.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace COMP1640_WebDev.ViewModels
{
	public class ContributeViewModel
	{
        public string Id { get; set; }
        public string AcademicYearId { get; set; }
        public IEnumerable<AcademicYear>? AcademicYears { get; set; }
        public string? MagazineId { get; set; }

        public string Title { get; set; }
        public string? Document { get; set; }

        [Display(Name = "File")]
        public IFormFile? FormFile { get; set; }
        [Display(Name = "Word")]
        public IFormFile? FormFileWord { get; set; }

    }
}