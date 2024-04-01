/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Gui
{
    public partial class BarHp : GProgressBar
    {
        public GImage Bg;
        public GTextField TitleSub;
        public const string URL = "ui://rbw1tvvvnblsrk";

        public static BarHp CreateInstance()
        {
            return (BarHp)UIPackage.CreateObject("Gui", "BarHp");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            Bg = (GImage)GetChildAt(0);
            TitleSub = (GTextField)GetChildAt(2);
        }
    }
}