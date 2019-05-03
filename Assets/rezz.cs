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

public class RezolveFile {
    Rezolve rez;
    RezFactory rezf;
    Action<Exception> exceptf;
    bool valid = false;
    bool started = false;
    bool todestroy = false;
    bool glitched = false;
    
    public RezolveFile(Assembly assm, String name, Action<Exception> __exceptf) {
        exceptf = __exceptf;
        try {
            rez = assm.CreateInstance(name) as Rezolve;
            rezf = new RezFactory();
            rez.rezFactory = rezf;
            valid = true;
        }
        catch(Exception exc) {
            destroy();
            __exceptf(exc);
        }
    }
    
    public void destroy() {
        if(rezf != null) {
            todestroy = true;
        }
        rez = null;
        valid = false;
    }
    
    public bool is_it_valid() {
        return valid;
    }
    
    void safetyrun(Action act) {
        if(!valid)
            return;
        try {
            act.Invoke();
        }
        catch(Exception exc) {
            destroy();
            exceptf(exc);
        }
    }
    
    public void update() {
        if(!valid) {
            if(todestroy) {
                rezf.DestroyEverything();
                rezf = null;
                todestroy = false;
            }
            return;
        }
        if(glitched)
            return;
        if(!started) {
            started = true;
            safetyrun(rez.start);
        }
        safetyrun(rez.update);
    }
    
    public void glitch() {
        if(rezf != null) rezf.Glitch();
        glitched = true;
    }
};      

public class rezz : MonoBehaviour
{
    [System.NonSerialized]
    System.IO.FileSystemWatcher dirwat;
    [System.NonSerialized]
    Dictionary<string, RezolveFile> rezolve;
    [System.NonSerialized]
    string path;
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
    
    [System.NonSerialized]
    bool compiled_anything = false;
    
    static rezz _rezz_instance = null;
    
    public static void Log(string st) {
        if(_rezz_instance != null)
            _rezz_instance.log(st);
    }
    
	void Start()
	{
        path = System.IO.Path.Combine(Application.streamingAssetsPath, "rezolve");
        path = System.IO.Path.GetFullPath(path);
        queue = new System.Collections.Generic.Queue<Action>();
        queuelock = new object();
        dirwat = new System.IO.FileSystemWatcher();
        dirwat.Path = path;
        dirwat.NotifyFilter = System.IO.NotifyFilters.LastWrite;
        dirwat.Filter = "*.cs";
        dirwat.Created += onchanged;
        dirwat.Changed += onchanged;
        dirwat.EnableRaisingEvents = true;
        dirwat.IncludeSubdirectories = true;
        
        make_log_queue();
        
        _rezz_instance = this;
        
        reload();
	}
    
    void Update()
    {
        Action thejam = null;
        bool not_empty = true;
        while(not_empty) {
            lock(queuelock) {
                if(queue.Count == 0)
                    not_empty = false;
                else
                    thejam = queue.Dequeue();
            }
            if(thejam != null) {
                thejam.Invoke();
                thejam = null;
            }
        }
        write_out_log_queue();
        foreach(KeyValuePair<string, RezolveFile> rez in rezolve) {
            rez.Value.update();
        }
    }
    
    string nameof(string file, bool backtrack = false)
    {
        if(backtrack) {
            var fi = new System.IO.FileInfo(file);
            int limit = 10;
            while(fi.DirectoryName != path) {
                if(--limit == 0)
                    throw new Exception("lost track of path while compiling");
                file = fi.DirectoryName;
                fi = new System.IO.FileInfo(file);
            }
        }
        return System.IO.Path.GetFileNameWithoutExtension(file);
    }
    
    void onchanged(object source, System.IO.FileSystemEventArgs e)
    {
        log(String.Format("modified {0}", e.Name));
        var name = nameof(e.FullPath, true);
        log(String.Format("Recompiling {0}", name));
        if(rezolve.ContainsKey(name)) {
            Action thejam = delegate() { glitch_object(name); };
            lock(queuelock) {
                queue.Enqueue(thejam);
            }
        }
        Compile(name, e.FullPath);
    }
    
    void reload()
    {
        if(rezolve == null)
            rezolve = new Dictionary<string, RezolveFile>();
        var watch = System.Diagnostics.Stopwatch.StartNew();
        Func<string, string, string[]>[] methods = {
            System.IO.Directory.GetFiles,
            System.IO.Directory.GetDirectories
        };
        compiled_anything = false;
        foreach(var method in methods) {
            foreach(var file in method(path, "*.cs")) {
                var name = nameof(file, false);
                Compile(name, file);
            }
        }
        lock(queuelock) {
            queue.Enqueue(delegate() {
                watch.Stop();
                var t = watch.ElapsedMilliseconds * 0.001f;
                log(String.Format("{0} took {1} seconds.", compiled_anything? "Compilation": "Loading",
                    t));
            });
        }
    }
    
