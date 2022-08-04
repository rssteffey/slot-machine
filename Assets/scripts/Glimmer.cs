using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Glimmer : MonoBehaviour {

    public Sprite[] glimmers;

    private SpriteRenderer sr;
    private bool glimmering;

	// Use this for initialization
	void Start () {
        sr = this.GetComponent<SpriteRenderer>();
        glimmering = false;
	}
	
	// Update is called once per frame
	void Update () {
        if (!glimmering)
        {
            glimmering = true;
            sr.sprite = glimmers[Random.Range(0, glimmers.Length)];
            StartCoroutine(glimmerTimeout(Random.Range(6.0f, 90.0f)));
        }
	}

    private IEnumerator glimmerTimeout(float timer)
    {
        float elapsedTime = 0.0f;
        while(elapsedTime < timer)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        yield return glimmer();
    }

    private IEnumerator glimmer()
    {
        float fadeIn = Random.Range(0.1f, 0.7f);
        float elapsedTime = 0.0f;
        Color faded = new Color(1, 1, 1, 0.0f);
        Color visible = new Color(1,1,1, 0.5f);

        //fade in
        while(elapsedTime < fadeIn)
        {
            elapsedTime += Time.deltaTime;
            sr.color = Color.Lerp(faded, Color.white, elapsedTime / fadeIn);
            yield return null;
        }
        
        //fade out
        elapsedTime = 0.0f;
        while (elapsedTime < fadeIn)
        {
            elapsedTime += Time.deltaTime;
            sr.color = Color.Lerp(Color.white, faded, elapsedTime / fadeIn);
            yield return null;
        }

        glimmering = false;
    }
}
