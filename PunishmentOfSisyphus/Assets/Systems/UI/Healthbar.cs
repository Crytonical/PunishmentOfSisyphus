using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Ephymeral.Events;
using Ephymeral.Data;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{
    #region REFERENCES
    [SerializeField] private PlayerEvent playerEvent;
    [SerializeField] private PlayerData playerData;
    private Image hpBar;
    #endregion


    private void Awake()
    {
        hpBar = GetComponent<Image>();
    }

    private void OnEnable()
    {
        playerEvent.damageEvent.AddListener(AdjustHealthBar);
    }

    private void OnDisable()
    {
        playerEvent.damageEvent.RemoveListener(AdjustHealthBar);
    }

    private void AdjustHealthBar(float damageTaken)
    {
        // Need to get current HP
        Debug.Log(hpBar.gameObject.name + " Fill Amount Before" + hpBar.fillAmount);
        hpBar.fillAmount = damageTaken / playerData.MAX_HP;
        Debug.Log("Healthbar Fill Amount After" + hpBar.fillAmount);
    }
}

