using System;
using Unity.Mathematics;
using Unity.Mathematics.Geometry;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using Unity.VisualScripting;
using NUnit;
using System.Collections;
using static UnityEngine.Rendering.GPUSort;
using System.IO;

public class FourX_Grid : MonoBehaviour
{
    private Card[,] cardsOnGrid;
    private bool[,] occupied;
    private int[] cards;
    private int rndm,store;
    private bool selected;
    private Card cardSelect1, cardSelect2;
    public int gridSize = 6;

    public GameManager manager;

    public Transform x4points, x6points, x7points, x8points;
    private Transform[,] x4pathpoints = new Transform[6, 6];
    private Transform[,] x6pathpoints = new Transform[8, 8];
    private Transform[,] x7pathpoints = new Transform[9, 9];
    private Transform[,] x8pathpoints = new Transform[10, 10];

    private readonly Vector2Int[] directions = {
        new Vector2Int(0,0),   // NoDirection
        new Vector2Int(1, 0),  // Right
        new Vector2Int(-1, 0), // Left
        new Vector2Int(0, 1),  // Up
        new Vector2Int(0, -1)  // Down
    };

    struct Node
    {
        public Vector2Int position;
        public Vector2Int direction;
        public int turnCount;

        public Node(Vector2Int pos, Vector2Int dir, int turns)
        {
            position = pos;
            direction = dir;
            turnCount = turns;
        }
    }

