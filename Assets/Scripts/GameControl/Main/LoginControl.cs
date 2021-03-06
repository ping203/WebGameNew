﻿using AppConfig;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using Beebyte.Obfuscator;

public class LoginControl : MonoBehaviour {
    public static LoginControl instance;
    [SerializeField]
    InputField ip_tk, ip_mk, ip_tk_quenmk;
    [SerializeField]
    Toggle tg_ghi_nho_mk;

	[SerializeField]
	UIPopUp ui_quen_mk;

    void Awake() {
        instance = this;
    }

    // Use this for initialization
    void Start() {
        Init();
        GameControl.instance.IsShowNoti = true;
//		SettingConfig.IsInvite = true;
    }
    public void Init() {
        ip_tk.text = ClientConfig.UserInfo.UNAME;
        ip_mk.text = ClientConfig.UserInfo.PASSWORD;
        tg_ghi_nho_mk.isOn = ClientConfig.UserInfo.SAVE_PASS == 1;
    }
    #region Click  
	[SkipRename]
    public void OnClick_Login() {
        string tk = ip_tk.text.Trim();
        string mk = ip_mk.text.Trim();
        if (string.IsNullOrEmpty(tk) || (tk.Length < 5 && tk.Length > 20) || !Regex.IsMatch(tk, @"^([a-zA-Z0-9])+$")) {
            PopupAndLoadingScript.instance.messageSytem.OnShow(ClientConfig.Language.GetText("register_tenkhonghople"));
            return;
        } else if (string.IsNullOrEmpty(mk) || (mk.Length < 5 && mk.Length > 20) || !Regex.IsMatch(mk, @"^([a-zA-Z0-9])+$")) {
            PopupAndLoadingScript.instance.messageSytem.OnShow(ClientConfig.Language.GetText("register_matkhauyeu"));
            return;
        }
        string imei = ClientConfig.HardWare.IMEI;//"352888065147086";
        SendData.doLogin(tk, mk, 4, imei, "", 0, tk, "", "", false);

        PopupAndLoadingScript.instance.ShowLoading();
    }

[SkipRename]
    public void OnClick_Ghi_Nho_Mk() {
        ClientConfig.UserInfo.SAVE_PASS = tg_ghi_nho_mk.isOn ? 1 : 0;
    }

[SkipRename]
	public void OnClick_LayLai_MK(){
		string tk_mk = ip_tk_quenmk.text.Trim ();
		if (string.IsNullOrEmpty(tk_mk) || (tk_mk.Length < 5 && tk_mk.Length > 20) || !Regex.IsMatch(tk_mk, @"^([a-zA-Z0-9])+$")) {
			PopupAndLoadingScript.instance.messageSytem.OnShow (ClientConfig.Language.GetText ("register_tenkhonghople"));
			ui_quen_mk.HideDialog ();
			return;
		}
		SendData.onGetPass (tk_mk);
		ui_quen_mk.HideDialog ();
	}
    #endregion
}
