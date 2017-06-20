using Ryan.Maps.Win.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Ryan.Maps.Win.ViewModels
{
    public class HyperlinkViewModel : ViewModelBase
    {

        #region Fields
        #endregion

        #region Properties


        private KeyValuePair<string, string> _dataSourceUrls;

        public KeyValuePair<string, string> DataSourceUrls
        {
            get { return _dataSourceUrls; }
            set
            {
                _dataSourceUrls = value;
                OnPropertyChanged("DataSourceUrls");
            }
        }

        private ObservableCollection<DataSourceLinkInfo> _dataSourceLinkInfo;

        public ObservableCollection<DataSourceLinkInfo> DataSourceLinkInfo
        {
            get { return _dataSourceLinkInfo; }
            set
            {
                _dataSourceLinkInfo = value;
                OnPropertyChanged("DataSourceLinkInfo");
            }
        }




        #endregion

        #region Constructors
        #endregion

        #region Methods

        private ICommand _navigateToUrlCommand;
        public ICommand NavigateToUrlCommand
        {
            get
            {
                return _navigateToUrlCommand ?? (_navigateToUrlCommand = new RelayCommand(param => NavigateToUrlCommandExecute(param)));
            }
        }


        private void NavigateToUrlCommandExecute(object param)
        {
            var dataSourceUrl = (string)param ?? "http://www.google.com";

            System.Diagnostics.Process.Start(dataSourceUrl);
        }

        #endregion
    }

    public class DataSourceLinkInfo : ViewModelBase
    {
        private string _logoFileName;

        public string LogoFileName
        {
            get { return _logoFileName; }
            set
            {
                _logoFileName = value;
                OnPropertyChanged("LogoFileName");
            }
        }

        private string _url;

        public string Url
        {
            get { return _url; }
            set {
                _url = value;
                OnPropertyChanged("Url");
            }
        }


    }

}
