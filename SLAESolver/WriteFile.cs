using System.IO;
using System.Windows;

namespace SLAESolver
{
	public static class WriteFile
	{
		private static void MatrixToString(Matrix coefficients, Matrix terms)
		{
			string[] equationsInStrings = new string[coefficients.Rows + 1];
			for (int i = 0; i < coefficients.Rows; i++)
			{
				for (int j = 0; j < coefficients.Columns; j++)
				{
					equationsInStrings[i] += coefficients[i, j] + " ";
				}

				equationsInStrings[i] += terms[i, 0];
			}

			for (int i = 0; i < MainWindow.roots.Rows; i++)
			{
				equationsInStrings[equationsInStrings.Length - 1] += MainWindow.roots[i, 0] + " ";
			}
      
			File.WriteAllLines("solution.txt", equationsInStrings);
		}
    
		public static void WriteSolutionToFile(object sender, RoutedEventArgs e)
		{
			MatrixToString(MainWindow.coefficients, MainWindow.constantTerms);
		}
	}
}