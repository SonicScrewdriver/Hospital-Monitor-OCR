using UnityEngine;

public class GetWebcamFrame : MonoBehaviour
{

    private int frameDiff = 30;
    private WebCamTexture webcamTexture;
    private Texture2D currTexture;
    private Texture2D prevTexture;
    private int frameCount;


    // Use this for initialization
    void Start()
    {
        WebCamDevice[] devices = WebCamTexture.devices;
        for (int i = 0; i < devices.Length; i++)
            Debug.Log(devices[i].name);

        webcamTexture = new WebCamTexture();
        webcamTexture.wrapMode = TextureWrapMode.Clamp;
        currTexture = new Texture2D(webcamTexture.width, webcamTexture.height);
        prevTexture = new Texture2D(webcamTexture.width, webcamTexture.height);


        if (devices.Length > 1)
        {
            webcamTexture.deviceName = devices[1].name;
            Debug.Log(webcamTexture.deviceName);
            webcamTexture.Play();
        }
        else
        {
            webcamTexture.Play();
        }
    }

    // Update is called once per frame
    void Update()
    {
        this.frameCount++;

        if (prevTexture.width < webcamTexture.width)
        {
            currTexture.Resize(webcamTexture.width, webcamTexture.height);
            prevTexture.Resize(webcamTexture.width, webcamTexture.height);
        }
        else
        {
            if (this.frameCount >= this.frameDiff)
            {
                prevTexture.SetPixels32(currTexture.GetPixels32());
                prevTexture.Apply();
                currTexture.SetPixels32(webcamTexture.GetPixels32());
                currTexture.Apply();
                this.frameCount = 0;
            }
        }
    }

    public Vector2 GetSize()
    {
        return new Vector2(webcamTexture.width, webcamTexture.height);
    }

    public Texture2D GetCurrFrame()
    {
        return currTexture;
    }

    public Texture2D GetPrevFrame()
    {
        return prevTexture;
    }
}
