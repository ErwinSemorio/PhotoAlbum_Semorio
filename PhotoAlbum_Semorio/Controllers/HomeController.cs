using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PhotoAlbum_FamilyName.Models;

namespace PhotoAlbum_FamilyName.Controllers
{
    public class HomeController : Controller
    {
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<HomeController> _logger;

        public HomeController(IWebHostEnvironment env, ILogger<HomeController> logger)
        {
            _env = env;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        private List<Photo> GetPhotos(string category, string descriptionPrefix)
        {
            var imagesDir = Path.Combine(_env.WebRootPath, "images", category);
            var photos = new List<Photo>();

            if (Directory.Exists(imagesDir))
            {
                // added .avif and use ToLowerInvariant for consistency
                var allowed = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp", ".avif" };

                var files = Directory.GetFiles(imagesDir)
                                     .Where(f => allowed.Contains(Path.GetExtension(f).ToLowerInvariant()))
                                     .OrderBy(f => f)
                                     .Take(20); // LIMIT TO 20 IMAGES

                int count = 1;

                foreach (var file in files)
                {
                    var fileName = Path.GetFileName(file);

                    photos.Add(new Photo
                    {
                        ImagePath = $"/images/{category}/{fileName}",
                        Title = $"{category.ToUpper()} Image {count}",
                        Description = $"{descriptionPrefix} photo number {count}."
                    });

                    count++;
                }
            }
            else
            {
                _logger.LogWarning("Images directory not found: {ImagesDir}", imagesDir);
            }

            return photos;
        }

        public IActionResult Nature()
        {
            var imagesDir = Path.Combine(_env.WebRootPath, "images", "nature");
            int filesFound = 0;
            int photosReturned = 0;

            if (Directory.Exists(imagesDir))
            {
                var allowed = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp", ".avif" };
                var files = Directory.GetFiles(imagesDir)
                                     .Where(f => allowed.Contains(Path.GetExtension(f).ToLowerInvariant()))
                                     .OrderBy(f => f)
                                     .ToList();

                filesFound = files.Count;
                _logger.LogInformation("Nature images found in folder: {Count}", filesFound);

                var selected = files.Take(20).ToList();
                photosReturned = selected.Count;

                var photos = new List<Photo>();
                int count = 1;
                foreach (var file in selected)
                {
                    var fileName = Path.GetFileName(file);
                    photos.Add(new Photo
                    {
                        ImagePath = $"/images/nature/{fileName}",
                        Title = $"NATURE Image {count}",
                        Description = $"Beautiful nature photo number {count}."
                    });
                    count++;
                }

                ViewBag.FilesFound = filesFound;
                ViewBag.PhotosReturned = photosReturned;

                _logger.LogInformation("Nature photos returned to view: {Count}", photosReturned);
                return View(photos);
            }
            else
            {
                _logger.LogWarning("Nature images directory does not exist: {Dir}", imagesDir);
                ViewBag.FilesFound = 0;
                ViewBag.PhotosReturned = 0;
                return View(new List<Photo>());
            }
        }

        public IActionResult Cities()
        {
            var photos = GetPhotos("cities", "Amazing city landscape");
            return View(photos);
        }

        public IActionResult Animals()
        {
            var photos = GetPhotos("animals", "Wonderful animal");
            return View(photos);
        }

        public IActionResult Food()
        {
            var photos = GetPhotos("food", "Delicious food");
            return View(photos);
        }

        public IActionResult Travel()
        {
            var photos = GetPhotos("travel", "Exciting travel destination");
            return View(photos);
        }
    }
}
