using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CardGame {
  public class GameManager : MonoBehaviour {
    [SerializeField]
    private GameData gameData;

    [SerializeField]
    private CardAnimation cardAnim;
    [SerializeField]
    private GameView view;
    [SerializeField]
    private UserInput input;

    private new Camera camera;
    private CardData[] cardDatas;
    private Card[] availableCards;
    private List<Card> playingCards;
    private HashSet<Card> playerCards;
    private HashSet<Card> aiCards;

    private Card currentPlayerCard;
    private Card currentAICard;

    private int playerPoint;
    private int aiPoint;

    private bool showResult;
    public bool isSelectingCard;
    public bool isAnimating;
    public bool isPlaying;
    public bool isPause;
    public bool isBattling;

    public bool isPlayerDrawing;
    public bool isAIDrawing;
    public bool isPlayerAttacking;

    #region main functions
    private void Awake() {
      camera = Camera.main;
      playerCards = new HashSet<Card>();
      aiCards = new HashSet<Card>();
      playingCards = new List<Card>();
      cardDatas = Resources.LoadAll<CardData>("CardData");
      new System.Random().Shuffle(cardDatas);
    }

    private void Start() {
      if(gameData.isNewGame) StartCoroutine(CreateNewGame());
      else StartCoroutine(LoadGame());
    }

    private void Update() {
      input.GameUpdate(Time.deltaTime);
      if (showResult) {
        ShowGameResult();
      }
      if(input.IsExit && isPlaying) {
        PauseGame();
      }
      if (isAnimating) return;
      if (input.IsMouseClick) {
        if (isSelectingCard) {
          Card selected = FindCardUnderMouse();
          if (selected != null) StartCoroutine(SelectPlayerCardForDeck(selected));

        }
        if (isPlaying && isPlayerDrawing) {
          Card selected = FindCardUnderMouse();
          if (selected != null && selected.isPlayer) {
            StartCoroutine(PlayerSelectCardForBattle(selected));
          }
        }
      }
      if(isPlaying && !isBattling && currentAICard != null && currentPlayerCard != null) {
        StartCoroutine(ProcessBattle());
      }
      if (isAIDrawing) {
        StartCoroutine(AISelectCardForBattle());
      }
    }
    #endregion

    #region game logics
    private Card FindCardUnderMouse() {
      RaycastHit hitInfo = new RaycastHit();
      bool hit = Physics.Raycast(camera.ScreenPointToRay(Input.mousePosition), out hitInfo, 30, Config.CardLayer);
      if (hit) {
        if (hitInfo.transform.CompareTag(Config.CardTag)) {
          return hitInfo.transform.GetComponent<Card>();
        }
      }
      return null;
    }

    private void ShowGameResult() {
      showResult = false;
      view.ShowResultBattlePanel(playerPoint > aiPoint);
    }

    private IEnumerator CreateNewGame() {
      CreateCards();
      yield return null;
      yield return StartCoroutine(cardAnim.Animate(availableCards, camera.gameObject));
      ClearCards();
      GetUsedCards();
      view.ShowSelectCardPanel();
      isSelectingCard = true;
    }

    private IEnumerator LoadGame() {
      LoadCards();
      camera.transform.position = Config.CamFinalPos;
      camera.transform.eulerAngles = Config.CamFinalRot;
      playerPoint = Config.CardPerPlayer - aiCards.Count;
      aiPoint = Config.CardPerPlayer - playerCards.Count;
      view.UpdatePlayerPoint(playerPoint);
      view.UpdateAIPoint(aiPoint);
      yield return new WaitForSeconds(1);
      if (playerPoint == Config.CardPerPlayer) {
        showResult = true;
        view.ShowResultBattlePanel(true);
      }
      else if (aiPoint == Config.CardPerPlayer) {
        showResult = true;
        view.ShowResultBattlePanel(false);
      }
      else {
        isPlaying = true;
        if (isBattling) StartCoroutine(ProcessBattle());
      }
    }

    private IEnumerator ProcessBattle() {
      isBattling = true;
      var waitTime = new WaitForSeconds(0.75f);
      yield return waitTime;
      while (currentAICard != null && currentPlayerCard != null) {
        if(isPlayerAttacking) {
          yield return StartCoroutine(cardAnim.PlayerCardAttack(currentPlayerCard));
          currentAICard.ReduceHp(currentPlayerCard.Atk);
          if (currentAICard.IsDeath) {
            currentAICard.Hide();
            aiCards.Remove(currentAICard);
            currentAICard = null;
            playerPoint++;
            view.UpdatePlayerPoint(playerPoint);
            if (aiCards.Count > 0) {
              isAIDrawing = true;
            }
            else {
              showResult = true;
            }
          }
          else {
            isPlayerAttacking = false;
          }
        }
        else {
          yield return StartCoroutine(cardAnim.AICardAttack(currentAICard));
          currentPlayerCard.ReduceHp(currentAICard.Atk);
          if (currentPlayerCard.IsDeath) {
            currentPlayerCard.Hide();
            playerCards.Remove(currentPlayerCard);
            currentPlayerCard = null;
            aiPoint++;
            view.UpdateAIPoint(aiPoint);
            if (playerCards.Count > 0) {
              isPlayerDrawing = true;
            }
            else {
              showResult = true;
            }
          }
          else {
            isPlayerAttacking = true;
          }
        }
        yield return waitTime;
      }
      if (showResult) isPlaying = false;
      isBattling = false;
    }

    private IEnumerator PlayerSelectCardForBattle(Card selected) {
      isAnimating = true;
      yield return StartCoroutine(cardAnim.MoveCardToPlayerFightPos(selected));
      isPlayerDrawing = false;//for saving data
      currentPlayerCard = selected;
      isPlayerAttacking = true;
      isAnimating = false;
    }

    private IEnumerator AISelectCardForBattle() {
      isAnimating = true;
      var card = GetAICardFromDeck();
      yield return StartCoroutine(cardAnim.MoveCardToAIFightPos(card));
      isAIDrawing = false;//for saving data
      card.ToggleFront(true);
      currentAICard = card;
      isPlayerAttacking = false;
      isAnimating = false;
    }

    private Card GetAICardFromDeck() {
      int value = Random.Range(0, aiCards.Count);
      int i = 0;
      foreach(var card in aiCards) {
        if (i == value) return card;
        i++;
      }
      return null;
    }

    private IEnumerator SelectPlayerCardForDeck(Card selected) {
      isAnimating = true;
      selected.slot = playerCards.Count;
      selected.isPlayer = true;
      playerCards.Add(selected);
      yield return StartCoroutine(cardAnim.AnimateUserSelectCard(selected, selected.slot));
      selected.ToggleFront(true);
      if(playerCards.Count == Config.AllowedCardAmount / 2) {
        GetAICards();
        yield return StartCoroutine(cardAnim.AnimateAISelectCard(aiCards));
        isSelectingCard = false;
        //show start battle
        yield return StartCoroutine(view.ShowStartBattlePanel());
        //AI draw first
        yield return StartCoroutine(AISelectCardForBattle());
        isPlayerDrawing = true;
        isPlaying = true;
      }
      isAnimating = false;
    }

    private void GetAICards() {
      int slot = 0;
      foreach (var card in playingCards) {
        if (!playerCards.Contains(card)) {
          aiCards.Add(card);
          card.slot = slot;
          slot++;
        }
      }
    }

    private void GetUsedCards() {
      for (int i = 0; i < Config.AllowedCardAmount; i++) {
        playingCards.Add(availableCards[i]);
        availableCards[i].UpdateUI();
      }
      for (int i = Config.AllowedCardAmount; i < availableCards.Length; i++) {
        Destroy(availableCards[i].gameObject);
      }
      availableCards = null;
    }

    private void ClearCards() {
      playerCards.Clear();
      aiCards.Clear();
      playingCards.Clear();
    }

    private void CreateCards() {
      availableCards = new Card[cardDatas.Length];
      for (int i = 0; i < cardDatas.Length; i++) {
        Card card = Factory.CreateCard(Config.DistributePosition + new Vector3(0, 10 + Config.CardThickness * i));
        card.UpdateData(cardDatas[i]);
        availableCards[i] = card;
      }
    }

    private void LoadCards() {
      var map = new Dictionary<int, CardData>();
      foreach(var cardData in cardDatas) {
        map.Add(cardData.id, cardData);
      }
      playerCards.Clear();
      aiCards.Clear();
      GameInfo gameInfo = DataUtils.LoadData();
      isBattling = gameInfo.isBattling;
      isPlayerAttacking = gameInfo.isPlayerAttacking;
      isPlayerDrawing = gameInfo.isPlayerDrawing;
      isAIDrawing = gameInfo.isAIDrawing;
      foreach (var cardInfo in gameInfo.cards) {
        Card card = Factory.CreateCard(Vector3.zero);

        if (cardInfo.isPlayerCard) {
          if (cardInfo.isInBattle) {
            currentPlayerCard = card;
            card.Pos = CardUtils.GetPlayerBattlePos();
          }
          else {
            card.Pos = CardUtils.GetPlayerWaitingPos(cardInfo.slot);
          }
          card.ToggleFront(true);
          playerCards.Add(card);
          card.isPlayer = true;
        }
        else {
          if (cardInfo.isInBattle) {
            currentAICard = card;
            card.Pos = CardUtils.GetAIBattlePos();
            card.ToggleFront(true);
          }
          else {
            card.Pos = CardUtils.GetAIWaitingPos(cardInfo.slot);
            card.ToggleFront(false);
          }
          aiCards.Add(card);
        }
        card.UpdateData(map[cardInfo.id]);
        card.UpdateUI();
        card.UpdateInfo(cardInfo);
      }
    }
    #endregion

    #region game flow
    public void PauseGame() {
      isPause = true;
      Time.timeScale = 0;
      view.ShowPausePanel();
    }

    public void Resume() {
      isPause = false;
      Time.timeScale = 1;
      view.HidePausePanel();
    }

    public void QuitAndSave() {
      Time.timeScale = 1;
      DataUtils.SaveData(currentPlayerCard, currentAICard, playerCards, aiCards,
          isBattling, isPlayerAttacking, isPlayerDrawing, isAIDrawing);
      Return();
    }

    public void Return() {
      Time.timeScale = 1;
      UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
    #endregion
  }
}