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

    public GameState gameState;
    private Dictionary<GameState, State> states;

    private CardData[] cardDatas;
    private Card[] availableCards;
    private List<Card> playingCards;
    private HashSet<Card> userCards;
    private HashSet<Card> aiCards;

    private Card currentUserCard;
    private Card currentAICard;

    private int userPoint;
    private int aiPoint;

    private StateMachine FSM;

    private bool isDeliveringCard;
    private bool isAISelectingDeck;
    private bool isAISelectingCard;
    private bool isUserSelectingCard;
    private bool isPlayerAttacking;
    private bool isAIAttacking;

    private bool isAnimating;
    private bool isPlaying;


    #region main functions
    private void Awake() {
      camera = Camera.main;
      userCards = new HashSet<Card>();
      aiCards = new HashSet<Card>();
      playingCards = new List<Card>();
      states = new Dictionary<GameState, State>();
      cardDatas = Resources.LoadAll<CardData>("CardData");
      new System.Random().Shuffle(cardDatas);
    }

    private IEnumerator Start() {
      if (gameData.isNewGame) {
        CreateCards();
        CreateStateMachine(GameState.DeliverCard);
      }
      else {
        LoadCards();
        LoadGame();
      }
      yield return null;
    }

    private void Update() {
      input.GameUpdate(Time.deltaTime);
      if(input.IsExit) {
        PauseGame();
      }
      FSM.Update();
    }
    #endregion

    #region game logics

    private void CreateStateMachine(GameState initState) {
      FSM = new StateMachine(this);
      var deliverCardState = new DeliverCard();
      states.Add(GameState.DeliverCard, deliverCardState);
      var userSelectDeck = new UserSelectDeck();
      states.Add(GameState.UserSelectDeck, userSelectDeck);
      var userSelectCard = new UserSelectCard();
      states.Add(GameState.UserSelectCard, userSelectCard);
      var aiSelectDeck = new AISelectDeck();
      states.Add(GameState.AISelectDeck, aiSelectDeck);
      var aiSelectCard = new AISelectCard();
      states.Add(GameState.AISelectCard, aiSelectCard);
      var userAttack = new UserAttack();
      states.Add(GameState.UserAttack, userAttack);
      var aiAttack = new AIAttack();
      states.Add(GameState.AIAttack, aiAttack);
      var startGame = new StartBattle();
      states.Add(GameState.StartBattle, startGame);
      var endGame = new EndGame();
      states.Add(GameState.EndGame, endGame);

      FSM.AddTransition(deliverCardState, new Transition() {
        next = userSelectDeck,
        condition = () => !isDeliveringCard
      });
      FSM.AddTransition(userSelectDeck, new Transition() {
        next = aiSelectDeck,
        condition = () => userCards.Count == Config.CardPerPlayer
      });
      FSM.AddTransition(aiSelectDeck, new Transition() {
        next = startGame,
        condition = () => !isAISelectingDeck
      });

      FSM.AddTransition(startGame, new Transition() {
        next = aiSelectCard,
        condition = () => isPlaying
      });

      FSM.AddTransition(aiSelectCard, new Transition() {
        next = userSelectCard,
        condition = () => currentUserCard == null && !isAISelectingCard
      });

      FSM.AddTransition(userSelectCard, new Transition() {
        next = userAttack,
        condition = () => currentUserCard != null && !isUserSelectingCard
      });
      FSM.AddTransition(userAttack, new Transition() {
        next = aiAttack,
        condition = () => currentAICard != null && !isPlayerAttacking
      });
      FSM.AddTransition(userAttack, new Transition() {
        next = aiSelectCard,
        condition = () => currentAICard == null && aiCards.Count > 0 && !isPlayerAttacking
      });
      FSM.AddTransition(userAttack, new Transition() {
        next = endGame,
        condition = () => currentAICard == null && aiCards.Count == 0 && !isPlayerAttacking
      });
      FSM.AddTransition(aiAttack, new Transition() {
        next = userAttack,
        condition = () => currentUserCard != null && !isAIAttacking
      });
      FSM.AddTransition(aiAttack, new Transition() {
        next = userSelectCard,
        condition = () => currentUserCard == null && userCards.Count > 0 && !isAIAttacking
      });
      FSM.AddTransition(aiAttack, new Transition() {
        next = endGame,
        condition = () => currentUserCard == null && userCards.Count == 0 && !isAIAttacking
      });

      FSM.AddTransition(aiSelectCard, new Transition() {
        next = aiAttack,
        condition = () => currentAICard != null && !isAISelectingCard
      });

      FSM.Start(states[initState]);
    }

    public void DeliverCard() {
      StartCoroutine(IDeliverCard());
    }

    private IEnumerator IDeliverCard() {
      isDeliveringCard = true;
      yield return StartCoroutine(cardAnim.Animate(availableCards, camera.gameObject));
      ClearCards();
      GetUsedCards();
      view.ShowSelectCardPanel();
      isDeliveringCard = false;
    }

    public void PlayerSelectCardForDeck(Card selected) {
      StartCoroutine(IPlayerSelectCardForDeck(selected));
    }

    private IEnumerator IPlayerSelectCardForDeck(Card selected) {
      isAnimating = true;
      selected.slot = userCards.Count;
      selected.isPlayer = true;
      userCards.Add(selected);
      yield return StartCoroutine(cardAnim.AnimateUserSelectCard(selected, selected.slot));
      selected.ToggleFront(true);
      isAnimating = false;
    }

    public void AISelectCardForDeck() {
      StartCoroutine(IAISelectCardForDeck());
    }

    private IEnumerator IAISelectCardForDeck() {
      isAnimating = true;
      isAISelectingDeck = true;
      GetAICards();
      yield return StartCoroutine(cardAnim.AnimateAISelectCard(aiCards));
      isAISelectingDeck = false;
      isAnimating = false;
    }

    public void AISelectCardForBattle() {
      StartCoroutine(IAISelectCardForBattle());
    }

    private IEnumerator IAISelectCardForBattle() {
      isAnimating = true;
      isAISelectingCard = true;
      var card = GetAICardFromDeck();
      yield return StartCoroutine(cardAnim.MoveCardToAIFightPos(card));
      card.ToggleFront(true);
      currentAICard = card;
      isAISelectingCard = false;
      isAnimating = false;
    }

    public void PlayerSelectCardForBattle(Card selected) {
      StartCoroutine(IPlayerSelectCardForBattle(selected));
    }

    private IEnumerator IPlayerSelectCardForBattle(Card selected) {
      isAnimating = true;
      isUserSelectingCard = true;
      yield return StartCoroutine(cardAnim.MoveCardToPlayerFightPos(selected));
      currentUserCard = selected;
      isUserSelectingCard = false;
      isAnimating = false;
    }

    public void PlayerAttack() {
      StartCoroutine(IPlayerAttack());
    }

    private IEnumerator IPlayerAttack() {
      isAnimating = true;
      isPlayerAttacking = true;
      yield return StartCoroutine(cardAnim.PlayerCardAttack(currentUserCard));
      currentAICard.ReduceHp(currentUserCard.Atk);
      if (currentAICard.IsDeath) {
        currentAICard.Hide();
        aiCards.Remove(currentAICard);
        currentAICard = null;
        userPoint++;
        view.UpdatePlayerPoint(userPoint);
      }
      isPlayerAttacking = false;
      isAnimating = false;
    }

    public void AIAttack() {
      StartCoroutine(IAIAttack());
    }

    private IEnumerator IAIAttack() {
      isAnimating = true;
      isAIAttacking = true;
      yield return StartCoroutine(cardAnim.AICardAttack(currentAICard));
      currentUserCard.ReduceHp(currentAICard.Atk);
      if (currentUserCard.IsDeath) {
        currentUserCard.Hide();
        userCards.Remove(currentUserCard);
        currentUserCard = null;
        aiPoint++;
        view.UpdateAIPoint(aiPoint);
      }
      isAIAttacking = false;
      isAnimating = false;
    }

    public void ShowGameResult() {
      view.ShowResultBattlePanel(userPoint > aiPoint);
    }

    public void StartBattle() {
      StartCoroutine(view.ShowStartBattlePanel());
    }
    #endregion

    #region Helpers
    public Card FindCardUnderMouse() {
      if (isAnimating || !input.IsMouseClick) return null;
      RaycastHit hitInfo = new RaycastHit();
      bool hit = Physics.Raycast(camera.ScreenPointToRay(Input.mousePosition), out hitInfo, 30, Config.CardLayer);
      if (hit) {
        if (hitInfo.transform.CompareTag(Config.CardTag)) {
          return hitInfo.transform.GetComponent<Card>();
        }
      }
      return null;
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

    private void GetAICards() {
      int slot = 0;
      foreach (var card in playingCards) {
        if (!userCards.Contains(card)) {
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
      userCards.Clear();
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
      foreach (var cardData in cardDatas) {
        map.Add(cardData.id, cardData);
      }
      userCards.Clear();
      aiCards.Clear();
      MGame mGame = DataUtils.LoadData();
      gameState = (GameState)mGame.GameState;
      for(int i = 0; i < mGame.CardsLength; i++) {
        var mCard = mGame.Cards(i).Value;
        Card card = Factory.CreateCard(mCard, map[mCard.Id]);
        if (card.isPlayer) {
          userCards.Add(card);
          if (mCard.IsInBattle) currentUserCard = card;
        }
        else {
          aiCards.Add(card);
          if (mCard.IsInBattle) currentAICard = card;
        }
      }
    }

    private void LoadGame() {
      camera.transform.position = Config.CamFinalPos;
      camera.transform.eulerAngles = Config.CamFinalRot;
      userPoint = Config.CardPerPlayer - aiCards.Count;
      aiPoint = Config.CardPerPlayer - userCards.Count;
      view.UpdatePlayerPoint(userPoint);
      view.UpdateAIPoint(aiPoint);
      CreateStateMachine(gameState);
    }
    #endregion

    #region game flow
    public void StartPlay() {
      isPlaying = true;
    }

    public void PauseGame() {
      Time.timeScale = 0;
      view.ShowPausePanel();
    }

    public void Resume() {
      Time.timeScale = 1;
      view.HidePausePanel();
    }

    public void QuitAndSave() {
      Time.timeScale = 1;
      if (gameState > GameState.StartBattle) {
        DataUtils.SaveData(currentUserCard, currentAICard, userCards, aiCards, gameState);
      }
      Return();
    }

    public void Return() {
      Time.timeScale = 1;
      UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
    #endregion
  }
}