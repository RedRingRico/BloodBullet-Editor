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
					m_ClearColour = Color.Red;
					break;
				}
				case VIEWPLANE.VIEWPLANE_XZ:
				{
					m_ClearColour = Color.Green;
					break;
				}
				case VIEWPLANE.VIEWPLANE_YZ:
				{
					m_ClearColour = Color.Blue;
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
