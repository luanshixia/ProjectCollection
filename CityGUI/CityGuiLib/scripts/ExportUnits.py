sw = StreamWriter('C:\\test.txt')
for unit in values:
    sw.WriteLine(unit.Value)
sw.Close()