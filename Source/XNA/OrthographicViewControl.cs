using System.Diagnostics;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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
			m_ViewPlane = p_ViewPlane;

			m_ClearColour = new Color( 32, 32, 32 );

			m_YPos = 0.0f;
			m_XPos = 0.0f;
			m_ZPos = 0.0f;

			switch( m_ViewPlane )
			{
				case VIEWPLANE.VIEWPLANE_XY:
				{
					this.Name = "Orthographic View [Front]";
					m_ZPos = -1.0f;
					break;
				}
				case VIEWPLANE.VIEWPLANE_XZ:
				{
					this.Name = "Orthographic View [Top]";
					m_YPos = 1.0f;
					break;
				}
				case VIEWPLANE.VIEWPLANE_YZ:
				{
					this.Name = "Orthographic View [Side]";
					m_XPos = 1.0f;
					break;
				}
				default:
				{
					break;
				}
			}
			
			m_MiddleButtonDown = false;
		}

		protected override int Initialise( )
		{
			Application.Idle += delegate { Invalidate( ); };

			m_Effect = new BasicEffect( this.GraphicsDevice );
			m_Effect.Alpha = 1.0f;
			m_Effect.LightingEnabled = false;
			m_Effect.VertexColorEnabled = true;

			m_VertexDeclaration = new VertexDeclaration( new VertexElement [ ]
				{
					new VertexElement( 0, VertexElementFormat.Vector3,
						VertexElementUsage.Position, 0 ),
					new VertexElement( 12, VertexElementFormat.Vector3,
						VertexElementUsage.Color, 0 )
				}
				);

			// Create a 100x100 grid, spaced 10 units apart
			float Stride = 10.0f;
			Vector3 [ ] Vertices = new Vector3[ 1000*4 + 4 ];
			float HalfRows = 1000 / 2;
			float HalfColumns = 1000 / 2;
			float RowStart = -HalfRows * Stride;
			float ColumnStart = -HalfColumns * Stride;
			float RowIndex = RowStart;
			float ColumnIndex = ColumnStart;
			int Row = 0;
			int Column = 0;

			for( Row = 0; Row < 1000*2; ++Row )
			{
				switch( m_ViewPlane )
				{
					case VIEWPLANE.VIEWPLANE_XY:
					{
						Vertices[ Row ] = new Vector3( ColumnStart, RowIndex, 0.0f );
						++Row;
						Vertices[ Row ] = new Vector3( -ColumnStart, RowIndex, 0.0f );
						break;
					}
					case VIEWPLANE.VIEWPLANE_XZ:
					{
						/*Vertices[ Row ] = new Vector3( RowIndex, 0.0f, ColumnStart );
						++Row;
						Vertices[ Row ] = new Vector3( RowIndex, 0.0f, -ColumnStart );*/
						Vertices[ Row ] = new Vector3( ColumnStart, 0.0f, RowIndex );
						++Row;
						Vertices[ Row ] = new Vector3( -ColumnStart, 0.0f, RowIndex );
						break;
					}
					case VIEWPLANE.VIEWPLANE_YZ:
					{
						Vertices[ Row ] = new Vector3( 0.0f, RowIndex, ColumnStart );
						++Row;
						Vertices[ Row ] = new Vector3( 0.0f, RowIndex, -ColumnStart );
						break;
					}
				}

				RowIndex += Stride;
			}

			// Add one final row edge
			switch( m_ViewPlane )
			{
				case VIEWPLANE.VIEWPLANE_XY:
				{
					Vertices[ Row ] = new Vector3( ColumnStart, RowIndex, 0.0f );
					++Row;
					Vertices[ Row ] = new Vector3( -ColumnStart, RowIndex, 0.0f );
					break;
				}
				case VIEWPLANE.VIEWPLANE_XZ:
				{
					Vertices[ Row ] = new Vector3( RowIndex, 0.0f, ColumnStart );
					++Row;
					Vertices[ Row ] = new Vector3( RowIndex, 0.0f, -ColumnStart );
					break;
				}
				case VIEWPLANE.VIEWPLANE_YZ:
				{
					Vertices[ Row ] = new Vector3( 0.0f, RowIndex, ColumnStart );
					++Row;
					Vertices[ Row ] = new Vector3( 0.0f, RowIndex, -ColumnStart );
					break;
				}
			}

			++Row;

			for( Column = 0; Column < 1000*2; ++Column )
			{
				switch( m_ViewPlane )
				{
					case VIEWPLANE.VIEWPLANE_XY:
					{
						Vertices[ Row + Column ] = new Vector3( ColumnIndex,
							RowStart, 0.0f );
						++Column;
						Vertices[ Row + Column ] = new Vector3( ColumnIndex,
							-RowStart, 0.0f );
						break;
					}
					case VIEWPLANE.VIEWPLANE_XZ:
					{
						Vertices[ Row + Column ] = new Vector3( RowStart,
							0.0f, ColumnIndex );
						++Column;
						Vertices[ Row + Column ] = new Vector3( -RowStart,
							0.0f, ColumnIndex );
						break;
					}
					case VIEWPLANE.VIEWPLANE_YZ:
					{
						Vertices[ Row + Column ] = new Vector3( 0.0f,
							RowStart, ColumnIndex );
						++Column;
						Vertices[ Row + Column ] = new Vector3( 0.0f,
							-RowStart, ColumnIndex );
						break;
					}
				}
				
				ColumnIndex += Stride;
			}

			switch( m_ViewPlane )
			{
				case VIEWPLANE.VIEWPLANE_XY:
				{
					Vertices[ Row + Column ] = new Vector3( ColumnIndex,
						RowStart, 0.0f );
					++Column;
					Vertices[ Row + Column ] = new Vector3( ColumnIndex, 
						-RowStart, 0.0f );
					break;
				}
				case VIEWPLANE.VIEWPLANE_XZ:
				{
					Vertices[ Row + Column ] = new Vector3( RowStart,
						0.0f, ColumnIndex );
					++Column;
					Vertices[ Row + Column ] = new Vector3( -RowStart,
						0.0f, ColumnIndex );
					break;
				}
				case VIEWPLANE.VIEWPLANE_YZ:
				{
					Vertices[ Row + Column ] = new Vector3( 0.0f,
						RowStart, ColumnIndex );
					++Column;
					Vertices[ Row + Column ] = new Vector3( 0.0f,
						-RowStart, ColumnIndex );
					break;
				}
			}

			m_Vertices = new VertexPositionColor[ 1000*4 + 4 ];

			int LineCount = 0;

			for( int i = 0; i < 1000*2 + 2; ++i )
			{
				m_Vertices[ i ].Position = Vertices[ i ];
				if( ( i % 20 ) == 0 )
				{
					m_Vertices[ i ].Color = new Color( 0, 0, 255 );
					m_Vertices[ i + 1 ].Color = new Color( 0, 0, 255 );
					++i;
					++LineCount;
					m_Vertices[ i ].Position = Vertices[ i ];
				}
				else
				{
					m_Vertices[ i ].Color = new Color( 32, 32, 128 );
				}
				++LineCount;
			}
			for( int i = 0; i < 1000*2 + 2; ++i )
			{
				m_Vertices[ i + LineCount ].Position = Vertices[ i + LineCount ];
				if( ( i % 20 ) == 0 )
				{
					m_Vertices[ i + LineCount ].Color = new Color( 0, 0, 255 );
					m_Vertices[ i + LineCount + 1 ].Color = new Color( 0, 0, 255 );
					++i;
					m_Vertices[ i + LineCount ].Position = Vertices[ i + LineCount ];
				}
				else
				{
					m_Vertices[ i + LineCount ].Color = new Color( 32, 32, 128 );
				}
			}

			m_VertexBuffer = new VertexBuffer( GraphicsDevice,
				typeof( VertexPositionColor ),
				4004, BufferUsage.None );

			m_VertexBuffer.SetData< VertexPositionColor >( m_Vertices );

			m_Scale = 1.0f;
			m_XDelta = 0.0f;
			m_YDelta = 0.0f;
			m_ZDelta = 0.0f;

			return 0;
		}

		protected override void Draw( )
		{
			m_ProjectionMatrix = Matrix.CreateOrthographic(
				GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height,
				1.0f, 1000000.0f );

			Vector3 LookPoint = new Vector3( 0.0f, 0.0f, 0.0f );

			switch( m_ViewPlane )
			{
				case VIEWPLANE.VIEWPLANE_XY:
				{
					LookPoint.X = m_XPos;
					LookPoint.Y = m_YPos;
					LookPoint.Z = 0.0f;
					break;
				}
				case VIEWPLANE.VIEWPLANE_XZ:
				{
					LookPoint.X = m_XPos;
					LookPoint.Y = 0.0f;
					LookPoint.Z = m_ZPos;
					break;
				}
				case VIEWPLANE.VIEWPLANE_YZ:
				{
					LookPoint.X = 0.0f;
					LookPoint.Y = m_YPos;
					LookPoint.Z = m_ZPos;
					break;
				}
			}
				
			m_ViewMatrix = Matrix.CreateLookAt(
				new Vector3( m_XPos, m_YPos, m_ZPos ), LookPoint, Vector3.Up );

			switch( m_ViewPlane )
			{
				case VIEWPLANE.VIEWPLANE_XY:
				{
					m_XPos += m_XDelta;
					m_YPos += m_YDelta;
					break;
				}
				case VIEWPLANE.VIEWPLANE_XZ:
				{
					m_XPos += m_XDelta;
					m_ZPos += m_ZDelta;
					break;
				}
				case VIEWPLANE.VIEWPLANE_YZ:
				{
					m_YPos += m_YDelta;
					m_ZPos += m_ZDelta;
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

			GraphicsDevice.SetVertexBuffer( m_VertexBuffer );

			m_WorldMatrix = Matrix.CreateScale( m_Scale );

			m_Effect.View = m_ViewMatrix;
			m_Effect.Projection = m_ProjectionMatrix;
			m_Effect.World = m_WorldMatrix;

			RasterizerState RasterState = new RasterizerState( );
			RasterState.CullMode = CullMode.None;
			GraphicsDevice.RasterizerState = RasterState;
			
			foreach( EffectPass Pass in m_Effect.CurrentTechnique.Passes )
			{
				Pass.Apply( );

				GraphicsDevice.DrawPrimitives( PrimitiveType.LineList, 0, 2002 );
			}

			m_ScaleAdd = 0.0f;
			m_XDelta = 0.0f;
			m_YDelta = 0.0f;
			m_ZDelta = 0.0f;
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
						m_XDelta = ( p_Args.X - m_MouseX ) * m_Scale;
						m_YDelta = ( p_Args.Y - m_MouseY ) * m_Scale;
						m_MouseX = p_Args.X;
						m_MouseY = p_Args.Y;
						break;
					}
					case VIEWPLANE.VIEWPLANE_XZ:
					{
						m_XDelta = ( p_Args.X - m_MouseX ) * m_Scale;
						m_ZDelta = ( p_Args.Y - m_MouseY ) * m_Scale;
						m_MouseX = p_Args.X;
						m_MouseY = p_Args.Y;
						break;
					}
					case VIEWPLANE.VIEWPLANE_YZ:
					{
						m_YDelta = ( p_Args.Y - m_MouseY ) * m_Scale;
						m_ZDelta = ( p_Args.X - m_MouseX ) * m_Scale;
						m_MouseX = p_Args.X;
						m_MouseY = p_Args.Y;
						break;
					}
				}
			}
		}

		private VIEWPLANE			m_ViewPlane;
		private BasicEffect			m_Effect;
		private VertexBuffer		m_VertexBuffer;
		private VertexDeclaration	m_VertexDeclaration;
		private VertexPositionColor	[ ] m_Vertices;
		private Matrix				m_WorldMatrix;
		private Matrix				m_ViewMatrix;
		private Matrix				m_ProjectionMatrix;
		private float				m_YPos, m_XPos, m_ZPos;
		private float				m_Scale;
		private float				m_ScaleAdd;
		private float				m_XDelta, m_YDelta, m_ZDelta;
		private float				m_MouseX, m_MouseY;
		private bool				m_MiddleButtonDown;
	}
}
