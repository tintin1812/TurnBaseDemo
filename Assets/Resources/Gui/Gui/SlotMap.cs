/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Gui
{
    public partial class SlotMap : GButton
    {
        public GImage Bg;
        public GImage BgWall;
        public GGraph Image;
        public GTextField Number;
        public GGraph Hightlight;
        public const string URL = "ui://rbw1tvvvimznqs";

        public static SlotMap CreateInstance()
        {
            return (SlotMap)UIPackage.CreateObject("Gui", "SlotMap");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            Bg = (GImage)GetChildAt(0);
            BgWall = (GImage)GetChildAt(1);
            Image = (GGraph)GetChildAt(2);
            Number = (GTextField)GetChildAt(3);
            Hightlight = (GGraph)GetChildAt(4);
        }
    }
}