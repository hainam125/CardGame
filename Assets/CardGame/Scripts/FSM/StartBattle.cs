
namespace CardGame {
  public class StartBattle : State {
    public override void Enter(GameManager game) {
      game.gameState = GameState.StartBattle;
      game.StartBattle();
    }
  }
}
