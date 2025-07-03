using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Script_PortalTerror : MonoBehaviour
{
    public SceneAsset SelectedScene;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }


    void OnTriggerEnter(Collider other)
    {
        SceneManager.LoadScene(SelectedScene.name);

    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
