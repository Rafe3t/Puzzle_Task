using System;
using Unity.Mathematics;
using Unity.Mathematics.Geometry;
using UnityEditor;
using UnityEngine;

public class FourX_Grid : MonoBehaviour
{
    public GameObject CardPrefab;
    public bool[] occupied;
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
                //occupied[j][i] = true;
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

            }
            else
            {

            }
        }
    }

    public bool checkVectors(Card card1,Card card2)
    {
        Vector2 rightDr = new Vector2(card1.cardPosition.x + 1, card1.cardPosition.y);
        return true;
    }
}
