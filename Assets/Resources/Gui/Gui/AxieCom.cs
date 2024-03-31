/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Gui
{
    public partial class AxieCom : GButton
    {
        public GGraph Image;
        public const string URL = "ui://rbw1tvvvprmnr4";

        public static AxieCom CreateInstance()
        {
            return (AxieCom)UIPackage.CreateObject("Gui", "AxieCom");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            Image = (GGraph)GetChildAt(0);
        }
    }
}