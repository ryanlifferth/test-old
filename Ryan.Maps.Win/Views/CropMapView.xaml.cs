using System;
using System.Collections.Generic;
using System.IO;
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
using Ryan.AddressUtility.Models;
using Ryan.Maps.Win.Models;
using Ryan.Maps.Win.ViewModels;

namespace Ryan.Maps.Win.Views
{
    /// <summary>
    /// Interaction logic for CropMapView.xaml
    /// </summary>
    public partial class CropMapView : UserControl
    {
        private CropMapViewModel _mapViewModel;

        private Point startDrag;

        public CropMapView()
        {
            InitializeComponent();

            this.DataContext = new CropMapViewModel
            {
                Subject = new Address
                {
                    AddressLine1 = "325 E Gordon Ave",
                    City = "Layton",
                    State = "UT",
                    Zip = "84041"
                },
                Comps = new List<Comp>
                {
                    new Comp
                    {
                        CompNumber = "1",
                        Address = new Address
                        {
                            AddressLine1 = "929 S Arbor Way",
                            City = "Layton",
                            State = "UT",
                            Zip = "84041"
                        }
                    },
                    new Comp
                    {
                        CompNumber = "2",
                        Address = new Address
                        {
                            AddressLine1 = "765 E Gordon Ave",
                            City = "Layton",
                            State = "UT",
                            Zip = "84041"
                        }
                    },
                    new Comp
                    {
                        CompNumber = "3",
                        Address = new Address
                        {
                            AddressLine1 = "2939 N 725 W",
                            City = "Layton",
                            State = "UT",
                            Zip = "84041"
                        }
                    }
                }
            };

            _mapViewModel = (CropMapViewModel)DataContext;

            canvas.MouseDown += new MouseButtonEventHandler(canvas_MouseDown);
            canvas.MouseUp += new MouseButtonEventHandler(canvas_MouseUp);
            canvas.MouseMove += new MouseEventHandler(canvas_MouseMove);
        }

        private void canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //Set the start point
            startDrag = e.GetPosition(canvas);
            //Move the selection marquee on top of all other objects in canvas
            Canvas.SetZIndex(rectangle, canvas.Children.Count);
            //Capture the mouse
            if (!canvas.IsMouseCaptured)
                canvas.CaptureMouse();
            canvas.Cursor = Cursors.Cross;
        }

