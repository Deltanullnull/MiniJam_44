using UnityEngine;
using System.Collections;

public class ScreenWrapBehavior : MonoBehaviour
{
    // Whether to use advancedWrapping or not
    public bool advancedWrapping = true;

    //Renderer[] renderers;
    new SpriteRenderer renderer;

    bool isWrappingX = false;
    bool isWrappingY = false;

    // We use ghosts in advanced wrapping to create a nice wrapping illusion
    Transform[] ghosts = new Transform[2];

    float screenWidth;
    float screenHeight;

    void Start()
    {
        //renderers = GetComponentsInChildren<Renderer>();
        renderer = GetComponent<SpriteRenderer>();

        var cam = Camera.main;

        // We need the screen width in world units, relative to the ship.
        // To do this, we transform viewport coordinates of the screen edges to 
        // world coordinates that lie on on the same Z-axis as the player.
        //
        // Viewport coordinates are screen coordinates that go from 0 to 1, ie
        // x = 0 is the coordinate of the left edge of the screen, while,
        // x = 1 is the coordinate of the right edge of the screen.
        // Similarly,
        // y = 0 is the bottom screen edge coordinate, while
        // y = 1 is the top screen edge coordinate.
        //
        // Which gives us this:
        // (0, 0) is the bottom left corner, to
        // (1, 1) is the top right corner.
        var screenBottomLeft = cam.ViewportToWorldPoint(new Vector3(0, 0, transform.position.z));
        var screenTopRight = cam.ViewportToWorldPoint(new Vector3(1, 1, transform.position.z));

        // The width is then equal to difference between the rightmost and leftmost x-coordinates
        screenWidth = screenTopRight.x - screenBottomLeft.x;
        // The height, similar to above is the difference between the topmost and the bottom yycoordinates
        screenHeight = screenTopRight.y - screenBottomLeft.y;

        CreateGhosts();
        
    }

    // Update is called once per frame
    void Update()
    {
        AdvancedScreenWrap();

        PositionGhosts();

    }


    void AdvancedScreenWrap()
    {
        //Debug.Log("Width: " + screenWidth);
        if (!renderer.isVisible && (this.transform.position.x >= screenWidth/2 || this.transform.position.x <= -screenWidth/2))
        //if (!renderer.isVisible)
        {
            SwapObjects();
        }
    }

    public void DestroyGhosts()
    {
        foreach (var ghost in ghosts)
        {
            if (ghost != null)
                Destroy(ghost.gameObject);
        }
    }

    void CreateGhosts()
    {
        for (int i = 0; i < 2; i++)
        {
            // Ghost ships should be a copy of the main ship
            ghosts[i] = Instantiate(transform, Vector3.zero, Quaternion.identity) as Transform;

            ghosts[i].GetComponent<Rocket>().ghost = true;

            // But without the screen wrapping component
            DestroyImmediate(ghosts[i].GetComponent<ScreenWrapBehavior>());
            //DestroyImmediate(ghosts[i].GetComponent<Rocket>());
            DestroyImmediate(ghosts[i].GetComponent<Collider2D>());
            DestroyImmediate(ghosts[i].GetComponent<AudioSource>());
        }

        PositionGhosts();
    }

    void PositionGhosts()
    {
        var ghostPosition = transform.position;

        ghostPosition.x = transform.position.x + screenWidth;
        ghostPosition.y = transform.position.y;
        ghosts[0].position = ghostPosition;

        // Left
        ghostPosition.x = transform.position.x - screenWidth;
        ghostPosition.y = transform.position.y;
        ghosts[1].position = ghostPosition;

        for (int i = 0; i < 2; i++)
        {
            ghosts[i].rotation = transform.rotation;
        }
    }

    void SwapObjects()
    {
        foreach (var ghost in ghosts)
        {
            if (ghost.position.x < screenWidth && ghost.position.x > -screenWidth)
            {

                transform.position = ghost.position;

                break;
            }
        }

    }

}