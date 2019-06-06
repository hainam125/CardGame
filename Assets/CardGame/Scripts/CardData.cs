using UnityEngine;

namespace CardGame {
  [CreateAssetMenu(fileName = "New Card Data", menuName = "Card Data")]
  public class CardData : ScriptableObject {
    public new string name;
    public string desc;
    public Sprite avatar;

    public int id;
    public int hp;
    public int attack;
  }
}
