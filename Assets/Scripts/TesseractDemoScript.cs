using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class TesseractDemoScript : MonoBehaviour
{

    // Set the main properties
    [SerializeField] private Texture2D imageToRecognize;
    [SerializeField] private TextMeshProUGUI displayText;
    [SerializeField] private RawImage outputImage;
    private Texture2D texture;
    private TesseractDriver _tesseractDriver;
    private Texture2D currentFrame;
    private string _text = "";
    private static GetWebcamFrame webcam;
    private int frameCount = 0;

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

            Texture2D snap = new Texture2D(texture.width, texture.height);
            snap.SetPixels(texture.GetPixels());
            snap.Apply();
            outputImage.material.mainTexture = snap;

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
        Debug.Log("Trying to recognize now, in recognize function");
        // Clear out the text
        ClearTextDisplay();

        // Add the Tesseract Version to the text to the Display 
        Debug.Log(_tesseractDriver.CheckTessVersion());

        // Start up the Tesseract Driver
        _tesseractDriver.Setup();

        // Add the Recognized Text to the Display
        GetWords(_tesseractDriver.Recognize(outputTexture));

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
        }
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