using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    public int cardID;
    public Sprite[] icons;
    public GameObject selectedIcon;
    public Vector2Int cardPosition;
    public FourX_Grid grid;
    private bool selected;

    public void initializeCard(int id,Vector2Int pos)
    {
        cardID = id;
        transform.GetComponent<Image>().sprite = icons[id];
        cardPosition = pos;
    }

    public void press()
    {
        selected = true;

        if (selected)
        {
            selectedIcon.SetActive(true);
        }

        grid.connect(this);
        
    }

    public void unSelect()
    {
        selectedIcon.SetActive(false);
        selected = false;
    }

}
