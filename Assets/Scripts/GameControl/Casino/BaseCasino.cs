﻿using AppConfig;
using DataBase;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Us.Mobile.Utilites;
using DG.Tweening;
using System.Xml.Linq;
using System.Linq;
using Beebyte.Obfuscator;

public abstract class BaseCasino : MonoBehaviour {
	protected List<BasePlayer> ListPlayer = new List<BasePlayer>();
	internal BasePlayer playerMe;
	[SerializeField]
	Transform[] tf_invite;
	[SerializeField]
	GameObject player_prefab;
	[SerializeField]
	Transform tf_parent_player;
	[SerializeField]
	Text txt_id_table, txt_bet_table, txt_game_name;

	protected bool IsPlaying = false;
	protected string MasterName;

	protected bool isStart = false;
	protected bool isView = false;
	protected string nickFire = "";
	protected string masterID = "";

	internal void Start() {
		for (int i = 1; i < tf_invite.Length; i++) {
			tf_invite[i].GetComponent<UIButton>()._onClick.AddListener(OnClickInvite);
		}
		UnloadAllSubScene();
	}

	public void UnloadAllSubScene() {
		StartCoroutine(WaitUnloadAllSubScene());
	}

	IEnumerator WaitUnloadAllSubScene() {
		yield return new WaitForEndOfFrame();
		GameControl.instance.UnloadScene(SceneName.SCENE_ROOM);
		GameControl.instance.UnloadScene(SceneName.SCENE_LOBBY);
		//		GameControl.instance.UnloadScene (SceneName.SCENE_MAIN);
		GameControl.instance.UnloadSubScene();
		PopupAndLoadingScript.instance.OnHideAll();
	}

	void OnClickInvite() {
		PopupAndLoadingScript.instance.ShowLoading();
		SendData.onGetListInvite();
	}

	#region Xu li trong game

	internal void OnChat(string nick, string msg) {
		BasePlayer pl = GetPlayerWithName(nick);
		if (pl != null) {
			pl.SetChat(msg);
		}
	}

	public Transform GetParentPlayer() {
		return tf_parent_player;
	}

	internal virtual void OnJoinTablePlaySuccess(Message message) {
		try {
			short idTable = message.reader().ReadShort();
			GameConfig.BetMoney = message.reader().ReadLong();
			long needMoney = message.reader().ReadLong();
			long maxMoney = message.reader().ReadLong();

			txt_id_table.text = "Bàn " + idTable;
			txt_bet_table.text = "Mức cược " + "<color=yellow>" + GameConfig.BetMoney + "</color>";
			txt_game_name.text = GameConfig.GameName[GameConfig.CurrentGameID];

			int luatPhom = message.reader().ReadByte();
			SetLuatChoi(luatPhom);
			string master = message.reader().ReadUTF();
			int len = message.reader().ReadByte();
			GameControl.instance.TimerTurnInGame = message.reader().ReadInt();
			IsPlaying = message.reader().ReadBoolean();
			for (int i = 0; i < len; i++) {
				PlayerData pl = new PlayerData();
				pl.NamePlayer = message.reader().ReadUTF();
				pl.DisplaName = message.reader().ReadUTF();
				pl.Avata_Link = message.reader().ReadUTF();
				pl.Avata_Id = message.reader().ReadInt();
				pl.SitOnSever = message.reader().ReadByte();
				pl.Money = message.reader().ReadLong();
				pl.IsReady = message.reader().ReadBoolean();
				pl.FolowMoney = message.reader().ReadLong();
				pl.IsMaster = pl.NamePlayer.Equals(master);
				if (IsPlaying) {
					pl.IsReady = false;
				}
				GameObject objPlayer = Instantiate(player_prefab);
				objPlayer.transform.SetParent(tf_parent_player);
				objPlayer.transform.localScale = Vector3.one;
				BasePlayer plUI = objPlayer.GetComponent<BasePlayer>();
				plUI.SetInfo(pl);
				if (pl.NamePlayer.Equals(ClientConfig.UserInfo.UNAME)) {
					playerMe = plUI;
				}
				objPlayer.SetActive(false);
				var match = ListPlayer.FirstOrDefault(item => item.NamePlayer == pl.NamePlayer);
				//ListPlayer.Any (item=>item.NamePlayer == pl.Name

				if (match == null) {
					ListPlayer.Add(plUI);
				} else {
					Destroy(plUI);
				}
			}

			OnJoinTableSuccess(master);
			SortSitPlayer();
		} catch (Exception e) {
			Debug.LogException(e);
		}
	}

