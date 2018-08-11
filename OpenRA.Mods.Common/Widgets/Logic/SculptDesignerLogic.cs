using System;
using System.Drawing;
using System.Linq;
using OpenRA.Graphics;
using OpenRA.Mods.Common.Traits;
using OpenRA.Widgets;

namespace OpenRA.Mods.Common.Widgets.Logic
{
    public class SculptDesignerLogic : ChromeLogic
    {
        SliderWidget corners;
        ScrollPanelWidget sculptsList;
        ScrollItemWidget sculptsTemplate;
        TextFieldWidget xPosition;
        TextFieldWidget yPosition;
        ColorMixerWidget colorMixer;
        SliderWidget alpha;
        SliderWidget radius;
        SliderWidget rotation;
        SliderWidget sclicer;
        SliderWidget sclicerradius;
        SliderWidget slicerrotation;

        Sculptlayer layer;
        string current;
        int nextSculpt;

        [ObjectCreator.UseCtor]
        public SculptDesignerLogic(Widget widget, World world, WorldRenderer worldRenderer)
        {
            corners = widget.Get<SliderWidget>("CORNERS_SLIDER");
            corners.OnChange += x => { UpdateSculpt(); };

            radius = widget.Get<SliderWidget>("RADIUS_SLIDER");
            radius.OnChange += x => { UpdateSculpt(); };

            rotation = widget.Get<SliderWidget>("ROTATION_SLIDER");
            rotation.OnChange += x => { UpdateSculpt(); };

            sclicer = widget.Get<SliderWidget>("SCLICER_SLIDER");
            sclicer.OnChange += x => { UpdateSculpt(); };

            sclicerradius = widget.Get<SliderWidget>("SCLICER_RADIUS_SLIDER");
            sclicerradius.OnChange += x => { UpdateSculpt(); };

            slicerrotation = widget.Get<SliderWidget>("SCLICER_ROTATION_SLIDER");
            slicerrotation.OnChange += x => { UpdateSculpt(); };

            xPosition = widget.Get<TextFieldWidget>("X_TEXTFIELD");
            xPosition.OnTextEdited = () => { UpdateSculpt(); };

            yPosition = widget.Get<TextFieldWidget>("Y_TEXTFIELD");
            yPosition.OnTextEdited = () => { UpdateSculpt(); };

            var centerButton = widget.Get<ButtonWidget>("CENTER_BUTTON");
            centerButton.OnClick = () =>
            {
                xPosition.Text = (world.Map.MapSize.X / 2).ToString();
                yPosition.Text = (world.Map.MapSize.Y / 2).ToString();
                UpdateSculpt();
            };

            var centerXButton = widget.Get<ButtonWidget>("CENTERX_BUTTON");
            centerXButton.OnClick = () =>
            {
                xPosition.Text = (world.Map.MapSize.X / 2).ToString();
                UpdateSculpt();
            };

            var centerYButton = widget.Get<ButtonWidget>("CENTERY_BUTTON");
            centerYButton.OnClick = () =>
            {
                yPosition.Text = (world.Map.MapSize.Y / 2).ToString();
                UpdateSculpt();
            };

            var createbutton = widget.Get<ButtonWidget>("CREATE_BUTTON");
            createbutton.OnClick = () => CreateSculpt(world);

            var deletebutton = widget.Get<ButtonWidget>("DELETE_BUTTON");
            deletebutton.OnClick = () => DeleteSculpt();

            var clonebutton = widget.Get<ButtonWidget>("CLONE_BUTTON");
            clonebutton.OnClick = () => CloneSculpt(world);

            var remx5 = widget.Get<ButtonWidget>("SHAPE_X_5_NEGATIVE");
            remx5.OnClick = () => ShiftSculptXY(-5, 0);

            var remx1 = widget.Get<ButtonWidget>("SHAPE_X_1_NEGATIVE");
            remx1.OnClick = () => ShiftSculptXY(-1, 0);

            var addx5 = widget.Get<ButtonWidget>("SHAPE_X_1_POSITIVE");
            addx5.OnClick = () => ShiftSculptXY(1, 0);

            var addx1 = widget.Get<ButtonWidget>("SHAPE_X_5_POSITIVE");
            addx1.OnClick = () => ShiftSculptXY(5, 0);

            var remY5 = widget.Get<ButtonWidget>("SHAPE_Y_5_NEGATIVE");
            remY5.OnClick = () => ShiftSculptXY(0, -5);

            var remY1 = widget.Get<ButtonWidget>("SHAPE_Y_1_NEGATIVE");
            remY1.OnClick = () => ShiftSculptXY(0, -1);

            var addy1 = widget.Get<ButtonWidget>("SHAPE_Y_1_POSITIVE");
            addy1.OnClick = () => ShiftSculptXY(0, 1);

            var addy5 = widget.Get<ButtonWidget>("SHAPE_Y_5_POSITIVE");
            addy5.OnClick = () => ShiftSculptXY(0, 5);

            sculptsList = widget.Get<ScrollPanelWidget>("SCULPTS_LIST");
            sculptsTemplate = sculptsList.Get<ScrollItemWidget>("SCULPTS_TEMPLATE");
            sculptsList.RemoveChild(sculptsTemplate);

            colorMixer = widget.Get<ColorMixerWidget>("MIXER");
            colorMixer.OnChange += () => UpdateSculpt();

            var hueSlider = widget.Get<HueSliderWidget>("HUE");
            hueSlider.OnChange += x => colorMixer.Set(hueSlider.Value);

            alpha = widget.Get<SliderWidget>("TRANSPARENCY_SLIDER");
            alpha.OnChange += x => { UpdateSculpt(); };

            layer = world.WorldActor.Trait<Sculptlayer>();
        }

