﻿
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AppConfig;
using System;
using System.Linq;
using UnityEngine.Events;
using DG.Tweening;

public enum Align_Anchor
{
	NONE,
	LEFT,
	RIGHT,
	CENTER,
	TOP,
	BOT}
;

public class ArrayCard : MonoBehaviour
{
	public Align_Anchor align_Anchor = Align_Anchor.LEFT;
	public float MaxWidth;
	public int CardCount;
	public bool isSmall = false;
	public bool isTouched = true;
	[HideInInspector]
	public List<Card> listCardHand;
	[HideInInspector]
	public List<int> listIdCardHand;
	private Vector3 CENTER_SCREEN = new Vector3 (Screen.width / 2, Screen.height / 2, 0);
	public Vector3 vtPosCenter;
	float w_card, h_card;
	bool IsInit = false;
	public void Init ()
	{
		if (!IsInit) {
			IsInit = true;
			listCardHand = new List<Card> ();
			listIdCardHand = new List<int> ();
			vtPosCenter = transform.InverseTransformPoint (CENTER_SCREEN);
			if (GameControl.instance.objCard != null) {
				GameObject objCard = GameControl.instance.objCard;
				for (int i = 0; i < CardCount; i++) {
					GameObject obj = Instantiate (objCard);
					obj.transform.SetParent (transform);
					obj.transform.localScale = Vector3.one;
					obj.transform.localPosition = Vector3.zero;
					Card card = obj.GetComponent<Card> ();
					card.SetCardWithId (53);
					card.setSmall (isSmall);
					card.SetTouched (isTouched);
					card.SetIsCardMauBinh ();
					if (i == 0) {
						w_card = card.W_Card;
						h_card = card.H_Card;
					}
					card.SetVisible (false);
					listCardHand.Add (card);
				}
			}

			SetPositonCardHand ();
		}
	}

	public void InitDemo (int[] arrcard)
	{
		if (arrcard == null || arrcard.Length <= 0)
			return;
		listCardHand = new List<Card> ();
		listIdCardHand = new List<int> ();

		LoadAssetBundle.LoadPrefab (BundleName.PREFAPS, PrefabsName.PRE_CARD, (objPre) => {
			for (int i = 0; i < arrcard.Length; i++) {
				GameObject obj = Instantiate (objPre);
				obj.transform.SetParent (transform);
				obj.transform.localScale = Vector3.one;
				obj.transform.localPosition = new Vector3 (i * 60, 0, 0);
				Card card = obj.GetComponent<Card> ();
				card.SetCardWithId (arrcard [i]);
				card.setSmall (isSmall);
				card.SetTouched (true);
				if (i == 0) {
					w_card = card.W_Card;
					h_card = card.H_Card;
				}
				//card.SetVisible(false);
				card.SetVisible (true);
				listIdCardHand.Add (arrcard [i]);
				listCardHand.Add (card);
			}
			Destroy (objPre);
			//SetCardKhiKetThucGame(arrcard);
		});


	}

	public void InitKhiVaoBanDangDanh ()
	{
		listCardHand = new List<Card> ();
		listIdCardHand = new List<int> ();

		if (GameControl.instance.objCard != null) {
			GameObject objCard = GameControl.instance.objCard;
			for (int i = 0; i < CardCount; i++) {
				GameObject obj = Instantiate (objCard);
				obj.transform.SetParent (transform);
				obj.transform.localScale = Vector3.one;
				obj.transform.localPosition = Vector3.zero;
				Card card = obj.GetComponent<Card> ();
				card.SetCardWithId (53);
				card.setSmall (isSmall);
				card.SetTouched (isTouched);
				if (i == 0) {
					w_card = card.W_Card;
					h_card = card.H_Card;
				}
				card.SetVisible (true);
				listCardHand.Add (card);
			}
		}
	}

	public Card AddAndGetCardOnArray ()
	{
		GameObject obj = Instantiate (GameControl.instance.objCard);
		obj.transform.SetParent (transform);
		obj.transform.localScale = Vector3.one;
		obj.transform.localPosition = Vector3.zero;
		Card card = obj.GetComponent<Card> ();
		card.SetCardWithId (53);
		card.setSmall (isSmall);
		card.SetTouched (isTouched);
		card.SetIsCardMauBinh ();

		card.SetVisible (false);
		listCardHand.Add (card);
		return card;
	}

