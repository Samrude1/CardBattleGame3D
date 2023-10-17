using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    public CardScriptableObject cardSO;
    public int defence, attack, coins;
    public TMP_Text coinsText, nameText, actionText, loreText;
    public Image cardArt, cardTypeImage;
    public float moveSpeed, rotateSpeed;
    public bool inHand;
    public int handPosition;
    public LayerMask whatIsDesktop, whatIsPlacement;
    public CardPlacePoint assignedPlace;
    public bool isHovered = false;
    public RoundController roundController;

    private Vector3 targetPoint; // tämä on siis 0,0,0
    private Quaternion targetRot;
    private HandController handController;
    public bool isSelected;
    public Collider theCol;
    private bool justPressed;
    private float zoomSpeed = 1f;

    // Start is called before the first frame update
    void Start()
    {
        roundController = FindObjectOfType<RoundController>();

        if (targetPoint == Vector3.zero)
        {
            targetPoint = transform.position;
            targetRot = transform.rotation;
        }
        SetupCard();
        handController = FindObjectOfType<HandController>();
        theCol = GetComponent<Collider>();
    }

    public void SetupCard()
    {
        if (cardSO != null)
        {
            defence = cardSO.defence;
            attack = cardSO.attack;
            coins = cardSO.coins;

            //healthText.text = health.ToString();
            //attackText.text = attack.ToString();
            coinsText.text = coins.ToString();

            nameText.text = cardSO.cardName;
            actionText.text = cardSO.math;
            loreText.text = cardSO.lore;

            cardArt.sprite = cardSO.cardImage;
            cardTypeImage.sprite = cardSO.cardTypeImage;
        }
        else
        {
            // Handle the case where cardSO is null, possibly log an error.
            Debug.LogError("cardSO is null in SetupCard");
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, targetPoint, moveSpeed * Time.deltaTime); // Target point on tässä 0,0,0
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, rotateSpeed * Time.deltaTime);

        if (isSelected)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100f, whatIsDesktop))
            {
                MoveToPoint(hit.point + new Vector3(0f, 2f, 0f), Quaternion.identity);
            }
            if (Input.GetMouseButtonDown(1))
            {
                ReturnToHand();
            }
            if (Input.GetMouseButtonDown(0) && justPressed == false)
            {
                if (Physics.Raycast(ray, out hit, 100f, whatIsPlacement) && BattleController.instance.turnOrder == BattleController.TurnOrder.playerTurn)
                {
                    CardPlacePoint selectedPoint = hit.collider.GetComponent<CardPlacePoint>();

                    if (selectedPoint.activeCard == null && selectedPoint.isPlayerPoint)
                    {
                        if (BattleController.instance.playerCoins >= coins)
                        {
                            selectedPoint.activeCard = this;
                            assignedPlace = selectedPoint;

                            MoveToPoint(selectedPoint.transform.position, Quaternion.identity);
                            inHand = false;
                            isSelected = false;

                            handController.RemoveCardFromHand(this);
                            BattleController.instance.SpendPlayerCoins(coins);
                            //Debug.Log("card played"); //This is when we put some points to pool
                            roundController.AddPlayerPoints(cardSO.attack, cardSO.defence);
                        }
                        else
                        {
                            ReturnToHand();
                            UiController.instance.ShowCoinsWarn();
                        }
                    }
                    else
                    {
                        ReturnToHand();
                    }
                }
                else
                {
                    ReturnToHand();
                }
            }
        }
        justPressed = false;
        ZoomCard();
    }

    public void MoveToPoint(Vector3 pointToMoveTo, Quaternion rotToMatch)
    {
        targetPoint = pointToMoveTo;
        targetRot = rotToMatch;
    }

    private void OnMouseOver()
    {
        //Debug.Log("Hovering...");
        if (inHand)
        {
            isHovered = true;
            MoveToPoint(handController.cardPositions[handPosition] + new Vector3(0f, 1f, .5f), Quaternion.identity);
        }
    }
    private void OnMouseExit()
    {
        if (inHand)
        {
            MoveToPoint(handController.cardPositions[handPosition], handController.minPos.rotation);
            isHovered = false;
        }
    }
    private void OnMouseDown()
    {
        if (inHand && BattleController.instance.turnOrder == BattleController.TurnOrder.playerTurn)
        {
            //Debug.Log("Pressed");
            isSelected = true;
            theCol.enabled = false;
            justPressed = true;
        }
    }
    public void ReturnToHand()
    {
        isSelected = false;
        theCol.enabled = true;

        MoveToPoint(handController.cardPositions[handPosition], handController.minPos.rotation);
    }

    void ZoomCard()
    {
        if (Input.GetMouseButton(1) && isHovered)
        {
            var targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = Vector3.Lerp(transform.position, targetPos, zoomSpeed * Time.deltaTime);
            //transform.position = new Vector3(3f, 2f, -3f); // just up, no towards camera
            transform.localScale = new Vector3(2f, 2f, 2f);
        }
        else
        {
            transform.localScale = new Vector3(1f, 1f, 1f);

        }
    }
    
}
