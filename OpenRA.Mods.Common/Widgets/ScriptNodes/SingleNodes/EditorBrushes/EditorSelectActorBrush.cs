#region Copyright & License Information

/*
 * Copyright 2007-2018 The OpenRA Developers (see AUTHORS)
 * This file is part of OpenRA, which is free software. It is made
 * available to you under the terms of the GNU General Public License
 * as published by the Free Software Foundation, either version 3 of
 * the License, or (at your option) any later version. For more
 * information, see COPYING.
 */

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using OpenRA.Graphics;
using OpenRA.Mods.Common.Traits;
using OpenRA.Mods.Common.Widgets.ScriptNodes;
using OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes;
using OpenRA.Traits;

namespace OpenRA.Mods.Common.Widgets
{
    public sealed class EditorSelectActorBrush : IEditorBrush
    {
        public readonly ActorInfo Actor;
        readonly EditorActorLayer editorLayer;
        readonly World world;

        int2 worldPixel;

        EditorViewportControllerWidget editorWidget;
        PipetPickActorWidget widget;

        public EditorSelectActorBrush(EditorViewportControllerWidget editorWidget, PipetPickActorWidget widget, WorldRenderer wr)
        {
            world = wr.World;
            this.editorWidget = editorWidget;
            this.widget = widget;
            editorLayer = world.WorldActor.Trait<EditorActorLayer>();
        }

        long CalculateActorSelectionPriority(EditorActorPreview actor)
        {
            var centerPixel = new int2(actor.Bounds.X, actor.Bounds.Y);
            var pixelDistance = (centerPixel - worldPixel).Length;

            // If 2+ actors have the same pixel position, then the highest appears on top.
            var worldZPosition = actor.CenterPosition.Z;

            // Sort by pixel distance then in world z position.
            return ((long)pixelDistance << 32) + worldZPosition;
        }

        public bool HandleMouseInput(MouseInput mi)
        {
            var underCursor = editorLayer.PreviewsAt(worldPixel).MinByOrDefault(CalculateActorSelectionPriority);

            if (mi.Button != MouseButton.Left && mi.Button != MouseButton.Right)
                return false;


            if (mi.Button == MouseButton.Right)
            {
                if (mi.Event == MouseInputEvent.Up)
                {
                    editorWidget.ClearBrush();
                    return true;
                }

                return false;
            }

            if (mi.Button == MouseButton.Left)
            {
                if (mi.Event == MouseInputEvent.Up)
                {
                    if (underCursor != null && underCursor.Info != null)
                    {
                        widget.PipetteActor = underCursor.Init<ActorInitializer>();
                        editorWidget.ClearBrush();
                        return true;
                    }
                }

                return false;
            }

            return true;
        }

        public void Tick()
        {
        }

        public void Dispose()
        {
        }
    }
}