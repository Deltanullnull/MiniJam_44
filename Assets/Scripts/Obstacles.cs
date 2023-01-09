using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacles : MonoBehaviour
{
    bool isHit = false;

    // Start is called before the first frame update
    void Start()
    {
        // TODO set random force

        Respawn();
    }

    public void Respawn()
    {
        float velX = Random.Range(4, 8);
        int xDir = Random.Range(0, 2) * 2 - 1;

        // TODO depending on direction, spawn left/right

        float posX = -10;


        this.GetComponent<Rigidbody2D>().velocity = new Vector2(velX * xDir, 0);

        if (xDir < 0) // facing left
        {
            posX = 10;
            this.transform.localScale = new Vector3(-1, 1, 1);
        }
        else
        {
            this.transform.localScale = new Vector3(1, 1, 1);
        }

        Vector3 pos = this.transform.position;
        pos.x = posX;

        this.transform.position = pos;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        isHit = true;
        Destroy(this.gameObject);
    }

    private void OnDestroy()
    {
        if (isHit)
        {
            GameObject exp = Instantiate(Resources.Load("Prefabs/Explosion") as GameObject, this.transform.position, Quaternion.identity);
            Destroy(exp, 0.5f);


            GameObject feathers = Instantiate(Resources.Load("Prefabs/Feathers") as GameObject, this.transform.position, Quaternion.identity);
            Destroy(feathers, 2f);

            Destroy(this.gameObject);
        }

        
    }

}
