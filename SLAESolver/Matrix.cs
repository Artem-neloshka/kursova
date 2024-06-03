using System;

namespace SLAESolver
{
  public class MatrixException : Exception
  {
    public MatrixException()
    {
    }
  }

  public class Matrix
  {
    private double[,] values;

    public int Rows
    {
      get => values.GetLength(0);
    }

    public int Columns
    {
      get => values.GetLength(1);
    }

    // конструктор для створення порожньої матриці
    public Matrix(int rows, int columns)
    {
      if (rows < 1)
      {
        throw new ArgumentOutOfRangeException(nameof(rows));
      }

      if (columns < 1)
      {
        throw new ArgumentOutOfRangeException(nameof(columns));
      }

      values = new double[rows, columns];
    }

    // валідація індексу на межі
    private void ValidateIndex(int row, int col)
    {
      if (row > Rows - 1 || row < 0)
      {
        throw new ArgumentException(null, nameof(row));
      }

      if (col > Columns - 1 || col < 0)
      {
        throw new ArgumentException(null, nameof(col));
      }
    }

    // отримання числа з матриці
    public double this[int row, int column]
    {
      get
      {
        ValidateIndex(row, column);
        return values[row, column];
      }
      set
      {
        ValidateIndex(row, column);
        values[row, column] = value;
      }
    }

    // перевантажені оператори
    public static Matrix operator -(Matrix matrix1, Matrix matrix2)
    {
      if (matrix1 == null)
      {
        throw new ArgumentNullException(nameof(matrix1));
      }

      if (matrix2 == null)
      {
        throw new ArgumentNullException(nameof(matrix2));
      }

      // якщо розмірності не збігаються, то матриці відняти неможливо
      if (matrix1.Rows != matrix2.Rows || matrix1.Columns != matrix2.Columns)
      {
        throw new MatrixException();
      }

      // від кожного віднімаємо кожний
      Matrix resultMatrix = new Matrix(matrix1.Rows, matrix2.Columns);
      for (int i = 0; i < resultMatrix.Rows; i++)
      {
        for (int j = 0; j < resultMatrix.Columns; j++)
        {
          resultMatrix.values[i, j] = matrix1.values[i, j] - matrix2.values[i, j];
        }
      }

      return resultMatrix;
    }

    public static int counter;

    public static Matrix operator *(Matrix matrix1, Matrix matrix2)
    {
      if (matrix1 == null)
      {
        throw new ArgumentNullException(nameof(matrix1));
      }

      if (matrix2 == null)
      {
        throw new ArgumentNullException(nameof(matrix2));
      }

      // якщо кількість стовпців матриці 1 не дорівнює рядкам матриці 2, то матриці не множаться
      if (matrix1.Columns != matrix2.Rows)
      {
        throw new MatrixException();
      }

      Matrix resultMatrix = new Matrix(matrix1.Rows, matrix2.Columns);

      counter = 0;
      // цикл для проходження по всіх рядках лівої матриці
      for (int i = 0; i < matrix1.Rows; i++)
      {
        // цикл для проходження по всіх стовпцях правої матриці
        for (int j = 0; j < matrix2.Columns; j++)
        {
          // кожен елемент рядка лівої матриці треба помножити на кожен елемент стовпця правої
          for (int k = 0; k < matrix1.Columns; k++)
          {
            resultMatrix.values[i, j] += matrix1.values[i, k] * matrix2.values[k, j];

            counter++;
          }
        }
      }

      return resultMatrix;
    }

    private Matrix Subtract(Matrix matrix) => this - matrix;

    public Matrix Multiply(Matrix matrix) => this * matrix;

    // перевірка матриць на рівність
    public bool IsMatrixEqualsTo(Matrix matrix)
    {
      if (matrix.Columns != Columns || matrix.Rows != Rows)
      {
        return false;
      }

      Matrix resultMatrix = Subtract(matrix);
      for (int i = 0; i < resultMatrix.Rows; i++)
      {
        for (int j = 0; j < resultMatrix.Columns; j++)
        {
          if (resultMatrix[i, j] != 0)
          {
            return false;
          }
        }
      }

      return true;
    }


    // метод для обміну рядків матриці місцями
    public void SwapRows(int upperRowToSwap, int lowerRowToSwap)
    {
      for (int i = 0; i < Columns; i++)
      {
        (this[upperRowToSwap, i], this[lowerRowToSwap, i]) = (this[lowerRowToSwap, i], this[upperRowToSwap, i]);
      }
    }

    public Matrix Copy()
    {
      Matrix copied = new Matrix(Rows, Columns);
      for (int i = 0; i < Rows; i++)
      {
        for (int j = 0; j < Columns; j++)
        {
          copied[i, j] = this[i, j];
        }
      }

      return copied;
    }

    // транспонування
    public Matrix Transpose()
    {
      // створення нової матриці з протилежними розмірностями
      Matrix transposeMatrix = new Matrix(Columns, Rows);
      for (int i = 0; i < Rows; i++)
      {
        for (int j = 0; j < Columns; j++)
        {
          // заповнення новоствореної матриці
          transposeMatrix[j, i] = this[i, j];
        }
      }

      return transposeMatrix;
    }

    public double CalculateDeterminant(Matrix matrix, ref int iterationsAmount)
    {
      int size = matrix.Rows;
      if (size == 1)
        return matrix[0, 0];

      double determinant = 0;

      for (int i = 0; i < size; i++)
      {
        iterationsAmount++;

        if (matrix[0, i] == 0)
          continue;

        Matrix minor = GetMinor(matrix, i, size, ref iterationsAmount);
        determinant += matrix[0, i] * Math.Pow(-1, i) * CalculateDeterminant(minor, ref iterationsAmount);
      }

      return determinant;
    }

    private Matrix GetMinor(Matrix matrix, int col, int size, ref int iterationsAmount)
    {
      Matrix minor = new Matrix(size - 1, size - 1);

      for (int i = 0, p = 0; i < size; i++)
      {
        iterationsAmount++;

        if (i == 0)
          continue;

        for (int j = 0, q = 0; j < size; j++)
        {
          iterationsAmount++;

          if (j != col)
            minor[p, q++] = matrix[i, j];
        }

        p++;
      }

      return minor;
    }
  }
}
