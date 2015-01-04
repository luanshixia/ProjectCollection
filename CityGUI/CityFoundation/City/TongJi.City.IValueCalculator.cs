using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using TongJi.Geometry;

namespace TongJi.City
{
    public interface IValueCalculator
    {
        double GetValue(Point2D p);
    }

    public class AnalysisDistrict : CityDistrict, IValueCalculator
    {
        public List<IFactor> Factors { get; set; }
        private AggregateType _aggregateType = AggregateType.Sum;
        public AggregateType AggregateType { get { return _aggregateType; } set { _aggregateType = value; } }

        public AnalysisDistrict()
            : base()
        {
        }

        public AnalysisDistrict(CityDistrict source, bool withoutFactors = false)
        {
            Name = source.Name;
            _properties = source.Properties;

            Extents = source.Extents;
            BasePrice = source.BasePrice;

            Roads = source.Roads;
            Parcels = source.Parcels;

            Factors = new List<IFactor>();
            //SetInitiate(new UniformInitiate(BasePrice));
            if (!withoutFactors)
            {
                CitySpots = source.CitySpots;
                CityLinears = source.CityLinears;
                CityRegions = source.CityRegions;
                SetTurbulence();
            }
        }

        public void AddFactor(IFactor factor)
        {
            Factors.Add(factor);
        }

        public void ToggleFactors(IEnumerable<IFactor> factors, bool on)
        {
            if (on)
            {
                foreach (var factor in factors)
                {
                    if (!Factors.Contains(factor))
                    {
                        Factors.Add(factor);
                    }
                }
            }
            else
            {
                foreach (var factor in factors)
                {
                    if (Factors.Contains(factor))
                    {
                        Factors.Remove(factor);
                    }
                }
            }
        }

        private void SetTurbulence()
        {
            Factors.Clear();
            foreach (var spot in CitySpots)
            {
                Factors.Add(spot);
            }
            foreach (var linear in CityLinears)
            {
                Factors.Add(linear);
            }
            foreach (var region in CityRegions)
            {
                Factors.Add(region);
            }
        }

        public void CountRoads(bool on)
        {
            if (on)
            {
                Roads.ForEach(r => Factors.Add(r));
            }
            else
            {
                Roads.ForEach(r => Factors.Remove(r));
            }
        }

        public void CountParcels(bool on)
        {
            if (on)
            {
                Parcels.ForEach(r => Factors.Add(r));
            }
            else
            {
                Parcels.ForEach(r => Factors.Remove(r));
            }
        }

        public double GetValue(Point2D p)
        {
            if (AggregateType == AggregateType.Sum)
            {
                return Factors.Sum(x => x.Formula(p));
            }
            else if (AggregateType == AggregateType.Min)
            {
                return Factors.Min0(x => x.Formula(p));
            }
            else if (AggregateType == AggregateType.Max)
            {
                return Factors.Max0(x => x.Formula(p));
            }
            else
            {
                return 0;
            }
        }

        public int GetSpotBelonging(Point2D p)
        {
            int minIndex = -1;
            double minValue = double.MaxValue;
            for (int i = 0; i < CitySpots.Count; i++)
            {
                double dist = CitySpots[i].Position.DistTo(p);
                if (dist < minValue)
                {
                    minValue = dist;
                    minIndex = i;
                }
            }
            return minIndex;
        }

        public override void AppendEntity(CityEntity ent)
        {
            base.AppendEntity(ent);
        }
    }

    public enum AggregateType
    {
        Sum,
        Min,
        Max
    }
}
