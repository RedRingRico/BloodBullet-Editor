using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BloodBulletEditor.CSG
{
	public struct CSGVertex
	{
		Vector3	Position;
		Vector3	Colour;
		Vector3 Normal;
	}

	public class CSGPrimitive
	{
		public CSGPrimitive( ref GraphicsDevice p_GraphicsDevice )
		{
			m_GraphicsDevice = p_GraphicsDevice;
		}

		public int ConvertToPolygons( )
		{
			if( m_Planes.Count == 0 )
			{
				return 1;
			}

			// Iterate over each plane, finding intersection points and storing
			// them, removing duplicates

			foreach( Plane PlaneA in m_Planes )
			{
				foreach( Plane PlaneB in m_Planes )
				{
					if( PlaneA == PlaneB )
					{
						continue;
					}
					Vector3 Parallel = Vector3.Cross( PlaneA.Normal,
						PlaneB.Normal );

					if( Math.Abs( Parallel.LengthSquared( ) ) < 1e-10f )
					{
						// Parallel
						continue;
					}

					Vector3 IntersectionPointA;
					Vector3 IntersectionPointB;
				}
			}

			return 0;
		}

		public int Union( ref CSGPrimitive p_Other )
		{
			return 0;
		}

		public int Difference( ref CSGPrimitive p_Other )
		{
			return 0;
		}

		public int Intersection( ref CSGPrimitive p_Other )
		{
			return 0;
		}

		public void RenderNormals( )
		{
		}

		public void RenderWireframe( )
		{
		}

		public void RenderPoints( )
		{
		}

		public void Render( )
		{
		}

		public int GetPolygons( ref UInt16 [ ] p_Indices,
			ref CSGVertex [ ] p_Polygons )
		{
			return 1;
		}

		protected List< Plane	>	m_Planes;
		private GraphicsDevice		m_GraphicsDevice;
		private List< CSGVertex >	m_Vertices;
		private List< UInt16 >		m_Indices;
		private VertexBuffer		m_VertexBuffer;
		private IndexBuffer			m_IndexBuffer;
	}
}
