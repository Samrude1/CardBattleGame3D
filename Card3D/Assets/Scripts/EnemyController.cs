using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public static EnemyController instance;

    private void Awake()
    {
        instance = this;
    }

    public List<CardScriptableObject> deckToUse = new List<CardScriptableObject>();
    public List<CardScriptableObject> activeCards = new List<CardScriptableObject>();
    public Card cardToSpawn;
    public Transform cardSpawnPoint;
    public CardPlacePoint[] enemyCardPoints;
    public float enemyTimer = 0.5f;
    public int enemyAttackPoints = 0;
    public int enemyDefencePoints = 0;
    public RoundController roundController;

    public enum AIType { fromDeck, handRandom, handDefence, handAttack}
    public AIType enemyType;

    public List<CardScriptableObject> cardsInHand = new List<CardScriptableObject>();
    public int startHandSize;

    // Start is called before the first frame update
    void Start()
    {

        roundController = FindObjectOfType<RoundController>();
        SetupDeck();
        

        if(enemyType != AIType.fromDeck)
        {
            SetupHand();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetupDeck()
    {
        activeCards.Clear();

        List<CardScriptableObject> tempDeck = new List<CardScriptableObject>();
        tempDeck.AddRange(deckToUse);

        int iterations = 0;

        while (tempDeck.Count > 0 && iterations < 500)
        {
            int selected = Random.Range(0, tempDeck.Count);
            activeCards.Add(tempDeck[selected]);
            tempDeck.RemoveAt(selected);
            iterations++;
        }
    }

    public void StartAction()
    {
        StartCoroutine(EnemyActionCo());
    }

    IEnumerator EnemyActionCo()
    {
        if (activeCards.Count == 0)
        {
            SetupDeck();
        }
        yield return new WaitForSeconds(.5f);

        if (enemyType != AIType.fromDeck)
        {
            for (int i = 0; i < BattleController.instance.cardsPerTurn; i++)
            {
                cardsInHand.Add(activeCards[0]);
                activeCards.RemoveAt(0);

                if (activeCards.Count == 0)
                {
                    SetupDeck();
                }
            }
        }

        List<CardPlacePoint> cardPoints = new List<CardPlacePoint>();
        cardPoints.AddRange(enemyCardPoints);

        int randomPoint = Random.Range(0, cardPoints.Count);
        CardPlacePoint selectedPoint = cardPoints[randomPoint];

        if (enemyType == AIType.fromDeck || enemyType == AIType.handRandom)
        {
            cardPoints.Remove(selectedPoint);

            while (selectedPoint.activeCard != null && cardPoints.Count > 0)
            {
                randomPoint = Random.Range(0, cardPoints.Count);
                selectedPoint = cardPoints[randomPoint];
                cardPoints.RemoveAt(randomPoint);
            }
        }

        CardScriptableObject selectedCard = null;
        int iterations = 0;
        
        switch (enemyType)
        {
            case AIType.fromDeck:
                if (selectedPoint.activeCard == null)
                {
                    Card newCard = Instantiate(cardToSpawn, cardSpawnPoint.position, cardSpawnPoint.rotation);
                    newCard.cardSO = activeCards[0];
                    activeCards.RemoveAt(0);
                    newCard.SetupCard();
                    newCard.MoveToPoint(selectedPoint.transform.position, selectedPoint.transform.rotation);
                    selectedPoint.activeCard = newCard;
                    newCard.assignedPlace = selectedPoint;
                }
                break;

            case AIType.handRandom:
                selectedCard = SelectedCardToPlay();

                iterations = 50;

                while (selectedCard != null && iterations > 0 && cardPoints.Count > 0)
                {
                    selectedPoint = null; // Reset selectedPoint at the beginning of the loop

                    if (selectedCard.isAttack)
                    {
                        // Choose a random attack slot
                        selectedPoint = cardPoints.Find(p => p.isAttack && p.activeCard == null);
                    }
                    else if (selectedCard.isDefense)
                    {
                        // Choose a random defense slot
                        selectedPoint = cardPoints.Find(p => p.isDefense && p.activeCard == null);
                    }

                    if (selectedPoint != null)
                    {
                        PlayCard(selectedCard, selectedPoint);
                        selectedCard = SelectedCardToPlay();
                    }

                    iterations--;
                    yield return new WaitForSeconds(2);

                    // Reset cardPoints for the next iteration
                    cardPoints.Clear();
                    cardPoints.AddRange(enemyCardPoints);

                   
                }
                break;

            case AIType.handDefence:
                selectedCard = SelectedDefenceCardToPlay();
                iterations = 50;
                while (selectedCard != null && iterations > 0 && selectedPoint.activeCard == null)
                {
                    PlayCard(selectedCard, selectedPoint);
                    selectedCard = SelectedDefenceCardToPlay();
                    iterations--;

                    yield return new WaitForSeconds(2);
                    while (selectedPoint.activeCard != null && cardPoints.Count > 0)
                    {
                        randomPoint = Random.Range(0, cardPoints.Count);
                        selectedPoint = cardPoints[randomPoint];
                        cardPoints.RemoveAt(randomPoint);
                    }
                }
                break;

            case AIType.handAttack:
                selectedCard = SelectedAttackCardToPlay();
                iterations = 50;
                while (selectedCard != null && iterations > 0 && selectedPoint.activeCard == null)
                {
                    PlayCard(selectedCard, selectedPoint);
                    selectedCard = SelectedAttackCardToPlay();
                    iterations--;

                    yield return new WaitForSeconds(2);
                    while (selectedPoint.activeCard != null && cardPoints.Count > 0)
                    {
                        randomPoint = Random.Range(0, cardPoints.Count);
                        selectedPoint = cardPoints[randomPoint];
                        cardPoints.RemoveAt(randomPoint);
                    }
                }
                break;
        }

        // If the opponent can't play any more cards, go to the next turn
        if (selectedCard == null)
        {
            BattleController.instance.ToNextPhase();
        }
    }



    void SetupHand()
    {
        for(int i = 0; i < startHandSize; i++)
        {
            if(activeCards.Count == 0)
            {
                SetupDeck();
            }
            cardsInHand.Add(activeCards[0]);
            activeCards.RemoveAt(0);
        }
    }

    public void PlayCard(CardScriptableObject cardSO, CardPlacePoint cardPlacePoint)
    {
        // Check if there are any active cards in the list
        if (activeCards.Count > 0)
        {
            //Debug.Log("Active cards are here");
            Card newCard = Instantiate(cardToSpawn, cardSpawnPoint.position, cardSpawnPoint.rotation);
            newCard.cardSO = cardSO;

            // Remove the first card (index 0) from the activeCards list
            //activeCards.RemoveAt(0);

            newCard.SetupCard();
            newCard.MoveToPoint(cardPlacePoint.transform.position, cardPlacePoint.transform.rotation);
            cardPlacePoint.activeCard = newCard;
            newCard.assignedPlace = cardPlacePoint;

            cardsInHand.Remove(cardSO);

            BattleController.instance.SpendEnemyCoins(cardSO.coins);
            //Debug.Log("Enemy plays"); //This is where we put some points in pool
            roundController.AddEnemyPoints(cardSO.attack, cardSO.defence);
        }
        else
        {
            // Handle the case when there are no active cards to play
            Debug.LogWarning("No active cards to play.");
            //BattleController.instance.ToNextPhase();
        }
    }

     CardScriptableObject SelectedCardToPlay()
    {
        CardScriptableObject cardToPlay = null;

        List<CardScriptableObject> cardsToPlay = new List<CardScriptableObject>();
        foreach(CardScriptableObject card in cardsInHand)
        {
            if(card.coins <= BattleController.instance.enemyCoins)
            {
                cardsToPlay.Add(card);
            }
        }

        /* Debug.Log to print the contents of the cardsToPlay list
        Debug.Log("Cards in cardsToPlay:");
        foreach (CardScriptableObject card in cardsToPlay)
        {
            Debug.Log("Card Name: " + card.cardName + ", Coins: " + card.coins);
        }
        */

        if (cardsToPlay.Count > 0)
        {
            int selected = Random.Range(0, cardsToPlay.Count);

            cardToPlay = cardsToPlay[selected];
        }

        return cardToPlay;
    }

    CardScriptableObject SelectedAttackCardToPlay()
    {
        CardScriptableObject cardToPlay = null;

        List<CardScriptableObject> attackCards = new List<CardScriptableObject>();

        // Iterate through the opponent's hand and select attack cards
        foreach (CardScriptableObject card in cardsInHand)
        {
            if (card.isAttack) // Check if the card is an attack card
            {
                attackCards.Add(card);
            }
        }

        // If there are attack cards in the hand, choose one based on some criteria
        if (attackCards.Count > 0)
        {
            // Here, you can implement your logic to choose the best attack card.
            // For example, you can choose the one with the highest attack value.
            // You can also add more sophisticated criteria if needed.
            int highestAttackValue = -1; // Initialize with a low value
            foreach (CardScriptableObject attackCard in attackCards)
            {
                if (attackCard.attack > highestAttackValue)
                {
                    highestAttackValue = attackCard.attack;
                    cardToPlay = attackCard;
                }
            }
        }

        return cardToPlay;
    }

    CardScriptableObject SelectedDefenceCardToPlay()
    {
        CardScriptableObject cardToPlay = null;

        List<CardScriptableObject> defenceCards = new List<CardScriptableObject>();

        // Iterate through the opponent's hand and select defend cards
        foreach (CardScriptableObject card in cardsInHand)
        {
            if (card.isDefense) // Check if the card is a defense card
            {
                defenceCards.Add(card);
            }
        }

        // If there are defense cards in the hand, choose one based on some criteria
        if (defenceCards.Count > 0)
        {
            // Here, you can implement your logic to choose the best defense card.
            // For example, you can choose the one with the highest defense value.
            // You can also add more sophisticated criteria if needed.
            int highestDefenceValue = 0; // Initialize with a low value
            foreach (CardScriptableObject defenceCard in defenceCards)
            {
                if (defenceCard.defence > highestDefenceValue)
                {
                    highestDefenceValue = defenceCard.defence;
                    cardToPlay = defenceCard;
                }
            }
        }

        return cardToPlay;
    }

}
