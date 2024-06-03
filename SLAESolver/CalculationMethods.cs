using System;

namespace SLAESolver
{
  public static class Methods
  {
    // метод LUP
    public static Matrix LUPMethod(Matrix coefficients, Matrix constantTerms, ref int counter)
    {
      // обмінюємо рядки СЛАР так, щоб діагональний елемент був максимальним за модулем
      for (int i = 0; i < coefficients.Columns; i++)
      {
        // вибір найбільшого елементу стовпця
        double maxInColumn = Math.Abs(coefficients[i, i]);
        int maxInColumnIndex = i;
        for (int j = i; j < coefficients.Rows; j++)
        {
          if (Math.Abs(coefficients[j, 0]) > maxInColumn)
          {
            // переініціалізація максимального елементу та обмін рядків
            maxInColumn = coefficients[j, 0];
            coefficients.SwapRows(maxInColumnIndex, j);
            constantTerms.SwapRows(maxInColumnIndex, j);
          }

          counter++;
        }
      }

      // матриці L та U
      Matrix matrixL = new Matrix(coefficients.Rows, coefficients.Columns);
      Matrix matrixU = new Matrix(coefficients.Rows, coefficients.Columns);

      for (int j = 0; j < coefficients.Rows; j++)
      {
        for (int i = j; i < coefficients.Columns; i++)
        {
          // обрахунок частини формули для матриці L
          double sumForL = 0;
          for (int k = 0; k < j; k++)
          {
            sumForL += matrixL[i, k] * matrixU[k, j];
            
            counter++;
          }

          // елементи матриць L та U нижче діагоналі
          matrixL[i, j] = coefficients[i, j] - sumForL;
          matrixU[i, j] = 0;

          // обрахунок частини формули для матриці U
          double sumForU = 0;
          for (int k = 0; k < j; k++)
          {
            sumForU += matrixL[j, k] * matrixU[k, i];
            
            counter++;
          }

          // елементи матриць L та U вище діагоналі
          if (j != i)
          {
            matrixL[j, i] = 0;
          }

          matrixU[j, i] = (coefficients[j, i] - sumForU) / matrixL[j, j];
        }
      }


      // x та y для матричних рівнянь
      Matrix matrixX = new Matrix(coefficients.Rows, 1);
      Matrix matrixY = new Matrix(coefficients.Rows, 1);
      for (int i = 0; i < coefficients.Rows; i++)
      {
        // сума для y
        double sumForY = 0;
        for (int j = 0; j < i; j++)
        {
          sumForY += matrixL[i, j] * matrixY[j, 0];
          
          counter++;
        }

        // заповнення матриці y
        matrixY[i, 0] = (constantTerms[i, 0] - sumForY) / matrixL[i, i];
      }

      for (int i = coefficients.Rows - 1; i >= 0; i--)
      {
        // сума для x
        double sumForY = 0;
        for (int j = i; j < coefficients.Columns; j++)
        {
          sumForY += matrixU[i, j] * matrixX[j, 0];
          
          counter++;
        }

        // заповнення матриці x
        matrixX[i, 0] = matrixY[i, 0] - sumForY;
      }

      return matrixX;
    }

    // метод обертання (якобі)
    public static Matrix JakobiMethod(Matrix coefficients, Matrix constantTerms, ref int counter)
    {
      for (int i = 0; i < coefficients.Rows; i++)
      {
        for (int j = i + 1; j < coefficients.Columns; j++)
        {
          // тимчасова матриця обертання
          Matrix temp = new Matrix(coefficients.Rows, coefficients.Columns);
          for (int k = 0; k < temp.Rows; k++)
          {
            // одинична головна діагональ
            temp[k, k] = 1;

            counter++;
          }

          // знаменник обох дробів
          double denominator = Math.Sqrt(Math.Pow(coefficients[i, i], 2) + Math.Pow(coefficients[j, i], 2));
          // косинус та синус кута обертання
          double c = coefficients[i, i] / denominator;
          double s = coefficients[j, i] / denominator;

          // заповнення матриці обертання
          temp[i, i] = c;
          temp[j, j] = c;
          temp[i, j] = s;
          temp[j, i] = -s;


          // вилучення змінних з нижніх рівнянь
          coefficients = temp.Multiply(coefficients);
          counter += Matrix.counter;
          
          constantTerms = temp.Multiply(constantTerms);
          counter += Matrix.counter;
        }
      }

      // матриця коренів
      Matrix roots = new Matrix(constantTerms.Rows, 1);
      for (int i = roots.Rows - 1; i >= 0; i--)
      {
        // сума для обчислення
        double sumForX = 0;
        for (int j = i + 1; j < coefficients.Rows; j++)
        {
          sumForX += coefficients[i, j] * roots[j, 0];

          counter++;
        }

        // заповнення матриці коренів
        roots[i, 0] = (constantTerms[i, 0] - sumForX) / coefficients[i, i];
      }

      return roots;
    }

    // метод квадратного кореня 
    public static Matrix SquareRootMethod(Matrix coefficients, Matrix constantTerms, ref int counter)
    {
      // матриця для обчислень
      Matrix matrixL = new Matrix(coefficients.Rows, coefficients.Columns);
      // проміжна матриця
      Matrix matrixY = new Matrix(matrixL.Rows, 1);
      for (int i = 0; i < matrixL.Rows; i++)
      {
        // матриця є нижньотрикутною
        for (int j = 0; j <= i; j++)
        {
          // сума для обрахунків
          double sumForL = 0;
          // діагональний елемент нижньотрикутної матриці
          if (i == j)
          {
            for (int k = 0; k < j; k++)
            {
              sumForL += Math.Pow(matrixL[j, k], 2);

              counter++;
            }

            if (sumForL > coefficients[j, j])
            {
              return new Matrix(1, 1);
            }
            
            matrixL[i, j] = Math.Sqrt(coefficients[j, j] - sumForL);
          }
          else
          {
            for (int k = 0; k < j; k++)
            {
              sumForL += matrixL[i, k] * matrixL[j, k];

              counter++;
            }

            // заповнення нижньотрикутної матриці
            matrixL[i, j] = (coefficients[i, j] - sumForL) / matrixL[j, j];
          }
        }

        // сума для обрахунків
        double sumForY = 0;
        for (int j = 0; j < i; j++)
        {
          sumForY += matrixL[i, j] * matrixY[j, 0];

          counter++;
        }

        // заповнення матриці
        matrixY[i, 0] = (constantTerms[i, 0] - sumForY) / matrixL[i, i];
      }

      // матриця коренів
      Matrix matrixX = new Matrix(matrixY.Rows, 1);
      // заповнення з кінця
      for (int i = matrixX.Rows - 1; i >= 0; i--)
      {
        // сума для обрахунків
        double sumForX = 0;
        for (int j = i + 1; j < coefficients.Columns; j++)
        {
          sumForX += matrixL[j, i] * matrixX[j, 0];

          counter++;
        }

        // заповнення матриці
        matrixX[i, 0] = (matrixY[i, 0] - sumForX) / matrixL[i, i];
      }

      return matrixX;
    }
  }
}
