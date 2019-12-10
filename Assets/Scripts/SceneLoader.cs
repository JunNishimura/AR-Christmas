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
            OnLoadSceneButton("Main");
        }

        public void OnLoadTreeButton() 
        {
            if (System.IO.File.Exists(Application.persistentDataPath + "/save.txt"))
            {
                // if save file exists
                GameSceneManager.isNewTree = false;
                OnLoadSceneButton("Main");
            }
            else 
            {
                // if no save file
                LoadRejectionUI.SetActive(true);
            }
        }
    }   
}