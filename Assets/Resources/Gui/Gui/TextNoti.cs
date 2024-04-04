/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Gui
{
    public partial class TextNoti : GComponent
    {
        public GImage BG;
        public GTextField Label;
        public const string URL = "ui://rbw1tvvvtk87rw";

        public static TextNoti CreateInstance()
        {
            return (TextNoti)UIPackage.CreateObject("Gui", "TextNoti");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            BG = (GImage)GetChildAt(0);
            Label = (GTextField)GetChildAt(1);
        }
    }
}