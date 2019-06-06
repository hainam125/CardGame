using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CardGame {
  public class Factory : MonoBehaviour {
    [SerializeField]
    private Transform cardParent;
    [SerializeField]
    private GameObject cardPrefab;

    private static Factory instance;

    private void Awake() {
      instance = this;
    }

    public static Card CreateCard(Vector3 pos) {
      return Instantiate(instance.cardPrefab,
          pos,
          Quaternion.Euler(new Vector3(-90, 0, 0)),
          instance.cardParent).GetComponent<Card>();
    }
  }
}