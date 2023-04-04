using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    public KeyCode button;

    private Rigidbody2D body;

    public delegate void OnExplodedEvent(Rocket r, float height);
    public OnExplodedEvent OnExploded;

    public float speed;
    public float rotSpeed;

    public float maxTime;

    private Vector3 velocityForward;
    private Vector3 velocityRight;
    public bool ghost = false;
    private bool hitSth = false;

    int rotDirection = 1;

    float startHeight = 0;
    float currentHeight = 0;

    bool endOfTime = false;

    public float CurrentHeight
    {
        get
        {
            return currentHeight - startHeight;
        }
    }

    void Start()
    {
        startHeight = currentHeight = this.transform.position.y;

        body = GetComponent<Rigidbody2D>();

        if (!ghost)
            StartCoroutine(LifeCycle());
    }

    IEnumerator LifeCycle()
    {
        float time = 0;

        while (time < maxTime)
        {
            time += Time.deltaTime;

            yield return null;
        }

        // TODO explode

        endOfTime = true;

        

        Destroy(this.gameObject);

        yield return null;
    }

    private void OnDestroy()
    {
        // Instantiate explosion
        if (!ghost || hitSth || endOfTime)
        {
            if (!ghost)
            {
                this.GetComponent<AudioSource>().Stop();
                OnExploded.Invoke(this, currentHeight - startHeight);

                this.GetComponent<ScreenWrapBehavior>().DestroyGhosts();
            }

            Instantiate(Resources.Load("Prefabs/Firework") as GameObject, this.transform.position, Quaternion.identity);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        hitSth = true;

        Destroy(this.gameObject);
    }


    // Update is called once per frame
    void Update()
    {

        if (!ghost)
        {
            velocityRight = Vector2.zero;

            currentHeight = this.transform.position.y;

            if (Input.GetKeyUp(button))
            {
                rotDirection *= -1;
            }

            if (Input.GetKey(button))
            {
                // rotate
                velocityRight = this.transform.right * rotDirection * Time.deltaTime * rotSpeed;
            }

            body.velocity = this.transform.up * speed + velocityRight;

            this.transform.up = body.velocity.normalized;
        }

        

        if (this.transform.position.y < 0)
        {
            hitSth = true;
            Destroy(this.gameObject);

        }

        
    }

}
