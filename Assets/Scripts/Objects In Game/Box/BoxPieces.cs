using UnityEngine;

public class BoxPieces : MonoBehaviour
{
    private float timer;
    private void Awake()
    {
        timer = Random.RandomRange(4, 7);
    }
    void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            Destroy(gameObject);
        }
    }
}
