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

			m_Connected = false;

			m_Effect = new BasicEffect( GraphicsDevice );

			m_Effect.LightingEnabled = false;
			m_Effect.VertexColorEnabled = true;

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

			AvailableNetworkSessionCollection NetworkSessions =
				NetworkSession.Find( NetworkSessionType.SystemLink, 1, null );

			if( NetworkSessions.Count > 0 )
			{
				foreach( AvailableNetworkSession Session in 
					NetworkSessions )
				{
					System.Diagnostics.Debug.Write( 
						Session.HostGamertag + "\n" );
				}
				m_NetworkSession = NetworkSession.Join(
					NetworkSessions[ 0 ] );

				m_NetworkSession.SessionEnded += NetworkSessionEnded;

				Return = true;
				m_Connected = true;
			}

			if( NetworkSessions != null )
			{
				NetworkSessions.Dispose( );
				NetworkSessions = null;
			}

			return Return;
		}

		void NetworkSessionEnded( object p_Sender,
			NetworkSessionEndedEventArgs p_Args )
		{
			m_Connected = false;
			if( p_Args.EndReason == NetworkSessionEndReason.Disconnected )
			{
				// Ask if the user wants to try again
				DialogResult Result = MessageBox.Show( null,
					"The connection to the host was lost.\nTry again?",
					"Connection Error",
					MessageBoxButtons.YesNo,
					System.Windows.Forms.MessageBoxIcon.Error );

				if( Result == DialogResult.Yes )
				{
					m_NetworkSession.Dispose( );
					m_NetworkSession = null;

					bool KeepRetrying = ( ConnectToConsole( ) == false );

					while( KeepRetrying )
					{
						DialogResult Result2 = MessageBox.Show( null,
							"No network sessions available",
							"Connect",
							MessageBoxButtons.RetryCancel,
							System.Windows.Forms.MessageBoxIcon.Warning );

						if( Result2 == DialogResult.Retry )
						{
							KeepRetrying = ( ConnectToConsole( ) == false );
						}
						else
						{
							KeepRetrying = false;
						}
					}
				}
			}
			System.Diagnostics.Debug.Write( "Disconnected: " +
				p_Args.EndReason.ToString( ) + "\n" );
		}

		private void LiveSubMenu_Disconnect( object p_Sender,
			EventArgs p_Args )
		{
			if( m_NetworkSession != null )
			{
				m_NetworkSession.Dispose( );
				m_NetworkSession = null;
			}

			m_Connected = false;
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
						uint MessageType = 1;
						m_PacketWriter.Write( MessageType );
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

			m_ConnectionTexture = new Texture2D( GraphicsDevice, 1, 1, false,
				SurfaceFormat.Color );
			byte [ ] PixelData = new byte [ 4 ];
			for( int i = 0; i < 4; ++i )
			{
				PixelData[ i ] = 0xFF;
			}
			// Create a white pixel to draw the connection status later
			m_ConnectionTexture.SetData< byte >( PixelData );

			float StartX, StartY, EndX, EndY;
			float AdvanceX, AdvanceY;
			AdvanceX = ClientSize.Width / 100.0f;
			AdvanceY = ClientSize.Height / 100.0f;
			StartX = 0.0f;
			StartY = AdvanceY * 98;
			EndX = StartX + ( AdvanceX * 3 );
			EndY = StartY + ( AdvanceY * 2 );

			m_ConnectedRectangle = new Rectangle( ( int )StartX, ( int )StartY,
				( int )EndX, ( int )EndY );
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
				new Vector3( 100.0f, 100.0f, 100.0f ),
				Vector3.Zero, Vector3.Up );

			m_WorldMatrix = Matrix.Identity;

			m_Grid.Render( m_WorldMatrix, m_ViewMatrix, m_ProjectionMatrix );

			m_Effect.World = Matrix.Identity;
			m_Effect.View = m_ViewMatrix;
			m_Effect.Projection = m_ProjectionMatrix;

			RasterizerState RasterState = new RasterizerState( );
			RasterizerState OldState = GraphicsDevice.RasterizerState;
			RasterState.CullMode = CullMode.None;
			RasterState.FillMode = FillMode.WireFrame;
			GraphicsDevice.RasterizerState = RasterState;

			foreach( Game.Cube GameCube in Editor.Cubes )
			{
				GraphicsDevice.Indices = GameCube.IndexBuffer;
				GraphicsDevice.SetVertexBuffer( GameCube.VertexBuffer );
				foreach( EffectPass Pass in m_Effect.CurrentTechnique.Passes )
				{
					Pass.Apply( );
					GraphicsDevice.DrawIndexedPrimitives(
						PrimitiveType.TriangleList, 0, 0,
						GameCube.IndexBuffer.IndexCount, 0, 12 );
				}
			}

			GraphicsDevice.RasterizerState = OldState;

			m_SpriteBatch.Begin( );
			m_SpriteBatch.Draw( m_ConnectionTexture,
				m_ConnectedRectangle,
				m_Connected ? Color.Lime : Color.Red );
			m_SpriteBatch.End( );
			
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

		protected override void OnResize( EventArgs p_Args )
		{
			base.OnResize( p_Args );
			float StartX, StartY, EndX, EndY;
			float AdvanceX, AdvanceY;
			AdvanceX = ClientSize.Width / 100.0f;
			AdvanceY = ClientSize.Height / 100.0f;
			StartX = 0.0f;
			StartY = AdvanceY * 98;
			EndX = StartX + ( AdvanceX * 3 );
			EndY = StartY + ( AdvanceY * 2 );

			m_ConnectedRectangle = new Rectangle( ( int )StartX, ( int )StartY,
				( int )EndX, ( int )EndY );
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
		private PacketWriter	m_PacketWriter;
		private ColorDialog		m_ColourPicker;
		private SpriteBatch		m_SpriteBatch;
		private RenderTarget2D	m_ScreenRender;
		private float			m_AspectRatio;
		private bool			m_Connected;
		private Texture2D		m_ConnectionTexture;
		private Rectangle		m_ConnectedRectangle;
		private BasicEffect		m_Effect;
	}
}
