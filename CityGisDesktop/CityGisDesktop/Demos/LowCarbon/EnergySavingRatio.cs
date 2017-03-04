using System;
using System.Collections.Generic;

namespace Dreambuild.Gis.Desktop.Demos.LowCarbon
{
    /// <summary>
    /// 节能标准
    /// </summary>
    public enum EnergySavingType
    {
        SavingRatioType,
        LeedStandardType,
        ChineseGreenBuildingStandardType
    }

    public class LeedStandard
    {
        private static Dictionary<int, double> _dict = new Dictionary<int, double>()
        {
            {0, 90},
            {1, 80}, 
            {2, 70}, 
            {3, 60}, 
        };

        public static double GetValue(int index)
        {
            return _dict[index];
        }
    }

    public class ChineseGreenBuildingStandard
    {
        private static Dictionary<int, double> _dict = new Dictionary<int, double>()
        {
            {0, 90},
            {1, 75}, 
            {2, 60}, 
        };

        public static double GetValue(int index)
        {
            return _dict[index];
        }
    }

    public class EnergySavingRatio
    {
        public EnergySavingType EnergyType { get; private set; }
        public double TempValue { get; private set; }

        public double Value
        {
            get
            {
                switch (EnergyType)
                {
                    case EnergySavingType.SavingRatioType:
                        return TempValue;
                    case EnergySavingType.LeedStandardType:
                        return LeedStandard.GetValue((int)TempValue);
                    case EnergySavingType.ChineseGreenBuildingStandardType:
                        return ChineseGreenBuildingStandard.GetValue((int)TempValue);
                    default:
                        throw new Exception("Unknown EneryType!");
                }
            }
        }

        public EnergySavingRatio(EnergySavingType energyType, double tmpValue)
        {
            EnergyType = energyType;
            TempValue = tmpValue;
        }

        public EnergySavingRatio(EnergySavingRatio other)
        {
            EnergyType = other.EnergyType;
            TempValue = other.TempValue;
        }

        public override String ToString()
        {
            return Value.ToString() + "%";
        }
    }
}
