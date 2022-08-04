using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {

    public AudioSource pigMusic;
    public AudioSource winningsLoop;
    public AudioSource golden;
    public AudioSource reels;


    public AudioClip ding, pig_excited, gem_gleam, reel_stop, reel_wiggle, bet_1, bet_3, fire, lever, bloop;

    public AudioClip[] pigSounds;

    private bool updateWinningCounter, winningStoppable, winningsIncrementing, musicPlaying;
    private float winningCounter, elapsedTime;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown("h"))
        {
            startWinningIncrease();
        }
        if (Input.GetKeyDown("j"))
        {
            endWinningIncrease();
        }

        if (updateWinningCounter)
        {
            winningCounter += Time.deltaTime;
            elapsedTime += Time.deltaTime;
            if(winningCounter >= 0.125f)
            {
                winningStoppable = true;
                winningCounter = elapsedTime % 01.25f;
            }else
            {
                winningStoppable = false;
            }
        }
    }

    public void playPig()
    {
        AudioSource.PlayClipAtPoint(pigSounds[Random.Range(0, pigSounds.Length)], this.transform.position);
    }

    public void play500Pig()
    {
        winningsLoop.volume = 0.1f;
        pigMusic.volume = 0.1f;
        
        golden.Play();
    }

    public void startReelsSpinning()
    {
        reels.Play();
    }

    public void endReelsSpinning()
    {
        reels.Stop();
    }

    public void playReelClick()
    {
        AudioSource.PlayClipAtPoint(reel_stop, this.transform.position, 0.9f);
    }

    public void playFire()
    {
        AudioSource.PlayClipAtPoint(fire, this.transform.position, 0.6f);
    }

    public void wiggleReel()
    {
        AudioSource.PlayClipAtPoint(reel_wiggle, this.transform.position, 0.4f);
    }

    public void Bet1()
    {
        AudioSource.PlayClipAtPoint(bet_1, this.transform.position, 0.5f);
    }

    public void Bet3()
    {
        AudioSource.PlayClipAtPoint(bet_3, this.transform.position, 0.5f);
    }

    public void PullLever()
    {
        AudioSource.PlayClipAtPoint(lever, this.transform.position, 0.5f);
    }

    public void PlayBloop()
    {
        AudioSource.PlayClipAtPoint(bloop, this.transform.position, 0.9f);
    }

    public void startWinningIncrease()
    {
        winningsLoop.Play();
        winningStoppable = false;
        updateWinningCounter = true;
        winningsIncrementing = true;
        elapsedTime = 0.0f;
    }

    public void endWinningIncrease()
    {
        StartCoroutine(stopWinning_co());
    }

    private IEnumerator stopWinning_co()
    {
        if (!winningsIncrementing)
        {
            yield return null;
        }
        else
        {
            while (!winningStoppable)
            {
                yield return null;
            }

            // stop winning loop
            winningsLoop.Stop();
            //play final "bloop"
            AudioSource.PlayClipAtPoint(ding, this.transform.position, 0.6f);
            winningsIncrementing = false;
        }
    }

    public void startPigMusic()
    {
        if (!musicPlaying)
        {
            StartCoroutine(fadeMusic(true, 0.01f));
            musicPlaying = true;
        }
    }

    public void endPigMusic()
    {
        StartCoroutine(fadeMusic(false, 1.5f));

    }

    private IEnumerator fadeMusic(bool fadeIn, float time)
    {
        float startVol = pigMusic.volume;
        float endVol;

        if (fadeIn)
        {
            endVol = 1.0f;
            pigMusic.Play();
        }else
        {
            endVol = 0.0f;
        }

        float elapsedTime = 0.0f;

        while (elapsedTime < time)
        {
            elapsedTime += Time.deltaTime;
            pigMusic.volume = Mathf.Lerp(startVol, endVol, elapsedTime / time);
            yield return null;
        }

        if (!fadeIn)
        {
            pigMusic.Stop();
            pigMusic.volume = 1.0f;
            musicPlaying = false;
        }
    }

}
