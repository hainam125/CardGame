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

    public static Card CreateCard(CardInfo cardInfo, CardData cardData) {
      Card card = CreateCard(Vector3.zero);

      if (cardInfo.isPlayerCard) {
        if (cardInfo.isInBattle) {
          card.Pos = CardUtils.GetPlayerBattlePos();
        }
        else {
          card.Pos = CardUtils.GetPlayerWaitingPos(cardInfo.slot);
        }
        card.ToggleFront(true);
        card.isPlayer = true;
      }
      else {
        if (cardInfo.isInBattle) {
          card.Pos = CardUtils.GetAIBattlePos();
          card.ToggleFront(true);
        }
        else {
          card.Pos = CardUtils.GetAIWaitingPos(cardInfo.slot);
          card.ToggleFront(false);
        }
      }
      card.UpdateData(cardData);
      card.UpdateUI();
      card.UpdateInfo(cardInfo);
      return card;
    }
  }
}