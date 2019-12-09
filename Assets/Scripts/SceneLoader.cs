using UnityEngine;
using UnityEngine.SceneManagement;

namespace ARChristmas
{
    public class SceneLoader : MonoBehaviour
    {
        public void OnLoadSceneButton(string _sceneNameToLoad) 
        {
            SceneManager.LoadScene(_sceneNameToLoad);
        }
        
        // True  -> Create New Tree 
        // False -> Load saved Tree
        public void OnTreeTypeSelectButton(bool _isNewTree) 
        {
            GameSceneManager.isNewTree = _isNewTree;
        }
    }   
}