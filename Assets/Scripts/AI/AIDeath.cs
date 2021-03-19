using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIDeath : MonoBehaviour
{
    [SerializeField]
    List<DeathList> list;

    float crushTimer = 1f;

    bool timerStart;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (timerStart)
            crushTimer -= Time.deltaTime;

        if(crushTimer <= 0)
            Destroy(gameObject);
        
    }
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
        if (list.Contains(DeathList.Shoot))
        {

        }
    }
    void CrushBot()
    {
        this.GetComponent<BoxCollider>().enabled = false;
        this.GetComponent<BasicAI>().enabled = false;
        this.transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y/3,transform.localScale.z);
        this.transform.localPosition -= new Vector3(0, .3f, 0);
        timerStart = true;
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
