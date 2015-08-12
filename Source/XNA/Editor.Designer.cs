using System.Windows.Forms;
using System.Drawing;
using System.Reflection;
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.GameStudio.PlatformTools;
using System.Runtime.InteropServices;

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
			Cubes = new List< Game. Cube >( );
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
            m_ToolMenu = new MenuItem( "&Tool" );
            m_ToolModelEditor = new MenuItem( "&Model Editor",
                this.ModelEditor, Shortcut.CtrlM );

			this.SuspendLayout( );

			m_MainMenu.MenuItems.Add( m_FileMenu );
			m_FileMenu.MenuItems.Add( m_FileExit );
            m_MainMenu.MenuItems.Add( m_ToolMenu );
            m_ToolMenu.MenuItems.Add( m_ToolModelEditor );

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

			Image CreateBoxImage;
			CreateBoxImage = Image.FromFile( @"Icons\CreateBox.png" );
			ToolStripButton CreateBoxButton = new ToolStripButton(
				String.Empty, CreateBoxImage, BoxCreate_OnClick,
				"Create Box" );

			Image StartGameImage;
			StartGameImage = Image.FromFile( @"Icons\StartGame.png" );
			ToolStripButton StartGameButton = new ToolStripButton(
				string.Empty, StartGameImage, StartGame_OnClick,
				"Start Game" );

			ToolStripItem [ ]ToolStripItems = new ToolStripItem[ 2 ];
			ToolStripItems[ 0 ] = CreateBoxButton;
			ToolStripItems[ 1 ] = StartGameButton;
			m_ToolStrip = new ToolStrip( ToolStripItems );

			this.Controls.Add( m_SplitContainers[ 0 ] );
			this.Controls.Add( m_ToolStrip );

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

        private void ModelEditor( object p_Sender, System.EventArgs p_Args )
        {
            if( m_ModelEditor == null )
            {
                m_ModelEditor = new Model.ModelEditor( );
                m_ModelEditor.Show( );
            }
            else
            {
                if( m_ModelEditor.IsClosed( ) )
                {
                    m_ModelEditor.Show( );
                }
                else
                {
                    m_ModelEditor.BringToFront( );
                }
            }
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

		void Ctrl_PreviewKeyDown( object p_Sender,
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

		void BoxCreate_OnClick( object p_Sender, EventArgs p_Args )
		{
			Microsoft.Xna.Framework.Vector3 Minimum =
				new Microsoft.Xna.Framework.Vector3( -50.0f, 0.0f, -50.0f );
			Microsoft.Xna.Framework.Vector3 Maximum =
				new Microsoft.Xna.Framework.Vector3( 50.0f, 100.0f, 50.0f );
			// Create a 1m^2 cube (1 world unit == 1cm)
			Game.Cube NewCube = new Game.Cube( Editor.GraphicsDevice,
				Minimum, Maximum, Microsoft.Xna.Framework.Color.White );

			Cubes.Add( NewCube );
		}

		void StartGame_OnClick( object p_Sender, EventArgs p_Args )
		{
			IPlatform Target = PlatformManager.GetPlatform( "Xbox 360",
				"Xbox 360.v4.0.HiDef" );
			ILaunchService Launcher = ( ILaunchService ) Target.GetTitlePlayer(
				Target.DefaultTitlePlayer ).GetService(
					typeof( ILaunchService ) );
			Assembly GameAssembly = Assembly.GetExecutingAssembly( );
			GuidAttribute AssemblyAttribute =
				( GuidAttribute )GameAssembly.GetCustomAttributes(
					typeof( GuidAttribute ), true )[ 0 ];
			string GUIDString = AssemblyAttribute.Value;


			using( ( IDisposable )Launcher )
			{
				Guid GameGUID = new Guid( );
				try
				{
					ITitleProcess TitleProcess = !Guid.TryParse(
						GUIDString, out GameGUID ) ?
						Launcher.Launch( GUIDString, string.Empty,
							string.Empty ) :
						Launcher.LaunchTitle( GUIDString, string.Empty,
							string.Empty );

					if( TitleProcess != null )
					{
						( ( IDisposable ) TitleProcess ).Dispose( );
					}
				}
				catch( PlatformToolsException Exception )
				{
					MessageBox.Show( Exception.Message,
						"Blood Bullet Editor" );
				}
			}
		}

		private OrthographicViewControl	[ ] m_OrthographicViews;
		private PerspectiveViewControl	m_PerspectiveView;
		private MainMenu				m_MainMenu;
		private MenuItem				m_FileMenu;
		private MenuItem				m_FileExit;
        private MenuItem                m_ToolMenu;
        private MenuItem                m_ToolModelEditor;
		private SplitContainer			[ ] m_SplitContainers;
		private ToolStrip				m_ToolStrip;

        private Model.ModelEditor       m_ModelEditor = null;

		public static GraphicsDevice	GraphicsDevice;
		public static List< Game.Cube > Cubes;
	}
}