	internal virtual void OnFireCardFail() {
	}

	internal virtual void OnFinishTurn() {
	}

	internal virtual void OnInfome(Message message) {
	}

	internal virtual void OnFinishGame(Message message) {
		try{
		IsPlaying = false;
		int total = message.reader().ReadByte();
		for (int i = 0; i < total; i++) {
			string nick = message.reader().ReadUTF();
			int rank = message.reader().ReadByte();
			if (rank != 1 && rank != 5) {
			}
			long money = message.reader().ReadLong();
			string dau = "";
			if (rank == 1 || rank == 5) {
				dau = "+";
			}
			BasePlayer pl = GetPlayerWithName(nick);
			if (pl != null) {
				pl.SetEffect(dau + MoneyHelper.FormatMoneyNormal(money));
				pl.SetRank(rank);
				pl.IsReady = false;
			}
		}
			} catch (Exception e) {
			Debug.LogException(e);
		}
	}

	//protected String[] luatchoi = new String[] { "TÁI GỬI", "KHÔNG TÁI GỬI" };
	internal virtual void SetLuatChoi(int rule) {

		//if (screen.game.gameID == GameID.PHOM)
		//    luatChoi.setText(luatchoi[readByte]);
	}

	internal virtual void OnJoinTableSuccess(string master) {

	}

	internal virtual void OnJoinTableSuccess(Message message) {
		try {
			sbyte rule = message.reader().ReadByte();
			SetLuatChoi(rule);
			string master = message.reader().ReadUTF();
			int len = message.reader().ReadByte();
			int timeTurn = message.reader().ReadInt();
			IsPlaying = message.reader().ReadBoolean();

			Debug.LogError("OnJoinTableSuccess So thang choi:  " + len);
			for (int i = 0; i < len; i++) {
				PlayerData pl = new PlayerData();
				pl.NamePlayer = message.reader().ReadUTF();
				pl.DisplaName = message.reader().ReadUTF();
				pl.Avata_Link = message.reader().ReadUTF();
				pl.Avata_Id = message.reader().ReadInt();
				pl.SitOnSever = message.reader().ReadByte();
				pl.Money = message.reader().ReadLong();
				pl.IsReady = message.reader().ReadBoolean();
				pl.FolowMoney = message.reader().ReadLong();
				pl.IsMaster = pl.NamePlayer.Equals(master);
				if (IsPlaying) {
					pl.IsReady = false;
				}
				GameObject objPlayer = Instantiate(player_prefab);
				objPlayer.transform.SetParent(GetParentPlayer());
				objPlayer.transform.localScale = Vector3.one;
				BasePlayer plUI = objPlayer.GetComponent<BasePlayer>();
				plUI.SetInfo(pl);
				if (pl.NamePlayer.Equals(ClientConfig.UserInfo.UNAME)) {
					playerMe = plUI;
				}
				objPlayer.SetActive(false);
				var match = ListPlayer.FirstOrDefault(item => item.NamePlayer == pl.NamePlayer);

				if (match == null) {
					ListPlayer.Add(plUI);
				} else {
					Destroy(plUI);
				}
			}

			OnJoinTableSuccess(master);
			SortSitPlayer();
		} catch (Exception ex) {
			Debug.LogException(ex);
		}
	}

