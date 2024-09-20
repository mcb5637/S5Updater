using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S5Updater2
{
    internal interface IUpdaterTask
    {
        Task Work(ProgressDialog.ReportProgressDel r);
    }
}
