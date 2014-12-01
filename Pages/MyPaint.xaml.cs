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
    /// Interaction logic for MyPaint.xaml
    /// </summary>
    public partial class MyPaint : UserControl
    {
        private const double DotHeight = 25;
        private const double DotWidth = 25;
        private SolidColorBrush blackBrush = Brushes.Black;
        private SolidColorBrush greenBrush = Brushes.Green;
        private SolidColorBrush yellowBrush = Brushes.Yellow;

        // Kinect variables.
        bool shouldDraw_Lefthand = false;
        bool shouldDraw_Righthand = false;

        PointF lastPoint;

        BodyFrameReader bodyReader;
        Body[] bodies;

        private TimeSpan lastTime;

        public MyPaint()
        {
            this.InitializeComponent();

            this.Loaded += InkCanvas_Loaded;
            this.Unloaded -= InkCanvas_Unloaded;
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

        void InkCanvas_Unloaded(object sender, RoutedEventArgs e)
        {
            this.bodyReader.FrameArrived -= this.BodyReader_FrameArrived;
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
        
        private void ShouldDraw(Body[] bodies)
        {
            if (bodies == null)
            {
                shouldDraw_Lefthand = false;
                shouldDraw_Righthand = false;
            }
            else
            {
                foreach (var body in bodies)
                {
                    if (body == null || !body.IsTracked)
                    {
                        continue;
                    }
                    
                    if (body.HandLeftState == HandState.Closed)
                    {
                        shouldDraw_Lefthand = true;
                        shouldDraw_Righthand = false;
                        return;
                    }
                    else if (body.HandRightState == HandState.Closed)
                    {
                        shouldDraw_Righthand = true;
                        shouldDraw_Lefthand = false;
                        return;

                        
                    }
                    else
                    {
                        shouldDraw_Lefthand = false;
                        shouldDraw_Righthand = false;
                    }
                }
            }
            

            // If no bodies are tracked.
            shouldDraw_Lefthand = false;
            shouldDraw_Righthand = false;
        }

        private void kinectCoreWindow_PointerMoved(object sender, KinectPointerEventArgs args)
        {
            KinectPointerPoint kinectPointerPoint = args.CurrentPoint;
            if (lastTime == TimeSpan.Zero || lastTime != kinectPointerPoint.Properties.BodyTimeCounter)
            {
                lastTime = kinectPointerPoint.Properties.BodyTimeCounter;
                
            }
            
            if (kinectPointerPoint.Properties.HandType == HandType.LEFT && shouldDraw_Lefthand == true)
            {
                RenderPointer(kinectPointerPoint.Properties.IsEngaged,
                kinectPointerPoint.Position,
                kinectPointerPoint.Properties.UnclampedPosition,
                kinectPointerPoint.Properties.HandReachExtent,
                kinectPointerPoint.Properties.BodyTimeCounter,
                kinectPointerPoint.Properties.BodyTrackingId,
                kinectPointerPoint.Properties.HandType);
            }
            else if (kinectPointerPoint.Properties.HandType == HandType.RIGHT && shouldDraw_Righthand == true)
            {
                RenderPointer(kinectPointerPoint.Properties.IsEngaged,
                kinectPointerPoint.Position,
                kinectPointerPoint.Properties.UnclampedPosition,
                kinectPointerPoint.Properties.HandReachExtent,
                kinectPointerPoint.Properties.BodyTimeCounter,
                kinectPointerPoint.Properties.BodyTrackingId,
                kinectPointerPoint.Properties.HandType);
            }
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
            var ellipseColor = blackBrush;
            /*
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
            */


            Line line = new Line();

            line.StrokeDashCap = PenLineCap.Round;
            line.StrokeStartLineCap = PenLineCap.Round;
            line.StrokeEndLineCap = PenLineCap.Round; 

            line.Stroke = new SolidColorBrush(myCanvas.DefaultDrawingAttributes.Color);
            line.StrokeThickness = 25;

            line.X1 = lastPoint.X;
            line.Y1 = lastPoint.Y;
            line.X2 = position.X;
            line.Y2 = position.Y;

            lastPoint = position;

            

            cursor.Children.Add(line);
            
            InkCanvas.SetLeft(cursor, position.X * myCanvas.ActualWidth - DotWidth / 2);
            InkCanvas.SetTop(cursor, position.Y * myCanvas.ActualHeight - DotHeight / 2);

            //myCanvas.Children.Add(line);
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            myCanvas.Strokes.Clear();
            myCanvas.Children.Clear();
        }

        private void BrushButton_Click(object sender, RoutedEventArgs e)
        {
            myCanvas.DefaultDrawingAttributes.Color = Colors.Black;
        }

        private void EraserButton_Click(object sender, RoutedEventArgs e)
        {
            myCanvas.DefaultDrawingAttributes.Color = Colors.White;
            
        }
    }
}
