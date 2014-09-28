//*******************************************************************
/*
 *	Project:	Alfray.BatteryInfo
 * 
 *	File:		StatusForm.cs
 * 
 *	RM (c) 2003
 * 
 */
//*******************************************************************


using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;

// -----------------------

using Microsoft.Win32;

// -----------------------

//**************************
namespace Alfray.BatteryInfo
{
	//**************************************
	/// <summary>
	/// Summary description for StatusForm.
	/// </summary>
	public class StatusForm : System.Windows.Forms.Form
	{

		private System.Windows.Forms.Label mLabelStatus;
		private System.Windows.Forms.ContextMenu mContextMenu;
		private System.Windows.Forms.MenuItem mMenuClose;

		// -------------------------------

		private bool mIsMoving = false;
		private Point mLastPt;

		private BatteryStatus mBatteryStatus = null;

		const int kPadding = 3;
		private System.Windows.Forms.Timer mTimer;
		private System.ComponentModel.IContainer components;


		//******************
		public StatusForm()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			// -------------------

			resize();
		}

		//************************************
		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.mLabelStatus = new System.Windows.Forms.Label();
			this.mContextMenu = new System.Windows.Forms.ContextMenu();
			this.mMenuClose = new System.Windows.Forms.MenuItem();
			this.mTimer = new System.Windows.Forms.Timer(this.components);
			this.SuspendLayout();
			// 
			// mLabelStatus
			// 
			this.mLabelStatus.BackColor = System.Drawing.Color.Transparent;
			this.mLabelStatus.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.mLabelStatus.CausesValidation = false;
			this.mLabelStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.mLabelStatus.Location = new System.Drawing.Point(3, 3);
			this.mLabelStatus.Name = "mLabelStatus";
			this.mLabelStatus.Size = new System.Drawing.Size(173, 16);
			this.mLabelStatus.TabIndex = 0;
			this.mLabelStatus.Text = "<status>";
			this.mLabelStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.mLabelStatus.UseMnemonic = false;
			this.mLabelStatus.MouseEnter += new System.EventHandler(this.mLabelStatus_MouseEnter);
			this.mLabelStatus.MouseUp += new System.Windows.Forms.MouseEventHandler(this.mLabelStatus_MouseUp);
			this.mLabelStatus.MouseMove += new System.Windows.Forms.MouseEventHandler(this.mLabelStatus_MouseMove);
			this.mLabelStatus.MouseLeave += new System.EventHandler(this.mLabelStatus_MouseLeave);
			this.mLabelStatus.MouseDown += new System.Windows.Forms.MouseEventHandler(this.mLabelStatus_MouseDown);
			// 
			// mContextMenu
			// 
			this.mContextMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																						 this.mMenuClose});
			// 
			// mMenuClose
			// 
			this.mMenuClose.Index = 0;
			this.mMenuClose.Text = "Close";
			this.mMenuClose.Click += new System.EventHandler(this.mMenuClose_Click);
			// 
			// mTimer
			// 
			this.mTimer.Interval = 6000;
			this.mTimer.Tick += new System.EventHandler(this.mTimer_Tick);
			// 
			// StatusForm
			// 
			this.AutoScale = false;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.BackColor = System.Drawing.SystemColors.ActiveCaption;
			this.ClientSize = new System.Drawing.Size(176, 20);
			this.ContextMenu = this.mContextMenu;
			this.ControlBox = false;
			this.Controls.Add(this.mLabelStatus);
			this.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "StatusForm";
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Text = "<status>";
			this.TopMost = true;
			this.TransparencyKey = System.Drawing.Color.Red;
			this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.StatusForm_MouseDown);
			this.Closing += new System.ComponentModel.CancelEventHandler(this.StatusForm_Closing);
			this.Load += new System.EventHandler(this.StatusForm_Load);
			this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.StatusForm_MouseUp);
			this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.StatusForm_MouseMove);
			this.MouseEnter += new System.EventHandler(this.StatusForm_MouseEnter);
			this.MouseLeave += new System.EventHandler(this.StatusForm_MouseLeave);
			this.ResumeLayout(false);

		}
		#endregion


		//*******************************************
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new StatusForm());
		}


		// ------------------------------------------------

		//*******************************
		private void moveWindow(Point pt)
		{
			int dx = pt.X - mLastPt.X;
			int dy = pt.Y - mLastPt.Y;

			this.Location = new Point(this.Location.X + dx, this.Location.Y + dy);

			mLastPt = pt;
		}

		//**********************
		private bool loadPrefs()
		{
			try
			{
				// Load location

				RegistryKey reg = Registry.CurrentUser;

				// Get the namespace of this class, typially "Alfray.BatteryInfo"
				// and transform it into two subdirs "Alfray/BatteryInfo"
				string path = this.GetType().Namespace.Replace('.', '/');
				
				reg = reg.CreateSubKey(path);

				int x = (int)reg.GetValue("x", this.Location.X);
				int y = (int)reg.GetValue("y", this.Location.Y);

				this.Location = new Point(x, y);

				reg.Close();

				return true;
			}
			catch(Exception ex)
			{
				System.Diagnostics.Debug.Fail(ex.ToString());
			}

			return false;
		}

		//**********************
		private bool savePrefs()
		{
			try
			{
				// Save location

				RegistryKey reg = Registry.CurrentUser;

				// Get the namespace of this class, typially "Alfray.BatteryInfo"
				// and transform it into two subdirs "Alfray/BatteryInfo"
				string path = this.GetType().Namespace.Replace('.', '/');

				reg = reg.CreateSubKey(path);

				reg.SetValue("x", this.Location.X);
				reg.SetValue("y", this.Location.Y);

				reg.Close();
				return true;
			}
			catch(Exception ex)
			{
				System.Diagnostics.Debug.Fail(ex.ToString());
			}

			return false;
		}

		//*******************
		private bool resize()
		{
			Size sz = mLabelStatus.ClientSize;
			sz.Height = mLabelStatus.Font.Height;

			// resize the control
			mLabelStatus.ClientSize = sz;

			// resize the window to fit the control
			mLabelStatus.Location = new Point(kPadding, kPadding);
			this.ClientSize = new Size(sz.Width + 2*kPadding, sz.Height + 2*kPadding);;

			return true;
		}

		//********************
		private bool getText()
		{
			// get battery status
			if (mBatteryStatus == null)
				mBatteryStatus = new BatteryStatus();
			else
				mBatteryStatus.UpdateStatus();

			// format string
			const string format = "{0:00}h{1:00} - {2:G}% - {3} {4}";

			mLabelStatus.Text = String.Format(
				format,
				mBatteryStatus.TimeLeft.Hours,
				mBatteryStatus.TimeLeft.Minutes,
				mBatteryStatus.LifePercent,
				mBatteryStatus.Flag,
				mBatteryStatus.AcLine);

			return true;
		}
		
		// ------------------------------------------------

		private void mLabelStatus_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			StatusForm_MouseDown(sender, e);
			mLastPt = mLabelStatus.PointToScreen(new Point(e.X, e.Y));
		}

		private void mLabelStatus_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			StatusForm_MouseUp(sender, e);
		}

		private void mLabelStatus_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (mIsMoving)
				moveWindow(mLabelStatus.PointToScreen(new Point(e.X, e.Y)));
		}

		// ------------------------------------------------

		private void mLabelStatus_MouseEnter(object sender, System.EventArgs e)
		{
			this.Cursor = Cursors.Hand;
		}

		private void mLabelStatus_MouseLeave(object sender, System.EventArgs e)
		{
			this.Cursor = Cursors.Default;		
		}

		// ------------------------------------------------

		private void StatusForm_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			mIsMoving = true;
			mLastPt = this.PointToScreen(new Point(e.X, e.Y));
		}

		private void StatusForm_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			mIsMoving = false;
		}

		private void StatusForm_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (mIsMoving)
				moveWindow(this.PointToScreen(new Point(e.X, e.Y)));
		}

		// ------------------------------------------------

		private void StatusForm_MouseEnter(object sender, System.EventArgs e)
		{
			this.Cursor = Cursors.Hand;		
		}

		private void StatusForm_MouseLeave(object sender, System.EventArgs e)
		{
			this.Cursor = Cursors.Default;
		}

		// ------------------------------------------------

		private void mMenuClose_Click(object sender, System.EventArgs e)
		{
			this.Close();
		}

		// ------------------------------------------------

		private void StatusForm_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			e.Cancel = !savePrefs();

			// Stop timer if closing goes on
			mTimer.Enabled = e.Cancel;
		}

		private void StatusForm_Load(object sender, System.EventArgs e)
		{
			loadPrefs();

			// Start timer
			mTimer.Enabled = true;
		}

		// ------------------------------------------------

		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);

			if (this.ClientSize.Height > mLabelStatus.ClientSize.Height + 2*kPadding)
				resize();
		}

		protected override void OnActivated(EventArgs e)
		{
			getText();
			base.OnActivated(e);
		}

	
		protected override void OnVisibleChanged(EventArgs e)
		{
			getText();
			base.OnVisibleChanged(e);
		}

		private void mTimer_Tick(object sender, System.EventArgs e)
		{
			getText();
		}

	} // class StatusForm
} // namespace Alfray.BatteryInfo

//*******************************************************************
/*
 *	$Log: StatusForm.cs,v $
 *	Revision 1.2  2004-06-16 03:13:27  ralf
 *	Charging time indicator
 *
 *	Revision 1.1.1.1  2003/08/01 00:49:57  ralf
 *	Initial working version
 *	
 * 
 */
//*******************************************************************
