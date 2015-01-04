using System.Windows.Controls;
using System.Windows.Graphics;
using System.Windows.Input;
using System.Windows;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Silverlight3dApp
{
    public partial class MainPage
    {
        Scene scene;
        bool _isMouseDown;
        bool _startDrag;
        System.Windows.Point _mousePos;

        MouseState lastMouseState;

        double width;
        double height;

        public MainPage()
        {
            InitializeComponent();

            Mouse.RootControl = this;
            Microsoft.Xna.Framework.Input.Keyboard.RootControl = this;
        }

        void UpdateCamera()
        {
            float c1 = 1;
            float c2 = 30;

            // Get the new keyboard and mouse state
            MouseState mouseState = Mouse.GetState();
            KeyboardState keyState = Microsoft.Xna.Framework.Input.Keyboard.GetState();

            // Determine how much the camera should turn
            float deltaX = (float)lastMouseState.X - (float)mouseState.X;
            float deltaY = (float)lastMouseState.Y - (float)mouseState.Y;

            float vX = 0;
            float vY = 0;

            if (mouseState.X > 0.9 * width) vX = -1;
            else if (mouseState.X < 0.1 * width) vX = 1;
            if (mouseState.Y > 0.9 * height) vY = -1;
            else if (mouseState.Y < 0.1 * height) vY = 1;

            //WriteLine(deltaX);
            //WriteLine(deltaY);

            if (scene.Camera is FreeFlyCamera)
            {
                FreeFlyCamera camera = scene.Camera as FreeFlyCamera;

                // Rotate the camera
                camera.RotateLeftRight((c1 * deltaX + c2 * vX) * .0005f);
                camera.RotateUpDown(-(c1 * deltaY + c2 * vY) * .0005f);

                // Determine in which direction to move the camera
                if (keyState.IsKeyDown(Key.W)) camera.Move(1, 0);
                if (keyState.IsKeyDown(Key.S)) camera.Move(-1, 0);
                if (keyState.IsKeyDown(Key.A)) camera.Move(0, 1);
                if (keyState.IsKeyDown(Key.D)) camera.Move(0, -1);

                // Update the camera
                camera.Update();
            }
            else if (scene.Camera is OneAxisLevelFlyCamera)
            {
                OneAxisLevelFlyCamera camera = scene.Camera as OneAxisLevelFlyCamera;

                // Rotate the camera
                camera.RotateLeftRight((c1 * deltaX + c2 * vX) * .0005f);
                camera.RotateUpDown(-(c1 * deltaY + c2 * vY) * .0005f);

                // Determine in which direction to move the camera
                if (keyState.IsKeyDown(Key.W)) camera.Move(1, 0);
                if (keyState.IsKeyDown(Key.S)) camera.Move(-1, 0);
                if (keyState.IsKeyDown(Key.A)) camera.Move(0, 1);
                if (keyState.IsKeyDown(Key.D)) camera.Move(0, -1);

                // Update the camera
                camera.Update();
            }
            else if (scene.Camera is TwoAxesLevelFlyCamera)
            {
                TwoAxesLevelFlyCamera camera = scene.Camera as TwoAxesLevelFlyCamera;

                // Rotate the camera
                camera.RotateLeftRight((c1 * deltaX + c2 * vX) * .0005f);
                camera.RotateUpDown(-(c1 * deltaY + c2 * vY) * .05f);

                // Determine in which direction to move the camera
                if (keyState.IsKeyDown(Key.W)) camera.Move(1, 0);
                if (keyState.IsKeyDown(Key.S)) camera.Move(-1, 0);
                if (keyState.IsKeyDown(Key.A)) camera.Move(0, 1);
                if (keyState.IsKeyDown(Key.D)) camera.Move(0, -1);

                // Update the camera
                camera.Update();
            }

            // Update the mouse state
            lastMouseState = mouseState;
        }

        private void myDrawingSurface_Draw(object sender, DrawEventArgs e)
        {
            UpdateCamera();

            // Render scene
            scene.Draw();

            // Let's go for another turn!
            e.InvalidateSurface();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            // Check if GPU is on
            if (GraphicsDeviceManager.Current.RenderMode != RenderMode.Hardware)
            {
                MessageBox.Show("Please activate enableGPUAcceleration=true on your Silverlight plugin page.", "Warning", MessageBoxButton.OK);
            }

            // Create the scene
            scene = new Scene(myDrawingSurface);

            myDrawingSurface.SizeChanged += new SizeChangedEventHandler(myDrawingSurface_SizeChanged);
            width = myDrawingSurface.ActualWidth;
            height = myDrawingSurface.ActualHeight;
        }

        void myDrawingSurface_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            width = myDrawingSurface.ActualWidth;
            height = myDrawingSurface.ActualHeight;
        }

        private void myDrawingSurface_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (_isMouseDown)
            {
                var pos = e.GetPosition(sender as UIElement);
                if (_startDrag)
                {
                    if (scene.Camera is ArcBallCamera)
                    {
                        ArcBallCamera abCamera = scene.Camera as ArcBallCamera;
                        abCamera.Rotate(-(float)(pos.X - _mousePos.X) / 100f, -(float)(pos.Y - _mousePos.Y) / 100f);
                    }
                    else if (scene.Camera is ArcBallCamera1)
                    {
                        ArcBallCamera1 abCamera = scene.Camera as ArcBallCamera1;
                        abCamera.Rotate(-(float)(pos.X - _mousePos.X) / 100f, -(float)(pos.Y - _mousePos.Y) / 100f);
                    }
                    else if (scene.Camera is FixAxisCamera)
                    {
                        FixAxisCamera abCamera = scene.Camera as FixAxisCamera;
                        abCamera.Rotate(-(float)(pos.X - _mousePos.X) / 100f);
                    }
                    else if (scene.Camera is LevelCamera)
                    {
                        LevelCamera abCamera = scene.Camera as LevelCamera;
                        abCamera.Rotate(-(float)(pos.X - _mousePos.X) / 100f);
                        abCamera.Translate(new Microsoft.Xna.Framework.Vector3(0, 0, (float)(pos.Y - _mousePos.Y) / 10f));
                    }
                }
                else
                {
                    _startDrag = true;
                }
                _mousePos = pos;
            }
        }

        private void myDrawingSurface_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _isMouseDown = true;
        }

        private void myDrawingSurface_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _isMouseDown = false;
            _startDrag = false;
        }

        private void myDrawingSurface_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (scene.Camera is IZoomCamera)
            {
                IZoomCamera abCamera = scene.Camera as IZoomCamera;
                float dist = abCamera.Distance;
                abCamera.Zoom(-(e.Delta / 120) * (dist / 5));
            }
        }

        public void WriteLine(object text)
        {
            outputArea.Dispatcher.BeginInvoke(() =>
            {
                outputArea.Text += "\n" + text;
                outputArea.Select(outputArea.Text.Length - 1, 0);
            });
        }
    }
}
