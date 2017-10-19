using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeOutIn : MonoBehaviour
{
    Color alpha = new Color(0, 0, 0, 0.01f);
    GameObject set;
    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (renderer.material.color.a >= 0)
            renderer.material.color -= alpha;
    }
}