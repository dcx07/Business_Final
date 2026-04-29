using System;
using Microsoft.UI.Xaml;

namespace BusinessFinal;

public static class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        Environment.SetEnvironmentVariable(
            "MICROSOFT_WINDOWSAPPRUNTIME_BASE_DIRECTORY",
            AppContext.BaseDirectory);

        Application.Start(_ => new App());
    }
}
