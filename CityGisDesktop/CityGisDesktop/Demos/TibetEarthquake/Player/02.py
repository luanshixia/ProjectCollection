sd.layer('地震点')
sd.prop('里氏震级')
sd.useenumsize()
sd.newenumsize()
sd.enumsize('8.0级以上',50)
sd.enumsize('7.0-7.9级',30)
sd.enumsize('6.0-6.9级',10)
sd.applysize()