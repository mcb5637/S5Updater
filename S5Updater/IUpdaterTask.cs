using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S5Updater
{
    internal interface IUpdaterTask
    {
        void Work(ProgressDialog.ReportProgressDel r);
    }
}
