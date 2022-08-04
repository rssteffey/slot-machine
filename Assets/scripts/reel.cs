using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class reel : MonoBehaviour {

    private ReelItem[] reel_symbols;
    private ReelItem blankItem;
    private ReelItem[] activeSymbols = new ReelItem[7]; //only middle 3 are sent as valid checks
    private GameObject[] active_objects;

    private GameObject spotA, spot1, spot2, spot3, spot4, spot5, spotB; //Positions to move icons through (A and B are off screen for animation purposes)
    private GameObject[] spots;

    // Misc
    private bool emptySpawn=false;
    public bool isStopping, isStopped;
    public float reelNumber = 0;
    public AnimationCurve stopCurve;
    private int current_spawn = 0;

    //----Config----
    public float icon_spacing = 1.0f;
    public float icon_rotation = 5.0f;
    private int win_light_flashes = 5;  //
    private float spin_speed = 0.04f;
    private float stop_speed = 0.17f;
    private int wiggle_count = 7;
    private float wiggle_time = 0.14f;


    // Use this for initialization
    void Start () {
        spots = new GameObject[] { spotA, spot1, spot2, spot3, spot4, spot5, spotB };
        active_objects = new GameObject[7];
        newWheel();
   
    }
	
	// Update is called once per frame
	void Update () {

	}

    public void setReelSymbols(ReelItem[] items)
    {
        reel_symbols = items;
    }

    public ReelItem[] getActiveItems()
    {
        ReelItem[] middleThree = new ReelItem[3];
        middleThree[0] = activeSymbols[2];
        middleThree[1] = activeSymbols[3];
        middleThree[2] = activeSymbols[4];
        return middleThree;
    }

    void newWheel()
    {
        //Initialize Reel Item Array (Alternating every other with a null item)
        blankItem = new ReelItem();
        blankItem.setBuffer(0);
        blankItem.setName("BLANK");

        //Create empties in world space as targetting positions
        for(int i = 0; i< spots.Length; i++)
        {
            spots[i] = new GameObject("reel" + reelNumber + "-spot" + i);
            spots[i].transform.parent = this.gameObject.transform;
            spots[i].transform.position = new Vector3(this.gameObject.transform.position.x, this.gameObject.transform.position.y, this.gameObject.transform.position.z);
            spots[i].transform.Translate(0.0f, icon_spacing * (i - 3) * -1, 0.0f);
            spots[i].transform.Rotate(Vector3.right, -1.0f * icon_rotation * (i-3));

            if (i < spots.Length - 1)
            {
                spawnSymbol(i);
            }else
            {
                emptySpawn = !emptySpawn;
            }
        }

        isStopped = true;   //wheel unmoving
        isStopping = false;
    }

    // Start the wheel spinning (coroutine)
    public void spinWheel()
    {
        isStopped = false;
        isStopping = false;

        
        StartCoroutine(spinCoroutine());
    }

    public IEnumerator spinCoroutine()
    {
        // spawn new reel item
        // spawnSymbol(0);

        //Repeat the one unit movement until told to stop
        while (!isStopping)
        {
            //use blur images
            for (int i = 0; i < active_objects.Length; i++)
            {
                if (activeSymbols[i] != null && activeSymbols[i].getName() != "BLANK")
                {
                    active_objects[i].GetComponent<SpriteRenderer>().sprite = activeSymbols[i].getBlurImage();
                }
            }

            //One unit movement, over the course of spin_speed
            float time = 0.0f;
            while (time < spin_speed)
            {
                for(int i = 0; i < (activeSymbols.Length - 1); i++)
                {
                    active_objects[i].transform.position = Vector3.Lerp(spots[i].transform.position, spots[i + 1].transform.position, time / spin_speed);
                }


                time = time + Time.deltaTime; //advance time
                yield return null;
            }

            //Delete the bottommost icon
            Destroy(active_objects[6]);

            //Advance all data symbols by one
            for (int i = 6; i > 0; i--)
            {
                active_objects[i] = active_objects[i - 1];
                activeSymbols[i] = activeSymbols[i - 1];
            }

            //Spawn new symbol
            spawnSymbol(0);
            
        }

        //Final advancement
        while(isStopping)
        {
            //use normal images
            for (int i = 0; i < active_objects.Length; i++)
            {
                if (activeSymbols[i] != null && activeSymbols[i].getName() != "BLANK")
                {
                    active_objects[i].GetComponent<SpriteRenderer>().sprite = activeSymbols[i].getImage();
                }
            }

            //One unit movement, over the CURVED course of spin_speed
            float time = 0.0f;
            float percentage = 0.0f;
            while (time < (stop_speed))
            {
                percentage = stopCurve.Evaluate(time / stop_speed);
                for (int i = 0; i < (activeSymbols.Length - 1); i++)
                {
                    active_objects[i].transform.position = Vector3.Lerp(spots[i].transform.position, spots[i + 1].transform.position, percentage);
                }


                time = time + Time.deltaTime; //advance time
                yield return null;
            }


            //Delete the bottommost icon
            Destroy(active_objects[6]);

            //Advance all data symbols by one
            for (int i = 6; i > 0; i--)
            {
                active_objects[i] = active_objects[i - 1];
                activeSymbols[i] = activeSymbols[i - 1];
            }



            //Spawn new symbol
            spawnSymbol(0);


            isStopping = false;
        }
        
        isStopped = true;
        yield return null;
    }

    public void wiggle(AnimationCurve wiggle_curve)
    {
        StartCoroutine(wiggleRoutine(wiggle_curve));
    }

    private IEnumerator wiggleRoutine(AnimationCurve wiggle_curve)
    {
        for (int wiggleCount = 0; wiggleCount < wiggle_count; wiggleCount++)
        {
            //wiggle up
            //One unit movement, over the CURVED course of spin_speed
            float time = 0.0f;
            float percentage = 0.0f;
            while (time < wiggle_time)
            {
                percentage = (wiggle_curve.Evaluate(time / wiggle_time));
                for (int i = 1; i < (activeSymbols.Length - 1); i++)
                {
                    active_objects[i].transform.position = Vector3.Lerp(spots[i].transform.position, spots[i].transform.position + (spots[i-1].transform.position - spots[i].transform.position) / 2, percentage);
                }


                time = time + Time.deltaTime; //advance time
                yield return null;
            }

            //wiggle back
            time = 0.0f;
            percentage = 0.0f;
            while (time < wiggle_time)
            {
                percentage = wiggle_curve.Evaluate(time / wiggle_time);
                for (int i = 1; i < (activeSymbols.Length - 1); i++)
                {
                    active_objects[i].transform.position = Vector3.Lerp(spots[i].transform.position + (spots[i - 1].transform.position - spots[i].transform.position) / 2, spots[i].transform.position, percentage);
                }


                time = time + Time.deltaTime; //advance time
                yield return null;
            }

        }
        yield return null;
    }

    private void spawnSymbol(int indexVal)
    {
        ReelItem newOne;

        if (emptySpawn)
        {
            //SPAWN blankItem
            newOne = new ReelItem();
            newOne.setImage(blankItem.getImage());
            newOne.setBlurImage(blankItem.getBlurImage());
            newOne.setName(blankItem.getName());
            newOne.setOdds(blankItem.getOdds());
            newOne.setBuffer(blankItem.getBuffer());
        }
        else
        {
            // Logic to figure out a random spawn
            /*
            int currIndex = 0;
            float currVal = 0.0f;
            float rand = Random.Range(0.0f, 100.0f);
            do
            {
                currVal += reel_symbols[currIndex].getIntegerOdds();
                currIndex++;
            } while (currVal < rand && reel_symbols.Length > currIndex - 1);
            */

            //Random selection was last index before increasing

            current_spawn++;
            if(current_spawn >= reel_symbols.Length)
            {
                current_spawn = 0;
            }

            newOne = new ReelItem();
            newOne.setImage(reel_symbols[current_spawn].getImage());
            newOne.setBlurImage(reel_symbols[current_spawn].getBlurImage());
            newOne.setName(reel_symbols[current_spawn].getName());
            newOne.setOdds(reel_symbols[current_spawn].getOdds());
            newOne.setBuffer(reel_symbols[current_spawn].getBuffer());
        }
            
        //SPAWN reel_symbols[currIndex - 1];
            
            
        activeSymbols[indexVal] = newOne;

        GameObject unityForm = new GameObject(newOne.getName());
        unityForm.transform.parent = this.gameObject.transform;
        if (!emptySpawn)
        {
            SpriteRenderer sr = unityForm.AddComponent<SpriteRenderer>();
            sr.sprite = newOne.getImage();
            newOne.spriteRenderer = sr;
        }

        // Move to the appropriate spot on the wheel
        unityForm.transform.position = new Vector3( spots[indexVal].transform.position.x, spots[indexVal].transform.position.y, spots[indexVal].transform.position.z);
        unityForm.transform.rotation = new Quaternion(spots[indexVal].transform.rotation.x, spots[indexVal].transform.rotation.y, spots[indexVal].transform.rotation.z, spots[indexVal].transform.rotation.w);
        active_objects[indexVal] = unityForm;
        
        //Flop the blank space next spawn
        emptySpawn = !emptySpawn;
    }

    // Stop the wheel, selecting a random icon array
    public void stopWheel()
    {
        isStopping = true;
    }

    // Flash the indicated icon 5 times
    public void lightUpSymbol(int symbol_index)
    {

    }
}
