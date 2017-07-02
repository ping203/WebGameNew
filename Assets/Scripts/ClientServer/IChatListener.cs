using UnityEngine;
using System.Collections;

interface IChatListener {
	void onDisConnect();
	void OnLogin(Message message);
	void OnGetPass(Message message);
	void OnGetPhoneCSKH(Message message);
	void OnRegister(Message message);
	void OnMessageServer(string message);
	void OnPopupNotify(Message message);
	void OnProfile(Message message);
	void OnChangeName(Message message);
	void OnChangePass(Message message);
	void OnChangeAvatar(Message message);
	void OnRateScratchCard(Message message);
	void OnMoneyFree(long money);
	void OnHistoryTranfer(Message message);
	void OnListBetMoney(Message message);
	void OnListProduct(Message message);
	void InfoGift(Message message);
	void OnTop(Message message);
	void OnInboxMessage(Message message);
	void OnGetAlertLink(Message message);
	void OnInfoSMS(Message message);
	void OnSMS9029(Message message);
	void OnListInvite(Message message);
	void OnJoinGame(Message message);
	void OnJoinRoom(Message message);
	void OnGameID(Message message);
	void OnListTable(int totalTB, Message message);
	void OnUpdateRoom(Message message);
	void OnInvite(Message message);
	void OnChat(Message message);
	void OnJoinTableSuccess(Message message);
	void OnJoinTableFail(string info);
	void OnJoinTablePlay(Message message);
	void OnUserExitTable(Message message);
	void OnUserJoinTable(Message message);
	void OnUpdateMoneyTbl(Message message);
	void OnTimeAuToStart(Message message);
	void InfoCardPlayerInTbl(Message message);
	void OnReady(Message message);
	void OnStartFail(string info);
	void OnStartSuccess(Message message);
	void OnStartForView(Message message);
	void OnSetNewMaster(string nick);
	void OnNickSkip(string nick, string turnName);
	void OnNickSkip(string nick, Message msg);
	//void OnFrieCard(Message message);
	void OnFinishGame(Message message);
	void OnAllCardPlayerFinish(Message message);
	void OnFinishTurnTLMN(Message message);
	void OnSetTurn(Message message);
	void OnBaoSam(Message message);
	void OnGetCardNocSuccess(Message message);
	void OnEatCardSuccess(Message message);
	void OnBalanceCard(Message message);
	void OnDropPhomSuccess(Message message);
	void OnAttachCard(Message message);
	void OnPhomha(Message message);
	void OnChangeRuleTbl(Message message);
	void OnRankMauBinh(Message message);
	void OnFinalMauBinh(Message message);
	void OnWinMauBinh(Message message);

	#region Xoc dia
	void OnBeGinXocDia(int time);
	void OnXocDia_DatCuoc(Message message);
	void OnXocDia_DatX2(Message message);
	void OnXocDia_DatLai(Message message);
	void OnBeGinXocDia_TG_DatCuoc(int time);
	void OnXocDia_TG_DungCuoc(Message message);
	void OnBeGinXocDia_MoBat(int quando);
	void OnXocDiaUpdateCua(Message message);
	void OnXocDiaHuyCuoc(Message message);
	void OnNhanCacMucCuocXD(Message message);
	void OnXocDia_LichSu(Message message);
	void OnXocDia_HuyCua_LamCai(Message message);
	#endregion
	void OnSetMoneyTable(Message message);
	void OnNickTheo(Message message);
	void OnNickSkip(Message message);
	void OnNickCuoc(Message message);
	void OnBeginRiseBacay(Message message);
	void OnCuoc3Cay(Message message);
	void OnInfoPockerTbale(Message message);
	void OnAddCardTbl(Message message);
	#region TAI XIU
	void OnUpdateMoneyTaiXiu(Message message);
	void OnJoinTaiXiu(Message message);
	void OnTimeStartTaiXiu(Message message);
	void OnAutoStartTaiXiu(Message message);
	void OnGameoverTaiXiu(Message message);
	void OnCuocTaiXiu(Message message);
	void OnInfoTaiXiu(Message message);
	void OnInfoLSTheoPhienTaiXiu(Message message);
	#endregion
}