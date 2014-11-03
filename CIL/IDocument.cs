using System.Windows;
using DevExpress.Xpf.Docking;

namespace WpfMdiApp1.CIL
{
    public interface IDocument : IMVVMDockingProperties
    {
        string Caption { get; set; }

        Point Location { get; set; }

        Size Size { get; set; }

        string Icon { get; }
    }
}
