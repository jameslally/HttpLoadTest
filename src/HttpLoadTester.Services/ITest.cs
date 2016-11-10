using System.Threading.Tasks;
using HttpLoadTester.Entites.Test;

namespace HttpLoadTester.Services
{
    public interface ITest
    {
        bool ResponsibleFor(string name);
        Task Run(TestResult result);
    }
}