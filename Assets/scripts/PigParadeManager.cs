using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PigParadeManager : MonoBehaviour {

    public pig[] parade;

    public GameObject[] pig_positions;

    public GameObject[] pig_prefabs;

    public ReelManager reel_man;
    public ScoreManager score_man;

    public bool pigsMoving;
    public bool pigsBouncing;

    private int[] prizeValues = new int[]        { 100,  200,  300,  500,  600,  700,  1000,  2000,  4000, 10000,  0 };
    private float[] prizeValueOdds = new float[] { .29f, .24f, .22f, .08f, .03f, .05f, .002f, .05f, .015f,  .01f, .05f };

    // Use this for initialization
    void Start () {
        pigsMoving = false;
        pigsBouncing = false;

    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown("p"))
        {
            movePigs();
        }
    }

    public void movePigs()
    {
        pigsMoving = true;
        Destroy(parade[0].gameObject);

        for (int i = 1; i < parade.Length; i++)
        {
            parade[i].walkToNextStage(this);
        }

        for (int j = 0; j < parade.Length - 1; j++)
        {
            parade[j] = parade[j + 1];
        }

        spawnRandomPig();

        StartCoroutine(waitOnWalkingPigs());

    }

    public IEnumerator waitOnWalkingPigs()
    {
        bool pigsStillMoving = true;
        while (pigsStillMoving)
        {
            pigsStillMoving = false;
            for (int i = 1; i < parade.Length; i++)
            {
                if (parade[i].walking)
                {
                    pigsStillMoving = true;
                }
            }
            yield return null;
        }

        pigsMoving = false;
    }

    public void spawnRandomPig()
    {
        GameObject newPig =  pig_prefabs[Random.Range(0, pig_prefabs.Length)];
        newPig.transform.position = new Vector3(pig_positions[pig_positions.Length - 1].transform.position.x, pig_positions[pig_positions.Length - 1].transform.position.y, pig_positions[pig_positions.Length - 1].transform.position.z);

        int value = 100;

        GameObject pig = Instantiate(newPig);
        pig.SetActive(true);
        pig pig_comp = pig.GetComponent<pig>();
        pig_comp.setPosition(8);

        //Normalize our values so we know the range
        float sum = 0.0f;
        for(int i = 0; i<prizeValueOdds.Length; i++)
        {
            sum += prizeValueOdds[i];
        }

        float chosenValue = Random.Range(0.0f, sum);

        //Use that random value to go ahead and pick a prize
        sum = 0.0f;
        for (int i = 0; i < prizeValueOdds.Length; i++)
        {
            sum += prizeValueOdds[i];
            if(chosenValue <= sum)
            {
                value = prizeValues[i];
                break;
            }
        }
        
        pig_comp.setPrize(value);
        parade[8] = pig_comp;
        updatePigs(reel_man.lines_played);
    }

    public void updatePigs(int linesPlayed)
    {
        for(int i=0; i<parade.Length; i++)
        {
            parade[i].updatePrizeIcon(linesPlayed);
        }
    }

    // reelNumber is 1-5 (0 is pig off-screen and 6/7 are newly spawned)
    public void bouncePig(int reelNumber)
    {
        pigsBouncing = true;
        parade[reelNumber].startBouncing();
    }

    public void stopBouncingPigs()
    {
        
        for(int i = 0; i< parade.Length; i++)
        {
            parade[i].stopBouncing();
        }
        pigsBouncing = false;
    }

    public int getPigValue(int reelNumber, int linesPlayed)
    {
        if ((parade[reelNumber].getPrize(linesPlayed) > 1000 && linesPlayed == 3) || (parade[reelNumber].getPrize(linesPlayed) > 900 && linesPlayed == 1))
        {
            if (!score_man.spin2win && reel_man.respins_available == 0)
            {
                FindObjectOfType<LogoSwapper>().swapLogo("gems");
            }
        }
        return parade[reelNumber].getPrize(linesPlayed);
    }

    public bool getPigSpinToWin(int reelNumber)
    {
        return parade[reelNumber].getSpinToWin();
    }
}
