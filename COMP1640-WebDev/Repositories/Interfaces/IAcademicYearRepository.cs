﻿using COMP1640_WebDev.Models;
using COMP1640_WebDev.ViewModels;

namespace COMP1640_WebDev.Repositories.Interfaces
{
    public interface IAcademicYearRepository
    {
        AcademicYearViewModel GetAcademicYearViewModel();
        AcademicYearViewModel GetAcademicYearViewModelByID(string idAcademicYear);
        Task<IEnumerable<AcademicYear>> GetAcademicYears();
        Task<AcademicYear> GetAcademicYear(string idAcademicYear);
        Task<AcademicYear> CreateAcademicYear(AcademicYearViewModel academicYearViewModel);
        Task<AcademicYear> RemoveAcademicYear(string idAcademicYear);
        Task<AcademicYear> UpdateAcademicYear(AcademicYearViewModel academicYearViewModel);
    }
}
