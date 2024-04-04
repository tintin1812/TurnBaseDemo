/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Gui
{
    public partial class TextDamage : GComponent
    {
        public GTextField Label;
        public const string URL = "ui://rbw1tvvvnblsrl";

        public static TextDamage CreateInstance()
        {
            return (TextDamage)UIPackage.CreateObject("Gui", "TextDamage");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            Label = (GTextField)GetChildAt(0);
        }
    }
}