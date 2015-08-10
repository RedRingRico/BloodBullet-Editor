using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BloodBulletEditor.CSG
{
	public class Cube : CSGPrimitive
	{
		public Cube( ref GraphicsDevice p_GraphicsDevice,
			Vector3 p_MinimumPoint, Vector3 p_MaximumPoint ) :
			base( ref p_GraphicsDevice )
		{
			m_Planes = new List< Plane >( );

			Plane Side;

			Side.D = -( p_MinimumPoint.X );
			Side.Normal = new Vector3( -1.0f, 0.0f, 0.0f );
			m_Planes.Add( Side );

			Side.D = -( p_MinimumPoint.Y );
			Side.Normal = new Vector3( 0.0f, -1.0f, 0.0f );
			m_Planes.Add( Side );

			Side.D = -( p_MinimumPoint.Z );
			Side.Normal = new Vector3( 0.0f, 0.0f, -1.0f );
			m_Planes.Add( Side );

			Side.D = -( p_MaximumPoint.X );
			Side.Normal = new Vector3( 1.0f, 0.0f, 0.0f );
			m_Planes.Add( Side );

			Side.D = -( p_MaximumPoint.Y );
			Side.Normal = new Vector3( 0.0f, 1.0f, 0.0f );
			m_Planes.Add( Side );

			Side.D = -( p_MaximumPoint.Z );
			Side.Normal = new Vector3( 0.0f, 0.0f, 1.0f );
			m_Planes.Add( Side );
		}
	}
}
