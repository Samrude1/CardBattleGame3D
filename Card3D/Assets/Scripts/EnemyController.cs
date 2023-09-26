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
    private List<CardScriptableObject> activeCards = new List<CardScriptableObject>();
    public Card cardToSpawn;
    public Transform cardSpawnPoint;
    public CardPlacePoint[] enemyCardPoints;

    public enum AIType { FromDeck, handRandom, handDefence, handAttack}
    public AIType enemyType;

    // Start is called before the first frame update
    void Start()
    {
        SetupDeck();
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

        List<CardPlacePoint> cardPoints = new List<CardPlacePoint>();
        cardPoints.AddRange(enemyCardPoints);

        int randomPoint = Random.Range(0, cardPoints.Count);
        CardPlacePoint selectedPoint = cardPoints[randomPoint];

        while (selectedPoint.activeCard != null && cardPoints.Count > 0)
        {
            randomPoint = Random.Range(0, cardPoints.Count);
            selectedPoint = cardPoints[randomPoint];
            cardPoints.RemoveAt(randomPoint);
        }

        switch (enemyType)
        {
            case AIType.FromDeck:

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

                break;

            case AIType.handDefence:

                break;
            
            case AIType.handAttack: 
                
                break;
        }   
    }


}
