﻿using COMP1640_WebDev.Models;
using COMP1640_WebDev.Repositories.Interfaces;
using COMP1640_WebDev.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO.Compression;

namespace COMP1640_WebDev.Controllers
{

    [Authorize(Roles = "Marketing Manager")]
    public class MarketingManagerController : Controller
    {
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IMagazineRepository _magazineRepository;
		private readonly IAcademicYearRepository _academicYearRepository;
		private readonly IFacultyRepository _facultyRepository;
        private readonly IUserRepository _userRepository;
		public List<AcademicYear> AcademicYears { get; set; }
		private readonly UserManager<User> _userManager;
		public MarketingManagerController(IWebHostEnvironment hostEnvironment, IMagazineRepository magazineRepository, IAcademicYearRepository academicYearRepository, UserManager<User> userManager, IUserRepository userRepository, IFacultyRepository facultyRepository)
        {
            _hostEnvironment = hostEnvironment;
            _magazineRepository = magazineRepository;
			_academicYearRepository = academicYearRepository;
            _facultyRepository = facultyRepository;
            _userRepository = userRepository;
            _userManager = userManager;
		}


		public IActionResult Index()
        {
            return View();
        }

        // 1. Magazines management
        public IActionResult MagazinesManagement()
        {
            return View();
        }
		[HttpGet]
		public async Task<IActionResult> CreateMagazine()
        {
			var userId = _userManager.GetUserId(User);
			var user = await _userRepository.GetUser(userId);
			var faculty = await _facultyRepository.GetFaculty(user.FacultyId);

			ViewBag.AcademicYears = faculty.AcademicYears;
			return View();
        }

		[HttpPost]
		public async Task<IActionResult> CreateMagazine(MagazineViewModel magazine, List<IFormFile> files)
		{

            
            var userId = _userManager.GetUserId(User);
            var uploadPath = Path.Combine(_hostEnvironment.WebRootPath, "images");
            var newFileName = "";
            foreach (var file in files)
            {
                if (file.Length > 0)
                {
					newFileName = Guid.NewGuid().ToString() + file.FileName;
					var filePath = Path.Combine(uploadPath, newFileName);
                    
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                }
            }
            var user = await _userRepository.GetUser(userId);
			var academicYear = await _academicYearRepository.GetAcademicYear(magazine.AcademicYearId);
			await _magazineRepository.CreateMagazine(new Magazine {
                FacultyId = user.FacultyId,
                Title = magazine.Title,
                Description = magazine.Description,
                CoverImage = "~/images/" + newFileName,

			});

            TempData["AlertMessage"] = "Created successfully!!!";


            return RedirectToAction("MagazinesManagement");
		}


		// 2.Download file
		public IActionResult DataManagement()
        {
            var uploadsPath = Path.Combine(_hostEnvironment.WebRootPath, "images");
            var fileModels = Directory.GetFiles(uploadsPath)
                                      .Select(file => Path.GetFileName(file)) // Use LINQ to select file names
                                      .ToList();       

            return View(fileModels);
        }


        public IActionResult DownloadZip1()
        {
            // Define the path to the uploads directory
            var uploadsPath = Path.Combine(_hostEnvironment.WebRootPath, "images");

            // Temporary filename for the ZIP archive
            var tempZipFileName = "MarketingFiles.zip";
            var tempZipPath = Path.Combine(Path.GetTempPath(), tempZipFileName);

            // Ensure any existing instance of the file is deleted
            if (System.IO.File.Exists(tempZipPath))
            {
                System.IO.File.Delete(tempZipPath);
            }

            // Create a new ZIP archive
            using (var zipStream = new FileStream(tempZipPath, FileMode.CreateNew))
            using (var archive = new ZipArchive(zipStream, ZipArchiveMode.Create, true))
            {
                var files = Directory.GetFiles(uploadsPath);
                foreach (var filePath in files)
                {
                    var fileInfo = new FileInfo(filePath);
                    var entry = archive.CreateEntry(fileInfo.Name);
                    using (var entryStream = entry.Open())
                    using (var fileStream = System.IO.File.OpenRead(filePath))
                    {
                        fileStream.CopyTo(entryStream);
                    }
                }
            }

            // Send the ZIP file to the browser
            return PhysicalFile(tempZipPath, "application/zip", tempZipFileName);
        }

    

        public IActionResult DownloadSingleFile(string file)
        {
            if (string.IsNullOrEmpty(file))
            {
                // Handle invalid file name
                return BadRequest("Invalid file name.");
            }

            // Define the path to the uploads directory
            var uploadsPath = Path.Combine(_hostEnvironment.WebRootPath, "images");

            var filePath = Path.Combine(uploadsPath, file);
            if (!System.IO.File.Exists(filePath))
            {
                // Handle case where file doesn't exist
                return NotFound();
            }

            // Return the file
            return PhysicalFile(filePath, "application/octet-stream", file);
        }
    }
}
