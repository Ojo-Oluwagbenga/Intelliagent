using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class General{
    void Start(){
        
    }

    // Update is called once per frame
    void Update(){
        
    }
    public ArrayList paintedNodes;
    public ArrayList findPath(GameObject start, GameObject end){
        Dictionary<string, GameObject> opened = new Dictionary<string, GameObject>();
        ArrayList path = new ArrayList();
        int loopcontrol = 0;
        GameObject startPointer = start;
        GameObject endPointer = end;


        Dictionary<string, GameObject> usedNodes = new Dictionary<string, GameObject>();
        usedNodes.Add(start.name, start);
        for (int a = 0; a < 2; a++){
            loopcontrol += 1;
            var keysort = opened.Keys.ToList();
            keysort.Sort();
            foreach(var small in keysort){
                start = opened[small];
                break;
            }
            
            ArrayList nearNodes = findNearNode(start);

            for (int i = 0; i < nearNodes.Count; i++){
                
                if( nearNodes[i] != null && !isClosed(nearNodes[i]) ){
                    GameObject cast = (GameObject)nearNodes[i];
                    GridnodeProp castprop = cast.GetComponent<GridnodeProp>();

                    usedNodes[cast.name] = cast;

                    bool nodeopened = castprop.opened;
                    string oldD = castprop.D+"";
                    string oldName = oldD.PadLeft(6, '0')+"*"+cast.name;

                    string newD = updateNode(cast, start, endPointer)+"";
                    string newName = newD.PadLeft(6, '0')+"*"+cast.name;

                    if (nodeopened){
                        opened.Remove(oldName);
                    }
                    opened.Add(newName, cast);
                }
            }

            GridnodeProp startprop = start.GetComponent<GridnodeProp>();
            startprop.closed = true;
            startprop.opened = false;
            string s_oldD = startprop.D+"";
            string s_oldName = s_oldD.PadLeft(6, '0')+"*"+start.name;
            opened.Remove(s_oldName);
            
            a = 0;
            
            if (start.name == end.name){
                loopcontrol = 0;
                path = readPath(startPointer, endPointer);
                clearNodes(usedNodes);
                a = 3;
            }

            if (loopcontrol > 1000){
                Debug.Log("Over Loop on findPATH");
                a = 3;
            }

            
        }

        return path;
        


    }
    void clearNodes(Dictionary<string, GameObject> used){
        foreach(var v in used){
            GameObject g = v.Value;
            v.Value.GetComponent<GridnodeProp>().reset();
        }
    }
    public void clearPaintedNodes(ArrayList nodes){
        foreach(var n in nodes){
            GameObject nn = (GameObject) n;
            nn.GetComponent<Renderer>().enabled = false;
        }

    }
    public void clearPaintedNodes(){

        Transform[] g = GameObject.Find("Nodegrid").GetComponentsInChildren<Transform>();
        foreach (Transform item in g){
            GridnodeProp gg = item.GetComponent<GridnodeProp>();
            if (gg != null){
                item.GetComponent<GridnodeProp>().clearPaint();
            }
        }
        

    }
    ArrayList readPath(GameObject start, GameObject end){
        GridnodeProp endnode = end.GetComponent<GridnodeProp>();
        GridnodeProp startnode = start.GetComponent<GridnodeProp>();

        ArrayList path = new ArrayList();
        ArrayList dirstart = getDir(endnode,endnode.parent.GetComponent<GridnodeProp>());
        string flow = (string)dirstart[0];
        float dist = 0;
        int loopcontrol = 0;

        paintedNodes = new ArrayList();

        int colalter = 0;
        for (int i = 0; i < 2; i++){
            loopcontrol += 0;
            if (colalter % 2 == 0){
                colalter = 0;
                endnode.addPaint();
            }
            colalter +=1;           

            paintedNodes.Add(endnode.gameobj);

            ArrayList dir = getDir(endnode,endnode.parent.GetComponent<GridnodeProp>());
            if(flow == (string)dir[0]){
                dist += (float)dir[1];
            }else{
                path.Add(new ArrayList(){flow, dist});
                flow = (string)dir[0];
                dist = (float)dir[1];
            }
            if (endnode.parent.name == start.name){
                endnode.addPaint();
                paintedNodes.Add(endnode.parent);

                path.Add(new ArrayList(){flow, dist});
                i = 3;
            }else{
                endnode = endnode.parent.GetComponent<GridnodeProp>();
                i = 0;
            }

            if (loopcontrol > 2048){
                Debug.Log("Over Loop on readPATH");
                i = 3;
            }
        }
        return path;
    }
    ArrayList getDir(GridnodeProp central, GridnodeProp other){
        int x = central.x-other.x;
        int z = central.z-other.z;

        float pw = (float) Math.Pow((x*x+z*z),0.5);
        
        float dist = (float)(pw)*20;
        return new ArrayList(){x+"_"+z, dist};
    }
    bool blocked(object node){
        return (node == null);
    }
    bool isClosed(object n){
        GameObject node = (GameObject) n;
        return node.GetComponent<GridnodeProp>().closed;
    }
    // TT is the distance to destination - TO TARGET
    int calcTT(GameObject target, GameObject node){
        GridnodeProp n = node.GetComponent<GridnodeProp>();
        GridnodeProp t = target.GetComponent<GridnodeProp>();

        return (int) Math.Pow((t.x - n.x)*(t.x - n.x) + (t.z - n.z)*(t.z - n.z), 0.5) * 10;
    }

    // FS is the distance from parent node - FROM SOURCE
    int calcFS(GameObject from, GameObject node){
        var k = calcTT(from,node);
        if (k != 10 && k != 20){
            Debug.Log("--------------------");
        }
        if (k == 10){
            return 10;
        }else{
            return 14;
        }  
        
        
    }


    int absol(int l){
        return Math.Abs(l);
        
    }
    int updateNode(GameObject node, GameObject from, GameObject target){        
        int ret =  node.GetComponent<GridnodeProp>().setCosts(calcFS(from,node), calcTT(node,target), from);
        return ret;
    }

    ArrayList findNearNode(GameObject n){
        //Oriented as x hori and z vert
        GridnodeProp node = n.GetComponent<GridnodeProp>();
        return new ArrayList() {
            GameObject.Find("x|"+ node.x+ "-z|"+ (node.z+1)),
            // GameObject.Find("x|"+ (node.x+1) +"-z|"+ (node.z+1)),
            GameObject.Find("x|"+ (node.x+1) +"-z|"+node.z),
            // GameObject.Find("x|"+ (node.x+1) + "-z|"+(node.z-1)),
            GameObject.Find("x|"+ node.x + "-z|"+ (node.z-1)),
            // GameObject.Find("x|"+ (node.x-1) +"-z|"+ (node.z-1)),
            GameObject.Find("x|"+(node.x-1)+"-z|"+node.z),
            // GameObject.Find("x|"+(node.x-1)+"-z|"+ (node.z+1))
        };

    }

    
}
