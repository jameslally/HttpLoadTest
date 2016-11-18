using HttpLoadTester.Services;
using System.Collections.Generic;

namespace HttpLoadTester.ViewModels
{
    public class HomeIndexViewModel
    {
        public IEnumerable<ITestDescriptors> Tests { get; set; }
    }
}
