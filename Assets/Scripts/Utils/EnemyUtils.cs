using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyUtils
{
    public static Vector3 randomVector3(float min_x,
                                        float max_x,
                                        float min_y,
                                        float max_y,
                                        float min_z,
                                        float max_z)
    {
        Vector3 vect = new Vector3();
        vect.x = Random.Range(min_x, max_x);
        vect.y = Random.Range(min_y, max_y);
        vect.z = Random.Range(min_z, max_z);
        return vect;
    }

    public static Vector3 randomVector3(float x, float y, float z)
    {
       return EnemyUtils.randomVector3(x * -1, x, y * -1, y, z * -1, z);
    }

    public static Vector3 randomVector3(float value)
    {
        return EnemyUtils.randomVector3(value * -1, value, value * -1, value, value * -1, value);
    }

}
