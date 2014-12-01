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
using System.ComponentModel;
using System.IO;

namespace Microsoft.Samples.Kinect.ControlsBasics
{
    /// <summary>
    /// Interaction logic for My2DZoom.xaml
    /// </summary>
    public partial class My2DZoom : UserControl
    {
        MainModel model;

        

        private double zoomValue = 1.0;

        public My2DZoom()
        {
            this.InitializeComponent();

            model = new MainModel();
            DataContext = model;

            // Initial image.
            model.SetImageData(File.ReadAllBytes(@"..\..\..\Images\beach.jpg"));

        }

        private void MyImage_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
            {
                zoomValue += 0.1;
            }
            else
            {
                zoomValue -= 0.1;
            }

            ScaleTransform scale = new ScaleTransform(zoomValue, zoomValue);

            MyImage.LayoutTransform = scale;
            e.Handled = true;
        }

        private void Standard_Click(object sender, RoutedEventArgs e)
        {
            // Initial image.
            model.SetImageData(File.ReadAllBytes(@"..\..\..\Images\beach.jpg"));
        }

        private void Countryside_Click(object sender, RoutedEventArgs e)
        {
            if (KinectModeEngaged.KinectMode)
            {
                model.SetImageData(File.ReadAllBytes(@"..\..\..\Images\CountrySideHiddenMeaningKinect.jpg"));
            }
            else
            {
                model.SetImageData(File.ReadAllBytes(@"..\..\..\Images\CountrySideHiddenMeaning.jpg"));
            }
            
        }

        private void Lake_Click(object sender, RoutedEventArgs e)
        {
            if (KinectModeEngaged.KinectMode)
            {
                model.SetImageData(File.ReadAllBytes(@"..\..\..\Images\LakeHiddenMeaningKinect.jpg"));
            }
            else
            {
                model.SetImageData(File.ReadAllBytes(@"..\..\..\Images\LakeHiddenMeaning.jpg"));
            }
        }

        private void Road_Click(object sender, RoutedEventArgs e)
        {
            if (KinectModeEngaged.KinectMode)
            {
                model.SetImageData(File.ReadAllBytes(@"..\..\..\Images\SnowyRoadHiddenMeaningKinect.jpg"));
            }
            else
            {
                model.SetImageData(File.ReadAllBytes(@"..\..\..\Images\SnowyRoadHiddenMeaning.jpg"));
            }
        }
    }

    class MainModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void SetImageData(byte[] data)
        {
            var source = new BitmapImage();
            source.BeginInit();
            source.StreamSource = new MemoryStream(data);
            source.EndInit();

            // use public setter
            ImageSource = source;
        }

        ImageSource imageSource;
        public ImageSource ImageSource
        {
            get { return imageSource; }
            set
            {
                imageSource = value;
                OnPropertyChanged("ImageSource");
            }
        }

        protected void OnPropertyChanged(string name)
        {
            var handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}
