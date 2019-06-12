
namespace CardGame {
  public class UserAttack : State {
    public override void Enter(GameManager game) {
      game.gameState = GameState.UserAttack;
      game.PlayerAttack();
    }
  }
}
