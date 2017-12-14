using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilityUI : MonoBehaviour {

    public Image abImg;
    public Slider slider;
    public Image sliBG, sliFill;
    public Sprite[] sprites;

    Sprite boostSpr, bounceSpr, climbSpr, rewindSpr;
    SessionData.Ability lastAb;
    PlayerController player;

	// Use this for initialization
	void Start () {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        AssignSprites();
        SetAbility(true);
        SetMeter();
    }

    // Update is called once per frame
    void Update () {
        SetAbility();
        SetMeter();
    }

    void AssignSprites() {
        boostSpr = sprites[0];
        bounceSpr = sprites[1];
        climbSpr = sprites[2];
        rewindSpr = sprites[3];
    }

    void SetAbility(bool forceChange = false) {
        SessionData.Ability ab = SessionData.currentAbility;

        // Only update when we change abilities.
        if (!forceChange && lastAb == ab) return;

        switch (ab) {
            case SessionData.Ability.None:
                abImg.color = Color.clear;
                sliFill.color = Color.clear;
                sliBG.color = Color.clear;
                break;
            case SessionData.Ability.Boost:
                ShowImageAndColors(boostSpr, Color.red);
                break;
            case SessionData.Ability.Bounce:
                ShowImageAndColors(bounceSpr, Color.blue);
                break;
            case SessionData.Ability.Climb:
                ShowImageAndColors(climbSpr, new Color(.80392f, .52156f, .24705f)); //taken from Climb.cs
                break;
            case SessionData.Ability.Rewind:
                ShowImageAndColors(rewindSpr, Color.green);
                break;
        }

        lastAb = ab;
    }

    void SetMeter() {
        SessionData.Ability ab = SessionData.currentAbility;
        if (ab == SessionData.Ability.None) return;

        slider.value = player.GetAbilityPP(ab);
    }

    void ShowImageAndColors(Sprite pic, Color baseColor) {
        abImg.color = Color.white;
        abImg.sprite = pic;

        Color darkerColor = baseColor;
        darkerColor.r *= .333f;
        darkerColor.g *= .333f;
        darkerColor.b *= .333f;

        sliFill.color = baseColor;
        sliBG.color = darkerColor;
    }
}
