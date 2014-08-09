using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Net;

namespace BloodBulletEditor
{
	class PerspectiveViewControl : GraphicsDeviceControl
	{
		protected override int Initialise( )
		{
			m_PerspectiveView = true;
			Application.Idle += delegate { Invalidate( ); };

			m_ClearColour = Microsoft.Xna.Framework.Color.Black;

			m_Grid = new Grid( this.GraphicsDevice );
			m_Grid.Create( VIEWPLANE.VIEWPLANE_XZ, 100, 100, 100.0f, 0.0f,
				new Color( 32, 32, 128 ), 10, Color.Blue );

			MenuItem [ ] PerspectiveMenu = new MenuItem[ 2 ];
			PerspectiveMenu[ 0 ] = new MenuItem( "Live Edit" );

			MenuItem [ ] LiveMenu = new MenuItem[ 2 ];
			LiveMenu[ 0 ] = new MenuItem( "Connect", LiveSubMenu_Connect );
			LiveMenu[ 1 ] = new MenuItem( "Disconnect",
				LiveSubMenu_Disconnect );

			for( int i = 0; i < 2; ++i )
			{
				PerspectiveMenu[ 0 ].MenuItems.Add( LiveMenu[ i ] );
			}

			PerspectiveMenu[ 1 ] = new MenuItem( "Change Clear Colour",
				ClearColour_ClickHandle );
			this.ContextMenu = new ContextMenu( PerspectiveMenu );

			m_ColourPicker = new ColorDialog( );

			m_SpriteBatch = new SpriteBatch( this.GraphicsDevice );

			m_ScreenRender = new RenderTarget2D( this.GraphicsDevice,
				this.Width, this.Height );

			m_AspectRatio = ( float )Width / ( float )Height;

			m_PacketWriter = new PacketWriter( );

			return 0;
		}
		
		private void LiveSubMenu_Connect( object p_Sender, EventArgs p_Args )
		{
			if( SignedInGamer.SignedInGamers.Count == 0 )
			{
				DialogResult Result = MessageBox.Show( null,
					"You are not currently signed in, please sign in",
					"Not signed in",
					MessageBoxButtons.YesNo,
					System.Windows.Forms.MessageBoxIcon.Warning );

				if( Result == DialogResult.Yes )
				{
					if( Guide.IsVisible == false )
					{
						Guide.ShowSignIn( 1, false );
					}
				}
			}
			else
			{
				// For selecting the network session, there should be a listbox
				// displaying the current consoles discovered
				bool KeepRetrying = ( ConnectToConsole( ) == false );
				while( KeepRetrying )
				{
					DialogResult Result = MessageBox.Show( null,
						"No network sessions available",
						"Connection",
						MessageBoxButtons.RetryCancel,
						System.Windows.Forms.MessageBoxIcon.Warning );

					if( Result == DialogResult.Retry )
					{
						KeepRetrying = ( ConnectToConsole( ) == false );
					}
					else
					{
						KeepRetrying = true;
					}
				}
			}
		}

		private bool ConnectToConsole( )
		{
			bool Return = false;
			if( m_NetworkSession != null )
			{
				DialogResult SessionExists =
					MessageBox.Show( null,
						"A session already exists, connect to a new one?",
						"Session exists",
						MessageBoxButtons.YesNo,
						System.Windows.Forms.MessageBoxIcon.Warning );

				if( SessionExists == DialogResult.Yes )
				{
					m_NetworkSession.Dispose( );
					m_NetworkSession = null;
				}
				else
				{
					return true;
				}
			}

			if( m_NetworkSessions != null )
			{
				m_NetworkSessions.Dispose( );
				m_NetworkSessions = null;
			}

			m_NetworkSessions = NetworkSession.Find(
				NetworkSessionType.SystemLink, 1, null );

			if( m_NetworkSessions.Count > 0 )
			{
				foreach( AvailableNetworkSession Session in 
					m_NetworkSessions )
				{
					System.Diagnostics.Debug.Write( 
						Session.HostGamertag + "\n" );
				}
				m_NetworkSession = NetworkSession.Join(
					m_NetworkSessions[ 0 ] );

				m_NetworkSessions.Dispose( );
				m_NetworkSessions = null;

				Return = true;
			}

			m_NetworkSessions.Dispose( );
			m_NetworkSessions = null;

			return Return;
		}

