
using System.Collections.Generic;

namespace CardGame {
  public class State {
    public virtual void Enter(GameManager game) { }
    public virtual void Execute(GameManager game) { }
    public virtual void Exit(GameManager game) { }
    //public virtual bool OnMessage() { return false; }
  }
}