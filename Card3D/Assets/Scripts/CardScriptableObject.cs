using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Card", menuName = "Card", order = 1)]
public class CardScriptableObject : ScriptableObject
{
    public string cardName;

    [TextArea]
    public string math, lore;


    public int attack, defence, coins;
    public Sprite cardImage, cardTypeImage;
    public bool isDefense;
    public bool isAttack;
}
