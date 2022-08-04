using UnityEngine;

public class pulse : MonoBehaviour {

	public float speed;
	public float size_factor;

	public float offset = 0;

	private float initial_scale_x;

	// Use this for initialization
	void Start () {
		initial_scale_x = this.transform.localScale.x;
	}
	
	// Update is called once per frame
	void Update () {
		float a = ((Mathf.Sin((Time.time + offset) * speed) + 1) / 2) * size_factor;
		this.transform.localScale = new Vector2(a + initial_scale_x, a + initial_scale_x);
	}
}
