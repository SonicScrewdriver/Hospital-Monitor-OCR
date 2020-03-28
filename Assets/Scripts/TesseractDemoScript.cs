using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using System.IO;

public class TesseractDemoScript : MonoBehaviour
{

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
        if(frameCount == 48)
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
            if(words[i].word == "RR")
            {
                Rect location = words[i].box;
                Debug.Log("Respiratory Rate found at: " + location.x + " " + location.y);
                
            }
        }
    }

    public static Color FromArgb(int red, int green, int blue)
    {
        // float fa = ((float)alpha) / 255.0f;
        float fr = ((float)red) / 255.0f;
        float fg = ((float)green) / 255.0f;
        float fb = ((float)blue) / 255.0f;
        return new Color(fr, fg, fb);
    }


    public void ApplyContrast(Texture2D outputTexture)
    {

        double contrastD = contrast;
 
        Texture2D contrastImg = new Texture2D(outputTexture.width, outputTexture.height);
     //   contrastImg.SetPixels(outputTexture.GetPixels());
      //  contrastImg.LoadImage(outputTexture.bytes);
        contrastImg.Apply();
       if (contrastD < -100) contrastD = -100;
        if (contrastD > 100) contrastD = 100;
        contrastD = (100.0 + contrastD) / 100.0;
        contrastD *= contrastD;
       Debug.Log("ContrastD " + contrastD);
        contrastD = 1;
      //  Debug.Log("ContrastD " + contrastD);
        Color color;
        for (int x = 0; x < contrastImg.width; x++)
        {
            for (int y = 0; y < contrastImg.height; y++)
            {
                color = outputTexture.GetPixel(x, y);
                double pR = color.r / 255.0;
                pR -= 0.5;
                pR *= contrastD;
                pR += 0.5;
                pR *= 255;
                if (pR < 0) pR = 0;
                if (pR > 255) pR = 255;

                double pG = color.g / 255.0;
                pG -= 0.5;
                pG *= contrastD;
                pG += 0.5;
                pG *= 255;
                if (pG < 0) pG = 0;
                if (pG > 255) pG = 255;

                double pB = color.b / 255.0;
                pB -= 0.5;
                pB *= contrastD;
                pB += 0.5;
                pB *= 255;
                if (pB < 0) pB = 0;
                if (pB > 255) pB = 255;
           //     Debug.Log("New Colour:" + pR + " " + pG + " " + pB);
                 Color c = new Color(System.Convert.ToInt32(pR), System.Convert.ToInt32(pG), System.Convert.ToInt32(pB));
                contrastImg.SetPixel(x, y, c);
                contrastImg.Apply();
            }
        }
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