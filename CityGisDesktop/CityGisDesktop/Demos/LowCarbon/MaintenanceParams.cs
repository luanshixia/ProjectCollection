using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Dreambuild.Gis.Desktop.Demos.LowCarbon
{
    public class MaintenanceParams // todo: params cannot be updated when BaseYear or City changed.
    {
        public double[] AirConditioning { get; set; }
        public double[] Equipment { get; set; }
        public double[] Heating { get; set; }
        public double[] Lighting { get; set; }

        const string DataDirectory = @"Demos\LowCarbon\Data";

        public MaintenanceParams()
        {
            Refresh();
        }

        public void Refresh()
        {
            InitArrays();
            FillArrays();
        }

        private void FillArrays()
        {
            FillArray(GetFilePath("AirConditioning.csv"), AirConditioning);
            FillArray(GetFilePath("Equipment.csv"), Equipment);
            FillArray(GetFilePath("Heating.csv"), Heating);
            FillArray(GetFilePath("Lighting.csv"), Lighting);
        }

        private string GetFilePath(string path)
        {
            return Path.Combine(DataDirectory, path);
        }

        private void InitArrays()
        {
            int buildingTypeCount = Enum.GetValues(typeof(BuildingType)).Length;
            AirConditioning = new double[buildingTypeCount];
            Equipment = new double[buildingTypeCount];
            Heating = new double[buildingTypeCount];
            Lighting = new double[buildingTypeCount];
        }

        private int GetBuildingTypeIndex(string buildingType)
        {
            switch (buildingType)
            {
                case "多层办公":
                    return 0;
                case "多层住宅":
                    return 1;
                case "高层办公":
                    return 2;
                case "高层住宅":
                    return 3;
                case "旅馆":
                    return 4;
                case "商场":
                    return 5;
                default:
                    return -1;
            }
        }

        private void FillArray(string filePath, double[] array)
        {
            var cityLocation = Parameter.Base.CityLocation;
            var baseYear = Parameter.Base.BaseYear;
            string[] lines = System.IO.File.ReadAllLines(filePath, Encoding.Default);
            var pairs = from line in lines
                        let fields = line.Split(',')
                        where fields[0] == cityLocation
                        select new
                        {
                            Key = fields[1],
                            Value = fields[baseYear - 2003]
                        };
            foreach (var pair in pairs)
            {
                int index = GetBuildingTypeIndex(pair.Key);
                array[index] = Convert.ToDouble(pair.Value);
            }
        }
    }
}