		private void LiveSubMenu_Disconnect( object p_Sender,
			EventArgs p_Args )
		{
			if( m_NetworkSession != null )
			{
			}
		}

		private void ClearColour_ClickHandle( object p_Sender,
			EventArgs p_Args )
		{
			DialogResult Result = m_ColourPicker.ShowDialog( );

			if( Result == DialogResult.OK ) 
			{
				m_ClearColour.R = m_ColourPicker.Color.R;
				m_ClearColour.G = m_ColourPicker.Color.G;
				m_ClearColour.B = m_ColourPicker.Color.B;

				if( m_NetworkSession != null )
				{
					foreach( LocalNetworkGamer Local in
						m_NetworkSession.LocalGamers )
					{
						m_PacketWriter.Write( m_ClearColour.ToVector4( ) );

						Local.SendData( m_PacketWriter,
							SendDataOptions.ReliableInOrder );
					}
				}
			}
		}

		protected override void OnCreateControl()
		{
			base.OnCreateControl();

			if( GamerServicesDispatcher.IsInitialized == false )
			{
				GamerServicesDispatcher.WindowHandle = Handle;
				try
				{
					GamerServicesDispatcher.Initialize( m_Services );
				}
				catch( GamerServicesNotAvailableException p_Exception )
				{
					System.Diagnostics.Debug.Write( p_Exception.Message );
				}
				finally
				{
				}
			}
		}

		protected override void Draw( )
		{
			if( Width != m_ScreenRender.Width ||
				Height != m_ScreenRender.Height )
			{
				m_ScreenRender = null;
				m_ScreenRender = new RenderTarget2D( this.GraphicsDevice,
					Width, Height );
				m_AspectRatio = ( float )Width / ( float )Height;
			}

			this.GraphicsDevice.SetRenderTarget( m_ScreenRender );
			this.GraphicsDevice.Clear( m_ClearColour );
		  
			m_ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(
				( float )Math.PI/2.0f, m_AspectRatio, 1.0f, 100000.0f );

			m_ViewMatrix = Matrix.CreateLookAt(
				new Vector3( 10.0f, 100.0f, 100.0f ),
				Vector3.Zero, Vector3.Up );

			m_WorldMatrix = Matrix.Identity;

			m_Grid.Render( m_WorldMatrix, m_ViewMatrix, m_ProjectionMatrix );
			
			this.GraphicsDevice.SetRenderTarget( null );

			m_SpriteBatch.Begin( );
			m_SpriteBatch.Draw( ( Texture2D ) m_ScreenRender,
				new Rectangle( 0, 0, 
				GraphicsDevice.PresentationParameters.BackBufferWidth,
					GraphicsDevice.PresentationParameters.BackBufferHeight ),
					new Rectangle( 0, 0, Width, Height ),
					Color.White );
			m_SpriteBatch.End( );

			if( m_NetworkSession != null )
			{
				m_NetworkSession.Update( );
			}
		}	
		// Logic to add:
		// When the user clicks on the view (single), the viewport is treated as
		// if it were a game, until the escape key is struck
		// Panning, rotation, and forward/backward translation are handled by
		// holding a modifier key while pressing the middle mouse button,
		// similar to Blender's navigation interface

		private Grid			m_Grid;
		private Matrix			m_WorldMatrix;
		private Matrix			m_ViewMatrix;
		private Matrix			m_ProjectionMatrix;
		private NetworkSession	m_NetworkSession;
		private AvailableNetworkSessionCollection	m_NetworkSessions;
		private PacketWriter	m_PacketWriter;
		private ColorDialog		m_ColourPicker;
		private SpriteBatch		m_SpriteBatch;
		private RenderTarget2D	m_ScreenRender;
		private float			m_AspectRatio;
	}
}
