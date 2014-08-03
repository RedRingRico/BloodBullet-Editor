using System.Windows.Forms;
using System.Drawing;

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
			m_MainMenu = new MainMenu( );
			m_SplitContainers = new SplitContainer[ 4 ];
			for( int i = 0; i < 4; ++i )
			{
				m_SplitContainers[ i ] =
					new SplitContainer( );
				m_SplitContainers[ i ].Name = "Split Container #" + i;
			}

			m_OrthographicViews = new OrthographicViewControl[ 3 ];

			for( int i = 0; i < 3; ++i )
			{
				m_OrthographicViews[ i ] = new OrthographicViewControl( );
				m_OrthographicViews[ i ].Dock = DockStyle.Fill;
				m_OrthographicViews[ i ].Location = new Point( 0, 0 );
			}

			m_OrthographicViews[ 0 ].ViewPlane = VIEWPLANE.VIEWPLANE_XY;
			m_OrthographicViews[ 0 ].Name = "Orthographic View [XY]";
			m_OrthographicViews[ 1 ].ViewPlane = VIEWPLANE.VIEWPLANE_XZ;
			m_OrthographicViews[ 1 ].Name = "Orthographic View [XZ]";
			m_OrthographicViews[ 2 ].ViewPlane = VIEWPLANE.VIEWPLANE_YZ;
			m_OrthographicViews[ 2 ].Name = "Orthographic View [YZ]";

			m_PerspectiveView = new PerspectiveViewControl( );
			m_PerspectiveView.Dock = DockStyle.Fill;
			m_PerspectiveView.Location = new Point( 0, 0 );
			m_PerspectiveView.Name = "Perspective View";

			// File sub-menu
			m_FileMenu = new MenuItem( "&File" );
			m_FileExit = new MenuItem( "&Exit",
				this.FileExit, Shortcut.CtrlQ );

			this.SuspendLayout( );

			m_MainMenu.MenuItems.Add( m_FileMenu );
			m_FileMenu.MenuItems.Add( m_FileExit );

			this.Menu = m_MainMenu;

			m_SplitContainers[ 1 ].Dock = DockStyle.Fill;
			m_SplitContainers[ 1 ].Location = new Point( 0, 0 );
			m_SplitContainers[ 1 ].Panel1.Controls.Add( m_OrthographicViews[ 0 ] );
			m_SplitContainers[ 1 ].Panel2.Controls.Add( m_OrthographicViews[ 1 ] );
			m_SplitContainers[ 1 ].TabIndex = 1;
			m_SplitContainers[ 1 ].Orientation = Orientation.Horizontal;

			m_SplitContainers[ 2 ].Dock = DockStyle.Fill;
			m_SplitContainers[ 2 ].Location = new Point( 0, 0 );
			m_SplitContainers[ 2 ].Panel1.Controls.Add( m_OrthographicViews[ 2 ] );
			m_SplitContainers[ 2 ].Panel2.Controls.Add( m_PerspectiveView );
			m_SplitContainers[ 2 ].TabIndex = 2;
			m_SplitContainers[ 2 ].Orientation = Orientation.Horizontal;

			m_SplitContainers[ 0 ].Dock = DockStyle.Fill;
			m_SplitContainers[ 0 ].Location = new Point( 0, 0 );
			m_SplitContainers[ 0 ].Panel1.Controls.Add( m_SplitContainers[ 1 ] );
			m_SplitContainers[ 0 ].Panel2.Controls.Add( m_SplitContainers[ 2 ] );
			m_SplitContainers[ 0 ].SplitterDistance =
				this.ClientRectangle.Width / 4;
			m_SplitContainers[ 0 ].TabIndex = 0;

			this.Controls.Add( m_SplitContainers[ 0 ] );

			this.components = new System.ComponentModel.Container( );
			this.AutoScaleMode = AutoScaleMode.Font;
			this.Name = "BloodBulletEditor";
			this.Text = "Blood Bullet Editor";
			this.WindowState = FormWindowState.Maximized;
			m_SplitContainers[ 0 ].Panel1.ResumeLayout( false );
			m_SplitContainers[ 0 ].Panel2.ResumeLayout( false );
			m_SplitContainers[ 0 ].ResumeLayout( false );
			this.ResumeLayout( false );
		}

		private void FileExit( object p_Sender, System.EventArgs p_Args )
		{
			this.Close( );
		}

		private OrthographicViewControl	[ ] m_OrthographicViews;
		private PerspectiveViewControl	m_PerspectiveView;
		private MainMenu				m_MainMenu;
		private MenuItem				m_FileMenu;
		private MenuItem				m_FileExit;
		private SplitContainer			[ ] m_SplitContainers;
	}
}
