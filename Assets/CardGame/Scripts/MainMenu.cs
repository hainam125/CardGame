using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace CardGame {
  public class MainMenu : MonoBehaviour {
    [SerializeField]
    private GameData data;
    [SerializeField]
    private GameObject loadingPanel;
    [SerializeField]
    private GameObject resumeBtn;

    public void Awake() {
      resumeBtn.SetActive(DataUtils.HasSavedData());
    }

    public void NewGame() {
      StartGame(true);
    }

    public void ResumeGame() {
      StartGame(false);
    }

    private void StartGame(bool isNew) {
      data.isNewGame = isNew;
      loadingPanel.SetActive(true);
      SceneManager.LoadScene(1);
    }
  }
}