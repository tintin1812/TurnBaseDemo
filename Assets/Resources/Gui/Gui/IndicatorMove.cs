/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Gui
{
    public partial class IndicatorMove : GComponent
    {
        public GImage move_01;
        public const string URL = "ui://rbw1tvvvtk87ru";

        public static IndicatorMove CreateInstance()
        {
            return (IndicatorMove)UIPackage.CreateObject("Gui", "IndicatorMove");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            move_01 = (GImage)GetChildAt(0);
        }
    }
}