using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIDeath : MonoBehaviour
{
    [SerializeField]
    List<DeathList> list;

    float crushTimer = 1f;

    private void OnTriggerEnter(Collider col)
    {
        if (list.Contains(DeathList.Jump))
        {
            if (col.gameObject.tag == "Player")
            {
                col.gameObject.GetComponent<PlayerMovement>().jumpOnEnemy();
                if(list.Contains(DeathList.Crush))
                {
                    CrushBot();
                    
                } else
                {
                    Destroy(this.gameObject);
                }
            }
        }
    }
    void CrushBot()
    {
        this.GetComponent<BoxCollider>().enabled = false;
        this.GetComponent<BasicAI>().enabled = false;
        this.transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y/3,transform.localScale.z);
        this.transform.localPosition -= new Vector3(0, .3f, 0);
        Destroy(gameObject, crushTimer);
    }
    enum DeathList
    {
        Jump,
        Shoot,
        Crush,
        attacked,
        falling
    }
}
