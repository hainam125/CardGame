using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CardGame {
  public static class Config {
    public const int CardPerPlayer = 5;
    public const int AllowedCardAmount = CardPerPlayer * 2;
    public static Vector3 DistributePosition = new Vector3(0, 0, -6f);
    public const float CardThickness = 0.02f;
    public const float CardHeight = 4.6f;
    public const float CardWidth = 3f;
    public const float CardSpace = 0.5f;
    public const int Row = 2;
    public const int Col = Config.AllowedCardAmount / Row;
    public const string CardTag = "Card";
    public const int CardLayer = 1 << 8;

    public static Vector3 CamFinalPos = new Vector3(0, 20, 0);
    public static Vector3 CamFinalRot = new Vector3(90, 0, 0);
    public static Vector3 CardHideRot = new Vector3(-90, 0, 0);
    public static Vector3 CardShowRot = new Vector3(90, 0, 0);
  }
}
