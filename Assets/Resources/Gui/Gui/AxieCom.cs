/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Gui
{
    public partial class AxieCom : GButton
    {
        public IndicatorMove IndicatorMove;
        public GGraph Image;
        public BarHp BarHp;
        public const string URL = "ui://rbw1tvvvprmnr4";

        public static AxieCom CreateInstance()
        {
            return (AxieCom)UIPackage.CreateObject("Gui", "AxieCom");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            IndicatorMove = (IndicatorMove)GetChildAt(0);
            Image = (GGraph)GetChildAt(1);
            BarHp = (BarHp)GetChildAt(2);
        }
    }
}