using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pig : MonoBehaviour {

    public Animator pigAnimator;

    public Sprite[] prizeIcons;

    public bool readyToSpin;
    
    public int prizeValue = 0;
    private int value_1, value_3;
    private string prizeString = "";
    private Sprite icon;
    private bool alreadyWon;
    public int paradePosition;

    public bool walking, bouncing, spinToWin;

	// Use this for initialization
	void Start () {
        readyToSpin = true;
        walking = false;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void setPosition(int ind)
    {
        paradePosition = ind;
    }

    public int getPosition()
    {
        return paradePosition;
    }

    public void setPrize(int pv)
    {
        spinToWin = false;

        switch (pv)
        {
            case 100:
                prizeValue = pv;
                value_1 = 100;
                value_3 = 100;
                prizeString = "100";
                icon = prizeIcons[0];
                break;
            case 200:
                prizeValue = pv;
                value_1 = 100;
                value_3 = 200;
                prizeString = "200";
                icon = prizeIcons[1];
                break;
            case 300:
                prizeValue = pv;
                value_1 = 200;
                value_3 = 300;
                prizeString = "300";
                icon = prizeIcons[2];
                break;
            case 500:
                prizeValue = pv;
                value_1 = 200;
                value_3 = 500;
                prizeString = "500";
                icon = prizeIcons[3];
                break;
            case 600:
                prizeValue = pv;
                value_1 = 200;
                value_3 = 600;
                prizeString = "600";
                icon = prizeIcons[4];
                break;
            case 700:
                prizeValue = pv;
                value_1 = 300;
                value_3 = 700;
                prizeString = "700";
                icon = prizeIcons[5];
                break;
            case 1000:
                prizeValue = pv;
                value_1 = 500;
                value_3 = 1000;
                prizeString = "1000";
                icon = prizeIcons[6];
                break;
            case 2000:
                prizeValue = pv;
                value_1 = 1000;
                value_3 = 2000;
                prizeString = "purple_gem";
                icon = prizeIcons[7];
                break;
            case 4000:
                prizeValue = pv;
                value_1 = 1500;
                value_3 = 4000;
                prizeString = "pink_gem";
                icon = prizeIcons[8];
                break;
            case 10000:
                prizeValue = pv;
                value_1 = 3000;
                value_3 = 10000;
                prizeString = "diamond_gem";
                icon = prizeIcons[9];
                break;
            case 0:
                if (Random.Range(0, 2) == 0)
                {
                    prizeValue = pv;
                    prizeString = "bonus";
                    icon = prizeIcons[10];
                }else
                {
                    prizeValue = pv;
                    prizeString = "spinToWin";
                    spinToWin = true;
                    icon = prizeIcons[11];
                }
                break;
            default:
                prizeValue = pv;
                prizeString = "100";
                icon = prizeIcons[0];
                break;
        }

        this.transform.Find("torso").Find("prize").GetComponent<SpriteRenderer>().sprite = icon;
    }

    public void walkToNextStage(PigParadeManager pp_man)
    {
        walking = true;
        StartCoroutine(walk_co(pp_man));
    }

    private IEnumerator walk_co(PigParadeManager pp_man)
    {
        pigAnimator.SetBool("pig_walk", true);
        Transform goingTo = pp_man.pig_positions[paradePosition - 1].transform;
        Transform comingFrom = pp_man.pig_positions[paradePosition].transform;

        float elapsed_time = 0.0f;
        float time_to_take = 2.2f;
        while(elapsed_time < time_to_take)
        {
            elapsed_time += Time.deltaTime;
            this.transform.position = Vector3.Lerp(comingFrom.position, goingTo.position, elapsed_time / time_to_take);
            yield return null;
        }

        paradePosition--;
        pigAnimator.SetBool("pig_walk", false);
        walking = false;

        yield return null;
    }

    public void startBouncing()
    {
        pigAnimator.SetBool("pig_bounce", true);
    }

    public void stopBouncing()
    {
        pigAnimator.SetBool("pig_bounce", false);
    }

    public void sailAway()
    {

    }

    public int getPrize(int linesPlayed)
    {
        if (linesPlayed == 1)
        {
            return value_1;
        }
        else
        {
            return value_3;
        }
    }

    public void updatePrizeIcon(int linesPlayed)
    {
        Sprite icon;
 
        switch (getPrize(linesPlayed))
        {
            case 100:
                icon = prizeIcons[0];
                break;
            case 200:
                icon = prizeIcons[1];
                break;
            case 300:
                icon = prizeIcons[2];
                break;
            case 500:
                icon = prizeIcons[3];
                break;
            case 600:
                icon = prizeIcons[4];
                break;
            case 700:
                icon = prizeIcons[5];
                break;
            case 1000:
                if (linesPlayed == 1)
                {
                    icon = prizeIcons[7];
                } else
                {
                    icon = prizeIcons[6];
                }
                break;
            default:
                return;
        }

        this.transform.Find("torso").Find("prize").GetComponent<SpriteRenderer>().sprite = icon;
    }

    public bool getSpinToWin()
    {
        return spinToWin;
    }
}
