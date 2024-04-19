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

    //[Authorize(Roles = "Marketing Manager")]
    public class MarketingManagerController : Controller
    {
        private readonly IMagazineRepository _magazineRepository;
		private readonly IAcademicYearRepository _academicYearRepository;
		private readonly IFacultyRepository _facultyRepository;
        private readonly IWebHostEnvironment _hostEnvironment;

        public MarketingManagerController(IWebHostEnvironment hostEnvironment, IMagazineRepository magazineRepository, IAcademicYearRepository academicYearRepository, IFacultyRepository facultyRepository)
        {
            _magazineRepository = magazineRepository;
			_academicYearRepository = academicYearRepository;
            _facultyRepository = facultyRepository;
            _hostEnvironment = hostEnvironment;
        }


        public IActionResult Index()
        {

            return View();
        }


       /* [HttpGet]
        public async Task<IActionResult> DetailsMagazine(string id) 
        {
            var result = await _magazineRepository.GetMagazine(id);

            string imageBase64Data = Convert.ToBase64String(result.CoverImage);
            string image = string.Format("data:image/jpg;base64, {0}", imageBase64Data);
            ViewBag.Image = image;

            return View(result);
        }*/

        public async Task<IActionResult> MagazinesManagementAsync()
        {
            var magazines = await _magazineRepository.GetMagazines();
            return View(magazines);
        }


		[HttpGet]
		public IActionResult CreateMagazine()
		{
			var magazineViewModel = _magazineRepository.GetMagazineViewModel();
			return View(magazineViewModel);
		}



		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> CreateMagazine(MagazineViewModel mViewModel)
        {
			if (ModelState.IsValid)
			{
				await _magazineRepository.CreateMagazine(mViewModel);
				TempData["AlertMessage"] = "Magazine created successfully!!!";
				return RedirectToAction("MagazinesManagement");
			}

			return View();
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
            var uploadsPath = Path.Combine(_hostEnvironment.WebRootPath, "images");

            var tempZipFileName = "MarketingFiles.zip";
            var tempZipPath = Path.Combine(Path.GetTempPath(), tempZipFileName);

            if (System.IO.File.Exists(tempZipPath))
            {
                System.IO.File.Delete(tempZipPath);
            }

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

            return PhysicalFile(tempZipPath, "application/zip", tempZipFileName);
        }

    

        public IActionResult DownloadSingleFile(string file)
        {
            if (string.IsNullOrEmpty(file))
            {
                return BadRequest("Invalid file name.");
            }

            var uploadsPath = Path.Combine(_hostEnvironment.WebRootPath, "images");

            var filePath = Path.Combine(uploadsPath, file);
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }

            return PhysicalFile(filePath, "application/octet-stream", file);
        }
    }
}