	internal virtual void OnJoinView(Message message) {
		try {
			    isView = true;

			int rule = message.reader().ReadByte();
			SetLuatChoi(rule);
			string master = message.reader().ReadUTF();
			//    masterID = master;
			int len = message.reader().ReadByte();
			GameControl.instance.TimerTurnInGame = message.reader().ReadInt();
			IsPlaying = message.reader().ReadBoolean();
			for (int i = 0; i < len; i++) {
				PlayerData pl = new PlayerData();
				pl.NamePlayer = message.reader().ReadUTF();
				pl.DisplaName = message.reader().ReadUTF();
				pl.Avata_Link = message.reader().ReadUTF();
				pl.Avata_Id = message.reader().ReadInt();
				pl.SitOnSever = message.reader().ReadByte();
				pl.Money = message.reader().ReadLong();
				pl.IsReady = message.reader().ReadBoolean();
				pl.FolowMoney = message.reader().ReadLong();
				pl.IsMaster = pl.NamePlayer.Equals(master);
				if (IsPlaying) {
					pl.IsReady = false;
				}

				GameObject objPlayer = Instantiate(player_prefab);
				objPlayer.transform.SetParent(tf_parent_player);
				objPlayer.transform.localScale = Vector3.one;
				BasePlayer plUI = objPlayer.GetComponent<BasePlayer>();
				plUI.SetInfo(pl);
				if (pl.NamePlayer.Equals(ClientConfig.UserInfo.UNAME)) {
					playerMe = plUI;
				}
				objPlayer.SetActive(false);
				var match = ListPlayer.FirstOrDefault(item => item.NamePlayer == pl.NamePlayer);
				//ListPlayer.Any (item=>item.NamePlayer == pl.Name

				if (match == null) {
					ListPlayer.Add(plUI);
				} else {
					Destroy(plUI);
				}
				OnJoinTableSuccess(master);
				//    screen.dialogWaitting.onShow();
				//    if (len < BaseInfo.gI().numberPlayer)
				//        SendData.onJoinTable(BaseInfo.gI().mainInfo.nick, BaseInfo.gI().idTable, "", -1);

				//} catch (IOException ex) {
				//    ex.printStackTrace();
				//}
				//try {
				//    setTableName("Phòng ");
			}
			SortSitPlayer();
		} catch (Exception e) {
			Debug.LogException(e);
		}
	}

	internal void OnUserExitTable(string nick, string master) {
		if (nick.Equals(ClientConfig.UserInfo.UNAME)) {
			//LoadAssetBundle.LoadScene(SceneName.SCENE_ROOM, SceneName.SCENE_ROOM, () => {
			GameControl.instance.CurrentCasino = null;
			//});
		} else {
			BasePlayer pl = GetPlayerWithName(nick);
			if (pl != null) {
				tf_invite[pl.SitOnClient].gameObject.SetActive(true);
				Destroy(pl.gameObject);
				ListPlayer.Remove(pl);
			}
			BasePlayer plMaster = GetPlayerWithName(master);
			if (plMaster != null) {
				plMaster.SetShowReady(false);
				plMaster.IsMaster = true;
				plMaster.SetShowMaster(true);
			}
		}
		SortSitPlayer(true);
	}

	internal void OnUserJoinTable(Message message) {
		int tbid = message.reader().ReadShort();

		PlayerData pl = new PlayerData();
		pl.NamePlayer = message.reader().ReadUTF();
		pl.DisplaName = message.reader().ReadUTF();
		pl.Avata_Link = message.reader().ReadUTF();
		pl.Avata_Id = message.reader().ReadInt();
		pl.SitOnSever = message.reader().ReadByte();
		pl.Money = message.reader().ReadLong();
		pl.FolowMoney = message.reader().ReadLong();
		pl.IsMaster = false;
		pl.IsReady = false;

		GameObject objPlayer = Instantiate(player_prefab);
		objPlayer.transform.SetParent(tf_parent_player);
		objPlayer.transform.localScale = Vector3.one;
		BasePlayer plUI = objPlayer.GetComponent<BasePlayer>();
		plUI.SetInfo(pl);
		//		if (pl.Name.Equals (ClientConfig.UserInfo.UNAME)) {
		//			playerMe = plUI;
		//			indexMe = i;
		//		}
		objPlayer.SetActive(false);
		var match = ListPlayer.FirstOrDefault(item => item.NamePlayer == pl.NamePlayer);
		//ListPlayer.Any (item=>item.NamePlayer == pl.Name

		if (match == null) {
			ListPlayer.Add(plUI);
		} else {
			Destroy(plUI);
		}
		SortSitPlayer(true);
	}

	internal void InfoCardPlayerInTbl(Message message) {
		try {
			string turnName = message.reader().ReadUTF();
			int time = message.reader().ReadInt();
			sbyte numP = message.reader().ReadByte();
			InfoCardPlayerInTbl(message, turnName, time, numP);
		} catch (Exception e) {
			Debug.LogException(e);
		}
	}

	internal virtual void InfoCardPlayerInTbl(Message message, string turnName, int time, sbyte numP) {
		for (int i = 0; i < ListPlayer.Count; i++) {
			ListPlayer[i].IsPlaying = false;
		}
	}

	internal virtual void OnReady(string nick, bool ready) {
		BasePlayer pl = GetPlayerWithName(nick);
		if (pl != null) {
			pl.IsReady = ready;
			pl.SetShowReady(ready);
		}
	}