	public void SetPositionCardInArray ()
	{
		float disCard = GetDistanceCard ();
		switch (align_Anchor) {
		case Align_Anchor.LEFT:
			Anchor_Left (disCard);
			break;
		case Align_Anchor.RIGHT:
			Anchor_Right (disCard);
			break;
		case Align_Anchor.CENTER:
			Anchor_Center (disCard);
			break;
		}
	}

	public void SetLaiIdCardSauKhiDanh ()
	{
		for (int i = 0; i < listCardHand.Count; i++) {
			Card card = listCardHand [i];
			if (card != null) {
				if (!card.isBatHayChua) {
					for (int k = i; k < listCardHand.Count; k++) {
						Card c = listCardHand [k];
						if (c == listCardHand [listCardHand.Count - 1]) {
							c.SetCardWithId (53);
							c.IsChoose = false;
							c.SetVisible (false);
						} else {
							if (listCardHand [k + 1].ID != 53) {
								c.SetCardWithId (listCardHand [k + 1].ID);
								c.SetVisible (true);
								listCardHand [k + 1].SetCardWithId (53);
								//listCardHand[k + 1].SetActiveBorder(false);
								listCardHand [k + 1].IsChoose = false;
								listCardHand [k + 1].SetVisible (false);
							} else {
								for (int l = k + 1; l < listCardHand.Count; l++) {
									if (listCardHand [l].ID != 53) {
										c.SetCardWithId (listCardHand [l].ID);
										c.SetVisible (true);
										listCardHand [l].SetCardWithId (53);
										//listCardHand[l].SetActiveBorder(false);
										listCardHand [l].IsChoose = false;
										listCardHand [l].SetVisible (false);
										break;
									}

								}
								if (c.ID == 53)
									c.SetVisible (false);
							}
						}
					}
				} else {
					if (card.ID == 53) {
						for (int k = i; k < listCardHand.Count; k++) {
							Card c = listCardHand [k];
							if (c == listCardHand [listCardHand.Count - 1]) {
								c.SetCardWithId (53);
								c.IsChoose = false;
								c.SetVisible (false);
							} else {
								if (listCardHand [k + 1].ID != 53) {
									c.SetCardWithId (listCardHand [k + 1].ID);
									c.SetVisible (true);
									listCardHand [k + 1].SetCardWithId (53);
									//listCardHand[k + 1].SetActiveBorder(false);
									listCardHand [k + 1].SetVisible (false);

								} else {
									for (int l = k + 1; l < listCardHand.Count; l++) {
										if (listCardHand [l].ID != 53) {
											c.SetCardWithId (listCardHand [l].ID);
											c.SetVisible (true);
											listCardHand [l].SetCardWithId (53);
											//listCardHand[l].SetActiveBorder(false);
											listCardHand [l].SetVisible (false);
											break;
										}
										c.SetVisible (false);
									}
								}
							}
						}
					}
				}
			}
		}

	}

	public void SetPositonCardHand ()
	{
		switch (align_Anchor) {
		case Align_Anchor.LEFT:
			Vector3 vtl = transform.localPosition;
			vtl.x = 110;//  = new Vector3(120, 0, 0);
			transform.localPosition = vtl;
			break;
		case Align_Anchor.RIGHT:
                //transform.localPosition = new Vector3(-120, 0, 0);

			Vector3 vtr = transform.localPosition;
			vtr.x = -110;
			transform.localPosition = vtr;
			break;
		case Align_Anchor.CENTER:
			float vty = transform.localPosition.y;
			Vector3 vt = transform.parent.InverseTransformPoint (CENTER_SCREEN);

			vt.y = vty;
			vt.z = 0;
//			vt.x = ;
			transform.localPosition = vt;
			break;
		}
	}


	public void SetPositonCardHandTaLa ()
	{
		float vty = transform.localPosition.y;
		Vector3 vt = vtPosCenter;
		vt.y = vty;
		vt.z = 0;
		vt.x += 100;
		//Debug.LogError(vt);
		transform.localPosition = vt;
	}

	//Camera GetCamera() {
	//    Camera ccc = null;
	//    foreach (Camera c in Camera.allCameras) {
	//        if (c.name.Equals("Camera")) {
	//            ccc = c;
	//            break;
	//        }
	//    }
	//    return ccc;
	//}

	#region ANCHOR

