using System.Collections;
using UnityEngine;

public class LogoSwapper : MonoBehaviour {

    public Sprite piggy_parade, spin_to_win, bonus_spins, gems;
    public string current = "logo";

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
       
    }

    public void swapLogo(string swapTo)
    {
        if (swapTo != current)
        {
            current = swapTo;
            StartCoroutine(swapLeft_co(swapTo));
        }
    }

    private IEnumerator swapLeft_co(string swapTo)
    {
        float outTime = 0.7f;

        float elapsedTime = 0.0f;

        Vector3 start = this.gameObject.transform.position;
        Vector3 end = this.gameObject.transform.position - new Vector3(3, 0, 0);

        while(elapsedTime < outTime)
        {
            elapsedTime += Time.deltaTime;
            this.gameObject.transform.position = Vector3.Lerp(start, end, elapsedTime / outTime);
            yield return null;
        }

        if (swapTo == "logo") {
            this.GetComponent<SpriteRenderer>().sprite = piggy_parade;
        } else if (swapTo == "bonus")
        {
            this.GetComponent<SpriteRenderer>().sprite = bonus_spins;
        } else if (swapTo == "spin")
        {
            this.GetComponent<SpriteRenderer>().sprite = spin_to_win;
        } else if (swapTo == "gems")
        {
            this.GetComponent<SpriteRenderer>().sprite = gems;
        }

        elapsedTime = 0.0f;
        while (elapsedTime < outTime)
        {
            elapsedTime += Time.deltaTime;
            this.gameObject.transform.position = Vector3.Lerp(end, start, elapsedTime / outTime);
            yield return null;
        }

        yield return null;
    }
}
