using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    public TMP_Text currentRoundText;
    public int currentRound = 1;
    public int maxRounds = 7; 
    public GameObject winText;
    public GameObject loseText;
    public GameObject tieText;
    public GameObject resetButton;
    public GameObject canvas2;

    private void Start()
    {
        
    }

    void Update()
    {
        // Update the TMP Text objects with the current health points
        playerHealthText.text = "HP: " + playerHealth.ToString();
        opponentHealthText.text = "HP: " + opponentHealth.ToString();
        currentRoundText.text = "Round: " + currentRound.ToString();
    }
    public void AddPlayerPoints(int attackPoints, int defensePoints)
    {
        playerAttackPoints += attackPoints;
        playerDefensePoints += defensePoints;
        //Debug.Log("Tryin to add points"); // Add a debug log here to check if the method is called
    }

    public void AddEnemyPoints(int enemyAttackPoints, int enemyDefensePoints)
    {
        opponentAttackPoints += enemyAttackPoints;
        opponentDefensePoints += enemyDefensePoints;
        //Debug.Log("Tryin to add points for Opponent"); // Add a debug log here to check if the method is called
    }

    // Function to calculate the result of the round and apply damage
    public void CalculateRoundResult()
    {
        // Calculate the result of the round based on attack and defense points
        int playerRoundResult = playerAttackPoints - opponentDefensePoints;
        Debug.Log("player points: " + playerRoundResult);
        int opponentRoundResult = opponentAttackPoints - playerDefensePoints;
        Debug.Log("enemy points: " + opponentRoundResult);

        // Apply damage to player and opponent health based on the round result
        // You can add more logic here to handle specific cases
        if (playerRoundResult > 0)
        {
            // Player wins the round, apply damage to the opponent
            opponentHealth -= playerRoundResult;
        }
        if (opponentRoundResult > 0)
        {
            // Opponent wins the round, apply damage to the player
            playerHealth -= opponentRoundResult;
        }

        // Reset accumulated points for the next round
        ResetRoundPoints();
    }
    public void EndGame()
    {
        if (currentRound >= 8)
        {

            if (playerHealth > opponentHealth)
            {
                winText.SetActive(true);
                resetButton.SetActive(true);
                canvas2.SetActive(false);
                Debug.Log("Player wins!");
                // Handle player win
            }
            else if (opponentHealth > playerHealth)
            {
                loseText.SetActive(true);
                resetButton.SetActive(true);
                canvas2.SetActive(false);
                Debug.Log("Opponent wins!");
                // Handle opponent win
            }
            else
            {
                tieText.SetActive(true);
                resetButton.SetActive(true);
                canvas2.SetActive(false);
                Debug.Log("It's a tie!");
                // Handle tie
            }
        }
    }

    public void ResetRoundPoints()
    {
        playerAttackPoints = 0;
        playerDefensePoints = 0;
        opponentAttackPoints = 0;
        opponentDefensePoints = 0;
    }

    public void AddRound()
    {
        currentRound++;
    }

    public void Reset()
    {
        currentRound = 0;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

}
