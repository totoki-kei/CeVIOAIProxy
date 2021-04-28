using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;
using CeVIOAIProxy.Servers;

namespace CeVIOAIProxy
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private CAPTcpServer server;

        public App()
        {
            this.ShutdownMode = ShutdownMode.OnMainWindowClose;
            RenderOptions.ProcessRenderMode = RenderMode.SoftwareOnly;

            this.Startup += this.App_Startup;
            this.Exit += this.App_Exit;

            this.DispatcherUnhandledException += this.App_DispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += this.CurrentDomain_UnhandledException;
        }

        private void App_Startup(object sender, StartupEventArgs e)
        {
            var c = Config.Instance;
            c.SetStartup(c.IsStartupWithWindows);

            this.server = new CAPTcpServer();
            this.server.Open(c.TcpServerPort);
        }

        private void App_Exit(object sender, ExitEventArgs e)
        {
            this.CloseServer();

            CeVIOAIProxy.MainWindow.Instance?.HideNotifyIcon();
            Config.Instance.Save();

            GC.SuppressFinalize(this);
        }

        private void CloseServer()
        {
            if (this.server != null)
            {
                this.server.Close();
                this.server.Dispose();
                this.server = null;
            }
        }

        private void App_DispatcherUnhandledException(
            object sender,
            DispatcherUnhandledExceptionEventArgs e)
        {
            this.DumpUnhandledException(e.Exception);
        }

        private void CurrentDomain_UnhandledException(
            object sender,
            UnhandledExceptionEventArgs e)
        {
            this.DumpUnhandledException(e.ExceptionObject as Exception);
        }

        private async void DumpUnhandledException(
            Exception ex)
        {
            await Task.Run(() =>
            {
                File.WriteAllText(
                    @".\CeVIOAIProxy.error.log",
                    $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}\n{ex}",
                    new UTF8Encoding(false));
            });

            MessageBox.Show(
                "予期しない例外を検知しました。アプリケーションを終了します。\n\n" +
                ex,
                "Fatal",
                MessageBoxButton.OK,
                MessageBoxImage.Error);

            this.CloseServer();

            CeVIOAIProxy.MainWindow.Instance?.HideNotifyIcon();
            Config.Instance.Save();

            GC.SuppressFinalize(this);

            this.Shutdown(1);
        }
    }
}