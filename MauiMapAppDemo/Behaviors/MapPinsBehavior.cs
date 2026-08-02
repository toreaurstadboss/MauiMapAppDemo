using MauiMapAppDemo.ViewModels;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;
using System.Windows.Input;

namespace MauiMapAppDemo.Behaviors
{

    public class MapPinsBehavior : Behavior<Microsoft.Maui.Controls.Maps.Map>
    {

        private Microsoft.Maui.Controls.Maps.Map? _map;
        private EventHandler<MapClickedEventArgs>? _mapClickedHandler;

        private Microsoft.Maui.Controls.Maps.Polyline? _measurementLine;
        private Microsoft.Maui.Controls.Maps.Polygon? _matrikkelPolygon;

        private Microsoft.Maui.Controls.Maps.Pin? _startPin;
        private Microsoft.Maui.Controls.Maps.Pin? _endPin;


        public static readonly BindableProperty IsMatrikkelModeProperty =
            BindableProperty.Create(
                nameof(IsMatrikkelMode),
                typeof(bool),
                typeof(MapPinsBehavior),
                false);

        public bool IsMatrikkelMode
        {
            get => (bool)GetValue(IsMatrikkelModeProperty);
            set => SetValue(IsMatrikkelModeProperty, value);
        }

        public static readonly BindableProperty MatrikkelPolygonPathProperty =
            BindableProperty.Create(
                nameof(MatrikkelPolygonPath),
                typeof(Location[]),
                typeof(MapPinsBehavior),
                defaultValue: Array.Empty<Location>(),
                propertyChanged: OnMatrikkelPolygonPathChanged);

        public IEnumerable<Location> MatrikkelPolygonPath
        {
            get => (IEnumerable<Location>)GetValue(MatrikkelPolygonPathProperty);
            set => SetValue(MatrikkelPolygonPathProperty, value);
        }


        public static readonly BindableProperty IsMeasuringModeProperty =
            BindableProperty.Create(
                nameof(IsMeasuringMode),
                typeof(bool),
                typeof(MapPinsBehavior),
                false,
                propertyChanged: OnMeasurementStateChanged);

        public bool IsMeasuringMode
        {
            get => (bool)GetValue(IsMeasuringModeProperty);
            set => SetValue(IsMeasuringModeProperty, value);
        }


        public static readonly BindableProperty MeasureStartProperty =
            BindableProperty.Create(
                nameof(MeasureStart),
                typeof(Location),
                typeof(MapPinsBehavior),
                propertyChanged: OnMeasurementChanged
            );

        public Location MeasureStart
        {
            get => (Location)GetValue(MeasureStartProperty);
            set => SetValue(MeasureStartProperty, value);
        }

        public static readonly BindableProperty MeasureEndProperty =
            BindableProperty.Create(
                nameof(MeasureEnd),
                typeof(Location),
                typeof(MapPinsBehavior),
                propertyChanged: OnMeasurementChanged
            );

        public Location MeasureEnd
        {
            get => (Location)GetValue(MeasureEndProperty);
            set => SetValue(MeasureEndProperty, value);
        }


        public static readonly BindableProperty CenterProperty =
         BindableProperty.Create(
             nameof(Center),
             typeof(Location),
             typeof(MapPinsBehavior),
             propertyChanged: OnCenterChanged
             );

        public Location Center
        {
            get => (Location)GetValue(CenterProperty);
            set => SetValue(CenterProperty, value);
        }

        public static readonly BindableProperty PinItemsProperty =
            BindableProperty.Create(
                nameof(PinItems),
                typeof(IEnumerable<MapPinModel>),
                typeof(MapPinsBehavior),
                propertyChanged: OnPinItemsChanged
                );

        public IEnumerable<MapPinModel>? PinItems
        {
            get => (IEnumerable<MapPinModel>?)GetValue(PinItemsProperty);
            set => SetValue(PinItemsProperty, value);
        }

        public static readonly BindableProperty MapClickedCommandProperty =
            BindableProperty.Create(
                nameof(MapClickedCommand),
                typeof(ICommand),
                typeof(MapPinsBehavior),
                defaultValue: null);

        public ICommand MapClickedCommand
        {
            get => (ICommand)GetValue(MapClickedCommandProperty);
            set => SetValue(MapClickedCommandProperty, value);
        }

        public static readonly BindableProperty PinClickedCommandProperty =
           BindableProperty.Create(
               nameof(PinClickedCommand),
               typeof(ICommand),
               typeof(MapPinsBehavior),
               defaultValue: null);

        public ICommand PinClickedCommand
        {
            get => (ICommand)GetValue(PinClickedCommandProperty);
            set => SetValue(PinClickedCommandProperty, value);
        }

        protected override void OnAttachedTo(Microsoft.Maui.Controls.Maps.Map bindable)
        {
            _map = bindable;

            WireUpMapClickedCommand(bindable);

            base.OnAttachedTo(bindable);

            RefreshPins();
            RefreshMatrikkelPolygon();

            if (Center is not null)
            {
                _map.MoveToRegion(MapSpan.FromCenterAndRadius(Center, Distance.FromKilometers(8)));
            }
        }

        private void WireUpMapClickedCommand(Microsoft.Maui.Controls.Maps.Map map)
        {
            _mapClickedHandler = (object? sender, MapClickedEventArgs e) =>
            {
                if (MapClickedCommand?.CanExecute(e.Location) == true)
                {
                    MapClickedCommand.Execute(e.Location);
                }
            };

            map.MapClicked += _mapClickedHandler;
        }

