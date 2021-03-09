using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class FoodUI : MonoBehaviour
{
    [SerializeField] private TMPro.TMP_Text foodCount;
    [SerializeField] private TMPro.TMP_Text drinksCount;

    private void Start()
    {
        ShowUI();
        GameManager.OnDrinksChanged += ShowUI;
        GameManager.OnFoodChanged += ShowUI;
    }

    private void OnDestroy()
    {
        GameManager.OnDrinksChanged -= ShowUI;
        GameManager.OnFoodChanged -= ShowUI;
    }

    private void ShowUI()
    {
        foodCount.SetText(GameManager.Food + "");
        drinksCount.SetText(GameManager.Drinks + "");
        foreach (MaskableGraphic graphic in GetComponentsInChildren<MaskableGraphic>())
            graphic.DOFade(1, .5f);
        StopAllCoroutines();
        StartCoroutine(HideUI());
    }

    private IEnumerator HideUI()
    {
        yield return new WaitForSeconds(5f);
        foreach (MaskableGraphic graphic in GetComponentsInChildren<MaskableGraphic>())
            graphic.DOFade(0, .5f);
    }
}
