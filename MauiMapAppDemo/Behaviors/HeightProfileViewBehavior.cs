using Microsoft.Maui.Graphics;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace MauiMapAppDemo.Behaviors
{
    public class HeightProfileViewBehavior : Behavior<GraphicsView>, IDrawable
    {
        private GraphicsView? _graphicsView;
        private INotifyPropertyChanged? _bindingContextPropertyChangedSource;

        private void OnGraphicsViewBindingContextChanged(object? sender, EventArgs e)
        {
            SyncBindingContext();
        }

        private void OnBindingContextPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName is nameof(HeightProfiles) or nameof(MeasureStart) or nameof(MeasureEnd) or "IsHeightProfilesUpdated")
            {
                InvalidateGraphicsView();
            }
        }

        private void SyncBindingContext()
        {
            if (_bindingContextPropertyChangedSource != null)
            {
                _bindingContextPropertyChangedSource.PropertyChanged -= OnBindingContextPropertyChanged;
                _bindingContextPropertyChangedSource = null;
            }

            if (_graphicsView == null)
            {
                BindingContext = null;
                return;
            }

            BindingContext = _graphicsView.BindingContext;
            _bindingContextPropertyChangedSource = BindingContext as INotifyPropertyChanged;

            if (_bindingContextPropertyChangedSource != null)
            {
                _bindingContextPropertyChangedSource.PropertyChanged += OnBindingContextPropertyChanged;
            }
        }

        protected override void OnAttachedTo(GraphicsView bindable)
        {
            base.OnAttachedTo(bindable);
            _graphicsView = bindable;
            SyncBindingContext();
            _graphicsView.BindingContextChanged += OnGraphicsViewBindingContextChanged;
            _graphicsView.Drawable = this;
        }

        protected override void OnDetachingFrom(GraphicsView bindable)
        {
            if (ReferenceEquals(_graphicsView, bindable))
            {
                _graphicsView.BindingContextChanged -= OnGraphicsViewBindingContextChanged;
                if (_bindingContextPropertyChangedSource != null)
                {
                    _bindingContextPropertyChangedSource.PropertyChanged -= OnBindingContextPropertyChanged;
                    _bindingContextPropertyChangedSource = null;
                }
                _graphicsView.Drawable = null;
                BindingContext = null;
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

            var leftMargin = 72f;
            var rightMargin = 12f;
            var topMargin = 12f;
            var bottomMargin = 30f;
            var plotRect = new RectF(
                dirtyRect.X + leftMargin,
                dirtyRect.Y + topMargin,
                Math.Max(0, dirtyRect.Width - leftMargin - rightMargin),
                Math.Max(0, dirtyRect.Height - topMargin - bottomMargin));

            DrawAxes(canvas, plotRect, points);

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

            DrawAxisTitles(canvas, plotRect, dirtyRect);

            canvas.RestoreState();
        }

        private static void DrawAxes(ICanvas canvas, RectF plotRect, IReadOnlyList<PointF> points)
        {
            canvas.StrokeColor = Colors.LightGray;
            canvas.StrokeSize = 1;

            canvas.DrawLine(plotRect.Left, plotRect.Bottom, plotRect.Right, plotRect.Bottom);
            canvas.DrawLine(plotRect.Left, plotRect.Top, plotRect.Left, plotRect.Bottom);

            DrawXTicks(canvas, plotRect, points);
            DrawYTicks(canvas, plotRect, points);
        }

        private static void DrawXTicks(ICanvas canvas, RectF plotRect, IReadOnlyList<PointF> points)
        {
            var minX = 0f;
            var maxX = points.Max(point => point.X);
            var tickCount = 5;

            canvas.FontSize = 10;
            canvas.FontColor = Colors.DimGray;

            for (var i = 0; i <= tickCount; i++)
            {
                var ratio = i / (float)tickCount;
                var x = plotRect.Left + ratio * plotRect.Width;
                var value = minX + ratio * (maxX - minX);

                canvas.DrawLine(x, plotRect.Bottom, x, plotRect.Bottom + 4);
                canvas.DrawString(
                    FormatKilometres(value),
                    new RectF(x - 18, plotRect.Bottom + 4, 36, 12),
                    HorizontalAlignment.Center,
                    VerticalAlignment.Top);
            }
        }

        private static void DrawYTicks(ICanvas canvas, RectF plotRect, IReadOnlyList<PointF> points)
        {
            var minY = points.Min(point => point.Y);
            var maxY = points.Max(point => point.Y);
            var tickCount = 5;

            canvas.FontSize = 10;
            canvas.FontColor = Colors.DimGray;

            for (var i = 0; i <= tickCount; i++)
            {
                var ratio = i / (float)tickCount;
                var y = plotRect.Bottom - ratio * plotRect.Height;
                var value = minY + ratio * (maxY - minY);

                canvas.DrawLine(plotRect.Left - 4, y, plotRect.Left, y);
                canvas.DrawString(
                    value.ToString("F1"),
                    new RectF(4, y - 6, plotRect.Left - 12, 12),
                    HorizontalAlignment.Right,
                    VerticalAlignment.Center);
            }
        }

        private static void DrawAxisTitles(ICanvas canvas, RectF plotRect, RectF dirtyRect)
        {
            canvas.FontColor = Colors.DimGray;
            canvas.FontSize = 12;

            canvas.DrawString(
                "Distance (km)",
                new RectF(plotRect.Left, dirtyRect.Bottom - 16, plotRect.Width, 14),
                HorizontalAlignment.Center,
                VerticalAlignment.Center);

            canvas.DrawString(
                "Elevation (m)",
                new RectF(6, plotRect.Top + 4, plotRect.Left - 12, 18),
                HorizontalAlignment.Left,
                VerticalAlignment.Center);
        }

        private static string FormatKilometres(float value)
        {
            var roundedToOneDecimal = MathF.Round(value, 1);
            var roundedToTwoDecimals = MathF.Round(value, 2);

            return MathF.Abs(roundedToOneDecimal - roundedToTwoDecimals) < 0.0001f
                ? roundedToOneDecimal.ToString("F1")
                : roundedToTwoDecimals.ToString("F2");
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
