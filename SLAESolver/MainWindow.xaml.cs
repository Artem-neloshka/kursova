using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using OxyPlot;
using OxyPlot.Wpf;
using HorizontalAlignment = System.Windows.HorizontalAlignment;
using VerticalAlignment = System.Windows.VerticalAlignment;

namespace SLAESolver
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow
  {
    private MainViewModel viewModel;
    public static Matrix coefficients;
    public static Matrix constantTerms;
    public static Matrix roots;

    public MainWindow()
    {
      InitializeComponent();

      AmountOfEquations.SelectedIndex = 0;
      SelectMethod.SelectedIndex = 0;

      viewModel = DataContext as MainViewModel;
      PlotContainer.Content = new PlotView { Model = viewModel.MyModel };
    }

    // створення обраної кількості рівнянь
    private void EquationsAmount_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      // очищуємо все при обранні нової кількості рівнянь
      Equations.Children.Clear();
      Solution.Children.Clear();
      SaveSolutionPanel.Children.Clear();
      ErrorMessage.Visibility = Visibility.Collapsed;
      PlotContainer.Visibility = Visibility.Collapsed;

      int selectedAmount = int.Parse(((ComboBoxItem)AmountOfEquations.SelectedItem).Content.ToString());

      for (int i = 0; i < selectedAmount; i++)
      {
        // створення місця для рівняння
        StackPanel equation = new StackPanel();
        equation.Orientation = Orientation.Horizontal;
        equation.Margin = new Thickness(0, 5, 0, 0);

        for (int j = 0; j < selectedAmount; j++)
        {
          // поле для вписування коефіцієнту
          TextBox coefficient = new TextBox();
          Styles.AddStylesTextBox(coefficient, 20, 30, Brushes.Gray, 2, 5, 5, 5, 0);

          // додавання реакції на зміну контенту в полі
          coefficient.TextChanged += Coefficient_TextChanged;
          equation.Children.Add(coefficient);

          // змінна та +
          TextBlock plus = new TextBlock();
          plus.Margin = new Thickness(0, 5, 0, 0);
          // якщо коефіцієнт останній, то плюс не ставиться
          plus.Text = (j != selectedAmount - 1) ? $"x{j + 1}+" : $"x{j + 1}";
          equation.Children.Add(plus);
        }

        // знак =
        TextBlock equalsSign = new TextBlock();
        equalsSign.Margin = new Thickness(5, 5, 0, 0);
        equalsSign.Text = "=";
        equation.Children.Add(equalsSign);

        // поле для вільного члена
        TextBox freeTerm = new TextBox();
        Styles.AddStylesTextBox(freeTerm, 20, 30, Brushes.Gray, 2, 5, 5, 5, 0);

        freeTerm.TextChanged += Coefficient_TextChanged;

        equation.Children.Add(freeTerm);
        Equations.Children.Add(equation);
      }
    }

    // реакція на введення контенту
    private void Coefficient_TextChanged(object sender, TextChangedEventArgs e)
    {
      ErrorMessage.Visibility = Visibility.Collapsed;

      foreach (var coefficientsInEquation in Equations.Children)
      {
        if (!(coefficientsInEquation is StackPanel panel)) continue;

        foreach (var possibleCoefficient in panel.Children)
        {
          if (!(possibleCoefficient is TextBox box)) continue;

          // отримання контенту
          string coefficient = box.Text;
          if (coefficient != string.Empty)
          {
            // перевірка контенту на число
            bool isValidPositiveFractional =
              coefficient.StartsWith("0") && coefficient.Length > 1 && !coefficient.Contains(",");
            bool isValidNegativeFractional =
              coefficient.StartsWith("-0") && coefficient.Length > 2 && !coefficient.Contains(",");


            if (coefficient.Contains(" ")
                || isValidNegativeFractional
                || isValidPositiveFractional
                || !double.TryParse(coefficient, out _))
            {
              // якщо контент не є числом, то показати повідомлення та зафарбувати клітинку червоним
              ErrorMessage.Visibility = Visibility.Visible;
              box.BorderBrush = Brushes.Red;
            }
            else
            {
              // якщо все класно, то зафарбувати клітинку чорний
              box.BorderBrush = Brushes.Black;
            }
          }
          else
          {
            // якщо клітинка є порожньою, то зафарбувати її сірим
            box.BorderBrush = Brushes.Gray;
          }
        }
      }
    }

    // ураааааа працює
    private void ReadCoefficients()
    {
      int currentEquation = 0;
      foreach (var stackPanel in Equations.Children)
      {
        if (!(stackPanel is StackPanel equation)) continue;

        int currentCoefficient = 0;
        foreach (var possibleCoefficient in equation.Children)
        {
          if (!(possibleCoefficient is TextBox coefficient)) continue;

          if (currentCoefficient < coefficients.Columns)
          {
            if (double.TryParse(coefficient.Text, out var coefficientValue))
              coefficients[currentEquation, currentCoefficient] = coefficientValue;

            currentCoefficient++;
          }
          else
          {
            if (double.TryParse(coefficient.Text, out var coefficientValue))
              constantTerms[currentEquation, 0] = coefficientValue;
          }
        }

        currentEquation++;
      }
    }

    // краааааа працює
    private void GenerateMatrix_OnClick(object sender, RoutedEventArgs e)
    {
      Solution.Children.Clear();
      SaveSolutionPanel.Children.Clear();
      ErrorMessage.Visibility = Visibility.Collapsed;
      PlotContainer.Visibility = Visibility.Collapsed;
      Random random = new Random();
      foreach (var coefficientsInEquation in Equations.Children)
      {
        if (!(coefficientsInEquation is StackPanel panel)) continue;

        foreach (var possibleCoefficient in panel.Children)
        {
          if (!(possibleCoefficient is TextBox box)) continue;

          box.Text = ((random.NextDouble() - 0.5) * 100).ToString();
          box.BorderBrush = Brushes.Black;
        }
      }
    }

    private void ClearMatrix_OnClick(object sender, RoutedEventArgs e)
    {
      Solution.Children.Clear();
      SaveSolutionPanel.Children.Clear();
      ErrorMessage.Visibility = Visibility.Collapsed;
      PlotContainer.Visibility = Visibility.Collapsed;
      foreach (var coefficientsInEquation in Equations.Children)
      {
        if (!(coefficientsInEquation is StackPanel panel)) continue;

        foreach (var possibleCoefficient in panel.Children)
        {
          if (!(possibleCoefficient is TextBox box)) continue;

          box.Text = "";
          box.BorderBrush = Brushes.Gray;
        }
      }
    }

    private void Calculate_OnClick(object sender, RoutedEventArgs e)
    {
      Solution.Children.Clear();
      SaveSolutionPanel.Children.Clear();
      ComplexityPanel.Children.Clear();
      PlotContainer.Visibility = Visibility.Collapsed;
      if (ErrorMessage.Visibility != Visibility.Collapsed) return;

      int sizeOfMatrix = int.Parse(((ComboBoxItem)AmountOfEquations.SelectedItem).Content.ToString());
      coefficients = new Matrix(sizeOfMatrix, sizeOfMatrix);
      constantTerms = new Matrix(sizeOfMatrix, 1);

      ReadCoefficients();
      Matrix coefficientsCopy = coefficients.Copy();
      Matrix constantTermsCopy = constantTerms.Copy();


      int iterations = 0;
      double determinant = coefficients.CalculateDeterminant(coefficients.Copy(), ref iterations);
      if (determinant == 0)
      {
        TextBlock detEqualsToZeroMessage = new TextBlock();
        detEqualsToZeroMessage.Text = "Determinant equals to 0, slae has not roots";
        Solution.Children.Add(detEqualsToZeroMessage);
      }
      else if (double.IsPositiveInfinity(determinant) || double.IsNegativeInfinity(determinant))
      {
        TextBlock detEqualsToZeroMessage = new TextBlock();
        detEqualsToZeroMessage.Text = "Determinant's value is out of double range, " +
                                      "impossible to know is determinant valid";
        Solution.Children.Add(detEqualsToZeroMessage);
      }
      else
      {
        string solveMethod = ((ComboBoxItem)SelectMethod.SelectedItem).Content.ToString();
        roots = new Matrix(coefficients.Rows, 1);

        int complexity = 0;
        TextBlock complexityTextBlock = new TextBlock();
        complexityTextBlock.Margin = new Thickness(5, 5, 5, 0);
        Stopwatch stopwatch = new Stopwatch();

        switch (solveMethod)
        {
          case "LUP":
            stopwatch.Start();
            roots = Methods.LUPMethod(coefficients, constantTerms, ref complexity);
            stopwatch.Stop();
            complexityTextBlock.Text = complexity + " " + stopwatch.Elapsed.TotalMilliseconds;
            ComplexityPanel.Children.Add(complexityTextBlock);
            break;

          case "Jacobi":
            stopwatch.Start();
            roots = Methods.JakobiMethod(coefficients, constantTerms, ref complexity);
            stopwatch.Stop();
            complexityTextBlock.Text = complexity + " " + stopwatch.Elapsed.TotalMilliseconds;
            ComplexityPanel.Children.Add(complexityTextBlock);
            break;

          case "Square root":
            if (!coefficients.IsMatrixEqualsTo(coefficients.Transpose()))
            {
              TextBlock forSquareRootShouldBeSym = new TextBlock();
              forSquareRootShouldBeSym.Text = "For this method matrix should be symetric";
              Solution.Children.Add(forSquareRootShouldBeSym);
              return;
            }
            else
            {
              stopwatch.Start();
              roots = Methods.SquareRootMethod(coefficients, constantTerms, ref complexity);
              stopwatch.Stop();
              if (roots.Rows == 1)
              {
                TextBlock forSquareRootShouldBePositiveDefinite = new TextBlock();
                forSquareRootShouldBePositiveDefinite.Text = "For this method matrix should be positive definite";
                Solution.Children.Add(forSquareRootShouldBePositiveDefinite);
                return;
              }

              complexityTextBlock.Text = complexity + " " + stopwatch.Elapsed.TotalMilliseconds;
              ComplexityPanel.Children.Add(complexityTextBlock);
            }

            break;

          default:
            throw new Exception();
        }

        for (int i = 0; i < roots.Rows; i++)
        {
          if (double.IsPositiveInfinity(roots[i, 0]) || double.IsNegativeInfinity(roots[i, 0]) ||
              double.IsNaN(roots[i, 0]))
          {
            TextBlock infError = new TextBlock();
            infError.Text = "Sorry, we cannot calculate so big numbers: it's out of double range!";
            Solution.Children.Add(infError);
            SaveSolutionPanel.Children.Clear();
            return;
          }
        }

        TextBlock rootsDisplayed = new TextBlock();
        string rootsString = "";
        for (int i = 0; i < roots.Rows; i++)
        {
          rootsString += $"x{i + 1}: {roots[i, 0]}\n";
        }

        rootsDisplayed.Text = rootsString;
        Solution.Children.Add(rootsDisplayed);


        if (((ComboBoxItem)AmountOfEquations.SelectedItem).Content.ToString() == "2")
        {
          viewModel.ClearPlotModel();
          PlotContainer.Visibility = Visibility.Visible;
          BuildLine(coefficientsCopy, constantTermsCopy, 0);
          BuildLine(coefficientsCopy, constantTermsCopy, 1);
        }

        Button saveResults = new Button();
        Styles.AddStylesButton(saveResults, "Save results", HorizontalAlignment.Left,
          VerticalAlignment.Top, 5, 5, 5, 0);
        saveResults.Name = "SaveResultsBtn";
        saveResults.Click += WriteFile.WriteSolutionToFile;
        SaveSolutionPanel.Children.Add(saveResults);
      }
    }

    private void PlotBuild(DataPoint point1, DataPoint point2, string lineTitle)
    {
      viewModel.AddLineToPlotModel(point1, point2, lineTitle);
    }

    private void BuildLine(Matrix coefficientsInEquation, Matrix constTerms, int equationNum)
    {
      double x1 = 0;
      double y1 = 0;
      double x2 = 0;
      double y2 = 0;

      double xRange = 100;

      double a = coefficientsInEquation[equationNum, 0];
      double b = coefficientsInEquation[equationNum, 1];
      double c = constTerms[equationNum, 0];

      if (b == 0)
      {
        x1 = x2 = c / a;
        y1 = roots[equationNum, 0] - xRange;
        y2 = roots[equationNum, 0] + xRange;
      }
      else
      {
        x1 = roots[equationNum, 0] - xRange;
        y1 = (c - a * x1) / b;

        x2 = roots[equationNum, 0] + xRange;
        y2 = (c - a * x2) / b;
      }

      PlotBuild(new DataPoint(x1, y1), new DataPoint(x2, y2), "Line " + equationNum);
    }

    private void ExitBtn_OnClick(object sender, RoutedEventArgs e)
    {
      Application.Current.Shutdown();
    }
  }
}
