using Avalonia.Controls;
using Avalonia.Threading;
using System.Threading.Tasks;

namespace S5Updater2;

internal partial class ProgressDialog : Window
{
    internal delegate void ReportProgressDel(int c, int m, string? progresstext, string? log);

    private IUpdaterTask? T;
    internal required MainWindow MM;

    public ProgressDialog()
	{
		InitializeComponent();
	}

    internal async Task ShowProgressDialog(IUpdaterTask t)
    {
        T = t;
        var run = Task.Run(RunTask);
        var dia = ShowDialog(MM);
        await run;
        await dia;
    }

    private async Task RunTask()
    {
        try
        {
            if (T == null)
                return;
            await T.Work(ReportProgress);
        }
        finally
        {
            Dispatcher.UIThread.Invoke(Close);
        }
    }
    private void ReportProgress(int c, int m, string? progresstext, string? log)
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            if (c >= 0)
            {
                Bar.Maximum = m;
                Bar.Value = c;
            }
            if (progresstext != null)
                LogText.Text = progresstext;
            if (log != null)
                MM.Log(log);
        });
    }
}