using Ryan.Maps.Win.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Ryan.Maps.Win.Views
{
    /// <summary>
    /// Interaction logic for HyperlinkView.xaml
    /// </summary>
    public partial class HyperlinkView : UserControl
    {
        public HyperlinkView()
        {
            InitializeComponent();

            this.DataContext = new HyperlinkViewModel()
            {
                DataSourceLinkInfo = new System.Collections.ObjectModel.ObservableCollection<DataSourceLinkInfo>
                {
                    new DataSourceLinkInfo { LogoFileName = "google.jpg", Url = "http://www.google.com" },
                    new DataSourceLinkInfo { LogoFileName = "lds.org", Url = "http://www.lds.org" }
                }
            };
        }
    }
}
