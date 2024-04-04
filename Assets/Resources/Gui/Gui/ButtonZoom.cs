/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Gui
{
    public partial class ButtonZoom : GButton
    {
        public GImage lightShort;
        public GImage hightLight;
        public const string URL = "ui://rbw1tvvvtk87rr";

        public static ButtonZoom CreateInstance()
        {
            return (ButtonZoom)UIPackage.CreateObject("Gui", "ButtonZoom");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            lightShort = (GImage)GetChildAt(0);
            hightLight = (GImage)GetChildAt(1);
        }
    }
}