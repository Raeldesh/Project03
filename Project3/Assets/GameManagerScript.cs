using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManagerScript : MonoBehaviour
{
    [SerializeField] GameObject cell;

    [SerializeField] int rows = 16;
    [SerializeField] int columns = 16;
    [SerializeField] float xStart = -8f;
    [SerializeField] float yStart = 7f;
    [SerializeField] float jump = 1.1f;
    [SerializeField] int numberOfMines = 40;

    public bool minesDistributed;
    public int minesRemaining;
    private int cellsLeftToReveal;
    public bool gameOver;

    public List<List<GameObject>> listOfRows;

    public Text minesLeftText;

    void Start()
    {
        listOfRows = new List<List<GameObject>>();

        SpawnGridCells();

        minesDistributed = false;
        minesRemaining = numberOfMines;
        cellsLeftToReveal = (rows * columns) - numberOfMines;

        minesLeftText.text = "Mines left: " + minesRemaining;

        gameOver = false;
    }

    public void SpawnGridCells()
    {
        float currentY = yStart;

        for (int r = 0; r < rows; r ++)
        {
            float currentX = xStart;
            listOfRows.Add (new List<GameObject>());

            for (int c = 0; c < columns; c ++)
            {
                var newCell = Instantiate(cell, new Vector3(currentX, currentY, 0), Quaternion.identity);
                listOfRows[r].Add(newCell);
                currentX += jump;
            }
            currentY -= jump;
        }
    }

    public void DistributeMines()
    {
        for (int i = 0; i < numberOfMines; i++)
        {
            bool cellChosen = false;
            while (!cellChosen)
            {
                int r = Random.Range(0, rows-1);
                int c = Random.Range(0, columns-1);

                CellScript targetCell = listOfRows[r][c].GetComponent<CellScript>();
                if (targetCell.hasMine == false && targetCell.firstCell == false)
                {
                    targetCell.hasMine = true;
                    cellChosen = true;
                }
            }
        }
        minesDistributed = true;
    }

    public void ReactToLeftClick()
    {
        for (int r = 0; r < rows; r ++)
        {
            for (int c = 0; c < columns; c ++)
            {
                CellScript currentCell = listOfRows[r][c].GetComponent<CellScript>();
                if (currentCell.clicked == true)
                {
                    if (currentCell.hasMine)
                    {
                        currentCell.RevealMine();
                        gameOver = true;
                        minesLeftText.text = "Oops! You lost.";
                    }
                    else
                    {
                        List<CellScript> neighbors = GetCellNeighbors(r, c);
                        int neighboringMines = 0;
                        foreach (CellScript neighbor in neighbors)
                        {
                            if (neighbor.hasMine)
                            {
                                neighboringMines ++;
                            }
                        }
                        currentCell.RevealNumberOfNearbyMines(neighboringMines);

                        currentCell.clicked = false;
                        currentCell.revealed = true;
                        cellsLeftToReveal -= 1;

                        if (neighboringMines == 0)
                        {
                            foreach (CellScript neighbor in neighbors)
                            {
                                if (neighbor.hasMine == false && !neighbor.flag.activeSelf)
                                {
                                    neighbor.HandleLeftClick();
                                }
                            }
                        }

                        if (cellsLeftToReveal == 0)
                        {
                            gameOver = true;
                            minesLeftText.text = "   You win!";
                        }
                    }
                }
            }
        }
    }

    public List<CellScript> GetCellNeighbors (int row, int column)
    {
        List<CellScript> neighborList = new List<CellScript>();
        for (int r = row - 1; r <= row + 1; r++)
        {
            for (int c = column - 1; c <= column + 1; c++)
            {
                if ((0 <= c && c < columns) && (0 <= r && r < rows))
                { 
                    CellScript currentCell = listOfRows[r][c].GetComponent<CellScript>();

                    if (!(r == row && c == column))
                    {
                        neighborList.Add(currentCell);
                    }
                }
            }
        }
        return (neighborList);
    }
}
