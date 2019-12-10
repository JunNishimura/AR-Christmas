using System.Collections;
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
            // all UI need to vanish while taking a screen shot
            var uiController = FindObjectOfType<UIController>();
            // deactivate all UI except for captureButton
            uiController.ControlUIActivation(false, false, false, false, false, false, false, false, true, false, false); 
            // capture button can't be deactivated because it has this script, so just make it transparent.
            uiController.captureButton.GetComponent<Image>().color = Color.clear; 
            FindObjectOfType<ObjectPlacement>().ToggleARPlaneDetection(false);

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
            // save photos
            while(true) 
            {
                if (File.Exists(path))
                    break;
                yield return null;
            }
            SaveToAlbum(path);

            // execute camera flash effect after saving photo
            // I need to clear sceans to capture a screenshot.
            // I thoght it would be complicated if I execute camera flash effect while saving a shot.
            // Saving doesn't take time, so this doesn't generate time lag and gives normal feedback to users.
            var uiController = FindObjectOfType<UIController>();
            uiController.captureButton.GetComponent<Image>().color = Color.white; // the button appears again
            uiController.ControlUIActivation(true, false, false, false, false, false, false, false, false, false, false); 
            FindObjectOfType<ObjectPlacement>().ToggleARPlaneDetection(true);
        }
    }
}
