using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CardGame {
  [CreateAssetMenu(fileName = "New Game Data", menuName = "Game Data")]
  public class GameData : ScriptableObject {
    public bool isNewGame;
  }
}
