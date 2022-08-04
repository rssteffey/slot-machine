using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class ReelManager : MonoBehaviour {

    public Transform reels_center;

    public float reels_offset = 1.0f;

    public ReelItem cherries_m;
    public ReelItem bar_m;
    public ReelItem barbar_m;
    public ReelItem barbarbar_m;
    public ReelItem seven_m;
    public ReelItem sizzlin_7_m;
    public ReelItem coin_m;

    public AnimationCurve stopCurve;
    public AnimationCurve wiggleCurve;

    public ScoreManager score_man;
    public PigParadeManager pig_man;

    public GoldManager gold_manager;
    public UserManager user_manager;
    public SoundManager sound_man;

    public SpriteRenderer gem_inlays, jackpot_inlay;
    public Sprite gems_cheap, gems_expensive, jackpot_cheap, jackpot_expensive;
    public Text lines_field, current_bet_field;

    private reel[] reels = new reel[5];
    private ReelItem[] reel_item_list;

    public int lines_played = 1;
    public int respins_available = 0;
    public bool GAME_PLAYABLE, PLAYER_ACTIVE;

    public Bouncer cash_out_bouncer, login_bouncer;

    //----Config----
    private float start_spin_time = 2.0f;
    private float stop_time_min = 0.3f;
    private float stop_time_max = 1.0f;

	// Use this for initialization
	void Start () {

        reel_item_list = new ReelItem[] { cherries_m, bar_m, barbar_m, barbarbar_m, seven_m, sizzlin_7_m, coin_m };
        InitializeReels();
        GAME_PLAYABLE = true;
        PLAYER_ACTIVE = false;
        lines_played = 1;
    }
	
	// Update is called once per frame
	void Update () {
        
        // if (respins > 0 && play_unlocked) { //Spin automatically }
        if (respins_available > 0 && GAME_PLAYABLE)
        {
            cash_out_bouncer.setInvisible();
            respins_available--;
            if(respins_available == 0)
            {
                FindObjectOfType<LogoSwapper>().swapLogo("logo");
                sound_man.endPigMusic();
            }
            score_man.respins_field.text = respins_available + "";
            playRound();
        }

        if ((Input.GetButtonDown("Spin") || Input.GetButtonDown("Spin2")) && GAME_PLAYABLE && PLAYER_ACTIVE)
        {
            // SOund Effect for Lever
            if (Input.GetButtonDown("Spin2"))
            {
                sound_man.PullLever();
            }

            Debug.Log("Spin");
            if (score_man.SubtractGameCost(lines_played))
            {
                cash_out_bouncer.setInvisible();
                playRound();
            }
        }

        if (Input.GetButtonDown("Bet1") && GAME_PLAYABLE && PLAYER_ACTIVE)
        {
            Debug.Log("1 lines");
            sound_man.Bet1();
            lines_played = 1;
            pig_man.updatePigs(lines_played);
            gem_inlays.sprite = gems_cheap;
            jackpot_inlay.sprite = jackpot_cheap;
            lines_field.text = "1";
            current_bet_field.text = "$0.50";
        }
        if (Input.GetButtonDown("Bet3") && GAME_PLAYABLE && PLAYER_ACTIVE)
        {
            Debug.Log("3 lines");
            sound_man.Bet3();
            lines_played = 3;
            pig_man.updatePigs(lines_played);
            gem_inlays.sprite = gems_expensive;
            jackpot_inlay.sprite = jackpot_expensive;
            lines_field.text = "3";
            current_bet_field.text = "$1.50";
        }
        if (Input.GetButtonDown("LogIn") && GAME_PLAYABLE && !PLAYER_ACTIVE)
        {
            user_manager.logIn();
            login_bouncer.setInvisible();
        }
        if (Input.GetButtonDown("CashOut") && GAME_PLAYABLE && PLAYER_ACTIVE)
        {
            cash_out_bouncer.setInvisible();
            user_manager.logOut();
            PLAYER_ACTIVE = false;
            //Start timer to prompt new players
            StartCoroutine(promotionalTimeout());
        }
    }

    private IEnumerator promotionalTimeout()
    {
        yield return null;
        float elapTime = 0.0f;
        while(elapTime < 30.0f)
        {
            if (PLAYER_ACTIVE) { break; }
            elapTime += Time.deltaTime;
            yield return null;
        }
        while (!PLAYER_ACTIVE) {

            login_bouncer.setVisible();

            elapTime = 0.0f;
            while (elapTime < 14.0f)
            {
                if (PLAYER_ACTIVE) { break; }
                elapTime += Time.deltaTime;
                yield return null;
            }

            login_bouncer.setInvisible();

            elapTime = 0.0f;
            while (elapTime < 60.0f)
            {
                if (PLAYER_ACTIVE) { break; }
                elapTime += Time.deltaTime;
                yield return null;
            }
        }
    }

    void InitializeReels()
    {
        //Spawn 5 reels
        for(int i = 0; i < 5; i++)
        {
            //reel newReel
            GameObject reel = new GameObject("Reel_" + (i + 1));
            reel.transform.position = new Vector3(reels_center.position.x * (reels_offset * -1 * (i - 2)), reels_center.position.y, reels_center.position.z);
            reel.AddComponent<reel>();
            reels[i] = reel.GetComponent<reel>();
            reels[i].reelNumber = i + 1;
            reels[i].icon_spacing = 0.7f;
            ReelItem[] newReels = getProperReelSymbols(i);
            reels[i].setReelSymbols(newReels);
            reels[i].stopCurve = stopCurve;
        }

    }

    ReelItem[] getProperReelSymbols(int index)
    {
        // Hardcoded reels (rather than doing the math to figure out our own odds, blindly copying these patterns from an existing machine)
        string[][] reel_groups = new string[][] {
            new string[] { "bar3", "coin", "seven", "seven", "bar", "bar", "seven", "bar3", "bar2", "sizzlin_seven", "seven"       ,"bar3", "seven", "seven", "bar", "bar", "seven", "bar3", "bar2", "sizzlin_seven", "seven" },
            new string[] { "sizzlin_seven", "seven", "bar", "bar2", "sizzlin_seven", "coin", "seven", "bar2", "bar3", "bar2", "bar2"       ,"sizzlin_seven", "seven", "bar", "bar2", "sizzlin_seven", "seven", "bar2", "bar3", "bar2", "bar2" },
            new string[] { "coin", "bar3", "bar", "bar3", "bar3", "seven", "bar2", "sizzlin_seven", "bar3", "seven", "sizzlin_seven"      ,"bar3", "bar", "bar3", "bar3", "seven", "bar2", "sizzlin_seven", "bar3", "seven", "sizzlin_seven" },
            new string[] { "bar", "seven", "seven", "sizzlin_seven", "bar3", "bar2", "seven", "sizzlin_seven", "coin", "bar2", "seven"         ,"bar", "seven", "seven", "sizzlin_seven", "bar3", "bar2", "seven", "sizzlin_seven", "bar2", "seven" },
            new string[] { "sizzlin_seven", "sizzlin_seven", "seven", "bar3", "bar", "sizzlin_seven", "seven", "coin", "bar3", "sizzlin_seven", "bar2"          ,"sizzlin_seven", "sizzlin_seven", "seven", "bar3", "bar", "sizzlin_seven", "seven", "bar3", "sizzlin_seven", "bar2" }
        };

        ReelItem[] items = new ReelItem[reel_groups[index].Length];
        for(int i =0; i< reel_groups[index].Length; i++)
        {
            for (int j = 0; j < reel_item_list.Length; j++) {
                if(reel_item_list[j].getName() == reel_groups[index][i])
                {
                    items[i] = reel_item_list[j];
                    break;
                }
            }
        }

        return items;
    }

    // Spin all the wheels, stop sequentially, grab results, 
    public void playRound()
    {
        //LOCK PLAY
        GAME_PLAYABLE = false;

        StartCoroutine(RoundCoroutine(new List<int>()));
    }

    private IEnumerator RoundCoroutine(List<int> stoppedReels, bool firstSpin = true)
    {
        //start spinning all reels
        for (int i = 0; i < reels.Length; i++)
        {
            if (!stoppedReels.Contains(i)) // Spin2Win Check
                {
                reels[i].spinWheel();
            }
        }

        sound_man.startReelsSpinning();

        float time = 0.0f;

        //one by one, send stop
        //Initial spinning time
        while(time < start_spin_time)
        {
            time += Time.deltaTime;
            yield return null;
        }
        //Stop each wheel sequentially
        for(int i = 0; i<5; i++)
        {
            if (!stoppedReels.Contains(i)) // Spin2Win Check
            {
                time = 0.0f;
                float reel_stop = Random.Range(stop_time_min, stop_time_max);
                while (time < reel_stop)
                {
                    time += Time.deltaTime;
                    yield return null;
                }
                reels[i].stopWheel();
                sound_man.playReelClick();
            }
        }
        sound_man.endReelsSpinning();

        //get results, check pig coins for respin pigs up top
        bool reelsDone = false;
        while (!reelsDone)
        {
            reelsDone = true;
            for (int i = 0; i < 5; i++)
            {
               if (!reels[i].isStopped)
                {
                    reelsDone = false;
                }
            }
            yield return null;
        }
        

        //Log reel symbols into our comparison matrix
        ReelItem[,] symbolArray = new ReelItem[5,3];
        for (int i = 0; i < 5; i++)
        {
            symbolArray[i, 0] = reels[i].getActiveItems()[0];
            symbolArray[i, 1] = reels[i].getActiveItems()[1];
            symbolArray[i, 2] = reels[i].getActiveItems()[2];
        }

        //Only count score if we aren't in a Spin2Win Situation, or if so,on the first spin only
        if (firstSpin)
        {
            // Score main reel symbols
            score_man.ScoreReels(symbolArray);
        }

        //tally pig coins 
        score_man.checkCoins(symbolArray);

        //Exit Spin2Win if needed
        if (score_man.spin2win || score_man.endSpin2Win)
        {
            if (score_man.endSpin2Win)
            {
                score_man.endSpin2Win = false;
            }
            yield break;
        }

        //Wait until the pig coins are settled
        while (!score_man.coinCheckDone || !score_man.scoreCheckDone)
        {
            yield return null;
        }


        //Adjust credits, payout, jackpot, launch gold fountain if need be


        while (pig_man.pigsBouncing)
        {
            yield return null;
        }
        if (score_man.animatingScore)
        {
            while (score_man.animatingScore)
            {
                yield return null;
            }
            yield return new WaitForSeconds(1.0f);
        }else
        {
            yield return new WaitForSeconds(1.0f);
        }

        gold_manager.endGold();
        pig_man.movePigs();
        score_man.animateUpdateCredit();

        //Swap back the logo if Gem rush
        if(FindObjectOfType<LogoSwapper>().current == "gems")
        {
            FindObjectOfType<LogoSwapper>().swapLogo("logo");
        }
        while (pig_man.pigsMoving)
        {
            yield return null;
        }
        if (score_man.animatingScore)
        {
            while (score_man.animatingScore)
            {
                yield return null;
            }
            yield return new WaitForSeconds(1.0f);
        }


        //CHECK ALL REELS FOR ACTIVE ANIMATIONS LOCK (wiggling, flashing)
        //UNLOCK PLAY
        GAME_PLAYABLE = true;

        //Auto cashout for zero credit
        if (score_man.getCredit() == 0)
        {
            user_manager.logOut();
        }else if (score_man.getCredit() < 50)
        {
            yield return new WaitForSeconds(10.0f);
            if (PLAYER_ACTIVE && respins_available == 0)
            {
                cash_out_bouncer.setVisible();
            }
        }

        yield return null;
    }

    public IEnumerator endRound_co()
    {
        while (score_man.animatingScore)
        {
            yield return null;
        }
        yield return new WaitForSeconds(2.0f);
        gold_manager.endGold();
        pig_man.movePigs();
        score_man.animateUpdateCredit();
        while (pig_man.pigsMoving || score_man.animatingScore)
        {
            yield return null;
        }


        //CHECK ALL REELS FOR ACTIVE ANIMATIONS LOCK (wiggling, flashing)
        //UNLOCK PLAY
        GAME_PLAYABLE = true;
    }


    
    public void playSpin2WinRound(List<int> stoppedReels, bool firstSpin)
    {
        StartCoroutine(RoundCoroutine(stoppedReels, firstSpin));
    }


    public void wiggleReel(int reel_index)
    {
        reels[reel_index].wiggle(wiggleCurve);
        sound_man.wiggleReel();
    }

    public void addRespins(int respinCount)
    {
        respins_available += respinCount;
        score_man.respins_field.text = respins_available + "";
    }

    public int getRespins()
    {
        return respins_available;
    }

    public void flashIcon(SpriteRenderer sr, int num_flashes = 5)
    {
        float flash_time = 0.7f;

        StartCoroutine(flash_co(num_flashes, flash_time, sr));
    }

    public IEnumerator flash_co(int count, float time, SpriteRenderer sr)
    {
        for (int i = 0; i < count; i++)
        {
            float elapsedTime = 0.0f;

            //Dim down
            while (elapsedTime < 0.05f)
            {
                elapsedTime += Time.deltaTime;
                sr.color = Color.Lerp(Color.white, new Color(1, 1, 1, 0.5f), elapsedTime / 0.05f);
                yield return null;
            }
            elapsedTime = 0.0f;
            //Stay dim for length
            while (elapsedTime < time)
            {
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            elapsedTime = 0.0f;
            //Un-dim
            while (elapsedTime < 0.05f)
            {
                elapsedTime += Time.deltaTime;
                sr.color = Color.Lerp(new Color(1, 1, 1, 0.5f), Color.white, elapsedTime / 0.05f);
                yield return null;
            }
            elapsedTime = 0.0f;
            //Stay bright for length
            while (elapsedTime < time)
            {
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }

        score_man.scoreCheckDone = true;
    }
}
