using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BloodBulletEditor
{
	public class Grid
	{
		public Grid( GraphicsDevice p_GraphicsDevice )
		{
			m_GraphicsDevice = p_GraphicsDevice;
			m_Effect = new BasicEffect( m_GraphicsDevice );
		}

		public int Create( VIEWPLANE p_Type, int p_Width, int p_Height,
			float p_Stride, float p_Offset, Color p_NormalColour,
			int p_HeavyDivision, Color p_HeavyColour )
		{
			m_Effect.Alpha = 1.0f;
			m_Effect.LightingEnabled = false;
			m_Effect.VertexColorEnabled = true;

			Vector3 [ ] Vertices = new Vector3[ ( ( p_Width + p_Height ) * 2 ) + 4 ];
			float HalfColumns = p_Width / 2;
			float HalfRows = p_Height / 2;
			float RowStart = -HalfRows * p_Stride;
			float ColumnStart = -HalfColumns * p_Stride;
			float RowIndex = RowStart;
			float ColumnIndex = ColumnStart;
			int Row = 0;
			int Column = 0;
			m_ViewPlane = p_Type;

			for( Row = 0; Row < p_Height * 2; ++Row )
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

				RowIndex += p_Stride;
			}

			// Add the last edge to the row
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
					Vertices[ Row ] = new Vector3( 0.0f, RowIndex, ColumnStart );
					break;
				}
			}

			++Row;

			for( Column = 0; Column < p_Width * 2; ++Column )
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
						Vertices[ Row + Column ] = new Vector3( ColumnIndex,
							0.0f, RowStart );
						++Column;
						Vertices[ Row + Column ] = new Vector3( ColumnIndex,
							0.0f, -RowStart );
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

				ColumnIndex += p_Stride;
			}

			// Add the last edge to the column
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
					Vertices[ Row + Column ] = new Vector3( ColumnIndex,
						0.0f, RowStart );
					++Column;
					Vertices[ Row + Column ] = new Vector3( ColumnIndex,
						0.0f, -RowStart );
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

			m_Vertices = new VertexPositionColor[ p_Width * p_Height * 2 ];

			int LineCount = 0;

			for( int i = 0; i < ( p_Height * 2 ) + 2; ++i )
			{
				m_Vertices[ i ].Position = Vertices[ i ];
				if( ( i % ( p_HeavyDivision * 2 ) ) == 0 )
				{
					m_Vertices[ i ].Color = p_HeavyColour;
					m_Vertices[ i + 1 ].Color = p_HeavyColour;
					++i;
					++LineCount;
					m_Vertices[ i ].Position = Vertices[ i ];
				}
				else
				{
					m_Vertices[ i ].Color = p_NormalColour;
				}
				++LineCount;
			}

			for( int i = 0; i < ( p_Width * 2 ) + 2; ++i )
			{
				m_Vertices[ i + LineCount ].Position = Vertices[ i + LineCount ];
				if( ( i % ( p_HeavyDivision *2 ) ) == 0 )
				{
					m_Vertices[ i + LineCount ].Color = p_HeavyColour;
					m_Vertices[ i + LineCount + 1 ].Color = p_HeavyColour;
					++i;
					m_Vertices[ i + LineCount ].Position = Vertices[ i + LineCount ];
				}
				else
				{
					m_Vertices[ i + LineCount ].Color = p_NormalColour;
				}
			}

			m_VertexBuffer = new VertexBuffer( m_GraphicsDevice,
				typeof( VertexPositionColor ), ( p_Width * p_Height * 2 ) + 4,
				BufferUsage.None );

			m_VertexBuffer.SetData< VertexPositionColor >( m_Vertices );

			return 0;
		}

		public void Render( Matrix p_World, Matrix p_View, Matrix p_Projection )
		{
			m_Effect.World = p_World;
			m_Effect.View = p_View;
			m_Effect.Projection = p_Projection;

			RasterizerState RasterState = new RasterizerState( );
			RasterizerState OldState = m_GraphicsDevice.RasterizerState;
			RasterState.CullMode = CullMode.None;
			m_GraphicsDevice.RasterizerState = RasterState;

			m_GraphicsDevice.SetVertexBuffer( m_VertexBuffer );

			foreach( EffectPass Pass in m_Effect.CurrentTechnique.Passes )
			{
				Pass.Apply( );
				m_GraphicsDevice.DrawPrimitives( PrimitiveType.LineList, 0,
					m_VertexBuffer.VertexCount / 2 );
			}

			m_GraphicsDevice.RasterizerState = OldState;
		}

		public VertexBuffer VertexBuffer
		{
			get
			{
				return m_VertexBuffer;
			}
		}

		private VIEWPLANE			m_ViewPlane;
		private BasicEffect			m_Effect;
		private VertexBuffer		m_VertexBuffer;
		private VertexPositionColor	[ ] m_Vertices;
		private GraphicsDevice		m_GraphicsDevice;
	}
}
