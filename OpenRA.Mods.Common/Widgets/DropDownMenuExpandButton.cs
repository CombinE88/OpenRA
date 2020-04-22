using System.Drawing;
using OpenRA.Widgets;

namespace OpenRA.Mods.Common.Widgets
{
    public class DropDownMenuExpandButton : ButtonWidget
    {
        public DropDownMenuWidget DropMenu;

        public DropDownMenuExpandButton(ModData modData, Rectangle bounds) : base(modData)
        {
            Bounds = new Rectangle(bounds.X, bounds.Y, bounds.Width + 25, bounds.Height);

            DropMenu = new DropDownMenuWidget()
            {
                Visible = false
            };

            OnClick = () =>
            {
                DropDownMenuWidget.Collapse(Parent);
                DropMenu.Visible = true;
                DropMenu.Bounds = new Rectangle(Bounds.X + Bounds.Width, 0, DropMenu.Bounds.Width,
                    DropMenu.Bounds.Height);
            };

            AddChild(new ImageWidget()
            {
                Bounds = new Rectangle(Bounds.Width - 15, 25 / 2 - 15 / 2, 15, 15),
                GetImageName = () => "right_arrow",
                GetImageCollection = () => "scrollbar"
            });

            AddChild(DropMenu);
        }

        public void AddDropDownMenu(Widget element)
        {
            DropMenu.AddDropDownMenu(element);
        }
    }
}