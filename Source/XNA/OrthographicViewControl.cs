using System.Diagnostics;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace BloodBulletEditor
{
	public enum VIEWPLANE
	{
		VIEWPLANE_XY,
		VIEWPLANE_XZ,
		VIEWPLANE_YZ
	};

	class OrthographicViewControl : GraphicsDeviceControl
	{
		public OrthographicViewControl( VIEWPLANE p_ViewPlane ) :
			base( )
		{
			this.Reset( p_ViewPlane );
		}

		protected override int Initialise( )
		{
            m_HasGWFL = false;
			m_PerspectiveView = false;
			Application.Idle += delegate { Invalidate( ); };

			m_Scale = 1.0f;
			m_XDelta = 0.0f;
			m_YDelta = 0.0f;
			m_ZDelta = 0.0f;

			m_Grid = new Grid( this.GraphicsDevice );
			m_Grid.Create( m_ViewPlane, 1000, 1000, 10.0f, 0.0f,
				new Color( 32, 32, 128 ), 10, Color.Blue );

			MenuItem [ ] ViewportMenu = new MenuItem[ 1 ];
			ViewportMenu[ 0 ] = new MenuItem( "Change Viewport" );

			MenuItem [ ] ViewportSubMenu = new MenuItem[ 3 ];
			ViewportSubMenu[ 0 ] = new MenuItem( "Front",
				ViewportSubMenu_Front_Click );
			ViewportSubMenu[ 1 ] = new MenuItem( "Side",
				ViewportSubMenu_Side_Click );
			ViewportSubMenu[ 2 ] = new MenuItem( "Top",
				ViewportSubMenu_Top_Click );

			for( int i = 0; i < 3; ++i )
			{
				ViewportMenu[ 0 ].MenuItems.Add( ViewportSubMenu[ i ] );
			}
			m_ContextMenu = new ContextMenu( ViewportMenu );

			this.ContextMenu = m_ContextMenu;

			m_Effect = new BasicEffect( GraphicsDevice );

            m_ScreenRender = new RenderTarget2D( GraphicsDevice, this.Width, this.Height );
            m_SpriteBatch = new SpriteBatch( this.GraphicsDevice );

			return 0;
		}

		private int Reset( VIEWPLANE p_ViewPlane )
		{
			m_Scale = 1.0f;
			m_WorldScale = 1.0f;
			m_XDelta = 0.0f;
			m_YDelta = 0.0f;
			m_ZDelta = 0.0f;

			m_ViewPlane = p_ViewPlane;

			m_CameraPosition = Vector3.Zero;
			m_LookPoint = Vector3.Zero;

			m_ClearColour = new Color( 8, 8, 8 );

			switch( m_ViewPlane )
			{
				case VIEWPLANE.VIEWPLANE_XY:
				{
					this.Name = "Orthographic View [Front]";
					m_CameraPosition.Z = -1000.0f;
					break;
				}
				case VIEWPLANE.VIEWPLANE_XZ:
				{
					this.Name = "Orthographic View [Top]";
					m_CameraPosition.Y = 1000.0f;
					break;
				}
				case VIEWPLANE.VIEWPLANE_YZ:
				{
					this.Name = "Orthographic View [Side]";
					m_CameraPosition.X = 1000.0f;
					break;
				}
			}
			
			m_MiddleButtonDown = false;

			if( m_Grid != null )
			{
				m_Grid.ViewPlane = m_ViewPlane;
			}

			return 0;
		}

		private void ViewportSubMenu_Front_Click( object p_Sender,
			EventArgs p_Args )
		{
			this.Reset( VIEWPLANE.VIEWPLANE_XY );
		}
		private void ViewportSubMenu_Side_Click( object p_Sender,
			EventArgs p_Args )
		{
			this.Reset( VIEWPLANE.VIEWPLANE_YZ );
		}
		private void ViewportSubMenu_Top_Click( object p_Sender,
			EventArgs p_Args )
		{
			this.Reset( VIEWPLANE.VIEWPLANE_XZ );
		}

		protected override void Draw( )
		{
            if( Width != m_ScreenRender.Width ||
                Height != m_ScreenRender.Height )
            {
                m_ScreenRender = null;
                m_ScreenRender = new RenderTarget2D( this.GraphicsDevice, this.Width,
                    this.Height );
            }

            this.GraphicsDevice.SetRenderTarget( m_ScreenRender );
            this.GraphicsDevice.Clear( m_ClearColour );

			m_ProjectionMatrix = Matrix.CreateOrthographic(
				GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height,
				0.1f, 1000000.0f );

			Vector3 Up = Vector3.Up;

			m_LookPoint = m_CameraPosition;

			switch( m_ViewPlane )
			{
				case VIEWPLANE.VIEWPLANE_XY:
				{
					m_LookPoint.Z = 0.0f;
					break;
				}
				case VIEWPLANE.VIEWPLANE_XZ:
				{
					m_LookPoint.Y = 0.0f;
					Up = Vector3.Forward;
					break;
				}
				case VIEWPLANE.VIEWPLANE_YZ:
				{
					m_LookPoint.X = 0.0f;
					break;
				}
			}

			m_ViewMatrix = Matrix.CreateLookAt( m_CameraPosition * m_Scale,
				m_LookPoint * m_Scale, Up );

			switch( m_ViewPlane )
			{
				case VIEWPLANE.VIEWPLANE_XY:
				{
					m_CameraPosition.X += m_XDelta;
					m_CameraPosition.Y += m_YDelta;
					break;
				}
				case VIEWPLANE.VIEWPLANE_XZ:
				{
					m_CameraPosition.X += m_XDelta;
					m_CameraPosition.Z += m_ZDelta;
					break;
				}
				case VIEWPLANE.VIEWPLANE_YZ:
				{
					m_CameraPosition.Y += m_YDelta;
					m_CameraPosition.Z += m_ZDelta;
					break;
				}
				default:
				{
					break;
				}
			}

			m_Scale += m_ScaleAdd;

			if( m_Scale < 0.1f )
			{
				m_Scale = 0.1f;
			}
			if( m_Scale > 30.0f )
			{
				m_Scale = 30.0f;
			}

			if( m_Scale >= 8.0f )
			{
				m_Label.Text = this.Name + " [cm]";
				m_WorldScale = 0.1f;
			}
			else if( m_Scale <= 0.6699997f )
			{
				m_Label.Text = this.Name + " [1m]";
				m_WorldScale = 10.0f;
			}
			else
			{
				m_Label.Text = this.Name + " [10cm]";
				m_WorldScale = 1.0f;
			}

			m_WorldMatrix = Matrix.CreateScale( m_Scale * m_WorldScale );

			m_Grid.Render( m_WorldMatrix, m_ViewMatrix, m_ProjectionMatrix );

			m_Effect.World = Matrix.CreateScale( m_Scale );
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

			m_ScaleAdd = 0.0f;
			m_XDelta = 0.0f;
			m_YDelta = 0.0f;
			m_ZDelta = 0.0f;

            this.GraphicsDevice.SetRenderTarget( null );

			m_SpriteBatch.Begin( );
			m_SpriteBatch.Draw(( Texture2D ) m_ScreenRender,
				new Rectangle( 0, 0, m_ScreenRender.Width, m_ScreenRender.Height ),/*
				GraphicsDevice.PresentationParameters.BackBufferWidth,
					GraphicsDevice.PresentationParameters.BackBufferHeight ),*/
					new Rectangle( 0, 0, Width, Height ),
					Color.White );
			m_SpriteBatch.End( );
		}

		public VIEWPLANE ViewPlane
		{
			get
			{
				return m_ViewPlane;
			}
			set
			{
				m_ViewPlane = value;
			}
		}

		protected override void OnMouseWheel( MouseEventArgs p_Args )
		{
			if( p_Args.Delta > 0 )
			{
				if( m_Scale > 1.0f )
				{
					m_ScaleAdd = 0.3f;
				}
				else
				{
					m_ScaleAdd = 0.01f;
				}
			}
			if( p_Args.Delta < 0 )
			{
				if( m_Scale > 1.0f )
				{
					m_ScaleAdd = -0.3f;
				}
				else
				{
					m_ScaleAdd = -0.01f;
				}
			}
		}

		protected override void OnMouseDown( MouseEventArgs p_Args )
		{
			if( p_Args.Button == System.Windows.Forms.MouseButtons.Middle )
			{
				m_MouseX = p_Args.X;
				m_MouseY = p_Args.Y;

				m_MiddleButtonDown = true;
			}
		}

		protected override void OnMouseUp( MouseEventArgs p_Args )
		{
			if( p_Args.Button == System.Windows.Forms.MouseButtons.Middle )
			{
				m_MouseX = 0.0f;
				m_MouseY = 0.0f;
				m_XDelta = 0.0f;
				m_YDelta = 0.0f;
				m_ZDelta = 0.0f;

				m_MiddleButtonDown = false;
			}
		}

		protected override void OnMouseMove( MouseEventArgs p_Args )
		{
			if( m_MiddleButtonDown )
			{
				switch( m_ViewPlane )
				{
					case VIEWPLANE.VIEWPLANE_XY:
					{
						m_XDelta = ( p_Args.X - m_MouseX );
						m_YDelta = ( p_Args.Y - m_MouseY );
						m_MouseX = p_Args.X;
						m_MouseY = p_Args.Y;
						break;
					}
					case VIEWPLANE.VIEWPLANE_XZ:
					{
						m_XDelta = -( p_Args.X - m_MouseX );
						m_ZDelta = -( p_Args.Y - m_MouseY );
						m_MouseX = p_Args.X;
						m_MouseY = p_Args.Y;
						break;
					}
					case VIEWPLANE.VIEWPLANE_YZ:
					{
						m_YDelta = ( p_Args.Y - m_MouseY );
						m_ZDelta = ( p_Args.X - m_MouseX );
						m_MouseX = p_Args.X;
						m_MouseY = p_Args.Y;
						break;
					}
				}
			}
		}

		private VIEWPLANE			m_ViewPlane;
		private Matrix				m_WorldMatrix;
		private Matrix				m_ViewMatrix;
		private Matrix				m_ProjectionMatrix;
		private Vector3				m_CameraPosition;
		private float				m_Scale;
		private float				m_ScaleAdd;
		private float				m_XDelta, m_YDelta, m_ZDelta;
		private float				m_MouseX, m_MouseY;
		private bool				m_MiddleButtonDown;
		private float				m_WorldScale;
		private Grid				m_Grid;
		private ContextMenu			m_ContextMenu;
		private Vector3				m_LookPoint;
		private BasicEffect		    m_Effect;
        private RenderTarget2D      m_ScreenRender;
        private SpriteBatch         m_SpriteBatch;
	}
}
