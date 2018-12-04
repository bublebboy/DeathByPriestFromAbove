using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{

    public string[] levelNames;
    public CanvasGroup mainMenu;
    public CanvasGroup loadingScreen;
    public CanvasGroup pauseScreen;
    public CanvasGroup gameOverScreen;

    private int levelIndex = -1;

    public void StartGame()
    {
        StartCoroutine(StartGameRoutine());
    }

    public void ReturnToMenu()
    {
        StartCoroutine(ReturnToMenuRoutine());
    }

    public IEnumerator StartGameRoutine()
    {
        GameOver(false);
        yield return StartCoroutine(FadeCanvasGroup(loadingScreen, 1f, 0.5f));
        mainMenu.interactable = false;
        mainMenu.alpha = 0;
        if (levelIndex > -1)
        {
            yield return SceneManager.UnloadSceneAsync(levelNames[levelIndex]);
        }
        ++levelIndex;
        if (levelIndex < levelNames.Length)
        {
            yield return SceneManager.LoadSceneAsync(levelNames[levelIndex]);
            yield return new WaitForSeconds(0.5f);
            yield return StartCoroutine(FadeCanvasGroup(loadingScreen, 0f, 0.5f));
        }
        else
        {
            StartCoroutine(ReturnToMenuRoutine());
        }
    }

    public IEnumerator ReturnToMenuRoutine()
    {
        GameOver(false);
        yield return StartCoroutine(FadeCanvasGroup(loadingScreen, 1f, 0.5f));
        mainMenu.interactable = false;
        mainMenu.alpha = 0;
        if (levelIndex > -1)
        {
            yield return SceneManager.UnloadSceneAsync(levelNames[levelIndex]);
        }
        levelIndex = -1;
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(FadeCanvasGroup(loadingScreen, 0f, 0.5f));
    }

    public void GameOver(bool active)
    {
        StartCoroutine(FadeCanvasGroup(gameOverScreen, active ? 1f : 0f, 0.25f));
    }

    public static IEnumerator FadeCanvasGroup(CanvasGroup group, float end, float duration)
    {
        group.interactable = false;
        group.blocksRaycasts = end > 0 ? true : false;
        float start = group.alpha;
        for (float i = 0; i > 0; i += Time.deltaTime)
        {
            group.alpha = Mathf.Lerp(start, end, i / duration);
            yield return null;
        }
        group.alpha = end;
        group.interactable = end > 0.1 ? true : false;
    }
}
