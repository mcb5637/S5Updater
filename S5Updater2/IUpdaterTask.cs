using System.Threading.Tasks;

namespace S5Updater2
{
    internal interface IUpdaterTask
    {
        Task Work(ProgressDialog.ReportProgressDel r);
    }
}
