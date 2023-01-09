using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public List<GameObject> targets;

    private Vector3 camPos;

    // Start is called before the first frame update
    void Start()
    {
        camPos = this.transform.position;

        StartCoroutine(FadeInAudio());
    }

    IEnumerator FadeInAudio()
    {
        var audioSource = this.GetComponent<AudioSource>();

        float targetVolume = 0.2f;
        audioSource.volume = 0f;

        while (audioSource.volume < targetVolume - 0.05f)
        {
            audioSource.volume = Mathf.Lerp(audioSource.volume, targetVolume, Time.deltaTime * 2f);

            yield return null;
        }

        yield return null;
    }

    // Update is called once per frame
    void Update()
    {
        bool found = false;
        //Vector3 topTargetPos = camPos;
        Vector3 topTargetPos = Vector3.zero;

        foreach (GameObject t in targets)
        {
            if (t != null)
            {
                if (t.transform.position.y > topTargetPos.y)
                {
                    topTargetPos = t.transform.position;
                    found = true;
                }
            }

            
        }

        if (found)
        {
            // TODO Lerp to new position

            Vector3 pos = this.transform.position;

            float camZ = pos.z;
            float camX = pos.x;

            pos = topTargetPos;

            pos.x = camX;
            pos.z = camZ;

            camPos = pos;

            pos.y += 2;

            float distance = Vector3.Distance(this.transform.position, pos);

            this.transform.position = Vector3.Lerp(this.transform.position, pos, Time.deltaTime * 50 / distance);

            //this.transform.position = pos;
        }

        

    }
}
