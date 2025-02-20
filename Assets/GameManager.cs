using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject _4x4Grid,_6x6Grid,_7x7Grid,_8x8Grid;
    public float turnTime;
    private bool isGameStarted;
    private float currentTime = 10;
    public bool playerPlayed;
    private turns playersTurn;
    private int player1Score, player2Score;
    public Text score1,score2;
    public GameObject player1Indicator, player2Indicator;
    public Transform timeIndicator1, timeIndicator2;
    public Animator anim;
    private int gameMode;
    public GameObject winPanel;
    public Text winText;
    public FourX_Grid grid;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(isGameStarted)
        {
            if(Time.time - currentTime >= turnTime || playerPlayed)
            {
                currentTime = Time.time;
                switchTurn();
                grid.deselectCards();
                playerPlayed = false;
                timeIndicator1.localScale = new Vector3(1, 1, 1);
                timeIndicator2.localScale = new Vector3(1, 1, 1);
            }

            if(playersTurn == turns.player1)
            {
                timeIndicator1.localScale = new Vector3(1-(Time.time - currentTime)/10.0f, 1, 1);
            }
            else
            {
                timeIndicator2.localScale = new Vector3(1 - (Time.time - currentTime) / 10.0f, 1, 1);
            }
        }
    }

    private void switchTurn()
    {
        if(playersTurn == turns.player1)
        {
            playersTurn = turns.player2;
            player1Indicator.SetActive(false);
            player2Indicator.SetActive(true);
        }
        else
        {
            playersTurn = turns.player1;
            player2Indicator.SetActive(false);
            player1Indicator.SetActive(true);
        }
    }

    public void addScore(int score)
    {
        if(playersTurn == turns.player1)
        {
            player1Score += score;
            score1.text = player1Score.ToString();
        }
        else
        {
            player2Score += score;
            score2.text = player2Score.ToString();
        }
    }

    public void startGame(int index)
    {
        
        player1Score = 0;
        player2Score = 0;
        score1.text = "0";
        score2.text = "0";
        playersTurn = turns.player1;
        player1Indicator.SetActive(true);
        player2Indicator.SetActive(false);
        currentTime = Time.time;
        isGameStarted = true;
        gameMode = index;

        anim.SetTrigger("ToGame");

        switch (index)
        {
            case 0:
                _4x4Grid.SetActive(false);
                _6x6Grid.SetActive(false);
                _7x7Grid.SetActive(false);
                _8x8Grid.SetActive(false);
                for (int i=0;i<4;i++)
                {
                    for(int j=0;j<4;j++)
                    {
                        _4x4Grid.transform.GetChild(0).GetChild(i).GetChild(j).gameObject.SetActive(true);
                    }
                }
                _4x4Grid.SetActive(true);
                grid = _4x4Grid.GetComponent<FourX_Grid>();
                break;
            case 1:
                _4x4Grid.SetActive(false);
                _6x6Grid.SetActive(false);
                _7x7Grid.SetActive(false);
                _8x8Grid.SetActive(false);
                for (int i = 0; i < 6; i++)
                {
                    for (int j = 0; j < 6; j++)
                    {
                        _6x6Grid.transform.GetChild(0).GetChild(i).GetChild(j).gameObject.SetActive(true);
                    }
                }
                _6x6Grid.SetActive(true);
                grid = _6x6Grid.GetComponent<FourX_Grid>();
                break;
            case 2:
                _4x4Grid.SetActive(false);
                _6x6Grid.SetActive(false);
                _7x7Grid.SetActive(false);
                _8x8Grid.SetActive(false);
                for (int i = 0; i < 7; i++)
                {
                    for (int j = 0; j < 7; j++)
                    {
                        _7x7Grid.transform.GetChild(0).GetChild(i).GetChild(j).gameObject.SetActive(true);
                    }
                }
                _7x7Grid.SetActive(true);
                grid = _7x7Grid.GetComponent<FourX_Grid>();
                break;
            case 3:
                _4x4Grid.SetActive(false);
                _6x6Grid.SetActive(false);
                _7x7Grid.SetActive(false);
                _8x8Grid.SetActive(false);
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        _8x8Grid.transform.GetChild(0).GetChild(i).GetChild(j).gameObject.SetActive(true);
                    }
                }
                _8x8Grid.SetActive(true);
                grid = _8x8Grid.GetComponent<FourX_Grid>();
                break;
        }
        grid.deselectCards();
    }

    public void noPlays(bool check)
    {
        isGameStarted = check;
        if(!check)
        {
            winPanel.SetActive(true);
            if (player1Score > player2Score)
            {
                winText.text = "Player 1 Won";
            }
            else if (player1Score < player2Score)
            {
                winText.text = "Player 2 Won";
            }
            else
            {
                winText.text = "Draw";
            }
        }
        
    }

    public void backToMenu()
    {
        isGameStarted = false;
        winPanel.SetActive(false);
        anim.SetTrigger("ToMenu");
    }


    enum turns
    {
        player1,player2
    }
}
