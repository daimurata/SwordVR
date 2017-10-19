using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BladeSystem : MonoBehaviour
{
    GameObject CutObject;

    [SerializeField, Header("Material")]
    Material Mat;

    //[SerializeField,Header("Cutter")]
    //Cutter _cutter;

    private LineRenderer _lineRenderer;
    private Plane _plane;

    private Vector3 normal;
    private Vector3 position;

    [SerializeField]
    OVRInput.Controller controller;

    private float speed;

    //CutMesh cutMesh;

    Mesh mesh;

    private Vector3 StartPos, EndPos;

    bool Cutjudge;

    /// <summary>
    /// 接触判定をするメソッド(処理)
    /// </summary>
    /// <param name="collision"></param>
    void OnCollisionEnter(Collision collision)
    {
        speed = OVRInput.GetLocalControllerAngularVelocity(controller).magnitude;

        //Debug.Log(controller + "：速度" + speed);

        if (speed >= 0.5)
        {
            //Tag[CutObject]のみ切ることが可能
            if(collision.gameObject.tag=="CutObject")
            {
                CutObject = collision.gameObject;
                
                //切断回数がまだある場合
                if (CutObject.GetComponent<CutControl>().Count != 0)
                {
                    StartPos = collision.contacts[0].point;

                    //対象オブジェクトのメッシュを取得
                    mesh = CutObject.GetComponent<MeshFilter>().mesh;

                    Cutjudge = true;
                    //CutObject.GetComponent<CutControl>().Count -= 1;

                    Debug.Log("切断");
                }
                else
                {
                    Debug.Log("もう切れません");
                }            
            }
            
        }

 
    }

    void OnCollisionStay(Collision collision)
    {
        if (Cutjudge)
        {
            EndPos = collision.contacts[0].point;
        }
     
    }

    void OnCollisionExit(Collision collision)
    {
        if (Cutjudge)
        {
            if (mesh != null)
            {
                Debug.Log("開始地点"+StartPos + "/終了地点"+ EndPos);

                Create();

                //オブジェクトカット
                Blade.Cut(CutObject, _plane, mesh, Mat);

                Cutjudge = false;
            }
        }

    }

    /// <summary>
    /// 平面を作成
    /// </summary>
    private void Create()
    {
        _plane = new Plane();

        position = (StartPos + EndPos) / 2;
        var p1 = StartPos - position;
        normal = (Quaternion.Euler(0f, 0f, 90f) * p1).normalized;
        _plane.SetNormalAndPosition(normal, position);

    }

    //void OnDrawGizmosSelected()
    //{
    //    float length = 10.0f;
    //    Gizmos.color = Color.blue;

    //    Gizmos.DrawLine(position, position + (normal * length));

    //}

    //public Plane Plane
    //{
    //    get
    //    {
    //        return _plane;
    //    }
    //}
}