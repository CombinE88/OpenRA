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
using OpenRA.Traits;

namespace OpenRA.Mods.Common.Traits
{
    public class EditorAction
    {
        public CPos Position;
        public byte Index;
        public ushort Type;
        public ResourceTile NewResourceTile;
        public ActorReference ActorReference;
        public EditorActorPreview ActorPreview;
        public byte Hight;
        public EditorAction Action;

        public bool Addrecource;
        public bool RemoveRecource;
        public bool Addactor;
        public bool RemoveActor;
        public bool Pasteterrain;
        public bool ReplacePreview;
    }

    public class EditorUndoRedoLayerInfo : ITraitInfo
    {
        public object Create(ActorInitializer init)
        {
            return new EditorUndoRedoLayer(init.Self);
        }
    }

    public class EditorUndoRedoLayer
    {
        public List<EditorAction[]> History = new List<EditorAction[]>();

        public EditorUndoRedoLayer(Actor self)
        {
        }

        public void ReplaceBuffer(EditorActorPreview preview, EditorAction selfaction)
        {
            foreach (var actionscollections in History)
            {
                foreach (var action in actionscollections)
                {
                    if (action.ActorPreview == preview)
                    {
                        selfaction.Action = action;
                        selfaction.ReplacePreview = true;
                    }
                }
            }
        }

        public void ReInsertBuffered(EditorAction editorAction, EditorActorPreview preview)
        {
            foreach (var actionscollections in History)
            {
                foreach (var action in actionscollections)
                {
                    if (editorAction == action)
                    {
                        action.ActorPreview = preview;
                    }
                }
            }
        }

        public void Undo(World world)
        {
            if (!History.Any())
                return;

            foreach (var entry in History.Last())
            {
                if (entry.ActorPreview != null && entry.RemoveActor)
                {
                    ReplaceBuffer(entry.ActorPreview, entry);
                    world.WorldActor.Trait<EditorActorLayer>().Remove(entry.ActorPreview, true);
                }

                if (entry.ActorReference != null && entry.Addactor)
                {
                    var actor = world.WorldActor.Trait<EditorActorLayer>().Add(entry.ActorReference);
                    if (entry.ReplacePreview && entry.Action != null)
                        ReInsertBuffered(entry.Action, actor);
                }

                if (entry.RemoveRecource)
                {
                    var mapResources = world.Map.Resources;
                    mapResources[entry.Position] = new ResourceTile();
                }

                if (entry.Addrecource)
                {
                    world.Map.Resources[entry.Position] = new ResourceTile(entry.NewResourceTile.Type, entry.NewResourceTile.Index);
                }

                if (entry.Pasteterrain)
                {
                    var mapTiles = world.Map.Tiles;
                    var mapHeight = world.Map.Height;
                    mapTiles[entry.Position] = new TerrainTile(entry.Type, entry.Index);
                    mapHeight[entry.Position] = entry.Hight;
                }
            }

            History.Remove(History.Last());
        }
    }
}