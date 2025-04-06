using UnityEngine;

public class Tile : MonoBehaviour
{
    public int y, x;
    public bool isOccupied;
    public SpriteRenderer slot;

    void Highlight() {
        if (GameManager.instance.board[0, x] == 0 && !GameManager.instance.isAi) {
            if (GameManager.instance.isRed) {
                SetColor(GameManager.instance.redHighlightedColor, x);
            } else {
                SetColor(GameManager.instance.yellowHighlightedColor, x);
            }
        }
    }

    void OnMouseEnter() {
        Highlight();
    }

    void OnMouseExit() {
        if (GameManager.instance.board[0, x] == 0 && !GameManager.instance.isAi) {
            SetColor(GameManager.instance.backGroundColor, x);
        }
    }

    void OnMouseDown() {
        if (GameManager.instance.board[0, x] == 0 && !GameManager.instance.isAi) {
            int y = HelperFunction.GetDropSquare(GameManager.instance.board, x);

            if (GameManager.instance.isRed) {
                SetColor(GameManager.instance.redColor, x);
            } else {
                SetColor(GameManager.instance.yellowColor, x);
            }
            
            GameManager.instance.MakeMove(y, x);
        }

        Highlight();
    }

    public void SetColor(Color color, int x) {
        int y = HelperFunction.GetDropSquare(GameManager.instance.board, x);
        Tile dropTile = GameManager.instance.tiles[y, x];
        slot = dropTile.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>();

        slot.color = color;
    }
}