    string outfile(string name, bool randomname = false)
    {
		if(randomname)
			name += "-" + System.Guid.NewGuid().ToString();
        return System.IO.Path.Combine(path, name + ".dll");
    }
    
    string[] AllSourceFiles(string fp)
    {
        return System.IO.Directory.GetFiles(
            fp,
            "*.cs",
            System.IO.SearchOption.AllDirectories);
    }
    
    static bool IsDirectory(System.IO.FileInfo f)
    {
        return (f.Attributes & System.IO.FileAttributes.Directory) != 0;
    }
    
    static bool IsDirectory(string fp)
    {
        return IsDirectory(new System.IO.FileInfo(fp));
    }
    
    bool CompareLastWriteTime(System.IO.FileInfo f, 
                              DateTime d)
    {
        if(f.LastWriteTimeUtc > d)
            return false;
        if(IsDirectory(f)) {
            foreach(var file in AllSourceFiles(f.FullName))
                if(new System.IO.FileInfo(file).LastWriteTimeUtc > d)
                    return false;
        }
        return true;
    }
                
    
    void Compile(String name, string file)
    {
        var source = new System.IO.FileInfo(file);
        string ouf = outfile(name, false);
        var assembly = new System.IO.FileInfo(ouf);
        if(assembly.Exists && CompareLastWriteTime(source, assembly.LastWriteTimeUtc)) {
            // take it easy
        }
        else {
            compiled_anything = true;
            log(String.Format("compiling {0}", ouf));
            var result = ReallyCompile(name, file);
            if(result == null) return;
        }
        
        var assm = System.Reflection.Assembly.Load(
            System.IO.File.ReadAllBytes(ouf));

        var rez = new RezolveFile(assm, name, except);
        
        if(rez.is_it_valid()) {
            Action thejam = delegate() {
                start_object(name, rez);
            };
            lock(queuelock) {
                queue.Enqueue(thejam);
            }
        }
    }

	System.CodeDom.Compiler.CompilerResults ReallyCompile(string name, string file)
	{
		var provider = new CSharpCodeProvider();
		var param = new CompilerParameters();
		
		/* Compile the assembly with a different random
		 * name each time to defeat the cache. */
		string outfile_stat = outfile(name, false);
		string outfile_guid = outfile(name, true);

		foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
		{
			param.ReferencedAssemblies.Add(assembly.Location);
		}

		param.GenerateExecutable = false;
		param.GenerateInMemory = false;
        param.OutputAssembly = outfile_guid;
        
        System.CodeDom.Compiler.CompilerResults result;
        
        if(IsDirectory(file)) {
            string[] what = AllSourceFiles(file);
            log(String.Format("compiling a batch {0}", what));
            result = provider.CompileAssemblyFromFile(param, AllSourceFiles(file));
        }
        else
            result = provider.CompileAssemblyFromFile(param, file);

		if (result.Errors.Count > 0) {
			var msg = new StringBuilder();
			foreach (CompilerError error in result.Errors) {
                msg.AppendFormat("{0} ({1}:{2}): {3}\n",
                    error.IsWarning? "Warning": "Error",
                    error.Line,
                    error.Column,
                    error.ErrorText
                    );
			}
            compileexcept(msg.ToString());
		}
        if(result.NativeCompilerReturnValue != 0) {
            return null;
        }
        
        /* If it worked, then move the assembly from
         * dogs-(guid).dll to dogs.dll.
         *
         * The result is that we have an assembly with
         * the simple name dogs-(guid).dll, but it's in the
         * file dogs.dll, so we know where to find it.
         *
         * We are going to use Load(ReadAllBytes()), so
         * the loader won't notice anything wrong.
         *
         * BEWARE: .NET does not unload assemblies unless
         * you hide them in another AppDomain, which I didn't
         * do because it is hard and would probably break
         * everything. So, all of the "destroyed" assemblies
         * hang around in memory until you close the program,
         * at a cost of probably a few kilobytes each?
         */
        System.IO.File.Delete(outfile_stat);
        System.IO.File.Move(outfile_guid, outfile_stat);
        
        return result;
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
        while(true) {
            lock(errorlock) {
                if(log_queue.Count == 0)
                    break;
                else
                    towrite = log_queue.Dequeue();
            }
            log_write.WriteLine(towrite);
            flush = true;
        }
        if(flush)
            log_write.Flush();
    }
    
    void log(string mesg) {
        lock(errorlock) {
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
    
    void glitch_object(string name)
    {
        rezolve[name].glitch();
    }
    
    void start_object(string name, RezolveFile rez)
    {
        if(rezolve.ContainsKey(name)) {
            rezolve[name].destroy();
            rezolve[name].update();
        }
        rezolve[name] = rez;
    }
    
    public void reboot()
    {
        string[] names = rezolve.Keys.ToArray();
        foreach(var s in names) {
            rezolve[s].destroy();
            rezolve[s].update();
        }
        reload();
    }
}