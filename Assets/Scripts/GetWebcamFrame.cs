using UnityEngine;
using UnityEngine.UI;

public class GetWebcamFrame : MonoBehaviour
{

    private int frameDiff = 30;
    private WebCamTexture webcamTexture;
    private Texture2D currTexture;
    private Texture2D prevTexture;
    private int frameCount;
    [SerializeField] private RawImage outputImage;

    // Use this for initialization
    void Start()
    {
        WebCamDevice[] devices = WebCamTexture.devices;
        for (int i = 0; i < devices.Length; i++)
            Debug.Log(devices[i].name);

        webcamTexture = new WebCamTexture(1920, 1080);
     
        webcamTexture.wrapMode = TextureWrapMode.Clamp;
        Debug.Log("Texture Size " + webcamTexture.width + " h: " + webcamTexture.height);
        currTexture = new Texture2D(1920, 1080, TextureFormat.ARGB32, false);
        prevTexture = new Texture2D(1920, 1080, TextureFormat.ARGB32, false);


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
            Debug.Log("Smaller" + webcamTexture.width + " cur" + prevTexture.width);
       //     currTexture.Resize(webcamTexture.width, webcamTexture.height);
       //     prevTexture.Resize(webcamTexture.width, webcamTexture.height);
        }
        else
        {
            if (this.frameCount >= this.frameDiff)
            {
               prevTexture.SetPixels32(currTexture.GetPixels32());
               prevTexture.Apply();
                currTexture.SetPixels(webcamTexture.GetPixels());
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
