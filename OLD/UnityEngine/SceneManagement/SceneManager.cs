namespace Netcode.io.OLD.UnityEngine.SceneManagement
{
    public static class SceneManager
    {
        public static event Action<Scene, Scene> activeSceneChanged;

        public static event Action<Scene> sceneUnloaded;
        public static int sceneCount => throw new NotImplementedException();
        public static int sceneCountInBuildSettings => throw new NotImplementedException();

        public static void MoveGameObjectToScene(GameObject gameObject, in Scene scene) => throw new NotImplementedException();

        public static Scene GetActiveScene() => throw new NotImplementedException();
        public static void SetActiveScene(in Scene nextScene) => throw new NotImplementedException();

        public static Scene GetSceneAt(int i) => throw new NotImplementedException();
        public static Scene GetSceneByBuildIndex(int index) => throw new NotImplementedException();

        public static AsyncOperation LoadSceneAsync(string sceneName, LoadSceneMode loadSceneMode = LoadSceneMode.Single) => throw new NotImplementedException();

        public static AsyncOperation UnloadSceneAsync(Scene scene) => throw new NotImplementedException();

    }
}