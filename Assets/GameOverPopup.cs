using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
public class GameOverPopup : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI score_text;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetScore(int score)
    {
        score_text.text = score.ToString();
    }

    public void ReloadScene()
    {
        SceneManager.LoadScene("Game");
    }
}
