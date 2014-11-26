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
using Microsoft.Kinect;
using Microsoft.Kinect.Input;

namespace Microsoft.Samples.Kinect.ControlsBasics
{
    /// <summary>
    /// Interaction logic for MyUC.xaml
    /// </summary>
    public partial class MyUC : UserControl
    {
        private const double DotHeight = 60;
        private const double DotWidth = 60;
        private SolidColorBrush blackBrush = Brushes.Black;
        private SolidColorBrush greenBrush = Brushes.Green;
        private SolidColorBrush yellowBrush = Brushes.Yellow;

        // Kinect variables.
        BodyFrameReader bodyReader;
        Body[] bodies;

        private TimeSpan lastTime;

        public MyUC()
        {
            this.InitializeComponent();

            this.Loaded += InkCanvas_Loaded;
        }

        void InkCanvas_Loaded(object sender, RoutedEventArgs e)
        {
            // Listen to Kinect pointer events
            KinectCoreWindow kinectCoreWindow = KinectCoreWindow.GetForCurrentThread();
            kinectCoreWindow.PointerMoved += kinectCoreWindow_PointerMoved;

            var sensor = KinectSensor.GetDefault();
            bodyReader = sensor.BodyFrameSource.OpenReader();

            this.bodyReader.FrameArrived += this.BodyReader_FrameArrived;
            this.bodies = new Body[this.bodyReader.BodyFrameSource.BodyCount];

        }

        private void BodyReader_FrameArrived(object sender, BodyFrameArrivedEventArgs args)
        {
            using (var frame = args.FrameReference.AcquireFrame())
            {
                if (frame != null)
                {
                    // This won't work if more than one player is tracked.
                    frame.GetAndRefreshBodyData(bodies);

                    ShouldDraw(bodies);
                }
            }
        }
        bool shouldDraw = false;
        private void ShouldDraw(Body[] bodies)
        {
            if (bodies == null)
            {
                shouldDraw = false;
            }
            else
            {
                foreach (var body in bodies)
                {
                    if (body == null || !body.IsTracked)
                    {
                        continue;
                    }
                    Console.WriteLine("Current hand state: " + body.HandRightState + " / " + HandState.Open);
                    if (body.HandLeftState == HandState.Closed)
                    {
                        shouldDraw = true;
                        return;
                    }
                    else if (body.HandRightState == HandState.Closed)
                    {
                        shouldDraw = true;
                        return;
                    }
                    else
                    {
                        shouldDraw = false;
                    }
                }
            }
            

            // If no bodies are tracked.
            shouldDraw = false;
        }

        private void kinectCoreWindow_PointerMoved(object sender, KinectPointerEventArgs args)
        {
            KinectPointerPoint kinectPointerPoint = args.CurrentPoint;
            if (lastTime == TimeSpan.Zero || lastTime != kinectPointerPoint.Properties.BodyTimeCounter)
            {
                lastTime = kinectPointerPoint.Properties.BodyTimeCounter;
                
            }

            RenderPointer(kinectPointerPoint.Properties.IsEngaged,
                kinectPointerPoint.Position,
                kinectPointerPoint.Properties.UnclampedPosition,
                kinectPointerPoint.Properties.HandReachExtent,
                kinectPointerPoint.Properties.BodyTimeCounter,
                kinectPointerPoint.Properties.BodyTrackingId,
                kinectPointerPoint.Properties.HandType);
        }

        private void RenderPointer(
            bool isEngaged,
            PointF position,
            PointF unclampedPosition,
            float handReachExtent,
            TimeSpan timeCounter,
            ulong trackingId,
            HandType handType)
        {
            StackPanel cursor = null;
            if (cursor == null)
            {
                cursor = new StackPanel();
                myCanvas.Children.Add(cursor);
            }

            cursor.Children.Clear();
            var ellipseColor = isEngaged ? greenBrush : yellowBrush;

            StackPanel sp = new StackPanel()
            {
                Margin = new Thickness(-5, -5, 0, 0),
                Orientation = Orientation.Horizontal
            };
            sp.Children.Add(new Ellipse()
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                Height = DotHeight,
                Width = DotWidth,
                Margin = new Thickness(5),
                Fill = ellipseColor
            });

            if (shouldDraw)
            {
                cursor.Children.Add(sp);
            }
            

            InkCanvas.SetLeft(cursor, position.X * myCanvas.ActualWidth - DotWidth / 2);
            InkCanvas.SetTop(cursor, position.Y * myCanvas.ActualHeight - DotHeight / 2);

/*            Ellipse unclampedCursor = new Ellipse()
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                Height = 60,
                Width = 60,
                StrokeThickness = 5,
                Stroke = blackBrush
            };

            myCanvas.Children.Add(unclampedCursor);
            InkCanvas.SetLeft(unclampedCursor, unclampedPosition.X * myCanvas.ActualWidth - DotWidth / 2);
            InkCanvas.SetTop(unclampedCursor, unclampedPosition.Y * myCanvas.ActualHeight - DotHeight / 2);*/
        }

    }
}
