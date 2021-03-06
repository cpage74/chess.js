using ChessDotNetBackend;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#pragma warning disable CS1718

namespace ChessTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestSquareEquality()
        {
            Square s1 = new Square(0, 0);
            Square s2 = new Square(0, 1);
            Square s3 = new Square(0, 1);
            Square s4 = new Square(1, 1);
            Square s5 = new Square(1, 1);

            Assert.AreEqual(s1, s1);
            Assert.AreNotEqual(s1, s2);
            Assert.AreEqual(s2, s3);
            Assert.AreNotEqual(s3, s4);
            Assert.AreEqual(s4, s5);
        }

        [TestMethod]
        public void TestSquareOffset()
        {
            Square s6 = new Square(4, 4);
            Square s7 = new Square(2, 6);
            Square s8 = s6.Offset(-2, 2);
            Assert.AreNotEqual(s6, s7);
            Assert.AreEqual(s7, s8);
        }

        [TestMethod]
        public void TestSquareCopy()
        {
            Square s1 = new Square(4, 4);
            Square s2 = s1;
            Assert.AreEqual(s1, s2);
            Assert.AreNotSame(s1, s2);
        }

        [TestMethod]
        public void TestInitBoard()
        {
            Board board = Board.InitNewGame();
            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 2; y++)
                {
                    // black pieces at top
                    IPiece piece = board.GetPieceOnSquare(new Square(x, y));
                    Assert.IsTrue(!piece.White);
                }
                for (int y = 6; y < 8; y++)
                {
                    // empty in the middle
                    IPiece piece = board.GetPieceOnSquare(new Square(x, y));
                    Assert.IsTrue(piece.White);
                }
                for (int y = 2; y < 6; y++)
                {
                    // white pieces at bottom
                    IPiece piece = board.GetPieceOnSquare(new Square(x, y));
                    Assert.IsNull(piece);
                }
            }
        }

        [TestMethod]
        public void TestPawnMove()
        {
            Board board = Board.InitNewGame();
            IPiece pawn = board.GetPieceOnSquare(new Square(4, 6));
            Assert.IsTrue(pawn.IsMoveValid(board, new Square(4, 5)));
            Assert.IsTrue(pawn.IsMoveValid(board, new Square(4, 4)));
            Assert.IsFalse(pawn.IsMoveValid(board, new Square(4, 3)));
            board.Pieces.Add(new Pawn(new Square(4, 5), false));
            board.Pieces.Add(new Pawn(new Square(3, 5), true));
            board.Pieces.Add(new Pawn(new Square(5, 5), true));
            Assert.IsFalse(pawn.IsMoveValid(board, new Square(4, 5)));
            Assert.IsFalse(pawn.IsMoveValid(board, new Square(4, 4)));
            Assert.IsFalse(pawn.IsMoveValid(board, new Square(3, 5)));
            Assert.IsFalse(pawn.IsMoveValid(board, new Square(5, 5)));

            pawn = board.GetPieceOnSquare(new Square(1, 1));
            Assert.IsFalse(pawn.IsMoveValid(board, new Square(0, 2)));
            Assert.IsTrue(pawn.IsMoveValid(board, new Square(1, 2)));
            Assert.IsFalse(pawn.IsMoveValid(board, new Square(2, 2)));
            board.Pieces.Add(new Pawn(new Square(0, 2), false));
            Assert.IsFalse(pawn.IsMoveValid(board, new Square(0, 2)));
            board.Pieces.Add(new Pawn(new Square(1, 2), true));
            Assert.IsFalse(pawn.IsMoveValid(board, new Square(1, 2)));
            board.Pieces.Add(new Pawn(new Square(2, 2), true));
            Assert.IsTrue(pawn.IsMoveValid(board, new Square(2, 2)));
        }

        [TestMethod]
        public void TestBoardComparedToNull()
        {
            Assert.AreNotEqual(new Board(), null);
            Assert.AreNotEqual(null, new Board());
            Assert.AreEqual(new Board(), new Board());
            Assert.AreNotSame(new Board(), new Board());
        }

        [TestMethod]
        public void TestCopyBoard()
        {
            Board board = Board.InitNewGame();
            Board board1 = new Board(board);
            Assert.AreNotSame(board, board1);
            Assert.AreEqual(board, board1);
            board1.Pieces.Add(new Pawn(new Square(4, 4), true));
            Assert.AreNotEqual(board, board1);
            board.Pieces.Add(new Pawn(new Square(4, 4), true));
            Assert.AreEqual(board, board1);
        }

        [TestMethod]
        public void TestScoring()
        {
            TranspositionTable transpositionTable = new TranspositionTable();
            Board empty = new Board();
            Assert.IsTrue(0 == empty.CalcWhitesScore(transpositionTable));

            Board start = Board.InitNewGame();
            Assert.IsTrue(0 == start.CalcWhitesScore(transpositionTable));

            start.Pieces.Add(new Pawn(new Square(4, 4), true));
            Assert.IsTrue(start.CalcWhitesScore(transpositionTable) > 0);
            start.Pieces.Add(new Queen(new Square(2, 4), false));
            Assert.IsTrue(start.CalcWhitesScore(transpositionTable) < 0);

        }

        [TestMethod]
        public void TestScoring2()
        {
            TranspositionTable transpositionTable = new TranspositionTable();
            Board pawn1 = new Board();
            Board pawn2 = new Board();
            pawn1.Pieces.Add(new Pawn(new Square(4, 6), true));
            pawn2.Pieces.Add(new Pawn(new Square(4, 5), true));
            Assert.IsTrue(pawn2.CalcWhitesScore(transpositionTable) > pawn1.CalcWhitesScore(transpositionTable));
            pawn1 = new Board();
            pawn2 = new Board();
            pawn1.Pieces.Add(new Pawn(new Square(2, 4), false));
            pawn2.Pieces.Add(new Pawn(new Square(2, 5), false));
            Assert.IsTrue(pawn2.CalcWhitesScore(transpositionTable) < pawn1.CalcWhitesScore(transpositionTable));
        }

        [TestMethod]
        public void TestZobrist()
        {
            var b1 = Board.InitNewGame();
            var h1 = new ZobristHash(b1);
            var b2 = Board.InitNewGame();
            var h2 = new ZobristHash(b2);
            Assert.AreEqual(h1, h2);

            b1.Pieces.Add(new Pawn(new Square(4, 4), true));
            b2.Pieces.Add(new Pawn(new Square(4, 4), false));
            var h3 = new ZobristHash(b1);
            var h4 = new ZobristHash(b2);
            Assert.AreNotEqual(h1, h3);
            Assert.AreNotEqual(h4, h3);
        }

        /*
        [TestMethod]
        public void TestTranspositionTablePerformance()
        {
            IUserInterface userInterface = new MockUI();
            Game game = new Game(false, false, userInterface);
        }
        */
    }
}
