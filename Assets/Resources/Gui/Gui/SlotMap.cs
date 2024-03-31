/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Gui
{
    public partial class SlotMap : GButton
    {
        public GGraph Hightlight;
        public GGraph Bg;
        public GGraph Image;
        public GTextField Number;
        public const string URL = "ui://rbw1tvvvimznqs";

        public static SlotMap CreateInstance()
        {
            return (SlotMap)UIPackage.CreateObject("Gui", "SlotMap");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            Hightlight = (GGraph)GetChildAt(0);
            Bg = (GGraph)GetChildAt(1);
            Image = (GGraph)GetChildAt(2);
            Number = (GTextField)GetChildAt(3);
        }
    }
}