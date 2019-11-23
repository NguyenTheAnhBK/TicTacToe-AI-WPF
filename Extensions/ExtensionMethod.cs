using System;
using System.Collections.Generic;
using System.Linq;

namespace TicTacToe.Extensions
{
    public static class ExtensionMethod
    {
        public static T[,] To2Dimension<T>(this IEnumerable<T> ena, int rows, int cols, Action<IEnumerable<T>> beforeExecute = null)
        {
            if (ena.Count() != rows * cols)
                return null;

            beforeExecute?.Invoke(ena);
            T[,] arr2D = new T[rows, cols];
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    arr2D[i, j] = ena.ToList()[rows * i + j];

            return arr2D;
        }
    }
}