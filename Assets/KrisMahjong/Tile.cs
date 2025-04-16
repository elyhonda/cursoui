using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Tile : MonoBehaviour
{
    public int id; // Identificador da peça
    public Image image; // Imagem da peça
    public int Row { get; private set; } // Linha no grid
    public int Col { get; private set; } // Coluna no grid
    private Button button;

    void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnTileClicked);
    }

    public void SetupTile(int tileId, Sprite sprite)
    {
        id = tileId;
        image.sprite = sprite;
    }

    // Define a posição do tile no grid
    public void SetPosition(int row, int col)
    {
        Row = row;
        Col = col;
    }

    void OnTileClicked()
    {
        GameManager.Instance.SelectTile(this);
    }
}
