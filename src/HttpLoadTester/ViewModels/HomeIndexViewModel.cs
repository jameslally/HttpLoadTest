using HttpLoadTester.DTOs;
using HttpLoadTester.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HttpLoadTester.ViewModels
{
    public class HomeIndexViewModel
    {
        public IEnumerable<ITestDescriptors> Tests { get; set; }
    }
}
