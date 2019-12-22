using System;
using System.Collections;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;

namespace ARChristmas.Utilities.ScreenShot.iOS
{
    public static class ScreenShotManager_iOS
    {
        [DllImport("__Internal")]
        private static extern void SaveToAlbum(string path);

        // first parameter needs to be a fileName, not a pathName because fileName will be used for capturing
        public static void CaptureAndSaveToAlbum(string fileName, Action callback) 
        {
            string pathToSave = Application.persistentDataPath + "/" + fileName;
            File.Delete(pathToSave);
            ScreenCapture.CaptureScreenshot(fileName);

            // cannot use StartCoroutine() because this static class does not inherit Monobehaviour
            CoroutineHandler.StartStaticCoroutine(SaveToCameraroll(pathToSave, callback));
        }

        static IEnumerator SaveToCameraroll(string path, Action callback) 
        {
            while(true) 
            {
                if (File.Exists(path))
                    break;
                yield return null;
            }
            SaveToAlbum(path);

            callback();
        }
    }
}
