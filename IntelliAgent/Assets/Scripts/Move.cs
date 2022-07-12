using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class Move : MonoBehaviour{
    
    public GameObject gameobj;
    public ArrayList dirsParam;
    public bool init = false;
    public float pace = 10;
    public ClickManager clm;

    int edgeind = 0;
    float edgemoved = 0;

    private void Start() {
        
    }

    int rotangle = 0;
    int angle = 0;
    bool rotate = false;
    private void Update() {
        
        if (rotate){
            rotangle += (angle/10);
            this.gameobj.transform.Rotate(0f, (angle/10), 0, Space.Self);

            if (angle > 0){
                if ((angle - rotangle) < (angle/10)){
                    this.gameobj.transform.Rotate(0f, (angle - rotangle), 0, Space.Self);
                    rotate = false;
                    angle = 0;
                    rotangle = 0;
                }
            }else{
                if ((angle - rotangle) > (angle/10)){
                    this.gameobj.transform.Rotate(0f, (angle - rotangle), 0, Space.Self);
                    rotate = false;
                    angle = 0;
                    rotangle = 0;
                }
            }
        }

        if (this.init){
            ArrayList dir =  (ArrayList)dirsParam[edgeind];

            if ((string)dir[0] != "##"){
                if (edgemoved == 0){
                    int ang = getAngle((string)dir[0]);
                    int iniang = (int)gameobj.transform.eulerAngles.y;
                    angle = ang-iniang;
                    rotate = true;
                }
                edgemoved+=pace;
                this.gameobj.transform.localPosition = getTransMove((string)dir[0], pace, gameobj);
            
                if ( ((float)dir[1] - edgemoved) < pace ){
                    this.gameobj.transform.localPosition = getTransMove((string)dir[0], ((float)dir[1] - edgemoved), gameobj);
                    if (edgeind < dirsParam.Count-1){
                        edgeind+=1;
                        edgemoved=0;
                    }else{
                        this.init = false;
                        edgemoved = 0;
                        edgeind = 0;
                        clm.doneFunc();
                    }
                }
            }else{
                
                Transform item = GameObject.Find((string)dir[1]).transform;
                item.parent = GameObject.Find("AgentSub").transform;
                item.transform.localPosition = new Vector3(-11.5f, 0, -30);
                item.transform.localScale = new Vector3(6, 11, 6);


                edgeind+=1;
            }
        }
    }


    

    public void initiate(){
        this.init = true;
    }

    public void setParam(GameObject gameobj, ArrayList dirsParam){
        this.gameobj = gameobj;
        this.dirsParam = dirsParam;
    }

    int getAngle(string x_z){
        int angle = 0;
        if (x_z == "0_1"){
            angle = 0;
        }
        if (x_z == "1_1"){
            angle = 45;
        }
        if (x_z == "1_0"){
            angle = 90;
        }
        if (x_z == "1_-1"){
            angle = 135;
        }
        if (x_z == "0_-1"){
            angle = 180;
        }
        if (x_z == "-1_-1"){
            angle = 225;
        }
        if (x_z == "-1_0"){
            angle = 270;
        }
        if (x_z == "-1_1"){
            angle = 315;
        }
        return angle;
    } 
    Vector3 getTransMove(string x_y, float distance, GameObject obj){
        string[] n = x_y.Split('_');
        float x = (Int32.Parse(n[0])*distance) + obj.transform.localPosition.x;
        float z = (Int32.Parse(n[1])*distance) + obj.transform.localPosition.z;
        
        return new Vector3(x,0,z);
    }
    
}
