using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Runtime.InteropServices;

namespace BloodBulletEditor.Game
{
	[StructLayout( LayoutKind.Sequential )]
	public struct CUBE_VERTEX
	{
		public Vector3	Position;
		public Color	Colour;
	}

	public class Cube
	{
		public Cube( GraphicsDevice p_GraphicsDevice,
			Vector3 p_Minimum, Vector3 p_Maximum, Color p_Colour )
		{
			CUBE_VERTEX [ ] Vertices = new CUBE_VERTEX[ 8 ];

			for( int i = 0; i < 8; ++i )
			{
				Vertices[ i ].Colour = p_Colour;
			}

			// Bottom
			Vertices[ 0 ].Position = p_Minimum;
			Vertices[ 1 ].Position = p_Minimum;
			Vertices[ 1 ].Position.Z = p_Maximum.Z;
			Vertices[ 2 ].Position = p_Maximum;
			Vertices[ 2 ].Position.Y = p_Minimum.Y;
			Vertices[ 3 ].Position = p_Minimum;
			Vertices[ 3 ].Position.X = p_Maximum.X;

			// Top
			Vertices[ 4 ].Position = p_Minimum;
			Vertices[ 4 ].Position.Y = p_Maximum.Y;
			Vertices[ 5 ].Position = p_Maximum;
			Vertices[ 5 ].Position.X = p_Minimum.X;
			Vertices[ 6 ].Position = p_Maximum;
			Vertices[ 7 ].Position = p_Maximum;
			Vertices[ 7 ].Position.Z = p_Minimum.Z;

			int Stride = ( 4 * 3 ) + 4;
			m_VertexData = new byte[ Stride * 8 ];

			VertexElement [ ] Elements = new VertexElement[ 2 ];

			Elements[ 0 ] = new VertexElement( 0, VertexElementFormat.Vector3,
				VertexElementUsage.Position, 0 );
			Elements[ 1 ] = new VertexElement( 12, VertexElementFormat.Color,
				VertexElementUsage.Color, 0 );

			VertexDeclaration Declaration = new VertexDeclaration( Stride,
				Elements );

			m_VertexBuffer = new VertexBuffer( p_GraphicsDevice,
				Declaration, 8, BufferUsage.None );

			m_VertexBuffer.SetData< CUBE_VERTEX >( Vertices );

			m_IndexBuffer = new IndexBuffer( p_GraphicsDevice,
				IndexElementSize.SixteenBits, 36, BufferUsage.None );

			m_IndexData = new UInt16[ 36 ];

			for( int i = 0; i < 36; ++i )
			{
				m_IndexData[ i ] = 0;
			}

			// Bottom
			m_IndexData[ 0 ] = 0;
			m_IndexData[ 1 ] = 1;
			m_IndexData[ 2 ] = 2;
			m_IndexData[ 3 ] = 2;
			m_IndexData[ 4 ] = 3;
			m_IndexData[ 5 ] = 0;
			
			// Top
			m_IndexData[ 6 ] = 4;
			m_IndexData[ 7 ] = 5;
			m_IndexData[ 8 ] = 6;
			m_IndexData[ 9 ] = 6;
			m_IndexData[ 10 ] = 7;
			m_IndexData[ 11 ] = 4;
			
			// Back
			m_IndexData[ 18 ] = 2;
			m_IndexData[ 19 ] = 6;
			m_IndexData[ 20 ] = 5;
			m_IndexData[ 21 ] = 5;
			m_IndexData[ 22 ] = 1;
			m_IndexData[ 23 ] = 2;
			
			// Front
			m_IndexData[ 12 ] = 0;
			m_IndexData[ 13 ] = 4;
			m_IndexData[ 14 ] = 7;
			m_IndexData[ 15 ] = 7;
			m_IndexData[ 16 ] = 3;
			m_IndexData[ 17 ] = 0;

			// Left
			m_IndexData[ 24 ] = 1;
			m_IndexData[ 25 ] = 5;
			m_IndexData[ 26 ] = 4;
			m_IndexData[ 27 ] = 4;
			m_IndexData[ 28 ] = 0;
			m_IndexData[ 29 ] = 1;

			// Right
			m_IndexData[ 30 ] = 3;
			m_IndexData[ 31 ] = 7;
			m_IndexData[ 32 ] = 6;
			m_IndexData[ 33 ] = 6;
			m_IndexData[ 34 ] = 2;
			m_IndexData[ 35 ] = 3;

			m_IndexBuffer.SetData< UInt16 >( m_IndexData );
		}

		public VertexBuffer VertexBuffer
		{
			get
			{
				return m_VertexBuffer;
			}
		}

		public IndexBuffer IndexBuffer
		{
			get
			{
				return m_IndexBuffer;
			}
		}

		private byte	[ ] m_VertexData;
		private UInt16	[ ] m_IndexData;

		private VertexBuffer	m_VertexBuffer;
		private IndexBuffer		m_IndexBuffer;
	}
}
