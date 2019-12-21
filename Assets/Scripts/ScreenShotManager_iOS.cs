using System;
using System.Collections;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;

namespace ARChristmas.ScreenShot
{
    public static class ScreenShotManager_iOS
    {
        [DllImport("__Internal")]
        private static extern void SaveToAlbum(string path);

        public static void CaptureAndSaveToAlbum(string fileName, Action callback) 
        {
            string pathToSave = Application.persistentDataPath + "/" + fileName;
            File.Delete(pathToSave);
            ScreenCapture.CaptureScreenshot(fileName);

            CoroutineHandler.StartStaticCoroutine(SaveToCameraroll(pathToSave, callback));
        }

        static IEnumerator SaveToCameraroll(string path, Action callback) 
        {
            // save photos
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
