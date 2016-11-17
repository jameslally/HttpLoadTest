using System.Threading.Tasks;
using HttpLoadTester.Entites.Test;

namespace HttpLoadTester.Services
{
    public interface ITest : ITestDescriptors
    {        
        Task Run(TestResult result);
    }
}