	void Anchor_Left (float disCard)
	{
		for (int i = 0; i < listCardHand.Count; i++) {
			listCardHand [i].transform.localPosition = new Vector3 (i * disCard, 0, 0);
			listCardHand [i].transform.SetSiblingIndex (i);
		}
	}

	void Anchor_Right (float disCard)
	{
		for (int i = listCardHand.Count - 1; i >= 0; i--) {
			listCardHand [i].transform.localPosition = new Vector3 (-(listCardHand.Count - 1 - i) * disCard, 0, 0);
			listCardHand [i].transform.SetSiblingIndex (i);
		}
	}

	void Anchor_Center (float disCard)
	{
		if (listCardHand.Count % 2 == 0) {
			for (int i = 0; i < listCardHand.Count; i++) {
				listCardHand [i].transform.localPosition = new Vector3 (
					-((int)listCardHand.Count / 2 - 0.5f)
					* disCard + i * disCard, 0, 0);
				listCardHand [i].transform.SetSiblingIndex (i);
			}
		} else {
			for (int i = 0; i < listCardHand.Count; i++) {
				listCardHand [i].transform.localPosition = new Vector3 (
					-((int)listCardHand.Count / 2) * disCard
					+ i * disCard, 0, 0);
				listCardHand [i].transform.SetSiblingIndex (i);
			}
		}

	}

	void Anchoir_Left_Card_Enable (bool isEffect = true, float dur = 0.1f)
	{
		float disCard = GetDistanceCard ();
		List<Card> list = new List<Card> ();
		for (int i = 0; i < listCardHand.Count; i++) {
			Card card = listCardHand [i];
			if (card.isBatHayChua) {
				list.Add (card);
				card.transform.SetSiblingIndex (i);
			}
		}
		for (int i = 0; i < list.Count; i++) {
			if (isEffect)
				StartCoroutine (list [i].MoveTo (new Vector3 (i * disCard, 0, 0), dur, i * 0.05f));
			else
				list [i].transform.localPosition = new Vector3 (i * disCard, 0, 0);
		}
	}

	void Anchoir_Right_Card_Enable (bool isEffect = true, float dur = 0.1f)
	{
		float disCard = GetDistanceCard ();
		List<Card> list = new List<Card> ();
		for (int i = 0; i < listCardHand.Count; i++) {
			Card card = listCardHand [i];
			if (card.isBatHayChua) {
				list.Add (card);
				card.transform.SetAsFirstSibling ();
			}
		}
		for (int i = 0; i < list.Count; i++) {
			if (isEffect)
				StartCoroutine (list [i].MoveTo (new Vector3 (-i * disCard, 0, 0), dur, i * 0.05f));
			else
				list [i].transform.localPosition = new Vector3 (-i * disCard, 0, 0);
		}
	}

	void Anchoir_Center_Card_Enable (bool isEffect = true, float dur = 0.1f)
	{
		float disCard = GetDistanceCard ();

		List<Card> list = new List<Card> ();
		for (int i = 0; i < listCardHand.Count; i++) {
			Card card = listCardHand [i];
			if (card.isBatHayChua) {
				list.Add (card);
				card.transform.SetSiblingIndex (i);
			}
		}
		if (list.Count % 2 == 0) {
			for (int i = 0; i < list.Count; i++) {
				Card card = list [i];
				Vector3 vt = new Vector3 (-((int)list.Count / 2 - 0.5f) * disCard + i * disCard, 0, 0);
				if (isEffect)
					StartCoroutine (card.MoveTo (vt, dur, i * 0.01f));
				else
					card.transform.localPosition = vt;

			}
		} else {
			for (int i = 0; i < list.Count; i++) {
				Card card = list [i];
				Vector3 vt = new Vector3 (-((int)list.Count / 2) * disCard + i * disCard, 0, 0);
				if (isEffect)
					StartCoroutine (card.MoveTo (vt, dur, i * 0.01f));
				else
					card.transform.localPosition = vt;

			}
		}


	}

	#endregion

	public int GetSizeOfListIdCardHand ()
	{
		return listIdCardHand.Count;
	}

	public int GetSizeOfListCardHand ()
	{
		return listCardHand.Count;
	}

	public void AddIdToListIdCardHand (int id)
	{
		listIdCardHand.Add (id);
	}

	public void SetListIDCard (int[] cards)
	{
		listIdCardHand.Clear ();
		for (int i = 0; i < cards.Length; i++) {
			AddIdToListIdCardHand (cards [i]);
		}
	}

