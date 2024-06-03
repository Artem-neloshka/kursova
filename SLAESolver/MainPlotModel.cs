using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace SLAESolver
{
  public class MainViewModel
  {
    public PlotModel MyModel { get; private set; }

    public MainViewModel()
    {
      MyModel = new PlotModel { Title = "Multiple Lines Example" };
      InitializeAxes();
    }

    private void InitializeAxes()
    {
      var xAxis = new LinearAxis
      {
        Position = AxisPosition.Bottom,
        Title = "X Axis",
        PositionAtZeroCrossing = true,
        MinimumRange = 1,
        MaximumRange = 200
      };
      var yAxis = new LinearAxis
      {
        Position = AxisPosition.Left,
        Title = "Y Axis",
        PositionAtZeroCrossing = true,
        MinimumRange = 1,
        MaximumRange = 200
      };

      MyModel.Axes.Add(xAxis);
      MyModel.Axes.Add(yAxis);
    }

    public void AddLineToPlotModel(DataPoint point1, DataPoint point2, string lineTitle)
    {
      var series = new LineSeries
      {
        Title = lineTitle,
        MarkerType = MarkerType.None
      };
      series.Points.Add(point1);
      series.Points.Add(point2);

      MyModel.Series.Add(series);
      MyModel.InvalidatePlot(true);
    }

    public void ClearPlotModel()
    {
      MyModel.Series.Clear();
      MyModel.InvalidatePlot(true);
    }
  }
}