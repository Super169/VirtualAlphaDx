using HelixToolkit.Wpf.SharpDX;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace VirtualAlphaDX
{
    public class Part
    {
        public int id = 0;
        public List<Object3D> objList;
        public List<MeshGeometryModel3D> mgmList;
        public Transform3DGroup T = null;
        public RotateTransform3D R = null;

        public bool isLocked = false;
        public double initAngle = 0;
        public double angle = 0;
        public int parentId = 0;
        public Part parent = null;

        public bool rotatable = true;
        public bool reverse = false;

        public Vector3D rotAxis;
        public Point3D rotPoint;

        public double minAngle = 0;
        public double maxAngle = 240;

        public bool animination = false;
        public double startAngle = 0;
        public double targetAngle = 0;
        public long startTicks = 0;
        public long durationTicks = 0;

        public Part(int id, List<Object3D> objList)
        {
            this.id = id;
            this.objList = objList;
        }

        public Part(int id, string resourceName)
        {
            this.id = id;

            var assembly = Assembly.GetExecutingAssembly();
            var ns = GetType().Namespace;
            string resourcePath = ns + "._3D_Models.";

            Stream stream = assembly.GetManifestResourceStream(resourcePath + resourceName);
            StreamReader stReader = new StreamReader(stream);
            var reader = new StudioReader();
            this.objList = reader.Read(stream);
        }

        public bool MoveTo(double target, int ms)
        {
            target = (target < this.minAngle ? minAngle : (target > this.maxAngle ? maxAngle : target));
            if (this.angle == target)
            {
                this.animination = false;
            }
            else
            {
                this.targetAngle = target;
                this.startAngle = this.angle;
                this.startTicks = DateTime.Now.Ticks;
                this.durationTicks = ms * TimeSpan.TicksPerMillisecond;
                this.animination = true;
            }
            return this.animination;
        }

    }

}