	internal virtual void StartTableOk(int[] cardHand, Message msg, string[] nickPlay) {
		GameControl.instance.TimerTurnInGame = 20;
		isView = false;
		for (int i = 0; i < nickPlay.Length; i++) {
			BasePlayer pl = GetPlayerWithName(nickPlay[i]);
			if (pl != null) {
				pl.IsReady = false;
				pl.SetShowReady(false);
				pl.IsPlaying = true;
			}
		}
	}

	internal virtual void OnStartForView(string[] nickPlay, Message msg) {
		for (int i = 0; i < ListPlayer.Count; i++) {
			ListPlayer[i].SetShowReady(false);
			ListPlayer[i].IsPlaying = false;
		}
		for (int i = 0; i < nickPlay.Length; i++) {
			BasePlayer pl = GetPlayerWithName(nickPlay[i]);
			if (pl != null) {
				pl.IsPlaying = (true);
			}
			//if (playingName[i].equals(BaseInfo.gI().mainInfo.nick)) {
			//    MainInfo.setPlayingUser(false);
			//}
		}
	}

	internal virtual void OnStartFail() {
	}

	internal virtual void SetTurn(string nick, Message message) {
		if (string.IsNullOrEmpty(nick)) {
			return;
		}
		for (int i = 0; i < ListPlayer.Count; i++) {
			ListPlayer[i].SetTurn(0);
		}
		BasePlayer plTurn = GetPlayerWithName(nick);
		if (plTurn != null) {
			plTurn.SetTurn(GameControl.instance.TimerTurnInGame);
		}
	}

	internal virtual void OnFireCard(string nick, string turnName, int[] card) {
		BasePlayer plFire = GetPlayerWithName(nick);
		if (plFire != null) {
			plFire.SetTurn(0);
		}
		BasePlayer plTurn = GetPlayerWithName(nick);
		if (plTurn != null) {
			plTurn.SetTurn(GameControl.instance.TimerTurnInGame);
		}
		SetTurn(turnName, null);
	}

	internal virtual void OnNickSkip(string nick, string turnname2) {
		BasePlayer plTurn = GetPlayerWithName(nick);
		if (plTurn != null) {
			plTurn.SetEffect(ClientConfig.Language.GetText("ingame_leave"));
			plTurn.SetTurn(0);
		}
	}

	internal virtual void OnNickSkip(string nick, Message msg) {
		BasePlayer plTurn = GetPlayerWithName(nick);
		if (plTurn != null) {
			plTurn.SetEffect(ClientConfig.Language.GetText("ingame_leave"));
			plTurn.SetTurn(0);
			//BaseInfo.gI().media_countdown.pause();
		}
	}

	internal virtual void SetMaster(string nick) {
		for (int i = 0; i < ListPlayer.Count; i++) {
			ListPlayer[i].IsMaster = false;
			ListPlayer[i].SetShowMaster(false);
		}
		BasePlayer plMaster = GetPlayerWithName(nick);
		if (plMaster != null) {
			plMaster.IsMaster = true;
			plMaster.IsReady = true;
			plMaster.SetShowMaster(true);
			playerMe.SetShowReady(false);
		}
	}

	internal void OnUpdateMoneyTbl(Message message) {
		try {
			int size = message.reader().ReadByte();
			string _name = "";
			long money = 0;
			//			long folowMoney = 0;
			for (int i = 0; i < size; i++) {
				_name = message.reader().ReadUTF();
				money = message.reader().ReadLong();
				long folowMoney = message.reader().ReadLong();
				bool isGetMoney = message.reader().ReadBoolean();
				BasePlayer pl = GetPlayerWithName(_name);
				if (pl != null) {
					long m = folowMoney - pl.Money;
					//					Debug.LogError (m + " folowMoney -  pl.Money " + folowMoney + "  " +pl.Money);
					pl.SetMoney(folowMoney);
					if (!isGetMoney) {
						pl.SetEffect(m > 0 ? "+" + MoneyHelper.FormatMoneyNormal(m) : MoneyHelper.FormatMoneyNormal(m));
					}
					if (_name.Equals(ClientConfig.UserInfo.UNAME)) {
						ClientConfig.UserInfo.CASH_FREE = money;
					}
				}
			}
		} catch (Exception ex) {
			Debug.LogError(ex);
		}
	}

	internal virtual void OnTimeAuToStart(int time) {}

