/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Gui
{
    public partial class Button : GButton
    {
        public GImage lightShort;
        public GImage hightLight;
        public const string URL = "ui://rbw1tvvvx1hkqt";

        public static Button CreateInstance()
        {
            return (Button)UIPackage.CreateObject("Gui", "Button");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            lightShort = (GImage)GetChildAt(0);
            hightLight = (GImage)GetChildAt(1);
        }
    }
}