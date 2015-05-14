using System.Threading.Tasks;
using System.IO;

namespace C1.Web.Api.Excel
{
    public interface IExcelHost
    {
        Workbook Read(Stream file);
        void Write(Workbook workbook, Stream file);
    }

    public interface IExcelHostOperations
    {

    }
}
