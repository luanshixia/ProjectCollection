using System;
using System.Collections.Generic;
using System.Linq;

using TongJi.Geometry;
using TongJi.Geometry3D;
using TongJi.Gis;

namespace TongJi.City.Analysis3D
{
    public class CityScene
    {
        public List<IFeature> Parcels { get; private set; }
        public List<IFeature> Roads { get; private set; }
        public List<Mesh> Meshes { get; private set; }

        public CityScene(Map cityMap)
        {
            Parcels = cityMap.Layers["地块"].Features.ToList();
            Roads = cityMap.Layers["道路"].Features.ToList();

            BuildMeshes();
        }

        private void BuildMeshes()
        {
            Meshes = Parcels.Select(p => Building.FromParcel(p).ToMesh()).ToList();
        }
    }

    public class Building
    {
        public Polyline BaseShape;
        public double Height;

        public const double HeightPerLevel = 3;

        public Building(Polyline baseShape, double height)
        {
            BaseShape = baseShape;
            Height = height;
        }

        public static Building FromParcel(IFeature parcel)
        {
            Polyline parcelShape = new Polyline(parcel.GeoData);
            double area = parcelShape.Area;
            double bd = Convert.ToDouble(parcel["建筑密度"]);
            bd = bd == 0 ? 1 : bd > 1 ? bd / 100 : bd;
            double far = Convert.ToDouble(parcel["容积率"]);
            double floor = far * area;
            int levels = (int)Math.Ceiling(floor / (bd * area));
            double sxy = Math.Sqrt(bd);
            Matrix3 scale = Matrix3.Scale(sxy, sxy, parcelShape.Centroid);
            Polyline buildingBaseShape = new Polyline(parcelShape.Points.Select(p => scale.MultiplyVector2(p)).ToList());
            return new Building(buildingBaseShape, levels * HeightPerLevel);
        }

        public Mesh ToMesh()
        {
            if (Height > 0)
            {
                return MeshBuilder.ExtrudeWithCaps(new Polyline(BaseShape.Points), Height).ToMesh();
            }
            else
            {
                return MeshBuilder.Planar(new Polyline(BaseShape.Points)).ToMesh();
            }
        }
    }
}
