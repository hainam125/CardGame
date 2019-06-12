
namespace CardGame {
  public class AIAttack : State {
    public override void Enter(GameManager game) {
      game.gameState = GameState.AIAttack;
      game.AIAttack();
    }
  }
}
