using COMP1640_WebDev.Data;
using COMP1640_WebDev.Models;
using COMP1640_WebDev.ViewModels;

namespace COMP1640_WebDev.Repositories.Interfaces
{
    public interface IMagazineRepository
    {
		MagazineViewModel GetMagazineViewModel();
		MagazineViewModel GetMagazineViewModelByID(string idMagazine);
        IEnumerable<MagazineTableView> GetAllMagazines();
        IEnumerable<MagazineTableView> SearchMagazines(string attribute, string value);
        Task<IEnumerable<Magazine>> GetMagazines();
        Task<Magazine> GetMagazineByID(string id);
        Task<Magazine> CreateMagazine(Magazine magazine, IFormFile? formFile);
        Task<Magazine> UpdateMagazine(Magazine magazine, IFormFile? formFile);
        Task<Magazine> RemoveMagazine(string id);
        List<MagazineTableView> GetAllMagazinesByFaculty(string userFacultyId);
        List<MagazineTableView> GetAllMagazinesForGuest();
        List<MagazineTableView> SearchMagazinesByTitle(string title);
        Task<int> CountMagazines();
    }
}
