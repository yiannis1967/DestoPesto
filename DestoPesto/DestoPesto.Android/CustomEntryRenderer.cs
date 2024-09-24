using Android.Graphics.Drawables;
using Android.Graphics;
using Android.Widget;
using System.ComponentModel;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms;
using DestoPesto.Extensions;
using DestoPesto.Droid;

[assembly: ExportRenderer(typeof(EntryEx), typeof(CustomEntryRenderer))]
namespace DestoPesto.Droid
{
    public class CustomEntryRenderer : EntryRenderer
    {
        EntryEx entry;
        public CustomEntryRenderer(Android.Content.Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);
            entry = Element as EntryEx;
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            //var nativeEditText = (EditText)Control;
            //var shape = new ShapeDrawable(new Android.Graphics.Drawables.Shapes.RectShape());
            //shape.Paint.Color = entry.BorderColor.ToAndroid(); //entry.BorderColor is the 'BorderColor' property of the custom entry class  
            //shape.Paint.SetStyle(Paint.Style.Stroke);
            //shape.Paint.StrokeWidth = 10;
            //nativeEditText.Background = shape;
        }
    }
}