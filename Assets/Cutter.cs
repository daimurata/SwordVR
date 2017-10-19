using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 実際にオブジェクトをカットする処理クラス
public class Cutter : MonoBehaviour
{
    public class MeshCutSide
    {
        public List<Vector3> vertices = new List<Vector3>();
        public List<Vector3> normals = new List<Vector3>();
        public List<Vector2> uvs = new List<Vector2>();
        public List<int> triangles = new List<int>();
        public List<List<int>> subIndices = new List<List<int>>();

        public void ClearAll()
        {
            vertices.Clear();
            normals.Clear();
            uvs.Clear();
            triangles.Clear();
            subIndices.Clear();
        }

        /// <summary>
        /// トライアングルとして3頂点を追加
        /// ※ 頂点情報は元のメッシュからコピーする
        /// </summary>
        /// <param name="p1">頂点1</param>
        /// <param name="p2">頂点2</param>
        /// <param name="p3">頂点3</param>
        /// <param name="submesh">対象のサブメシュ</param>
        //public void AddTriangle(int p1, int p2, int p3, int submesh)
        //{
        //    // triangle index order goes 1,2,3,4....

        //    // 頂点配列のカウント。随時追加されていくため、ベースとなるindexを定義する。
        //    // ※ AddTriangleが呼ばれるたびに頂点数は増えていく。
        //    int base_index = vertices.Count;

        //    // 対象サブメッシュのインデックスに追加していく
        //    subIndices[submesh].Add(base_index + 0);
        //    subIndices[submesh].Add(base_index + 1);
        //    subIndices[submesh].Add(base_index + 2);

        //    // 三角形郡の頂点を設定
        //    triangles.Add(base_index + 0);
        //    triangles.Add(base_index + 1);
        //    triangles.Add(base_index + 2);

        //    // 対象オブジェクトの頂点配列から頂点情報を取得し設定する
        //    // （victim_meshはstaticメンバなんだけどいいんだろうか・・）
        //    vertices.Add(victim_mesh.vertices[p1]);
        //    vertices.Add(victim_mesh.vertices[p2]);
        //    vertices.Add(victim_mesh.vertices[p3]);

        //    // 同様に、対象オブジェクトの法線配列から法線を取得し設定する
        //    normals.Add(victim_mesh.normals[p1]);
        //    normals.Add(victim_mesh.normals[p2]);
        //    normals.Add(victim_mesh.normals[p3]);

        //    // 同様に、UVも。
        //    uvs.Add(victim_mesh.uv[p1]);
        //    uvs.Add(victim_mesh.uv[p2]);
        //    uvs.Add(victim_mesh.uv[p3]);
        //}

        /// <summary>
        /// トライアングルを追加する
        /// ※ オーバーロードしている他メソッドとは異なり、引数の値で頂点（ポリゴン）を追加する
        /// </summary>
        /// <param name="points3">トライアングルを形成する3頂点</param>
        /// <param name="normals3">3頂点の法線</param>
        /// <param name="uvs3">3頂点のUV</param>
        /// <param name="faceNormal">ポリゴンの法線</param>
        /// <param name="submesh">サブメッシュID</param>
        public void AddTriangle(Vector3[] points3, Vector3[] normals3, Vector2[] uvs3, Vector3 faceNormal, int submesh)
        {
            // 引数の3頂点から法線を計算
            Vector3 calculated_normal = Vector3.Cross((points3[1] - points3[0]).normalized, (points3[2] - points3[0]).normalized);

            int p1 = 0;
            int p2 = 1;
            int p3 = 2;

            // 引数で指定された法線と逆だった場合はインデックスの順番を逆順にする（つまり面を裏返す）
            if (Vector3.Dot(calculated_normal, faceNormal) < 0)
            {
                p1 = 2;
                p2 = 1;
                p3 = 0;
            }

            int base_index = vertices.Count;

            subIndices[submesh].Add(base_index + 0);
            subIndices[submesh].Add(base_index + 1);
            subIndices[submesh].Add(base_index + 2);

            triangles.Add(base_index + 0);
            triangles.Add(base_index + 1);
            triangles.Add(base_index + 2);

            vertices.Add(points3[p1]);
            vertices.Add(points3[p2]);
            vertices.Add(points3[p3]);

            normals.Add(normals3[p1]);
            normals.Add(normals3[p2]);
            normals.Add(normals3[p3]);

            uvs.Add(uvs3[p1]);
            uvs.Add(uvs3[p2]);
            uvs.Add(uvs3[p3]);
        }
    }

