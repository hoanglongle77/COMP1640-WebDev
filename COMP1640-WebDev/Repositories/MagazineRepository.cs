using COMP1640_WebDev.Data;
using COMP1640_WebDev.Models;
using COMP1640_WebDev.Repositories.Interfaces;
using COMP1640_WebDev.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Plugins;


namespace COMP1640_WebDev.Repositories
{
    public class MagazineRepository : IMagazineRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public MagazineRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // 1. Get all magazines
        public async Task<IEnumerable<Magazine>> GetMagazines()
        {
            return await _dbContext.Magazines!.ToListAsync();
        }

        // 2. Get all magazines with view model
        public IEnumerable<MagazineTableView> GetAllMagazines()
        {
            var magazines = _dbContext.Magazines!.Select(c => new MagazineTableView()
            {
                Id = c.Id,
                Title = c.Title,
                Faculty = c.Faculty!.FacultyName,
                AcademicYear = c.AcademicYear!.StartDate.ToString()
            }).ToList();

            return magazines;
        }

        // 3. Search magazine by attribute and value
        public IEnumerable<MagazineTableView> SearchMagazines(string attribute, string value)
        {
            var magazines = GetAllMagazines();
            if (!string.IsNullOrEmpty(attribute) && !string.IsNullOrEmpty(value))
            {
                switch (attribute)
                {
                    case "Title":
                        magazines = magazines.Where(u => u.Title != null && u.Title.Contains(value));
                        break;
                    case "Faculty":
                        magazines = magazines.Where(u => u.Faculty != null && u.Faculty.Contains(value));
                        break;
                    default:
                        break;
                }

  
            }
            return magazines;
        }

        // 5. Get magazine view model
        public MagazineViewModel GetMagazineViewModel()
        {
            var viewModel = new MagazineViewModel()
            {
                Falulties = _dbContext.Faculties!.ToList(),
                AcademicYears = _dbContext.AcademicYears!.ToList(),
            };
            return viewModel;
        }

        // 6. Get magazine view model by id
        public MagazineViewModel GetMagazineViewModelByID(string idMagazine)
        {
            throw new NotImplementedException();
        }

        // 7. Get magazine by id
        public async Task<Magazine> GetMagazineByID(string id)
        {
            var magazineInDB = await _dbContext.Magazines!
               .Include(u => u.Faculty)
            .Include(u => u.AcademicYear)
            .SingleOrDefaultAsync(i => i.Id == id);

            if (magazineInDB == null)
            {
                throw new InvalidOperationException($"Magazine with ID {id} not found.");
            }

            return magazineInDB;
        }

        // 8. Create new magazine
        public async Task<Magazine> CreateMagazine(Magazine magazine, IFormFile? formFile)
        {
            using (var memoryStream = new MemoryStream())
            {
                await formFile!.CopyToAsync(memoryStream);

                magazine.CoverImage = memoryStream.ToArray(); 
           

                var result = await _dbContext.Magazines!.AddAsync(magazine);
                await _dbContext.SaveChangesAsync();
                return result.Entity;
            }
        }

        // 9. Update magazine
        public async Task<Magazine> UpdateMagazine(Magazine magazine, IFormFile? formFile)
        {
            using (var memoryStream = new MemoryStream())
            {
                await formFile!.CopyToAsync(memoryStream);

                magazine.CoverImage = memoryStream.ToArray();


                var result =  _dbContext.Magazines!.Update(magazine);
                await _dbContext.SaveChangesAsync();
                return result.Entity;
            }
        }


        // 10. Remove magazine
        public async Task<Magazine> RemoveMagazine(string id)
        {
            var magazine = await _dbContext.Magazines!.FindAsync(id);
            if (magazine == null)
            {
                throw new KeyNotFoundException($"Magazine with ID {id} not found.");
            }

            _dbContext.Magazines.Remove(magazine);
            await _dbContext.SaveChangesAsync();
            return magazine;
        }

        // 10. Search magazine by title 
        public List<MagazineTableView> SearchMagazinesByTitle(string title)
        {
            var magazines = _dbContext.Magazines!
                .Where(m => m.Title!.Contains(title, StringComparison.OrdinalIgnoreCase))
                .Select(m => new MagazineTableView
                {
                    Id = m.Id,
                    CoverImage = ConvertByteArrayToStringBase64(m.CoverImage!),
                    Title = m.Title,
                    Description = m.Description,
                    Faculty = m.Faculty!.FacultyName
                }).ToList();

            return magazines;
        }

        // 11. Get all magazines with by faculty
        public List<MagazineTableView> GetAllMagazinesByFaculty(string userFacultyId)
        {
            var magazinesWithImage = _dbContext.Magazines!
                .Where(m => m.FacultyId == userFacultyId)
                .Select(m => new MagazineTableView
                {
                    Id = m.Id,
                    CoverImage = ConvertByteArrayToStringBase64(m.CoverImage!),
                    Title = m.Title,
                    Description = m.Description,
                    Faculty = m.Faculty!.FacultyName 
                }).ToList();

            return magazinesWithImage;
        }

        // 12. Get all magazines
        public List<MagazineTableView> GetAllMagazinesForGuest()
        {
            var magazinesWithImage = _dbContext.Magazines!
                .Select(m => new MagazineTableView
                {
                    Id = m.Id,
                    CoverImage = ConvertByteArrayToStringBase64(m.CoverImage!),
                    Title = m.Title,
                    Description = m.Description,
                    Faculty = m.Faculty != null ? m.Faculty.FacultyName : string.Empty
                })
                .ToList();

            return magazinesWithImage;
        }


        // 12. Get all magazines with image
        private static string ConvertByteArrayToStringBase64(byte[] imageArray)
        {
            if (imageArray == null || imageArray.Length == 0)
            {
                return null!; 
            }

            string imageBase64Data = Convert.ToBase64String(imageArray);
            return string.Format("data:image/jpg;base64,{0}", imageBase64Data);
        }

        public Task<int> CountMagazines()
        {
            return _dbContext.Magazines!.CountAsync();
        }
    }
}
