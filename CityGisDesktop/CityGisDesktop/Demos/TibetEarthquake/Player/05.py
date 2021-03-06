import Dreambuild.Geomety as geo

spots = dm.LatestMap.Layers['地震点'].Features
regions = dm.LatestMap.Layers['区域'].Features
totalArea = linq.ready(regions).sum(lambda x : x.Area)

def calcexpr1(f):
    poly = geo.Polygon(f.GeoData)
    i = 0
    for spot in spots:
        point = geo.Point2D(spot.GeoData)
        if poly.IsPointIn(point):
            i += 1
    return i

def calcexpr2(f):
    poly = geo.Polygon(f.GeoData)
    i = 0
    for spot in spots:
        point = geo.Point2D(spot.GeoData)
        if poly.IsPointIn(point):
            i += 1
    return i / (f.Area / totalArea)

pycmd.calcfield('区域','全县地震点数',calcexpr1)
pycmd.calcfield('区域','地震点密度',calcexpr2)
sd.layer('区域')
sd.prop('全县地震点数')
sd.maxcolor(255,0,0)
sd.apply()