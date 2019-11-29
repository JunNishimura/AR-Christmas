using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
            string filename = "test.png";
            string path = Application.persistentDataPath + "/" + filename;

            File.Delete(path);
            ScreenCapture.CaptureScreenshot(filename);
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
        }
    }
}
