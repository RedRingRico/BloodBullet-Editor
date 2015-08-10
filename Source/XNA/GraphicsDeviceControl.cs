using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.GamerServices;

namespace BloodBulletEditor
{
	abstract public class GraphicsDeviceControl : Control
	{
		GraphicsDeviceService		m_GraphicsDeviceService;
		protected ServiceContainer	m_Services = new ServiceContainer( );
		protected Label				m_Label = new Label( );
		protected bool				m_PerspectiveView;
        protected bool              m_HasGWFL;

		protected Microsoft.Xna.Framework.Color	m_ClearColour;


		public GraphicsDevice GraphicsDevice
		{
			get
			{
				return m_GraphicsDeviceService.GraphicsDevice;
			}
		}

		public ServiceContainer Services
		{
			get
			{
				return m_Services;
			}
		}

		public Microsoft.Xna.Framework.Color ClearColour
		{
			get
			{
				return m_ClearColour;
			}
			set
			{
				m_ClearColour = value;
			}
		}

		protected override void OnCreateControl( )
		{
			if( !DesignMode )
			{
				m_GraphicsDeviceService = GraphicsDeviceService.AddReference(
					Handle, ClientSize.Width, ClientSize.Height );
				
				m_Services.AddService< IGraphicsDeviceService >(
					m_GraphicsDeviceService );

				Initialise( );

				m_Label.Text = this.Name;
				m_Label.AutoSize = true;
				this.Controls.Add( m_Label );

                System.Diagnostics.Debug.WriteLine( "Handle: {0}", Handle );
			}

			base.OnCreateControl( );
		}

		protected override void OnResize( EventArgs p_Args )
		{
			m_Label.Location = new Point( 0,
				this.Size.Height - m_Label.Size.Height );
		}

		protected override void Dispose( bool p_Disposing )
		{
			if( m_GraphicsDeviceService != null )
			{
				m_GraphicsDeviceService.Release( p_Disposing );
				m_GraphicsDeviceService = null;
			}

			base.Dispose( p_Disposing );
		}

		protected override void OnPaint( PaintEventArgs p_Args )
		{
			int BeginDrawError = BeginDraw( );

			if( BeginDrawError == 0 )
			{
				Draw( );
				EndDraw( );
			}
			else
			{
				PaintUsingSystemDrawing( p_Args.Graphics, "General Error" );
			}

			base.OnPaint( p_Args );
		}

		int BeginDraw( )
		{
			if( m_GraphicsDeviceService == null )
			{
				return 1;
			}

			int DeviceResetError = HandleDeviceReset( );

			if( DeviceResetError != 0 )
			{
				return 2;
			}

			Viewport VP = new Viewport( );

			VP.X = 0;
			VP.Y = 0;
			VP.Width = ClientSize.Width;
			VP.Height = ClientSize.Height;
			VP.MinDepth = 0.0f;
			VP.MaxDepth = 1.0f;

			GraphicsDevice.Viewport = VP;

			GraphicsDevice.Clear( m_ClearColour );

			return 0;
		}

		void EndDraw( )
		{
			try
			{
                if( GamerServicesDispatcher.IsInitialized )
                {
                    GamerServicesDispatcher.Update( );
                }
				// The GFWL Guide only seems to work with Present with no
                // parameters
				if( m_HasGWFL )
				{
					GraphicsDevice.Present( );
				}
				else
				{
					Microsoft.Xna.Framework.Rectangle SourceRectangle =
						new Microsoft.Xna.Framework.Rectangle( 0, 0,
							ClientSize.Width, ClientSize.Height );

					GraphicsDevice.Present( SourceRectangle, null, this.Handle );
				}
			}
			catch
			{
			}
		}

		int HandleDeviceReset( )
		{
			bool DeviceNeedsReset = false;

			switch( GraphicsDevice.GraphicsDeviceStatus )
			{
				case GraphicsDeviceStatus.Lost:
				{
					return 1;
				}
				case GraphicsDeviceStatus.NotReset:
				{
					DeviceNeedsReset = true;

					break;
				}
				default:
				{
					PresentationParameters Present =
						GraphicsDevice.PresentationParameters;
					
					DeviceNeedsReset =
						( ClientSize.Width > Present.BackBufferWidth ) ||
						( ClientSize.Height > Present.BackBufferHeight );

					break;
				}
			}

			if( DeviceNeedsReset )
			{
				try
				{
					m_GraphicsDeviceService.ResetDevice( ClientSize.Width,
						ClientSize.Height );
				}
				catch( Exception p_Exception )
				{
					return 3;
				}
			}

			return 0;
		}

		protected virtual void PaintUsingSystemDrawing( Graphics p_Graphics,
			string p_Text )
		{
			p_Graphics.Clear( Color.CornflowerBlue );

			using( Brush SolidColour = new SolidBrush( Color.Black ) )
			{
				using( StringFormat StringFmt = new StringFormat( ) )
				{
					StringFmt.Alignment = StringAlignment.Center;
					StringFmt.LineAlignment = StringAlignment.Center;

					p_Graphics.DrawString( p_Text, Font, SolidColour,
						ClientRectangle, StringFmt );
				}
			}
		}

		protected override void OnPaintBackground( PaintEventArgs p_PaintEvent )
		{
		}

		protected override void OnMouseClick( MouseEventArgs p_Args )
		{
			// This is mainly for the GFWL Guide, as if the control does not
			// have focus, it will not handle the home key correclty
			this.Focus( );
			base.OnMouseClick( p_Args );
		}

		protected abstract int Initialise( );
		protected abstract void Draw( );
	}
}
