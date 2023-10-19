using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleController : MonoBehaviour
{
    public enum TurnOrder { playerTurn, enemyTurn, calculateRound}
    public TurnOrder turnOrder;
    public int currentmaxCoins, currentEnemyMaxcoins;
    public static BattleController instance;
    public int cardsPerTurn = 1;
    public int startingCards = 3;
    public RoundController roundController;
    public GameObject enemyDraws;
    private void Awake()
    { 
        instance = this;
    }

    public int startingCoins = 1, maxCoins = 12;
    public int playerCoins, enemyCoins;

    // Start is called before the first frame update
    void Start()
    {
        roundController = FindObjectOfType<RoundController>();
        playerCoins = startingCoins;
        UiController.instance.SetPlayerCoinsText(playerCoins);
        enemyCoins = startingCoins;
        DeckController.instance.AutoDraw(startingCards);
        currentmaxCoins = startingCoins;
        currentEnemyMaxcoins = startingCoins;
        UiController.instance.SetEnemyCoinsText(enemyCoins);
          
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    public void SpendPlayerCoins(int amountToSpend)
    {
        playerCoins -= amountToSpend;
        if(playerCoins < 0)
        {
            playerCoins = 0;
        }
        UiController.instance.SetPlayerCoinsText(playerCoins);
    }

    public void SpendEnemyCoins(int amountToSpend)
    {
        enemyCoins -= amountToSpend;
        if (enemyCoins < 0)
        {
            enemyCoins = 0;
        }
        UiController.instance.SetEnemyCoinsText(enemyCoins);
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

                UiController.instance.coinsWarn.SetActive(false);
                UiController.instance.playerTurn.SetActive(false);
                UiController.instance.drawCardButton.SetActive(false);
                UiController.instance.enemyTurn.SetActive(true);
                Debug.Log("Enemy making moves");
                //NextTurn();
                EnemyController.instance.EnemyDrawCard();
                EnemyController.instance.StartAction();

                if (currentEnemyMaxcoins < maxCoins)
                {
                    currentEnemyMaxcoins++;
                }
                //FillEnemyMoneyzz();

                break;

            case TurnOrder.calculateRound:

                enemyDraws.SetActive(false);
                UiController.instance.enemyTurn.SetActive(false);
                UiController.instance.makeCalc.SetActive(true);
                Debug.Log("Making calculations");
                //NextTurn();

                //FillMyMoneyzz();
                FillEnemyMoneyzz();
                roundController.AddRound();
                roundController.CalculateRoundResult();
                //DeckController.instance.drawCost++;
                roundController.EndGame();
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
        enemyCoins = currentEnemyMaxcoins;
        UiController.instance.SetPlayerCoinsText(playerCoins);
        UiController.instance.SetEnemyCoinsText(enemyCoins);
    }

    public void FillEnemyMoneyzz()
    {
        enemyCoins = currentEnemyMaxcoins;
        UiController.instance.SetEnemyCoinsText(enemyCoins);
    }

    public void ToNextPhase()
    {
        NextTurn();
    }   
    
}
