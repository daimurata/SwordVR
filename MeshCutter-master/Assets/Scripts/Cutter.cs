using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 実際にオブジェクトをカットする処理クラス
public class Cutter : MonoBehaviour
{

    private Vector3 _pos1; // planeとmeshの交点その1
    private Vector3 _pos2; // planeとmeshの交点その2

    public void Cut(Plane plane , CutMesh _cutMesh)
    {

        var group1ObjPosList = new List<Vector3>();
        var group1ObjTriList = new List<int>();

        var group2ObjPosList = new List<Vector3>();
        var group2ObjTriList = new List<int>();

        // 色々必要になってしまった
        var meshTriangles = _cutMesh.Mesh.triangles;
        var meshVertices = _cutMesh.Mesh.vertices;
        var meshNormals = _cutMesh.Mesh.normals;
        var meshPos = _cutMesh.transform.position;
        var meshScale = _cutMesh.transform.localScale;

        for (var i = 0; i < meshTriangles.Length; i += 3)
        {
            var group1PosList = new List<Vector3>();
            var group1TriList = new List<int>();
            var group2PosList = new List<Vector3>();
            var group2TriList = new List<int>();

            var idx0 = meshTriangles[i];
            var idx1 = meshTriangles[i + 1];
            var idx2 = meshTriangles[i + 2];

            var verts = new List<Vector3>();

            // 頂点位置をscaleやpositionに合わせてしっかり計算しないとおかしくなる
            // あれ、もうmatrixで計算したほうがいい？
            var v1 = Vector3.Scale(meshVertices[idx0], meshScale) + meshPos;
            var v2 = Vector3.Scale(meshVertices[idx1], meshScale) + meshPos;
            var v3 = Vector3.Scale(meshVertices[idx2], meshScale) + meshPos;

            verts.Add(v1);
            verts.Add(v2);
            verts.Add(v3);

            // そのポリゴンの法線を計算しておく
            var normal = Vector3.Cross(meshVertices[idx2] - meshVertices[idx0], meshVertices[idx1] - meshVertices[idx0]);


            CheckPlaneSide(plane, verts, group1PosList, group2PosList); // 1.グループ分け

            if (group1PosList.Count > 0 && group2PosList.Count > 0)
            {
                CalcCrossPoint(plane, group1PosList, group2PosList); // 2.planeとの交点を求める

                // 3.両方のグループともに交点を入れる
                group1PosList.Add(_pos1);
                group1PosList.Add(_pos2);

                group2PosList.Add(_pos1);
                group2PosList.Add(_pos2);
            }

            if (group1PosList.Count > 0)
            {
                var tris1 = CreateTriangles(group1PosList , normal);
                var triIdx = group1ObjPosList.Count;

                group1ObjPosList.AddRange(group1PosList);

                // 二つめ以降ならidxがずれることに注意
                foreach (var triI in tris1)
                {
                    group1ObjTriList.Add(triI + triIdx);
                }
            }

            if (group2PosList.Count > 0)
            {
                var tris2 = CreateTriangles(group2PosList , normal);
                var triIdx = group2ObjPosList.Count;

                group2ObjPosList.AddRange(group2PosList);

                // 二つめ以降ならidxがずれることに注意
                foreach (var triI in tris2)
                {
                    group2ObjTriList.Add(triI + triIdx);
                }
            }
            
        }

        // 4.2つのグループに分けたオブジェクトを作成する
        CreateCutObj(group1ObjPosList, group1ObjTriList);
        CreateCutObj(group2ObjPosList, group2ObjTriList);



        _cutMesh.gameObject.SetActive(false); // 5.元となるオブジェクトを非表示にする
    }

    // planeのどちらにあるかを計算して振り分ける
    private void CheckPlaneSide(Plane plane, List<Vector3> vertices, List<Vector3> group1, List<Vector3> group2)
    {
        foreach (var v in vertices)
        {
            // どちらかのグループに振り分ける
            if (plane.GetSide(v))
            {
                group1.Add(v);
            }
            else
            {
                group2.Add(v);
            }
        }
    }

    // planeとmeshの交点を求める
    private void CalcCrossPoint(Plane plane, List<Vector3> group1, List<Vector3> group2)
    {
        float distance = 0;
        Vector3 basePos; // 計算する基準となる頂点
        Vector3 tmpPos1; // 基準点以外の頂点1
        Vector3 tmpPos2; // 基準点以外の頂点2

        // 少ない方からplaneに対して交差するpointを聞く
        if (group2.Count < group1.Count)
        {
            basePos = group2[0];
            tmpPos1 = group1[0];
            tmpPos2 = group1[1];
        }
        else
        {
            basePos = group1[0];
            tmpPos1 = group2[0];
            tmpPos2 = group2[1];
        }

        // 少ない所から多い片方の頂点に向かってrayを飛ばす。
        Ray ray1 = new Ray(basePos, (tmpPos1 - basePos).normalized);
        // planeと交差する距離を求める
        plane.Raycast(ray1, out distance);
        // ray1がその距離を進んだ位置を取得(ここが交点になる)
        _pos1 = ray1.GetPoint(distance);

        // 同じようにもう片方も計算
        Ray ray2 = new Ray(basePos, (tmpPos2 - basePos).normalized);
        plane.Raycast(ray2, out distance);
        _pos2 = ray2.GetPoint(distance);
    }

    // 頂点インデックスを計算する
    private List<int> CreateTriangles(List<Vector3> pos , Vector3 normal)
    {
        if (pos.Count < 3)
        {
            return null;
        }


        var triangles = new List<int>();

        var triIdx = 0;
        var triIdx0 = 0; // 0固定
        var triIdx1 = 0;
        var triIdx2 = 0;
        var cross = Vector3.zero;
        var inner = 0.0f;

        for (int i = 0; i < pos.Count; i += 3)
        {
            triIdx0 = triIdx;
            triIdx1 = triIdx + 1;
            triIdx2 = triIdx + 2;

            cross = Vector3.Cross(pos[triIdx2] - pos[triIdx0], pos[triIdx1] - pos[triIdx0]);
            inner = Vector3.Dot(cross, normal);

            // 逆向いている場合は反転させる
            if (inner < 0)
            {
                triIdx0 = triIdx2;
                triIdx2 = triIdx;
            }

            triangles.Add(triIdx0);
            triangles.Add(triIdx1);
            triangles.Add(triIdx2);
            triIdx++;
        }

        return triangles;
    }

    // cutしたmeshを作る
    private void CreateCutObj(List<Vector3> verts, List<int> tris)
    {
        var obj = new GameObject("cut obj", typeof(MeshFilter), typeof(MeshRenderer) , typeof(Rigidbody));

        var mesh = new Mesh();
        mesh.vertices = verts.ToArray();
        mesh.triangles = tris.ToArray();
        mesh.RecalculateNormals();

        obj.GetComponent<MeshFilter>().mesh = mesh;

        var rigidBody = obj.GetComponent<Rigidbody>();
        rigidBody.AddForce(Random.Range(-100, 100), Random.Range(-100, 100), Random.Range(-100, 100), ForceMode.Force);
    }

}