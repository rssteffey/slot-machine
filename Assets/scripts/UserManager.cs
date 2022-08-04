using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class UserManager : MonoBehaviour {

    const string apiHost = "steffey-casinos.onthewifi.com:4815"; //Swap for your API host

    public string username;

    private int userScore;

    public ReelManager reel_man;
    public ScoreManager score_man;

    public GameObject QR;

    public Bouncer scan_bouncer;

    private bool qr_done;
    private GameObject qr_inst;

	// Use this for initialization
	void Start () {
        qr_inst = QR;
        qr_inst.SetActive(false);
    }
	
	// Update is called once per frame
	void Update () {
        
    }
    

    public void logIn()
    {
        //spawn QR reader and start request process
        StartCoroutine(runLogin());
    }

    public void logOut()
    {
        // set final score
        setFinalBalance(score_man.getCredit());

    }

    public void setBalance(int newScore)
    {
        StartCoroutine(setUserBalance(username, newScore));
    }

    public void setFinalBalance(int newScore)
    {
        StartCoroutine(setFinalUserBalance(username, newScore));
    }

    private IEnumerator timeOutRequest()
    {
        yield return new WaitForSeconds(30.0f);
        if (qr_inst != null)
        {
            qr_inst.SetActive(false);
        }
        yield return null;
    }

    private IEnumerator runLogin()
    {
        qr_done = false;
        qr_inst.SetActive(true);
        yield return null;
        qr codeReader = qr_inst.GetComponent<qr>();
        scan_bouncer.setVisible();
        codeReader.userId = null;
        codeReader.found = false;
        while (!codeReader.found)
        {
            yield return null;
        }

        scan_bouncer.setInvisible();

        username = codeReader.userId;

        StartCoroutine(getUserBalance(codeReader.userId));

        while (!qr_done)
        {
            yield return null;
        }

        //Clear camera screen so next login doesn't use old image
        

        qr_inst.SetActive(false);
        yield return null;
    }

    private IEnumerator setUserBalance(string username, int newScore)
    {
        Debug.Log("Starting Set Request");
        string score = score_man.formatForMoney(newScore);
        byte[] myData = System.Text.Encoding.UTF8.GetBytes("This is some pointless data to placate Unity");
        UnityWebRequest www = UnityWebRequest.Put(apiHost + "/api/balance/" + username + "/set/" + score, myData);
        yield return www.Send();

        if (www.isError)
        {
            Debug.Log(www.error);
        }
        else
        {
            // Show results as text
            Debug.Log(www.downloadHandler.text);
        }
    }

    private IEnumerator setFinalUserBalance(string username, int newScore)
    {
        Debug.Log("Starting Set Request");
        string score = score_man.formatForMoney(newScore);
        byte[] myData = System.Text.Encoding.UTF8.GetBytes("This is some pointless data to placate Unity");
        UnityWebRequest www = UnityWebRequest.Put(apiHost + "/api/balance/" + username + "/set/" + score, myData);
        yield return www.Send();

        if (www.isError)
        {
            Debug.Log(www.error);
        }
        else
        {
            // Show results as text
            Debug.Log(www.downloadHandler.text);
            
            //reset game credit and inactivate user
            score_man.instantUpdateCredit(0);
            reel_man.PLAYER_ACTIVE = false;
            username = null;
        }
    }

    private IEnumerator getUserBalance(string username)
    {
        Debug.Log("Starting Request");
        UnityWebRequest www = UnityWebRequest.Get(apiHost + "/api/balance/" + username);
        yield return www.Send();

        if (www.isError)
        {
            Debug.Log(www.error);
        }
        else
        {
            // Show results as text
            Debug.Log(www.downloadHandler.text);
            float userScoreFloat = float.Parse(www.downloadHandler.text);
            userScore = (int)(100 * userScoreFloat);
            Debug.Log(userScore);

            reel_man.PLAYER_ACTIVE = true;
            score_man.instantUpdateCredit(userScore);
            qr_done = true;
            
        }
    }
}
