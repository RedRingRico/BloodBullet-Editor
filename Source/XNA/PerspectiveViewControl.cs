using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Input;

namespace BloodBulletEditor
{
	class PerspectiveViewControl : GraphicsDeviceControl
	{
		protected override int Initialise( )
		{
            m_HasGWFL = false;
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
			m_MiddleButtonDown = false;

			m_CameraPosition = new Vector3( 0.0f, 100.0f, 100.0f );
			m_CameraLookAt = Vector3.Zero;
			m_Orbiting = m_Dollying = m_Panning = false;

			m_Orientation = new Quaternion( );

			m_CameraPositionTemp = m_CameraPosition;

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
						KeepRetrying = false;
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
				m_ClearColour = new Color( 
					m_ColourPicker.Color.R, m_ColourPicker.Color.G,
					m_ColourPicker.Color.B, m_ColourPicker.Color.A );

				System.Diagnostics.Debug.Write( "Clear colour: " +
					m_ClearColour.ToString( ) );

				if( m_NetworkSession != null )
				{
					foreach( LocalNetworkGamer Local in
						m_NetworkSession.LocalGamers )
					{
						uint MessageType = 1;
						m_PacketWriter.Write( MessageType );
						m_PacketWriter.Write( m_ClearColour.ToVector3( ) );

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
                    m_HasGWFL = true;
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
			GamePadState NewGamePadState = GamePad.GetState( PlayerIndex.One );

			m_CameraPosition.Z -= NewGamePadState.ThumbSticks.Left.Y;
			m_CameraPosition.X += NewGamePadState.ThumbSticks.Left.X;

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

			m_ViewMatrix = Matrix.CreateLookAt( m_CameraPosition,
				m_CameraLookAt, Vector3.Up );

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
			/*m_SpriteBatch.Draw(( Texture2D ) m_ScreenRender,
				new Rectangle( 0, 0, m_ScreenRender.Width, m_ScreenRender.Height ),/*
				GraphicsDevice.PresentationParameters.BackBufferWidth,
					GraphicsDevice.PresentationParameters.BackBufferHeight ),*
					new Rectangle( 0, 0, Width, Height ),
					Color.White );*/

            if( m_HasGWFL )
            {
			m_SpriteBatch.Draw(( Texture2D ) m_ScreenRender,
				new Rectangle( 0, 0, 
				GraphicsDevice.PresentationParameters.BackBufferWidth,
					GraphicsDevice.PresentationParameters.BackBufferHeight ),
					new Rectangle( 0, 0, Width, Height ),
					Color.White );
            }
            else
            {
			m_SpriteBatch.Draw(( Texture2D ) m_ScreenRender,
				new Rectangle( 0, 0, m_ScreenRender.Width, m_ScreenRender.Height ),/*
				GraphicsDevice.PresentationParameters.BackBufferWidth,
					GraphicsDevice.PresentationParameters.BackBufferHeight ),*/
					new Rectangle( 0, 0, Width, Height ),
					Color.White );
            }
			m_SpriteBatch.End( );

			if( m_NetworkSession != null )
			{
				m_NetworkSession.Update( );
			}

			m_PreviousGamePadState = NewGamePadState;
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

		protected override void OnMouseDown( MouseEventArgs p_Args )
		{
			if( p_Args.Button == System.Windows.Forms.MouseButtons.Middle )
			{
				m_MouseX = p_Args.X;
				m_MouseY = p_Args.Y;

				m_MiddleButtonDown = true;
			}

			base.OnMouseDoubleClick( p_Args );
		}

		protected override void OnMouseUp( MouseEventArgs p_Args )
		{
			if( p_Args.Button == System.Windows.Forms.MouseButtons.Middle )
			{
				m_MouseX = 0.0f;
				m_MouseY = 0.0f;
				m_XMouseDelta = 0.0f;
				m_YMouseDelta = 0.0f;
				m_MiddleButtonDown = false;
			}
			base.OnMouseUp( p_Args );
		}

		protected override void OnKeyDown( KeyEventArgs p_Args )
		{
			if( p_Args.Shift )
			{
				if( !m_Orbiting && !m_Dollying )
				{
					m_Panning = true;
				}
			}

			if( p_Args.Control )
			{
				if( !m_Orbiting && !m_Panning )
				{
					m_CameraPosition.Z += m_YMouseDelta * 10.0f;

					m_Dollying = true;
				}
			}

			base.OnKeyDown( p_Args );
		}

		protected override void OnKeyUp( KeyEventArgs p_Args )
		{
			if( !p_Args.Shift )
			{
				m_Panning = false;
			}

			if( !p_Args.Control )
			{
				m_Dollying = false;
			}

			base.OnKeyUp( p_Args );
		}

		private void Rotate( Vector3 p_Axis, float p_Angle )
		{
			float Sine, Cosine;
			Sine = ( float )Math.Sin( p_Angle );
			Cosine = ( float )Math.Cos( p_Angle );

			m_Rotation.X = p_Axis.X * Sine;
			m_Rotation.Y = p_Axis.Y * Sine;
			m_Rotation.Z = p_Axis.Z * Sine;
			m_Rotation.W = Cosine;
		}

		private void RecalculateAxes( )
		{
			m_Orientation *= m_Rotation;

			Matrix Axes = Matrix.CreateFromQuaternion( m_Orientation );

			m_LocalRight.X = Axes.M11;
			m_LocalRight.Y = Axes.M21;
			m_LocalRight.Z = Axes.M31;

			m_LocalUp.X = Axes.M12;
			m_LocalUp.Y = Axes.M22;
			m_LocalUp.Z = Axes.M32;

			m_LocalForward.X = Axes.M13;
			m_LocalForward.Y = Axes.M23;
			m_LocalForward.Z = Axes.M33;
		}

		private void SetView3D( Vector3 p_Right, Vector3 p_Up,
			Vector3 p_Forward, Vector3 p_Position )
		{
			m_ViewMatrix.M41 = m_ViewMatrix.M42 = m_ViewMatrix.M43 = 0.0f;
			m_ViewMatrix.M44 = 1.0f;

			m_ViewMatrix.M11 = m_LocalRight.X;
			m_ViewMatrix.M21 = m_LocalRight.Y;
			m_ViewMatrix.M31 = m_LocalRight.Z;

			m_ViewMatrix.M12 = m_LocalUp.X;
			m_ViewMatrix.M22 = m_LocalUp.Y;
			m_ViewMatrix.M32 = m_LocalUp.Z;

			m_ViewMatrix.M13 = m_LocalForward.X;
			m_ViewMatrix.M23 = m_LocalForward.Y;
			m_ViewMatrix.M33 = m_LocalForward.Z;

			m_ViewMatrix.M14 = p_Position.X;
			m_ViewMatrix.M24 = p_Position.Y;
			m_ViewMatrix.M34 = p_Position.Z;
		}

		protected override void OnMouseMove( MouseEventArgs p_Args )
		{
			if( m_MiddleButtonDown )
			{
				m_XMouseDelta = -( ( p_Args.Y - m_MouseY ) * 0.01f );
				m_YMouseDelta = ( p_Args.X - m_MouseX ) * 0.01f;

				if( !m_Dollying && !m_Panning )
				{
					m_Orbit.X = m_XMouseDelta;
					m_Orbit.Y = m_YMouseDelta;
					/*
					if( m_Orbit.X > ( Math.PI / 2.0f ) )
					{
						m_Orbit.X = ( float )( Math.PI / 2.0f );
					}*/

					m_MouseX = p_Args.X;
					m_MouseY = p_Args.Y;

					Vector3 OrbitPoint = Vector3.Zero;
					Vector3 Position = m_CameraPosition - OrbitPoint;

					Quaternion Q = Quaternion.CreateFromYawPitchRoll( -m_MouseX,
						m_MouseY, 0.0f );

					Quaternion Inverse = Quaternion.Conjugate( m_Orientation );

					m_Orientation = Quaternion.Normalize( m_Orientation * Q * Inverse );

					this.Rotate( Vector3.Right, m_MouseX );
					this.Rotate( Vector3.Up, m_MouseY );

					this.RecalculateAxes( );

					Matrix Axes = Matrix.Identity;

					Axes.M11 = m_LocalRight.X;
					Axes.M21 = m_LocalRight.Y;
					Axes.M31 = m_LocalRight.Z;

					Axes.M12 = m_LocalUp.X;
					Axes.M22 = m_LocalUp.Y;
					Axes.M32 = m_LocalUp.Z;

					Axes.M13 = -m_LocalForward.X;
					Axes.M23 = -m_LocalForward.Y;
					Axes.M33 = -m_LocalForward.Z;

					Vector3 CameraPosition = Vector3.Transform(
						-m_CameraPosition, Axes );
					System.Diagnostics.Debug.WriteLine( "Position: " +
						m_CameraPosition.ToString( ) );


					

					Matrix Rotation = //Matrix.CreateFromQuaternion( m_Orientation );
						Matrix.CreateRotationX( m_Orbit.X ) *
						Matrix.CreateRotationY( m_Orbit.Y );

					//m_CameraPosition = Vector3.Transform( m_CameraPosition, Rotation );

					Position = Vector3.Transform( m_CameraLookAt, Matrix.Invert( Rotation ) );


					/*System.Diagnostics.Debug.WriteLine( "Old Position: " +
						m_CameraPosition.ToString( ) );*/

					m_CameraLookAt += Position;
					/*
					System.Diagnostics.Debug.WriteLine( "New Position: " +
						m_CameraPosition.ToString( ) );*/

				}
				else if( m_Panning )
				{
					Vector3 PanPositionY = new Vector3(
						0.0f,
						m_YMouseDelta,
						0.0f );//m_CameraPosition.Z );
					Vector3 PanPositionX = new Vector3(
						m_YMouseDelta,
						0.0f, 0.0f );

					Vector3 PanDirection, PanRight, PanUp;

					//PanDirection = m_CameraLookAt - m_CameraPosition;
					PanDirection = Vector3.Forward;
					PanDirection.Normalize( );

					PanRight = Vector3.Cross( PanDirection, Vector3.Up );
					PanRight.Normalize( );

					PanUp = Vector3.Cross( PanRight, PanDirection );
					PanUp.Normalize( );

					Matrix Transform = new Matrix( );
					Transform.Right = PanRight;
					Transform.Up = PanUp;
					Transform.Forward = PanDirection;

					m_CameraPosition.Y += Vector3.Transform( PanPositionY,
						Transform ).Y;

					m_CameraPosition.X += Vector3.Transform( PanPositionX,
						Transform ).X;
					m_CameraLookAt = Vector3.Transform( Vector3.Forward,
						Transform ) + m_CameraPosition;

					/*
					m_CameraPosition.X *= PanRight.X *
						m_XMouseDelta;

					m_CameraPosition.Y *= PanUp.Y * m_YMouseDelta;*/

					/*System.Diagnostics.Debug.WriteLine( "Up: " + PanUp.ToString( ) );
					System.Diagnostics.Debug.WriteLine( "Right: " + PanRight.ToString( ) );
					System.Diagnostics.Debug.WriteLine( "Direction: " + PanDirection.ToString( ) );
					System.Diagnostics.Debug.WriteLine( "Camera: " + m_CameraPosition.ToString( ) );*/
				}
			}

			base.OnMouseMove( p_Args );
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

		private Vector3		m_CameraPosition;
		private Vector3		m_CameraLookAt;
		private Vector2		m_Orbit;

		private float	m_MouseX;
		private float	m_MouseY;
		private float	m_XMouseDelta;
		private float	m_YMouseDelta;
		private bool	m_MiddleButtonDown;
		private bool	m_Orbiting;
		private bool	m_Panning;
		private bool	m_Dollying;

		private Quaternion	m_Orientation;
		private Quaternion	m_Rotation;
		private Vector3		m_LocalUp, m_LocalRight, m_LocalForward;
		private Vector3		m_CameraPositionTemp;

		private GamePadState m_PreviousGamePadState;
	}
}
