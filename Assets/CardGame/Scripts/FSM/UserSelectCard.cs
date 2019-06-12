
namespace CardGame {
  public class UserSelectCard : State {
    public override void Enter(GameManager game) {
      game.gameState = GameState.UserSelectCard;
    }

    public override void Execute(GameManager game) {
      Card selected = game.FindCardUnderMouse();
      if (selected != null) game.PlayerSelectCardForBattle(selected);
    }
  }
}
