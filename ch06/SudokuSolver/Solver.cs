namespace SudokuSolver;

public class Solver
{
  private readonly int[,] _board;
  private readonly int _numRows;
  private readonly int _numCols;
  private readonly int _gridSize;

  public Solver(int[,] board)
  {
    _board = board;
    _numRows = _board.GetLength(0);
    _numCols = _board.GetLength(1);
    _gridSize = (int)Math.Sqrt(_numRows);
  }

  public bool IsValid()
  {
    if (_numRows != _numCols || _numRows < 4
        || (_gridSize * _gridSize) != _numRows)
      return false;

    return CheckRows()
        && CheckColumns() 
        && CheckSubGrids();
  }

  internal bool CheckRows() 
  {
    var usedSet = new HashSet<int>();
    for (int row = 0; row < _numRows; row++)
    {
      usedSet.Clear();
      for (int col = 0; col < _numCols; col++) 
      {
        int num = _board[row, col];
        if (num == 0)
          continue;
        if (usedSet.Contains(num))
          return false;
        usedSet.Add(num);
      }
    }

    return true;
  }

  internal bool CheckColumns()
  {
    var usedSet = new HashSet<int>();
    for (int col = 0; col < _numCols; col++)
    {
      usedSet.Clear();
      for (int row = 0; row < _numRows; row++) 
      {
        int num = _board[row, col];
        if (num == 0)
          continue;
        if (usedSet.Contains(num))
          return false;
        usedSet.Add(num);
      }
    }

    return true;
  }

  internal bool CheckSubGrids()
  {
    var usedSet = new HashSet<int>();
    for (int grid = 0; grid < _numRows; grid++)
    {
      usedSet.Clear();
      int startCol = (grid % _gridSize) * _gridSize;
      int startRow = (grid / _gridSize) * _gridSize;
      for (int cell = 0; cell < _numRows; cell++)
      {
        int col = startCol + (cell % _gridSize);
        int row = startRow + (cell / _gridSize);
        int num = _board[row, col];
        if (num == 0)
          continue;
        if (usedSet.Contains(num))
          return false;
        usedSet.Add(num);
      }
    }

    return true;
  }
}
