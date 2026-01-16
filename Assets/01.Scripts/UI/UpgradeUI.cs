using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeUI : MonoBehaviour
{
    public static UpgradeUI instance;

    public Button[] upgradeButtons;

    public Image[] upgradeIcons;
    public TextMeshProUGUI[] upgradeNames;
    public TextMeshProUGUI[] upgradeDescriptions;

    private List<UpgradeSO> currentChoices;

    private void Awake()
    {
        instance = this;
        gameObject.SetActive(false);
        Events.OnLevelUp += ShowUpgrades;
    }
    private void OnDestroy()
    {
        Events.OnLevelUp -= ShowUpgrades;
    }
    private void ShowUpgrades(int level, List<UpgradeSO> choices) 
    {
        currentChoices = choices;
        gameObject.SetActive(true);

        for (int i = 0; i < upgradeButtons.Length; i++) 
        {
            if (i < choices.Count) 
            {
                UpgradeSO upgrade = choices[i];

                upgradeButtons[i].gameObject.SetActive(true);
                upgradeIcons[i].sprite = upgrade.icon;
                upgradeNames[i].text = upgrade.name;
                upgradeDescriptions[i].text = upgrade.description;

                int index = i;
                upgradeButtons[i].onClick.RemoveAllListeners();
                upgradeButtons[i].onClick.AddListener(() => OnUpgradeSelected(currentChoices[index]));
            }
        }
    }

    public void OnUpgradeSelected(UpgradeSO selected) 
    {
        Events.RaiseUpgradeSelected(selected);
        gameObject.SetActive(false);
    }
}
