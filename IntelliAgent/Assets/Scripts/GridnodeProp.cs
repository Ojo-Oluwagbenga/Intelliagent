using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridnodeProp : MonoBehaviour{
    public bool closed = false;    
    public bool opened = false;
    public int x = 0;
    public int y = 0;
    public int z = 0;
    public int FS = 0;
    public int TT = 0;
    public int D = -1;
    public GameObject gameobj;

    public GameObject parent;

   


    public int setCosts(int FS, int TT, GameObject parent){
        if (this.D > FS+TT || this.D == -1){
            this.FS = FS;
            this.TT = TT;
            this.D = FS+TT;
            this.parent = parent;
            this.opened = true;                                    
        }
        
        

        return this.D;
    } 

    
    public void clearPaint(){
        this.gameobj.GetComponent<Renderer>().enabled = false;
    }
    public void addPaint(){
        this.gameobj.GetComponent<Renderer>().enabled = true;
    }

    public void reset(){
        this.FS = 0;
        this.TT = 0;
        this.D = -1;
        this.parent = null;
        this.opened = false;
        this.closed = false;
    }
}
