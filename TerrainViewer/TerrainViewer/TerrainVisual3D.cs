using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows;
using HelixToolkit;


using System.Collections.Generic;


namespace terrainviewer
{
    /// <summary>
    /// Terrain model - reading model from .bt files
    /// The origin of  model will be centered around the midpoint of the model.
    /// Also support a ".btz" format - compressed with gzip. A compression method to convert from ".bt" to ".btz" can be found in the GZipHelper.
    /// No advanced LOD algorithm imported - this is for small terrains only...
    /// </summary>
    public class TerrainVisual3D : ModelVisual3D
    {         
        public string Source
        {
            get { return (string)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }
        public double ElevationRate {get; set; }
        public LinearGradientBrush Brush{get; set; }
        public TextureType Texture { get; set; }
        
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(string), typeof(TerrainVisual3D), new UIPropertyMetadata(null, SourceChanged));

        protected static void SourceChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            //((TerrainVisual3D)obj).UpdateModel();
        }
        public void LoadFromData(double[] elevations, int width, double left, double right, double top, double bottom)
        {
            var r = new TerrainModel();
            r.ElevationRate = ElevationRate;
            r.LoadFromData(elevations, width, left, right, top, bottom);
            switch (Texture)
            {
                case TextureType.Elevation:
                    r.Texture = new ElevationTexture(Brush, 8);
                    break;
                case TextureType.Slope:
                    r.Texture = new SlopeTexture(Brush, 8);
                    break;
            }
            visualChild.Content = r.CreateModel(2);
        }
        private readonly ModelVisual3D visualChild;

        public TerrainVisual3D()
        {
            visualChild = new ModelVisual3D();
            Children.Add(visualChild);
            ElevationRate = 1;
            Brush = GradientBrushes.BlueWhiteRed;
            Texture = TextureType.Slope;
        }

