﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Collections.Generic;
using DG.Tweening;
using Us.Mobile.Utilites;
using AppConfig;
using Beebyte.Obfuscator;
using DataBase;

public class PokerControl : BaseCasino {
	public static PokerControl instance;
	[SerializeField]
	UIButton btn_bo, btn_xembai, btn_theo, btn_to;
	[SerializeField]
	UIButton btn_ruttien;
	[SerializeField]
	Text txt_bo, txt_xem, txt_theo, txt_to;
	long MoneyCuoc = 0;
	long MinToMoney = 0, MaxToMoney = 0;
	long tongMoney = 0;
	//LiengPlayer plMe;

	[SerializeField]
	ChipControl SumChipControl;
	[SerializeField]
	GroupTo groupMoneyTo;
	[SerializeField]
	TimeCountDown timeCountDown;
	[SerializeField]
	CardTablePoker cardTableManager;

	[SerializeField]
	Transform dealerPos;
	void Awake() {
		instance = this;
		SumChipControl.IsChipSum = true;
	}

	void SetActiveButton(bool is_bo = true, bool is_xem = true, bool is_theo = true, bool is_to = true) {
		btn_bo.gameObject.SetActive(is_bo);
		btn_xembai.gameObject.SetActive(is_xem);
		btn_theo.gameObject.SetActive(is_theo);
		btn_to.gameObject.SetActive(is_to);
	}

	void SetEnableButton(bool is_bo = true, bool is_xem = true, bool is_theo = true, bool is_to = true) {
		btn_bo.enabled = is_bo;
		btn_xembai.enabled = is_xem;
		btn_theo.enabled = is_theo;
		btn_to.enabled = is_to;

		txt_bo.color = is_bo ? Color.white : Color.gray;
		txt_xem.color = is_xem ? Color.white : Color.gray;
		txt_theo.color = is_theo ? Color.white : Color.gray;
		txt_to.color = is_to ? Color.white : Color.gray;
	}

	internal override void StartTableOk(int[] cardHand, Message msg, string[] nickPlay) {
		base.StartTableOk(cardHand, msg, nickPlay);
		for (int i = 0; i < ListPlayer.Count; i++) {
			((LiengPlayer)ListPlayer[i]).MoneyChip = 0;
		}
		groupMoneyTo.OnHide();
		MinToMoney = GameConfig.BetMoney * 2;
		MaxToMoney = ClientConfig.UserInfo.CASH_FREE - MinToMoney;

		SumChipControl.MoneyChip = 0;
		tongMoney = 0;
		cardTableManager.XoaHetBaiTrenBai();
		list_my_card.Clear();
		list_my_card.AddRange(cardHand);
		for (int i = 0; i < ListPlayer.Count; i++) {
			LiengPlayer player = (LiengPlayer)ListPlayer[i];
			if (player != null) {
				if (player.SitOnClient == 0) {
					player.CardHand.SetAllDark(false);
				}
				player.CardHand.ChiaBaiPoker(cardHand, dealerPos, i,  player.SitOnClient == 0);
			}
		}
	}
	internal override void OnStartForView(string[] nickPlay, Message msg) {
		base.OnStartForView(nickPlay, msg);
		tongMoney = 0;
		for (int i = 0; i < nickPlay.Length; i++) {
			LiengPlayer player = (LiengPlayer)GetPlayerWithName(nickPlay[i]);
			if (player != null) {
				if (player.SitOnClient != 0)
					player.CardHand.ChiaBaiPoker(new int[] { 52, 52 }, dealerPos, i, false);
			}
		}

		SetActiveButton(false, false, false, false);
	}
	internal override void OnJoinView(Message message) {
		// TODO Auto-generated method stub
		base.OnJoinView(message);
		//plMe = (LiengPlayer)playerMe;

		cardTableManager.Init();
		SetActiveButton(false, false, false, false);
	}
	internal override void OnJoinTableSuccess(string master) {
		//plMe = (LiengPlayer)playerMe;
		cardTableManager.Init();
		for (int i = 0; i < ListPlayer.Count; i++) {
			((LiengPlayer)ListPlayer[i]).SetDiemLieng(false, null);
		}

		SetActiveButton(false, false, false, false);
	}

	internal override void SetTurn(string nick, Message message) {
		base.SetTurn(nick, message);
		try {
			MoneyCuoc = message.reader().ReadLong();
			//Debug.LogError(nick + " -=-=-=- " + MoneyCuoc);
			if (nick.Equals(ClientConfig.UserInfo.UNAME)) {
				baseSetTurn();
			} else {
				hideThanhTo();
				SetActiveButton(false, false, false, false);
			}
		} catch (Exception e) {
			// TODO: handle exception
			Debug.LogException(e);
		}
	}

