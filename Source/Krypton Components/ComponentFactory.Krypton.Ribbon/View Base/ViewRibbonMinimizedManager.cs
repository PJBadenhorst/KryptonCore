using ComponentFactory.Krypton.Toolkit;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace ComponentFactory.Krypton.Ribbon
{
	internal class ViewRibbonMinimizedManager : ViewManager
	{
		private KryptonRibbon _ribbon;

		private ViewDrawRibbonGroupsBorderSynch _viewGroups;

		private ViewDrawRibbonGroup _activeGroup;

		private NeedPaintHandler _needPaintDelegate;

		private bool _minimizedMode;

		private bool _active;

		private bool _layingOut;

		private ViewBase _focusView;

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

		public ViewRibbonMinimizedManager(KryptonRibbon control, ViewDrawRibbonGroupsBorderSynch viewGroups, ViewBase root, bool minimizedMode, NeedPaintHandler needPaintDelegate) : base(control, root)
		{
			Debug.Assert(viewGroups != null);
			Debug.Assert(root != null);
			Debug.Assert(needPaintDelegate != null);
			this._ribbon = control;
			this._viewGroups = viewGroups;
			this._needPaintDelegate = needPaintDelegate;
			this._active = true;
			this._minimizedMode = minimizedMode;
		}

		public void Active()
		{
			this._active = true;
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

		public void Inactive()
		{
			if (this._active)
			{
				this.MouseLeave(EventArgs.Empty);
				this._active = false;
			}
		}

		public override void KeyDown(KeyEventArgs e)
		{
			if (this.FocusView == null)
			{
				this._ribbon.MinimizedKeyDown(e.KeyData);
			}
			else
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
				Form ownerForm = this._ribbon.FindForm();
				if ((ownerForm == null ? true : ownerForm.WindowState != FormWindowState.Minimized))
				{
					this._ribbon.CalculatedValues.Recalculate();
					base.Layout(context);
					this._layingOut = false;
				}
			}
		}

		public override void MouseLeave(EventArgs e)
		{
			Debug.Assert(e != null);
			if (e == null)
			{
				throw new ArgumentNullException("e");
			}
			if (this._active)
			{
				if (this._activeGroup != null)
				{
					this._activeGroup.PerformNeedPaint(false, this._activeGroup.ClientRectangle);
					this._activeGroup.Tracking = false;
					this._activeGroup = null;
				}
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
			if (this._active)
			{
				if (this._minimizedMode == this._ribbon.RealMinimizedMode)
				{
					ViewDrawRibbonGroup viewGroup = this._viewGroups.ViewGroupFromPoint(new Point(e.X, e.Y));
					if (viewGroup != this._activeGroup)
					{
						if (this._activeGroup != null)
						{
							this._activeGroup.Tracking = false;
							this._activeGroup.PerformNeedPaint(false, this._activeGroup.ClientRectangle);
						}
						this._activeGroup = viewGroup;
						if (this._activeGroup != null)
						{
							this._activeGroup.Tracking = true;
							this._activeGroup.PerformNeedPaint(false, this._activeGroup.ClientRectangle);
						}
					}
				}
			}
			base.MouseMove(e, rawPt);
		}

		private void PerformNeedPaint(bool needLayout)
		{
			this.PerformNeedPaint(needLayout, Rectangle.Empty);
		}

		private void PerformNeedPaint(bool needLayout, Rectangle invalidRect)
		{
			if (this._needPaintDelegate != null)
			{
				this._needPaintDelegate(this, new NeedLayoutEventArgs(needLayout, invalidRect));
			}
		}

		protected override void UpdateViewFromPoint(System.Windows.Forms.Control control, Point pt)
		{
			if (this._active)
			{
				base.UpdateViewFromPoint(control, pt);
			}
			else if (!base.MouseCaptured)
			{
				ViewBase mouseView = base.Root.ViewFromPoint(pt);
				if (!(mouseView is ViewDrawRibbonAppButton))
				{
					base.ActiveView = null;
				}
				else
				{
					base.ActiveView = mouseView;
				}
			}
		}
	}
}