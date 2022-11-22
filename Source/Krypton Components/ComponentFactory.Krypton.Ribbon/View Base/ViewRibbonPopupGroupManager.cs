using ComponentFactory.Krypton.Toolkit;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace ComponentFactory.Krypton.Ribbon
{
	internal class ViewRibbonPopupGroupManager : ViewManager
	{
		private KryptonRibbon _ribbon;

		private ViewDrawRibbonGroup _viewGroup;

		private NeedPaintHandler _needPaintDelegate;

		private ViewBase _focusView;

		private bool _layingOut;

		public ViewBase FocusView
		{
			get
			{
				return this._focusView;
			}
			set
			{
				if (this._focusView != value)
				{
					if (this._focusView != null)
					{
						this._focusView.LostFocus(base.Root.OwningControl);
					}
					this._focusView = value;
					if (this._focusView != null)
					{
						this._focusView.GotFocus(base.Root.OwningControl);
					}
				}
			}
		}

		public ViewRibbonPopupGroupManager(System.Windows.Forms.Control control, KryptonRibbon ribbon, ViewBase root, ViewDrawRibbonGroup viewGroup, NeedPaintHandler needPaintDelegate) : base(control, root)
		{
			Debug.Assert(ribbon != null);
			Debug.Assert(viewGroup != null);
			Debug.Assert(needPaintDelegate != null);
			this._ribbon = ribbon;
			this._viewGroup = viewGroup;
			this._needPaintDelegate = needPaintDelegate;
		}

		public override void Dispose()
		{
			this.FocusView = null;
			base.Dispose();
		}

		public override Size GetPreferredSize(IRenderer renderer, Size proposedSize)
		{
			this._ribbon.CalculatedValues.Recalculate();
			return base.GetPreferredSize(renderer, proposedSize);
		}

		public override void KeyDown(KeyEventArgs e)
		{
			if (this.FocusView != null)
			{
				this.FocusView.KeyDown(e);
			}
		}

		public override void KeyPress(KeyPressEventArgs e)
		{
			if (this.FocusView != null)
			{
				this.FocusView.KeyPress(e);
			}
		}

		public override void KeyUp(KeyEventArgs e)
		{
			if (this.FocusView != null)
			{
				base.MouseCaptured = this.FocusView.KeyUp(e);
			}
		}

		public override void Layout(ViewLayoutContext context)
		{
			if (!this._layingOut)
			{
				this._layingOut = true;
				this._ribbon.CalculatedValues.Recalculate();
				base.Layout(context);
				this._layingOut = false;
			}
		}

		public override void MouseLeave(EventArgs e)
		{
			Debug.Assert(e != null);
			if (e == null)
			{
				throw new ArgumentNullException("e");
			}
			if (this._viewGroup.Tracking)
			{
				this._viewGroup.Tracking = false;
				this._viewGroup.PerformNeedPaint(false, this._viewGroup.ClientRectangle);
			}
			base.MouseLeave(e);
		}

		public override void MouseMove(MouseEventArgs e, Point rawPt)
		{
			Debug.Assert(e != null);
			if (e == null)
			{
				throw new ArgumentNullException("e");
			}
			Rectangle clientRectangle = this._viewGroup.ClientRectangle;
			bool tracking = clientRectangle.Contains(new Point(e.X, e.Y));
			if (tracking != this._viewGroup.Tracking)
			{
				this._viewGroup.Tracking = tracking;
				this._viewGroup.PerformNeedPaint(false, this._viewGroup.ClientRectangle);
			}
			base.MouseMove(e, rawPt);
		}

		private void PerformNeedPaint(bool needLayout, Rectangle invalidRect)
		{
			if (this._needPaintDelegate != null)
			{
				this._needPaintDelegate(this, new NeedLayoutEventArgs(needLayout, invalidRect));
			}
		}
	}
}