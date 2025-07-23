using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[ExecuteAlways]
public class MapMeshLoader : MonoBehaviour
{
    public string mapPath;
    private Mesh mesh;

    public void Start()
    {
        mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        GetComponent<MeshFilter>().mesh = mesh;
        CreateGeometry();
    }

    private void CreateGeometry()
    {
        Map map = Map.instance;

        Debug.Log($"Loading geometry {map.width}x{map.height}");

        List<Vector3> verticies = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<int> triangles = new List<int>();

        for (int y = 0; y < map.height; y++)
        {
            for (int x = 0; x < map.width; x++)
            {
                var point = new Vector3(x * Map.metersPerPixel, map.heightmap[y, x], y * Map.metersPerPixel);
                verticies.Add(point);
                uvs.Add(new Vector2(x / (float)map.width, y / (float)map.height));


                if (y < map.height - 1 && x < map.width - 1)
                {
                    int a = x + y * map.width;
                    int b = (x + 1) + y * map.width;
                    int c = x + (y + 1) * map.width;
                    int d = (x + 1) + (y + 1) * map.width;

                    // First tri
                    triangles.Add(a);
                    triangles.Add(c);
                    triangles.Add(d);

                    // Second tri
                    triangles.Add(a);
                    triangles.Add(d);
                    triangles.Add(b);
                }
            }
        }


        mesh.Clear();
        mesh.vertices = verticies.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.RecalculateNormals();

        Debug.Log($"Loaded {verticies.Count} verts and {triangles.Count} tris");
    }
}
