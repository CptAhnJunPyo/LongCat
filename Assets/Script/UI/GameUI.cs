using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private Transform WinPanel;
    [SerializeField] private Transform LosePanel;
    [SerializeField] private CaptureEffect captureWinScreenController;
    [SerializeField] private FadeController fadeController;
    [SerializeField] private OutOfLivesPanel outOfLivesPanel;
    [SerializeField] private ParticleSystem leftPartical;
    [SerializeField] private ParticleSystem rightPartical;
    private void Start()
    {
        WinPanel.GetComponentInChildren<Button>().onClick.AddListener(()=> 
        {
            fadeController.FadeInOut(ClickBtnNextLevel);
        });
        LosePanel.GetComponentInChildren<Button>().onClick.AddListener(() =>
        {
            fadeController.FadeInOut(ClickBtnRetry);
        });
        GameManager.Instance.OnWin += OnWinGame;
        GameManager.Instance.OnLose += OnLoseGame;
        GameManager.Instance.OnLoadLevel += OnLoadLevel;
    }

    public void OnWinGame()
    {
        Action callback = ShowListWinUI;
        captureWinScreenController.PlayCaptureAnimation(callback);
        
    }  
    
    public void OnLoseGame()
    {
        Action callback = ShowListLoseUI;
        captureWinScreenController.PlayCaptureAnimation(callback);
    }

    public void OnLoadLevel(int level)
    {
        levelText.text = $"Level {level}";
    }

    public void ShowListWinUI()
    {
        WinPanel.gameObject.SetActive(true);
        rightPartical.gameObject.SetActive(true);
        leftPartical.gameObject.SetActive(true);
        rightPartical.Play();
        leftPartical.Play();
    }

    public void ShowListLoseUI()
    {
        LosePanel.gameObject.SetActive(true);
    }

    public void ClickBtnNextLevel()
    {
         GameManager.Instance.LoadNextLevel();
         WinPanel.gameObject.SetActive(false);
         captureWinScreenController.gameObject.SetActive(false);
    }
    public void ClickBtnRetry()
    {
        if (GameManager.Instance.ReloadLevel())
        {
            LosePanel.gameObject.SetActive(false);
            captureWinScreenController.gameObject.SetActive(false);
        }
        else
        {
            var remain = GameData.TimeUntilNextLife();
            outOfLivesPanel.Show(remain);
        }
    }
}
