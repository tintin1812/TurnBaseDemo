/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Gui
{
    public partial class HomeScreen : GComponent
    {
        public Map Map;
        public GLoader Bar;
        public GSlider SliderZoom;
        public Button BtPreview;
        public Button BtNext;
        public const string URL = "ui://rbw1tvvvlrn42z";

        public static HomeScreen CreateInstance()
        {
            return (HomeScreen)UIPackage.CreateObject("Gui", "HomeScreen");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            Map = (Map)GetChildAt(0);
            Bar = (GLoader)GetChildAt(1);
            SliderZoom = (GSlider)GetChildAt(2);
            BtPreview = (Button)GetChildAt(3);
            BtNext = (Button)GetChildAt(4);
        }
    }
}