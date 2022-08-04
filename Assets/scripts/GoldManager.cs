using UnityEngine;

public class GoldManager : MonoBehaviour {

    public SoundManager sound_man;

    public ParticleSystem coins, glows;

    public SpriteRenderer topTray, bottomTray;

    public Sprite topGold, topPurple, bottomGold, bottomPurple;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void startGold()
    {
        coins.Play();
        glows.Play();
        topTray.sprite = topGold;
        bottomTray.sprite = bottomGold;
        sound_man.play500Pig();
    }

    public void endGold()
    {
        coins.Stop();
        glows.Stop();
        topTray.sprite = topPurple;
        bottomTray.sprite = bottomPurple;
        sound_man.winningsLoop.volume = 0.5f;
        sound_man.pigMusic.volume = 0.8f;
    }
}