	public void SetListCardHand (Card card)
	{
		if (!listCardHand.Contains (card))
			listCardHand.Add (card);
	}

	public void AddIdToList (int id)
	{
		listIdCardHand.Add (id);
	}

	public void RemoveIdFromList (int id)
	{
		listIdCardHand.Remove (id);
	}

	//public void ChiaBaiPoker(int[] arrcard, bool isTao, UnityAction callBack = null) {
	//    Vector3 vt = transform.InverseTransformPoint(vtPosCenter);
	//    if (isTao)
	//        SetPositionCardInArray();
	//    else
	//        SetLaiHetCardVeToaDo0();
	//    SetListIDCard(arrcard);
	//    for (int i = 0; i < listCardHand.Count; i++) {
	//        //Card c = listCardHand[i];
	//        listCardHand[i].SetVisible(false);
	//        listCardHand[i].IsChoose = false;
	//        if (i < arrcard.Length) {
	//            if (isTao) {
	//                listCardHand[i].SetActiveBorder(false);
	//                listCardHand[i].LatBaiLen(arrcard[i]);
	//            } else {
	//                listCardHand[i].SetCardWithId(53);
	//                listCardHand[i].setSmall(true);
	//                listCardHand[i].SetVisible(false);
	//                listCardHand[i].SetActiveBorder(false);
	//            }
	//            StartCoroutine(listCardHand[i].MoveFrom(vt, 1f, i * 0.1f));
	//        } else
	//            listCardHand[i].SetVisible(false);
	//    }
	//    if (callBack != null) {
	//        callBack();
	//        callBack = null;
	//    }
	//    StartCoroutine(ShowCardKhiChiaXong());
	//}

	public void ChiaBaiTienLen (int[] arrcard, bool isTao, UnityAction callBack = null)
	{
		Vector3 vt = transform.InverseTransformPoint (CENTER_SCREEN);
		if (isTao)
			SetPositionCardInArray ();
		else {
			SetLaiHetCardVeToaDo0 ();
		}
		SetListIDCard (arrcard);
		for (int i = 0; i < listCardHand.Count; i++) {
			listCardHand [i].IsChoose = false;
			if (i < arrcard.Length) {
				if (isTao) {
					listCardHand [i].name = "" + i;
					listCardHand [i].SetCardWithId (arrcard [i]);
				} else {
					listCardHand [i].SetCardWithId (53);
					listCardHand [i].setSmall (true);
				}
				if (i < arrcard.Length - 1)
					StartCoroutine (listCardHand [i].MoveFrom (vt, 0.5f, i * 0.1f));
				else {
					StartCoroutine (listCardHand [i].MoveFrom (vt, 0.5f, i * 0.1f, () => {
						if (callBack != null) {
							callBack.Invoke ();
							callBack = null;
						}
					}));
				}
			} else
				listCardHand [i].SetVisible (false);
		}
	}

	IEnumerator ShowCardKhiChiaXong ()
	{
		yield return new WaitForSeconds (listCardHand.Count * 0.1f + 1);
		for (int i = 0; i < listCardHand.Count; i++) {
			Card card = listCardHand [i];
			if (card.ID != 53) {
				card.SetVisible (true);
			}
		}
	}

	public void SetBaiKhiKetNoiLai (int[] arrcard, bool isTao)
	{
		if (isTao)
			SetPositionCardInArray ();
		else
			SetLaiHetCardVeToaDo0 ();
		SetListIDCard (arrcard);
		for (int i = 0; i < listCardHand.Count; i++) {
			//Card c = listCardHand[i];
			listCardHand [i].SetVisible (false);
			listCardHand [i].IsChoose = false;
			if (i < arrcard.Length) {
				listCardHand [i].SetVisible (true);
				if (isTao) {
					listCardHand [i].SetCardWithId (arrcard [i]);
				} else {
					listCardHand [i].SetCardWithId (53);
					listCardHand [i].setSmall (true);
					//listCardHand[i].transform.localPosition = Vector3.zero;
				}
			}
		}
		if (isTao)
			SortCardActive ();
	}

	public Card GetCardbyIDCard (int id)
	{
		try {
			for (int i = 0; i < GetSizeOfListCardHand (); i++) {
				if (listCardHand [i].ID == id) {
					return listCardHand [i];
				}
			}
		} catch (Exception e) {
			Debug.LogException (e);
		}

		return null;
	}

