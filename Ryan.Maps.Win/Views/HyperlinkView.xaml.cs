using Ryan.Maps.Win.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Printing;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Xps;
using System.Windows.Xps.Packaging;

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
                DataSourceLinkInfoCollection = new System.Collections.ObjectModel.ObservableCollection<DataSourceLinkInfo>
                {
                    //new DataSourceLinkInfo { LogoFileName = "google.jpg", Url = "http://www.google.com" },
                    //new DataSourceLinkInfo { LogoFileName = "lds.png", Url = "http://www.lds.org" }
                    new DataSourceLinkInfo
                    {
                        FullLogoImagePath = "/Ryan.Maps.Win;component/Resources/Images/Logo_PortlandRmls.png",
                        LogoFileName = "Logo_PortlandRmls.png",
                        MlsDisplayName = "RMLS",
                        Url = "http://www.rmlsweb.com"
                    },
                    new DataSourceLinkInfo
                    {
                        FullLogoImagePath = "/Ryan.Maps.Win;component/Resources/Images/Logo_Crmls_Crmls.png",
                        LogoFileName = "Logo_Crmls_Crmls.png",
                        MlsDisplayName = "CRMLS",
                        Url = "http://www.crmls.org"
                    },
                    new DataSourceLinkInfo
                    {
                        FullLogoImagePath = "/Ryan.Maps.Win;component/Resources/Images/Logo_Crmls_Claw.png",
                        LogoFileName = "Logo_Crmls_Claw.png",
                        MlsDisplayName = "CLAW",
                        Url = "http://www.themls.com/"
                    },
                    new DataSourceLinkInfo
                    {
                        FullLogoImagePath = "/Ryan.Maps.Win;component/Resources/Images/Logo_Crmls_Itech.png",
                        LogoFileName = "Logo_Crmls_Itech.png",
                        MlsDisplayName = "iTech",
                        Url = "http://itech.rapmls.com/"
                    }

                }
            };

            //var logo = new BitmapImage();
            //logo.BeginInit();
            //logo.UriSource = new Uri("pack://application:,,,/Ryan.Maps.Translation;component/Resources/Images/Logo_Wfr.png", UriKind.Absolute);
            //logo.EndInit();
            //TestButton.Source = logo;

            //var assembly = System.Reflection.Assembly.LoadFrom("Ryan.Maps.Translation.dll");
            //var imageStream = assembly.GetManifestResourceStream("Ryan.Maps.Translation.g.resources");
            //var image = new BitmapImage();
            //image.StreamSource = imageStream;
            //TestButton.Source = image;
            //var img = System.Drawing.Image.FromStream(imageStream);



            var sourceUri = new Uri("pack://application:,,,/Ryan.Maps.Translation;component/Resources/Images/Logo_Wfr.png", UriKind.Absolute);
            TestButton.Source = new BitmapImage(sourceUri);


        }

        private void PrintControl(object sender, RoutedEventArgs e)
        {
            //PrintResizePage();
            PrintFixedDocument();

        }

        private void PrintResizePage()
        {
            var printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == true)
            {
                //printDialog.PrintVisual(this, "Hyperlink Printing");

                // Get the printer's capabilities
                var capabilities = printDialog.PrintQueue.GetPrintCapabilities(printDialog.PrintTicket);

                // Get the scale of the print wrt to screen of WPF visual
                var scale = Math.Min(capabilities.PageImageableArea.ExtentWidth / this.ActualWidth,
                                     capabilities.PageImageableArea.ExtentHeight / this.ActualHeight);

                // transform the visual to scale
                this.LayoutTransform = new ScaleTransform(scale, scale);

                // Get the size of the printer page
                var printerSize = new Size(capabilities.PageImageableArea.ExtentWidth, capabilities.PageImageableArea.ExtentHeight);

                // Update the layout of the visual to the printer page size
                this.Measure(printerSize);
                this.Arrange(new Rect(new Point(capabilities.PageImageableArea.OriginWidth,
                                                capabilities.PageImageableArea.OriginHeight), printerSize));

                // Now print the visual to printer to fit on the page
                printDialog.PrintVisual(this, "Hyperlink Printing");

                // Reset back to normal scale
                this.LayoutTransform = new ScaleTransform(1.0, 1.0);
                this.Measure(new Size(this.ActualWidth, this.ActualHeight));
                this.Arrange(new Rect(new Point(0, 0), new Size(this.ActualWidth, this.ActualHeight)));

            }
        }

        private void PrintFixedDocument()
        {
            var grid = new Grid();
            grid = ContentGrid;

            var doc = new FixedDocument();
            var content = new PageContent();
            var fixedPage = new FixedPage();

            fixedPage.Children.Add(grid);
            ((System.Windows.Markup.IAddChild)content).AddChild(fixedPage);
            doc.Pages.Add(content);

            var printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == true)
            { 
                printDialog.PrintDocument(doc.DocumentPaginator, "Fixed Doc");
            }
        }




    }
}
