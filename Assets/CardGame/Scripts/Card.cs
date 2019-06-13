using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CardGame {
  public class Card : MonoBehaviour {
    [SerializeField]
    private CardData data;

    [SerializeField]
    private GameObject front;
    [SerializeField]
    private GameObject back;

    [SerializeField]
    private Image avatarImg;
    [SerializeField]
    private Text nameText;
    [SerializeField]
    private Text descText;
    [SerializeField]
    private Text atkText;
    [SerializeField]
    private Text hpText;

    private Transform mTransform;

    public bool isPlayer;
    public int slot;

    public bool IsDeath { get { return Hp <= 0; } }
    public int Id { get { return data.id; } }
    public string Name { get { return data.name; } }
    public string Desc { get { return data.desc; } }
    public int Atk { get { return data.attack; } }
    public int Hp { get; private set; }

    public Vector3 Pos { 
      get { return mTransform.position; }
      set { mTransform.position = value; }
    }

    public Vector3 Rot {
      get { return mTransform.rotation.eulerAngles; }
      set { mTransform.eulerAngles = value; }
    }

    private void Awake() {
      mTransform = transform;
    }

    public void UpdateUI() {
      avatarImg.sprite = data.avatar;
      nameText.text = data.name;
      descText.text = data.desc;
      atkText.text = data.attack.ToString();
      hpText.text = data.hp.ToString();
    }

    public void UpdateData(CardData cardData) {
      data = cardData;
      Hp = data.hp;
    }

    public void UpdateInfo(MCard info) {
      Hp = info.Hp;
      slot = info.Slot;
      hpText.text = Hp.ToString();
    }

    public void ReduceHp(int dam) {
      Hp -= dam;
      if (Hp < 0) Hp = 0;
      hpText.text = Hp.ToString();
    }

    public void Hide() {
      gameObject.SetActive(false);
    }

    public void ToggleFront(bool show) {
      if(show) Rot = Config.CardShowRot;
      else Rot = Config.CardHideRot;
      front.SetActive(show);
      back.SetActive(!show);
    }
  }
}