        private void canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            //Release the mouse
            if (canvas.IsMouseCaptured)
                canvas.ReleaseMouseCapture();
            canvas.Cursor = Cursors.Arrow;
        }

        private void canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (canvas.IsMouseCaptured)
            {
                Point currentPoint = e.GetPosition(canvas);

                //Calculate the top left corner of the rectangle regardless of drag direction
                double x = startDrag.X < currentPoint.X ? startDrag.X : currentPoint.X;
                double y = startDrag.Y < currentPoint.Y ? startDrag.Y : currentPoint.Y;

                if (rectangle.Visibility == Visibility.Hidden)
                    rectangle.Visibility = Visibility.Visible;

                //Move the rectangle to proper place
                rectangle.RenderTransform = new TranslateTransform(x, y);

                var maxHeight = MapImage.ActualHeight - y;
                var maxWidth = MapImage.ActualWidth - x < 820;
                //Set its size
                var width = Math.Abs(e.GetPosition(canvas).X - startDrag.X) < 820 ? Math.Abs(e.GetPosition(canvas).X - startDrag.X) : 820;
                rectangle.Width = width;
                var height = Math.Abs(e.GetPosition(canvas).Y - startDrag.Y) < maxHeight ? Math.Abs(e.GetPosition(canvas).Y - startDrag.Y) : maxHeight;
                rectangle.Height = Math.Abs(e.GetPosition(canvas).Y - startDrag.Y);
            }
        }

        private void PrintButton_Click(object sender, RoutedEventArgs e)
        {
            var printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == true)
            {
                var doc = new FlowDocument();
                doc.FontFamily = new System.Windows.Media.FontFamily("Segoe UI");

                doc.PageWidth = printDialog.PrintableAreaWidth; // CentimeterToPoint(21.59);
                doc.PageHeight = printDialog.PrintableAreaHeight; // CentimeterToPoint(27.94);
                // margins (top, left
                doc.PagePadding = new Thickness(48, 72, 48, 72);

                doc.ColumnWidth = doc.PageWidth;
                doc.IsOptimalParagraphEnabled = true;

                var grid = new Grid();
                grid.Margin = new Thickness(0);
                grid.ColumnDefinitions
                    .Add(new ColumnDefinition()); // { Width = new GridLength(Convert.ToInt32(doc.ColumnWidth), GridUnitType.Pixel)});
                grid.RowDefinitions.Add(new RowDefinition());
                grid.RowDefinitions.Add(new RowDefinition());
                grid.ShowGridLines = true;

                //grid.Children.Add(mapImage);
                //Grid.SetColumn(mapImage, 0);
                //Grid.SetRow(mapImage, 0);

                RenderTargetBitmap rtBmp = new RenderTargetBitmap((int)MapImage.ActualWidth, (int)MapImage.ActualHeight,
                    96, 96, PixelFormats.Pbgra32);
                MapImage.Measure(new System.Windows.Size((int)MapImage.ActualWidth, (int)MapImage.ActualHeight));
                MapImage.Arrange(new Rect(new System.Windows.Size((int)MapImage.ActualWidth, (int)MapImage.ActualHeight)));

                rtBmp.Render(MapImage);


                var pngImage = new JpegBitmapEncoder();
                pngImage.Frames.Add(BitmapFrame.Create(rtBmp));
                BitmapImage bitmap;
                using (var stream = new System.IO.MemoryStream()) // File.Create(@"C:\Users\Ron\Documents\DataMaster\testmap2.jpeg"))
                {
                    pngImage.Save(stream);
                    stream.Seek(0, System.IO.SeekOrigin.Begin);

                    //JpegBitmapEncoder encoder2 = new JpegBitmapEncoder();
                    //encoder2.QualityLevel = 90;
                    //encoder2.Frames.Add(BitmapFrame.Create(rtBmp));
                    //encoder2.Save(stream);

                    bitmap = new BitmapImage(); //new Uri(@"C:\Users\Ron\Documents\DataMaster\testmap.jpeg"));
                    bitmap.BeginInit();
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.StreamSource = stream;
                    bitmap.EndInit();

                    stream.Close();
                }

                var mapImage2 = new Image();

                //MapImage.Measure(new System.Windows.Size((int)MapImage.ActualWidth, (int)MapImage.ActualHeight));
                //MapImage.Arrange(new Rect(new System.Windows.Size((int)MapImage.ActualWidth, (int)MapImage.ActualHeight)));

                //rtBmp.Render(MapImage);

                //PngBitmapEncoder encoder = new PngBitmapEncoder();
                //MemoryStream stream = new MemoryStream();
                //encoder.Frames.Add(BitmapFrame.Create(rtBmp));

                //// Save to memory stream and create Bitamp from stream
                //encoder.Save(stream);
                
                var centerx = MapImage.ActualWidth / 2;

                double printwidth = rectangle.Width; // MapImage.ActualWidth < 820 ? MapImage.ActualWidth : 820;
                var topX = startDrag.X; //   centerx - (printwidth / 2);

                var centery = MapImage.ActualHeight / 2;
                double printheight = rectangle.Height;  // MapImage.ActualHeight < 1024 ? MapImage.ActualHeight : 1024;
                var topy = startDrag.Y; // centery - (printheight / 2);

                var image = new CroppedBitmap(bitmap, new Int32Rect(Convert.ToInt32(topX), Convert.ToInt32(topy), Convert.ToInt32(printwidth), Convert.ToInt32(printheight)));

                var grid2 = new Grid();
                grid2.Margin = new Thickness(0);
                grid2.ColumnDefinitions.Add(new ColumnDefinition()); // { Width = new GridLength(Convert.ToInt32(doc.ColumnWidth), GridUnitType.Pixel)});
                grid2.RowDefinitions.Add(new RowDefinition());


                mapImage2.Source = image;
                mapImage2.Height = image.Height;
                mapImage2.Stretch = Stretch.None;
       
                grid2.Children.Add(mapImage2);
                Grid.SetColumn(mapImage2, 0);
                Grid.SetRow(mapImage2, 0);

                doc.Blocks.Add(new BlockUIContainer(grid2) { BorderBrush = Brushes.Black, BorderThickness = new Thickness(1, 2, 1, 2) });

                var comps = _mapViewModel.Comps;

                var gridAdresses = new Grid();
                gridAdresses.Margin = new Thickness(0, 15, 0, 5);
                gridAdresses.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(Convert.ToInt32(100), GridUnitType.Pixel) });
                gridAdresses.ColumnDefinitions.Add(new ColumnDefinition()
                {
                    Width = new GridLength(Convert.ToInt32(600), GridUnitType.Pixel)
                });

                int row = 0;

                gridAdresses.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(25, GridUnitType.Pixel) });
                var subject = new TextBlock(new Bold(new Run("SUBJECT")));
                gridAdresses.Children.Add(subject);
                Grid.SetColumn(subject, 0);
                Grid.SetRow(subject, row);

                string addressFormat = "{0} {1} {2}, {3} {4}";

                var address = new TextBlock();
                address.Inlines.Add(new Bold(new Run(string.Format("{0} {1}", _mapViewModel.Subject.AddressLine1, _mapViewModel.Subject.AddressLine2))));
                address.Inlines.Add(new Run(string.Format("{0}, {1} {2}", _mapViewModel.Subject.City, _mapViewModel.Subject.State, _mapViewModel.Subject.Zip)));

                //var address = new TextBlock(new Run(string.Format(addressFormat, _mapViewModel.Subject.AddressLine1, _mapViewModel.Subject.AddressLine2, _mapViewModel.Subject.City, _mapViewModel.Subject.State, _mapViewModel.Subject.Zip)));
                gridAdresses.Children.Add(address);
                Grid.SetColumn(address, 1);
                Grid.SetRow(address, row++);

                foreach (var comp in _mapViewModel.Comps)
                {
                    gridAdresses.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(25, GridUnitType.Pixel) });

                    var compNumber = new TextBlock(new Bold(new Run("COMP " + comp.CompNumber)));
                    gridAdresses.Children.Add(compNumber);
                    Grid.SetColumn(compNumber, 0);
                    Grid.SetRow(compNumber, row);

                    address = new TextBlock();
                    address.Inlines.Add(new Bold(new Run(string.Format("{0} {1}", comp.Address.AddressLine1, comp.Address.AddressLine2))));
                    address.Inlines.Add(new Run(string.Format("{0}, {1} {2}", comp.Address.City, comp.Address.State, comp.Address.Zip)));

                    //address = new TextBlock(new Run(string.Format(addressFormat, comp.Address.AddressLine1, comp.Address.AddressLine2, comp.Address.City, comp.Address.State, comp.Address.Zip)));
                    gridAdresses.Children.Add(address);
                    Grid.SetColumn(address, 1);
                    Grid.SetRow(address, row);

                    row++;
                }

                doc.Blocks.Add(new BlockUIContainer(gridAdresses));


                IDocumentPaginatorSource paginationSource = doc;

                printDialog.PrintDocument(paginationSource.DocumentPaginator, "Sample printing");
            }
        }
    }
}
