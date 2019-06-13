using UnityEngine;

namespace CardGame {
  public class Factory : MonoBehaviour {
    [SerializeField]
    private Transform cardParent;
    [SerializeField]
    private GameObject cardPrefab;

    private static Factory instance;

    private void Awake() {
      instance = this;
    }

    public static Card CreateCard(Vector3 pos) {
      return Instantiate(instance.cardPrefab,
          pos,
          Quaternion.Euler(Config.CardHideRot),
          instance.cardParent).GetComponent<Card>();
    }

    public static Card CreateCard(MCard mCard, CardData cardData) {
      Card card = CreateCard(Vector3.zero);

      if (mCard.IsPlayerCard) {
        if (mCard.IsInBattle) {
          card.Pos = CardUtils.GetPlayerBattlePos();
        }
        else {
          card.Pos = CardUtils.GetPlayerWaitingPos(mCard.Slot);
        }
        card.ToggleFront(true);
        card.isPlayer = true;
      }
      else {
        if (mCard.IsInBattle) {
          card.Pos = CardUtils.GetAIBattlePos();
          card.ToggleFront(true);
        }
        else {
          card.Pos = CardUtils.GetAIWaitingPos(mCard.Slot);
          card.ToggleFront(false);
        }
      }
      card.UpdateData(cardData);
      card.UpdateUI();
      card.UpdateInfo(mCard);
      return card;
    }
  }
}