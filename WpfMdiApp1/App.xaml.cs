using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Markup;
using WpfMdiApp1.CIL;

namespace WpfMdiApp1
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private ILogger _logger;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            AppDomain.CurrentDomain.UnhandledException += AppDomainUnhandledException;

            Bootstrap();

            _logger.Debug("Client is started");

            var window = IoC.Instance.GetInstance<Window>();

            window.Closed += (s, a) =>
            {
                _logger.Debug("Client is stopped");
                Current.Shutdown();
            };

            window.Show();
        }

        private void AppDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            File.WriteAllText("FatalError " + DateTime.Now.ToString("yy-MM-dd HH-mm-ss") + ".log", e.ExceptionObject.ToString());
        }

        private void Bootstrap()
        {
            CultureInfo myCulture = new CultureInfo("ru-RU", true)
            {
                NumberFormat = { NumberDecimalSeparator = ".", NumberDecimalDigits = 0, NumberGroupSeparator = " " }
            };

            Thread.CurrentThread.CurrentCulture = myCulture;

            FrameworkElement.LanguageProperty.OverrideMetadata(typeof(FrameworkElement),
                new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));

            //  Application name
            IoC.Instance.Params.Add("AppName", Assembly.GetEntryAssembly().GetName().Name);

            //  Logging level from application parameters
            IoC.Instance.Params.Add("LogLevel", WpfMdiApp1.Properties.Settings.Default.LogLevelFile);

            //  Look for assemblies with project prefix and interface IPackage
            IoC.Instance.RegisterPackages("WpfMdiApp1.");

            _logger = IoC.Instance.GetInstance<ILogger>();

            //  Log assemblies info - build date
            for (int i = 0; i < IoC.Instance.PackageList.Count; i++)
            {
                string package = IoC.Instance.PackageList[i];

                _logger.Debug(package);
            }

            var assembly = Assembly.GetExecutingAssembly();

            //  Log entry assembly info
            _logger.Debug(string.Format("Assembly: {0}  Date: {1}",
                assembly.FullName.Replace(", Culture=neutral, PublicKeyToken=null", ""),
                IoC.Instance.RetrieveLinkerTimestamp(assembly)));
        }
    }
}
