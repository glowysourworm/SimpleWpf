using System.Windows;
using System.Windows.Input;

using SimpleWpf.Extensions.Event;

namespace SimpleWpf.UI.Controls
{
    /// <summary>
    /// Handler for the MouseSelectorRectangleChangedEvent routed event
    /// </summary>
    /// <param name="topLeft">Top left of the selection rectangle</param>
    /// <param name="bottomRight">Bottom Right of the selection rectangle</param>
    /// <param name="engaged">Mouse left button is pressed</param>
    public delegate SimpleEventHandler<Point, Point, bool> MouseSelectorRectangleChangedHandler(Point topLeft, Point bottomRight, bool engaged);

    /// <summary>
    /// UIElement for handling mouse events for simple mouse rubber-band selection. The MouseDown and
    /// MouseUp events have preview hooks, here, and there is an event to listen to called 
    /// MouseSelectorChangedEvent. The height and width are set during these interactions. So,
    /// the layout of the user control would put this control in its hierarchy; and keep track of this
    /// event.
    /// </summary>
    public class MouseSelectorRectangle : UIElement
    {
        /// <summary>
        /// Event for the selector changed (this includes mouse movement while the mouse is down)
        /// </summary>
        public event MouseSelectorRectangleChangedHandler MouseSelectorChangedEvent;

        Point? _mouseDownPoint;

        public MouseSelectorRectangle()
        {
            _mouseDownPoint = null;
        }

        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseLeftButtonDown(e);

            _mouseDownPoint = e.GetPosition(this);
        }
        protected override void OnPreviewMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseLeftButtonUp(e);

            if (_mouseDownPoint != null && this.MouseSelectorChangedEvent != null)
                this.MouseSelectorChangedEvent(_mouseDownPoint.Value, e.GetPosition(this), false);

            _mouseDownPoint = null;
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (e.LeftButton == MouseButtonState.Pressed && _mouseDownPoint != null && this.MouseSelectorChangedEvent != null)
                this.MouseSelectorChangedEvent(_mouseDownPoint.Value, e.GetPosition(this), true);
        }
    }
}
