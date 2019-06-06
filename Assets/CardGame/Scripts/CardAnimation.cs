using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CardGame {
  public class CardAnimation : MonoBehaviour {

    #region card battle
    public IEnumerator PlayerCardAttack(Card card) {
      iTween.MoveTo(card.gameObject, iTween.Hash("x", 0, "easeType", "easeInOutExpo", "delay", .1));
      yield return new WaitForSeconds(0.75f);
      yield return StartCoroutine(MoveCardToPlayerFightPos(card));
    }

    public IEnumerator AICardAttack(Card card) {
      iTween.MoveTo(card.gameObject, iTween.Hash("x", 0, "easeType", "easeInOutExpo", "delay", .1));
      yield return new WaitForSeconds(0.75f);
      yield return StartCoroutine(MoveCardToAIFightPos(card));
    }
    #endregion

    #region pick and draw cards
    public IEnumerator MoveCardToPlayerFightPos(Card card) {
      var pos = CardUtils.GetPlayerBattlePos();
      iTween.MoveTo(card.gameObject,
          iTween.Hash("x", pos.x, "z", pos.z, "easeType", "easeInOutExpo", "delay", .1));
      yield return new WaitForSeconds(1f);
    }

    public IEnumerator MoveCardToAIFightPos(Card card) {
      var pos = CardUtils.GetAIBattlePos();
      iTween.RotateTo(card.gameObject,
          iTween.Hash("x", 90, "easeType", "easeInOutExpo", "delay", .1));
      iTween.MoveTo(card.gameObject,
          iTween.Hash("x", pos.x, "z", pos.z, "easeType", "easeInOutExpo", "delay", .1));
      yield return new WaitForSeconds(0.75f);
    }

    public IEnumerator AnimateUserSelectCard(Card card, int slot) {
      yield return new WaitForSeconds(0.25f);
      var pos = CardUtils.GetPlayerWaitingPos(slot);
      iTween.MoveTo(card.gameObject,
          iTween.Hash("x", pos.x, "y", pos.y, "z", pos.z, "easeType", "easeInOutExpo", "delay", .1));
      iTween.RotateTo(card.gameObject,
          iTween.Hash("x", 90, "easeType", "easeInOutExpo", "delay", .1));
      yield return new WaitForSeconds(0.75f);
    }

    public IEnumerator AnimateAISelectCard(HashSet<Card> cards) {
      yield return new WaitForSeconds(0.25f);
      int i = 0;
      foreach (var card in cards) {
        var pos = CardUtils.GetAIWaitingPos(i);
        iTween.MoveTo(card.gameObject,
          iTween.Hash("x", pos.x, "y", pos.y, "z", pos.z, "easeType", "easeInOutExpo", "delay", .1));
        i++;
        yield return new WaitForSeconds(0.7f);
      }

      yield return new WaitForSeconds(0.7f);
    }
    #endregion

    #region showCards
    public IEnumerator Animate(Card[] availableCards, GameObject camera) {
      yield return StartCoroutine(AnimateShowCards(availableCards));
      yield return new WaitForSeconds(1f);
      yield return StartCoroutine(AnimateMixCards(availableCards));
      yield return new WaitForSeconds(0.75f);
      yield return StartCoroutine(DistributeCards(availableCards));
      yield return new WaitForSeconds(0.5f);
      yield return StartCoroutine(HideRemainingCards(availableCards));
      yield return new WaitForSeconds(0.5f);
      yield return StartCoroutine(MoveCamera(camera));
    }

    private IEnumerator AnimateShowCards(Card[] availableCards) {
      for (int i = 0; i < availableCards.Length; i++) {
        yield return null;
        iTween.MoveTo(availableCards[i].gameObject, iTween.Hash("y", 0 + Config.CardThickness * i, "easeType", "easeInOutExpo", "delay", .2));
      }
    }

    private IEnumerator AnimateMixCards(Card[] availableCards) {
      //go up
      for (int i = 0; i < availableCards.Length; i++) {
        yield return null;
        iTween.MoveTo(availableCards[i].gameObject, iTween.Hash("y", 1.5 + Config.CardThickness * i, "easeType", "easeInOutExpo", "delay", .1));
      }
      yield return new WaitForSeconds(0.5f);
      //split
      for (int i = 0; i < availableCards.Length; i += 2) {
        iTween.MoveTo(availableCards[i].gameObject, iTween.Hash("x", -3.3, "y", 2.25 + Config.CardThickness * i, "easeType", "easeInOutExpo", "delay", .1));
      }
      for (int i = 1; i < availableCards.Length; i += 2) {
        iTween.MoveTo(availableCards[i].gameObject, iTween.Hash("x", 3.3, "y", 2.25 + Config.CardThickness * i, "easeType", "easeInOutExpo", "delay", .1));
      }
      yield return new WaitForSeconds(.5f);
      for (int i = 0; i < availableCards.Length; i += 2) {
        iTween.RotateTo(availableCards[i].gameObject, iTween.Hash("x", -110, "y", 64, "z", -52, "easeType", "easeInOutExpo", "delay", .1));
      }
      for (int i = 1; i < availableCards.Length; i += 2) {
        iTween.RotateTo(availableCards[i].gameObject, iTween.Hash("x", -110, "y", -64, "z", 52, "easeType", "easeInOutExpo", "delay", .1));
      }
      yield return new WaitForSeconds(.5f);
      //combine
      for (int i = 0; i < availableCards.Length; i++) {
        yield return null;
        iTween.MoveTo(availableCards[i].gameObject, iTween.Hash("x", 0, "y", 1.5 + Config.CardThickness * i, "z", Config.DistributePosition.z, "easeType", "easeInOutExpo", "delay", .1));
        iTween.RotateTo(availableCards[i].gameObject, iTween.Hash("x", -90, "y", 0, "z", 0, "easeType", "easeInOutExpo", "delay", .1));
      }
    }

    private IEnumerator DistributeCards(Card[] availableCards) {
      System.Array.Reverse(availableCards);
      int currentRow = 0;
      int currentCol = 0;
      float space = 0.2f;
      for (int i = 0; i < Config.AllowedCardAmount; i++) {
        if (currentCol >= Config.Col) {
          currentRow++;
          currentCol = 0;
        }
        iTween.MoveTo(availableCards[i].gameObject,
          iTween.Hash("z", Config.DistributePosition.z + Config.CardHeight + space, "easeType", "easeInOutExpo", "delay", .1));

        yield return new WaitForSeconds(0.7f);
        iTween.MoveTo(availableCards[i].gameObject,
          iTween.Hash("x", (Config.CardWidth + space) * ((-Config.Col + 1) * 0.5f + currentCol), "y", 0, "z", (Config.CardHeight + space) * ((-Config.Row + 1) * 0.5f + currentRow),
          "easeType", "easeInOutExpo", "delay", .1));
        currentCol++;
        yield return new WaitForSeconds(0.7f);
      }
    }

    private IEnumerator HideRemainingCards(Card[] availableCards) {
      for (int i = Config.AllowedCardAmount; i < availableCards.Length; i++) {
        iTween.MoveTo(availableCards[i].gameObject, iTween.Hash("z", Config.DistributePosition.z - 12, "easeType", "easeInOutExpo", "delay", .2));
      }
      yield return new WaitForSeconds(0.75f);
    }

    public IEnumerator MoveCamera(GameObject camera) {
      var pos = Config.CamFinalPos;
      var rot = Config.CamFinalRot;
      iTween.MoveTo(camera,
        iTween.Hash("x", pos.x, "y", pos.y, "z", pos.z, "easeType", "easeInOutExpo", "delay", .3));
      iTween.RotateTo(camera,
        iTween.Hash("x", rot.x, "y", rot.y, "z", rot.z, "easeType", "easeInOutExpo", "delay", .3));
      yield return new WaitForSeconds(0.5f);
    }
  }
  #endregion
}