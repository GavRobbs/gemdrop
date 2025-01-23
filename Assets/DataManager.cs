using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DataManager : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI score_text;

    [SerializeField]
    GameObject gameOverMenu;

    private Coroutine counter_coroutine;

    int score = 0;

    int new_score = 0;

    bool _gameOver = false;

    public bool IsGameOver
    {
        get
        {
            return _gameOver;
        }
    }
    public void GameOver()
    {
        _gameOver = true;
        gameOverMenu.SetActive(true);
        gameOverMenu.GetComponent<GameOverPopup>().SetScore(new_score);
    }

    IEnumerator AnimateScore(int start, int end)
    {
        float elapsed_time = 0.0f;
        while (elapsed_time < 1.25f)
        {
            elapsed_time += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed_time / 2.0f);
            int current_value = Mathf.RoundToInt(Mathf.Lerp(start, end, t));
            score_text.text = "Score: " + current_value.ToString();
            score = current_value;
            yield return null;
        }

        score_text.text = "Score: " + end.ToString();
        score = end;
    }

    void UpdateScore()
    {
        if (counter_coroutine != null)
        {
            StopCoroutine(counter_coroutine);
        }

        counter_coroutine = StartCoroutine(AnimateScore(score, new_score));
    }

    public void AddScore(int type, int combo_level)
    {
        new_score += 100 * type * combo_level;
        UpdateScore();
    }
}
