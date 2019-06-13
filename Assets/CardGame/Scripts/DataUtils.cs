using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.IO;
using FlatBuffers;

namespace CardGame {
  public static class DataUtils {
    private static string DataPath = Path.Combine(Application.persistentDataPath, "data");

    public static bool HasSavedData() {
      return File.Exists(DataPath);
    }

    public static MGame LoadData() {
      byte[] bytes;
      using (var file = new FileStream(DataPath, FileMode.Open, FileAccess.Read)) {
        bytes = new byte[file.Length];
        file.Read(bytes, 0, bytes.Length);
      }
      var buf = new ByteBuffer(bytes);
      return MGame.GetRootAsMGame(buf);
    }

    public static void SaveData(Card currentPlayerCard, Card currentAICard, HashSet<Card> playerCards, HashSet<Card> aiCards,
        GameState gameState) {

      if (!HasSavedData()) {
        var file = File.Create(DataPath);
        file.Dispose();
      }

      var builder = new FlatBufferBuilder(1024);
      var cards = new Offset<MCard>[playerCards.Count + aiCards.Count];
      int i = 0;
      foreach(var card in playerCards) {
        var mCard = MCard.CreateMCard(builder, card.slot, card.Id, card.Hp, true, card == currentPlayerCard);
        cards[i] = mCard;
        i++;
      }
      foreach (var card in aiCards) {
        var mCard = MCard.CreateMCard(builder, card.slot, card.Id, card.Hp, false, card == currentAICard);
        cards[i] = mCard;
        i++;
      }

      var cardsOffset = MGame.CreateCardsVector(builder, cards);

      var game = MGame.CreateMGame(builder, (int)gameState, cardsOffset);
      builder.Finish(game.Value);

      using (var ms = new MemoryStream(builder.SizedByteArray())) {
        File.WriteAllBytes(DataPath, ms.ToArray());
        Debug.Log("SAVED !");
      }
    }
  }

}
