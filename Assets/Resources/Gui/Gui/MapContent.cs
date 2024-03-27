/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Gui
{
    public partial class MapContent : GComponent
    {
        public GLoader Bg;
        public GList ListSlot;
        public const string URL = "ui://rbw1tvvvtsj8r1";

        public static MapContent CreateInstance()
        {
            return (MapContent)UIPackage.CreateObject("Gui", "MapContent");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            Bg = (GLoader)GetChildAt(0);
            ListSlot = (GList)GetChildAt(1);
        }
    }
}