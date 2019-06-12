
namespace CardGame {
  public class EndGame : State {
    public override void Enter(GameManager game) {
      game.gameState = GameState.EndGame;
      game.ShowGameResult();
    }
  }
}
