﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimationsEffect : MonoBehaviour {
    [SerializeField]
    Sprite[] spriteAnims;
    [SerializeField]
    Image img;
    [SerializeField]
    float speed = 1;
    [SerializeField]
    bool isLoop = false;
    // Use this for initialization
    void Start() {
//        StartCoroutine(RunAnim());
		InvokeRepeating ("RunAnim",0.5f,0.5f);
    }
//    IEnumerator RunAnim() {
//        yield return new WaitForEndOfFrame();
//        for (int i = 0; i < spriteAnims.Length; i++) {
//            img.sprite = spriteAnims[i];
//            yield return new WaitForSeconds(speed * 0.1f);
//        }
//        if (isLoop) {
//            yield return new WaitForSeconds(speed * 0.1f);
//            StartCoroutine(RunAnim());
//        }
//    }
	int index = 0;
	void RunAnim(){
		img.sprite = spriteAnims[index];
		index++;
	}
}