	public Card GetCardbyIndex (int index)
	{
		if (index > listCardHand.Count - 1) {
			index = listCardHand.Count - 1;
		}
		if (index < 0)
			index = 0;
		return listCardHand [index];
	}

	public void ClearCardHand ()
	{
		listIdCardHand.Clear ();
		for (int i = 0; i < listCardHand.Count; i++) {
			Destroy (listCardHand [i].gameObject);
		}
		listCardHand.Clear ();
	}

	public void RemoveCardById (int id)
	{
		listIdCardHand.Remove (id);
		listCardHand.Remove (GetCardbyIDCard (id));
	}

	public void SetCardKhiKetThucGame (int[] arrCards, int sitNumer = -1)
	{
		float disCard = GetDistanceCard ();

		if (align_Anchor == Align_Anchor.RIGHT) {
			int j = arrCards.Length - 1;
			for (int i = listCardHand.Count - 1; i >= 0; i--) {
				Card card = listCardHand [i];
				card.SetVisible (false);
				if (j >= 0) {
					card.SetVisible (true);
					card.SetDarkCard (false);
					card.SetCardWithId (arrCards [j]);
					card.setSmall (true);
					card.transform.localPosition = Vector3.zero;
					card.transform.DOLocalMoveX (-(CardCount - 1 - i) * disCard, .1f);
					j--;
					card.transform.SetSiblingIndex (i);
				}
			}
		} else if (align_Anchor == Align_Anchor.LEFT) {
			for (int i = 0; i < listCardHand.Count; i++) {
				Card card = listCardHand [i];
				card.SetVisible (false);
				if (i < arrCards.Length) {
					card.SetVisible (true);
					card.SetDarkCard (false);
					card.SetCardWithId (arrCards [i]);
					card.setSmall (true);
					card.transform.localPosition = Vector3.zero;
					card.transform.DOLocalMoveX (i * disCard, .1f);
					card.transform.SetSiblingIndex (i);
				}
			}
		} else if (align_Anchor == Align_Anchor.CENTER) {
			for (int i = 0; i < listCardHand.Count; i++) {
				Card card = listCardHand [i];
				card.SetVisible (false);
				if (i < arrCards.Length) {
					card.SetVisible (true);
					card.SetCardWithId (arrCards [i]);
				}
			}
			Anchoir_Center_Card_Enable ();
		}
	}

	public void SetCardWithArrID (int[] arrCards, bool isSort = true)
	{
		for (int i = 0; i < listCardHand.Count; i++) {
			Card card = listCardHand [i];
			if (i < arrCards.Length) {
				card.SetCardWithId (arrCards [i]);
			}
		}
		if (isSort) {
			SetPositionCardInArray ();
		}
	}

	public void SetActiveCardWithArrID (int[] arrCards, bool isSort = true)
	{
		for (int i = 0; i < listCardHand.Count; i++) {
			Card card = listCardHand [i];
			if (i < arrCards.Length) {
				card.SetVisible (true);
				card.SetCardWithId (arrCards [i]);
			} else {
				card.SetVisible (false);
			}
		}
		if (isSort) {
			SetPositionCardInArray ();
		}
	}

	public void SetChooseCard (int[] cards)
	{
		if (cards != null) {
			for (int i = 0; i < listCardHand.Count; i++) {
				Card c = listCardHand [i];
				bool isNotRight = true;
				if (c.isBatHayChua) {
					for (int j = 0; j < cards.Length; j++) {
						if (c.ID == cards [j]) {
							c.SetDarkCard (false);
							c.SetTouched (true);
							isNotRight = false;
							break;
						}
					}
					if (isNotRight) {
						c.IsChoose = false;
						c.SetDarkCard (true);
						c.SetTouched (false);
					}
				}
			}
		}
	}

	public void ResetCard (bool isBorder = false)
	{
		for (int i = 0; i < listCardHand.Count; i++) {
			listCardHand [i].SetDarkCard (false);
			listCardHand [i].SetTouched (true);
			listCardHand [i].IsChoose = false;
			if (isBorder) {
				//listCardHand[i].isCardAnnnnn = false;
				//listCardHand[i].SetActiveBorder(false);
			}
		}
	}

	public void SetCardWithId53 ()
	{
		for (int i = 0; i < listCardHand.Count; i++) {
			listCardHand [i].SetCardWithId (53);
		}
	}

