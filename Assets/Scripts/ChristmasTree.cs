using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChristmasTree : MonoBehaviour
{
    public GameObject decorationItemPrefab;
    // save "Local(relative to the parent tree)" position, not "World" position
    public List<Vector3> decorationItemLocalPos { get; set; }
    public List<Color> decorationItemColors { get; set; }
    private List<Light> lightsForTree;

    private void Awake() 
    {
        decorationItemLocalPos = new List<Vector3>();
        decorationItemColors = new List<Color>();
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

    public void DecorateWithLoadData() 
    {
        if (decorationItemLocalPos.Count != decorationItemColors.Count)
            return;
        
        for (int i = 0; i < decorationItemLocalPos.Count; i++)
        {
            // ----- important ----- //
            // At first, instantiate and set tree as item's parent.
            // Then, set item's local position based on the save data.
            var item = Instantiate(decorationItemPrefab, this.transform.Find("Christmas Tree")) as GameObject;
            item.transform.localPosition = decorationItemLocalPos[i]; 
            item.GetComponent<Renderer>().material.SetColor("Color_A9AB75C1", decorationItemColors[i]); // base color
            item.GetComponent<Renderer>().material.SetColor("Color_B37F01A0", decorationItemColors[i]); // emission color
        }
    }
}