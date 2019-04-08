import csv
import math

X_FIELD, Y_FIELD, Z_FIELD = 17, 18, 19

f = open("star.csv", "r")
rr = csv.reader(f, delimiter=',')
next(rr)
distance = []
for r in rr:
    x, y, z = map(float, (r[X_FIELD], r[Y_FIELD], r[Z_FIELD]))
    distance.append(math.sqrt(x*x + y*y + z*z))
