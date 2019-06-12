using System;

namespace CardGame {
  public class Transition {
    public State next;
    public Func<bool> condition;

    public bool Check() {
      return condition.Invoke();
    }
  }
}
