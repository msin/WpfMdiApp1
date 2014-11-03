using System.Collections.ObjectModel;

namespace WpfMdiApp1.CIL
{
    public interface IMain
    {
        ObservableCollection<IDocument> Documents { get; set; }

        void Loaded();
    }
}
