using Microsoft.Maui.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace MauiMapAppDemo.Behaviors
{
    public class HeightProfileViewBehavior : Behavior<GraphicsView>, IDrawable
    {
        private GraphicsView? _graphicsView;

        protected override void OnAttachedTo(GraphicsView bindable)
        {
            base.OnAttachedTo(bindable);
            _graphicsView = bindable;
            _graphicsView.Drawable = this;
        }

        protected override void OnDetachingFrom(GraphicsView bindable)
        {
            if (ReferenceEquals(_graphicsView, bindable))
            {
                _graphicsView.Drawable = null;
                _graphicsView = null;
            }

            base.OnDetachingFrom(bindable);
        }

        public static readonly BindableProperty HeightProfilesProperty =
            BindableProperty.Create(
                nameof(HeightProfiles),
                typeof(Dictionary<double, double>),
                typeof(HeightProfileViewBehavior),
                propertyChanged: OnHeightProfileChanged);

        public Dictionary<double, double>? HeightProfiles
        {
            get => (Dictionary<double, double>?)GetValue(HeightProfilesProperty);
            set => SetValue(HeightProfilesProperty, value);
        }


        public static readonly BindableProperty MeasureStartProperty =
            BindableProperty.Create(
                nameof(MeasureStart),
                typeof(Location),
                typeof(HeightProfileViewBehavior),
                propertyChanged: OnMeasurementChanged);

        public Location MeasureStart
        {
            get => (Location)GetValue(MeasureStartProperty);
            set => SetValue(MeasureStartProperty, value);
        }

        public static readonly BindableProperty MeasureEndProperty =
            BindableProperty.Create(
                nameof(MeasureEnd),
                typeof(Location),
                typeof(HeightProfileViewBehavior),
                propertyChanged: OnMeasurementChanged
            );

        public Location MeasureEnd
        {
            get => (Location)GetValue(MeasureEndProperty);
            set => SetValue(MeasureEndProperty, value);
        }

        private static void OnMeasurementChanged(
           BindableObject bindable,
           object oldValue,
           object newValue)
        {
            ((HeightProfileViewBehavior)bindable).InvalidateGraphicsView();
        }

        private static void OnHeightProfileChanged(
            BindableObject bindable,
            object oldValue,
            object newValue)
        {
            ((HeightProfileViewBehavior)bindable).InvalidateGraphicsView();
        }

        private void InvalidateGraphicsView()
        {
            _graphicsView?.Invalidate();
        }

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            canvas.SaveState();
            canvas.FillColor = Colors.White;
            canvas.FillRectangle(dirtyRect);

            var points = HeightProfiles?
                .OrderBy(item => item.Key)
                .Select(item => new PointF((float)item.Key, (float)item.Value))
                .ToList() ?? [];

            if (points.Count < 2)
            {
                DrawEmptyState(canvas, dirtyRect);
                canvas.RestoreState();
                return;
            }

            var margin = 10f;
            var plotRect = new RectF(
                dirtyRect.X + margin,
                dirtyRect.Y + margin,
                Math.Max(0, dirtyRect.Width - (margin * 2)),
                Math.Max(0, dirtyRect.Height - (margin * 2)));

            DrawAxes(canvas, plotRect);

            var minX = points.Min(point => point.X);
            var maxX = points.Max(point => point.X);
            var minY = points.Min(point => point.Y);
            var maxY = points.Max(point => point.Y);

            if (Math.Abs(maxX - minX) < 0.0001f)
            {
                maxX = minX + 1f;
            }

            if (Math.Abs(maxY - minY) < 0.0001f)
            {
                maxY = minY + 1f;
            }

            canvas.StrokeColor = Colors.DarkSlateBlue;
            canvas.StrokeSize = 2;

            PointF? previousPoint = null;
            foreach (var point in points)
            {
                var x = plotRect.Left + ((point.X - minX) / (maxX - minX)) * plotRect.Width;
                var y = plotRect.Bottom - ((point.Y - minY) / (maxY - minY)) * plotRect.Height;
                var currentPoint = new PointF(x, y);

                if (previousPoint.HasValue)
                {
                    canvas.DrawLine(previousPoint.Value, currentPoint);
                }

                previousPoint = currentPoint;
            }

            foreach (var point in points)
            {
                var x = plotRect.Left + ((point.X - minX) / (maxX - minX)) * plotRect.Width;
                var y = plotRect.Bottom - ((point.Y - minY) / (maxY - minY)) * plotRect.Height;

                canvas.FillColor = Colors.DarkOrange;
                canvas.FillCircle(x, y, 2.5f);
            }

            canvas.RestoreState();
        }

        private static void DrawAxes(ICanvas canvas, RectF plotRect)
        {
            canvas.StrokeColor = Colors.LightGray;
            canvas.StrokeSize = 1;

            canvas.DrawLine(plotRect.Left, plotRect.Bottom, plotRect.Right, plotRect.Bottom);
            canvas.DrawLine(plotRect.Left, plotRect.Top, plotRect.Left, plotRect.Bottom);
        }

        private static void DrawEmptyState(ICanvas canvas, RectF dirtyRect)
        {
            canvas.FontColor = Colors.DarkSlateGray;
            canvas.FontSize = 12;
            canvas.DrawString(
                "Select two points to draw the height profile.",
                dirtyRect,
                HorizontalAlignment.Center,
                VerticalAlignment.Center);
        }

    }
}
