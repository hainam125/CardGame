using UnityEngine;
using UnityEngine.EventSystems;

public class UserInput : MonoBehaviour {
  private EventSystem currentEventSystem;

  public bool IsMouseClick { get; private set; }
  public bool IsExit { get; private set; }

  private void Awake() {
    currentEventSystem = EventSystem.current;
  }

  public void GameUpdate(float deltaTime) {
    IsMouseClick = false;
    IsExit = false;

    if (Input.GetMouseButtonDown(0) && !currentEventSystem.IsPointerOverGameObject()) {
      IsMouseClick = true;
    }
    if (Input.GetKeyDown(KeyCode.Escape)) {
      IsExit = true;
    }
  }
}