    [SerializeField]
    Material Mat;

    private Vector3 _pos1; // planeとmeshの交点その1
    private Vector3 _pos2; // planeとmeshの交点その2

//    public void Cut(Plane plane, CutMesh _cutMesh)
//    {
//        //二つのオブジェクトの法線と三角形のリストを作成
//        List<Vector3> group1ObjPosList = new List<Vector3>();
//        List<int> group1ObjTriList = new List<int>();

//        List<Vector3> group2ObjPosList = new List<Vector3>();
//        List<int> group2ObjTriList = new List<int>();

//        //Meshの三角形数
//        int[] meshTriangles = _cutMesh.Mesh.triangles;

//        //
//        Vector3[] meshVertices = _cutMesh.Mesh.vertices;

//        //Nomalマッピング用座標
//        Vector3[] meshNormals = _cutMesh.Mesh.normals;

//        //メッシュ座標
//        Vector3 meshPos = _cutMesh.transform.position;

//        //メッシュスケール
//        Vector3 meshScale = _cutMesh.transform.localScale;

//        for (int i = 0; i < meshTriangles.Length; i += 3)
//        {
//            List<Vector3> group1PosList = new List<Vector3>();
//            List<int> group1TriList = new List<int>();
//            List<Vector3> group2PosList = new List<Vector3>();
//            List<int> group2TriList = new List<int>();

//            int idx0 = meshTriangles[i];
//            int idx1 = meshTriangles[i + 1];
//            int idx2 = meshTriangles[i + 2];

//            List<Vector3> verts = new List<Vector3>();

//            // 頂点位置をscaleやpositionに合わせてしっかり計算しないとおかしくなる
//            Vector3 v1 = Vector3.Scale(meshVertices[idx0], meshScale) + meshPos;
//            Vector3 v2 = Vector3.Scale(meshVertices[idx1], meshScale) + meshPos;
//            Vector3 v3 = Vector3.Scale(meshVertices[idx2], meshScale) + meshPos;

//            verts.Add(v1);
//            verts.Add(v2);
//            verts.Add(v3);

//            //ポリゴンの法線を計算
//            Vector3 normal = Vector3.Cross(meshVertices[idx2] - meshVertices[idx0], meshVertices[idx1] - meshVertices[idx0]);

//            //1.グループ分け
//            CheckPlaneSide(plane, verts, group1PosList, group2PosList);

//            if (group1PosList.Count > 0 && group2PosList.Count > 0)
//            {
//                //2.planeとの交点を求める
//                CalcCrossPoint(plane, group1PosList, group2PosList);

//                //3.両方のグループともに交点を入れる
//                group1PosList.Add(_pos1);
//                group1PosList.Add(_pos2);

//                group2PosList.Add(_pos1);
//                group2PosList.Add(_pos2);
//            }

//            //
//            if (group1PosList.Count > 0)
//            {
//                List<int> tris1 = CreateTriangles(group1PosList, normal);
//                int triIdx = group1ObjPosList.Count;

//                group1ObjPosList.AddRange(group1PosList);

//                //二つめ以降ならidxがずれることに注意
//                foreach (int triI in tris1)
//                {
//                    group1ObjTriList.Add(triI + triIdx);
//                }
//            }

//            if (group2PosList.Count > 0)
//            {
//                var tris2 = CreateTriangles(group2PosList, normal);
//                var triIdx = group2ObjPosList.Count;

//                group2ObjPosList.AddRange(group2PosList);

//                // 二つめ以降ならidxがずれることに注意
//                foreach (var triI in tris2)
//                {
//                    group2ObjTriList.Add(triI + triIdx);
//                }
//            }
//        }

//         private static MeshCutSide left_side = new MeshCutSide();
//    private static MeshCutSide right_side = new MeshCutSide();

//    // List<List<int>>型のリスト。サブメッシュ一つ分のインデックスリスト
//    left_side.subIndices.Add(new List<int>());  // 左
//        right_side.subIndices.Add(new List<int>()); // 右

//        //メッシュを生成
//        //MeshCutSideクラスのメンバから各値をコピー
//        Mesh left_HalfMesh = new Mesh();
//    left_HalfMesh.name = "Split Mesh Left";
//        left_HalfMesh.vertices = left_side.vertices.ToArray();
//        left_HalfMesh.triangles = left_side.triangles.ToArray();
//        left_HalfMesh.normals = left_side.normals.ToArray();
//        left_HalfMesh.uv = left_side.uvs.ToArray();

//        left_HalfMesh.subMeshCount = left_side.subIndices.Count;
//        for (int i = 0; i<left_side.subIndices.Count; i++)
//        {
//            left_HalfMesh.SetIndices(left_side.subIndices[i].ToArray(), MeshTopology.Triangles, i);
//        }

//        // 4.2つのグループに分けたオブジェクトを作成する
//        //(Verts, tris)
//        CreateCutObj(group1ObjPosList, group1ObjTriList);
//        CreateCutObj(group2ObjPosList, group2ObjTriList);



//_cutMesh.gameObject.SetActive(false); // 5.元となるオブジェクトを非表示にする
//    }

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