	public void SetLaiHetCardVeToaDo0 ()
	{
		for (int i = 0; i < listCardHand.Count; i++) {
			listCardHand [i].transform.localPosition = Vector3.zero;
		}
	}

	public void SortCardActive (bool isEffect = true, float dur = 0.2f)
	{
		switch (align_Anchor) {
		case Align_Anchor.LEFT:
			Anchoir_Left_Card_Enable (isEffect, dur);
			break;
		case Align_Anchor.RIGHT:
			Anchoir_Right_Card_Enable (isEffect, dur);
			break;
		case Align_Anchor.CENTER:
			Anchoir_Center_Card_Enable (isEffect, dur);
			break;
		}
	}

	/// <summary>
	/// Tra ve gia tri tao do cua la bai vua them vao
	/// </summary>
	public Vector3 GetPositonCardActive ()
	{
		Vector3 vt = Vector3.zero;
		float disCard = GetDistanceCard ();

		int count = listCardHand.Count (x => x.isBatHayChua);
		count++;
		switch (align_Anchor) {
		case Align_Anchor.LEFT:
			vt = new Vector3 (count * disCard, 0, 0);
			break;
		case Align_Anchor.RIGHT:
			vt = new Vector3 (-count * disCard, 0, 0);
			break;
		case Align_Anchor.CENTER:
			if (count % 2 == 0) {
				vt = new Vector3 (-((int)count / 2 - 0.5f) * disCard + count * disCard, 0, 0);
			} else {
				vt = new Vector3 (-((int)count / 2) * disCard + count * disCard, 0, 0);
			}
			break;
		}
		return vt;
	}

	public void SetActiveCardHand (bool isActive = false)
	{
		for (int i = 0; i < listCardHand.Count; i++) {
			listCardHand [i].SetVisible (isActive);
		}
	}

	public void SetVisible (bool isVisible)
	{
		gameObject.SetActive (isVisible);
	}

	public void SetTouchCardHand (bool isTouched = false)
	{
		this.isTouched = isTouched;
		for (int i = 0; i < listCardHand.Count; i++) {
			listCardHand [i].SetTouched (isTouched);
		}
	}

	bool isSetInputChooseCard = false;
	public void SetInputChooseCard ()
	{
		if (!isSetInputChooseCard) {
			isSetInputChooseCard = true;
			switch (GameConfig.CurrentGameID) {
			case GameID.TLMN:
			case GameID.TLMNSL:
				for (int i = 0; i < listCardHand.Count; i++) {
					Card c = listCardHand [i];
					c.setListenerClick (delegate {
						AutoChooseCard.ClickCard (c, listCardHand.ToArray ());
					});
					c.isAuto = true;
				}
				break;
			case GameID.SAM:
				for (int i = 0; i < listCardHand.Count; i++) {
					Card c = listCardHand [i];
					c.setListenerClick (delegate {
						AutoChooseCardSam.ClickCard (c, listCardHand.ToArray ());
					});
					c.isAuto = true;
				}
				break;
			}
		}
	}

	public void SetAutoChooseCard (bool isAuto)
	{
		for (int i = 0; i < listCardHand.Count; i++) {
			Card c = listCardHand [i];
			c.isAuto = isAuto;
		}
	}

	public void SetIsCardDragDrop (bool isMauBinh)
	{
		for (int i = 0; i < listCardHand.Count; i++) {
			listCardHand [i].SetIsCardMauBinh (isMauBinh);
		}
	}

	public int[] GetCardChoose ()
	{
		List<int> list = new List<int> ();
		for (int i = 0; i < listCardHand.Count; i++) {
			if (listCardHand [i].isBatHayChua && listCardHand [i].IsChoose && listCardHand [i].ID != 52) {
				list.Add (listCardHand [i].ID);
			}
		}
		return list.ToArray ();
	}

	float GetDistanceCard ()
	{
		if (MaxWidth >= CardCount * w_card) {
			return w_card;
		} else {
			return (MaxWidth / CardCount);
		}
	}
	public void ResetForTala(bool isTouched = false){
		for (int i = 0; i < listCardHand.Count; i++) {
			Card c = listCardHand [i];
			c.SetCardWithId(53);
			c.SetDarkCard (false);
			c.SetTouched (isTouched);
			c.IsChoose = false;
			c.SetActiveBorder (false);
			c.SetIsCardMauBinh (isTouched);
			c.SetVisible (false);
		}
	}
}
