
namespace CardGame {
  public class AISelectDeck : State {
    public override void Enter(GameManager game) {
      game.gameState = GameState.AISelectDeck;
      game.AISelectCardForDeck();
    }
  }
}