	private void hideThanhTo() {
		groupMoneyTo.OnHide();
	}

	internal override void OnFinishGame(Message message) {
		try {
			IsPlaying = false;
			isStart = false;

			int total = message.reader().ReadByte();
			for (int i = 0; i < total; i++) {
				string nick = message.reader().ReadUTF();
				int rank = message.reader().ReadByte();
				long mn = message.reader().ReadLong();
				LiengPlayer pl = (LiengPlayer)GetPlayerWithName(nick);
				if (pl != null) {
					pl.SetRank(rank);
					pl.IsReady = false;
					pl.SetShowReady(false);
					pl.MoneyChip = 0;
					//pl.SetDiemLieng(true, pl.CardHand.GetArrayIDCard());
				}
			}
			SetActiveButton(false, false, false, false);
			OnJoinTableSuccess(masterID);
			for (int j = 0; j < ListPlayer.Count; j++) {
				ListPlayer[j].SetShowReady(false);
				ListPlayer[j].SetTurn(0, false);
			}

			tongMoney = 0;
			SumChipControl.MoneyChip = tongMoney;
		} catch (Exception ex) {
			Debug.LogException(ex);
		}
	}

	internal override void OnInfoWinPlayer(InfoWin infoWin) {
		base.OnInfoWinPlayer(infoWin);
		//Debug.LogError("Vao day roi ma!  " + infoWin.name);
		LiengPlayer player = (LiengPlayer)GetPlayerWithName(infoWin.name);
		if (player != null) {
			player.CardHand.SetAllDark(true);

			int k = infoWin.arrCard.Length - 5;
			//Debug.LogError("kkkkkkkkkkkk     "+ k);
			if (k <= 0) {
				player.CardHand.SetAllDark(false);
				return;
			}
			if (infoWin.typeCard >= 0 && infoWin.typeCard <= 8) {
			}

			cardTableManager.showCardFinish(infoWin.arrCard, k);

			for (int i = k; i < infoWin.arrCard.Length; i++) {
				Card c = player.CardHand.GetCardbyIDCard(infoWin.arrCard[i]);
				if (c != null) {
					c.SetDarkCard(false);
					c.IsChoose = true;
				}
			}
		}
	}

	internal override void OnNickSkip(string nick, Message msg) {
		try {

			string nick_turn = msg.reader().ReadUTF();
			LiengPlayer pl = (LiengPlayer)GetPlayerWithName(nick);
			if (pl != null) {
				pl.SetEffect("Bỏ");
				pl.SetTurn(0, false);
				pl.SetRank(4);
				pl.CardHand.SetAllDark(true);
				pl.IsPlaying = false;
			}

			pl.MoveChip(pl.MoneyChip, SumChipControl.transform.position);
			pl.MoneyChip = 0;

			SetTurn(nick_turn, msg);
			if (nick.Equals(ClientConfig.UserInfo.UNAME)) {
				SetActiveButton();
			}else if (nick_turn.Equals(ClientConfig.UserInfo.UNAME)) {
                SetActiveButton();
				baseSetTurn();
			} else {
				hideThanhTo();
				SetActiveButton(false, false, false, false);
			}
		} catch (Exception e) {
			Debug.LogException(e);
		}
	}

	internal override void OnNickCuoc(Message message) {
		try {
			long moneyInPot = message.reader().ReadLong();
			MoneyCuoc = message.reader().ReadLong();
			long moneyBoRa = message.reader().ReadLong();
			//Debug.LogError("So tien to: " + MoneyCuoc);
			//Debug.LogError("So tien bo ra: " + moneyBoRa);
			string nick = message.reader().ReadUTF();
			string nick_turn = message.reader().ReadUTF();
			if (GameConfig.BetMoney * 2 >= MoneyCuoc) {
				MinToMoney = GameConfig.BetMoney * 2;
			} else {
				MinToMoney = MoneyCuoc;
			}
			LiengPlayer pl = (LiengPlayer)GetPlayerWithName(nick);
			if (pl != null) {
				pl.SetEffect("Tố");
				pl.MoveChip(moneyBoRa + pl.MoneyChip, SumChipControl.transform.position);
				pl.MoneyChip += moneyBoRa;
				//gameControl.sound.startToAudio();
				tongMoney += MoneyCuoc;
				SumChipControl.MoneyChip = tongMoney;
			}

			SetTurn(nick_turn, message);
			if (nick.Equals(ClientConfig.UserInfo.UNAME)) {
				hideThanhTo();
				SetActiveButton(false, false, false, false);
			}else if (nick_turn.Equals(ClientConfig.UserInfo.UNAME)) {
                SetActiveButton();
				baseSetTurn();
			} else {
				hideThanhTo();
				SetActiveButton(false, false, false, false);
			}
		} catch (Exception e) {
			Debug.LogException(e);
		}
	}