        void CreateSculpt(World world)
        {


            var sculpt = new Sculpt(
                4,
                10,
                new CPos(world.Map.MapSize.X / 2, world.Map.MapSize.Y / 2),
                45,
                Color.FromArgb(255, 255, 255, 255),
                world.Map.Grid.TileSize,
                1,
                0,
                0);

            var name = "Shape: " + ++nextSculpt;

            var sculptItem = ScrollItemWidget.Setup(
                sculptsTemplate,
                () => current == name,
                () =>
                {
                    current = name;
                    UpdatedUiSelectedSculpt();
                });

            sculptItem.Get<LabelWidget>("SCULPT_LABEL").GetText = () => name;
            sculptItem.Get<LabelWidget>("SCULPT_LABEL").GetColor = () => Color.FromArgb((int)(layer.Sculpts[name].Color.ToArgb() | 0xff000000));
            sculptsList.AddChild(sculptItem);

            layer.Sculpts.Add(name, sculpt);
            current = name;
            UpdatedUiSelectedSculpt();
        }

        void CloneSculpt(World world)
        {
            if (current == null || !layer.Sculpts.ContainsKey(current))
                return;
;
            var sculpt = new Sculpt(
                layer.Sculpts[current].Corner,
                layer.Sculpts[current].Radius,
                new CPos(world.Map.MapSize.X / 2, world.Map.MapSize.Y / 2),
                layer.Sculpts[current].Rot,
                layer.Sculpts[current].Color,
                world.Map.Grid.TileSize,
                layer.Sculpts[current].Slice,
                layer.Sculpts[current].Sliceradius,
                layer.Sculpts[current].Slicerot);

            var name = "Shape: " + ++nextSculpt;

            var sculptItem = ScrollItemWidget.Setup(
                sculptsTemplate,
                () => current == name,
                () =>
                {
                    current = name;
                    UpdatedUiSelectedSculpt();
                });

            sculptItem.Get<LabelWidget>("SCULPT_LABEL").GetText = () => name;
            sculptItem.Get<LabelWidget>("SCULPT_LABEL").GetColor = () => Color.FromArgb((int)(layer.Sculpts[name].Color.ToArgb() | 0xff000000));
            sculptsList.AddChild(sculptItem);

            layer.Sculpts.Add(name, sculpt);
            current = name;
            UpdatedUiSelectedSculpt();
        }

        void DeleteSculpt()
        {
            if (current == null || !layer.Sculpts.ContainsKey(current))
                return;

            sculptsList.RemoveChild(sculptsList.Children.Find(sculptItem => sculptItem.Get<LabelWidget>("SCULPT_LABEL").GetText() == current));

            sculptsList.Layout.AdjustChildren();

            layer.Sculpts.Remove(current);
            current = null;
        }

        void UpdatedUiSelectedSculpt()
        {
            if (current == null || !layer.Sculpts.ContainsKey(current))
                return;

            var sculpt = layer.Sculpts[current];

            corners.Value = sculpt.Corner;
            radius.Value = sculpt.Radius;
            rotation.Value = sculpt.Rot;
            sclicer.Value = sculpt.Slice;
            sclicerradius.Value = sculpt.Sliceradius;
            slicerrotation.Value = sculpt.Slicerot;

            xPosition.Text = sculpt.Pos.X.ToString();
            yPosition.Text = sculpt.Pos.Y.ToString();
            alpha.Value = sculpt.Color.A;
            colorMixer.Set(HSLColor.FromRGB(sculpt.Color.R, sculpt.Color.G, sculpt.Color.B));
        }

        void ShiftSculptXY(int x, int y)
        {
            if (current == null || !layer.Sculpts.ContainsKey(current))
                return;

            var sculpt = layer.Sculpts[current];

            sculpt.Pos += new CVec(x, y);

            UpdatedUiSelectedSculpt();
        }

        void UpdateSculpt()
        {
            if (current == null || !layer.Sculpts.ContainsKey(current))
                return;

            var sculpt = layer.Sculpts[current];

            sculpt.Corner = (int)corners.Value;
            sculpt.Radius = (int)radius.Value;
            sculpt.Rot = (int)rotation.Value;
            sculpt.Slice = (int)sclicer.Value;
            sculpt.Sliceradius = (int)sclicerradius.Value;
            sculpt.Slicerot = (int)slicerrotation.Value;

            if (xPosition.Text != "" && yPosition.Text != "")
                sculpt.Pos = new CPos(int.Parse(xPosition.Text), int.Parse(yPosition.Text));

            sculpt.Color = colorMixer.Color.RGB;
            sculpt.Color = Color.FromArgb((int)alpha.Value, sculpt.Color.R, sculpt.Color.G, sculpt.Color.B);
        }
    }
}