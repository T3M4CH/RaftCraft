using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;
using Reflex.Attributes;
using GTap.Analytics;
using UnityEngine;

public class SplashScreen : MonoBehaviour
{
    private BiomesSceneManager _biomesSceneManager;
    
    [Inject]
    private void Construct(BiomesSceneManager biomesSceneManager)
    {
        _biomesSceneManager = biomesSceneManager;
    }
    
    private async void Start()
    {
        GtapAnalytics.GameplayStarted();
        await WaitLoadGame();
    }

    private async UniTask WaitLoadGame()
    {
        var asyncTask = SceneManager.LoadSceneAsync(_biomesSceneManager.CurrentScene, LoadSceneMode.Additive);
        asyncTask.completed += Completed;
        while (!asyncTask.isDone)
        {
            await UniTask.Yield();
        }

        await UniTask.DelayFrame(20);
        await SceneManager.UnloadSceneAsync("GameLoader");
    }

    private void Completed(AsyncOperation operation)
    {
        operation.completed -= Completed;
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(_biomesSceneManager.CurrentScene));
    }
}