	internal override void OnNickTheo(Message message) {
		try {
			long money = message.reader().ReadLong();
			string nick = message.reader().ReadUTF();
			string nick_turn = message.reader().ReadUTF();

			LiengPlayer pl = (LiengPlayer)GetPlayerWithName(nick);
			if (money == 0) {
				pl.SetEffect("Xem");
			} else {
				pl.SetEffect("Theo");

				pl.MoveChip(money + pl.MoneyChip, SumChipControl.transform.position);
				pl.MoneyChip += money;
				tongMoney += money;
				SumChipControl.MoneyChip = tongMoney;
			}
			SetTurn(nick_turn, message);
			if (nick.Equals(ClientConfig.UserInfo.UNAME)) {
				SetActiveButton(false, false, false, false);
			}else if (nick_turn.Equals(ClientConfig.UserInfo.UNAME)) {
                SetActiveButton();
				baseSetTurn();
			} else {
				hideThanhTo();
				SetActiveButton(false, false, false, false);
			}
		} catch (Exception e) {
			// TODO: handle exception
			Debug.LogException(e);
		}
	}

	void baseSetTurn() {
		SetActiveButton();
		if (MoneyCuoc <= 0) {
			SetEnableButton(true, true, false, true);
		} else if (MoneyCuoc < ((LiengPlayer)playerMe).MoneyFollow) {
			SetEnableButton(true, false, true, false);
			txt_theo.text = "Theo " + MoneyHelper.FormatMoneyNormal(MoneyCuoc);
		} else {
			SetEnableButton(true, false, true, false);
			txt_theo.text = "Theo " + MoneyHelper.FormatMoneyNormal(((LiengPlayer)playerMe).MoneyFollow);
		}
	}
	#region Click
	[SkipRename]
	public void clickButtonBo() {
		SendData.onSendSkipTurn();
	}
	[SkipRename]
	public void clickButtonXem() {
		SendData.onAccepFollow();
	}
	[SkipRename]
	public void clickButtonTheo() {
		SendData.onAccepFollow();
	}
	[SkipRename]
	public void clickButtonTo() {
		groupMoneyTo.OnShow(MinToMoney, MaxToMoney);
	}
	[SkipRename]
	public void clickButtnRutTien() {
		if (ClientConfig.UserInfo.CASH_FREE < GameConfig.BetMoney * 10) {
			PopupAndLoadingScript.instance.messageSytem.OnShow("Không đủ tiền để rút, bạn có muốn nạp thêm?");
		} else {
			//Show rut tien
			Debug.LogError("Show rut tien");
			LoadAssetBundle.LoadScene(SceneName.SUB_RUT_TIEN, SceneName.SUB_RUT_TIEN, () => {
				
			});
		}
	}
	#endregion
	public void OnTo(long moneyTo) {
		SendData.onCuocXT(-99, moneyTo);
	}

	internal override void OnTimeAuToStart(int time) {
		base.OnTimeAuToStart(time);
		timeCountDown.SetTime(time);
	}


	internal override void InfoCardPlayerInTbl(Message message, string turnName, int time, sbyte numP) {
		base.InfoCardPlayerInTbl(message, turnName, time, numP);
		try {
			for (int i = 0; i < numP; i++) {
				string nick = message.reader().ReadUTF();
				sbyte isSkip = message.reader().ReadByte(); // = 0 Skip.
				long chips = message.reader().ReadLong();
				LiengPlayer pl = (LiengPlayer)GetPlayerWithName(nick);
				if (pl != null) {
					pl.MoneyChip = chips;
					pl.IsPlaying = true;
					pl.CardHand.SetBaiKhiKetNoiLaiGamePoker(new int[] { 52, 52 }, true);
					if (isSkip == 0) {
						pl.CardHand.SetAllDark(true);
					}
					//tongMoney += chips;
				}
			}

			int size = message.reader().ReadInt();
			int[] cards = new int[size];
			for (int i = 0; i < size; i++) {
				cards[i] = message.reader().ReadByte();
			}
			cardTableManager.SinhCardKhiKetNoiLai(cards);
			sbyte len1 = message.reader().ReadByte();
			for (int i = 0; i < len1; i++) {
				long money = message.reader().ReadLong();
				sbyte len2 = message.reader().ReadByte();
				for (int j = 0; j < len2; j++) {
					string nameNN = message.reader().ReadUTF();
					//moneyInPot[i].addChip2(money / len2, name, false);
				}
				//moneyInPot[i].setmMoneyInPotNonModifier(money);
			}

			//SumChipControl.MoneyChip = tongMoney;
			//gameControl.sound.startchiabaiAudio();
			SetTurn(turnName, message);
			if (turnName.Equals(ClientConfig.UserInfo.UNAME)) {
				SetActiveButton();
				SetEnableButton();
			}
		} catch (Exception e) {
			// TODO: handle exception
			Debug.LogException(e);
		}
	}

