
namespace CardGame {
  public class DeliverCard : State {
    public override void Enter(GameManager game) {
      game.gameState = GameState.DeliverCard;
      game.DeliverCard();
    }
  }
}
