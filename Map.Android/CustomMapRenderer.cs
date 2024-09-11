using System;
using System.Collections.Generic;
using System.Net;
using Android.Content;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Graphics;
using Android.Widget;
using Maps;
using Maps.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Maps.Android;
using System.Linq;
using System.Threading.Tasks;
using AndroidX.Annotations;

[assembly: ExportRenderer(typeof(MapEx), typeof(CustomMapRenderer))]
namespace Maps.Droid
{
    public class CustomMapRenderer : MapRenderer, GoogleMap.IInfoWindowAdapter
    {
        List<PinEx> customPins;

        public CustomMapRenderer(Context context) : base(context)
        {
        }
        MapEx MapEx;
        protected override void OnElementChanged(Xamarin.Forms.Platform.Android.ElementChangedEventArgs<Xamarin.Forms.Maps.Map> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
            {
                NativeMap.InfoWindowClick -= OnInfoWindowClick;

            }

            if (e.NewElement is MapEx)
            {
                MapEx = e.NewElement as MapEx;
                customPins = MapEx.CustomPins;
            }
        }

        protected override void OnMapReady(GoogleMap map)
        {
            base.OnMapReady(map);

            NativeMap.InfoWindowClick += OnInfoWindowClick;
            NativeMap.SetInfoWindowAdapter(this);
            string mapStyleJson = @"[
      {
        ""featureType"": ""landscape"",
        ""elementType"": ""labels"",
        ""stylers"": [{ ""visibility"": ""off"" }]
      },
      {
        ""featureType"": ""poi"",
        ""stylers"": [{ ""visibility"": ""off"" }]
      },
      {
        ""featureType"": ""transit"",
        ""stylers"": [{ ""visibility"": ""off"" }]
      }
 ]";

            NativeMap.SetMapStyle(new MapStyleOptions(mapStyleJson));
        }

        static Dictionary<string, Bitmap> Icons = new Dictionary<string, Bitmap>();
        protected override MarkerOptions CreateMarker(Pin pin)
        {
            var marker = new MarkerOptions();
            marker.SetPosition(new LatLng(pin.Position.Latitude, pin.Position.Longitude));
            //marker.SetTitle(pin.Label);
            //marker.SetSnippet(pin.Address);
            Bitmap bitmap = null;
            var iconUri = GetCustomPin(pin.Position)?.Url;
            var iconFileName = GetCustomPin(pin.Position)?.IconFileName;
            //if (iconUri == null)
            //    iconUri = "https://destopesto.blob.core.windows.net/images/fast-food.png";
            if (!string.IsNullOrWhiteSpace(iconUri))
            {
                lock (Icons)
                {
                    if (!Icons.TryGetValue(iconUri, out bitmap))
                    {

                        byte[] imageBytes = System.IO.File.ReadAllBytes(iconFileName);
                        //var webClient = new WebClient();
                        //byte[] wImageBytes = webClient.DownloadData(iconUri);

                        bitmap = BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length);
                        Icons[iconUri] = bitmap;
                    }
                }

                marker.SetIcon(BitmapDescriptorFactory.FromBitmap(bitmap));

              //  marker.SetIcon(BitmapDescriptorFactory.FromResource(Resource.Drawable.pin));
            }
            return marker;
        }

        void OnInfoWindowClick(object sender, GoogleMap.InfoWindowClickEventArgs e)
        {
            var customPin = GetCustomPin(e.Marker);
            if (customPin == null)
            {
                throw new Exception("Custom pin not found");
            }

            //if (!string.IsNullOrWhiteSpace(customPin.Url))
            //{
            //    var url = Android.Net.Uri.Parse(customPin.Url);
            //    var intent = new Intent(Intent.ActionView, url);
            //    intent.AddFlags(ActivityFlags.NewTask);
            //    Android.App.Application.Context.StartActivity(intent);
            //}
        }

        public Android.Views.View GetInfoContents(Marker marker)
        {
            //var inflater = Android.App.Application.Context.GetSystemService(Context.LayoutInflaterService) as Android.Views.LayoutInflater;
            //if (inflater != null)
            //{
            //    Android.Views.View view;

            //    var customPin = GetCustomPin(marker);
            //    if (customPin == null)
            //    {
            //        throw new Exception("Custom pin not found");
            //    }

            //    if (customPin.Name.Equals("Xamarin"))
            //    {
            //        view = inflater.Inflate(Resource.Layout.XamarinMapInfoWindow, null);
            //    }
            //    else
            //    {
            //        view = inflater.Inflate(Resource.Layout.MapInfoWindow, null);
            //    }

            //    var infoTitle = view.FindViewById<TextView>(Resource.Id.InfoWindowTitle);
            //    var infoSubtitle = view.FindViewById<TextView>(Resource.Id.InfoWindowSubtitle);

            //    if (infoTitle != null)
            //    {
            //        infoTitle.Text = marker.Title;
            //    }
            //    if (infoSubtitle != null)
            //    {
            //        infoSubtitle.Text = marker.Snippet;
            //    }

            //    return view;
            //}
            return null;
        }

        public Android.Views.View GetInfoWindow(Marker marker)
        {
            return null;
        }

        PinEx GetCustomPin(Marker annotation)
        {
            var position = new Position(annotation.Position.Latitude, annotation.Position.Longitude);
            if (position != null)
            {
                var pins = this.MapEx?.CustomPins?.ToList();
                if (pins != null)
                    foreach (var pin in pins)
                    {
                        if (pin.Position == position)
                        {
                            return pin;
                        }
                    }
            }
            return null;
        }

        PinEx GetCustomPin(Position position)
        {

            foreach (var pin in MapEx.CustomPins)
            {
                if (pin.Position == position)
                {
                    return pin;
                }
            }
            return null;
        }
    }
}

