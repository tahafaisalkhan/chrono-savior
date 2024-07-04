using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeTemplate : MonoBehaviour
{
    [SerializeField] Text titleText;
    [SerializeField] Text upgradeCountText;
    [SerializeField] Text priceText;

    public void SetUpgradeItem(UpgradeTemplateSO upgradeItem)
    {
        titleText.text = upgradeItem.title;
        if(StateManagement.Instance != null)
        {
            int index = StateManagement.Instance.GetUpgradeIndex(upgradeItem.title);
            upgradeCountText.text = (index) + "/5";
            priceText.text = "Coins: " + upgradeItem.prices[index].ToString();
        }
        else
        {
            upgradeCountText.text = "0/5";
            priceText.text = "Coins: " + upgradeItem.prices[0].ToString();
        }
        
        
        
    }
}