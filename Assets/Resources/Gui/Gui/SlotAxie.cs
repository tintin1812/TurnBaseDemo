/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Gui
{
    public partial class SlotAxie : GButton
    {
        public GGraph Bg;
        public GGraph Image;
        public const string URL = "ui://rbw1tvvvimznqs";

        public static SlotAxie CreateInstance()
        {
            return (SlotAxie)UIPackage.CreateObject("Gui", "SlotAxie");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            Bg = (GGraph)GetChildAt(0);
            Image = (GGraph)GetChildAt(1);
        }
    }
}