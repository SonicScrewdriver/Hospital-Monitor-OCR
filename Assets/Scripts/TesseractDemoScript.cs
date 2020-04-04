using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using System.IO;
using System.Runtime.InteropServices;

public class TesseractDemoScript : MonoBehaviour
{
    [DllImport("OpenCV")]
    private static extern void ProcessImage(ref Color32[] rawImage, int width, int height);
    // Set the main properties
    [SerializeField] private Texture2D imageToRecognize;
    [SerializeField] private TextMeshProUGUI displayText;
    [SerializeField] private RawImage outputImage;
    // [Range(0.0f, 10.0f)] public float mySliderFloat;
    [Range(-100.0f, 100.0f)] [SerializeField] public double contrast = 100;
    private Texture2D texture;
    private TesseractDriver _tesseractDriver;
    private Texture2D currentFrame;
    private string _text = "";
    private static GetWebcamFrame webcam;
    private int frameCount = 0;
    private Texture2D snap;
    private Texture2D zoomedSnap;

    private void Start()
    {
        webcam = GameObject.Find("Webcam").GetComponent<GetWebcamFrame>();
    }

    private void Update()
    {
        frameCount++;
        if (frameCount == 48)
        {
            Debug.Log("Trying to recognize now");
            texture = webcam.GetCurrFrame();

            // Set the texture for the image we want to recognize, set it to 32bit
            //  texture = new Texture2D(currentFrame.width, currentFrame.height, TextureFormat.ARGB32, false); 
            //  texture.SetPixels32(currentFrame.GetPixels32());
            texture.Apply();

            snap = new Texture2D(texture.width, texture.height);
            snap.SetPixels(texture.GetPixels());
            snap.Apply();



            _tesseractDriver = new TesseractDriver();



            // Recognize the Texture
            Recognize(snap);


            // Display the image
            SetImageDisplay();
        }
        else
        {
            return;
        }
    }
    private void Recognize(Texture2D outputTexture)
    {
        Debug.Log("Contrasting");

        Debug.Log("Trying to recognize now, in recognize function");
        // Clear out the text
        ClearTextDisplay();

        // Add the Tesseract Version to the text to the Display 
        Debug.Log(_tesseractDriver.CheckTessVersion());

        // Start up the Tesseract Driver
        _tesseractDriver.Setup();
        ApplyContrast(outputTexture);
        // Add the Recognized Text to the Display


        // Add any error messages To the Display
        //  Debug.LogError(_tesseractDriver.GetErrorMessage());
    }

    // Clears the Text display
    private void ClearTextDisplay()
    {
        _text = "";
    }

    // Add text to the display -- if it's an error, console log it instead
    private void GetWords(List<WordList> words, bool isError = false)
    {
        for (int i = 0; i < words.Count; i++)
        {
            var output = JsonUtility.ToJson(words[i], true);
            Debug.Log("Word #" + i + " " + output.ToString());
            if (words[i].word == "RR")
            {
                Rect location = words[i].box;
                Debug.Log("Respiratory Rate found at: " + location.x + " " + location.y);

            }
        }
    }

    public static Color FromArgb(float red, float green, float blue)
    {
       // Debug.Log("Fromargb" + red + green + blue);
        // float fa = ((float)alpha) / 255.0f;
        float fr = red;
        float fg = green;
        float fb = blue;
        return new Color(fr, fg, fb);
    }


    public void ApplyContrast(Texture2D outputTexture)
    {

        float contrastD = (float)contrast;

        Texture2D contrastImg = new Texture2D(outputTexture.width, outputTexture.height);
          contrastImg.SetPixels(outputTexture.GetPixels());
        //  contrastImg.LoadImage(outputTexture.bytes);
        contrastImg.Apply();
        var rawImage = outputTexture.GetPixels32();
        ProcessImage(ref rawImage, outputTexture.width, outputTexture.height);
        contrastImg.SetPixels32(rawImage);
        contrastImg.Apply();

        /*
        if (contrastD < -100f) contrastD = -100f;
        if (contrastD > 100f) contrastD = 100f;
        contrastD = (100.0f + contrastD) / 100.0f;
        contrastD *= contrastD;
        Debug.Log("ContrastD " + contrastD);
        //  contrastD = 1;
        //  Debug.Log("ContrastD " + contrastD);
        Color color;
        for (int x = 0; x < contrastImg.width; x++)
        {
            for (int y = 0; y < contrastImg.height; y++)
            {
                color = webcam.GetPixel(x, y);
              //  Debug.Log("Original Colour:" + color);
                float pR = color.r;
                pR -= 0.5f;
                pR *= contrastD;
                pR += 0.5f;
                if (pR < 0f) pR = 0f;
                if (pR > 1f) pR = 1f;

                float pG = color.g;
                pG -= 0.5f;
                pG *= contrastD;
                pG += 0.5f;
                if (pG < 0f) pG = 0f;
                if (pG > 1f) pG = 1;

                float pB = color.b;
                pB -= 0.5f;
                pB *= contrastD;
                pB += 0.5f;
                if (pB < 0f) pB = 0f;
                if (pB > 1f) pB = 1f;
              //      Debug.Log("New Colour:" + System.Convert.ToByte(pR) + " " + pG + " " + pB);
                Color c = FromArgb(pR, pG, pB);
             //  Debug.Log("New Colour2:" + c);
               contrastImg.SetPixel(x, y, c);
                contrastImg.Apply();
            }
        } */
        contrastImg.Apply();


        outputImage.material.mainTexture = contrastImg;
        byte[] bytes = contrastImg.EncodeToPNG();
        File.WriteAllBytes(Application.dataPath + "/Snapshots/Screenshot_" + System.DateTime.Now.ToString("dd-MM-yyyy_hh-mm-ss") + ".png", bytes);
        Debug.Log("Contrast done");
        GetWords(_tesseractDriver.Recognize(contrastImg));
    }

    // Called Every frame, after all the update functions have been called.
    private void LateUpdate()
    {
        displayText.text = _text;
    }

    // Create the Highlights
    private void SetImageDisplay()
    {

        RectTransform rectTransform =
             outputImage.GetComponent<RectTransform>();

        rectTransform.SetSizeWithCurrentAnchors(
            RectTransform.Axis.Vertical,
            rectTransform.rect.width *
            _tesseractDriver.GetHighlightedTexture().height /
            _tesseractDriver.GetHighlightedTexture().width);

        outputImage.texture =
            _tesseractDriver.GetHighlightedTexture();
    }

}