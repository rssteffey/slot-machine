using UnityEngine;

public class Bouncer : MonoBehaviour {

    public SpriteRenderer sr;

    public float height, bounceTime;

    public AnimationCurve upCurve, downCurve;

    private bool upward, bouncing;

    private Vector3 start, end;

    private float elapsedTime;

	// Use this for initialization
	void Start () {
        upward = true;
        elapsedTime = 0.0f;
        setInvisible();
        start = this.transform.position;
        end = this.transform.position + new Vector3(0, height, 0);
	}
	
	// Update is called once per frame
	void Update () {
        if (bouncing)
        {
            if(elapsedTime > bounceTime)
            {
                upward = !upward;
                elapsedTime = 0.0f;
            }

            elapsedTime += Time.deltaTime;

            if (upward)
            {
                this.transform.position = Vector3.Lerp(start, end, upCurve.Evaluate(elapsedTime / bounceTime));
            }else
            {
                this.transform.position = Vector3.Lerp(end, start, downCurve.Evaluate(elapsedTime / bounceTime));
            }
        }
	}

    public void setVisible()
    {
        sr.color = Color.white;
        bouncing = true;
        FindObjectOfType<SoundManager>().PlayBloop();
    }

    public void setInvisible()
    {
        sr.color = new Color(1, 1, 1, 0.0f);
        bouncing = false;
    }
    
}
