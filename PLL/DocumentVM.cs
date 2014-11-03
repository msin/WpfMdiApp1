// Developer Express Code Central Example:
// How to use the IMVVMDockingProperties interface in an MVVM application
// 
// This example demonstrates how to use the DXDocking IMVVMDockingProperties
// interface. This interface provides you a way to specify target groups for each
// of your view models.
// 
// You can find sample updates and versions for different programming languages here:
// http://www.devexpress.com/example=E20026

using System;
using System.Windows;
using DevExpress.Xpf.Docking;
using WpfMdiApp1.CIL;

namespace WpfMdiApp1.PLL
{
    public class DocumentVM : IDocument
    {
        private string _icon;

        public DocumentVM(string caption, Point point, Size size, string icon)
        {
            Caption = caption;
            Location = point;
            Size = size;
            _icon = icon;
        }

        string IMVVMDockingProperties.TargetName
        {
            get{ return "DocumentsGroup"; }
            set { throw new NotImplementedException(); }
        }

        public string Caption { get; set; }

        public Point Location { get; set; }

        public Size Size { get; set; }

        public string Icon { get { return "/WpfMdiApp1.UIL;component/Images/" + _icon + "_16x16.png"; } }
    }
}
