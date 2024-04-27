using System;
using System.Collections;
using UnityEngine;

public class Board : MonoBehaviour
{
    public GameObject cellPrefab;
    public bool paused;

    private const int CELLSIZE = 50;

    private Vector2Int boardSize;
    private GameObject[,] board;

    public float waitForNextUpdate = 0.25f;
    private float timer;


    private void Start()
    {
        boardSize.x = Screen.width / CELLSIZE; // Calculate the number of cells on the X-axis
        boardSize.y = Screen.height / CELLSIZE;// Calculate the number of cells on the Y-axis
        CreateBoard();
        paused = true;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            paused = !paused;
            timer = 0;
        }

        if (!paused)
        {
            timer += Time.deltaTime;
            if (timer >= waitForNextUpdate)
            {
                UpdateBoard();
                timer = 0;
            }
        }
    }

    private void CreateBoard()
    {
        board = new GameObject[boardSize.x, boardSize.y]; // Initialize the board array

        for (int x = 0; x < boardSize.x; x++)
        {
            for (int y = 0; y < boardSize.y; y++)
            {
                // Calculate the position of the square
                Vector2 position = new Vector2((x * CELLSIZE) - (Screen.width / 2), (y * CELLSIZE) - (Screen.height / 2));
                // Instantiate a cell at the calculated position
                GameObject cell = Instantiate(cellPrefab, position, Quaternion.identity);
                // Set the parent of the cell to the current transform
                cell.transform.SetParent(transform);
                // Add the cell to the list
                board[x, y] = cell;
            }
        }

        // Calculate the average position of all cells
        Vector2 averagePosition = Vector2.zero;
        foreach (GameObject cell in board)
        {
            averagePosition += (Vector2)cell.transform.position;
        }
        averagePosition /= board.Length;

        // Calculate the displacement needed to center the board
        Vector2 displacement = (Vector2)transform.position - averagePosition;

        // Move each square by the displacement amount to center the board
        foreach (GameObject cell in board)
        {
            cell.transform.position += (Vector3)displacement;
        }
    }

    private void UpdateBoard()
    {
        // Create a temporary board to store the next state
        bool[,] newBoard = new bool[boardSize.x, boardSize.y];

        // Iterate through each cell in the board
        for (int x = 0; x < boardSize.x; x++)
        {
            for (int y = 0; y < boardSize.y; y++)
            {
                // Get the current state of the cell
                bool isAlive = board[x, y].GetComponent<Cell>().isAlive;
                // Calculate the number of alive neighbors
                int aliveNeighbours = CheckStateOfNeighbour(x, y);
                // Apply Conway's rules to determine the next state of the cell
                newBoard[x, y] = (isAlive && aliveNeighbours == 2) || aliveNeighbours == 3;
            }
        }

        // Update the state of each cell in the board
        for (int x = 0; x < boardSize.x; x++)
        {
            for (int y = 0; y < boardSize.y; y++)
            {
                board[x, y].GetComponent<Cell>().isAlive = newBoard[x, y];
                board[x, y].GetComponent<Cell>().UpdateVisualState();
            }
        }
    }

    int CheckStateOfNeighbour(int row, int col)
    {
        int count = 0;
        for (int i = row - 1; i <= row + 1; i++)
        {
            for (int j = col - 1; j <= col + 1; j++)
            {
                // Skip the current cell
                if (i == row && j == col)
                    continue;

                // Check if the neighboring cell is within the bounds of the board
                if (IsInBoard(i, j))
                {
                    // Check if the neighboring cell is alive
                    if (board[i, j].GetComponent<Cell>().isAlive)
                        count++;
                }
            }
        }

        return count;
    }

    bool IsInBoard(int row, int col)
    {
        return row >= 0 && row < boardSize.x && col >= 0 && col < boardSize.y;
    }
}