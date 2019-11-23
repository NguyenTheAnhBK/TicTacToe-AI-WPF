using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using TicTacToe.Extensions;

namespace TicTacToe
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Start();
        }

        private int N = 3;
        private Button[,] board = null;
        private string com = "O", hum = "X";
        private int isWin;
        private bool isX = true;

        private void Start()
        {
            Player1.Opacity = 1;
            Player2.Opacity = 0.1;
            board = Board.Children.Cast<Button>().To2Dimension(N, N, (btns) =>
            {
                foreach (var btn in btns)
                {
                    btn.Content = null;
                    btn.Background = Brushes.White;
                }
            });

            Modal.Show();
        }

        private void NewGame_Click(object sender, RoutedEventArgs e)
        {
            Modal.Hide();
            
            if (!isX) //Computer first
            {
                Move move = FindBestMove(board);
                Turn(com, move.Row, move.Col);
            }
        }

        private void IsX_Click(object sender, RoutedEventArgs e)
        {
            isX = !isX;
            if (isX)
            {
                hum = "X";
                com = "O";
            }
            else
            {
                hum = "O";
                com = "X";
            }

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button curr = sender as Button;
            if (curr.Content != null || curr.Content?.ToString() == com)
                return;

            Turn(hum, Grid.GetRow(curr), Grid.GetColumn(curr));

            Move move = FindBestMove(board);
            if (move.isValid(N))
                Turn(com, move.Row, move.Col);

            isWin = Evaluate(board, (type, pos) =>
            {
                switch (type)
                {
                    case "row":
                        board[pos, 0].Background = board[pos, 1].Background = board[pos, 2].Background = Brushes.GreenYellow; break;
                    case "col":
                        board[0, pos].Background = board[1, pos].Background = board[2, pos].Background = Brushes.GreenYellow; break;
                    case "ldiagon":
                        board[0, 0].Background = board[1, 1].Background = board[2, 2].Background = Brushes.GreenYellow; break;
                    case "rdiagon":
                        board[0, 2].Background = board[1, 1].Background = board[2, 0].Background = Brushes.GreenYellow; break;
                }
            });

            if (isWin != 0)
            {
                if (isWin == 10)
                {
                    MessageText.Text = "Computer (" + com + ") win!!!";
                    Message.Show();
                }
                else
                {
                    MessageText.Text = "You (" + hum + ") Win!!!";
                    Message.Show();
                }
            }
            if (!IsMoveLeft(board))
            {
                MessageText.Text = "Fair";
                Message.Show();
            }
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            Message.Hide();
            Start();
        }

        private void Turn(string player, int row, int col)
        {
            board[row, col].Content = player;
            board[row, col].Foreground = player == "X" ? Brushes.Red : Brushes.Green;
            if (player == "X")
            {
                Player1.Opacity = 0.1;
                Player2.Opacity = 1;
            }
            else
            {
                Player1.Opacity = 1;
                Player2.Opacity = 0.1;
            }
        }

        private int Evaluate(Button[,] board, Action<string, int> executeWin = null)
        {
            // Checking for Rows for X or O victory.
            for (int row = 0; row < N; row++)
            {
                if (board[row, 0].Content?.ToString() == board[row, 1].Content?.ToString() &&
                    board[row, 1].Content?.ToString() == board[row, 2].Content?.ToString())
                {
                    if (board[row, 0].Content?.ToString() == com)
                    {
                        executeWin?.Invoke("row", row);
                        return +10;
                    }
                    else if (board[row, 0].Content?.ToString() == hum)
                    {
                        executeWin?.Invoke("col", row);
                        return -10;
                    }
                }
            }

            // Checking for Columns for X or O victory.
            for (int col = 0; col < 3; col++)
            {
                if (board[0, col].Content?.ToString() == board[1, col].Content?.ToString() &&
                    board[1, col].Content?.ToString() == board[2, col].Content?.ToString())
                {
                    if (board[0, col].Content?.ToString() == com)
                    {
                        executeWin?.Invoke("col", col);
                        return +10;
                    }
                    else if (board[0, col].Content?.ToString() == hum)
                    {
                        executeWin?.Invoke("col", col);
                        return -10;
                    }
                }
            }

            // Checking for Diagonals for X or O victory.
            if (board[0, 0].Content?.ToString() == board[1, 1].Content?.ToString() &&
                board[1, 1].Content?.ToString() == board[2, 2].Content?.ToString())
            {
                if (board[0, 0].Content?.ToString() == com)
                {
                    executeWin?.Invoke("ldiagon", 0);
                    return +10;
                }
                else if (board[0, 0].Content?.ToString() == hum)
                {
                    executeWin?.Invoke("ldiagon", 0);
                    return -10;
                }
            }

            if (board[0, 2].Content?.ToString() == board[1, 1].Content?.ToString() &&
                board[1, 1].Content?.ToString() == board[2, 0].Content?.ToString())
            {
                if (board[0, 2].Content?.ToString() == com)
                {
                    executeWin?.Invoke("rdiagon", 2);
                    return +10;
                }
                else if (board[0, 2].Content?.ToString() == hum)
                {
                    executeWin?.Invoke("rdiagon", 2);
                    return -10;
                }
            }

            return 0;
        }

        private bool IsMoveLeft(Button[,] board)
        {
            for (int i = 0; i < N; i++)
                for (int j = 0; j < N; j++)
                    if (board[i, j].Content == null)
                        return true;
            return false;
        }

        private int Minimax(Button[,] board, int depth, bool isMax)
        {
            int score = Evaluate(board);

            if (score == 10)
                return score;
            if (score == -10)
                return score;
            if (!IsMoveLeft(board))
                return 0;

            if (isMax)
            {
                int best = -1000;
                for (int i = 0; i < N; i++)
                    for (int j = 0; j < N; j++)
                    {
                        if (board[i, j].Content == null)
                        {
                            board[i, j].Content = com;
                            best = Math.Max(best, Minimax(board, depth + 1, !isMax));
                            board[i, j].Content = null;
                        }
                    }
                return best;
            }
            else
            {
                int best = 1000;
                for (int i = 0; i < N; i++)
                    for (int j = 0; j < N; j++)
                    {
                        if (board[i, j].Content == null)
                        {
                            board[i, j].Content = hum;
                            best = Math.Min(best, Minimax(board, depth + 1, !isMax));
                            board[i, j].Content = null;
                        }
                    }
                return best;
            }
        }

        private int AlphaBetaPrunning(Button[,] board, int depth, int alpha, int beta, bool isMax)
        {
            int score = Evaluate(board);

            if (score == 10)
                return score;
            if (score == -10)
                return score;

            if (!IsMoveLeft(board))
                return 0;

            if (isMax)
            {
                int v = -1000;
                for (int i = 0; i < N; i++)
                    for (int j = 0; j < N; j++)
                    {
                        if (board[i, j].Content == null)
                        {
                            board[i, j].Content = com;
                            v = Math.Max(v, AlphaBetaPrunning(board, depth + 1, alpha, beta, !isMax));
                            alpha = Math.Max(alpha, v);
                            board[i, j].Content = null;
                            if (beta <= alpha)
                                break;
                        }
                    }
                return v;
            }
            else
            {
                int v = 1000;
                for (int i = 0; i < N; i++)
                    for (int j = 0; j < N; j++)
                    {
                        if (board[i, j].Content == null)
                        {
                            board[i, j].Content = hum;
                            v = Math.Min(v, AlphaBetaPrunning(board, depth + 1, alpha, beta, !isMax));
                            beta = Math.Min(beta, v);
                            board[i, j].Content = null;
                            if (beta <= alpha)
                                break;
                        }
                    }
                return v;
            }
        }

        private Move FindBestMove(Button[,] board)
        {
            int bestVal = -1000;
            Move bestMove = new Move(-1, -1);
            for (int i = 0; i < N; i++)
                for (int j = 0; j < N; j++)
                    if (board[i, j].Content == null)
                    {
                        board[i, j].Content = com;
                        //int moveVal = Minimax(board, 0, false);
                        int moveVal = AlphaBetaPrunning(board, 0, -1000, 1000, false);
                        board[i, j].Content = null;
                        if (moveVal > bestVal)
                        {
                            bestVal = moveVal;
                            bestMove.Row = i;
                            bestMove.Col = j;
                        }
                    }
            return bestMove;
        }

        private class Move
        {
            public Move(int row, int col)
            {
                Row = row;
                Col = col;
            }

            public int Row { get; set; }
            public int Col { get; set; }

            public bool isValid(int n)
            {
                return Row >= 0 && Row <= n && Col >= 0 && Col <= n;
            }
        }
    }
}