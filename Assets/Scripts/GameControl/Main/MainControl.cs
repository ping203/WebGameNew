﻿using AppConfig;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainControl : MonoBehaviour {
    public void OnClick_LoginFacebook() {
    }
    public void OnClick_Login() {
        LoadAssetBundle.LoadScene(SceneName.SUB_LOGIN, SceneName.SUB_LOGIN);
    }
    public void OnClick_Reg() {
        LoadAssetBundle.LoadScene(SceneName.SUB_REGISTER, SceneName.SUB_REGISTER);
    }
    public void OnClick_Setting() {
        LoadAssetBundle.LoadScene(SceneName.SUB_SETTING, SceneName.SUB_SETTING);
    }
    public void OnClick_Hotline() {

    }
}
