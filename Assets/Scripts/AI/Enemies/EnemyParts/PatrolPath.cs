using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolPath : MonoBehaviour
{
    // Start is called before the first frame update

    public enum PathPatterns
    {
        SequentialLoop,
        Random,
        BackAndForth

    }

    public List<Transform> points = new List<Transform>();
    public PathPatterns pattern = PathPatterns.SequentialLoop;
    private int point_idx = 0;
    private bool decrement = false;

    private void Awake()
    {
        points = new List<Transform>();
        foreach(Transform child in transform)
        {
            points.Add(child);
        }

    }

    public Transform getNextPoint()
    {
        switch (pattern)
        {
            case PathPatterns.BackAndForth:
                return get_point_back_and_forth();
            case PathPatterns.Random:
                return get_random_point();
            case PathPatterns.SequentialLoop:
                return get_point_sequential();
            default:
                return get_point_sequential();
        }
    }

    private Transform get_random_point()
    {
        int idx = Random.Range(0, points.Count);
        return this.points[idx];

    }

    private Transform get_point_sequential()
    {
        if (point_idx >= points.Count)
            point_idx = 0;
        Transform to_return = points[point_idx];
        point_idx++;
        return to_return;
    }

    private Transform get_point_back_and_forth()
    {
        if (point_idx >= points.Count)
            decrement = true;
        if (point_idx <= 0)
            decrement = false;

        Transform to_return = points[point_idx];


        if (decrement)
            point_idx--;
        else
            point_idx++;
        return to_return;
    }

}
