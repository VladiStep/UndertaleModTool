using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace UndertaleModTool
{
    /// <summary>
    /// Interaction logic for UndertaleTileLayerEditor.xaml
    /// </summary>
    public partial class UndertaleTileLayerEditor : DataUserControl
    {
        public UndertaleTileLayerEditor()
        {
            InitializeComponent();
        }

        private void TileLayerEditor_Loaded(object sender, RoutedEventArgs e)
        {
            TileLayerEditor_DataContextChanged(null, new());
        }
        private void TileLayerEditor_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (IsLoaded)
            {
                LayerScroll.ScrollChanged -= LayerScroll_ScrollChanged;

                LayerScroll.ScrollToVerticalOffset(0);
                LayerScroll.ScrollToHorizontalOffset(0);

                _ = Task.Run(() =>
                {
                    Thread.Sleep(100);
                    Dispatcher.Invoke(() => LayerScroll.ScrollChanged += LayerScroll_ScrollChanged);
                });
            }
        }

        private void LayerScroll_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            ScrollViewer scrollViewer = sender as ScrollViewer;

            if (e.ExtentHeightChange != 0 || e.ExtentWidthChange != 0)
            {
                double xMousePositionOnScrollViewer = Mouse.GetPosition(scrollViewer).X;
                double yMousePositionOnScrollViewer = Mouse.GetPosition(scrollViewer).Y;
                double offsetX = e.HorizontalOffset + xMousePositionOnScrollViewer;
                double offsetY = e.VerticalOffset + yMousePositionOnScrollViewer;

                double oldExtentWidth = e.ExtentWidth - e.ExtentWidthChange;
                double oldExtentHeight = e.ExtentHeight - e.ExtentHeightChange;

                double relx = offsetX / oldExtentWidth;
                double rely = offsetY / oldExtentHeight;

                offsetX = Math.Max(relx * e.ExtentWidth - xMousePositionOnScrollViewer, 0);
                offsetY = Math.Max(rely * e.ExtentHeight - yMousePositionOnScrollViewer, 0);

                ScrollViewer scrollViewerTemp = sender as ScrollViewer;
                scrollViewerTemp.ScrollToHorizontalOffset(offsetX);
                scrollViewerTemp.ScrollToVerticalOffset(offsetY);
            }
        }

        private void Canvas_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            e.Handled = true;
            var element = sender as Canvas;
            var mousePos = e.GetPosition(element);
            var transform = element.LayoutTransform as MatrixTransform;
            var matrix = transform.Matrix;
            var scale = e.Delta >= 0 ? 1.1 : (1.0 / 1.1); // choose appropriate scaling factor

            if ((matrix.M11 > 0.2 || (matrix.M11 <= 0.2 && scale > 1)) && (matrix.M11 < 3 || (matrix.M11 >= 3 && scale < 1)))
            {
                matrix.ScaleAtPrepend(scale, scale, mousePos.X, mousePos.Y);
            }
            element.LayoutTransform = new MatrixTransform(matrix);
        }
    }

    public class BGGeometryConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            double w = 0;
            double h = 0;
            if (values.All(x => x is double))
            {
                w = (double)values[0];
                h = (double)values[1];
            }
            else if (values.All(x => x is uint))
            {
                w = (uint)values[0];
                h = (uint)values[1];
            }
            else
                return null;

            if (parameter is string par)
            {
                if (par == "1")
                    return Geometry.Parse($"M0,0 L{w},0 {w},{h}, 0,{h}Z");
                else if (par == "2")
                {
                    double x = Math.Round(w * 0.5);
                    double y = Math.Round(h * 0.5);
                    return Geometry.Parse($"M0,{y} L{w},{y} {w},{h}, {x},{h} {x},0 0,0Z");
                }
            }

            return null;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
