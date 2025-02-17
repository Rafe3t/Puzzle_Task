using System;
using Unity.Mathematics;
using Unity.Mathematics.Geometry;
using UnityEditor;
using UnityEngine;

public class FourX_Grid : MonoBehaviour
{
    public GameObject CardPrefab;
    public bool[,] occupied = new bool[6,6];
    public Transform[] rows;
    public int[] cards;
    private int rndm,store;
    private bool selected;
    private Card cardSelect1, cardSelect2;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        generateNumbers();
        generateCards();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void generateCards()
    {
        for(int i=0;i<4;i++)
        {
            for(int j=0;j<4;j++)
            {
                GameObject card = Instantiate(CardPrefab, rows[i]);
                card.GetComponent<Card>().initializeCard(cards[i * 4 + j], new Vector2(j,i));
                card.GetComponent<Card>().grid = this;
                occupied[j+1,i+1] = true;
            }
        }
    }

    void generateNumbers()
    {
        for(int i=0;i<16;i+=2)
        {
            cards[i] = UnityEngine.Random.Range(0, 9);
            cards[i + 1] = cards[i];
            
        }

        for(int i=0;i<16;i++)
        {
            rndm = UnityEngine.Random.Range(0, 15);
            store = cards[i];
            cards[i] = cards[rndm];
            cards[rndm] = store;
        }
    }

    public void connect(Card card)
    {
        if(!selected)
        {
            selected = true;
            cardSelect1 = card;
        }
        else
        {
            selected = false;
            cardSelect2 = card;

            if(cardSelect1.cardID == cardSelect2.cardID)
            {
                bool check = startVectors(cardSelect1,cardSelect2);
                if(check)
                {
                    Debug.Log("correct");
                }
                else
                {
                    Debug.Log("false");
                }
            }
            else
            {
                Debug.Log("false");
            }
        }
    }

    public bool startVectors(Card card1,Card card2)
    {
        return checkVectors(card1.cardPosition,card1,card2);
    }

    public bool checkVectors(Vector2 currentNode, Card card1, Card card2)
    {
        Vector2 rightDr = new Vector2(currentNode.x + 1, currentNode.y);
        Vector2 leftDr = new Vector2(currentNode.x - 1, currentNode.y);
        Vector2 upDr = new Vector2(currentNode.x, currentNode.y + 1);
        Vector2 downDr = new Vector2(currentNode.x, currentNode.y - 1);
        bool right_check = false;
        bool left_check = false;
        bool up_check = false;
        bool down_check = false;

        if (rightDr.x + 1 < 6 && rightDr.y + 1 < 6)
        {
            if (occupied[(int)rightDr.x + 1, (int)rightDr.y + 1] == false)
            {
                right_check = checkVectors(rightDr,card1, card2);
            }
            else
            {
                if(card2.cardPosition == rightDr)
                {
                    //correct
                    occupied[(int)card1.cardPosition.x + 1, (int)card1.cardPosition.y + 1] = false;
                    occupied[(int)card2.cardPosition.x + 1, (int)card2.cardPosition.y + 1] = false;
                    Destroy(card1.gameObject);
                    Destroy(card2.gameObject);

                    return true;
                }

            }
        }
        //---------------//
        if (leftDr.x + 1 < 6 && leftDr.y + 1 < 6)
        {
            if (occupied[(int)leftDr.x + 1, (int)leftDr.y + 1] == false)
            {
                left_check = checkVectors(leftDr,card1, card2);
            }
            else
            {
                if (card2.cardPosition == leftDr)
                {
                    //correct
                    occupied[(int)card1.cardPosition.x + 1, (int)card1.cardPosition.y + 1] = false;
                    occupied[(int)card2.cardPosition.x + 1, (int)card2.cardPosition.y + 1] = false;
                    Destroy(card1.gameObject);
                    Destroy(card2.gameObject);

                    return true;
                }

            }
        }
        //---------------//
        if (upDr.x + 1 < 6 && upDr.y + 1 < 6)
        {
            if (occupied[(int)upDr.x + 1, (int)upDr.y + 1] == false)
            {
                up_check = checkVectors(upDr,card1, card2);
            }
            else
            {
                if (card2.cardPosition == upDr)
                {
                    //correct
                    occupied[(int)card1.cardPosition.x + 1, (int)card1.cardPosition.y + 1] = false;
                    occupied[(int)card2.cardPosition.x + 1, (int)card2.cardPosition.y + 1] = false;
                    Destroy(card1.gameObject);
                    Destroy(card2.gameObject);

                    return true;
                }

            }
        }
        //---------------//
        if (downDr.x + 1 < 6 && downDr.y + 1 < 6)
        {
            if (occupied[(int)downDr.x + 1, (int)downDr.y + 1] == false)
            {
                down_check = checkVectors(downDr,card1 ,card2);
            }
            else
            {
                if (card2.cardPosition == downDr)
                {
                    //correct
                    occupied[(int)card1.cardPosition.x + 1, (int)card1.cardPosition.y + 1] = false;
                    occupied[(int)card2.cardPosition.x + 1, (int)card2.cardPosition.y + 1] = false;
                    Destroy(card1.gameObject);
                    Destroy(card2.gameObject);

                    return true;
                }

            }
        }

        if(right_check&left_check&up_check&down_check)
        {
            return true;
        }
        return false;
    }
}
