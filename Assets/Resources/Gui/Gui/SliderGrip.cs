/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Gui
{
    public partial class SliderGrip : GButton
    {
        public GImage grip;
        public const string URL = "ui://rbw1tvvvtsj8r0";

        public static SliderGrip CreateInstance()
        {
            return (SliderGrip)UIPackage.CreateObject("Gui", "SliderGrip");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            grip = (GImage)GetChildAt(0);
        }
    }
}