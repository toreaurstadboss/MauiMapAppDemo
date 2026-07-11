using MauiMapAppDemo.ViewModels;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;
using System.Net.NetworkInformation;
using System.Windows.Input;

namespace MauiMapAppDemo.Behaviors
{

    public class MapPinsBehavior : Behavior<Microsoft.Maui.Controls.Maps.Map>
    {

        private Microsoft.Maui.Controls.Maps.Map? _map;

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

            if (Center is not null)
            {
                _map.MoveToRegion(MapSpan.FromCenterAndRadius(Center, Distance.FromKilometers(8)));
            }
        }

        private void WireUpMapClickedCommand(Microsoft.Maui.Controls.Maps.Map map)
        {
            map.MapClicked += (object? sender, MapClickedEventArgs e) =>
            {
                if (MapClickedCommand?.CanExecute(e.Location) == true)
                {
                    MapClickedCommand.Execute(e.Location);
                }
            };
        }

        protected override void OnDetachingFrom(Microsoft.Maui.Controls.Maps.Map bindable)
        {
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

        private static void OnPinItemsChanged(
            BindableObject bindable,
            object oldValue,
            object newValue)
        {
            ((MapPinsBehavior)bindable).RefreshPins();
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
