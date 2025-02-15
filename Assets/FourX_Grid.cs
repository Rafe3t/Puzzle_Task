using Unity.Mathematics.Geometry;
using UnityEngine;

public class FourX_Grid : MonoBehaviour
{
    public GameObject CardPrefab;
    public GameObject[][] _4x4;
    public Transform[] rows;
    public int[] cards;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
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
                Instantiate(CardPrefab, rows[i]);
            }
        }
    }

    void generateNumbers()
    {
        cards[0] = Random.Range(0, 9);
    }
}