    private void Start()
    {
        for(int i=0;i<6;i++)
        {
            for(int j=0;j<6;j++)
            {
                x4pathpoints[j, i] = x4points.GetChild(i).GetChild(j);
            }
        }

        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                x6pathpoints[j, i] = x6points.GetChild(i).GetChild(j);
            }
        }

        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                x7pathpoints[j, i] = x7points.GetChild(i).GetChild(j);
            }
        }

        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                x8pathpoints[j, i] = x8points.GetChild(i).GetChild(j);
            }
        }
    }

    private void OnEnable()
    {
        cards = new int[(gridSize - 2) * (gridSize - 2)];
        cardsOnGrid = new Card[(gridSize-2), (gridSize - 2)];
        occupied = new bool[gridSize, gridSize];
        generateNumbers();
        generateCards();
    }

    public void deselectCards()
    {
        foreach(Card card in cardsOnGrid)
        {
            card.unSelect();
        }
    }


    void generateCards()
    {
        for(int i=0;i<(gridSize-2);i++)
        {
            for(int j=0;j<(gridSize-2);j++)
            {
                //GameObject card = Instantiate(CardPrefab, rows[i]);
                Transform card = transform.GetChild(0).GetChild(i).GetChild(j);
                card.GetComponent<Card>().initializeCard(cards[i * (gridSize-2) + j], new Vector2Int(j,i));
                card.GetComponent<Card>().grid = this;
                cardsOnGrid[j, i] = card.GetComponent<Card>();
                occupied[j+1,i+1] = true;
            }
        }
    }

    void generateNumbers()
    {
        for(int i=0;i<((gridSize-2)*(gridSize-2));i+=2)
        {
            cards[i] = UnityEngine.Random.Range(0, 11);
            if(gridSize != 9)
            {
                cards[i + 1] = cards[i];
            }
        }

        int jolly = UnityEngine.Random.Range(8, 12);
        if(jolly == 11)
        {
            cards[0] = 11;
        }

        for (int i=0;i< (gridSize - 2) * (gridSize - 2); i++)
        {
            rndm = UnityEngine.Random.Range(0, (gridSize - 2) * (gridSize - 2)-1);
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

            if((cardSelect1.cardID == cardSelect2.cardID || cardSelect1.cardID == 11 || cardSelect2.cardID == 11) && cardSelect1.cardPosition != cardSelect2.cardPosition)
            {
                bool check = checkVectors(cardSelect1.cardPosition + new Vector2Int(1,1), cardSelect2.cardPosition + new Vector2Int(1, 1),true);
                if(check)
                {
                    //Matched & clear path 
                    occupied[cardSelect1.cardPosition.x+1, cardSelect1.cardPosition.y+1] = false;
                    occupied[cardSelect2.cardPosition.x + 1, cardSelect2.cardPosition.y + 1] = false;
                    cardSelect1.gameObject.SetActive(false);
                    cardSelect2.gameObject.SetActive(false);
                    if(cardSelect1.cardID == 10 || cardSelect2.cardID == 10)
                    {
                        manager.addScore(2);
                    }
                    else
                    {
                        manager.addScore(1);
                    }
                    StartCoroutine(clearPath());
                }
                else
                {
                    //Matched but not clear path
                    cardSelect1.unSelect();
                    cardSelect2.unSelect();
                    manager.addScore(-1);
                }
            }
            else
            {
                //Not Even Matched
                cardSelect1.unSelect();
                cardSelect2.unSelect();
                manager.addScore(-1);
            }
            manager.playerPlayed = true;
            manager.noPlays(checkAvailablePlays());
        }
    }

    public bool checkAvailablePlays()
    {
        Queue<Card> theCards = new Queue<Card>();
        for(int i = 0;i<(gridSize-2);i++)
        {
            for (int j = 0; j < (gridSize - 2); j++)
            {
                if (occupied[j+1,i+1])
                {
                    theCards.Enqueue(cardsOnGrid[j, i]);
                }
            }
        }

        while(theCards.Count > 0)
        {
            Card card = theCards.Dequeue();
            List<Card> identical = new List<Card>();
            foreach (Card c in theCards)
            {
                if(c.cardID == card.cardID)
                {
                    identical.Add(c);
                }
            }
            foreach(Card _card in identical)
            {
                bool check = checkVectors(_card.cardPosition, card.cardPosition,false);
                if(check)
                {
                    return true;
                }
            }  
        }

        return false;
    }


    public bool checkVectors(Vector2Int start,Vector2Int end,bool draw)
    {
        Queue<Node> queue = new Queue<Node>();
        HashSet<(Vector2Int, Vector2Int)> visited = new HashSet<(Vector2Int, Vector2Int)>();
        Dictionary<Vector2Int, Vector2Int> cameFrom = new Dictionary<Vector2Int, Vector2Int>();

            queue.Enqueue(new Node(start, directions[0], 0));
            visited.Add((start, directions[0]));

        while(queue.Count > 0)
        {
            Node current = queue.Dequeue();

            for(int i=1;i<=4;i++)
            {
                Vector2Int newPos = current.position + directions[i];
                int newTurnCount = 0;
                if (directions[i] == current.direction || current.direction == directions[0])
                {
                    newTurnCount = current.turnCount;
                }
                else
                {
                    newTurnCount = current.turnCount + 1;
                }


                if (isValidPos(newPos) && !visited.Contains((newPos, directions[i])) && newTurnCount <= 2)
                {
                    queue.Enqueue(new Node(newPos, directions[i], newTurnCount));
                    visited.Add((newPos, directions[i]));

                    if (!cameFrom.ContainsKey(newPos))
                    {
                        cameFrom[newPos] = current.position;
                    }
                }

                if (newPos == end && newTurnCount <= 2)
                {
                    cameFrom[newPos] = current.position;

                    if(draw)
                    {
                        drawPath(checkPath(cameFrom, start, end));
                    }
                    
                    return true;
                }

            }
        }

        return false;
    }

    bool isValidPos(Vector2Int pos)
    {
        if(pos.x >= 0 && pos.x < gridSize && pos.y >= 0 && pos.y < gridSize && !occupied[pos.x,pos.y])
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private List<Vector2Int> checkPath(Dictionary<Vector2Int, Vector2Int> cameFrom, Vector2Int start, Vector2Int end)
    {
        List<Vector2Int> path = new List<Vector2Int>();
        Vector2Int current = end;

        while (current != start)
        {
            path.Add(current);
            current = cameFrom[current];
        }

        path.Add(start);
        path.Reverse();

        return path;
    }

    private void drawPath(List<Vector2Int> path)
    {
        switch(gridSize)
        {
            case 6:
                foreach(Vector2Int pos in path)
                {
                    x4pathpoints[pos.x, pos.y].gameObject.SetActive(true);
                }
                break;
            case 8:
                foreach (Vector2Int pos in path)
                {
                    x6pathpoints[pos.x, pos.y].gameObject.SetActive(true);
                }
                break;
            case 9:
                foreach (Vector2Int pos in path)
                {
                    x7pathpoints[pos.x, pos.y].gameObject.SetActive(true);
                }
                break;
            case 10:
                foreach (Vector2Int pos in path)
                {
                    x8pathpoints[pos.x, pos.y].gameObject.SetActive(true);
                }
                break;
        }
    }

    void clearDrawTest()
    {
        switch (gridSize)
        {
            case 6:
                foreach (Transform pos in x4pathpoints)
                {
                    pos.gameObject.SetActive(false);
                }
                break;
            case 8:
                foreach (Transform pos in x6pathpoints)
                {
                    pos.gameObject.SetActive(false);
                }
                break;
            case 9:
                foreach (Transform pos in x7pathpoints)
                {
                    pos.gameObject.SetActive(false);
                }
                break;
            case 10:
                foreach (Transform pos in x8pathpoints)
                {
                    pos.gameObject.SetActive(false);
                }
                break;
        }
    }

    IEnumerator clearPath()
    {
        yield return new WaitForSeconds(0.35f);
        clearDrawTest();
    }
}