	internal void OnSetMoneyTable(Message message) {
		long money = message.reader().ReadLong();
	}
	internal virtual void AllCardFinish(string nick, int[] card) {}
	internal virtual void OnNickTheo(Message message) {}
	internal virtual void OnNickCuoc(Message message) {}
	internal  void OnInfoWin(List<InfoWin> listInfoWin) {
		StartCoroutine(IEOnInfoWin(listInfoWin));
	}

	IEnumerator IEOnInfoWin(List<InfoWin> listInfoWin) {
		yield return new WaitForEndOfFrame();
       for (int i = 0; i<listInfoWin.Count; i++) {
			InfoWin inf = listInfoWin[i];
            OnInfoWinPlayer(inf);
			yield return new WaitForSeconds(1.5f);
		}
	}
	 
	internal virtual void OnInfoWinPlayer(InfoWin infoWinPlayer) {
	}
	#endregion

	#region Xep cho ngoi

	public void SortSitPlayer(bool issEffect = false) {
		int indexMe = 0;
		int j = 1;
		int i = 0;
		for (i = 0; i < ListPlayer.Count; i++) {
			if (ListPlayer[i].NamePlayer.Equals(ClientConfig.UserInfo.UNAME)) {
				indexMe = i;
				playerMe = ListPlayer[i];
				ListPlayer[i].SitOnClient = 0;
				break;
			}
		}
		if (playerMe != null) {
			playerMe.transform.localPosition = tf_invite[0].localPosition;
			playerMe.SitOnClient = 0;
			playerMe.gameObject.SetActive(true);
			playerMe.name = ClientConfig.UserInfo.UNAME;
		}
		for (i = indexMe + 1; i < ListPlayer.Count; i++) {
			if (j >= tf_invite.Length)
				break;
			ListPlayer[i].transform.localPosition = tf_invite[j].localPosition;
			tf_invite[j].gameObject.SetActive(false);
			ListPlayer[i].gameObject.SetActive(true);
			ListPlayer[i].SitOnClient = j;
			ListPlayer[i].name = ListPlayer[i].NamePlayer;
			j++;
		}
		j = tf_invite.Length - 1;
		for (i = indexMe - 1; i >= 0; i--) {
			if (j <= 0)
				break;
			ListPlayer[i].transform.localPosition = tf_invite[j].localPosition;
			tf_invite[j].gameObject.SetActive(false);
			ListPlayer[i].gameObject.SetActive(true);
			ListPlayer[i].SitOnClient = j;
			ListPlayer[i].name = ListPlayer[i].NamePlayer;
			j--;
		}
		Debug.LogError("Ngoi dc roi!");
		switch (GameConfig.CurrentGameID) {
			case GameID.TLMN:
			case GameID.TLMNSL:
				InitPlayerTLMN();
				break;
			case GameID.SAM:
				InitPlayerSam();
				break;
			case GameID.PHOM:
				InitInfoPlayer_PHOM();
				break;
			case GameID.MAUBINH:
				InitInfoPlayer_MAUBINH();
				break;
			case GameID.XOCDIA:
				InitInfoPlayer_XOCDIA();
				break;
			case GameID.LIENG:
			case GameID.BACAY:
				InitPlayerLieng();
				break;
			case GameID.POKER:
				InitPlayerPoker();
				break;
			case GameID.XITO:
				InitPlayerXiTo();
				break;
		}
	}

	#region Init Player TLMN

	void InitPlayerTLMN() {
		for (int i = 0; i < ListPlayer.Count; i++) {
			TLMNPlayer pl = (TLMNPlayer)ListPlayer[i];
			pl.CardHand.CardCount = 13;
			switch (pl.SitOnClient) {
				case 0:
					pl.CardHand.isSmall = false;
					pl.CardHand.isTouched = true;
					pl.CardHand.align_Anchor = Align_Anchor.CENTER;
					pl.CardHand.MaxWidth = 800;
					pl.SetPositionChatLeft(true);
					pl.CardHand.Init();
					pl.CardHand.SetInputChooseCard();
					break;
				case 1:
					pl.CardHand.isSmall = true;
					pl.CardHand.isTouched = false;
					pl.CardHand.align_Anchor = Align_Anchor.RIGHT;
					pl.CardHand.MaxWidth = 500;
					pl.SetPositionChatLeft(false);
					pl.SetPositionChatAction(Align_Anchor.RIGHT);
					pl.CardHand.Init();
					break;
				case 2:
					pl.CardHand.isSmall = true;
					pl.CardHand.isTouched = false;
					pl.CardHand.align_Anchor = Align_Anchor.LEFT;
					pl.CardHand.MaxWidth = 500;
					pl.SetPositionChatLeft(true);
					pl.SetPositionChatAction(Align_Anchor.BOT);
					pl.CardHand.Init();
					break;
				case 3:
					pl.CardHand.isSmall = true;
					pl.CardHand.isTouched = false;
					pl.CardHand.align_Anchor = Align_Anchor.LEFT;
					pl.CardHand.MaxWidth = 500;
					pl.SetPositionChatLeft(true);
					pl.SetPositionChatAction(Align_Anchor.LEFT);
					pl.CardHand.Init();
					break;
			}
		}
	}