        public void UpdateModel()
        {
            var r = new TerrainModel();
            r.ElevationRate = ElevationRate;
            r.Load(Source);            
            switch (Texture)
            {
                case TextureType.Elevation:
                    r.Texture = new ElevationTexture(Brush, 8);
                    
                    break;
                case TextureType.Slope:
                    r.Texture = new SlopeTexture(Brush, 8);
                    break;
            }
            
            visualChild.Content = r.CreateModel(2);
        }
    }

    public class TerrainTexture
    {
        public Material Material { get; set; }
        public PointCollection TextureCoordinates { get; set; }

        public TerrainTexture()
        {
            Material = Materials.Green;
        }

        public virtual void Calculate(TerrainModel model, MeshGeometry3D mesh)
        {
        }
    }

    /// <summary>
    /// Terrain texture using a bitmap. Set the Left,Right,Bottom and Top coordinates to get the right alignment.
    /// </summary>
    public class MapTexture : TerrainTexture
    {
        public double Left { get; set; }
        public double Right { get; set; }
        public double Bottom { get; set; }
        public double Top { get; set; }

        public MapTexture(string source)
        {
            Material = MaterialHelper.CreateImageMaterial(source,1);
        }

        public override void Calculate(TerrainModel model, MeshGeometry3D mesh)
        {
            var texcoords = new PointCollection();
            foreach (var p in mesh.Positions)
            {
                double x = p.X + model.Offset.X;
                double y = p.Y + model.Offset.Y;
                double u = (x - Left) / (Right - Left);
                double v = (y - Top) / (Bottom - Top);
                texcoords.Add(new Point(u, v));
            }
            TextureCoordinates = texcoords;
        }
    }

    /// <summary>
    /// Texture by the slope angle.
    /// </summary>
    public class SlopeTexture : TerrainTexture
    {
        public Brush Brush { get; set; }

        public SlopeTexture(LinearGradientBrush gradient, int gradientSteps)
        {
            if (gradientSteps > 0)
                Brush = BrushHelper.CreateSteppedGradientBrush(gradient, gradientSteps);
            else
                Brush = gradient;
        }
        public SlopeTexture()
        {

        }
        public override void Calculate(TerrainModel model, MeshGeometry3D mesh)
        {
            var normals = MeshGeometryHelper.CalculateNormals(mesh);
            var texcoords = new PointCollection();
            var up = new Vector3D(0, 0, 1);
            for (int i = 0; i < normals.Count; i++)
            {
                double slope = Math.Acos(Vector3D.DotProduct(normals[i], up)) * 180 / Math.PI;
                double u = slope / 40;
                if (u > 1) u = 1;
                if (u < 0) u = 0;
                texcoords.Add(new Point(u, u));
            }
            TextureCoordinates = texcoords;
            Material = MaterialHelper.CreateMaterial(Brush);
        }
    }
    /// <summary>
    /// Texture by the elevation of the terrain.
    /// </summary>
    //public class ElevationTexture : TerrainTexture
    //{
    //     public Brush Brush { get; set; }

    //    public ElevationTexture(LinearGradientBrush gradient, int gradientSteps)
    //    {
    //        if (gradientSteps > 0)
    //            Brush = BrushHelper.CreateSteppedGradientBrush(gradient, gradientSteps);
    //        else
    //            Brush = gradient;
    //    }

    //    public override void Calculate(TerrainModel model, MeshGeometry3D mesh)
    //    {
    //        var texcoords = new PointCollection();
    //        foreach (var p in mesh.Positions)
    //        {
    //            double x = p.X + model.Offset.X;
    //            double y = p.Y + model.Offset.Y;
    //            double u = (model.GetElevation(x, y) - model.MinimumZ) / (model.MaximumZ - model.MinimumZ) ;
    //            if (u >= 1)  
    //                u = 1;
    //            if (u <= 0)  
    //                u = 0;
    //            texcoords.Add(new Point(u, u));
    //        }
    //        TextureCoordinates = texcoords;
    //        Material = MaterialHelper.CreateMaterial(Brush);
    //    }
    //}
    /// <summary>
    /// Texture by the direction of the steepest gradient.
    /// </summary>
    public class SlopeDirectionTexture : TerrainTexture
    {
        public Brush Brush { get; set; }

        public SlopeDirectionTexture(int gradientSteps)
        {
            if (gradientSteps > 0)
                Brush = BrushHelper.CreateSteppedGradientBrush(GradientBrushes.Hue, gradientSteps);
            else
                Brush = GradientBrushes.Hue;
        }
        public override void Calculate(TerrainModel model, MeshGeometry3D mesh)
        {
            var normals = MeshGeometryHelper.CalculateNormals(mesh);
            var texcoords = new PointCollection();
            for (int i = 0; i < normals.Count; i++)
            {
                double slopedir = Math.Atan2(normals[i].Y, normals[i].X) * 180 / Math.PI;
                if (slopedir < 0) slopedir += 360;
                double u = slopedir / 360;
                texcoords.Add(new Point(u, u));
            }
            TextureCoordinates = texcoords;
            Material = MaterialHelper.CreateMaterial(Brush);
        }
    }

    /// <summary>
    /// Read .bt files from disk, keeps the model data and creates the Model3D.
    /// The .btz format is a gzip compressed version of the .bt format.
    /// </summary>
    public class TerrainModel
    {
        public double Left { get; set; }
        public double Right { get; set; }
        public double Bottom { get; set; }
        public double Top { get; set; }
        public double MinimumZ { get; set; }
        public double MaximumZ { get; set; }
        public double[] Data { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public Point3D Offset { get; set; }
        public TerrainTexture Texture { get; set; }
        public double ElevationRate { get; set; }

        public void Load(string source)
        {
            var ext = Path.GetExtension(source).ToLower();
            switch (ext)
            {
                case ".btz": ReadBTZ(source);
                    break;
                case ".bt": ReadBT(source);
                    break;
                case ".dat": ReadDAT(source);
                    break;

            }
        }
        public void LoadFromData(double[] elevations, int width, double left, double right, double top, double bottom)
        {
            Data = (double[])elevations.Clone();
            Width = width;
            Height = elevations.Length / width;
            Left = left;
            Right = right;
            Top = top;
            Bottom = bottom;
        }
        public double GetElevation(double x, double y)
        {
            double colWidth = (Right - Left) / (Width-1) ;
            int col = (int)((x - Left) / colWidth) ;
            double rowWidth = (Bottom - Top) / (Height-1) ;
            int row = (int)((y - Top) / rowWidth);
            if (row == Height - 1)
            {
                row--;
            }
            if (col == Width - 1)
            {
                col--;
            }
            double lbElevation = Data[Width * row + col]; // 左下网格点高程
            double ltElevation = Data[Width * (row + 1) + col]; // 左上网格点高程
            double rtElevation = Data[Width * (row + 1) + col + 1]; // 右上网格点高程
            double rbElevation = Data[Width * row + col + 1];  // 右下网格点高程
            double u = (x - Left) / colWidth - col;
            double v = (y - Top) / rowWidth - row;
            return (1 - u) * (1 - v) * lbElevation + (1 - u) * v * ltElevation + u * (1 - v) * rbElevation + u * v * rtElevation;
        }
        private void ReadDAT(string source)
        {
            // 读数据
            double yfirst = 0;  //记录第一个读出来的y坐标，如果发生变化，说明第一行数据读取完毕
            Width = 0;
            List<double> databuf = new List<double>();
            StreamReader sr = new StreamReader(source);
            int index = 0;
            MinimumZ = double.MaxValue;
            MaximumZ = double.MinValue;
            while (!sr.EndOfStream)
            {
                string strRead = sr.ReadLine();
                if (strRead != "")
                {
                    string[] strs = strRead.Split(' ');
                    double z = double.Parse(strs[2]) * ElevationRate;
                    databuf.Add(z);
                    if (z < MinimumZ) MinimumZ = z;
                    if (z > MaximumZ) MaximumZ = z;
                    // 记录图幅范围,并统计行数
                    if (index == 0)
                    {
                        yfirst = double.Parse(strs[1]);
                        Point pt = new Point(double.Parse(strs[0]), double.Parse(strs[1]));
                        Left = pt.X;
                        Top = pt.Y;                        
                    }
                    if (sr.EndOfStream)
                    {
                        Point pt = new Point(double.Parse(strs[0]), double.Parse(strs[1]));
                        Right = pt.X;
                        Bottom = pt.Y;                        
                    }
                    // y值发生变化，记录下列数
                    if (double.Parse(strs[1]) != yfirst && Width == 0)
                    {
                        Width = index;
                    }
                }
                index++;
            }
            Height = databuf.Count / Width;
            Data = databuf.ToArray();
            sr.Close();
            
        }
        private void ReadBTZ(string source)
        {
            var infile = File.OpenRead(source);
            var deflateStream = new GZipStream(infile, CompressionMode.Decompress, true);

            ReadBT(deflateStream);
            deflateStream.Close();
            infile.Close();
        }

        private void ReadBT(string source)
        {
            var infile = File.OpenRead(source);
            ReadBT(infile);
            infile.Close();
        }

        /// <summary>
        /// Creates the 3D model of the terrain.
        /// </summary>
        /// <param name="lod">The level of detail.</param>
        /// <returns></returns>
        public GeometryModel3D CreateModel(int lod)
        {

            int ni = Height / lod;
            int nj = Width / lod;
            var pts = new Point3DCollection(ni * nj);

            double mx = (Left + Right) / 2;
            double my = (Top + Bottom) / 2;
            double mz = (MinimumZ + MaximumZ) / 2;

            Offset = new Point3D(mx, my, mz);

            for (int i = 0; i < ni; i++)
                for (int j = 0; j < nj; j++)
                {
                    double x = Left + (Right - Left) * j / (nj - 1);
                    double y = Top + (Bottom - Top) * i / (ni - 1);
                    double z = Data[i * lod * Width + j * lod];

                    x -= Offset.X;
                    y -= Offset.Y;
                    z -= Offset.Z;
                    pts.Add(new Point3D(x, y, z));
                }

            var mb = new MeshBuilder(false, false);
            mb.AddRectangularMesh(pts, nj);
            var mesh = mb.ToMesh();

            var material = Materials.Green;

            if (Texture != null)
            {
                Texture.Calculate(this, mesh);
                material = Texture.Material;
                mesh.TextureCoordinates = Texture.TextureCoordinates;
            }

            var model = new GeometryModel3D();
            model.Geometry = mesh;
            model.Material = material;
            model.BackMaterial = material;
            return model;
        }

        /// <summary>
        /// Reads a .bt (Binary terrain) file.
        /// http://www.vterrain.org/Implementation/Formats/BT.html
        /// </summary>
        /// <param name="s">The stream.</param>
        /// <returns></returns>
        public void ReadBT(Stream s)
        {
            var reader = new BinaryReader(s);

            var buffer = reader.ReadBytes(10);
            var enc = new ASCIIEncoding();
            var marker = enc.GetString(buffer);
            if (!marker.StartsWith("binterr"))
                throw new FileFormatException("Invalid marker.");
            var version = marker.Substring(7);

            Width = reader.ReadInt32();
            Height = reader.ReadInt32();
            short dataSize = reader.ReadInt16();
            bool isFloatingPoint = reader.ReadInt16() == 1;
            short horizontalUnits = reader.ReadInt16();
            short utmZone = reader.ReadInt16();
            short datum = reader.ReadInt16();
            Left = reader.ReadDouble();
            Right = reader.ReadDouble();
            Bottom = reader.ReadDouble();
            Top = reader.ReadDouble();
            short proj = reader.ReadInt16();
            float scale = reader.ReadSingle();
            var padding = reader.ReadBytes(190);

            int index = 0;
            Data = new double[Width * Height];
            MinimumZ = double.MaxValue;
            MaximumZ = double.MinValue;

            for (int y = 0; y < Height; y++)
                for (int x = 0; x < Width; x++)
                {
                    double z;

                    if (dataSize == 2)
                    {
                        z = reader.ReadUInt16();
                    }
                    else
                    {
                        z = isFloatingPoint ? reader.ReadSingle() : reader.ReadUInt32();
                    }
                    Data[index++] = z;
                    if (z < MinimumZ) MinimumZ = z;
                    if (z > MaximumZ) MaximumZ = z;
                }
            reader.Close();
        }        
    }
}
