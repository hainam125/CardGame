using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.IO;
using System.Linq;
using System.Text;

namespace CardGame {
  public class GameInfo {
    public bool isBattling;
    public bool isPlayerAttacking;
    public bool isPlayerDrawing;
    public bool isAIDrawing;
    public List<CardInfo> cards = new List<CardInfo>();
  }

  public class CardInfo {
    public int id;
    public int slot;
    public int hp;
    public bool isPlayerCard;
    public bool isInBattle;

    public CardInfo(XmlNode node) {
      id = int.Parse(node.Attributes["id"].Value);
      slot = int.Parse(node.Attributes["slot"].Value);
      hp = int.Parse(node.Attributes["hp"].Value);
      isPlayerCard = node.Attributes["player"].Value == "1";
      isInBattle = node.Attributes["battle"].Value == "1";
    }
  }
  public static class DataUtils {
    private static string DataPath = Path.Combine(Application.persistentDataPath, "data");

    public static bool HasSavedData() {
      return File.Exists(DataPath);
    }

    public static GameInfo LoadData() {
      if (!HasSavedData()) return null;
      XmlDocument xmlDoc = new XmlDocument();
      GameInfo gameInfo = new GameInfo();
      xmlDoc.Load(DataPath);
      XmlNode levelNode = xmlDoc.GetElementsByTagName("level").Item(0);
      gameInfo.isBattling = levelNode.Attributes["battle"].Value == "1";
      gameInfo.isPlayerAttacking = levelNode.Attributes["playerAtk"].Value == "1";
      gameInfo.isPlayerDrawing = levelNode.Attributes["playerDrawing"].Value == "1";
      gameInfo.isAIDrawing = levelNode.Attributes["AIDrawing"].Value == "1";

      XmlNode cardBlockNode = xmlDoc.GetElementsByTagName("cards").Item(0);
      XmlNodeList cards = cardBlockNode.ChildNodes;
      foreach (XmlNode card in cards) {
        gameInfo.cards.Add(new CardInfo(card));
      }
      return gameInfo;
    }

    public static void SaveData(Card currentPlayerCard, Card currentAICard, HashSet<Card> playerCards, HashSet<Card> aiCards,
        bool isBattling, bool isPlayerAttacking, bool isPlayerDrawing, bool isAIDrawing) {

      if (!HasSavedData()) {
        var file = File.Create(DataPath);
        file.Dispose();
      }
      XmlDocument xmlDoc = new XmlDocument();
      XmlElement elmRoot = xmlDoc.CreateElement("level");
      elmRoot.SetAttribute("battle", isBattling ? "1" : "0");
      elmRoot.SetAttribute("playerAtk", isPlayerAttacking ? "1" : "0");
      elmRoot.SetAttribute("playerDrawing", isPlayerDrawing ? "1" : "0");
      elmRoot.SetAttribute("AIDrawing", isAIDrawing ? "1" : "0");
      xmlDoc.AppendChild(elmRoot);

      XmlElement cardBlockElm = xmlDoc.CreateElement("cards");
      elmRoot.AppendChild(cardBlockElm);
      foreach(var card in playerCards) {
        XmlElement elmCard = CreateCardElm(card, xmlDoc, true, card == currentPlayerCard);
        cardBlockElm.AppendChild(elmCard);
      }
      foreach (var card in aiCards) {
        XmlElement elmCard = CreateCardElm(card, xmlDoc, false, card == currentAICard);
        cardBlockElm.AppendChild(elmCard);
      }
      xmlDoc.Save(DataPath);
    }

    public static XmlElement CreateCardElm(Card card, XmlDocument xmlDoc, bool isUser, bool isBattling) {
      XmlElement elmCard = xmlDoc.CreateElement("card");
      elmCard.SetAttribute("id", card.Id.ToString());
      elmCard.SetAttribute("slot", card.slot.ToString());
      elmCard.SetAttribute("hp", card.Hp.ToString());
      elmCard.SetAttribute("player", isUser ? "1" : "0");
      elmCard.SetAttribute("battle", isBattling ? "1" : "0");
      return elmCard;
    }
  }

}
