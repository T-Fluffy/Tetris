using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class Board : MonoBehaviour
{
    public Tilemap tilemap { get; private set; }
    public Piece activePiece { get; private set; }
    public TetrominoData[] tetrominoses;
    public Vector3Int spawnPosition;
    public Vector2Int boardSize = new Vector2Int(10, 20);
    public GameObject gameOverText;

    public RectInt Bounds
    {
        get
        {
            Vector2Int position = new Vector2Int(-this.boardSize.x / 2, -this.boardSize.y / 2);
            return new RectInt(position, this.boardSize);
        }
    }
    private void Awake()
    {
        this.tilemap = GetComponentInChildren<Tilemap>();
        this.activePiece = GetComponentInChildren<Piece>();
        for(int i = 0; i < this.tetrominoses.Length; i++)
        {
            this.tetrominoses[i].Init();
        }
    }
    private void Start()
    {
        SpawnPieace();
    }

    public void SpawnPieace()
    {
        int random = Random.Range(0, this.tetrominoses.Length);
        TetrominoData data = this.tetrominoses[random];
        this.activePiece.Init(this, data, this.spawnPosition);
        if (IsValidPosition(this.activePiece,this.spawnPosition))
        {
            Set(this.activePiece);
        }
        else
        {
            GameOver();
        }
        Set(this.activePiece);
    }
    private void GameOver()
    {
        this.tilemap.ClearAllTiles();
        
    }
    public void Set(Piece piece)
    {
        for(int i = 0; i < piece.Cells.Length; i++)
        {
            Vector3Int tilePosition = piece.Cells[i]+piece.position;
            this.tilemap.SetTile(tilePosition, piece.data.tile);
        }
    }
    public void Clear(Piece piece)
    {
        for(int i = 0; i < piece.Cells.Length; i++)
        {
            Vector3Int tilePosition = piece.Cells[i]+piece.position;
            this.tilemap.SetTile(tilePosition, null);
        }
        
    }
    
    public bool IsValidPosition(Piece piece,Vector3Int position)
    {
        RectInt bounds = this.Bounds;
        for(int i = 0; i < piece.Cells.Length; i++)
        {
            Vector3Int TilePosition = piece.Cells[i] + position;
            if (!bounds.Contains((Vector2Int)TilePosition))
            {
                return false;
            }
            if (this.tilemap.HasTile(TilePosition))
            {
                return false;
            }
        }
        return true;
    }

    public void ClearLines()
    {
        RectInt bounds = this.Bounds;
        int row = bounds.yMin;
        while (row < bounds.yMax)
        {
            if (IsLineFull(row))
            {
                LineClear(row);
            }
            else
            {
                row++;
            }
        }
    }
    private bool IsLineFull(int row)
    {
        RectInt bounds = this.Bounds;
        for (int col=bounds.xMin;col<bounds.xMax;col++)
        {
            Vector3Int position = new Vector3Int(col, row, 0);
            if (!this.tilemap.HasTile(position))
            {
                return false;
            }
        }
        return true;
    }
    private void LineClear(int row )
    {
        RectInt bounds = this.Bounds;
        for (int col=bounds.xMin;col<bounds.xMax;col++)
        {
            Vector3Int position = new Vector3Int(col, row, 0);
            this.tilemap.SetTile(position, null);
        }
        while (row < bounds.yMax)
        {
            for (int col = bounds.xMin; col < bounds.xMax; col++)
            {
                Vector3Int position = new Vector3Int(col, row+1, 0);
                TileBase above = this.tilemap.GetTile(position);

                position = new Vector3Int(col, row, 0);
                this.tilemap.SetTile(position, above);
            }
            row++;
        }


    }
}
