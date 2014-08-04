using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BloodBulletEditor
{
	class PerspectiveViewControl : GraphicsDeviceControl
	{
		protected override int Initialise( )
		{
			Application.Idle += delegate { Invalidate( ); };

			m_ClearColour = Microsoft.Xna.Framework.Color.Black;

			m_Grid = new Grid( this.GraphicsDevice );
			m_Grid.Create( VIEWPLANE.VIEWPLANE_XZ, 500, 500, 10.0f, 0.0f,
				new Color( 32, 32, 128 ), 10, Color.Blue );

			return 0;
		}

		protected override void Draw( )
		{
			m_ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(
				(float)Math.PI/2.0f, this.GraphicsDevice.Viewport.AspectRatio,
				1.0f, 100000.0f );

			m_ViewMatrix = Matrix.CreateLookAt(
				new Vector3( 10.0f, 50.0f, 70.0f ),
				Vector3.Zero, Vector3.Up );

			m_WorldMatrix = Matrix.Identity;

			m_Grid.Render( m_WorldMatrix, m_ViewMatrix, m_ProjectionMatrix );
		}

		private Grid	m_Grid;
		private Matrix	m_WorldMatrix;
		private Matrix	m_ViewMatrix;
		private Matrix	m_ProjectionMatrix;
	}
}
