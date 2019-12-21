using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARChristmas.Snow
{
    public class AccumulateSnow : MonoBehaviour
    {
        [SerializeField] float snowCoverdSeconds; // 積雪量が最大値に達するまでにかかる時間
        private Renderer treeRenderer;
        private void Start() 
        {
            treeRenderer = this.GetComponent<Renderer>();
        }
        private void Update() 
        {
            // 時間経過に伴い雪が積もるように、現フレームの積雪量と最大積雪量との比をシェーダー側に送る
            treeRenderer.material.SetFloat("Vector1_11A0A3C5", Mathf.Min(Time.time / snowCoverdSeconds, 1.0f));
        }
    }   
}