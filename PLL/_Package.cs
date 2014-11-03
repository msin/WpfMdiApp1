using DevExpress.Mvvm.POCO;
using WpfMdiApp1.CIL;

namespace WpfMdiApp1.PLL
{
    public class _Package : IPackage
    {
        public void RegisterServices()
        {
            IoC.Instance.RegisterSingle<IMain>(ViewModelSource.Create<MainVM>);
        }
    }
}
