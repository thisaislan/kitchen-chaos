using Unity.Netcode;
using UnityEngine.SceneManagement;

public static class Loader
{

    public enum SceneName
    {
        MainMenuScene,
        LoadingScene,
        GameScene,
        LoobyScene,
        CharacterSelectScene
    }

    private static SceneName targetScene;

    public static void LoadScene(SceneName targetScene)
    {
        Loader.targetScene = targetScene;

        LoadLoadingScene();        
    }

    public static void LoadNetworkScene(SceneName targetScene) =>
        NetworkManager.Singleton.SceneManager.LoadScene(targetScene.ToString(), LoadSceneMode.Single);

    public static void LoaderCallback() =>
        SceneManager.LoadScene(targetScene.ToString());

    private static void LoadLoadingScene() =>
        SceneManager.LoadScene(SceneName.LoadingScene.ToString());

}
