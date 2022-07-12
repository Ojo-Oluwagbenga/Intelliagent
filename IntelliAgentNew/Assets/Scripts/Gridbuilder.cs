using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
// using test;



public class Gridbuilder : MonoBehaviour{
    // Start is called before the first frame update
    GameObject gridHold;
    public General pathfinder;


    void Start(){
        gridHold = GameObject.Find("Nodegrid");

        createGrid();
        // GameObject start = GameObject.Find("x|6-z|6");
        // GameObject end = GameObject.Find("x|100-z|150");

        // pathfinder = new General();
        // ArrayList pathConfig = pathfinder.findPath(start, end);
        // pathConfig.Reverse();
        

        // GameObject Agent = GameObject.Find("Agent");
        // Agent.AddComponent<Move>().setParam(Agent, pathConfig);  
        // Agent.GetComponent<Move>().initiate();
        // Agent.GetComponent<Move>().clm = new ClickManager();

    }
    public void doneFunc(){

    }

    void createGrid(){
        ArrayList vert1 = new ArrayList(){
            6,140,
            194,194
        };
        ArrayList vert2 = new ArrayList(){
            6,194
        };
        ArrayList vert3 = new ArrayList(){
            6,53,
            100,100,
            147,194
        };
        ArrayList vert4 = new ArrayList(){
            6,194
        };
        ArrayList vert5 = new ArrayList(){
            6,6,
            53,100,
            147,194
        };

        ArrayList hori1 = new ArrayList(){
            6,194
        };
        ArrayList hori2 = new ArrayList(){
            6,53
        };
        ArrayList hori3 = new ArrayList(){
            6,194
        };
        ArrayList hori4 = new ArrayList(){
            53,53,
            100,194
        };
        ArrayList hori5 = new ArrayList(){
            6,100,
            147,194
        };
        ArrayList gridvert = new ArrayList(){
            vert1,vert2, vert3, vert4, vert5
        };
        ArrayList gridhori = new ArrayList(){
            hori1,hori2, hori3, hori4, hori5
        };
        ArrayList bounds = new ArrayList(){6,53,100,147,194};

        for (int j = 0; j < gridvert.Count; j++){
            ArrayList item = (ArrayList) gridvert[j];
            for (int i = 0; i < item.Count; i++){
                int start = (int) item[i];
                int end = (int) item[i + 1];
                i += 1;

                
                for (int l = start; l < end+1; l++){
                    string s = "x|"+bounds[j]+"-z|"+l;
                    if (gridHold.transform.Find(s) == null){
                        GameObject node = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        node.transform.parent = gridHold.transform;
                        node.transform.localScale = new Vector3(20,2,20);
                        node.transform.localPosition = new Vector3((int)bounds[j]*20, 0, l*20);
                        node.AddComponent<GridnodeProp>();

                        node.GetComponent<GridnodeProp>().x = (int)bounds[j];
                        node.GetComponent<GridnodeProp>().z = l;
                        node.GetComponent<Renderer>().enabled = false;
                        node.GetComponent<Renderer>().material.color = new Color(0, 204, 102);
                        
                        node.name = "x|"+bounds[j]+"-z|"+l;
                        node.GetComponent<GridnodeProp>().gameobj = node;
                    }
                }
                

            }
        }
        for (int j = 0; j < gridhori.Count; j++){
            ArrayList item = (ArrayList) gridhori[j];
            for (int i = 0; i < item.Count; i++){
                int start = (int) item[i];
                int end = (int) item[i + 1];

                i += 1;

                
                for (int l = start; l < end; l++){
                    string s = "x|"+l+"-z|"+bounds[j];
                    // Debug.Log(gridHold.transform.Find(s));
                    if (gridHold.transform.Find(s) == null){
                        GameObject node = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        node.transform.parent = gridHold.transform;
                        node.transform.localScale = new Vector3(20,2,20);
                        node.transform.localPosition = new Vector3(l*20, 0, (int)bounds[j]*20);
                        node.AddComponent<GridnodeProp>();

                        node.GetComponent<GridnodeProp>().x = l;
                        node.GetComponent<GridnodeProp>().z = (int)bounds[j];
                        node.GetComponent<Renderer>().enabled = false;
                        node.GetComponent<Renderer>().material.color = new Color(0, 204, 102);

                        node.name = "x|"+l+"-z|"+(int)bounds[j];
                        node.GetComponent<GridnodeProp>().gameobj = node;
                    }
                }               
            }
        }
    }

}
