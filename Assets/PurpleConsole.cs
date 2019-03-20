using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameConsole {
    [System.NonSerialized]
    public static PurpleConsole _theconsole = null;
    
    public static void set(PurpleConsole z) {
        _theconsole = z;
    }
    
    public static void clear(PurpleConsole z) {
        _theconsole = null;
    }
    
    public static void add(string zz) {
        if(_theconsole)
            _theconsole.add(zz);
    }
}

class Ring<T> {
    T[] buffer;
    int head;
    int tail;
    
    public Ring(int len) {
        buffer = new T[len];
        head = 0;
        tail = 0;
    }
    
    public void add(T z) { 
        buffer[tail % buffer.Length] = z;
        if(tail - head == buffer.Length)
            head += 1;
        tail += 1;
    }
    
    public T[] get() {
        T[] z = new T[tail - head];
        for(int i = 0; i < tail - head; i++)
            z[i] = buffer[(head + i) % buffer.Length];
        return z;
    }
};

public class PurpleConsole : MonoBehaviour
{
    public float blinkTime = 1.0f;
    public string cursorChar = "█";
    
    Ring<string> console;
    string line = "";
    float blinkt = 0;
    bool shown = true;
    
    public GameObject InputLine;
    
    // Start is called before the first frame update
    void Start()
    {
        reboot();
    }
    
    void reboot()
    {
        console = new Ring<string>(24);
        GameConsole.set(this);
    }
    
    public void add(string zz) {
        console.add(zz);
    }
    
    void update_console() {
        if(console == null)
            reboot();
        var t = GetComponent<Text>();
        if(console != null && t != null)
            t.text = System.String.Join("\n", console.get());
    }
    
    void updateline() {
        InputLine.GetComponent<Text>().text = line + (shown? cursorChar: "");
    }

    // Update is called once per frame
    void Update()
    {
        bool updated = false;
        foreach(char x in Input.inputString)
        {
            updated = true;
            if(x == '\b') {
                if(line.Length > 0) {
                    line = line.Substring(0, line.Length - 1);
                }
            }
            else if(x == '\n' || x == '\r') {
                console.add(line);
                line = "";
            }
            else {
                line += x;
            }
        }
        blinkt += Time.deltaTime;
        if(blinkt > blinkTime) {
            blinkt -= blinkTime;
            shown = !shown;
            updated = true;
        }
        if(updated) {
            updateline();
            update_console();
        }
    }
}
