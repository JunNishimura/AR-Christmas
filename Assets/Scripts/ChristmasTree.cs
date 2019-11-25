using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChristmasTree : MonoBehaviour
{
    [SerializeField] private float snowCoverdSeconds;
    private Renderer treeRenderer;

    private void Start() 
    {
        treeRenderer = this.GetComponent<Renderer>();
    }

    private void Update() 
    {
        treeRenderer.material.SetFloat("Vector1_11A0A3C5", Mathf.Min(Time.time / snowCoverdSeconds, 1.0f));
    }
}
