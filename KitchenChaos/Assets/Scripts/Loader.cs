using UnityEngine.SceneManagement;

public static class Loader
{

    public enum SceneName
    {
        MainMenuScene,
        LoadingScene,
        GameScene
    }

    private static SceneName targetScene;

    public static void LoadScene(SceneName targetScene)
    {
        Loader.targetScene = targetScene;

        LoadLoadingScene();        
    }

    public static void LoaderCallback() =>
        SceneManager.LoadScene(targetScene.ToString());

    private static void LoadLoadingScene() =>
        SceneManager.LoadScene(SceneName.LoadingScene.ToString());

}