	internal override void OnInfome(Message message) {
		base.OnInfome(message);
		try {
			isStart = true;
			((LiengPlayer)playerMe).IsPlaying = true;
			int sizeCardHand = message.reader().ReadByte();
			int[] cardHand = new int[sizeCardHand];
			for (int j = 0; j < sizeCardHand; j++) {
				cardHand[j] = message.reader().ReadByte();
			}
			((LiengPlayer)playerMe).CardHand.SetBaiKhiKetNoiLaiGamePoker(cardHand, true);

			bool upBai = message.reader().ReadBoolean();
			if (upBai) {
				((LiengPlayer)playerMe).CardHand.SetAllDark(true);
			}
			string turnvName = message.reader().ReadUTF();
			int turnvTime = message.reader().ReadInt();
			long money = message.reader().ReadLong();
			long moneyC = message.reader().ReadLong();
			long mIP = message.reader().ReadLong();
			((LiengPlayer)playerMe).MoneyChip = moneyC;
			tongMoney += moneyC;
			SetTurn(turnvName, message);
			if (turnvName.Equals(ClientConfig.UserInfo.UNAME)) {
				SetActiveButton();
				SetEnableButton();
			}
			SumChipControl.MoneyChip = tongMoney;
		} catch (Exception e) {
			Debug.LogException(e);
		}
	}
	List<int> list_my_card = new List<int>();
	internal void OnInfo(Message msg) {
		try {
			string nicksb = msg.reader().ReadUTF();
			string nickbb = msg.reader().ReadUTF();
			long moneyInPot = msg.reader().ReadLong();
			if (((LiengPlayer)playerMe).IsPlaying) {
				//players[0].setLoaiBai(PokerCard.getTypeOfCardsPoker(PokerCard.add2ArrayCard(
				//		players[0].cardHand.getArrCardAll(), cardTable.getArrCardAll())));
				int type = PokerCard.getTypeOfCardsPoker(PokerCard.add2ArrayCard(list_my_card.ToArray(), cardTableManager.list_card.ToArray()));
				((LiengPlayer)playerMe).SetTypeCard(type);
			}
			Debug.LogError("nicksb " + nicksb
						  + "\nnickbb " + nickbb
						  + "\nmoneyInPot " + moneyInPot);
			LiengPlayer plbb = (LiengPlayer)GetPlayerWithName(nickbb);
			LiengPlayer plsb = (LiengPlayer)GetPlayerWithName(nicksb);
			if (plbb != null) {
				plbb.MoneyChip = moneyInPot * 2 / 3;
				plbb.MoveChip(moneyInPot * 2 / 3, SumChipControl.transform.position);
			}
			if (plsb != null) {
				plsb.MoneyChip = moneyInPot / 3;
				plsb.MoveChip(moneyInPot / 3, SumChipControl.transform.position);
			}

			tongMoney = 0;
			for (int i = 0; i < ListPlayer.Count; i++) {
				if (ListPlayer[i].IsPlaying)
					tongMoney += ((LiengPlayer)ListPlayer[i]).MoneyChip;
			}
			//for (int i = 0; i < nUsers; i++) {
			//	if (players[i].isPlaying()) {
			//		tongMoney = tongMoney + players[i].getMoneyChip();

			//	}
			//}
			//chip_tong.setMoneyChip(tongMoney);
			//chip_tong.setVisible(true);

			SumChipControl.MoneyChip = tongMoney;
		} catch (Exception ex) {
			Debug.LogException(ex);
		}
	}

	public void OnAddCardTbl(Message message) {
		try {
			// byte type = message.reader().readByte();
			int size = message.reader().ReadInt();
			int[] cards = new int[size];
			for (int i = 0; i < size; i++) {
				cards[i] = message.reader().ReadByte();
			}

			cardTableManager.AddCard(cards);

			if (((LiengPlayer)playerMe).IsPlaying) {
				int type = PokerCard.getTypeOfCardsPoker(PokerCard.add2ArrayCard(list_my_card.ToArray(), cardTableManager.list_card.ToArray()));
				((LiengPlayer)playerMe).SetTypeCard(type);
			}
		} catch (Exception ex) {
			Debug.LogException(ex);
		}
	}
}
