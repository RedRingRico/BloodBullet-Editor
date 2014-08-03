namespace BloodBulletEditor
{
	partial class Editor
	{
		private System.ComponentModel.IContainer components = null;

		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			m_MainMenu = new System.Windows.Forms.MainMenu( );

			// File sub-menu
			m_FileMenu = new System.Windows.Forms.MenuItem( "&File" );
			m_FileExit = new System.Windows.Forms.MenuItem( "&Exit",
				this.FileExit, System.Windows.Forms.Shortcut.CtrlQ );

			this.SuspendLayout( );

			m_MainMenu.MenuItems.Add( m_FileMenu );
			m_FileMenu.MenuItems.Add( m_FileExit );

			this.Menu = m_MainMenu;

			m_Orthographic0 = new OrthographicViewControl( );

			this.Controls.Add( m_Orthographic0 );

			m_Orthographic0.Dock = System.Windows.Forms.DockStyle.Fill;
			m_Orthographic0.Location = new System.Drawing.Point( 0, 0 );
			m_Orthographic0.Name = "Orthographic View 0";

			this.components = new System.ComponentModel.Container();
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Name = "BBBEditor";
			this.Text = "Bang Bang Banquet Editor";
			this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
			this.ResumeLayout( false );
		}

		private void FileExit( object p_Sender, System.EventArgs p_Args )
		{
			this.Close( );
		}

		private OrthographicViewControl m_Orthographic0;
		private System.Windows.Forms.MainMenu	m_MainMenu;
		private System.Windows.Forms.MenuItem	m_FileMenu;
		private System.Windows.Forms.MenuItem	m_FileExit;
	}
}
