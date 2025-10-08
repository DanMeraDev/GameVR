using UnityEngine;
using UnityEngine.SceneManagement;
public static class SceneMessenger
{
    public static int nextScene;

    public static void Load(int targetScene)
    {
        nextScene = targetScene;
        SceneManager.LoadScene("LoadScene");
    }

    public static void LoadMenu()
    {
        nextScene = 0;
        SceneManager.LoadScene("LoadScene");
    }

}