        //少ない所から多い片方の頂点に向かってrayを飛ばす。
        Ray ray1 = new Ray(basePos, (tmpPos1 - basePos).normalized);

        //planeと交差する距離を求める
        plane.Raycast(ray1, out distance);

        //ray1がその距離を進んだ位置を取得(ここが交点になる)
        _pos1 = ray1.GetPoint(distance);

        //同じようにもう片方も計算
        Ray ray2 = new Ray(basePos, (tmpPos2 - basePos).normalized);
        plane.Raycast(ray2, out distance);
        _pos2 = ray2.GetPoint(distance);
    }

    /// <summary>
    /// 頂点インデックスを計算する
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="normal"></param>
    /// <returns></returns>
    private List<int> CreateTriangles(List<Vector3> pos , Vector3 normal)
    {
        if (pos.Count < 3)
        {
            return null;
        }

        List<int> triangles = new List<int>();

        int triIdx = 0;
        int triIdx0 = 0; // 0固定
        int triIdx1 = 0;
        int triIdx2 = 0;
        Vector3 cross = Vector3.zero;
        float inner = 0.0f;

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
        //オブジェクト作成
        GameObject obj = new GameObject("cut obj", typeof(MeshFilter), typeof(MeshRenderer) , typeof(Rigidbody), typeof(MeshCollider), typeof(CutMesh));

        //メッシュ作成
        Mesh objmesh = new Mesh();
        objmesh.vertices = verts.ToArray();
        objmesh.triangles = tris.ToArray();
        objmesh.RecalculateNormals();

        //MeshRenderer作成
        MeshRenderer renderer = obj.GetComponent<MeshRenderer>();

        //マテリアルを設定
        renderer.material = Mat;

        //作成したメッシュを設定
        obj.GetComponent<MeshFilter>().mesh = objmesh;

        //Rigidbodyの設定
        Rigidbody rigidBody = obj.GetComponent<Rigidbody>();
        rigidBody.AddForce(Random.Range(-100, 100), Random.Range(-100, 100), Random.Range(-100, 100), ForceMode.Force);

        //MeshColliderの設定
        MeshCollider col = obj.GetComponent<MeshCollider>();
        col.sharedMesh = objmesh;
        col.convex = true;
    }

}