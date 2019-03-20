using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;

public class star {
    const int BAYER_FLAMSTEED_FIELD = 5;
    const int PROPER_FIELD = 6;
    const int X_FIELD = 17;
    const int Y_FIELD = 18;
    const int Z_FIELD = 19;
    const int ABSMAG_FIELD = 14;
    const int SPECTRAL_FIELD = 15;
    const int COLOUR_INDEX_FIELD = 16;
    const int CONSTELLATION_FIELD = 29;
    const int NFILEFIELDS = 37;
    public string bf, proper, spect, constellation; // Bayer/Flamsteed designation
    public float x, y, z, absmag, ci;
    public UInt32 id;
    public static star makestar(BinaryReader bs, Func<UInt32, UInt32, string> getstring) {
        var star = new star {
            id = bs.ReadUInt32(),
            x = bs.ReadSingle(),
            y = bs.ReadSingle(),
            z = bs.ReadSingle(),
            absmag = bs.ReadSingle(),
            ci = bs.ReadSingle()
        };
        star.bf = getstring(bs.ReadUInt32(), bs.ReadUInt32());
        star.proper = getstring(bs.ReadUInt32(), bs.ReadUInt32());
        star.spect = getstring(bs.ReadUInt32(), bs.ReadUInt32());
        star.constellation = getstring(bs.ReadUInt32(), bs.ReadUInt32());
        return star;
    }
    public bool part_of_group {
        get { return (id & 0x80000000) != 0; }
    }
    /*
    public star(string s) {
        string[] fields = s.Split(',');
        if(fields.Length != NFILEFIELDS)
            throw new Exception("invalid file: wrong number of fields");
        bf = fields[BAYER_FLAMSTEED_FIELD];
        proper = fields[PROPER_FIELD];
        x = parsenan(fields[X_FIELD]);
        y = parsenan(fields[Y_FIELD]);
        z = parsenan(fields[Z_FIELD]);
        absmag = parsenan(fields[ABSMAG_FIELD]);
        ci = parsenan(fields[COLOUR_INDEX_FIELD]);
        spect = fields[SPECTRAL_FIELD];
        constellation = fields[CONSTELLATION_FIELD];
    }
    */
    public static float parsenan(string s) {
        if(s == "")
            return float.NaN;
        else
            return float.Parse(s);
    }
    public Vector3 vec {
        get { return new Vector3(x, z, -y); }
    }
}

public class starmap {
    star[] stars;
    const string EXPECTED_LINE =
        "id,hip,hd,hr,gl,bf,proper,ra,dec,dist,pmra,pmdec,rv,mag,absmag,spect,ci,x,y,z,vx,vy,vz,rarad,decrad,pmrarad,pmdecrad,bayer,flam,con,comp,comp_primary,base,lum,var,var_min,var_max";
    public const int NFIELDS = 36;
    public const int NFILEFIELDS = 37;
    
    /*
    public void read_csv(string path) {
        var starlist = new System.Collections.Generic.List<star>();
        path = Path.Combine(path, "star\\star.csv");
        using(var sr = new StreamReader(new FileStream(path, System.IO.FileMode.Open))) {
            string line;
            line = sr.ReadLine();
            if(line != EXPECTED_LINE)
                throw new Exception("wrong first line");
            while((line = sr.ReadLine()) != null) {
                star s = new star(line);
                starlist.Add(s);
            }
        }
        stars = starlist.ToArray();
        Debug.Log(String.Format("{0} stars", starlist.Count));
    }
    */
    
    public void read_dat(string path) {
        var starlist = new System.Collections.Generic.List<star>();
        string path1 = Path.Combine(path, "star\\star.dat");
        string path2 = Path.Combine(path, "star\\star.string");
        FileStream fs = File.OpenRead(path2);
        var unicodeencode = new System.Text.UTF8Encoding() ;
        Func<UInt32, UInt32, string> getstring = delegate(UInt32 offset, UInt32 length) {
            fs.Seek(offset, SeekOrigin.Begin);
            byte[] str = new byte[length];
            fs.Read(str, 0, checked((int) length));
            return unicodeencode.GetString(str);
        };
        using(var br = new BinaryReader(new FileStream(path1, System.IO.FileMode.Open))) {
            try {
                while(true) {
                    star s = star.makestar(br, getstring);
                    starlist.Add(s);
                    GameDad.starprogress++;
                }
            }
            catch(EndOfStreamException) {}
        }
        stars = starlist.ToArray();
        Debug.Log(String.Format("{0} stars", starlist.Count));
    }
    
    public int nstars() {
        return stars.Length;
    }
    
    public star getstar(int i) {
        return stars[i];
    }
    
    public IEnumerable<star> starlist(int i)
    {
        bool first = true;
        while(0 <= i && i < nstars()) {
            star s = getstar(i);
            if(first && s.part_of_group)
                throw new Exception("first star is not the first of a group");
            if(!first && !s.part_of_group)
                break;
            first = false;
            yield return s;
            i++;
        }
    }
}

public class StarReader {
    string path;
    public StarReader(string _path) {
        path = _path;
    }
    public void read(object ob) {
        Debug.Log("running starreads");
        try {
            var manystars = new starmap();
            manystars.read_dat(path);
            GameDad.manystars = manystars;
            GameDad.starprogress = -1;
        }
        catch(System.Exception e) {
            Debug.Log(e.ToString());
        }
    }
}

public class starlistreader : MonoBehaviour {
    void Start() {
        var s = new StarReader(Application.streamingAssetsPath);
        System.Threading.ThreadPool.QueueUserWorkItem(s.read);
    }
    void Update() {
    }
};