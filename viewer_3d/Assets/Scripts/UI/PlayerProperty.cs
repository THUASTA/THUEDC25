using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerProperty : MonoBehaviour
{
    public GameObject HPSlider;
    public GameObject ATKSlider;
    public GameObject APSlider;
    public GameObject HPText;
    public GameObject ATKText;
    public GameObject APText;
    public int playerId = -1;

    public void maxHP(int maxHP)
    {
        HPSlider.GetComponent<Slider>().maxValue = maxHP;
    }
    public void HPFluctuation(int HP)
    {
        HPSlider.GetComponent<Slider>().value = HP;
    }

    public void ATKFluctuation(int ATK)
    {
        ATKSlider.GetComponent<Slider>().value = ATK;
    }

    public void APFluctuation(float ATK)
    {
        APSlider.GetComponent<Slider>().value = ATK;
    }

    public void HPScore(int HP)
    {
        HPText.GetComponent<TextMeshProUGUI>().text = HP.ToString();
    }

    public void ATKScore(int ATK)
    {
        ATKText.GetComponent<TextMeshProUGUI>().text = ATK.ToString();
    }

    public void APScore(float AP)
    {
        APText.GetComponent<TextMeshProUGUI>().text =(AP.ToString());
    }
}
