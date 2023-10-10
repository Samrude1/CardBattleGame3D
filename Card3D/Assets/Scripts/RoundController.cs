using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RoundController : MonoBehaviour
{
    public int playerAttackPoints;
    public int playerDefensePoints;
    public int opponentAttackPoints;
    public int opponentDefensePoints;
    public int playerHealth = 100; 
    public int opponentHealth = 100;
    public TMP_Text playerHealthText;
    public TMP_Text opponentHealthText;

    private void Start()
    {
        
    }

    void Update()
    {
        // Update the TMP Text objects with the current health points
        playerHealthText.text = "HP: " + playerHealth.ToString();
        opponentHealthText.text = "HP: " + opponentHealth.ToString();

        // ...
    }
    public void AddPlayerPoints(int attackPoints, int defensePoints)
    {
        playerAttackPoints += attackPoints;
        playerDefensePoints += defensePoints;
        Debug.Log("Tryin to add points"); // Add a debug log here to check if the method is called
    }

    public void AddEnemyPoints(int enemyAttackPoints, int enemyDefensePoints)
    {
        opponentAttackPoints += enemyAttackPoints;
        opponentDefensePoints += enemyDefensePoints;
        Debug.Log("Tryin to add points to Opponent"); // Add a debug log here to check if the method is called
    }

    // Function to calculate the result of the round and apply damage
    public void CalculateRoundResult()
    {
        // Calculate the result of the round based on attack and defense points
        int playerRoundResult = playerAttackPoints - opponentDefensePoints;
        int opponentRoundResult = opponentAttackPoints - playerDefensePoints;

        // Apply damage to player and opponent health based on the round result
        // You can add more logic here to handle specific cases
        if (playerRoundResult > 0)
        {
            // Player wins the round, apply damage to the opponent
            opponentHealth -= playerRoundResult;
        }
        else if (opponentRoundResult > 0)
        {
            // Opponent wins the round, apply damage to the player
            playerHealth -= opponentRoundResult;
        }

        // Reset accumulated points for the next round
        ResetRoundPoints();
    }

    public void ResetRoundPoints()
    {
        playerAttackPoints = 0;
        playerDefensePoints = 0;
        opponentAttackPoints = 0;
        opponentDefensePoints = 0;
    }

}
