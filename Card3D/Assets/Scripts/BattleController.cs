using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleController : MonoBehaviour
{
    public enum TurnOrder { playerTurn, enemyTurn, calculateRound}
    public TurnOrder turnOrder;
    public int currentmaxCoins;
    public static BattleController instance;
    public int cardsPerTurn = 1;
    public int startingCards = 3;
    
    private void Awake()
    { 
        instance = this;
    }

    public int startingCoins = 4, maxCoins = 12;
    public int playerCoins;

    // Start is called before the first frame update
    void Start()
    {
        //playerCoins = startingCoins;
        //UiController.instance.SetPlayerCoinsText(playerCoins);
        currentmaxCoins = startingCoins;
        DeckController.instance.AutoDraw(startingCards);
        FillMyMoneyzz();
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    public void SpendPlayerCoins(int amountToSpend)
    {
        playerCoins = playerCoins - amountToSpend;
        if(playerCoins < 0)
        {
            playerCoins = 0;
        }
        UiController.instance.SetPlayerCoinsText(playerCoins);
    }

    public void NextTurn()
    {
        turnOrder++;

        if ((int)turnOrder >= System.Enum.GetValues(typeof(TurnOrder)).Length)
        {
            turnOrder = 0;
        }
        switch(turnOrder)
        {
            case TurnOrder.playerTurn:
                Debug.Log("PlayerTurn");
                UiController.instance.makeCalc.SetActive(false);
                UiController.instance.playerTurn.SetActive(true);
                UiController.instance.endTurnButton.SetActive(true);
                UiController.instance.drawCardButton.SetActive(true);
                if(currentmaxCoins < maxCoins)
                {
                    currentmaxCoins++;
                }
                FillMyMoneyzz();
                DeckController.instance.AutoDraw(cardsPerTurn);
                break;

                case TurnOrder.enemyTurn:
                UiController.instance.playerTurn.SetActive(false);
                UiController.instance.drawCardButton.SetActive(false);
                UiController.instance.enemyTurn.SetActive(true);
                Debug.Log("Enemy makin moves");
                //NextTurn();
                break;

                case TurnOrder.calculateRound:
                UiController.instance.enemyTurn.SetActive(false);
                UiController.instance.makeCalc.SetActive(true);
                Debug.Log("Making calculations");
                //NextTurn();

                break;
        }
    }
    public void EndPlayerTurn()
    {
        UiController.instance.endTurnButton.SetActive(false);
        UiController.instance.drawCardButton.SetActive(false) ;
        NextTurn();
    }

    public void FillMyMoneyzz()
    {
        playerCoins = currentmaxCoins;
        UiController.instance.SetPlayerCoinsText(playerCoins);
    }

    public void ToNextPhase()
    {
        NextTurn();
    }   
    
}
