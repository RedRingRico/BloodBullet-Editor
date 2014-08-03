using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BloodBulletEditor
{
	class PerspectiveViewControl : GraphicsDeviceControl
	{
		protected override int Initialise( )
		{
			Application.Idle += delegate { Invalidate( ); };

			m_ClearColour = Microsoft.Xna.Framework.Color.CornflowerBlue;

			return 0;
		}

		protected override void Draw( )
		{
		}
	}
}
