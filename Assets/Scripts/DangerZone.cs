using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class DangerZone : MonoBehaviour
{
    float dangerTime = 7.0f;

    [SerializeField]
    TextMeshProUGUI dangerWarning;

    List<TorqueCreator> containing_balls = new();

    [SerializeField]
    DataManager dataManager;

    void OnTriggerEnter(Collider other)
    {
        var ball_component = other.GetComponent<TorqueCreator>();

        if (ball_component.Deployed)
        {
            ball_component.ActivateDanger();
            containing_balls.Add(ball_component);
        }

    }
    void OnTriggerExit(Collider other)
    {
        var ball_component = other.GetComponent<TorqueCreator>();

        if (ball_component.Deployed)
        {
            ball_component.DeactivateDanger();
            containing_balls.Remove(ball_component);
        }

    }

    void Update()
    {
        if (dataManager.IsGameOver)
        {
            return;
        }

        bool ctf = false;

        foreach (var ball in containing_balls)
        {
            if(ball == null)
            {
                continue;
            }

            if (ball.GetComponent<Rigidbody>().velocity.sqrMagnitude > 0.1f)
            {
                ctf = ctf || false;
            }
            else
            {
                ctf = ctf || true;
            }

        }

        if (ctf)
        {
            dangerTime -= Time.deltaTime;
            ShowDangerWarning();
            
        }
        else
        {
            dangerTime = 7.0f;
            HideDangerWarning();
            Debug.Log("Danger time reset");
        }

        if (dangerTime <= 0.0f)
        {
            HideDangerWarning();
            dataManager.GameOver();
        }
    }

    Coroutine show_coroutine;
    Coroutine hide_coroutine;

    void ShowDangerWarning()
    {
        if (show_coroutine == null)
        {
            show_coroutine = StartCoroutine(ShowDangerWarningCoroutine());
        }

        if(hide_coroutine != null)
        {
            StopCoroutine(hide_coroutine);
            hide_coroutine = null;
        }
    } 
    IEnumerator ShowDangerWarningCoroutine()
    {
        yield return new WaitForSeconds(0.5f);
        dangerWarning.gameObject.SetActive(true);
        dangerWarning.text = "DANGER " + System.Math.Round(dangerTime, 2);
        show_coroutine = null;
    }

    IEnumerator HideDangerWarningCoroutine()
    {
        yield return new WaitForSeconds(0.5f);
        dangerWarning.gameObject.SetActive(false);
        hide_coroutine = null;
        show_coroutine = null;

    }

    void HideDangerWarning()
    {
        if(hide_coroutine == null)
        {
            hide_coroutine = StartCoroutine(HideDangerWarningCoroutine());
        }

        if (show_coroutine != null)
        {
            StopCoroutine(show_coroutine);
            show_coroutine = null;
        }
    }
}
