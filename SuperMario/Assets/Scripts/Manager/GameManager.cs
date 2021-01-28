using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    Player player;

    //UI
    public Slider healthBar;
    public Text points;

    void Start()
    {
        player = FindObjectOfType<Player>();
        healthBar.maxValue = player.maxPlayerHealth;  
    }

    void Update()
    {
        points.text = "POINT " + player.currentPoints.ToString();

        if (player.isDead)
        {
            Invoke("RestartGame", 2);
        }

        UpdateUI();
    }

    public void RestartGame()
    {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

    void UpdateUI()
    {
        healthBar.value = player.currentPlayerHealth;

        if (player.currentPlayerHealth <= 0)
            healthBar.minValue = 0;
    }
}
