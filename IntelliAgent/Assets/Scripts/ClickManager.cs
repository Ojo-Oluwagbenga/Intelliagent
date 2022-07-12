using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using TMPro;


public class ClickManager : MonoBehaviour{
    // Start is called before the first frame update
    

    // Update is called once per frame


    public General pathfinder;
    
    private void Start() {        
        pathfinder = new General();
    }

    ArrayList movepack = new ArrayList(){
        new ArrayList(){0,0},
        new ArrayList(){0,0},
        new ArrayList(){0,0},
        new ArrayList(){0,0},
        new ArrayList(){0,0}
    };

    int onpack = 0;
    string pointtype= "";
    bool pointselect = false;
    


    
    bool startbig = false;
    float inc = 1;
    Transform delipack;
    void Update(){
        if (startbig){
            if (inc < 1.5){
                inc += 0.1f;
                delipack.localScale = new Vector3(inc-0.02f, inc, inc-0.02f);
            }else{
                startbig = false;
                inc = 1;
                delipack.parent.transform.parent = GameObject.Find("Presents").transform;
                delipack.parent.transform.localPosition = new Vector3(0,0,0);
                delipack = null;
            }
        }
        
        if (Input.GetMouseButtonDown(0)) {  
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);  
            RaycastHit hit;  
            if (Physics.Raycast(ray, out hit, 1000000f)) {
                string itemname = hit.transform.name; 
                if (!pointselect){                     
                    // Debug.Log(itemname);
                    if (itemname == "Showpanel"){
                        TogglePanel(1);
                    }
                    if (itemname == "HidePanelId"){
                        TogglePanel(0);
                    }  
                    if (itemname == "start" || itemname == "end"){
                        if (!isFetching){
                            string pname = hit.transform.parent.name;
                            onpack = Int32.Parse(pname)-1;
                            pointtype = itemname;
                            openForClick();
                        }
                    }   
                    if (itemname == "remove"){                        
                        int v = Int32.Parse(hit.transform.parent.name) - 1;
                        Debug.Log("REMOVE--" + v);
                        clearpack(v);
                    }
                    if (itemname == "INITIATE"){   

                        if (checkLocComplete()){ 
                            if (!isFetching){
                                isFetching = true;
                                Agent = GameObject.Find("Agent");
                                Agent.AddComponent<Move>();
                                journeycomp = false;
                                meaningfulJourn = -10;    
                                // toggleScreen(0);   
                                movepackdict = ArrayToDict(movepack);
                                        
                                initiateAgent("x|6-z|6");
                                
                            }
                        }
                    }
                    if (itemname == "Togglescreen"){   
                        if (!carmode) {
                            carmode = true;
                            toggleScreen(0);
                        }else{
                            carmode = false;
                            toggleScreen(1);
                        }
                        
                    }          
                }else{
                    if (pointtype == "start"){
                        ((ArrayList) movepack[onpack])[0] = itemname;
                    }else{
                        ((ArrayList) movepack[onpack])[1] = itemname;
                    }
                    closeForClick();
                    writeArray();
                                        
                }
            }  
        }         
    }

    bool isFetching = false;
    int meaningfulJourn = -10;
    string aPathGen = "";
    string retrievenode = "";
    string delivenode = "";
    Dictionary<int, object> movepackdict = new Dictionary<int, object>();
    GameObject Agent;

    bool carmode = false;
    Dictionary<int, object> ArrayToDict(ArrayList a){
        Dictionary <int, object> d= new Dictionary<int, object>();
        for (int i = 0; i < a.Count; i++){
            d.Add(i, a[i]);            
        }
        return d;
    }
    void toggleScreen(int i){
        Transform camhold = GameObject.Find("Camhold").transform;
        GameObject init = GameObject.Find("INITIATE");
        GameObject shp = GameObject.Find("Showpanel");
        if (i == 0){
            TogglePanel(0);
            GameObject ag = GameObject.Find("Agent");
           

            init.transform.localPosition  = new Vector3(10000, 0, 0);
            shp.transform.localPosition  = new Vector3(10000, 0, 0);

            camhold.parent = ag.transform;
            camhold.localPosition = new Vector3(0, 62.2f, 0);
            camhold.eulerAngles = new Vector3(4, ag.transform.eulerAngles.y , 0);
            carmode = true;
        }else{            
            carmode = false;
            camhold.parent = GameObject.Find("IntelliAgent").transform;
            camhold.localPosition = new Vector3(-412, 931, 395);
            camhold.eulerAngles = new Vector3(90, 0, 0);
            init.transform.localPosition  = new Vector3(-492, -261, 756);
            shp.transform.localPosition  = new Vector3(-492, -412, 756);
        }
        
        

    }

    bool checkLocComplete(){
        int mn = 0;
        for (int i = 0; i < movepack.Count; i++){
            ArrayList a = (ArrayList) movepack[i];
            string f = a[0] + "";
            string s = a[1] + "";
            
            if ( (f == "0" || s == "0")){
                if ((f != s)){
                    string Error = "There is an error/incompleteness in location " + (i+1) +"; ";
                    Debug.Log(Error);
                }
            }else{
                mn +=1;
            }           

        }
        if (mn == 0){
            Debug.Log("Kindly set locations before starting agent");
        }

        
        return mn > 0;
    }

    int jcount = 0;
    void initiateAgent(string aPath){

        
        ArrayList dists1 = new ArrayList();
        GameObject agentpos = GameObject.Find(aPath);//reconf;

        string Error = "";

        int mn = 0;
        for (int i = 0; i < movepack.Count; i++){
            ArrayList a = (ArrayList) movepack[i];
            string f = a[0] + "";
            string s = a[1] + "";
            
            if ( (f == "0" || s == "0")){
                if ((f != s)){
                    Error += "There is an error/incompleteness in location " + (i+1) +"; ";
                }
            }else{
                mn +=1;
                GameObject start = GameObject.Find(f);
                dists1.Add(pathfinder.findPath(agentpos, start));
            }           

        }
        if (meaningfulJourn == -10){
            meaningfulJourn = mn;
        }
        


        ArrayList dists = new ArrayList();
        Dictionary<int, ArrayList> pairstore = new Dictionary<int, ArrayList>();
        int psval = 0;
        // int c = 0;
        for (int i = 0; i < movepack.Count; i++){
            
            ArrayList a = (ArrayList) movepack[i];
            string f = a[0] + "";//first node 
            string s = a[1] + "";//second node
            if ( (f == "0" || s == "0") ){
                if ((f != s)){
                    Error += "There is an error/incompleteness in location " + (i+1) +"; ";
                }
            }else{
                
                GameObject start = GameObject.Find(f);
                GameObject end = GameObject.Find(s);
                dists.Add(pathfinder.findPath(start, end));
                pairstore.Add(psval, new ArrayList(){f, s, i});
                psval += 1;
            }
            

            
        }


        Debug.Log(Error);

        if (meaningfulJourn > 0){
            int si = getshortest(dists, dists1);// Gets the shortest, whose distance from agent plus distance from delivery is smallest; 
            aPathGen = pairstore[si][1] + "";

            int key = movepackdict.Keys.ToList()[(int)pairstore[si][2]];
            Debug.Log("Key"+ key);

            retrievenode = retrievenodepack(key);
            delivenode = "collector"+ (key);


            ArrayList pathConfig = blendMoves((ArrayList)dists1[si], (ArrayList)dists[si]);
            

            movepack.RemoveAt(si);
            movepackdict.Remove(key);

            meaningfulJourn -= 1;

            
            Agent.GetComponent<Move>().setParam(Agent, pathConfig);  
            Agent.GetComponent<Move>().clm = this;
            Agent.GetComponent<Move>().initiate();

        }else{
            isFetching = false;
            journeycomp = true;
            movepack = new ArrayList(){
                new ArrayList(){0,0},
                new ArrayList(){0,0},
                new ArrayList(){0,0},
                new ArrayList(){0,0},
                new ArrayList(){0,0}
            };
        }
        jcount += 1;
    }

    string retrievenodepack(int i){
        string insObj = "";
        if (i == 0){
            insObj = ("Teddybear");
        }
        if (i  == 1){
            insObj = ("boxPresent");
        }
        if (i == 2){
            insObj = ("Christmas-Ball");
        }
        if (i == 3){
            insObj = ("Teddy2");
        }
        if (i == 4){
            insObj = ("chris2");
        }
        return insObj;
    }

    bool journeycomp = false;
    public void doneFunc(){
        pathfinder.clearPaintedNodes();

        if (delipack != null){
            delipack.parent.transform.parent = GameObject.Find("Presents").transform;
            delipack.parent.transform.localPosition = new Vector3(0,0,0);
            delipack = null;
            startbig = false;
            inc = 1;
        }
        

        Transform item = GameObject.Find(retrievenode).transform;
        item.parent = GameObject.Find(delivenode).transform;
        item.localPosition = new Vector3(0, 10, 0);
        item.eulerAngles = new Vector3(0,0,0);
        delipack = item;
        startbig = true;

        if (meaningfulJourn > 0){
            initiateAgent(aPathGen);
        }else{
            if (!journeycomp){
                journeycomp = true;
                GameObject start = GameObject.Find(aPathGen);
                GameObject end = GameObject.Find("x|6-z|6");

                ArrayList pathConfig = pathfinder.findPath(start, end);
                pathConfig.Reverse();
                Agent.GetComponent<Move>().setParam(Agent, pathConfig);  
                Agent.GetComponent<Move>().initiate();
                isFetching = false;
                
                movepack = new ArrayList(){
                    new ArrayList(){0,0},
                    new ArrayList(){0,0},
                    new ArrayList(){0,0},
                    new ArrayList(){0,0},
                    new ArrayList(){0,0}
                };
                
            }else{
                GameObject.Find("Agent").transform.eulerAngles = new Vector3(0,0,0);
            }

        }
    }


    ArrayList blendMoves(ArrayList first, ArrayList sec){
        first.Reverse();
        sec.Reverse();

        for (int i = 0; i < sec.Count; i++){
            if (i == 0){
                first.Add(new ArrayList(){"##", retrievenode});
            }
            first.Add(sec[i]);           
        }

        return first;
        
    }
    int getshortest(ArrayList dists, ArrayList astart){

        float sd = -1;
        int small = 0;

        for (int i = 0; i < dists.Count; i++){
            ArrayList pathArray =(ArrayList) dists[i];
            ArrayList pathArraya =(ArrayList) astart[i];

            float dd = 0;
            for (int l = 0; l < pathArray.Count; l++) {
                ArrayList movebund = (ArrayList) pathArray[l];
                dd += (float) movebund[1];           
            }
            for (int l = 0; l < pathArraya.Count; l++) {
                ArrayList movebund = (ArrayList) pathArraya[l];
                dd += (float) movebund[1];
            }
            
            if (sd == -1 || dd < sd){
                sd = dd;
                small = i;
            }

        }
        return small;

    }





    void TogglePanel(int tog){
        if (tog == 0){
            GameObject.Find("Interactor").transform.localPosition = new Vector3(300000,0,0);
        }else{
            writeArray();
            GameObject.Find("Interactor").transform.localPosition = new Vector3(57,-133,4);

        }
    }
    void openForClick(){
        GameObject.Find("Superstreet").transform.localPosition = new Vector3(0,0,30000);
        TogglePanel(0);
        Transform[] ts = GameObject.Find("Nodegrid").GetComponentsInChildren<Transform>();

        foreach (Transform c in ts){
            c.gameObject.GetComponent<Renderer>().enabled = true;
        }
        pointselect = true;
    }
    void closeForClick(){
        GameObject.Find("Superstreet").transform.localPosition = new Vector3(0,0,0);
        TogglePanel(1);
        Transform[] ts = GameObject.Find("Nodegrid").GetComponentsInChildren<Transform>();

        foreach (Transform c in ts){
            c.gameObject.GetComponent<Renderer>().enabled = false;
        }

        Transform[] ts2 = GameObject.Find("Nodegrid/Agentsupercont").GetComponentsInChildren<Transform>();
        foreach (Transform c in ts2){
            c.gameObject.GetComponent<Renderer>().enabled = true;
        }


        pointselect = false;

    }


    void clearpack(int i){

        ((ArrayList) movepack[i])[0] = 0;
        ((ArrayList) movepack[i])[1] = 0;

        writeArray();
    }


    void writeArray(){
        for(int i = 0; i<movepack.Count; i++){
            ArrayList varr = (ArrayList) movepack[i];
            
            GameObject.Find("Interactor/Ipanel/" + (i+1) + "/start").GetComponent<Renderer>().material.SetColor("_Color", Color.red);
            GameObject.Find("Interactor/Ipanel/" + (i+1) + "/end").GetComponent<Renderer>().material.SetColor("_Color", Color.red);

            if ((varr[0]+"") != "0"){
                GameObject.Find("Interactor/Ipanel/" + (i+1) + "/start").GetComponent<Renderer>().material.SetColor("_Color", Color.green);
            }
            if ((varr[1]+"") != "0"){
                GameObject.Find("Interactor/Ipanel/" + (i+1) + "/end").GetComponent<Renderer>().material.SetColor("_Color", Color.green);
            }
        }
        insertPointers();
    }

    void insertPointers(){
        for (int i = 0; i < 5; i++){
            GameObject insObj = null;
            if (i == 0){
                insObj = GameObject.Find("Teddybear");
            }
            if (i  == 1){
                insObj = GameObject.Find("boxPresent");
            }
            if (i == 2){
                insObj = GameObject.Find("Christmas-Ball");
            }
            if (i == 3){
                insObj = GameObject.Find("Teddy2");
            }
            if (i == 4){
                insObj = GameObject.Find("chris2");
            }
            GameObject colObj = GameObject.Find("collector"+i);
            colObj.transform.parent = GameObject.Find("Presents").transform;
            colObj.transform.localPosition = new Vector3(0,0,0);

            insObj.GetComponent<Renderer>().enabled =true;
            colObj.GetComponent<Renderer>().enabled =true;
            insObj.transform.parent = GameObject.Find("Presents").transform;
            insObj.transform.localPosition = new Vector3(0,0,0);
        }

        for(int i = 0; i< 5; i++){
            ArrayList varr = (ArrayList) movepack[i];

            GameObject insObj = null;
            GameObject coll = null;
            
            if (i == 0){
                insObj = GameObject.Find("Presents/Teddybear");
            }
            if (i  == 1){
                insObj = GameObject.Find("Presents/boxPresent");
            }
            if (i == 2){
                insObj = GameObject.Find("Presents/Christmas-Ball");
            }
            if (i == 3){
                insObj = GameObject.Find("Presents/Teddy2");
            }
            if (i == 4){
                insObj = GameObject.Find("Presents/chris2");
            }


            if ((varr[0]+"") != "0"){
                insObj.transform.parent = GameObject.Find(varr[0]+"").transform;
                insObj.transform.localScale = new Vector3(1, 10, 1);
                insObj.transform.localPosition = new Vector3(0, 0, 0);
            }

            coll = GameObject.Find("Presents/collector" + i);
            if ((varr[1]+"") != "0"){
                coll.transform.parent = GameObject.Find(varr[1]+"").transform;
                coll.transform.localScale = new Vector3(5, 1, 5);
                coll.transform.localPosition = new Vector3(0, 20, 0);
            }

        }

    }
}
