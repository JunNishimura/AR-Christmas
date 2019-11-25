using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    private void Update() 
    {
        transform.position = new Vector3(Mathf.Repeat(Time.time, 10), transform.position.y, transform.position.z);    
    }
}
