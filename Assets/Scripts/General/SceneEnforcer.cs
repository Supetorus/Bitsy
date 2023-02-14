using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneEnforcer : MonoBehaviour
{
    [SerializeField] string sceneToLoadName;
    [SerializeField] LoadSceneMode loadType;

    // Start is called before the first frame update
    void Start()
    {
        SceneManager.LoadScene(sceneToLoadName, loadType);
    }
}
