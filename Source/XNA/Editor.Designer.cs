using System.Windows.Forms;
using System.Drawing;
using System.Reflection;
using System;

namespace BloodBulletEditor
{
	partial class Editor
	{
		private System.ComponentModel.IContainer m_Components = null;

		protected override void Dispose( bool p_Disposing )
		{
			if( p_Disposing && ( m_Components != null ) )
			{
				m_Components.Dispose( );
			}

			base.Dispose( p_Disposing );
		}

		private void InitializeComponent( )
		{
			m_MainMenu = new MainMenu( );
			m_SplitContainers = new SplitContainer[ 3 ];
			for( int i = 0; i < 3; ++i )
			{
				m_SplitContainers[ i ] =
					new SplitContainer( );
				m_SplitContainers[ i ].Name = "Split Container #" + i;
			}

			m_OrthographicViews = new OrthographicViewControl[ 3 ];

			m_OrthographicViews[ 0 ] =
				new OrthographicViewControl( VIEWPLANE.VIEWPLANE_XY );
			m_OrthographicViews[ 1 ] =
				new OrthographicViewControl( VIEWPLANE.VIEWPLANE_XZ );
			m_OrthographicViews[ 2 ] =
				new OrthographicViewControl( VIEWPLANE.VIEWPLANE_YZ );

			for( int i = 0; i < 3; ++i )
			{
				m_OrthographicViews[ i ].Dock = DockStyle.Fill;
				m_OrthographicViews[ i ].Location = new Point( 0, 0 );
			}

			m_PerspectiveView = new PerspectiveViewControl( );
			m_PerspectiveView.Dock = DockStyle.Fill;
			m_PerspectiveView.Location = new Point( 0, 0 );

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
			m_SplitContainers[ 1 ].Panel2.Controls.Add( m_OrthographicViews[ 1 ] );

			m_SplitContainers[ 1 ].Panel1.Controls.Add( m_PerspectiveView );
			m_SplitContainers[ 1 ].TabIndex = 1;
			m_SplitContainers[ 1 ].Orientation = Orientation.Horizontal;

			m_SplitContainers[ 2 ].Dock = DockStyle.Fill;
			m_SplitContainers[ 2 ].Location = new Point( 0, 0 );
			m_SplitContainers[ 2 ].Panel1.Controls.Add( m_OrthographicViews[ 2 ] );
			m_SplitContainers[ 2 ].Panel2.Controls.Add( m_OrthographicViews[ 0 ] );
			m_SplitContainers[ 2 ].TabIndex = 2;
			m_SplitContainers[ 2 ].Orientation = Orientation.Horizontal;

			m_SplitContainers[ 0 ].Dock = DockStyle.Fill;
			m_SplitContainers[ 0 ].Location = new Point( 0, 0 );
			m_SplitContainers[ 0 ].Panel1.Controls.Add( m_SplitContainers[ 1 ] );
			m_SplitContainers[ 0 ].Panel2.Controls.Add( m_SplitContainers[ 2 ] );
			m_SplitContainers[ 0 ].SplitterDistance = Width / 4;
			m_SplitContainers[ 0 ].TabIndex = 0;

			this.Controls.Add( m_SplitContainers[ 0 ] );

			this.m_Components = new System.ComponentModel.Container( );
			this.AutoScaleMode = AutoScaleMode.Font;
			this.Name = "BloodBulletEditor";
			this.Text = "Blood Bullet Editor";
			this.WindowState = FormWindowState.Maximized;
			m_SplitContainers[ 0 ].Panel1.ResumeLayout( false );
			m_SplitContainers[ 0 ].Panel2.ResumeLayout( false );
			m_SplitContainers[ 0 ].ResumeLayout( false );
			this.ResumeLayout( false );

			RemoveCursorNavigation( this.Controls );
		}

		private void FileExit( object p_Sender, System.EventArgs p_Args )
		{
			this.Close( );
		}

		private void RemoveCursorNavigation(
			Control.ControlCollection p_Controls )
		{
			foreach( Control Ctrl in p_Controls )
			{
				Ctrl.PreviewKeyDown += new PreviewKeyDownEventHandler(Ctrl_PreviewKeyDown);
				RemoveCursorNavigation( Ctrl.Controls );
			}
		}

		void Ctrl_PreviewKeyDown( object sender,
			PreviewKeyDownEventArgs p_Args )
		{
			switch( p_Args.KeyCode )
			{
				case Keys.Up:
				case Keys.Down:
				case Keys.Left:
				case Keys.Right:
				{
					p_Args.IsInputKey = false;
					break;
				}
				default:
				{
					break;
				}
			}
		}

		private OrthographicViewControl	[ ] m_OrthographicViews;
		private PerspectiveViewControl	m_PerspectiveView;
		private MainMenu				m_MainMenu;
		private MenuItem				m_FileMenu;
		private MenuItem				m_FileExit;
		private SplitContainer			[ ] m_SplitContainers;
	}
}
