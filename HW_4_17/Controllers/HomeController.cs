using HW_4_17.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.VisualBasic;
using PasswordLink.Data;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Security.Cryptography.Xml;
using System.Text.Json;

namespace HW_4_17.Controllers
{
    public class HomeController : Controller
    {
        private string _connectionString = @"Data Source=.\sqlexpress;Initial Catalog=PasswordLinkDB; Integrated Security=true;";


        private IWebHostEnvironment _webHostEnvironment;

        public HomeController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }


       
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Upload(IFormFile imageFile, string password)
        {
            var fileName = $"{Guid.NewGuid()}-{imageFile.FileName}";
            var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", fileName);
            using var fs = new FileStream(filePath, FileMode.CreateNew);
            imageFile.CopyTo(fs);

            PasswordLinkManager mgr = new PasswordLinkManager(_connectionString);
            int id = mgr.Add(fileName, password);

           
            return Redirect($"/Home/Uploaded?Id={id}");
        }
        public IActionResult Uploaded(int id)
        {
            var mgr = new PasswordLinkManager(_connectionString);
            UploadedImagesViewModel vm = new UploadedImagesViewModel();

            vm.Image = mgr.GetImage(id);
           

            return View(vm);
        }
        public IActionResult ViewImage(int id)
        {
            UploadedImagesViewModel vm = new UploadedImagesViewModel();
            PasswordLinkManager mgr = new PasswordLinkManager(_connectionString);
            var ids = HttpContext.Session.Get<List<int>>("ids");
            if (ids == null)
            {
                ids = new List<int>();
            }
            if (TempData["message"] != null)
            {
                vm.Message = (string)TempData["message"];
            }
            if (ids.Contains(id))
            {              
                vm.Contains = true;
                mgr.UpdateView(id);
                vm.Image = mgr.GetImage(id);
            }
            else
            {
                vm.Contains = false;
                vm.Image = new Images
                {
                    Id = id
                };
            }
            
                                 
            return View(vm);
           
        }
        [HttpPost]
        public IActionResult ViewImage(int id, string password)
        {           
            var mgr = new PasswordLinkManager(_connectionString);
            Images image = mgr.GetImage(id);
          
            if (image.Password.ToLower() != password.ToLower())
            {
                TempData["message"] = "Invalid Password!";
                return Redirect($"/Home/ViewImage?id={id}");
            }
            else
            {
                var ids = HttpContext.Session.Get<List<int>>("ids");
                if (ids == null)
                {
                    ids = new List<int>();
                }
                ids.Add(id);
                HttpContext.Session.Set("ids", ids);
                
            }
            return Redirect($"/Home/ViewImage?id={id}");


        }
        public IActionResult ImagePage(int id)
        {
            var vm = new UploadedImagesViewModel();
            var mgr = new PasswordLinkManager(_connectionString);
           

           

            vm.Image = mgr.GetImage(id);
            


            return View(vm);
        }





    }
    public static class SessionExtensions
    {
        public static void Set<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonSerializer.Serialize(value));
        }

        public static T Get<T>(this ISession session, string key)
        {
            string value = session.GetString(key);

            return value == null ? default(T) :
                JsonSerializer.Deserialize<T>(value);
        }
    }
}