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
					m_ZPos = 1.0f;
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

			m_MouseWheelDelta = 0;
		}

		protected override int Initialise( )
		{
			Application.Idle += delegate { Invalidate( ); };


			m_Effect = new BasicEffect( this.GraphicsDevice );
			m_Effect.AmbientLightColor = new Vector3( 1.0f, 1.0f, 1.0f );
			m_Effect.Alpha = 1.0f;
			m_Effect.DiffuseColor = new Vector3( 1.0f, 1.0f, 1.0f );
			m_Effect.LightingEnabled = false;

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
			Vector3 [ ] Vertices = new Vector3[ 100*4 ];
			float HalfRows = 100 / 2;
			float HalfColumns = 100 / 2;
			float RowStart = -HalfRows * Stride;
			float ColumnStart = -HalfColumns * Stride;
			float RowIndex = RowStart;
			float ColumnIndex = ColumnStart;
			int Row = 0;

			for( Row = 0; Row < 100*2; ++Row )
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
						Vertices[ Row ] = new Vector3( RowIndex, 0.0f, ColumnStart );
						++Row;
						Vertices[ Row ] = new Vector3( RowIndex, 0.0f, -ColumnStart );
						break;
					}
					case VIEWPLANE.VIEWPLANE_YZ:
					{
						Vertices[ Row ] = new Vector3( 0.0f, ColumnStart, RowIndex );
						++Row;
						Vertices[ Row ] = new Vector3( 0.0f, -ColumnStart, RowIndex );
						break;
					}
				}

				RowIndex += Stride;
			}

			for( int Column = 0; Column < 100*2; ++Column )
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
							ColumnIndex, RowStart );
						++Column;
						Vertices[ Row + Column ] = new Vector3( 0.0f,
							ColumnIndex, -RowStart );
						break;
					}
				}
				
				ColumnIndex += Stride;
			}

			m_Vertices = new VertexPositionColor[ 100*4 ];

			for( int i = 0; i < 100*4; ++i )
			{
				m_Vertices[ i ].Color = Color.White;
				m_Vertices[ i ].Position = Vertices[ i ];
			}

			m_VertexBuffer = new VertexBuffer( GraphicsDevice,
				typeof( VertexPositionColor ),
				400, BufferUsage.None );

			m_VertexBuffer.SetData< VertexPositionColor >( m_Vertices );

			m_Scale = 1.0f;

			return 0;
		}

		protected override void Draw( )
		{
			m_ProjectionMatrix = Matrix.CreateOrthographic(
				GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height,
				1.0f, 1000000.0f );
				
			m_ViewMatrix = Matrix.CreateLookAt(
				new Vector3( m_XPos, m_YPos, m_ZPos ), Vector3.Zero, Vector3.Up );

			switch( m_ViewPlane )
			{
				case VIEWPLANE.VIEWPLANE_XY:
				{
					break;
				}
				case VIEWPLANE.VIEWPLANE_XZ:
				{
					break;
				}
				case VIEWPLANE.VIEWPLANE_YZ:
				{
					break;
				}
				default:
				{
					break;
				}
			}

			m_Scale += m_ScaleAdd;

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

				GraphicsDevice.DrawPrimitives( PrimitiveType.LineList, 0, 200 );
			}

			m_ScaleAdd = 0.0f;
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
				m_ScaleAdd = 0.01f;
			}
			if( p_Args.Delta < 0 )
			{
				m_ScaleAdd = -0.01f;
			}
		}

		protected override void OnMouseDown( MouseEventArgs p_Args )
		{
			if( p_Args.Button == System.Windows.Forms.MouseButtons.Middle )
			{
				// Set grab
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
		private int					m_MouseWheelDelta;
	}
}
