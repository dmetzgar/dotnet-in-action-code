using Xunit;

namespace SudokuSolver.UnitTests;

public class SolverTests
{
  [Fact]
  public void Empty4x4Board()
  {
    int[,] empty = new int[4,4];
    var solver = new Solver(empty);
    Assert.True(solver.IsValid());
  }

  [Fact]
  public void NonSquareBoard()
  {
    int[,] empty = new int[4,9];
    var solver = new Solver(empty);
    Assert.False(solver.IsValid());
  }

  [Theory]
  [InlineData(0, false)]
  [InlineData(1, false)]
  [InlineData(4, true)]
  [InlineData(8, false)]
  [InlineData(9, true)]
  [InlineData(10, false)]
  [InlineData(16, true)]
  public void EmptyBoardSizes(int size, bool isValid)
  {
    int[,] empty = new int[size, size];
    var solver = new Solver(empty);
    Assert.Equal(isValid, solver.IsValid());
  }

  public static IEnumerable<object[]> Boards 
  {
    get
    {
       var board = new int[4,4];
       board[1,0] = 1;
       board[3,0] = 1;
       yield return new object[] { board, false };
       board = new int[4,4];
       board[1,0] = 1;
       board[1,2] = 1;
       yield return new object[] { board, false };
       board = new int[4,4];
       board[1,2] = 1;
       board[0,3] = 1;
       yield return new object[] { board, false };
       board = new int[4,4];
       board[1,1] = 1;
       board[2,3] = 1;
       yield return new object[] { board, true };
    }
  }

  [Theory]
  [MemberData(nameof(Boards))]
  public void CheckRules(int[,] board, bool isValid)
  {
    var solver = new Solver(board);
    Assert.Equal(isValid, solver.IsValid());
  }

  [Theory]
  [MemberData(nameof(CheckRowsData))]
  public void CheckRows(int[,] board, bool isValid)
  {
    var solver = new Solver(board);
    Assert.Equal(isValid, solver.CheckRows());
  }

  public static IEnumerable<object[]> CheckRowsData
  {
    get
    {
       yield return new object[] { new int[,] {{1,2,3,0,1}}, false };
       yield return new object[] { new int[,] {{1,2,3,0,4}}, true };
       yield return new object[] { new int[,] {{0,0,3}}, true };
       yield return new object[] { new int[,] {{0,2,0,2}}, false };
    }
  }
} 
