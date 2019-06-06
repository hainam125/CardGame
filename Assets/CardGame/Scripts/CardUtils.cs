using UnityEngine;

namespace CardGame {
  public static class CardUtils {
    public static Vector3 GetPlayerWaitingPos(int slot) {
      return new Vector3((Config.CardWidth + Config.CardSpace) * ((-Config.Col + 1) * 0.5f + slot),
              0,
              -(Config.CardHeight + Config.CardSpace) * 1.5f);
    }

    public static Vector3 GetAIWaitingPos(int slot) {
      return new Vector3((Config.CardWidth + Config.CardSpace) * ((-Config.Col + 1) * 0.5f + slot),
              0,
              (Config.CardHeight + Config.CardSpace) * 1.5f);
    }

    public static Vector3 GetPlayerBattlePos() {
      return new Vector3((Config.CardWidth + Config.CardSpace) * 1f, 0);
    }

    public static Vector3 GetAIBattlePos() {
      return new Vector3(-(Config.CardWidth + Config.CardSpace) * 1f, 0);
    }
  }
}