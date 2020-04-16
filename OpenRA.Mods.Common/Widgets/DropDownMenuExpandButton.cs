using System.Drawing;
using OpenRA.Widgets;

namespace OpenRA.Mods.Common.Widgets
{
    public class DropDownMenuExpandButton : ButtonWidget
    {
        DropDownMenuWidget dropMenu;

        public DropDownMenuExpandButton(ModData modData, Rectangle bounds) : base(modData)
        {
            Bounds = new Rectangle(bounds.X, bounds.Y, bounds.Width + 25, bounds.Height);

            dropMenu = new DropDownMenuWidget()
            {
                Visible = false
            };

            OnClick = () =>
            {
                DropDownMenuWidget.Collapse(Parent);
                dropMenu.Visible = true;
                dropMenu.Bounds = new Rectangle(Bounds.X + Bounds.Width, 0, dropMenu.Bounds.Width,
                    dropMenu.Bounds.Height);
            };

            AddChild(new ImageWidget()
            {
                Bounds = new Rectangle(Bounds.Width - 15, 25 / 2 - 15 / 2, 15, 15),
                GetImageName = () => "right_arrow",
                GetImageCollection = () => "scrollbar"
            });

            AddChild(dropMenu);
        }

        public void AddDropDownMenu(Widget element)
        {
            dropMenu.AddDropDownMenu(element);
        }
    }
}