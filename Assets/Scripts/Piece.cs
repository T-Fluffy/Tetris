using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Piece : MonoBehaviour
{
    public Board board { get; private set; }
    public TetrominoData data { get; private set; }
    public Vector3Int[] Cells { get; private set; }
    public Vector3Int position { get; private set; }
    public int rotationIndex { get; private set; }
    public float stepDelay = 1f;
    public float lockDelay = 0.5f;
    public float stepTime,lockTime;

    public void Init(Board board,TetrominoData data,Vector3Int position)
    {
        this.board = board;
        this.position = position;
        this.data = data;
        this.rotationIndex = 0;
        this.stepTime = Time.time + this.stepDelay;
        this.lockTime = 0f;
        if (this.Cells == null)
        {
            this.Cells = new Vector3Int[data.Cells.Length];
        }
        for(int i = 0; i < data.Cells.Length; i++)
        {
            this.Cells[i] = (Vector3Int)data.Cells[i];
        }
    }
    private void Update()
    {
        this.board.Clear(this);
        this.lockTime += Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.R))
        {
            Rotate(-1);
        }else if (Input.GetKeyDown(KeyCode.T))
        {
            Rotate(1);
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Move(Vector2Int.left);
        }else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            Move(Vector2Int.right);
        }else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            Move(Vector2Int.down);
        }else if (Input.GetKeyDown(KeyCode.Space))
        {
            HardDrop();
        }
        if (Time.time >= this.stepTime)
        {
            Step();
        }
        this.board.Set(this);
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("UI");
        }
    }
    private void Step()
    {
        this.stepTime = Time.time + this.stepDelay;
        Move(Vector2Int.down);
        if (this.lockTime>=this.lockDelay)
        {
            Lock() ;
        }
    }
    private void Lock()
    {
        this.board.Set(this);
        this.board.ClearLines();
        this.board.SpawnPieace();
    }
    private void Rotate(int direction)
    {
        int originalRotation = this.rotationIndex;
        this.rotationIndex = Wrap(this.rotationIndex,0,4);
        ApplyRotationMatrix(direction);
        if (!TestWallKicks(this.rotationIndex,direction))
        {
            this.rotationIndex = originalRotation;
            ApplyRotationMatrix(-direction);  
        }
    }
    private void ApplyRotationMatrix(int direction)
    {
        for (int i = 0; i < this.Cells.Length; i++)
        {
            Vector3 cell = this.Cells[i];
            int x, y;
            switch (this.data.tetromino)
            {
                case Tetromino.I:
                case Tetromino.O:
                    cell.x -= 0.5f;
                    cell.y -= 0.5f;
                    x = Mathf.CeilToInt((cell.x * Data.RotationMatrix[0] * direction) + (cell.y * Data.RotationMatrix[1] * direction));
                    y = Mathf.CeilToInt((cell.x * Data.RotationMatrix[2] * direction) + (cell.y * Data.RotationMatrix[3] * direction));
                    break;
                default:
                    x = Mathf.RoundToInt((cell.x * Data.RotationMatrix[0] * direction) + (cell.y * Data.RotationMatrix[1] * direction));
                    y = Mathf.RoundToInt((cell.x * Data.RotationMatrix[2] * direction) + (cell.y * Data.RotationMatrix[3] * direction));
                    break;
            }
            this.Cells[i] = new Vector3Int(x, y, 0);
        }
    }
    private bool TestWallKicks(int rotationIndex, int rotationdirection)
    {
        int WallKickIndex = GetWallLickIndex(rotationIndex, rotationdirection);
        for(int i = 0; i < this.data.wallKicks.GetLength(1); i++)
        {
            Vector2Int translation = this.data.wallKicks[WallKickIndex, i];
            if (Move(translation))
            {
                return true;
            }
        }
        return false;
    }
    private int  GetWallLickIndex(int rotationIndex, int rotationdirection)
    {
        int wallkickindex = rotationIndex * 2;
        if (rotationdirection < 0)
        {
            wallkickindex--;
        }
        return Wrap(wallkickindex,0, this.data.wallKicks.GetLength(0));
    }
    private int Wrap(int input, int min, int max)
    {
        if (input < min)
        {
            return max - (min - input) % (max - min);
        }
        else
        {
            return min + (input - min) % (max - min);
        }
    }

    private void HardDrop()
    {
        while (Move(Vector2Int.down))
        {
            continue;
        }
    }

    private bool Move(Vector2Int translation)
    {
        Vector3Int newPosition = this.position;
        newPosition.x += translation.x;
        newPosition.y += translation.y;
        bool valid = this.board.IsValidPosition(this, newPosition);
        if (valid)
        {
            this.position = newPosition;
            this.lockTime = 0f;
        }
        return valid;
    }
}
