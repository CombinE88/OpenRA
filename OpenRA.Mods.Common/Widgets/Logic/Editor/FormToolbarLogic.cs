using System.Collections.Generic;
using System.Linq;
using OpenRA.Graphics;
using OpenRA.Mods.Common.Traits;
using OpenRA.Server;
using OpenRA.Widgets;

namespace OpenRA.Mods.Common.Widgets.Logic
{
    public class FormToolbarLogic : ChromeLogic
    {
        string methode = "none";

        SliderWidget brushslider;

        EditorBrushLayer ebl;

        EditorViewportControllerWidget editor;
        MouseInput mouseInput;

        [ObjectCreator.UseCtor]
        public FormToolbarLogic(Widget widget, World world, WorldRenderer worldRenderer)
        {
            ebl = world.WorldActor.Trait<EditorBrushLayer>();

            editor = widget.Parent.Get<EditorViewportControllerWidget>("MAP_EDITOR");

            brushslider = widget.Get<SliderWidget>("BRUSHSIZE_SLIDER");
            brushslider.OnChange += x => { Update(); };

            var methsquare = widget.Get<ButtonWidget>("SLECET_SQUARE_BUTTON");
            methsquare.OnClick = () =>
            {
                methode = "square";
                Update();
            };

            var methfree = widget.Get<ButtonWidget>("SLECET_FREE_BUTTON");
            methfree.OnClick = () =>
            {
                methode = "free";
                Update();
            };

            var methcircle = widget.Get<ButtonWidget>("SLECET_CIRCLE_BUTTON");
            methcircle.OnClick = () =>
            {
                methode = "circle";
                Update();
            };

            editor.SetBrush(new EditorToolbarBrush(editor, worldRenderer));
        }

        void Update()
        {
            ebl.Brushsize = (int) brushslider.Value;
            ebl.Methode = methode;
        }
    }
}