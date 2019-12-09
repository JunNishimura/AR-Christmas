using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChristmasTree : MonoBehaviour
{
    public List<Vector3> decoItemPositions { get; set; }
    public List<Color> decoItemColors { get; set; }
    private List<Light> lightsForTree;

    private void Awake() 
    {
        decoItemPositions = new List<Vector3>();
        decoItemColors = new List<Color>();
        lightsForTree = new List<Light>();
        foreach(Transform light in this.transform.Find("Lights").transform) 
        {
            lightsForTree.Add(light.GetComponent<Light>());
        }
    }

    public void SetTreeLight(bool isLightON) 
    {
        foreach(var light in lightsForTree)
        {
            light.enabled = isLightON;
        }
    }
}
