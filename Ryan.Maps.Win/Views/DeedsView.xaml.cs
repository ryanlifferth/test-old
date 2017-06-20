using Ryan.Maps.Win.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for DeedsView.xaml
    /// </summary>
    public partial class DeedsView : UserControl
    {
        public DeedsView()
        {
            InitializeComponent();

            this.DataContext = new DeedsViewModel
            {
                Title = "Deeds View Model",
                PublicRecordsDeedsList = new List<PublicRecordsDeedFacade>
                {
                    new PublicRecordsDeedFacade { DocNumber = "154213", RecordDate = DateTime.Parse("12/9/2016"), DeedType = "Warranty Deed", Amount = 310000, Grantor = "Freeman Living Trust", Grantee = "Johnson Billie E"},
                    new PublicRecordsDeedFacade { DocNumber = "104466", RecordDate = DateTime.Parse("09/22/2015"), DeedType = "Warranty Deed", Grantor = "Freeman Kathleen I", Grantee = "Freeman Trust"},
                    new PublicRecordsDeedFacade { DocNumber = "10845", RecordDate = DateTime.Parse("1/26/1995"), DeedType = "Warranty Deed-Special", Amount = 154000, Grantor = "Pacific Western Homes Inc", Grantee = "Freeman Kathleen I"},
                    new PublicRecordsDeedFacade { DocNumber = "10845", RecordDate = DateTime.Parse("1/26/1995"), DeedType = "Trustee Substitution", Amount = 999888000, Grantor = "Am Sam I", Grantee = "Pacific Western Homes Inc"}

                    ,
                    new PublicRecordsDeedFacade { DocNumber = "154213", RecordDate = DateTime.Parse("12/9/2016"), DeedType = "Warranty Deed", Amount = 310000, Grantor = "Freeman Living Trust", Grantee = "Johnson Billie E"},
                    new PublicRecordsDeedFacade { DocNumber = "104466", RecordDate = DateTime.Parse("5/7/2003"), DeedType = "Warranty Deed", Grantor = "Freeman Kathleen I", Grantee = "Freeman Trust"},
                    new PublicRecordsDeedFacade { DocNumber = "10845", RecordDate = DateTime.Parse("1/26/1995"), DeedType = "Warranty Deed-Special", Amount = 154000, Grantor = "Pacific Western Homes Inc", Grantee = "Freeman Kathleen I"},
                    new PublicRecordsDeedFacade { DocNumber = "10845", RecordDate = DateTime.Parse("1/26/1995"), DeedType = "Trustee Substitution", Amount = 999888000, Grantor = "Am Sam I", Grantee = "Pacific Western Homes Inc"},
                    new PublicRecordsDeedFacade { DocNumber = "154213", RecordDate = DateTime.Parse("12/9/2016"), DeedType = "Warranty Deed", Amount = 310000, Grantor = "Freeman Living Trust", Grantee = "Johnson Billie E"},
                    new PublicRecordsDeedFacade { DocNumber = "104466", RecordDate = DateTime.Parse("5/7/2003"), DeedType = "Warranty Deed", Grantor = "Freeman Kathleen I", Grantee = "Freeman Trust"},
                    new PublicRecordsDeedFacade { DocNumber = "10845", RecordDate = DateTime.Parse("1/26/1995"), DeedType = "Warranty Deed-Special", Amount = 154000, Grantor = "Pacific Western Homes Inc", Grantee = "Freeman Kathleen I"},
                    new PublicRecordsDeedFacade { DocNumber = "10845", RecordDate = DateTime.Parse("1/26/1995"), DeedType = "Trustee Substitution", Amount = 999888000, Grantor = "Am Sam I", Grantee = "Pacific Western Homes Inc"}

                },
                MlsListingHistoryList = new List<ViewModels.MlsListingHistory>
                {
                    new MlsListingHistory { MlsNumber = "16089008", ClosedDate = DateTime.Parse("12/9/2016"), Status = "Sold", SoldPrice = 310000, ListDate = DateTime.Parse("8/11/2016"), ListPrice = 315000, DaysOnMarket = 36 },
                    new MlsListingHistory { MlsNumber = "9876541", ClosedDate = DateTime.Parse("09/21/2015"), Status = "Sold", SoldPrice = 300000, ListDate = DateTime.Parse("08/22/2015"), ListPrice = 300000, DaysOnMarket = 36 },
                    new MlsListingHistory { MlsNumber = "0123654",  ClosedDate = DateTime.Parse("4/24/1992"), Status = "Sold", SoldPrice = 199000, ListDate = DateTime.Parse("4/14/1992"), ListPrice = 205000, DaysOnMarket = 10 }
                }
            };


        }
    }
}
