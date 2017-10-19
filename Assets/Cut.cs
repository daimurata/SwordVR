using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cut : MonoBehaviour
{
    GameObject CutObject;

    [SerializeField,Header("Blade")]
    Blade _blade;

    [SerializeField,Header("Material")]
    Material Mat;

    Vector3 StartPos, EndPos;

    void OnCollisionEnter(Collision collision)
    {
        CutObject = collision.gameObject;
        StartPos = collision.contacts[0].point;
    }

    void OnCollisionStay(Collision collision)
    {
        EndPos = collision.contacts[0].point;
    }

    void OnCollisionExit(Collision collision)
    {
        //オブジェクトカット
        //Blade.Cut(CutObject, StartPos, EndPos, Mat);
    }
}