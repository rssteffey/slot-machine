using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour {

    public int bar_val, bar2_val, bar3_val, seven_val, sizzlin_val;

    public Text win_field, credit_field, respins_field;

    public ReelManager reel_man;
    public PigParadeManager pig_man;

    public GoldManager gold_manager;

    public UserManager user_manager;
    public SoundManager sound_man;

    // --- Config ----
    private int respinCount = 3;


    private int winnings, credit, jackpot;

    private List<int> coinReels;

    private AudioSource coinSound;
    public bool coinCheckDone, reelScoreDone, bonusFound, animatingScore, scoreCheckDone, spin2win, firstPass, endSpin2Win;

    private bool awarded_seven, awarded_sizzlin, awarded_bar, awarded_bar2, awarded_bar3;

    private int reelWinnings;

    // Use this for initialization
    void Start () {
        winnings = 0;
        credit = 0;
        bonusFound = false;
        spin2win = false;
        endSpin2Win = false;
        animatingScore = false;
        scoreCheckDone = true;
        firstPass = true;

        coinReels = new List<int>();

        jackpot = 1000000;

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public bool SubtractGameCost(int linesPlayed)
    {
        int cost;

        if(linesPlayed == 3)
        {
            cost = 150;
        }
        else
        {
            cost = 50;
        }

        if(credit < cost)
        {
            return false;
        }
        
        credit -= cost;
        instantUpdateCredit(credit);
        return true;
    }

    public void ScoreReels(ReelItem[,] values)
    {

        awarded_bar = false;
        awarded_bar2 = false;
        awarded_bar3 = false;
        awarded_seven = false;
        awarded_sizzlin = false;

        scoreCheckDone = false;
        int[,,] combos_5;
        int[,,] combos_3;
        int[,,] rainbows;

        bool[,] icon_flashes = new bool[5, 3] {
            { false, false, false},
            { false, false, false},
            { false, false, false},
            { false, false, false},
            { false, false, false}
        };

        if (reel_man.lines_played == 3) { // Max bet
            combos_5 = new int[,,]
            {
                {
                    {0,0}, {1,0}, {2,0}, {3,0}, {4,0} // Across the Top
                },
                {
                    {0,1}, {1,1}, {2,1}, {3,1}, {4,1} // Across the Middle
                },
                {
                    {0,2}, {1,2}, {2,2}, {3,2}, {4,2} // Across the Bottom
                },
            };

            combos_3 = new int[,,]
            {
                {
                    {0,0}, {1,0}, {2,0} // Across the Top Left
                },
                {
                    {1,0}, {2,0}, {3,0} // Across the Top Center
                },
                {
                    {2,0}, {3,0}, {4,0} // Across the Top Right
                },
                {
                    {0,1}, {1,1}, {2,1} // Across the Middle Left
                },
                {
                    {1,1}, {2,1}, {3,1} // Across the Middle Center
                },
                {
                    {2,1}, {3,1}, {4,1} // Across the Middle Right
                },
                {
                    {0,2}, {1,2}, {2,2} // Across the Bottom Left
                },
                {
                    {1,2}, {2,2}, {3,2} // Across the Bottom Center
                },
                {
                    {2,2}, {3,2}, {4,2} // Across the Bottom Right
                },
                {
                    {0,0}, {1,1}, {2,2} // Down Diagonal 1
                },
                {
                    {1,0}, {2,1}, {3,2} // Down Diagonal 2
                },
                {
                    {2,0}, {3,1}, {4,2} // Down Diagonal 3
                },
                {
                    {2,0}, {1,1}, {0,2} // Up Diagonal 1
                },
                {
                    {3,0}, {2,1}, {1,2} // Up Diagonal 2
                },
                {
                    {4,0}, {3,1}, {2,2} // Up Diagonal 3
                },
            };

            rainbows = new int[,,]
            {
                {
                    {0,1}, {1,0}, {2,1} // Up Rainbow Top Left
                },
                {
                    {0,2}, {1,1}, {2,2} // Up Rainbow Bottom Left
                },
                {
                    {1,1}, {2,0}, {3,1} // Up Rainbow Top Center
                },
                {
                    {1,2}, {2,1}, {3,2} // Up Rainbow Bottom Center
                },
                {
                    {2,1}, {3,0}, {4,1} // Up Rainbow Top Right
                },
                {
                    {2,2}, {3,1}, {4,2} // Up Rainbow Bottom Right
                },
                  // Down Rainbows
                {
                    {0,0}, {1,1}, {2,0} // Down Rainbow Top Left
                },
                {
                    {0,1}, {1,2}, {2,1} // Down Rainbow Bottom Left
                },
                {
                    {1,0}, {2,1}, {3,0} // Down Rainbow Top Center
                },
                {
                    {1,1}, {2,2}, {3,1} // Down Rainbow Bottom Center
                },
                {
                    {2,0}, {3,1}, {4,0} // Down Rainbow Top Right
                },
                {
                    {2,1}, {3,2}, {4,1} // Down Rainbow Bottom Right
                }

            };
        }
        else // Only the middle line
        {
            rainbows = new int[0,0,0];

            combos_3 = new int[,,]
            {
                {
                    {0,1}, {1,1}, {2,1} // Across the Middle Left
                },
                {
                    {1,1}, {2,1}, {3,1} // Across the Middle Center
                },
                {
                    {2,1}, {3,1}, {4,1} // Across the Middle Right
                }
            };

            combos_5 = new int[,,]
            {
                {
                    {0,1}, {1,1}, {2,1}, {3,1}, {4,1} // Across the Middle
                }
            };
        }

        int reel, slot;
        int tempWinnings;
        int totalWinnings = 0;

        //----- FIVE PATTERNS ------
        for (int i = 0; i < combos_5.GetLength(0); i++) // Each possible combo region
        {
            tempWinnings = 0;
            string[] names = new string[5];
            for (int j = 0; j < combos_5.GetLength(1); j++) // each of the 3 in the region
            {
                reel = combos_5[i, j, 0];
                slot = combos_5[i, j, 1];

                names[j] = values[reel, slot].getName();
            }

            tempWinnings = check5(names[0], names[1], names[2], names[3], names[4]);

            //Successful check
            if (tempWinnings > 0)
            {
                totalWinnings += tempWinnings;  //add score to total

                for (int j = 0; j < combos_5.GetLength(1); j++) // each of the 3 in the region
                {
                    reel = combos_5[i, j, 0];
                    slot = combos_5[i, j, 1];

                    icon_flashes[reel, slot] = true;
                }
            }
        }

        //---- THREE PATTERNS -----
        for (int i = 0; i < combos_3.GetLength(0); i++) // Each possible combo region
        {
            tempWinnings = 0;
            string[] names = new string[3];
            for(int j=0; j< combos_3.GetLength(1); j++) // each of the 3 in the region
            {
                reel = combos_3[i, j, 0];
                slot = combos_3[i, j, 1];

                names[j] = values[reel, slot].getName();
            }

            tempWinnings = check3(names[0], names[1], names[2]);

            //Successful check
            if(tempWinnings > 0)
            {
                totalWinnings += tempWinnings;  //add score to total

                for (int j = 0; j < combos_3.GetLength(1); j++) // each of the 3 in the region
                {
                    reel = combos_3[i, j, 0];
                    slot = combos_3[i, j, 1];

                    icon_flashes[reel, slot] = true;
                }
            }
        }

        //---- RAINBOW PATTERNS -----
        for (int i = 0; i < rainbows.GetLength(0); i++) // Each possible combo region
        {
            tempWinnings = 0;
            string[] names = new string[3];
            for (int j = 0; j < rainbows.GetLength(1); j++) // each of the 3 in the region
            {
                reel = rainbows[i, j, 0];
                slot = rainbows[i, j, 1];

                names[j] = values[reel, slot].getName();
            }

            tempWinnings = checkRainbow(names[0], names[1], names[2]);

            //Successful check
            if (tempWinnings > 0)
            {
                totalWinnings += tempWinnings;  //add score to total

                for (int j = 0; j < rainbows.GetLength(1); j++) // each of the 3 in the region
                {
                    reel = rainbows[i, j, 0];
                    slot = rainbows[i, j, 1];

                    icon_flashes[reel, slot] = true;
                }
            }
        }

        int num_flashes = 3;

        // Show the winnings going in
        if (totalWinnings > 0)
        {
            //animateUpdateWinnings(totalWinnings);
            reelWinnings = totalWinnings;

            if (totalWinnings < 100)
            {
                num_flashes = 3;
            }
            else if (totalWinnings <= 400)
            {
                num_flashes = 4;
            }
            else
            {
                num_flashes = 5;
            }

        } else
        {
            scoreCheckDone = true;
        }

        

        

        //Flash appropriate icons
        for(int i = 0; i<icon_flashes.GetLength(0); i++)
        {
            for(int j = 0; j < icon_flashes.GetLength(1); j++)
            {
                if (icon_flashes[i, j])
                {
                    reel_man.flashIcon(values[i, j].spriteRenderer, num_flashes);
                }
            }
        }
        
    }

    private int checkRainbow(string item1, string item2, string item3)
    {
        int sevens = 77;
        int bars = 55;
        int bar2s = 80;
        int bar3s = 100;
        int sizzlins = 177;

        int bar_ascend = 50;
        int bar_any_order = 10;
        int sevens_any_order = 21;

        List<string> misc_sevens = new List<string>() {"seven", "sizzlin_seven"};
        List<string> misc_bars = new List<string>() { "bar", "bar2", "bar3" };

        //Match 3
        if (item1 == item2 && item2 == item3)
        {
            switch (item1)
            {
                case "seven":
                    if (awarded_seven)
                    {
                        break;
                    }
                    else
                    {
                        awarded_seven = true;
                        return sevens;
                    }
                case "bar":
                    if (awarded_bar)
                    {
                        break;
                    }
                    else
                    {
                        awarded_bar = true;
                        return bars;
                    }
                case "bar2":
                    if (awarded_bar2)
                    {
                        break;
                    }
                    else
                    {
                        awarded_bar2 = true;
                        return bar2s;
                    }
                case "bar3":
                    if (awarded_bar3)
                    {
                        break;
                    }
                    else
                    {
                        awarded_bar3 = true;
                        return bar3s;
                    }
                case "sizzlin_seven":
                    if (awarded_sizzlin)
                    {
                        break;
                    }
                    else
                    {
                        awarded_sizzlin = true;
                        sound_man.playFire();
                        return sizzlins;
                    }
            }
        }

        // Any 3 sevens
      //  if (misc_sevens.Contains(item1) && misc_sevens.Contains(item2) && misc_sevens.Contains(item3))
      //  {
      //      return sevens_any_order;
      //  }

        // sets of bars in ascending order
        if(item1 == "bar" && item2 == "bar2" && item3== "bar3")
        {
            return bar_ascend;
        }

        // Any bars, any order
        if(misc_bars.Contains(item1) && misc_bars.Contains(item2) && misc_bars.Contains(item3))
        {
            return bar_any_order;
        }

        return 0;
    }

    private int check3(string item1, string item2, string item3)
    {
        int sevens = 177;
        int bars = 100;
        int bar2s = 125;
        int bar3s = 150;
        int sizzlins = 210;

        int bar_ascend = 123;
        int bar_any_order = 15;
        int sevens_any_order = 21;

        List<string> misc_sevens = new List<string>() { "seven", "sizzlin_seven" };
        List<string> misc_bars = new List<string>() { "bar", "bar2", "bar3" };

        //Match 3
        if (item1 == item2 && item2 == item3)
        {
            switch (item1)
            {
                case "seven":
                    if (awarded_seven)
                    {
                        break;
                    }
                    else
                    {
                        awarded_seven = true;
                        return sevens;
                    }
                case "bar":
                    if (awarded_bar)
                    {
                        break;
                    }
                    else
                    {
                        awarded_bar = true;
                        return bars;
                    }
                case "bar2":
                    if (awarded_bar2)
                    {
                        break;
                    }
                    else
                    {
                        awarded_bar2 = true;
                        return bar2s;
                    }
                case "bar3":
                    if (awarded_bar3)
                    {
                        break;
                    }
                    else
                    {
                        awarded_bar3 = true;
                        return bar3s;
                    }
                case "sizzlin_seven":
                    if (awarded_sizzlin)
                    {
                        break;
                    }
                    else
                    {
                        awarded_sizzlin = true;
                        sound_man.playFire();
                        return sizzlins;
                    }
            }
        }

        // Any 3 sevens
        //if (misc_sevens.Contains(item1) && misc_sevens.Contains(item2) && misc_sevens.Contains(item3))
       // {
       //     return sevens_any_order;
       // }

        // sets of bars in ascending order
        if (item1 == "bar" && item2 == "bar2" && item3 == "bar3")
        {
            return bar_ascend;
        }

        // Any bars, any order
        if (misc_bars.Contains(item1) && misc_bars.Contains(item2) && misc_bars.Contains(item3))
        {
            return bar_any_order;
        }

        return 0;
    }

    private int check5(string item1, string item2, string item3, string item4, string item5)
    {
        int sevens = 777;
        int bars = 500;
        int bar2s = 750;
        int bar3s = 1000;
        int sizzlins = 2100;
        
        int bar_any_order = 100;
        int sevens_any_order = 210;

        List<string> misc_sevens = new List<string>() { "seven", "sizzlin_seven" };
        List<string> misc_bars = new List<string>() { "bar", "bar2", "bar3" };

        //Match 3
        if (item1 == item2 && item2 == item3 && item3 == item4 && item4 == item5)
        {
            switch (item1)
            {
                case "seven":
                    if (awarded_seven)
                    {
                        break;
                    }
                    else
                    {
                        awarded_seven = true;
                        return sevens;
                    }
                case "bar":
                    if (awarded_bar)
                    {
                        break;
                    }
                    else
                    {
                        awarded_bar = true;
                        return bars;
                    }
                case "bar2":
                    if (awarded_bar2)
                    {
                        break;
                    }
                    else
                    {
                        awarded_bar2 = true;
                        return bar2s;
                    }
                case "bar3":
                    if (awarded_bar3)
                    {
                        break;
                    }
                    else
                    {
                        awarded_bar3 = true;
                        return bar3s;
                    }
                case "sizzlin_seven":
                    if (awarded_sizzlin)
                    {
                        break;
                    }
                    else
                    {
                        awarded_sizzlin = true;
                        sound_man.playFire();
                        return sizzlins;
                    }
            }
        }

        // Any 5 sevens
        if (misc_sevens.Contains(item1) && misc_sevens.Contains(item2) && misc_sevens.Contains(item3) && misc_sevens.Contains(item4) && misc_sevens.Contains(item5))
        {
            return sevens_any_order;
        }

        // Any 5 bars, any order
        if (misc_bars.Contains(item1) && misc_bars.Contains(item2) && misc_bars.Contains(item3) && misc_bars.Contains(item4) && misc_bars.Contains(item5))
        {
            return bar_any_order;
        }

        return 0;
    }

    //Returns seconds to wait while bounce plays
    public void checkCoins(ReelItem[,] values)
    {
        coinCheckDone = false;
        bool coinsFound = false;
        int winningsAmount = 0;
        for (int i = 0; i< 5; i++)
        {
            if (!coinReels.Contains(i))
            {
                for (int j = 0; j < 3; j++)
                {
                    if (values[i, j].getName() == "coin" && (!spin2win || (spin2win && !coinReels.Contains(i)) ) )
                    {
                        sound_man.playPig();
                        coinsFound = true;
                        //wiggle reel
                        reel_man.wiggleReel(i);
                        //bounce pig
                        pig_man.bouncePig(i + 2);
                        //award points
                        winningsAmount += pig_man.getPigValue(i + 2, reel_man.lines_played);
                        //add coin to list
                        if (!coinReels.Contains(i))
                        {
                            coinReels.Add(i);
                        }
                        
                        //Bonus flag set
                        if (pig_man.getPigValue(i + 2, reel_man.lines_played) == 0)
                        {
                            if (pig_man.getPigSpinToWin(i + 2) && !spin2win)
                            {
                                spin2win = true;
                                firstPass = true;
                                FindObjectOfType<LogoSwapper>().swapLogo("spin");
                            } else if (pig_man.getPigSpinToWin(i + 2))
                            {
                                //do nothing with dupe spin2win pig
                            }
                            else //this is a bonus spins pig
                            {
                                bonusFound = true;
                                sound_man.startPigMusic();
                            }
                        }
                    }
                }
            }
        }

        if (winningsAmount > 200)
        {
            gold_manager.startGold();
        }

        if(coinReels.Count >= 5)
        {
            //JACKPOT
            winningsAmount = jackpot;
        }


        // If bonus rounds
        if (bonusFound)
        {
            if (!spin2win)
            {
                FindObjectOfType<LogoSwapper>().swapLogo("bonus");
            }
            reel_man.addRespins(respinCount);
            bonusFound = false;
        }

        //Always run the incrementer
        animateUpdateWinnings(reelWinnings + winningsAmount);

        //Coins found, update those values
        if (coinsFound)
        {
            //Animate scoreboard increase if we added money
            if (winningsAmount > 0)
            {
                StartCoroutine(bounceAndWiggleWait(false));
            }
            else
            {
                StartCoroutine(bounceAndWiggleWait(true));
            }


            if (spin2win) //And we get to continue spinning 2 win!
            {
                Debug.Log("Spin2Win");
                //reel_man.playSpin2WinRound(coinReels, firstPass);  //COMMENTED OUT
                StartCoroutine(startSpin2WinAfterScore(firstPass));


                //Reset firstPass if we just used it
                if (firstPass)
                {
                    firstPass = false;
                }
            }else // If we aren't in Spin2Win, clear the reel counter
            {
                coinReels.Clear();
            }
        } else if (spin2win) //Spin to win is over
        {
            spin2win = false;
            endSpin2Win = true;
            sound_man.endPigMusic();
            FindObjectOfType<LogoSwapper>().swapLogo("logo");
            firstPass = true;
            coinReels.Clear();
            StartCoroutine(reel_man.endRound_co());
        }
        else
        {
            coinCheckDone = true;
        }

        
    }

    public IEnumerator startSpin2WinAfterScore(bool firstSpin)
    {
        //Only wait on the score check if spin2win is about to take off
        if (firstSpin)
        {
            while (!scoreCheckDone)
            {
                yield return null;
            }
        }

        reel_man.playSpin2WinRound(coinReels, firstPass);
    }

    public IEnumerator bounceAndWiggleWait(bool stopCoinCheck)
    {
        yield return new WaitForSeconds(5.0f); //Minimum bounce time of 5 seconds
        while (animatingScore)
        {
            yield return null;
        }
        pig_man.stopBouncingPigs();
        if (stopCoinCheck)
        {
            coinCheckDone = true;
        }
    }

    public void instantUpdateWinnings(int addValue)
    {
        credit = addValue;
        win_field.text = credit + "";
    }

    public string formatForMoney(int input)
    {
        if(input == 0)
        {
            return "0.00";
        }
        decimal fVal = (decimal)input;
        fVal /= 100;
        decimal finalVal = decimal.Round(fVal, 2, System.MidpointRounding.AwayFromZero);
        return finalVal.ToString();
    }

    public void logOut()
    {
        credit = 0;
        winnings = 0;
    }

    //Visually increase the 
    public void animateUpdateWinnings(int addValue)
    {
        animatingScore = true;
        float timeToTake = 0.0f;
        if (spin2win)
        {
            timeToTake = 2.0f;
        } else if(addValue <= 100)
        {
            timeToTake = 2.0f;
        } else if(addValue < 500)
        {
            timeToTake = 5.0f;
        } else
        {
            timeToTake = 10.0f;
        }

        //start looping sound effect

        if (addValue > 0)
        {
            sound_man.startWinningIncrease();
            StartCoroutine(animateUpdateWinnings_co(addValue, timeToTake));
        }else
        {
            coinCheckDone = true; //POTENTIALLY BAD?  WHAT IS THIS?
            animatingScore = false;
        }
    }

    private IEnumerator animateUpdateWinnings_co(int addValue, float timeToTake)
    {
        
        // animate increase
        float elapsedTime = 0.0f;
        int oldVal = winnings;
        winnings += addValue;
        while (elapsedTime < timeToTake)
        {
            elapsedTime += Time.deltaTime;
            int winningsPortion = Mathf.RoundToInt(addValue * (elapsedTime / timeToTake));
            int displayVal = oldVal + winningsPortion;
            win_field.text = displayVal + ""; // formatForMoney(displayVal);
            yield return null;
        }
        sound_man.endWinningIncrease();

        win_field.text = winnings + ""; // formatForMoney(winnings);
        
        coinCheckDone = true; //POTENTIALLY BAD?  WHAT IS THIS?
        animatingScore = false;
        reelWinnings = 0;

        //end looping sound effect

        yield return null;
    }

    public void instantUpdateCredit(int newCredit)
    {
        credit = newCredit;
        credit_field.text = formatForMoney(credit);
    }

    public void animateUpdateCredit()
    {
        float timeToTake = 2.0f;
        animatingScore = true;

        //start looping sound effect

        StartCoroutine(animateUpdateCredit_co(timeToTake));
        // Remove value from Win box
        StartCoroutine(animateUpdateWinnings_co((-1 * winnings), 2.0f));
    }

    private IEnumerator animateUpdateCredit_co(float timeToTake)
    {
        // animate increase
        float elapsedTime = 0.0f;
        int dumpVal = winnings;
        int oldVal = credit;
        credit += dumpVal;

        while (elapsedTime < timeToTake)
        {
            elapsedTime += Time.deltaTime;
            int creditsPortion = Mathf.RoundToInt(dumpVal * (elapsedTime / timeToTake));
            int displayVal = oldVal + creditsPortion;
            credit_field.text = formatForMoney(displayVal);
            yield return null;
        }

        credit_field.text = formatForMoney(credit);
        animatingScore = false;
        //end looping sound effect

        yield return null;
    }

    public int getCredit()
    {
        return credit;
    }
}
