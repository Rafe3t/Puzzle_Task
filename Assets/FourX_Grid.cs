using System;
using Unity.Mathematics;
using Unity.Mathematics.Geometry;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using Unity.VisualScripting;
using NUnit;
using static UnityEditor.PlayerSettings;

public class FourX_Grid : MonoBehaviour
{
    public GameObject CardPrefab;
    public bool[,] occupied = new bool[6,6];
    public int[] cards;
    private int rndm,store;
    private bool selected;
    private Card cardSelect1, cardSelect2;
    private int gridSize = 6;
    public Transform[] pathPoints1, pathPoints2, pathPoints3, pathPoints4,pathPoints5,pathPoints6;

    private readonly Vector2Int[] directions = {
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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        generateNumbers();
        generateCards();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown("o"))
        {
            clearDrawTest();
        }
    }

    void generateCards()
    {
        for(int i=0;i<4;i++)
        {
            for(int j=0;j<4;j++)
            {
                //GameObject card = Instantiate(CardPrefab, rows[i]);
                Transform card = transform.GetChild(0).GetChild(i).GetChild(j);
                card.GetComponent<Card>().initializeCard(cards[i * 4 + j], new Vector2Int(j,i));
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

            if(cardSelect1.cardID == cardSelect2.cardID && cardSelect1.cardPosition != cardSelect2.cardPosition)
            {
                bool check = checkVectors(cardSelect1.cardPosition + new Vector2Int(1,1), cardSelect2.cardPosition + new Vector2Int(1, 1));
                if(check)
                {
                    Debug.Log("Matched & clear path :)");
                    occupied[cardSelect1.cardPosition.x+1, cardSelect1.cardPosition.y+1] = false;
                    occupied[cardSelect2.cardPosition.x + 1, cardSelect2.cardPosition.y + 1] = false;
                    Destroy(cardSelect1.gameObject);
                    Destroy(cardSelect2.gameObject);
                }
                else
                {
                    Debug.Log("Matched but not clear path :(");
                    cardSelect1.unSelect();
                    cardSelect2.unSelect();
                }
            }
            else
            {
                Debug.Log("Not Even Matched :(");
                cardSelect1.unSelect();
                cardSelect2.unSelect();
            }
        }
    }


    public bool checkVectors(Vector2Int start,Vector2Int end)
    {
        Queue<Node> queue = new Queue<Node>();
        HashSet<(Vector2Int, Vector2Int)> visited = new HashSet<(Vector2Int, Vector2Int)>();
        Dictionary<Vector2Int, Vector2Int> cameFrom = new Dictionary<Vector2Int, Vector2Int>();

        foreach (var dir in directions)
        {
            queue.Enqueue(new Node(start, dir, 0));
            visited.Add((start, dir));
        }

        while(queue.Count > 0)
        {
            Node current = queue.Dequeue();

            foreach(var dir in directions)
            {
                Vector2Int newPos = current.position + dir;
                int newTurnCount = 0;
                if(dir == current.direction)
                {
                    newTurnCount = current.turnCount;
                }
                else
                {
                    newTurnCount = current.turnCount + 1;
                }


                if (isValidPos(newPos) && !visited.Contains((newPos,dir)) && newTurnCount <= 3)
                {
                    queue.Enqueue(new Node(newPos, dir, newTurnCount));
                    visited.Add((newPos, dir));

                    if (!cameFrom.ContainsKey(newPos))
                    {
                        cameFrom[newPos] = current.position;
                    }
                }

                if (newPos == end && newTurnCount <= 3)
                {
                    cameFrom[newPos] = current.position;

                    drawPath(checkPath(cameFrom, start, end));

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
        foreach(Vector2Int pos in path)
        {
            if (pos.y == 0)
            {
                pathPoints1[pos.x].gameObject.SetActive(true);
            }
            else if (pos.y == 1)
            {
                pathPoints2[pos.x].gameObject.SetActive(true);
            }
            else if (pos.y == 2)
            {
                pathPoints3[pos.x].gameObject.SetActive(true);
            }
            else if (pos.y == 3)
            {
                pathPoints4[pos.x].gameObject.SetActive(true);
            }
            else if (pos.y == 4)
            {
                pathPoints5[pos.x].gameObject.SetActive(true);
            }
            else if (pos.y == 5)
            {
                pathPoints6[pos.x].gameObject.SetActive(true);
            }
        }
    }

    void clearDrawTest()
    {
        for (int i = 0; i < 6; i++)
        {
            pathPoints1[i].gameObject.SetActive(false);
        }
        for (int i = 0; i < 6; i++)
        {
            pathPoints2[i].gameObject.SetActive(false);
        }
        for (int i = 0; i < 6; i++)
        {
            pathPoints3[i].gameObject.SetActive(false);
        }
        for (int i = 0; i < 6; i++)
        {
            pathPoints4[i].gameObject.SetActive(false);
        }
        for (int i = 0; i < 6; i++)
        {
            pathPoints5[i].gameObject.SetActive(false);
        }
        for (int i = 0; i < 6; i++)
        {
            pathPoints6[i].gameObject.SetActive(false);
        }
    }
}