	#endregion

	#region Init Player Sam

	void InitPlayerSam() {
		for (int i = 0; i < ListPlayer.Count; i++) {
			SamPlayer pl = (SamPlayer)ListPlayer[i];
			pl.CardHand.CardCount = 10;
			switch (pl.SitOnClient) {
				case 0:
					pl.CardHand.isSmall = false;
					pl.CardHand.isTouched = true;
					pl.CardHand.align_Anchor = Align_Anchor.CENTER;
					pl.CardHand.MaxWidth = 800;
					pl.SetPositionChatLeft(true);
					pl.CardHand.Init();
					pl.CardHand.SetInputChooseCard();
					break;
				case 1:
					pl.CardHand.isSmall = true;
					pl.CardHand.isTouched = false;
					pl.CardHand.align_Anchor = Align_Anchor.RIGHT;
					pl.CardHand.MaxWidth = 500;
					pl.SetPositionChatLeft(false);
					pl.SetPositionChatAction(Align_Anchor.RIGHT);
					pl.CardHand.Init();
					break;
				case 2:
					pl.CardHand.isSmall = true;
					pl.CardHand.isTouched = false;
					pl.CardHand.align_Anchor = Align_Anchor.LEFT;
					pl.CardHand.MaxWidth = 500;
					pl.SetPositionChatLeft(true);
					pl.SetPositionChatAction(Align_Anchor.BOT);
					pl.CardHand.Init();
					break;
				case 3:
					pl.CardHand.isSmall = true;
					pl.CardHand.isTouched = false;
					pl.CardHand.align_Anchor = Align_Anchor.LEFT;
					pl.CardHand.MaxWidth = 500;
					pl.SetPositionChatLeft(true);
					pl.SetPositionChatAction(Align_Anchor.LEFT);
					pl.CardHand.Init();
					break;
			}
		}
	}

	#endregion

	#region Init Player Phom

	public void InitInfoPlayer_PHOM() {
		for (int i = 0; i < ListPlayer.Count; i++) {
			PhomPlayer pl = (PhomPlayer)ListPlayer[i];

			switch (pl.SitOnClient) {
				case 0:
					pl.cardTaLaManager.Init(true);
					pl.cardTaLaManager.SetDragDropCard();
					pl.cardTaLaManager.SetPositionArryCard(Align_Anchor.CENTER, Align_Anchor.LEFT, Align_Anchor.CENTER, pl.SitOnClient);
					pl.SetPositionChatLeft(true);
					break;
				case 1:
					pl.cardTaLaManager.Init(false);
					pl.SetPositionChatLeft(false);
					pl.cardTaLaManager.SetPositionArryCard(Align_Anchor.RIGHT, Align_Anchor.RIGHT, Align_Anchor.RIGHT, pl.SitOnClient);
					pl.SetPositionChatAction(Align_Anchor.RIGHT);
					break;
				case 2:
					pl.cardTaLaManager.Init(false);
					pl.cardTaLaManager.SetPositionArryCard(Align_Anchor.LEFT, Align_Anchor.RIGHT, Align_Anchor.CENTER, pl.SitOnClient);
					pl.SetPositionChatLeft(false);
					pl.SetPositionChatAction(Align_Anchor.BOT);
					break;
				case 3:
					pl.cardTaLaManager.Init(false);
					pl.cardTaLaManager.SetPositionArryCard(Align_Anchor.LEFT, Align_Anchor.LEFT, Align_Anchor.LEFT, pl.SitOnClient);
					pl.SetPositionChatLeft(true);
					pl.SetPositionChatAction(Align_Anchor.LEFT);
					break;
			}
		}
	}

	#endregion

	#region Init Player Mau Binh

