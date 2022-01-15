using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.AI;
using System.Linq;
using UnityEngine.UIElements;

public class NavMeshHelpers : EditorWindow
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

    [MenuItem("Tools/Navmesh Tools")]
    public static void ShowWindow()
    {
        GetWindow(typeof(NavMeshHelpers));
    }

    private void OnEnable()
    {
        buildWindow();
    }

    private static void rebuild_navmesh()
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


    private static void toggle_navmesh_barrier()
    {
        var objs = GameObject.FindGameObjectsWithTag("NavMeshBarrier");
        if(objs.Length <= 0)
        {
            Debug.Log("No barriers found");
            return;
        }

        foreach(GameObject go in objs)
        {
            MeshRenderer rend = go.GetComponent<MeshRenderer>();
            rend.enabled = !rend.enabled;
        }
    }

    private void buildWindow()
    {
        Button btn_rebuild_nav = new Button();
        btn_rebuild_nav.text = "Rebuild Navmesh";
        Button btn_toggle_navmesh_barrier = new Button();
        btn_toggle_navmesh_barrier.text = "Toggle Barrier";

        btn_rebuild_nav.clicked += () => rebuild_navmesh();
        btn_toggle_navmesh_barrier.clicked += () => toggle_navmesh_barrier();

        rootVisualElement.Add(btn_rebuild_nav);
        rootVisualElement.Add(btn_toggle_navmesh_barrier);

    }
}
