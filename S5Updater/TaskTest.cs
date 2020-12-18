using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace S5Updater
{
    class TaskTest : IUpdaterTask
    {
        public void Work(ProgressDialog.ReportProgressDel r)
        {
            for (int i = 1; i < 11; i++)
            {
                Thread.Sleep(1000);
                r(i * 10, "" + i);
            }
        }
    }
}
