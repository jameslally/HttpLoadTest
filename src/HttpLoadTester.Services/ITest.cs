using System.Threading.Tasks;
using HttpLoadTester.Entites.Test;

namespace HttpLoadTester.Services
{
    public interface ITest
    {
        Task Run(TestResult result);
    }
}