	public void InitInfoPlayer_MAUBINH() {
		for (int i = 0; i < ListPlayer.Count; i++) {
			MauBinhPlayer pl = (MauBinhPlayer)ListPlayer[i];

			switch (pl.SitOnClient) {
				case 0:
					pl.cardMauBinh.Init(true);
					pl.cardMauBinh.SetPositionArryCard(Align_Anchor.CENTER);
					pl.SetPositionChatLeft(true);
					break;
				case 1:
					pl.cardMauBinh.Init(false);
					pl.SetPositionChatLeft(false);
					pl.cardMauBinh.SetPositionArryCard(Align_Anchor.RIGHT);
					pl.SetPositionChatAction(Align_Anchor.RIGHT);
					break;
				case 2:
					pl.cardMauBinh.Init(false);
					pl.cardMauBinh.SetPositionArryCard(Align_Anchor.LEFT);
					pl.SetPositionChatLeft(false);
					pl.SetPositionChatAction(Align_Anchor.BOT);
					break;
				case 3:
					pl.cardMauBinh.Init(false);
					pl.cardMauBinh.SetPositionArryCard(Align_Anchor.LEFT);
					pl.SetPositionChatLeft(true);
					pl.SetPositionChatAction(Align_Anchor.LEFT);
					break;
			}
		}
	}

	#endregion

	#region Init Player Xoc Dia

	public void InitInfoPlayer_XOCDIA() {
		for (int i = 0; i < ListPlayer.Count; i++) {
			XocDiaPlayer pl = (XocDiaPlayer)ListPlayer[i];

			switch (pl.SitOnClient) {
				case 0:
				case 5:
				case 6:
				case 7:
				case 8:
					pl.SetPositionChatLeft(true);
					break;
				case 1:
				case 2:
				case 3:
				case 4:
					pl.SetPositionChatLeft(false);
					break;
			}
		}
	}

	#endregion

	#region Init Player Lieng Ba Cay

	void InitPlayerLieng() {
		Debug.LogError("Init player lieng!");
		for (int i = 0; i < ListPlayer.Count; i++) {
			LiengPlayer pl = (LiengPlayer)ListPlayer[i];
			pl.CardHand.CardCount = 3;
			switch (pl.SitOnClient) {
				case 0:
					pl.CardHand.isSmall = false;
					pl.CardHand.isTouched = true;
					pl.CardHand.align_Anchor = Align_Anchor.LEFT;
					pl.CardHand.MaxWidth = 300;
					pl.SetPositionChatLeft(true);
					pl.CardHand.Init();
					//pl.CardHand.SetInputChooseCard();
					pl.chipControl.SetPosititon(Align_Anchor.TOP);
					break;
				case 1:
				case 2:
					pl.CardHand.isSmall = true;
					pl.CardHand.isTouched = false;
					pl.CardHand.align_Anchor = Align_Anchor.RIGHT;
					pl.CardHand.MaxWidth = 140;
					pl.SetPositionChatLeft(true);
					pl.SetPositionChatAction(Align_Anchor.BOT);
					pl.CardHand.Init();

					pl.chipControl.SetPosititon(Align_Anchor.RIGHT);
					break;
				case 3:
				case 4:
					pl.CardHand.isSmall = true;
					pl.CardHand.isTouched = false;
					pl.CardHand.align_Anchor = Align_Anchor.LEFT;
					pl.CardHand.MaxWidth = 140;
					pl.SetPositionChatLeft(true);
					pl.SetPositionChatAction(Align_Anchor.LEFT);
					pl.CardHand.Init();

					pl.chipControl.SetPosititon(Align_Anchor.LEFT);
					break;
			}
		}
	}

	#endregion

	#region Init Player Poker

	void InitPlayerPoker() {
		for (int i = 0; i < ListPlayer.Count; i++) {
			LiengPlayer pl = (LiengPlayer)ListPlayer[i];
			pl.CardHand.CardCount = 2;
			switch (pl.SitOnClient) {
				case 0:
					pl.CardHand.isSmall = false;
					pl.CardHand.isTouched = true;
					pl.CardHand.align_Anchor = Align_Anchor.LEFT;
					pl.CardHand.MaxWidth = 200;
					pl.SetPositionChatLeft(true);
					pl.CardHand.Init();

					pl.chipControl.SetPosititon(Align_Anchor.TOP);
					break;
				case 1:
				case 2:
					pl.CardHand.isSmall = true;
					pl.CardHand.isTouched = false;
					pl.CardHand.align_Anchor = Align_Anchor.RIGHT;
					pl.CardHand.MaxWidth = 140;
					pl.SetPositionChatLeft(true);
					pl.SetPositionChatAction(Align_Anchor.BOT);
					pl.CardHand.Init();

					pl.chipControl.SetPosititon(Align_Anchor.RIGHT);
					break;
				case 3:
				case 4:
					pl.CardHand.isSmall = true;
					pl.CardHand.isTouched = false;
					pl.CardHand.align_Anchor = Align_Anchor.LEFT;
					pl.CardHand.MaxWidth = 140;
					pl.SetPositionChatLeft(true);
					pl.SetPositionChatAction(Align_Anchor.LEFT);
					pl.CardHand.Init();

					pl.chipControl.SetPosititon(Align_Anchor.LEFT);
					break;
			}
		}
	}

