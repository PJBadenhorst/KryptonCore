using ComponentFactory.Krypton.Toolkit;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace ComponentFactory.Krypton.Ribbon
{
	internal class ViewRibbonQATOverflowManager : ViewManager
	{
		private KryptonRibbon _ribbon;

		private ViewLayoutRibbonQATContents _qatContents;

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

		public ViewLayoutRibbonQATContents QATContents
		{
			get
			{
				return this._qatContents;
			}
		}

		public ViewRibbonQATOverflowManager(KryptonRibbon ribbon, System.Windows.Forms.Control control, ViewLayoutRibbonQATContents qatContents, ViewBase root) : base(control, root)
		{
			Debug.Assert(ribbon != null);
			Debug.Assert(qatContents != null);
			this._ribbon = ribbon;
			this._qatContents = qatContents;
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
	}
}