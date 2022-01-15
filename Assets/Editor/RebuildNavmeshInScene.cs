using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.AI;
using System.Linq;

public class RebuildNavmeshInScene : EditorWindow
{
    private static NavMeshSurface[] get_surfaces_in_scene()
    {
       var objs =  GameObject.FindGameObjectsWithTag("NavMeshArea");
       IEnumerable<NavMeshSurface> surfaces = from i in objs
                                   select i.GetComponent<NavMeshSurface>();
        return surfaces.ToArray();
    }

    private static void rebake_navmesh(NavMeshSurface[] surfaces)
    {
        NavMesh.RemoveAllNavMeshData();
        foreach(var surface in surfaces)
        {
            surface.BuildNavMesh();
        }
    }

    [MenuItem("Tools/Rebake NavMesh In Scene")]
    private static void NewMenuOption()
    {
        var surfaces = get_surfaces_in_scene();
        if(surfaces.Length <= 0)
        {
            Debug.Log("No Meshs Found");
            return;
        }
        Debug.Log("Rebuilding NavMesh");
        rebake_navmesh(surfaces);
        Debug.Log("NavMesh Rebuilt");
    }
}
