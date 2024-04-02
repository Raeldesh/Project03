using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CellScript : MonoBehaviour
{
    public bool hasMine;
    public bool revealed;
    public bool clicked;
    public bool firstCell;

    private bool clickLocked;

    GameManagerScript gameManager;
    [SerializeField] Text mineText;
    public GameObject flag;
    SpriteRenderer spRenderer;

    void OnAwake()
    {
        hasMine = false;
        revealed = false;
        clicked = false;
        firstCell = false;

    }

    void Start()
    {
        gameManager = FindObjectOfType<GameManagerScript>();
        spRenderer = GetComponent<SpriteRenderer>();

        clickLocked = false;
    }

    private void OnMouseOver() 
    {
        if (Input.GetMouseButtonDown(0) && !clickLocked && !gameManager.gameOver)
        {
            clickLocked = true;
            HandleLeftClick();
            if (!gameManager.gameOver)
            {
                gameManager.minesLeftText.text = "Mines left: " + gameManager.minesRemaining;
            }
        }
        else if (Input.GetMouseButtonDown(1) && !clickLocked && !gameManager.gameOver)
        {
            clickLocked = true;
            if (!revealed && flag.activeSelf == false)
            {
                flag.SetActive(true);
                gameManager.minesRemaining --;

            }
            else if (!revealed && flag.activeSelf == true)
            {
                flag.SetActive(false);
                gameManager.minesRemaining ++;
            }
            gameManager.minesLeftText.text = "Mines left: " + gameManager.minesRemaining;
        }
        else
        {
            clickLocked = false;
        }
    }

    public void HandleLeftClick()
    {
        if (!revealed)
        {
            clicked = true;
            spRenderer.color = new Color(0, 250, 0);
            if (gameManager.minesDistributed == false)
            {
                firstCell = true;
                gameManager.DistributeMines();
            }
            if (flag.activeSelf == true)
            {
                flag.SetActive(false);
                gameManager.minesRemaining ++;
            }
            gameManager.ReactToLeftClick();
        }
    }

    public void RevealMine()
    {
        transform.GetChild(0).gameObject.SetActive(true);
    }

    public void RevealNumberOfNearbyMines(int mines)
    {
        if (mines == 0)
        {
            mineText.text = " ";
        }
        else
        {
            mineText.text = mines.ToString();
        }
    }
}
