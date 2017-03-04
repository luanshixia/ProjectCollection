using IronPython.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dreambuild.Gis.Desktop.Demos.LowCarbon
{
    public class LcComputation
    {
        public ComputationUnitType CaculateType { get; private set; }
        public Map ModelMap { get; private set; }
        public List<ComputationUnit> AllUnits { get; private set; }

        public LcComputation(Map city, ComputationUnitType caculateType)
        {
            CaculateType = (ComputationUnitType)caculateType;
            ModelMap = city;
            UpdateAllUnits();
        }

        public void UpdateAllUnits()
        {
            List<ComputationUnit> tmpList = ModelMap.Layers.SelectMany(layer => layer.Features.Select(ComputationUnit.Get)).ToList();
            //AllUnits = tmpList.Where(u => (u.UnitType == CaculateType)).ToList();
            AllUnits = tmpList.ToList();
        }

        public void SetEnergySavingRatio(EnergySavingRatio ratio)
        {
            foreach (var unit in AllUnits)
            {
                unit.EnergySavingCoefficient = ratio;
            }
        }

        public void SetPerCapitaIndustry(TripParams trip)
        {
            foreach (var unit in AllUnits)
            {
                unit.PerCapitaIndustry = trip.PerCapitaIndustry;
            }
        }

        public void SetPerCapitaOffice(TripParams trip)
        {
            foreach (var unit in AllUnits)
            {
                unit.PerCapitaOffice = trip.PerCapitaOffice;
            }
        }

        public void SetPerCapitaResidencial(TripParams trip)
        {
            foreach (var unit in AllUnits)
            {
                unit.PerCapitaResidencial = trip.PerCapitaResidencial;
            }
        }

        #region Green

        public double GetGreenCarbonConsumption()
        {
            return AllUnits.Sum(u => GetGreenCarbonConsumption(u));
        }

        public static double GetGreenCarbonConsumption(ComputationUnit unit)
        {
            double result = 0;
            if (unit.Type == BuildingType.Mixed)
            {
                ComputationUnit tmp = new ComputationUnit(unit);
                tmp.Area = unit.Area * unit.MixGreenFieldPercent;
                tmp.Type = BuildingType.GreenField;
                result = GetGreenCarbonConsumption(tmp);
            }
            else if (unit.Type == BuildingType.GreenField)
            {
                switch (unit.GreenField)
                {
                    case GreenFieldType.Bush:
                        result = unit.Area * Parameter.Green.BushesUnit;
                        break;
                    case GreenFieldType.Farm:
                        result = unit.Area * Parameter.Green.FarmUnit;
                        break;
                    case GreenFieldType.Park:
                        result = unit.Area * Parameter.Green.ParkUnit;
                        break;
                    case GreenFieldType.Forest:
                        result = unit.Area * Parameter.Green.ForestUnit;
                        break;
                    default:
                        result = 0;
                        break;
                }
            }
            unit.GreenFieldConsumption = result;
            return result;
        }

        #endregion Green

        #region Building

        public BuildingResult GetBuildingCarbonProduction()
        {
            var list = AllUnits.Select(GetBuildingCarbonProduction).ToList();
            return new BuildingResult
            {
                Material = list.Sum(x => x.Material),
                Construction = list.Sum(x => x.Construction),
                Maintenance = new MaintenanceResult
                {
                    AirConditioning = list.Sum(x => x.Maintenance.AirConditioning),
                    Lighting = list.Sum(x => x.Maintenance.Lighting),
                    Equipment = list.Sum(x => x.Maintenance.Equipment),
                    Heating = list.Sum(x => x.Maintenance.Heating)
                },
                Recycle = list.Sum(x => x.Recycle)
            };
        }

        public static BuildingResult GetBuildingCarbonProduction(ComputationUnit building)
        {
            BuildingResult result = new BuildingResult();
            if (building.Type == BuildingType.Mixed)
            {
                List<BuildingResult> resultList = new List<BuildingResult>();
                if (building.MixEducationPercent != 0.0 || building.MixIndustryPercent != 0.0
                    || building.MixOfficePercent != 0.0 || building.MixOtherPercent != 0.0) // 教育和工业属同一类型：办公
                {
                    ComputationUnit tmp = new ComputationUnit(building);
                    tmp.Area = building.Area * (building.MixEducationPercent + building.MixIndustryPercent + building.MixOfficePercent + building.MixOtherPercent);
                    if (tmp.IsHighBuilding())
                    {
                        tmp.Type = BuildingType.HighOffice;
                    }
                    else
                    {
                        tmp.Type = BuildingType.MidHighOffice;
                    }
                    resultList.Add(_GetBuildingCarbonProduction(tmp));
                }
                if (building.MixGreenFieldPercent != 0.0)
                {
                    ComputationUnit tmp = new ComputationUnit(building);
                    tmp.Area = building.Area * building.MixRetailPercent;
                    tmp.Type = BuildingType.GreenField;
                    resultList.Add(_GetBuildingCarbonProduction(tmp));
                }
                if (building.MixHotelPercent != 0.0)
                {
                    ComputationUnit tmp = new ComputationUnit(building);
                    tmp.Area = building.Area * building.MixHotelPercent;
                    tmp.Type = BuildingType.Hotel;
                    resultList.Add(_GetBuildingCarbonProduction(tmp));
                }
                if (building.MixResidencialPercent != 0.0)
                {
                    ComputationUnit tmp = new ComputationUnit(building);
                    tmp.Area = building.Area * building.MixResidencialPercent;
                    if (tmp.IsHighBuilding())
                    {
                        tmp.Type = BuildingType.HighResidencial;
                    }
                    else
                    {
                        tmp.Type = BuildingType.MidHighResidencial;
                    }
                    resultList.Add(_GetBuildingCarbonProduction(tmp));
                }
                if (building.MixRetailPercent != 0.0)
                {
                    ComputationUnit tmp = new ComputationUnit(building);
                    tmp.Area = building.Area * building.MixRetailPercent;
                    tmp.Type = BuildingType.Retail;
                    resultList.Add(_GetBuildingCarbonProduction(tmp));
                }
                foreach (var r in resultList)
                {
                    result.Material += r.Material;
                    result.Construction += r.Construction;
                    result.Maintenance.AirConditioning += r.Maintenance.AirConditioning;
                    result.Maintenance.Lighting += r.Maintenance.Lighting;
                    result.Maintenance.Equipment += r.Maintenance.Equipment;
                    result.Maintenance.Heating += r.Maintenance.Heating;
                    result.Recycle += r.Recycle;
                }
                building.CarbonProduction = result;
                return result;
            }
            else
            {
                result = _GetBuildingCarbonProduction(building);
            }
            building.CarbonProduction = result;
            return result;
        }

        public static BuildingResult _GetBuildingCarbonProduction(ComputationUnit building)
        {
            BuildingResult result = new BuildingResult();

            if (building.Type == BuildingType.GreenField || building.Type == BuildingType.Unknown)
            {
                return result;
            }

            double energyUsingRatio = 1 - building.EnergySavingCoefficient.Value / 100;
            double otherCleanEnergyRatio = Parameter.Base.OtherCleanEnergyRatio / 100;

            BuildingType computeBuildingType = building.Type;
            if (building.Type == BuildingType.Education || building.Type == BuildingType.Industry || building.Type == BuildingType.Other || building.Type == BuildingType.Office)
            {
                computeBuildingType = building.IsHighBuilding() ? BuildingType.HighOffice : BuildingType.HighOffice;
            }
            else if (building.Type == BuildingType.Residencial)
            {
                computeBuildingType = building.IsHighBuilding() ? BuildingType.HighResidencial : BuildingType.MidHighResidencial;
            }

            // 1. 建材准备
            result.Material = building.Floor * GetPrepareConstructionProduction(computeBuildingType, Parameter.Building) * energyUsingRatio;

            // 2. 建造过程
            result.Construction = building.Floor * GetConstructionProduction(computeBuildingType, building.Structure) * energyUsingRatio;

            // 3. 运行维护
            // a. 空调
            result.Maintenance.AirConditioning = building.Floor *
                                                 GetOperationProduction(computeBuildingType, Parameter.Maintenance.AirConditioning) *
                                                 Parameter.Base.ElectricityProductionFactor *
                                                 (1 - otherCleanEnergyRatio) * energyUsingRatio;
            // b. 照明
            result.Maintenance.Lighting = building.Floor *
                                          GetOperationProduction(computeBuildingType, Parameter.Maintenance.Lighting) *
                                          Parameter.Base.ElectricityProductionFactor * (1 - otherCleanEnergyRatio) *
                                          energyUsingRatio;
            // c. 设备
            result.Maintenance.Equipment = building.Floor *
                                           GetOperationProduction(computeBuildingType, Parameter.Maintenance.Equipment) *
                                           Parameter.Base.ElectricityProductionFactor * (1 - otherCleanEnergyRatio) *
                                           energyUsingRatio;
            // d. 供暖
            result.Maintenance.Heating = building.Floor *
                                         GetOperationProduction(computeBuildingType, Parameter.Maintenance.Heating) *
                                         Parameter.Base.GasProductionFactor * (1 - otherCleanEnergyRatio) *
                                         energyUsingRatio;

            // 4. 拆毁回收
            result.Recycle = building.Floor * GetRecycleProduction(computeBuildingType, building.Structure) * energyUsingRatio;

            return result;

        }

        private static double GetOperationProduction(BuildingType buildingType, double[] productionArray)
        {
            switch (buildingType)
            {
                case BuildingType.MidHighOffice:
                    return productionArray[0];
                case BuildingType.MidHighResidencial:
                    return productionArray[1];
                case BuildingType.HighOffice:
                    return productionArray[2];
                case BuildingType.HighResidencial:
                    return productionArray[3];
                case BuildingType.Hotel:
                    return productionArray[4];
                case BuildingType.Retail:
                    return productionArray[5];
                case BuildingType.GreenField:
                    return 0;
                default:
                    throw new Exception("Unknown building type [" + buildingType + "]");
            }
        }

        private static double GetPrepareConstructionProduction(BuildingType buildingType, BuildingParams buildingParams)
        {
            switch (buildingType)
            {
                case BuildingType.GreenField:
                    return 0;
                case BuildingType.Other:
                    return buildingParams.PublicBuildingMaterial;
                case BuildingType.Mixed:
                    return 0;
                case BuildingType.Unknown:
                    return 0;
                case BuildingType.MidHighResidencial:
                case BuildingType.HighResidencial:
                    return buildingParams.ResidentialBuildingMaterial;
                default:
                    return buildingParams.PublicBuildingMaterial;
            }
        }

        private static double GetConstructionProduction(BuildingType buildingType, StructuralType structure)
        {
            switch (buildingType)
            {
                case BuildingType.GreenField:
                    return 0;
                case BuildingType.Mixed:
                    return 0;
            }

            switch (structure)
            {
                case StructuralType.Steel:
                    return Parameter.Building.SteelStructureContruction;
                case StructuralType.Timber:
                    return Parameter.Building.TimberStructureContruction;
                default:
                    return Parameter.Building.ConcreteStructureContruction;
            }
        }

        private static double GetRecycleProduction(BuildingType buildingType, StructuralType structure)
        {
            switch (buildingType)
            {
                case BuildingType.GreenField:
                    return 0;
                case BuildingType.Mixed:
                    return 0;
            }

            switch (structure)
            {
                case StructuralType.Steel:
                    return Parameter.Building.SteelStructureRecycle;
                case StructuralType.Timber:
                    return Parameter.Building.TimberStructureRecycle;
                default:
                    return Parameter.Building.ConcreteStructureRecycle;
            }
        }

        #endregion

        #region Trip

        public List<ComputationUnit> SplitAllMixedUnits()
        {
            List<ComputationUnit> resultList = new List<ComputationUnit>();

            UpdateAllUnits();
            foreach (var unit in AllUnits)
            {
                if (unit.Type == BuildingType.Mixed)
                {
                    if (unit.MixOfficePercent != 0.0)
                    {
                        ComputationUnit tmp = new ComputationUnit(unit);
                        tmp.Type = BuildingType.Office;
                        tmp.Area = unit.Area * unit.MixOfficePercent;
                        tmp.UpdatePopulation();
                        resultList.Add(tmp);
                    }
                    if (unit.MixIndustryPercent != 0.0)
                    {
                        ComputationUnit tmp = new ComputationUnit(unit);
                        tmp.Type = BuildingType.Industry;
                        tmp.Area = unit.Area * unit.MixIndustryPercent;
                        tmp.UpdatePopulation();
                        resultList.Add(tmp);
                    }
                    if (unit.MixEducationPercent != 0.0)
                    {
                        ComputationUnit tmp = new ComputationUnit(unit);
                        tmp.Type = BuildingType.Education;
                        tmp.Area = unit.Area * unit.MixEducationPercent;
                        tmp.UpdatePopulation();
                        resultList.Add(tmp);
                    }
                    if (unit.MixHotelPercent != 0.0)
                    {
                        ComputationUnit tmp = new ComputationUnit(unit);
                        tmp.Type = BuildingType.Hotel;
                        tmp.Area = unit.Area * unit.MixHotelPercent;
                        tmp.UpdatePopulation();
                        resultList.Add(tmp);
                    }
                    if (unit.MixGreenFieldPercent != 0.0)
                    {
                        ComputationUnit tmp = new ComputationUnit(unit);
                        tmp.Type = BuildingType.GreenField;
                        tmp.Area = unit.Area * unit.MixGreenFieldPercent;
                        tmp.UpdatePopulation();
                        resultList.Add(tmp);
                    }
                    if (unit.MixOtherPercent != 0.0)
                    {
                        ComputationUnit tmp = new ComputationUnit(unit);
                        tmp.Type = BuildingType.Other;
                        tmp.Area = unit.Area * unit.MixOtherPercent;
                        tmp.UpdatePopulation();
                        resultList.Add(tmp);
                    }
                    if (unit.MixRetailPercent != 0.0)
                    {
                        ComputationUnit tmp = new ComputationUnit(unit);
                        tmp.Type = BuildingType.Retail;
                        tmp.Area = unit.Area * unit.MixRetailPercent;
                        tmp.UpdatePopulation();
                        resultList.Add(tmp);
                    }
                    if (unit.MixResidencialPercent != 0.0)
                    {
                        ComputationUnit tmp = new ComputationUnit(unit);
                        tmp.Type = BuildingType.Residencial;
                        tmp.Area = unit.Area * unit.MixResidencialPercent;
                        tmp.UpdatePopulation();
                        resultList.Add(tmp);
                    }
                }
                else
                {
                    resultList.Add(unit);
                }
            }

            return resultList;
        }

        public TripResult GetTripCarbonProduction()
        {
            TripResult result = new TripResult();

            List<ComputationUnit> AllMixedUnits = SplitAllMixedUnits();
            //UpdateAllUnits();
            var homes = AllMixedUnits.Where(x => x.IsHome()).ToList();
            var offices = AllMixedUnits.Where(x => x.IsOffice()).ToList();
            var schools = AllMixedUnits.Where(x => x.IsSchool()).ToList();
            var entertainments = AllMixedUnits.Where(f => f.IsEntertainment()).ToList();
            double residentNumbers = homes.Sum(f => f.Populations);
            double jobCapacity = offices.Sum(f => f.Populations);
            double adultRate = Parameter.Trip.AdultRate;

            // 1. 外部通勤

            double inOutPeople = Math.Abs(residentNumbers * adultRate - jobCapacity);
            result.OutWork = GetOutsideCarbonProductionOfGivenPeople(inOutPeople);
            double outPeople = Math.Max(0, residentNumbers * adultRate - jobCapacity);

            foreach (var home in homes)
            {
                var pos = home.Position;
                double pop = home.Populations * adultRate - outPeople * home.Populations / residentNumbers;

                // 2. 内部通勤

                double[] dists2 = offices.Select(f => f.Position.EzDistTo(pos) / 1000).ToArray(); // mod 20130226 注意图上单位是m，需要转为km
                double[] pops2 = offices.Select(f => f.Populations).ToArray();
                double sumPops2 = pops2.Sum();
                double[] proportions2 = pops2.Select(p => p / sumPops2).ToArray();
                result.InWork += GetInsideCarbonProductionOfGivenRParcel(pop, dists2, proportions2, Parameter.Trip.TripFrequencyOfTransit);

                // 4. 其他活动
                pop = home.Populations;

                // 获得目的地集合
                List<double> prepareDistanceSet = entertainments.Select(f =>
                {
                    if (f.Area >= 200000)
                    {
                        return f.Position.EzDistTo(pos) / 1000;
                    }
                    return 0;
                }).Where(w => w != 0.0).ToList();

                // 选取随机目的地集合
                List<int> prepareIndexSet = prepareDistanceSet.Select(f => 0).ToList();
                if (prepareIndexSet.Count > 0)
                {
                    Random ra = new Random();
                    int maxPosition = prepareDistanceSet.Count;
                    for (int i = 0; i < Parameter.Trip.TripFrequencyOfOtherEvent; i++)
                    {
                        int position = ra.Next(0, maxPosition);
                        prepareIndexSet[position]++;
                    }

                    double[] dists4 = prepareDistanceSet.Select((f, i) =>
                    {
                        if (prepareIndexSet[i] != 0)
                        {
                            return f;
                        }
                        return 0;
                    }).Where(w => w != 0).ToArray();

                    List<int> frequency = prepareIndexSet.Where(w => w != 0).ToList();

                    // 计算碳排放
                    for (int i = 0; i < frequency.Count; i++)
                    {
                        result.Other += GetCarbonProductionOfGivenPeopleAndDistance(pop, dists4[i], frequency[i]);
                    }
                }

                //double[] dists4 = entertainments.Select(f =>
                //{
                //    if (f.Area >= 200000)
                //    {
                //        return f.Position.EzDistTo(pos) / 1000;
                //    }
                //    return 0;
                //}).Where(w => w != 0.0).ToArray();
                //double[] pops4 = dists4.Select(f => 1.0).ToArray();
                //double[] proportions4 = pops4.Select(p => p).ToArray();
                //result.Other += GetInsideCarbonProductionOfGivenRParcel(pop, dists4, proportions4, Parameter.Trip.TripFrequencyOfOtherEvent);
            }

            // 3. 上学
            result.School = GetSchoolCarbonProduction(homes, schools);

            return result;
        }

        private static double GetOutsideCarbonProductionOfGivenPeople(double people)
        {
            return TripDistanceInterval.All.Sum(
                interv => GetCarbonProductionOfGivenPeopleAndDistance(
                    people * Parameter.Trip.TripDistanceIntervalProportions[interv],
                    Parameter.Trip.TripDistanceIntervalAverageDistances[interv],
                    Parameter.Trip.TripFrequencyOfTransit
                )
            );
        }

        private static double GetInsideCarbonProductionOfGivenRParcel(double people, double[] dists, double[] proportions, double frequecy)
        {
            return Enumerable.Range(0, dists.Length).Sum(
                i => GetCarbonProductionOfGivenPeopleAndDistance(
                    people * proportions[i],
                    dists[i],
                    frequecy
                )
            );
        }

        private static double GetCarbonProductionOfGivenPeopleAndDistance(double people, double dist, double frequecy)
        {
            int interv = Parameter.GetDistIntervalIndex(dist);
            return TripMethod.All.Sum(
                method => GetTripCarbonProduction(
                    people * Parameter.Trip.TripMethodProportions[interv][method],
                    dist,
                    Parameter.Trip.TripMethodUnitCarbonProductions[method],
                    frequecy
                )
            );
        }

        private static double GetTripCarbonProduction(double people, double distance, double unitProduction, double tripFrequecy)
        {
            return people * distance * unitProduction * tripFrequecy;
        }

        private static double GetSchoolCarbonProduction(List<ComputationUnit> homes, List<ComputationUnit> schools)
        {
            if (schools.Count == 0)
                return 0;

            // 初始化 schoolCollections
            Dictionary<ComputationUnit, List<ComputationUnit>> schoolCollections = new Dictionary<ComputationUnit, List<ComputationUnit>>();

            foreach (var school in schools)
            {
                schoolCollections.Add(school, new List<ComputationUnit>());
            }

            // 划分教学片区
            foreach (var home in homes)
            {
                ComputationUnit shool = GetNearestSchool(home, schools);
                List<ComputationUnit> schoolAreas = schoolCollections[shool];
                schoolAreas.Add(home);
            }

            // 分片区计算
            double sum = 0;
            foreach (var reagion in schoolCollections)
            {
                sum += GetOneReagionCarbonProduction(reagion.Key, reagion.Value);
            }

            return sum;
        }

        private static double GetOneReagionCarbonProduction(ComputationUnit school, List<ComputationUnit> homes)
        {
            double studentRate = Parameter.Trip.StudentRate;
            var pos = school.Position;

            double[] dists3 = homes.Select(f => f.Position.EzDistTo(pos) / 1000).ToArray();
            double[] pops3 = homes.Select(f => f.Populations).ToArray();
            double sumPops3 = pops3.Sum();
            double[] proportions3 = pops3.Select(p => p / sumPops3).ToArray();

            double result = GetInsideCarbonProductionOfGivenRParcel(sumPops3 * studentRate, dists3, proportions3, Parameter.Trip.TripFrequencyOfSchool);
            return result;
        }

        private static ComputationUnit GetNearestSchool(ComputationUnit home, List<ComputationUnit> schools)
        {
            ComputationUnit result = null;
            double minDistance = double.MaxValue;
            foreach (var school in schools)
            {
                double distance = home.Position.Dist(school.Position);
                if (distance < minDistance)
                {
                    result = school;
                }
            }

            return result;
        }

        #endregion
    }
}
