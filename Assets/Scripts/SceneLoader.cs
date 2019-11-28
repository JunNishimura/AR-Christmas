using UnityEngine;
using UnityEngine.SceneManagement;

namespace ARChristmas
{
    public class SceneLoader : MonoBehaviour
    {
        public void SceneLoad(string sceneNameToLoad) 
        {
            SceneManager.LoadScene(sceneNameToLoad);
        }
    }   
}