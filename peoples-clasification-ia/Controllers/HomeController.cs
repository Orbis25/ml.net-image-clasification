using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using peoples_clasification_ia.Models;
using Peoples_clasification_iaML.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace peoples_clasification_ia.Controllers
{
    public class Post
    {
        public IFormFile Image { get; set; }
    }
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IWebHostEnvironment _env;
        public HomeController(ILogger<HomeController> logger, IWebHostEnvironment env)
        {
            _logger = logger;
            _env = env;
        }

        public IActionResult Index()
        {
          
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> IndexAsync(Post model)
        {

            var filePath = string.Empty;
            var name = Path.GetRandomFileName();
            if (model.Image != null)
            {
                filePath = Path.Combine(_env.WebRootPath,name);

                using (var stream = System.IO.File.Create(filePath + ".jpg"))
                {
                    await model.Image.CopyToAsync(stream);
                }
            }
            var input = new ModelInput()
            {
                ImageSource = $"{filePath}.jpg"
            };

            ModelOutput result = ConsumeModel.Predict(input);
            ViewBag.Result = result.Prediction;
            ViewBag.Imagen = $"/{name}.jpg";
            ViewBag.Sospech = Math.Round(result.Score[0] * 100);
            ViewBag.Normal = Math.Round(result.Score[1] * 100);
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
