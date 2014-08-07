using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Net;

namespace BloodBulletEditor
{
	class PerspectiveViewControl : GraphicsDeviceControl
	{
		protected override int Initialise( )
		{
			m_PerspectiveView = true;
			Application.Idle += delegate { Invalidate( ); };

			m_ClearColour = Microsoft.Xna.Framework.Color.Black;

			m_Grid = new Grid( this.GraphicsDevice );
			m_Grid.Create( VIEWPLANE.VIEWPLANE_XZ, 100, 100, 100.0f, 0.0f,
				new Color( 32, 32, 128 ), 10, Color.Blue );

			MenuItem [ ] PerspectiveMenu = new MenuItem[ 1 ];
			PerspectiveMenu[ 0 ] = new MenuItem( "Xbox 360 Live Edit",
				Preview_ClickHandle );
			this.ContextMenu = new ContextMenu( PerspectiveMenu );

			return 0;
		}
		
		private void Preview_ClickHandle( object p_Sender, EventArgs p_Args )
		{
			if( SignedInGamer.SignedInGamers.Count == 0 )
			{
				if( Guide.IsVisible == false )
				{
					Guide.ShowSignIn( 1, false );
				}

				// Check if the gamer signed in here and start looking for
				// network sessions (this may require the GUID to be modified)
			}
		}

		protected override void OnCreateControl()
		{
			base.OnCreateControl();

			if( GamerServicesDispatcher.IsInitialized == false )
			{
				GamerServicesDispatcher.WindowHandle = Handle;
				try
				{
					GamerServicesDispatcher.Initialize( m_Services );
				}
				catch( GamerServicesNotAvailableException p_Exception )
				{
					System.Diagnostics.Debug.Write( p_Exception.Message );
				}
				finally
				{
				}
			}
		}

		protected override void Draw( )
		{
			m_ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(
				(float)Math.PI/2.0f, this.GraphicsDevice.Viewport.AspectRatio,
				1.0f, 100000.0f );

			m_ViewMatrix = Matrix.CreateLookAt(
				new Vector3( 10.0f, 100.0f, 100.0f ),
				Vector3.Zero, Vector3.Up );

			m_WorldMatrix = Matrix.Identity;

			m_Grid.Render( m_WorldMatrix, m_ViewMatrix, m_ProjectionMatrix );
		}	
		// Logic to add:
		// When the user clicks on the view (single), the viewport is treated as
		// if it were a game, until the escape key is struck
		// Panning, rotation, and forward/backward translation are handled by
		// holding a modifier key while pressing the middle mouse button,
		// similar to Blender's navigation interface

		private Grid	m_Grid;
		private Matrix	m_WorldMatrix;
		private Matrix	m_ViewMatrix;
		private Matrix	m_ProjectionMatrix;
		private long	m_UpdateCount = 0;
		private NetworkSession	m_NetSession;
	}
}
