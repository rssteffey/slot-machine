using ZXing;
using ZXing.QrCode;
using UnityEngine;
using UnityEngine.UI;
using System;

public class qr : MonoBehaviour
{
    private WebCamTexture camTexture;
    private Rect screenRect;
    private int frameSkipMax = 6;
    private int frameSkip = 0;

    public string userId;
    public bool found;

    void Start()
    {
        userId = null;
        found = false;
        screenRect = new Rect(600, 920, 350, 200);
        camTexture = new WebCamTexture();

        camTexture.deviceName = WebCamTexture.devices[WebCamTexture.devices.Length - 1].name;
        camTexture.requestedHeight = Screen.height;
        camTexture.requestedWidth = Screen.width;
        if (camTexture != null)
        {
            camTexture.Play();
        }
    }

    void Update()
    {

    }
    
    void onDestroy()
    {
        camTexture.Stop();
    }

    void onDisable()
    {
        camTexture.Stop();
    }

    void OnGUI()
    {
        
        // drawing the camera on screen
        GUI.DrawTexture(screenRect, camTexture, ScaleMode.ScaleToFit);
       
        // do the reading — you might want to attempt to read less often than you draw on the screen for performance sake
        if (frameSkip == 0 && camTexture.didUpdateThisFrame)
        {
            //Debug.Log("Reading");
            try
            {
                IBarcodeReader barcodeReader = new BarcodeReader();
                // decode the current frame
                var result = barcodeReader.Decode(camTexture.GetPixels32(),
                  camTexture.width, camTexture.height);
                if (result != null)
                {
                    Debug.Log("DECODED TEXT FROM QR: " + result.Text);
                    userId = result.Text;
                    found = true;
                }
            }
            catch (Exception ex) {  }
        }
        frameSkip++;
        if (frameSkip >= frameSkipMax)
        {
            frameSkip = 0;
        }
    }
}