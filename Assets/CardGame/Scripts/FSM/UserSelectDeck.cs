
namespace CardGame {
  public class UserSelectDeck : State {
    public override void Enter(GameManager game) {
      game.gameState = GameState.UserSelectDeck;
    }

    public override void Execute(GameManager game) {
      Card selected = game.FindCardUnderMouse();
      if (selected != null) game.PlayerSelectCardForDeck(selected);
    }
  }
}