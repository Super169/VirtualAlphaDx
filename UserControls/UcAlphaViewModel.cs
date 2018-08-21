using HelixToolkit.Wpf.SharpDX;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using Media3D = System.Windows.Media.Media3D;

namespace VirtualAlphaDX
{

    public partial class UcAlphaViewModel : ObservableObject
    {
        public delegate void ServoMovedEventHandler(int id, double angle);
        public event ServoMovedEventHandler servoMovedEventHandler = null;

        public void ServoMoved(int id, double angle)
        {
            servoMovedEventHandler?.Invoke(id, angle);
        }

        public Viewport3DX viewport { get; set; }

        // Binding properties
        public string Title { get; set; } = "VirtualAlpha";
        public Brush Background { get; set; }
        public SharpDX.Color4 BackgroundColor { get; set; }
        public Camera Camera { get; set; }
        public bool ShowCameraInfo { get; set; } = false;
        public bool ShowCoordinateSystem { get; set; } = true;
        public string CoordinateSystemVerticalPosition { get; set; } = "Bottom";
        public string CoordinateSystemHorizontalPosition { get; set; } = "Left";

        public DefaultEffectsManager EffectsManager { get; private set; }
        public DefaultRenderTechniquesManager RenderTechniquesManager { get; private set; }

        public Element3DCollection ModelGeometry { get; private set; }
        public Media3D.Transform3D ModelTransform { get; private set; }

        public List<Part> parts = new List<Part>();

        Timer aniTimer;
        Timer dummyTimer;

        bool isReady = false;

        List<MeshGeometryModel3D> mg = new List<MeshGeometryModel3D>();

        public UcAlphaViewModel(Viewport3DX viewport, ServoMovedEventHandler servoMovedEventHandler = null)
        {
            this.viewport = viewport;
            this.servoMovedEventHandler = servoMovedEventHandler;
            Background = new LinearGradientBrush(Colors.Black, Colors.DarkBlue, 90);
            BackgroundColor = new SharpDX.Color4(0, 0, 0, 0);
            this.Camera = new PerspectiveCamera
            {
                LookDirection = new Media3D.Vector3D(-5.059, 1.949, -23.856),
                UpDirection = new Media3D.Vector3D(0.067, 0.946, 0.319),
                Position = new Media3D.Point3D(3.648, 5.954, 23.353)
            };

            RenderTechniquesManager = new DefaultRenderTechniquesManager();
            EffectsManager = new DefaultEffectsManager(RenderTechniquesManager);

            this.ModelTransform = new Media3D.TranslateTransform3D(0, 0, 0);
            this.ModelGeometry = new Element3DCollection();

        }

        public void DummyAction()
        {
            
        }

        public void Initialization()
        {
            // only initialize once
            if (isReady) return;
            
            LoadModels();
            AttachParts();
            TransformParts();
            InitializeTimer();

            isReady = true;
        }

        public bool IsAnimation(int id)
        {
            Part part = parts.Find(x => x.id == id);
            if (part == null) return false;
            return (part.animination);
        }

        public bool MoveTo(int id, double angle, int ms)
        {
            Part part = parts.Find(x => x.id == id);
            if (part == null) return false;
            if (angle > 0xF0) return true;  // no action will be take
            if (ms < 0) ms = 0;
            return part.MoveTo(angle, ms);
        }

        public double GetServoAngle(int id)
        {
            return parts[id].angle;
        }

        public Part GetPart(int id)
        {
            return (parts.Find(x => x.id == id));
        }

        public void StartAnimation()
        {
            aniTimer.Enabled = true;
        }

        public void StopAnimation()
        {
            if (aniTimer != null) aniTimer.Stop();
        }

        private void InitializeTimer()
        {
            aniTimer = new Timer();
            aniTimer.Interval = 50;  // 20 fps
            aniTimer.Tick += new EventHandler(aniTimer_TickHandler);

            // InitializeDummyTimer();

        }

        bool oneMoreWait = true;
        long endTicks = 0;

        private void aniTimer_TickHandler(object sender, EventArgs e)
        {
            aniTimer.Stop();
            if (UpdateAnimation())
            {
                aniTimer.Enabled = true;
            }
            else
            {
                // TODO: how to make sure the last one is show?
                if (oneMoreWait)
                {
                    oneMoreWait = false;
                    aniTimer.Enabled = true;
                    endTicks = DateTime.Now.Ticks + 200 * TimeSpan.TicksPerMillisecond;
                }
                else
                {
                    if (DateTime.Now.Ticks > endTicks)
                    {
                        oneMoreWait = true;
                        aniTimer.Enabled = false;
                    }
                }
            }
        }

        private void InitializeDummyTimer()
        {
            dummyTimer = new Timer();
            dummyTimer.Interval = 1000;
            dummyTimer.Tick += new EventHandler(dummyTimer_TickHandler);

            dummyTimer.Enabled = true;
            dummyTimer.Start();

        }

        private void dummyTimer_TickHandler(object sender, EventArgs e)
        {
            dummyTimer.Stop();

            MoveTo(1, 0, 1000);
            MoveTo(3, 180, 2000);

            aniTimer.Start();
        }

        private bool UpdateAnimation()
        {
            if (!parts.Exists(x => x.animination)) return false;
            long currTicks = DateTime.Now.Ticks;
            bool goAnimation = false;
            foreach (Part part in parts)
            {
                if (part.animination)
                {
                    if (part.angle == part.targetAngle)
                    {
                        part.animination = false;
                    }
                    else if (currTicks >= (part.startTicks + part.durationTicks))
                    {
                        part.angle = part.targetAngle;
                        part.animination = false;
                    }
                    else
                    {
                        double moveAngle = (part.targetAngle - part.startAngle) * (currTicks - part.startTicks) / part.durationTicks;
                        part.angle = part.startAngle + moveAngle;
                        goAnimation = true;
                    }
                    ServoMoved(part.id, part.angle);
                }
            }
            TransformParts();
            return goAnimation;
        }
    }
}
