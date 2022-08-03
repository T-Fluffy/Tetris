using System;
using UnityEngine.Tilemaps;
using UnityEngine;
public enum Tetromino
{
    I,
    O,
    T,
    J,
    L,
    S,
    Z
}
[Serializable] 
public struct TetrominoData
{
    public Tetromino tetromino;
    public Tile tile;
    public Vector2Int[] Cells  { get; private set; }
    public Vector2Int[,] wallKicks { get; private set; }
    
    public void Init()
    {
        this.Cells = Data.Cells[this.tetromino];
        this.wallKicks = Data.WallKicks[this.tetromino];
    }
}