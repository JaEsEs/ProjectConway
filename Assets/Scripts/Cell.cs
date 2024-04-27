using UnityEngine;
using UnityEngine.UI;

public class Cell : MonoBehaviour
{
    public bool isAlive;    // Variable to store the cell's life state

    private Image img;

    void Start()
    {
        img = GetComponent<Image>();
        isAlive = false;
        UpdateVisualState();
    }

    // Function to toggle the cell's life state
    public void ToggleIsAlive()
    {
        isAlive = !isAlive;
        UpdateVisualState();
    }

    // Function to update the cell's visual state
    public void UpdateVisualState()
    {
        img.color = isAlive ? Color.white : Color.black;
    }
}
