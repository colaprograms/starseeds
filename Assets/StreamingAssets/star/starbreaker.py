"""Convert the CSV file formatted like hygdatav3.csv into a big list of
single-precision floating point numbers and a strings file."""

import math
import csv
import struct
HIPPARC_FIELD = 1
HD_FIELD = 2
BAYER_FLAMSTEED_FIELD = 5
PROPER_FIELD = 6
X_FIELD = 17
Y_FIELD = 18
Z_FIELD = 19
ABSMAG_FIELD = 14
SPECTRAL_FIELD = 15
COLOUR_INDEX_FIELD = 16
CONSTELLATION_FIELD = 29
NFILEFIELDS = 37

PART_OF_GROUP = 0x80000000

class outfile:
    "Keep track of both the data and strings file"
    def __init__(self):
        self.data_file = open("star.dat", mode="wb")
        self.strings_file = open("star.string", mode="wb")
        self.strings_offset = 0
    def addstring(self, string):
        encodedstring = string.encode("utf-8")
        ret = (self.strings_offset, len(encodedstring))
        if self.strings_offset != self.strings_file.tell():
            raise Exception("%d %d" %
                            (self.strings_offset,
                             self.strings_file.tell()))
        self.strings_file.write(encodedstring)
        self.strings_file.flush()
        self.strings_offset += len(encodedstring)
        return ret
    def output(self, group):
        "Output star group to the data files"
        first = True
        for star in group:
            bf_s, bf_l = self.addstring(star.bf)
            proper_s, proper_l = self.addstring(star.proper)
            spect_s, spect_l = self.addstring(star.spect)
            con_s, con_l = self.addstring(star.con)
            starid = star.id
            if not first:
                starid |= PART_OF_GROUP
            else:
                first = False
            self.data_file.write(struct.pack("IIIfffffIIIIIIII",
                starid,
                star.hip,
                star.hd,
                star.x,
                star.y,
                star.z,
                star.absmag,
                star.ci,
                bf_s, bf_l,
                proper_s, proper_l,
                spect_s, spect_l,
                con_s, con_l
            ))

class Union:
    "Implement some kind of union-find algorithm"
    def __init__(self):
        self.parent = {}
        self.object = {}
        self.ix = 0
    def add(self, ob):
        i = self.ix
        self.ix += 1
        self.object[ob] = i
        self.parent[i] = i
    def union(self, o1, o2):
        i = self.object[o1]
        j = self.object[o2]
        root_i = self.top(i)
        root_j = self.top(j)
        self.parent[root_i] = root_j
    def top(self, i):
        top = i
        while top != self.parent[top]:
            top = self.parent[top]
        cur = i
        while cur != top:
            next = self.parent[cur]
            self.parent[cur] = top
            cur = next
        return top
    def get_all_groups(self):
        objs = list(self.object.keys())
        grou = {}
        for o in objs:
            v = self.object[o]
            top = self.top(v)
            if top not in grou:
                grou[top] = []
            grou[top] += [o]
        return list(grou.values())

def roundstring(f, d):
    """If |f - f_| < 0.5, then the list [roundstring(f, i) for i in [-1, 1]]
       overlaps the list [roundstring(f_, i) for i in [-1, 1]]"""
    f = round(f)
    if d < 0:
        return "%.0f" % (f - 1)
    else:
        return "%.0f" % f

class IDs:
    def __init__(self):
        self.ix = 0
    def getid(self):
        self.ix += 1
        return self.ix

ids = IDs()

class Star:
    def __init__(self, r):
        try:
            self.id = int(r[0])
            self.hip = int(r[HIPPARC_FIELD]) if r[HIPPARC_FIELD] != "" else 0
            self.hd = int(r[HD_FIELD]) if r[HD_FIELD] != "" else 0
            self.bf = r[BAYER_FLAMSTEED_FIELD]
            self.proper = r[PROPER_FIELD]
            self.x = float(r[X_FIELD])
            self.y = float(r[Y_FIELD])
            self.z = float(r[Z_FIELD])
            self.absmag = float(r[ABSMAG_FIELD])
            self.spect = r[SPECTRAL_FIELD]
            if r[COLOUR_INDEX_FIELD] != "":
                self.ci = float(r[COLOUR_INDEX_FIELD])
            else:
                self.ci = 0.0 # whatever
            self.con = r[CONSTELLATION_FIELD]
        except ValueError:
            print("error", r)
            raise
    def strings(self):
        outs = []
        for dx in [-1, 1]:
            for dy in [-1, 1]:
                for dz in [-1, 1]:
                    outs += [",".join(roundstring(f, d) for (f, d) in
                                      [(self.x, dx),
                                       (self.y, dy),
                                       (self.z, dz)])]
        return outs
    def vector(self):
        return (self.x, self.y, self.z)

def dist(s1, s2):
    x1, y1, z1 = s1.vector()
    x2, y2, z2 = s2.vector()
    x = x2 - x1
    y = y2 - y1
    z = z2 - z1
    return math.sqrt(x*x + y*y + z*z)
        

class Pile:
    def __init__(self):
        self.pile = {}
        self.un = Union()
    def addto(self, st):
        self.un.add(st)
        for zz in st.strings():
            if zz not in self.pile:
                self.pile[zz] = []
            self.pile[zz].append(st)
    def run(self):
        for _, v in self.pile.items():
            for i in range(len(v)):
                for j in range(i + 1, len(v)):
                    if dist(v[i], v[j]) < 0.4:
                        self.un.union(v[i], v[j])
        return self.un.get_all_groups()

import math

def run():
    f = open("star.csv", "r")
    rr = csv.reader(f, delimiter=',')
    lines = 0
    o = outfile()
    next(rr) # eat first
    pile = Pile()
    for r in rr:
        st = Star(r)
        d = math.sqrt(st.x * st.x + st.y * st.y + st.z * st.z)
        if d > 100:
            continue
        lines += 1
        if lines % 10000 == 0: print("%d lines" % lines)
        pile.addto(st)
    print("%d lines" % lines)
    print("Collecting groups...")
    groups = pile.run()
    print("%d groups" % len(groups))
    count = 0
    print("Writing out the groups.")
    for v in groups:
        count += 1
        if count % 10000 == 0: print("%d groups" % count)
        o.output(v)
