using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using HttpLoadTester.Services;

namespace HttpLoadTester.Controllers
{
    public class HomeController : Controller
    {
        private readonly IEnumerable<ITest> _tests;

        public HomeController(IEnumerable<ITest> tests)
        {
            _tests = tests;
        }

        public IActionResult Index()
        {
            var model = new ViewModels.HomeIndexViewModel();
            model.Tests = _tests.Where(t => t.Name == "Dummy");
            return View(model);
        }
        

        public IActionResult Error()
        {
            return View();
        }
    }
}
