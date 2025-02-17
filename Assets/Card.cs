using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    public int cardID;
    public Sprite[] icons;
    public Vector2 cardPosition;
    public FourX_Grid grid;

    public void initializeCard(int id,Vector2 pos)
    {
        cardID = id;
        transform.GetComponent<Image>().sprite = icons[id];
        cardPosition = pos;
    }

    public void press()
    {
        grid.connect(this);
    }

}
