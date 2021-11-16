using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plopmann : Enemy, IRespawnable
{
    public void reset_data()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    bool on_the_ground()
    {
        Vector3 end = this.transform.position;
        end.y -= 1;
        return Physics.Linecast(this.transform.position, end);
    }
}
