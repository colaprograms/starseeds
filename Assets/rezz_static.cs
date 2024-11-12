/* the ReallyCompile routine is based on the code from http://www.arcturuscollective.com/archives/22 */

using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Reflection;
using System.Text;
using System.Collections.Generic;
using UnityEngine;

/* This class manages a bunch of scripts in the rezolve\ directory
 * that create game objects. When a script is modified, all its
 * game objects are deleted, it is recompiled, and then it starts
 * again and makes some more game objects.
 *
 * To make an object that is owned by a script, call RezFactory.Rez().
 * To delete it, call RezFactory.DeRez() or just crash.
 */

/*
public class RezolveDirectory {
    public static Dictionary<string, Rezolve> directory =
        new Dictionary<string, Rezolve>();
}
*/

public class RezFactory {
    //string name = null;
    Dictionary<GameObject, bool> registered;
    public RezFactory() {
        registered = new Dictionary<GameObject, bool>();
    }
    public GameObject Rez(GameObject template)
    {
        GameObject newObject = UnityEngine.Object.Instantiate(template);
        if(registered == null)
            throw new System.Exception("tried to rez after we destroyed everything");
        if(registered.ContainsKey(newObject))
            throw new System.Exception("the dictionary is broken");
        registered[newObject] = true;
        //Debug.Log(String.Format("creating {0}", newObject));
        return newObject;
    }
    public void DeRez(GameObject obj)
    {
        if(!registered.ContainsKey(obj))
            throw new System.Exception("cannot derez: object was never rezed");
        registered.Remove(obj);
        //Debug.Log(String.Format("destroying {0}", obj));
        UnityEngine.Object.Destroy(obj);
    }
    /*
    public void SetName(string name, Rezolve ob)
    {
        if(RezolveDirectory.directory.ContainsKey(name))
            throw new System.Exception(name + " already in dictionary");
        RezolveDirectory.directory[name] = ob;
    }
    public void UnsetName()
    {
        if(name != null) {
            RezolveDirectory.directory.Remove(name);
            name = null;
        }
    }
    */
    public void DestroyEverything()
    {
        //UnsetName();
        if(registered == null) {
            return;
        }
        foreach(KeyValuePair<GameObject, bool> badObject in registered) {
            //Debug.Log(String.Format("destroying {0}", badObject.Key));
            UnityEngine.Object.Destroy(badObject.Key);
        }
        registered = null;
    }
    public void Glitch()
    {
        var glitch = Resources.Load("glitchmaterial") as Material;
        foreach(KeyValuePair<GameObject, bool> badObject in registered) {
            var rend = badObject.Key.GetComponent<Renderer>();
            rend.material = glitch;
        }
    }
}

public class Rezolve {
    public RezFactory rezFactory;
    public GameObject Rez(GameObject template) {
        return rezFactory.Rez(template);
    }
    public void DeRez(GameObject obj) {
        rezFactory.DeRez(obj);
    }
    public GameObject Find(string name) {
        return GameObject.Find("rezolve").transform.Find(name).gameObject;
    }
    public GameObject RezFind(string name) {
        return Rez(Find(name));
    }
    /*
    public void SetName(string name = null) {
        if(name == null)
            name = this.GetType().Name;
        Debug.Log("registering " + name);
        rezFactory.SetName(name, this);
    }
    public T Lookup<T>() where T:class {
        return RezolveDirectory.directory[typeof(T).Name] as T;
    }
    */
    
    public virtual void start() {}
    public virtual void update() {}
}

public class rezz_static : MonoBehaviour
{
    public List<Rezolve> rezolve;

    [System.NonSerialized]
    System.Collections.Generic.Queue<Action> queue;
    
    
    [System.NonSerialized]
    object queuelock;

    [System.NonSerialized]
    System.Collections.Generic.Queue<string> log_queue;


    [System.NonSerialized]
    object errorlock;


    [System.NonSerialized]
    System.IO.StreamWriter log_write;

    static rezz_static _rezz_instance = null;

    [System.NonSerialized]
    RezFactory rezFactory;

    public static void Log(string st)
    {
        if (_rezz_instance != null)
            _rezz_instance.log(st);
        else
            Debug.Log(st);
    }

    void Start()
    {
        queue = new System.Collections.Generic.Queue<Action>();
        queuelock = new object();
        
        make_log_queue();

        _rezz_instance = this;

        rezFactory = new RezFactory();

        rezolve = new List<Rezolve>{
            new annulu(),
            new best(),
            new config(),
            new glitchanimator(),
            new launch(),
            new linedrawer(),
            new particlescaler(),
            new redstar(),
            new starinstantiator(),
            new starlined()
        };

        foreach(Rezolve rez in rezolve)
        {
            Debug.Log(String.Format("starting {0}", rez));
            rez.rezFactory = rezFactory;
            rez.start();
        }
    }

    void Update()
    {
        if (rezolve == null)
            return;

        Action thejam = null;
        bool not_empty = true;
        while (not_empty)
        {
            lock (queuelock)
            {
                if (queue.Count == 0)
                    not_empty = false;
                else
                    thejam = queue.Dequeue();
            }
            if (thejam != null)
            {
                thejam.Invoke();
                thejam = null;
            }
        }
        
        write_out_log_queue();
        
        foreach (Rezolve rez in rezolve)
        {
            rez.update();
        }
    }

    void make_log_queue()
    {
        string path = System.IO.Path.Combine(System.IO.Path.GetFullPath("."), "run.log");
        Debug.Log("rezolve log is " + path);
        log_write = new System.IO.StreamWriter(
            path);
        log_queue = new System.Collections.Generic.Queue<string>();
        errorlock = new object();
    }

    void write_out_log_queue()
    {
        string towrite = "";
        bool flush = false;
        while (true)
        {
            lock (errorlock)
            {
                if (log_queue.Count == 0)
                    break;
                else
                    towrite = log_queue.Dequeue();
            }
            log_write.WriteLine(towrite);
            flush = true;
        }
        if (flush)
            log_write.Flush();
    }

    void log(string mesg)
    {
        lock (errorlock)
        {
            log_queue.Enqueue(mesg);
        }
        Debug.Log(mesg);
    }

    void except(Exception e)
    {
        log(e.ToString());
    }

    void compileexcept(string probs)
    {
        log(probs);
    }

    public void reboot()
    {
        
    }
}
