using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UiController : MonoBehaviour
{
    public static UiController instance;
    public GameObject coinsWarn;
    public GameObject drawCardButton, endTurnButton, enemyTurn, makeCalc, playerTurn;
    public float coinsWarnTime;
    private float coinsWarnCounter;

    private void Awake()
    {
        instance = this;
    }

    public TMP_Text playerManaText;

    // Start is called before the first frame update
    void Start()
    {
        playerTurn.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if(coinsWarnCounter > 0)
        {
            coinsWarnCounter -= Time.deltaTime;
            if(coinsWarnCounter < 0)
            {
                coinsWarn.SetActive(false);
            }
        }
    }

    public void SetPlayerCoinsText(int mana)
    {
        playerManaText.text = "Coins: " + mana;
    }

    public void ShowCoinsWarn()
    {
        coinsWarn.SetActive(true);
        coinsWarnCounter = coinsWarnTime;
    }
    public void DrawCard()
    {
        DeckController.instance.DrawCost();
    }

    public void EndTurn()
    {
        BattleController.instance.EndPlayerTurn();
    }
}