        protected override void OnDetachingFrom(Microsoft.Maui.Controls.Maps.Map bindable)
        {
            if (_mapClickedHandler != null)
            {
                bindable.MapClicked -= _mapClickedHandler;
                _mapClickedHandler = null;
            }

            ClearMeasurementGraphics();
            ClearMatrikkelPolygon();
            _map = null;

            base.OnDetachingFrom(bindable);
        }

        private static void OnCenterChanged(
            BindableObject bindable,
            object oldValue,
            object newValue)
        {
            var behavior = (MapPinsBehavior)bindable;

            if (behavior._map is not null && newValue is Location location)
            {
                behavior._map.MoveToRegion(MapSpan.FromCenterAndRadius(location,
                    Distance.FromKilometers(8)));
            }
        }

        private static void OnMeasurementChanged(
            BindableObject bindable,
            object oldValue,
            object newValue)
        {
            ((MapPinsBehavior)bindable).RefreshMeasurementLine();
        }


        private static void OnMeasurementStateChanged(
            BindableObject bindable,
            object oldValue,
            object newValue)
        {
            var behavior = (MapPinsBehavior)bindable;

            if (newValue is not bool isMeasuringMode || !isMeasuringMode)
            {
                behavior.ClearMeasurementGraphics();
            }

            behavior.RefreshMatrikkelPolygon();
        }

        private static void OnMatrikkelPolygonPathChanged(
            BindableObject bindable,
            object oldValue,
            object newValue)
        {
            ((MapPinsBehavior)bindable).RefreshMatrikkelPolygon();
        }


        private void RefreshMeasurementLine()
        {
            if (_map == null)
            {
                return;
            }

            ClearMeasurementGraphics();

            if (MeasureStart == null)
            {
                return;
            }
            else
            {
                _startPin = new MeasurementPin
                {
                    Label = "Start",
                    Address = "Measurement Start",
                    Location = MeasureStart,
                    IconResourceName = "startmarkerv2"
                };

                _map.Pins.Add(_startPin);
            }

            if (MeasureEnd == null)
            {
                return;
            }
            else
            {


                _endPin = new MeasurementPin
                {
                    Label = "End",
                    Address = "Measurement End",
                    Location = MeasureEnd,
                    IconResourceName = "endmarkerv2"
                };

                _map.Pins.Add(_endPin);
            }

            _measurementLine = new Polyline
            {
                StrokeColor = Colors.Red,
                StrokeWidth = 5
            };

            _measurementLine.Geopath.Add(MeasureStart);
            _measurementLine.Geopath.Add(MeasureEnd);

            _map.MapElements.Add(_measurementLine);
        }

        private static void OnPinItemsChanged(
            BindableObject bindable,
            object oldValue,
            object newValue)
        {
            ((MapPinsBehavior)bindable).RefreshPins();
        }

        private void ClearMeasurementGraphics()
        {
            if (_map == null)
            {
                return;
            }

            if (_startPin != null)
            {
                _map.Pins.Remove(_startPin);
            }

            if (_endPin != null)
            {
                _map.Pins.Remove(_endPin);
            }

            if (_measurementLine != null)
            {
                _map.MapElements.Remove(_measurementLine);
            }

            _startPin = null;
            _endPin = null;
            _measurementLine = null;
        }

        private void ClearMatrikkelPolygon()
        {
            if (_map == null)
            {
                return;
            }

            if (_matrikkelPolygon != null)
            {
                _map.MapElements.Remove(_matrikkelPolygon);
            }

            _matrikkelPolygon = null;
        }

        private void RefreshMatrikkelPolygon()
        {
            if (_map?.MapElements == null)
            {
                return;
            }

            ClearMatrikkelPolygon();

            if (!IsMatrikkelMode || !MatrikkelPolygonPath.Any())
            {
                return;
            }

            var polygon = new Polygon
            {
                StrokeColor = Colors.Red,
                StrokeWidth = 5,
                FillColor = Color.FromArgb("#22FF0000")
            };

            foreach (var location in MatrikkelPolygonPath)
            {
                polygon.Geopath.Add(location);
            }

            if (polygon.Geopath.Count < 3)
            {
                return;
            }

            var firstLocation = polygon.Geopath.First();
            var lastLocation = polygon.Geopath.Last();

            if (firstLocation.Latitude != lastLocation.Latitude || firstLocation.Longitude != lastLocation.Longitude)
            {
                polygon.Geopath.Add(firstLocation);
            }

            _matrikkelPolygon = polygon;
            _map.MapElements.Add(polygon);
        }

        private void RefreshPins()
        {
            if (_map is null || PinItems is null)
                return;

            _map.Pins.Clear();

            foreach (var item in PinItems.OfType<MapPinModel>())
            {
                var pin = new Pin
                {
                    Label = item.Label,
                    Address = item.Address,
                    Location = new Location(
                        item.Latitude,
                        item.Longitude)
                };

                pin.MarkerClicked += (_, _) =>
                {
                    if (PinClickedCommand?.CanExecute(item) == true)
                    {
                        PinClickedCommand.Execute(item);
                    }
                };

                _map.Pins.Add(pin);
            }
        }




    }
}
