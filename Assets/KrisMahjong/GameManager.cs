using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameObject tilePrefab;
    public Transform gridParent;
    public List<Sprite> tileSprites;

    private Tile firstSelected, secondSelected;
    private List<Tile> tiles = new List<Tile>();

    private int[,] grid; // Representação do grid para verificar caminhos
    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        GenerateGrid(6, 6); // Gera um grid 6x6
    }

    void GenerateGrid(int rows, int cols)
    {
        if (tilePrefab == null || gridParent == null)
        {
            Debug.LogError("tilePrefab ou gridParent não foi atribuído no Inspector!");
            return;
        }

        if (tileSprites == null || tileSprites.Count == 0)
        {
            Debug.LogError("A lista de tileSprites está vazia!");
            return;
        }

        // Inicializa o grid
        grid = new int[rows, cols];

        List<TileData> tilesData = new List<TileData>();

        // Cria pares de tiles com o mesmo ID e imagem
        for (int i = 0; i < (rows * cols) / 2; i++)
        {
            Sprite sprite = tileSprites[i % tileSprites.Count]; // Seleciona a imagem
            int id = i; // ID único para o par

            // Adiciona dois tiles com o mesmo ID e imagem
            tilesData.Add(new TileData(id, sprite)); // Primeiro tile do par
            tilesData.Add(new TileData(id, sprite)); // Segundo tile do par
        }

        // Embaralha os dados dos tiles (IDs e imagens)
        tilesData = ShuffleList(tilesData);

        // Cria os tiles com os dados embaralhados
        int index = 0;
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                GameObject tileObj = Instantiate(tilePrefab, gridParent);
                Tile tile = tileObj.GetComponent<Tile>();

                if (tile == null)
                {
                    Debug.LogError("O prefab da peça não tem o script Tile anexado!");
                    return;
                }

                // Atribui o ID e a imagem ao tile
                tile.SetupTile(tilesData[index].id, tilesData[index].sprite);
                tile.SetPosition(row, col); // Define a posição no grid
                tiles.Add(tile);

                // Atualiza o grid
                grid[row, col] = tilesData[index].id;

                index++;
            }
        }
    }

    public void SelectTile(Tile tile)
    {
        if (firstSelected == null)
        {
            firstSelected = tile;
        }
        else if (secondSelected == null)
        {
            if (firstSelected == tile) // Verifica se a mesma peça foi clicada duas vezes
            {
                Debug.Log("Você não pode selecionar a mesma peça duas vezes!");
                return;
            }

            secondSelected = tile;
            CheckMatch();
        }
    }

    void CheckMatch()
    {
        if (firstSelected.id == secondSelected.id)
        {
            // Verifica se há um caminho válido entre os dois tiles
            if (IsPathValid(firstSelected, secondSelected))
            {
                Debug.Log("Par encontrado e caminho válido!");
                Destroy(firstSelected.gameObject);
                Destroy(secondSelected.gameObject);
                // Atualiza o grid para marcar os tiles como removidos
                grid[firstSelected.Row, firstSelected.Col] = -1;
                grid[secondSelected.Row, secondSelected.Col] = -1;
            }
            else
            {
                Debug.Log("Caminho inválido!");
            }
        }
        else
        {
            Debug.Log("Não é um par!");
        }
        firstSelected = null;
        secondSelected = null;
    }

    bool IsPathValid(Tile start, Tile end)
    {
        // Verifica se há um caminho com no máximo duas curvas
        return HasValidPath(start.Row, start.Col, end.Row, end.Col, 0, -1);
    }

    bool HasValidPath(int startRow, int startCol, int endRow, int endCol, int curves, int direction)
    {
        // Verifica se chegou ao destino
        if (startRow == endRow && startCol == endCol)
        {
            return true;
        }

        // Verifica se excedeu o número máximo de curvas (2)
        if (curves > 2)
        {
            return false;
        }

        // Tenta mover nas quatro direções (cima, baixo, esquerda, direita)
        int[] rowOffsets = { -1, 1, 0, 0 };
        int[] colOffsets = { 0, 0, -1, 1 };

        for (int i = 0; i < 4; i++)
        {
            int newRow = startRow + rowOffsets[i];
            int newCol = startCol + colOffsets[i];

            // Verifica se a nova posição está dentro do grid e está livre
            if (newRow >= 0 && newRow < grid.GetLength(0) && newCol >= 0 && newCol < grid.GetLength(1))
            {
                if (grid[newRow, newCol] == -1 || (newRow == endRow && newCol == endCol))
                {
                    // Verifica se a direção mudou (curva)
                    int newDirection = i;
                    int newCurves = curves;
                    if (direction != -1 && newDirection != direction)
                    {
                        newCurves++;
                    }

                    // Recursão para verificar o próximo passo
                    if (HasValidPath(newRow, newCol, endRow, endCol, newCurves, newDirection))
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    // Método genérico para embaralhamento
    List<T> ShuffleList<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            T temp = list[i];
            int randomIndex = Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
        return list;
    }

    // Classe para armazenar dados do tile
    class TileData
    {
        public int id;
        public Sprite sprite;

        public TileData(int id, Sprite sprite)
        {
            this.id = id;
            this.sprite = sprite;
        }
    }
}