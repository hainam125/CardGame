
namespace CardGame {
  public class AISelectCard : State {
    public override void Enter(GameManager game) {
      game.gameState = GameState.AISelectCard;
      game.AISelectCardForBattle();
    }
  }
}
