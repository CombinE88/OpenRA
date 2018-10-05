using System;
using System.Collections.Generic;
using System.Linq;
using OpenRA.Activities;
using OpenRA.Graphics;
using OpenRA.Mods.Common.Traits;
using OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.InfoNodes;
using OpenRA.Traits;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes.EditorNodeBrushes
{
    public class EditorNodeBrushBrush : IEditorBrush
    {
        readonly Map map;
        readonly EditorViewportControllerWidget editorWidget;
        readonly EditorActorLayer editorLayer;
        public List<EditorActorPreview> Actor = new List<EditorActorPreview>();

        OutConnection outCon;
        NodeSelectionLayer nodeSelectionLayer;
        WorldRenderer worldRenderer;
        int range;
        Action function;
        int2 worldPixel;

        public EditorNodeBrushBrush(CellPicking mode, OutConnection outConnection, EditorViewportControllerWidget editorWidget, WorldRenderer wr, Action onFinished = null)
        {
            outCon = outConnection;
            worldRenderer = wr;
            this.editorWidget = editorWidget;

            map = wr.World.Map;

            nodeSelectionLayer = wr.World.WorldActor.Trait<NodeSelectionLayer>();
            editorLayer = wr.World.WorldActor.Trait<EditorActorLayer>();

            nodeSelectionLayer.Mode = mode;

            outCon.Widget.Screen.Bgw.Visible = false;

            function = onFinished;

            if (outCon.CellArray != null && outCon.CellArray.Any())
                nodeSelectionLayer.LoadInPath(outCon.CellArray);

            if (outCon.Number != 0 && mode == CellPicking.Range)
                nodeSelectionLayer.SetRange(new WDist(range * 1024));
            else if (mode == CellPicking.Range)
            {
                nodeSelectionLayer.SetRange(new WDist(0));
            }

            if (outCon.ActorPrevs != null && outCon.ActorPrevs.Any())
            {
                nodeSelectionLayer.Actors = outCon.ActorPrevs.ToList();
            }
        }

        public void Dispose()
        {
            nodeSelectionLayer.Clear();
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
            worldPixel = worldRenderer.Viewport.ViewToWorldPx(mi.Location);
            var underCursor = editorLayer.PreviewsAt(worldPixel).MinByOrDefault(CalculateActorSelectionPriority);

            if (mi.Button != MouseButton.Left && mi.Button != MouseButton.Right)
                return false;

            if (nodeSelectionLayer.Mode == CellPicking.Actor && mi.Event == MouseInputEvent.Down && mi.Button == MouseButton.Left && underCursor != null)
            {
                if (nodeSelectionLayer.Actors.Contains(underCursor) && mi.Modifiers == Modifiers.Ctrl)
                    nodeSelectionLayer.Actors.Remove(underCursor);
                else if (!nodeSelectionLayer.Actors.Contains(underCursor))
                    nodeSelectionLayer.Actors.Add(underCursor);

                return true;
            }

            if (mi.Button == MouseButton.Right)
            {
                if (mi.Event == MouseInputEvent.Up)
                {
                    if (nodeSelectionLayer.Mode != CellPicking.Single && nodeSelectionLayer.Mode != CellPicking.Range)
                        outCon.CellArray = nodeSelectionLayer.CellRegion;
                    if ((nodeSelectionLayer.Mode == CellPicking.Single || nodeSelectionLayer.Mode == CellPicking.Range) && nodeSelectionLayer.CellRegion.Any())
                        outCon.Location = nodeSelectionLayer.CellRegion.First();
                    outCon.Widget.Screen.Bgw.Visible = true;
                    if (nodeSelectionLayer.Mode == CellPicking.Range)
                        outCon.Number = range;
                    if (nodeSelectionLayer.Actors.Any() && nodeSelectionLayer.Mode == CellPicking.Actor)
                    {
                        outCon.ActorPrevs = nodeSelectionLayer.Actors.ToArray();
                    }
                    else
                    {
                        outCon.ActorPrevs = null;
                    }

                    nodeSelectionLayer.Actors = new List<EditorActorPreview>();
                    function();
                    editorWidget.ClearBrush();
                    return true;
                }

                return false;
            }

            if (mi.Button == MouseButton.Left && mi.Modifiers == Modifiers.Shift && nodeSelectionLayer.Mode == CellPicking.Array)
            {
                nodeSelectionLayer.RemoveCell(worldRenderer.Viewport.ViewToWorld(mi.Location));
                return true;
            }

            if (mi.Button == MouseButton.Left && mi.Modifiers == Modifiers.Shift && nodeSelectionLayer.Mode == CellPicking.Path && mi.Event == MouseInputEvent.Down)
            {
                nodeSelectionLayer.RemoveCell(worldRenderer.Viewport.ViewToWorld(mi.Location));
                return true;
            }

            if (mi.Button == MouseButton.Left && nodeSelectionLayer.Mode == CellPicking.Path && mi.Event == MouseInputEvent.Down)
            {
                nodeSelectionLayer.AddCell(worldRenderer.Viewport.ViewToWorld(mi.Location));
                return true;
            }

            if (mi.Button == MouseButton.Left && nodeSelectionLayer.Mode == CellPicking.Array)
            {
                nodeSelectionLayer.AddCell(worldRenderer.Viewport.ViewToWorld(mi.Location));
                return true;
            }

            if (mi.Button == MouseButton.Left && mi.Event == MouseInputEvent.Down && nodeSelectionLayer.Mode == CellPicking.Range)
            {
                nodeSelectionLayer.Clear();
                nodeSelectionLayer.AddCell(worldRenderer.Viewport.ViewToWorld(mi.Location));
                nodeSelectionLayer.FixedCursorPosition = worldRenderer.Viewport.ViewToWorld(mi.Location);
                return true;
            }

            if (mi.Button == MouseButton.Left && nodeSelectionLayer.Mode == CellPicking.Range)
            {
                range = (nodeSelectionLayer.FixedCursorPosition - worldRenderer.Viewport.ViewToWorld(mi.Location)).Length;
                nodeSelectionLayer.SetRange(new WDist(range * 1024));
                return true;
            }

            if (mi.Button == MouseButton.Left && nodeSelectionLayer.Mode == CellPicking.Single && mi.Event == MouseInputEvent.Down)
            {
                nodeSelectionLayer.Clear();
                nodeSelectionLayer.AddCell(worldRenderer.Viewport.ViewToWorld(mi.Location));
                return true;
            }

            return false;
        }

        public void Tick()
        {
        }
    }
}