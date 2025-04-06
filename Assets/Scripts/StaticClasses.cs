using System.Collections.Generic;
using System;
using UnityEngine;

public class StatClasses : MonoBehaviour {}

public static class MiniMax {
    public static (int, int, Dictionary<int, int>) GetBestMove(int[,] board, bool isX, int maxDepth) {
        int bestScore = -9999;
        int bestMove = 3;
        int totalIterations = 0;
        int alpha = -9999;
        int beta = 9999;

        List<int> posMoves = HelperFunction.GetMoves(board);
        HelperFunction.SortFromMiddle(posMoves);
        Dictionary<int, int> ScoreMap = new Dictionary<int, int>();

        foreach (int move in posMoves) {
            totalIterations++;
            HelperFunction.CommitMove(board, move, isX);

            int score;
            if (HelperFunction.IsGameOver(board, move)) {
                score = 10;
                HelperFunction.UndoMove(board, move);
            } else {
                score = Beta(board, !isX, 1, ref totalIterations, alpha, beta, maxDepth);
            }

            if (score > bestScore) {
                bestScore = score;
                bestMove = move;
            }

            alpha = Math.Max(alpha, bestScore);
            ScoreMap[move] = score;

            HelperFunction.UndoMove(board, move);
        }

        return (bestMove, totalIterations, ScoreMap);
    }

    static int Alpha(int[,] board, bool isX, int depth, ref int totalIterations, int alpha, int beta, int maxDepth) {
        if (depth > maxDepth) return 0;
        List<int> posMoves = HelperFunction.GetMoves(board);
        if (posMoves.Count <= 0) return 0;

        int bestScore = -9999;

        foreach (int move in posMoves) {
            totalIterations++;
            HelperFunction.CommitMove(board, move, isX);

            int score;
            if (HelperFunction.IsGameOver(board, move)) {
                score = 10 - depth;
            } else {
                score = Beta(board, !isX, depth + 1, ref totalIterations, alpha, beta, maxDepth);
            }

            bestScore = Math.Max(bestScore, score);
            alpha = Math.Max(alpha, bestScore);

            HelperFunction.UndoMove(board, move);

            if (alpha >= beta && depth > 3) break;
        }

        return bestScore;
    }

    static int Beta(int[,] board, bool isX, int depth, ref int totalIterations, int alpha, int beta, int maxDepth) {
        if (depth > maxDepth) return 0;
        List<int> posMoves = HelperFunction.GetMoves(board);
        if (posMoves.Count <= 0) return 0;

        int bestScore = 9999;

        foreach (int move in posMoves) {
            totalIterations++;
            HelperFunction.CommitMove(board, move, isX);

            int score;
            if (HelperFunction.IsGameOver(board, move)) {
                score = -10 + depth;
            } else {
                score = Alpha(board, !isX, depth + 1, ref totalIterations, alpha, beta, maxDepth);
            }

            bestScore = Math.Min(bestScore, score);
            beta = Math.Min(beta, bestScore);

            HelperFunction.UndoMove(board, move);

            if (alpha >= beta && depth > 3) break;
        }

        return bestScore;
    }
}

public static class HelperFunction {
    public static int GetDropSquare(int[,] board, int x) {
        int rows = board.GetLength(0);

        for (int i = rows - 1; i >= 0; i--)
        {
            if (board[i, x] == 0)
                return i;
        }

        return -1;
    }

    public static void SortFromMiddle(List<int> list) {
        int middle = 3; 

        list.Sort((a, b) => Math.Abs(a - middle).CompareTo(Math.Abs(b - middle)));
    }
    
    public static void CommitMove(int[,] board, int x, bool isX) {
        int y = GetDropSquare(board, x);

        board[y, x] = isX ? 1 : -1;
    }

    public static void UndoMove(int[,] board, int x) {
        int y = GetDropSquare(board, x) + 1;
        if (y < 6 && y >= 0) board[y, x] = 0; 
    }

    public static List<int> GetMoves(int[,] board) {
        List<int> Moves = new List<int>();
        for (int j = 0; j < 7; j++) {
            if (board[0, j] == 0) {
                Moves.Add(j);
            }
        }

        return Moves;
    }

    static readonly List<(int, int)> directionMap = new List<(int, int)> {(1, 0), (0, 1), (1, 1), (1, -1)};

    public static bool IsGameOver(int[,] board, int x) {
        int y = GetDropSquare(board, x) + 1;
        int player = board[y, x];

        foreach (var (i, j) in directionMap) {
            int count = 1;
            
            count += CountInDirection(board, y, x, i, j, player);
            count += CountInDirection(board, y, x, -i, -j, player);

            if (count >= 4) return true;
        }
           
        return false;
    }

    private static int CountInDirection(int[,] board, int startY, int startX, int dy, int dx, int player) {
        int newY = startY + dy;
        int newX = startX + dx;
        int count = 0;

        while (newY >= 0 && newY < 6 && newX >= 0 && newX < 7) {
            if (board[newY, newX] == player) {
                count += 1;
                newY += dy;
                newX += dx;
            } else {
                break;
            }
        }

        return count;
    }

public static void ShowBoard(int[,] board)
{
    string boardString = "";

    for (int i = 0; i < board.GetLength(0); i++)
    {
        string row = "| ";
        for (int j = 0; j < board.GetLength(1); j++)
        {
            string player = board[i, j] == 0 ? "." : board[i, j] == 1 ? "X" : "O";
            row += $" {player} ";
        }
        row += " |";
        boardString += row + "\n";
    }

    boardString += "-------------------------";
    Debug.Log(boardString);
}
}
