using System.Collections.ObjectModel;
using System.Windows;
using DevExpress.Mvvm;
using WpfMdiApp1.CIL;
using IDocument = WpfMdiApp1.CIL.IDocument;

namespace WpfMdiApp1.PLL
{
    public class MainVM : IMain
    {
        public virtual ObservableCollection<IDocument> Documents { get; set; }

        public MainVM()
        {
            Documents = new ObservableCollection<IDocument>();

            Documents.Add(new DocumentVM("Document1", new Point(50d, 50d), new Size(300d, 200d), "IDE"));
            Documents.Add(new DocumentVM("Document2", new Point(150d, 100d), new Size(300d, 200d), "Mail"));
            Documents.Add(new DocumentVM("Document3", new Point(250d, 150d), new Size(300d, 200d), "OS"));
        }

        public void Loaded()
        {
            Messenger.Default.Send(new Message(MessageType.Loaded));
        }
    }
}
