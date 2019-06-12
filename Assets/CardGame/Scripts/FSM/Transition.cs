using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CardGame {
  public class Transition : MonoBehaviour {
    public State next;
    public Func<bool> condition;

    public bool Check() {
      return condition.Invoke();
    }
  }
}
