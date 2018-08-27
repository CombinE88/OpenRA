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
        public ResourceTile OldResourceTile;
        public ResourceTile NewResourceTile;
        public bool Addrecource;
        public bool RemoveRecource;
        public ActorReference ActorReference;
        public bool Addactor;
        public bool RemoveActor;
        public EditorActorPreview ActorPreview;
        public bool Pasteterrain;
        public byte Hight;
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
        public List<String> HistoryLog = new List<string>();

        public EditorUndoRedoLayer(Actor self)
        {
        }

        public void Undo(World world)
        {
            if (!History.Any())
                return;

            foreach (var entry in History.Last())
            {
                if (entry.ActorReference != null && entry.Addactor)
                {
                    world.WorldActor.Trait<EditorActorLayer>().Add(entry.ActorReference);
                }

                if (entry.ActorPreview != null && entry.RemoveActor)
                {
                    world.WorldActor.Trait<EditorActorLayer>().Remove(entry.ActorPreview, true);
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
            if (HistoryLog.Any())
                HistoryLog.Remove(HistoryLog.Last());
        }
    }
}