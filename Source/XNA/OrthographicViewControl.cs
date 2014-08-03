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

			switch( m_ViewPlane )
			{
				case VIEWPLANE.VIEWPLANE_XY:
				{
					m_ClearColour = Color.Red;
					this.Name = "Orthographic View [Front]";
					break;
				}
				case VIEWPLANE.VIEWPLANE_XZ:
				{
					m_ClearColour = Color.Green;
					this.Name = "Orthographic View [Top]";
					break;
				}
				case VIEWPLANE.VIEWPLANE_YZ:
				{
					m_ClearColour = Color.Blue;
					this.Name = "Orthographic View [Side]";
					break;
				}
				default:
				{
					break;
				}
			}
		}

		protected override int Initialise( )
		{
			Application.Idle += delegate { Invalidate( ); };

			return 0;
		}

		protected override void Draw( )
		{
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

		private VIEWPLANE m_ViewPlane;
	}
}
