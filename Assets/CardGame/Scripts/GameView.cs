using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CardGame {
  public class GameView : MonoBehaviour {
    private const string ShowTrigger = "show";

    [SerializeField]
    private GameObject pausePanel;

    [SerializeField]
    private Animator selectCardAnimator;
    [SerializeField]
    private Animator startBattleAnimator;
    [SerializeField]
    private Animator resultBattleAnimator;

    [SerializeField]
    private Text resultText;
    [SerializeField]
    private Text playerPointText;
    [SerializeField]
    private Text aiPointText;

    public void ShowSelectCardPanel() {
      selectCardAnimator.SetTrigger(ShowTrigger);
    }

    public IEnumerator ShowStartBattlePanel() {
      startBattleAnimator.SetTrigger(ShowTrigger);
      yield return new WaitForSeconds(1f);
    }

    public void ShowResultBattlePanel(bool isWin) {
      resultBattleAnimator.SetTrigger(ShowTrigger);
      resultText.text = isWin ? "You Won!" : "You Lost";
    }

    public void UpdatePlayerPoint(int point) {
      playerPointText.text = point.ToString();
    }

    public void UpdateAIPoint(int point) {
      aiPointText.text = point.ToString();
    }

    public void ShowPausePanel() {
      pausePanel.SetActive(true);
    }

    public void HidePausePanel() {
      pausePanel.SetActive(false);
    }
  }
}