	#endregion

	#region Init Player Xi To

	void InitPlayerXiTo() {
		//Debug.LogError("Init Xi to");
		for (int i = 0; i < ListPlayer.Count; i++) {
			LiengPlayer pl = (LiengPlayer)ListPlayer[i];
			pl.CardHand.CardCount = 5;
			switch (pl.SitOnClient) {
				case 0:
					pl.CardHand.isSmall = false;
					pl.CardHand.isTouched = true;
					pl.CardHand.align_Anchor = Align_Anchor.LEFT;
					pl.CardHand.MaxWidth = 400;
					pl.SetPositionChatLeft(true);
					pl.CardHand.Init();

					pl.CardHand.ClickCardXiTo();
					pl.chipControl.SetPosititon(Align_Anchor.TOP);
					break;
				case 1:
				case 2:
					pl.CardHand.isSmall = true;
					pl.CardHand.isTouched = false;
					pl.CardHand.align_Anchor = Align_Anchor.RIGHT;
					pl.CardHand.MaxWidth = 200;
					pl.SetPositionChatLeft(true);
					pl.SetPositionChatAction(Align_Anchor.BOT);
					pl.CardHand.Init();

					pl.chipControl.SetPosititon(Align_Anchor.RIGHT);
					break;
				case 3:
				case 4:
					pl.CardHand.isSmall = true;
					pl.CardHand.isTouched = false;
					pl.CardHand.align_Anchor = Align_Anchor.LEFT;
					pl.CardHand.MaxWidth = 200;
					pl.SetPositionChatLeft(true);
					pl.SetPositionChatAction(Align_Anchor.LEFT);
					pl.CardHand.Init();

					pl.chipControl.SetPosititon(Align_Anchor.LEFT);
					break;
			}
		}
	}

	#endregion

	#endregion

	internal BasePlayer GetPlayerWithName(string nick) {
		for (int i = 0; i < ListPlayer.Count; i++) {
			if (ListPlayer[i].NamePlayer.Equals(nick)) {
				return ListPlayer[i];
			}
		}
		return null;
	}

	#region Button Click

[SkipRename]
	public void OnClickBack() {
		PopupAndLoadingScript.instance.messageSytem.OnShow(ClientConfig.Language.GetText("popup_quittable"), () => {
			SendData.onOutTable();
			PopupAndLoadingScript.instance.ShowLoading();
		});
	}

[SkipRename]
	public void OnClickChat() {
		LoadAssetBundle.LoadScene(SceneName.SUB_CHAT, SceneName.SUB_CHAT);
	}

[SkipRename]
	public void OnClickSetting() {
		LoadAssetBundle.LoadScene(SceneName.SUB_SETTING, SceneName.SUB_SETTING);
	}

	#endregion

	int index = 0;
	public void DemoNguoiVao() {
		PlayerData pl = new PlayerData();
		pl.NamePlayer = "ten";
		pl.DisplaName = "Tendep" + index;
		index++;
		pl.Avata_Link = "";
		pl.Avata_Id = UnityEngine.Random.Range(0, 30);
		pl.SitOnSever = 1;
		pl.Money = 9999;
		pl.IsReady = true;
		pl.FolowMoney = 9999;

		GameObject objPlayer = Instantiate(player_prefab);
		objPlayer.transform.SetParent(tf_parent_player);
		BasePlayer plUI = objPlayer.GetComponent<BasePlayer>();
		plUI.SetInfo(pl);
		objPlayer.SetActive(false);
		if (!ListPlayer.Contains(plUI)) {
			ListPlayer.Add(plUI);
		} else {
			Destroy(plUI);
		}
		SortSitPlayer();
	}
}
