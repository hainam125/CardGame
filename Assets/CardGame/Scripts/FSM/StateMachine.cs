using System.Collections.Generic;

namespace CardGame {
  public class StateMachine {
    private GameManager owner;
    private State current;
    private Dictionary<State, List<Transition>> transitions = new Dictionary<State, List<Transition>>();

    public StateMachine(GameManager game) {
      owner = game;
    }

    public void Update() {
      if (current == null || !transitions.ContainsKey(current)) return;
      var transitionList = transitions[current];
      for(int i = 0; i < transitionList.Count; i++) {
        if (transitionList[i].Check()) {
          ChangeState(transitionList[i].next);
          return;
        }
      }
      current.Execute(owner);
    }

    public void AddTransition(State state, Transition transition) {
      if (transitions.ContainsKey(state)) {
        transitions[state].Add(transition);
      }
      else {
        transitions.Add(state, new List<Transition>() { transition });
      }
    }

    public void ChangeState(State state) {
      current.Exit(owner);
      current = state;
      current.Enter(owner);
    }

    public void Start(State state) {
      current = state;
      current.Enter(owner);
    }
  }
}