using System.Windows;
using coffre_fort2.ViewModels;

namespace coffre_fort2.Views
{
    public partial class A2FView : Window
    {
        public A2FView()
        {
            InitializeComponent();
            DataContext = new A2FViewModel();
        }
    }
}
