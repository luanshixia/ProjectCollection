brIds = QuickSelection.SelectAll('INSERT')
fileName = Application.DocumentManager.MdiActiveDocument.Name + '.elevations.txt';
sw = System.IO.StreamWriter(fileName)
try :
    for i in range(0, brIds.Length) :
        br = QuickSelection.QOpenForRead(brIds[i])
        attrs = DbHelper.GetBlockAttributes(br)
        if attrs.ContainsKey('height') :
            x = br.Position.X
            y = br.Position.Y
            z = attrs['height']
            sw.WriteLine('{0} {1} {2}', x, y, z)
finally :
    sw.Close()