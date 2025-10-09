using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MatchRotation : MonoBehaviour  
{
    public Transform target;
    void Start(){
        transform.rotation = target.rotation;
    }
    void Update(){
        transform.rotation = target.rotation;   
    }
    
}
