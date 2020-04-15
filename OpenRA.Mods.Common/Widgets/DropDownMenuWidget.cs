using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using OpenRA.Widgets;

namespace OpenRA.Mods.Common.Widgets
{
    public class DropDownMenuWidget : Widget
    {
        public readonly string Background;

        private List<Widget> elementList = new List<Widget>();

        public DropDownMenuWidget(string background = "panel-black")
        {
            Background = background;
        }

        public void AddDropDownMenu(Widget element)
        {
            elementList.Add(element);

            element.Bounds = new Rectangle(
                0,
                element == elementList.First()
                    ? 0
                    : elementList[elementList.IndexOf(element) - 1].Bounds.Y +
                      elementList[elementList.IndexOf(element) - 1].Bounds.Height,
                element.Bounds.Width,
                element.Bounds.Height);

            Bounds = new Rectangle(0, 0, elementList.Max(e => e.Bounds.Width),
                elementList.Sum(e => e.Bounds.Height));

            AddChild(element);
        }

        public static void Collapse(Widget widget)
        {
            foreach (var element in widget.Children)
            {
                if (element is DropDownMenuWidget)
                {
                    element.Visible = false;
                }
                Collapse(element);
            }
        }

        public override void Draw()
        {
            WidgetUtils.DrawPanel(Background, RenderBounds);
        }
    }
}