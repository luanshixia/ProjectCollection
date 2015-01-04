units = pycmd.query(200, 500)
sb = System.Text.StringBuilder()
for unit in units:
    sb.AppendFormat('{0} - {1:0.00}\n', unit.Position, unit.Value)
MessageBox.Show(sb.ToString())