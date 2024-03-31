/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Gui
{
    public partial class LoadingScreen : GComponent
    {
        public GTextField TextLoading;
        public const string URL = "ui://rbw1tvvvjluwr7";

        public static LoadingScreen CreateInstance()
        {
            return (LoadingScreen)UIPackage.CreateObject("Gui", "LoadingScreen");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            TextLoading = (GTextField)GetChildAt(1);
        }
    }
}