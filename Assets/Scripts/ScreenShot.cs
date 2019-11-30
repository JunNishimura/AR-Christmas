﻿using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Runtime.InteropServices;

namespace ARChristmas
{
    public class ScreenShot : MonoBehaviour
    {
        [DllImport("__Internal")]
        private static extern void SaveToAlbum(string path);

        public void ScreenShotPressed() 
        {   
            // clean up all UI on the screen for taking a screen shot
            var uiController = FindObjectOfType<UIController>();
            uiController.ControlUIActivation(false, false, false, false, false, false, false, false, true); // deactivate UIs except for captureButton
            uiController.captureButton.GetComponent<Image>().color = Color.clear; // make captureButton disappear from the scene

            // execute capturing
            string filename = "test.png";
            string path = Application.persistentDataPath + "/" + filename;
            File.Delete(path);
            ScreenCapture.CaptureScreenshot(filename);
            GetComponent<AudioSource>().Play(); 
            StartCoroutine(SaveToCameraroll(path));
        }

        IEnumerator SaveToCameraroll(string path) 
        {
            while(true) 
            {
                if (File.Exists(path))
                    break;
                yield return null;
            }
            SaveToAlbum(path);

            // execute camera flash effect after saving photo may sound wierd. 
            // I need to clear sceans to capture a screenshot.
            // I thoght it would be complicated if I execute camera flash effect while saving a shot.
            // Saving doesn't take time, so this doesn't generate time lag and gives normal feedback to users.
            var uiController = FindObjectOfType<UIController>();
            uiController.captureButton.GetComponent<Image>().color = Color.white;
            uiController.ControlUIActivation(true, false, false, false, false, false, false, false, false); 
        }
    }
}