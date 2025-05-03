using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using coffre_fort2.ViewModels;

namespace coffre_fort2.Views
{
    public partial class MainView : Window
    {
        private GridViewColumnHeader _dernierTri = null;
        private ListSortDirection _sensTri = ListSortDirection.Ascending;

        public MainView(string utilisateur, string jwtToken)
        {
            InitializeComponent();
            DataContext = new MainViewModel(utilisateur, jwtToken);
        }

        private void GridViewColumnHeader_Click(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource is not GridViewColumnHeader header || header.Column?.DisplayMemberBinding == null)
                return;

            var binding = (Binding)header.Column.DisplayMemberBinding;
            string sortBy = binding.Path.Path;

            if (_dernierTri == header)
            {
                _sensTri = _sensTri == ListSortDirection.Ascending
                    ? ListSortDirection.Descending
                    : ListSortDirection.Ascending;
            }
            else
            {
                _sensTri = ListSortDirection.Ascending;
            }

            _dernierTri = header;

            ICollectionView view = CollectionViewSource.GetDefaultView(ListViewMotsDePasse.ItemsSource);
            view.SortDescriptions.Clear();
            view.SortDescriptions.Add(new SortDescription(sortBy, _sensTri));
            view.Refresh();
        }
    }
}
