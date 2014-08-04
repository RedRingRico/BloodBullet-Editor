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

			m_Scale = 1.0f;
			m_XDelta = 0.0f;
			m_YDelta = 0.0f;
			m_ZDelta = 0.0f;

			m_Grid = new Grid( this.GraphicsDevice );
			m_Grid.Create( m_ViewPlane, 1000, 1000, 10.0f, 0.0f,
				new Color( 32, 32, 128 ), 10, Color.Blue );

			return 0;
		}

		protected override void Draw( )
		{
			m_ProjectionMatrix = Matrix.CreateOrthographic(
				GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height,
				1.0f, 1000000.0f );

			Vector3 LookPoint = new Vector3( 0.0f, 0.0f, 0.0f );
			Vector3 Up = Vector3.Up;

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
					Up = Vector3.Forward;
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
				new Vector3( m_XPos, m_YPos, m_ZPos ), LookPoint, Up );

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

			m_WorldMatrix = Matrix.CreateScale( m_Scale );

			m_Grid.Render( m_WorldMatrix, m_ViewMatrix, m_ProjectionMatrix );

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
						m_XDelta = -( p_Args.X - m_MouseX ) * m_Scale;
						m_ZDelta = -( p_Args.Y - m_MouseY ) * m_Scale;
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
		private Matrix				m_WorldMatrix;
		private Matrix				m_ViewMatrix;
		private Matrix				m_ProjectionMatrix;
		private float				m_YPos, m_XPos, m_ZPos;
		private float				m_Scale;
		private float				m_ScaleAdd;
		private float				m_XDelta, m_YDelta, m_ZDelta;
		private float				m_MouseX, m_MouseY;
		private bool				m_MiddleButtonDown;
		private Grid				m_Grid;
	}
}
