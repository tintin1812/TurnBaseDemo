/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Gui
{
    public partial class HomeScreen : GComponent
    {
        public GLoader Bg;
        public GList ListSlot;
        public const string URL = "ui://rbw1tvvvlrn42z";

        public static HomeScreen CreateInstance()
        {
            return (HomeScreen)UIPackage.CreateObject("Gui", "HomeScreen");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            Bg = (GLoader)GetChildAt(0);
            ListSlot = (GList)GetChildAt(1);
        }
    }
}