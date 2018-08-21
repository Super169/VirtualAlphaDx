#define TransformOnly

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

        private void LoadModels()
        {
            parts.Add(new Part(0, "HeadAndBodyLogoYZ.3ds")
            {
                initAngle = 90,
                angle = 90,
                rotatable = false
            });

            parts.Add(new Part(1, "RightArmMountYZ.3ds")
            {
                initAngle = 90,
                angle = 90,
                reverse = false,
                rotAxis = new Media3D.Vector3D(1, 0, 0),
                rotPoint = new Media3D.Point3D(0, 11.475, 0.7),
                minAngle = 0,
                maxAngle = 180,
            });

            parts.Add(new Part(2, "RightArmYZ.3ds")
            {
                initAngle = 0,
                angle = 0,
                reverse = true,
                rotAxis = new Media3D.Vector3D(0, 0, 1),
                rotPoint = new Media3D.Point3D(-2.85, 11.475, 0),
                minAngle = 0,
                maxAngle = 180,
                parentId = 1
            });

            parts.Add(new Part(3, "RightHandFrontYZ.3ds")
            {
                initAngle = 90,
                angle = 90,
                reverse = true,
                rotAxis = new Media3D.Vector3D(0, 0, 1),
                rotPoint = new Media3D.Point3D(-2.85, 8.8, 0),
                minAngle = 0,
                maxAngle = 180,
                parentId = 2
            });

            parts.Add(new Part(4, "LeftArmMountYZ.3ds")
            {
                initAngle = 90,
                angle = 90,
                reverse = true,
                rotAxis = new Media3D.Vector3D(1, 0, 0),
                rotPoint = new Media3D.Point3D(0, 11.475, 0.7),
                minAngle = 0,
                maxAngle = 180
            });

            parts.Add(new Part(5, "LeftArmYZ.3ds")
            {
                initAngle = 180,
                angle = 180,
                reverse = true,
                rotAxis = new Media3D.Vector3D(0, 0, 1),
                rotPoint = new Media3D.Point3D(2.85, 11.475, 0),
                minAngle = 0,
                maxAngle = 180,
                parentId = 4
            });

            parts.Add(new Part(6, "LeftHandFrontYZ.3ds")
            {
                initAngle = 90,
                angle = 90,
                reverse = true,
                rotAxis = new Media3D.Vector3D(0, 0, 1),
                rotPoint = new Media3D.Point3D(2.85, 8.8, 0),
                minAngle = 0,
                maxAngle = 180,
                parentId = 5
            });

            parts.Add(new Part(7, "RightFoot01YZ.3ds")
            {
                initAngle = 90,
                angle = 90,
                reverse = false,
                rotAxis = new Media3D.Vector3D(0, 0, 1),
                rotPoint = new Media3D.Point3D(-1.15, 8.36, 0),
                minAngle = 0,
                maxAngle = 140,
            });

            parts.Add(new Part(8, "RightFoot02YZ.3ds")
            {
                initAngle = 60,
                angle = 60,
                reverse = false,
                rotAxis = new Media3D.Vector3D(1, 0, 0),
                rotPoint = new Media3D.Point3D(0, 6.55, 0.3),
                minAngle = 9,
                maxAngle = 180,
                parentId = 7
            });

            parts.Add(new Part(9, "RightFoot03YZ.3ds")
            {
                initAngle = 76,
                angle = 76,
                reverse = true,
                rotAxis = new Media3D.Vector3D(1, 0, 0),
                rotPoint = new Media3D.Point3D(0, 4.7, 0.3),
                minAngle = 0,
                maxAngle = 176,
                parentId = 8
            });

            parts.Add(new Part(10, "RightFoot04YZ.3ds")
            {
                initAngle = 110,
                angle = 110,
                reverse = true,
                rotAxis = new Media3D.Vector3D(1, 0, 0),
                rotPoint = new Media3D.Point3D(0, 2.38, 0.3),
                minAngle = 0,
                maxAngle = 180,
                parentId = 9
            });


            parts.Add(new Part(11, "RightFoot05YZ.3ds")
            {
                initAngle = 90,
                angle = 90,
                reverse = false,
                rotAxis = new Media3D.Vector3D(0, 0, 1),
                rotPoint = new Media3D.Point3D(-0.9, 0.955, 0),
                minAngle = 70,
                maxAngle = 180,
                parentId = 10
            });

            parts.Add(new Part(12, "LeftFoot01YZ.3ds")
            {
                initAngle = 90,
                angle = 90,
                reverse = false,
                rotAxis = new Media3D.Vector3D(0, 0, 1),
                rotPoint = new Media3D.Point3D(1.15, 8.36, 0),
                minAngle = 40,
                maxAngle = 180,
            });

            parts.Add(new Part(13, "LeftFoot02YZ.3ds")
            {
                initAngle = 120,
                angle = 120,
                reverse = true,
                rotAxis = new Media3D.Vector3D(1, 0, 0),
                rotPoint = new Media3D.Point3D(0, 6.55, 0.3),
                minAngle = 0,
                maxAngle = 171,
                parentId = 12
            });

            parts.Add(new Part(14, "LeftFoot03YZ.3ds")
            {
                initAngle = 104,
                angle = 104,
                reverse = false,
                rotAxis = new Media3D.Vector3D(1, 0, 0),
                rotPoint = new Media3D.Point3D(0, 4.7, 0.3),
                minAngle = 4,
                maxAngle = 180,
                parentId = 13
            });

            parts.Add(new Part(15, "LeftFoot04YZ.3ds")
            {
                initAngle = 70,
                angle = 70,
                reverse = false,
                rotAxis = new Media3D.Vector3D(1, 0, 0),
                rotPoint = new Media3D.Point3D(0, 2.38, 0.3),
                minAngle = 0,
                maxAngle = 180,
                parentId = 14
            });


            parts.Add(new Part(16, "LeftFoot05YZ.3ds")
            {
                initAngle = 90,
                angle = 90,
                reverse = false,
                rotAxis = new Media3D.Vector3D(0, 0, 1),
                rotPoint = new Media3D.Point3D(0.9, 0.955, 0),
                minAngle = 0,
                maxAngle = 110,
                parentId = 15
            });

            // Set the link to parent object for fast operation later
            foreach (Part part in parts)
            {
                if (part.parentId > 0)
                {
                    part.parent = parts.Find(x => x.id == part.parentId);
                }
            }

        }

        private void AttachParts()
        {
            Element3DCollection e3Collection = new Element3DCollection();
            foreach (Part part in parts)
            {
                part.mgmList = new List<MeshGeometryModel3D>();
                foreach (var ob in part.objList)
                {

                    var s = new MeshGeometryModel3D
                    {
                        Geometry = ob.Geometry,
                        Material = ob.Material,
                    };
                    part.mgmList.Add(s);
                    e3Collection.Add(s);
                    s.Attach(this.viewport.RenderHost);
                }
            }
            this.ModelGeometry = e3Collection;
            this.OnPropertyChanged("ModelGeometry");
        }

        private void TransformParts()
        {
            foreach (Part part in parts)
            {
                Media3D.Transform3DGroup F = new Media3D.Transform3DGroup();
                if (part.angle == part.initAngle)
                {
                    part.R = null;
                }
                else
                {
                    double angle = (part.reverse ? part.initAngle - part.angle : part.angle - part.initAngle);
                    part.R = new Media3D.RotateTransform3D(new Media3D.AxisAngleRotation3D(part.rotAxis, angle), part.rotPoint);
                    F.Children.Add(part.R);
                }

                Part parentPart = part.parent;
                while (parentPart != null)
                {
                    if (parentPart.R != null)
                    {
                        F.Children.Add(parentPart.R);
                    }
                    parentPart = parentPart.parent;
                }
                if (F.Children.Count == 0)
                {
                    part.T = null;
                }
                else
                {
                    part.T = F;
                }

                foreach (MeshGeometryModel3D s in part.mgmList)
                {
                    s.Transform = part.T;
                }
            }
            this.OnPropertyChanged("ModelGeometry");
        }


    }
}
