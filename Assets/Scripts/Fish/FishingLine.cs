using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishingLine : MonoBehaviour
{
    // Start is called before the first frame update
    public LineRenderer fishing_line;
    public Transform cast_point;
    public Transform lure;

    private void Awake()
    {
        fishing_line.SetPosition(0, cast_point.position);
        fishing_line.gameObject.transform.position = this.transform.position;

    }

    // Update is called once per frame
    void Update()
    {
        fishing_line.SetPosition(1, lure.position);
    }
}
