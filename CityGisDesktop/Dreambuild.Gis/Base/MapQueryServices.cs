using Dreambuild.Collections;
using Dreambuild.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dreambuild.Gis
{
    /// <summary>
    /// Types of data query operations
    /// </summary>
    public enum DataQueryOperation
    {
        Equals,
        NotEquals,
        GreaterThan,
        LessThan
    }

    /// <summary>
    /// Types of spatial query operations
    /// </summary>
    public enum SpatialQueryOperation
    {
        Point,
        Window,
        Cross
    }

    /// <summary>
    /// Map query services
    /// </summary>
    public static class MapQueryServices
    {
        public static PairList<ILayer, IFeature> QueryFeatures(this Map map, SpatialQueryOperation operation, object param, double tol)
        {
            return map.Layers.CreatePairList(l => l, l => l.QueryFeatures(operation, param, tol));
        }

        public static IEnumerable<IFeature> QueryFeatures(this ILayer layer, SpatialQueryOperation operation, object param, double tol)
        {
            if (operation == SpatialQueryOperation.Point)
            {
                Vector pos = (Vector)param;
                return PointQuery(layer, pos, tol);
            }
            else if (operation == SpatialQueryOperation.Window)
            {
                Extents extents = (Extents)param;
                return WindowQuery(layer, extents);
            }
            else if (operation == SpatialQueryOperation.Cross)
            {
                Extents extents = (Extents)param;
                return CrossQuery(layer, extents);
            }
            return null;
        }

        private static IEnumerable<IFeature> PointQuery(ILayer layer, Vector pos, double tol)
        {
            if (layer.GeoType == VectorLayer.GEOTYPE_POINT)
            {
                foreach (var f in layer.Features)
                {
                    var p = f.GeoData[0];
                    if (p.Dist(pos) < tol)
                    {
                        yield return f;
                    }
                }
            }
            else if (layer.GeoType == VectorLayer.GEOTYPE_LINEAR)
            {
                foreach (var f in layer.Features)
                {
                    var poly = new PointString(f.GeoData);
                    if (poly.DistToPoint(pos) < tol)
                    {
                        yield return f;
                    }
                }
            }
            else if (layer.GeoType == VectorLayer.GEOTYPE_REGION)
            {
                foreach (var f in layer.Features)
                {
                    var poly = new PointString(f.GeoData);
                    if (poly.IsPointIn(pos))
                    {
                        yield return f;
                    }
                }
            }
        }

        private static IEnumerable<IFeature> WindowQuery(ILayer layer, Extents extents)
        {
            if (layer.GeoType == VectorLayer.GEOTYPE_POINT)
            {
                foreach (var f in layer.Features)
                {
                    var p = f.GeoData[0];
                    if (extents.IsPointIn(p))
                    {
                        yield return f;
                    }
                }
            }
            else if (layer.GeoType == VectorLayer.GEOTYPE_LINEAR)
            {
                foreach (var f in layer.Features)
                {
                    var poly = new PointString(f.GeoData);
                    if (extents.IsExtentsIn(poly.GetExtents()))
                    {
                        yield return f;
                    }
                }
            }
            else if (layer.GeoType == VectorLayer.GEOTYPE_REGION)
            {
                foreach (var f in layer.Features)
                {
                    var poly = new PointString(f.GeoData);
                    if (extents.IsExtentsIn(poly.GetExtents()))
                    {
                        yield return f;
                    }
                }
            }
        }

        private static IEnumerable<IFeature> CrossQuery(ILayer layer, Extents extents)
        {
            if (layer.GeoType == VectorLayer.GEOTYPE_POINT)
            {
                foreach (var f in layer.Features)
                {
                    var p = f.GeoData[0];
                    if (extents.IsPointIn(p))
                    {
                        yield return f;
                    }
                }
            }
            else if (layer.GeoType == VectorLayer.GEOTYPE_LINEAR)
            {
                foreach (var f in layer.Features)
                {
                    var poly = new PointString(f.GeoData);
                    foreach (var point in poly.Points)
                    {
                        if (extents.IsPointIn(point))
                        {
                            yield return f;
                        }
                    }
                }
            }
            else if (layer.GeoType == VectorLayer.GEOTYPE_REGION)
            {
                foreach (var f in layer.Features)
                {
                    var poly = new PointString(f.GeoData);
                    foreach (var point in poly.Points)
                    {
                        if (extents.IsPointIn(point))
                        {
                            yield return f;
                        }
                    }
                }
            }
        }

        public static IEnumerable<IFeature> QueryFeatures(this ILayer layer, string prop, DataQueryOperation operation, object param)
        {
            return layer.Features.Where(f => FeatureSelector(f, prop, operation, param)).ToList();
        }

        public static IEnumerable<IFeature> FindFeatures(this Map map, string searchWord)
        {
            foreach (var layer in map.Layers)
            {
                foreach (var feature in layer.Features)
                {
                    if (FeatureSelector(feature, searchWord))
                    {
                        yield return feature;
                    }
                }
            }
        }

        public static bool FeatureSelector(IFeature feature, string searchWord)
        {
            return feature.FeatId.ToUpper().Contains(searchWord.ToUpper()) || feature.Properties.Any(x => x.Value.ToUpper().Contains(searchWord.ToUpper()));
        }

        public static bool FeatureSelector(IFeature feature, string prop, DataQueryOperation operation, object param)
        {
            if (operation == DataQueryOperation.Equals)
            {
                return feature.Properties.ContainsKey(prop) && ((param.IsNumber() && feature[prop].TryParseToDouble() == Convert.ToDouble(param)) || (feature[prop] == param.ToString()));
            }
            else if (operation == DataQueryOperation.NotEquals)
            {
                return feature.Properties.ContainsKey(prop) && ((param.IsNumber() && feature[prop].TryParseToDouble() != Convert.ToDouble(param)) || (feature[prop] != param.ToString()));
            }
            else if (operation == DataQueryOperation.GreaterThan)
            {
                return feature.Properties.ContainsKey(prop) && (param.IsNumber() && feature[prop].TryParseToDouble() > Convert.ToDouble(param));
            }
            else if (operation == DataQueryOperation.LessThan)
            {
                return feature.Properties.ContainsKey(prop) && (param.IsNumber() && feature[prop].TryParseToDouble() < Convert.ToDouble(param));
            }
            return false;
        }

        private static bool IsNumber(this object param)
        {
            double num;
            return param is int || param is double || param is decimal || param is float || double.TryParse(param.ToString(), out num);
        }
    }
}
