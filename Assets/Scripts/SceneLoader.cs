using UnityEngine;
using UnityEngine.SceneManagement;

namespace ARChristmas
{
    public class SceneLoader : MonoBehaviour
    {
        public GameObject LoadRejectionUI;
        public void OnLoadSceneButton(string _sceneNameToLoad) 
        {
            SceneManager.LoadScene(_sceneNameToLoad);
        }

        public void OnNewTreeButton() 
        {
            GameSceneManager.isNewTree = true;
            SceneManager.LoadScene("Play");
        }

        public void OnLoadTreeButton() 
        {
            if (System.IO.File.Exists(Application.persistentDataPath + "/save.txt"))
            {
                // if save file exists, start loading
                GameSceneManager.isNewTree = false;
                SceneManager.LoadScene("Play");
            }
            else 
            {
                // if no save file, show load rejection UI
                LoadRejectionUI.SetActive(true);
            }
        }
    }   
}