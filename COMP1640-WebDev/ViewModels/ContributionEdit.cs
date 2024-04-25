using COMP1640_WebDev.Models;
using System.ComponentModel.DataAnnotations;

namespace COMP1640_WebDev.ViewModels
{
    public class ContributionEdit
    {
        public string Id { get; set; }
        public string AcademicYearId { get; set; }
        public string MagazineId { get; set; }
        public IEnumerable<AcademicYear>? AcademicYears { get; set; }

        public string Title { get; set; }
    
    }
}
