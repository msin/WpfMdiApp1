using System.Windows;
using WpfMdiApp1.CIL;
using WpfMdiApp1.UIL.Views;

namespace WpfMdiApp1.UIL
{
    public class _Package : IPackage
    {
        public void RegisterServices()
        {
            IoC.Instance.RegisterSingle<Window, MainWindow>();
        }
    }
}
