using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private void Awake()
    {
        if (instance == null) {
            instance = this;
        } else Destroy(gameObject);
    }

    public Color redColor = Color.red;
    public Color redHighlightedColor = new Color(0.5f, 0f, 0f); // Dark red
    public Color yellowColor = Color.yellow;
    public Color yellowHighlightedColor = new Color(0.6f, 0.6f, 0f); // Dark yellow
    public Color backGroundColor = new Color32(0xBD, 0xC8, 0xD9, 0xFF); // BG

    public GameObject tilePrefab, evalLabelPref, MoveButton;
    public Tile[,] tiles;
    public int[,] board;
    public float tileScale = 2;
    public bool isRed, isGameOver, showEval;
    public bool isAi, isAiNext = false;
    public int maxDepth = 5;
    public (int, int) AiMove;
    public int playerScore, aiScore = 0;
    public TextMeshProUGUI ScoreText, ResultText, DepthText, IterationsText;
    public TextMeshProUGUI[] CalculatedVals = new TextMeshProUGUI[7];
    public Transform labelSpawn, boardParent;

    void Start() {
        SetUpBoard();
    }

    public void SetUpBoard() {
        if (CalculatedVals[0] != null) {
            foreach (TextMeshProUGUI text in CalculatedVals) {
                Destroy(text.gameObject);
            }
        }

        DepthText.text = maxDepth.ToString();

        isRed = true;
        isGameOver = false;
        isAi = isAiNext;

        board = new int[6, 7];
        tiles = new Tile[6, 7];
        CalculatedVals = new TextMeshProUGUI[7];

        if (!isAi) MoveButton.SetActive(false);

        for (int j = 0; j < 7; j++) {
            for (int i = 0; i < 6; i++) {
                GameObject tileObj = Instantiate(tilePrefab, boardParent);
                tileObj.name = "tile-" + i + "" + j;
                tileObj.transform.position = new Vector2(j * tileScale - tileScale * 3 + boardParent.position.x, tileScale * 2.5f - i * tileScale + boardParent.position.y);
                tileObj.transform.localScale = new Vector3(tileScale, tileScale, 1);
                Tile tile = tileObj.GetComponent<Tile>();
                tile.y = i;
                tile.x = j;
                tile.isOccupied = false;
                tiles[i, j] = tile;
            }

            GameObject evalLabel = Instantiate(evalLabelPref, labelSpawn);
            evalLabel.name = "label-" + j;
            RectTransform rt = evalLabel.GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2((j * tileScale - tileScale * 3) * 33, -210);
            TextMeshProUGUI evalLabelText = evalLabel.GetComponent<TextMeshProUGUI>();
            CalculatedVals[j] = evalLabelText;
            evalLabelText.text = "";
        }

        if (isAi) StartCoroutine(DelayedAiMove());
    }


    public void MakeMove(int y, int x) {
        Tile tile = tiles[y, x];

        if (!tile.isOccupied) {
            tile.isOccupied = true;
            if (isRed) {
                tile.SetColor(redColor, x);
            } else {
                tile.SetColor(yellowColor, x);
            }

            board[y, x] = this.isRed ? 1 : -1;
            // HelperFunction.ShowBoard(board);

            List<int> moves = HelperFunction.GetMoves(board);

            if (HelperFunction.IsGameOver(board, x)) {
                End(false);
                return;
            } else if (moves.Count <= 0) {
                End(true);
                return;
            }

            isAi  = !isAi;
            isRed = !isRed;

            if (isAi) {
                StartCoroutine(DelayedAiMove());
            } else {
                MoveButton.SetActive(false);
                IterationsText.enabled = false;
                foreach (TextMeshProUGUI label in CalculatedVals) {
                    label.enabled = false;
                }
            }
        }
    }

    public void SetAiMove() {
        var (bestMoveX, totalIterations, ScoreMap) = MiniMax.GetBestMove(board, isRed, maxDepth);
        int bestMoveY = HelperFunction.GetDropSquare(board, bestMoveX);

        Debug.Log(bestMoveY + " " + bestMoveX);
        if (board[bestMoveY, bestMoveX] != 0) {
            AiMove = (bestMoveY, bestMoveX);
        } else {
            Debug.Log("hello");
            Debug.LogError("AI MOVE IS OBSTRUCTED");
        }

        if (showEval) {
            foreach (KeyValuePair<int, int> entry in ScoreMap) {
                CalculatedVals[entry.Key].enabled = true;
                CalculatedVals[entry.Key].text = entry.Value.ToString();
            }

            IterationsText.enabled = true;
            IterationsText.text = "Iterations: " + totalIterations.ToString();
            MoveButton.SetActive(true);

        } else {
            MakeAiMove();
        }
    }

    private IEnumerator DelayedAiMove() {
        yield return null;
        SetAiMove();
    }   

    public void MakeAiMove() {
        MakeMove(AiMove.Item1, AiMove.Item2);
    }

    public void End(bool isDraw) {
        isGameOver = true;
        foreach (Tile tile in tiles) {
            tile.isOccupied = true;
        }

        if (isDraw) {
            ResultText.text = "DRAW";
        }
        else if (isAi) {
            aiScore++;
            ResultText.text = "AI WINS!";
        } else {
            playerScore++;
            ResultText.text = "PLAYER WINS!";
        }
        ScoreText.text = $"{playerScore} - {aiScore}";
    }

    public void SetMaxDepth(float depth) {
        DepthText.text = maxDepth.ToString();
        maxDepth = (int)depth;
    }

    public void SetNextAi(bool isOn) {
        this.isAiNext = isOn;
    }

    public void SetShowEval(bool isOn) {
        showEval = isOn;
    }
}
