using System;
using System.Threading;
using Microsoft.Xna.Framework.Graphics;

namespace BloodBulletEditor
{
	/// <summary>
	/// Creates and manages the GraphicsDevice.  Multiple GraphicsDeviceControl
	/// instances use the same GraphicsDeviceService.
	/// </summary>
	class GraphicsDeviceService : IGraphicsDeviceService
	{
		static GraphicsDeviceService Instance;
		static int m_ReferenceCount;

		PresentationParameters	m_PresentationParameters;
		GraphicsDevice			m_GraphicsDevice;

		public event EventHandler< EventArgs > DeviceCreated;
		public event EventHandler< EventArgs > DeviceDisposing;
		public event EventHandler< EventArgs > DeviceReset;
		public event EventHandler< EventArgs > DeviceResetting;

		GraphicsDeviceService( IntPtr p_WindowHandle, int p_Width,
			int p_Height )
		{
			m_PresentationParameters = new PresentationParameters( );

			m_PresentationParameters.BackBufferWidth = Math.Max( p_Width, 1 );
			m_PresentationParameters.BackBufferHeight = Math.Max( p_Height, 1 );
			m_PresentationParameters.BackBufferFormat = SurfaceFormat.Color;
			m_PresentationParameters.DepthStencilFormat =
				DepthFormat.Depth24Stencil8;
			m_PresentationParameters.DeviceWindowHandle = p_WindowHandle;
			m_PresentationParameters.PresentationInterval =
				PresentInterval.Immediate;
			m_PresentationParameters.IsFullScreen = false;

			m_GraphicsDevice = new GraphicsDevice(
				GraphicsAdapter.DefaultAdapter,
				GraphicsProfile.HiDef, m_PresentationParameters );
		}

		public static GraphicsDeviceService AddReference( IntPtr p_WindowHandle,
				int p_Width, int p_Height )
		{
			if (Interlocked.Increment(ref m_ReferenceCount) == 1)
			{
				Instance = new GraphicsDeviceService( p_WindowHandle, p_Width,
					p_Height );
			}

			return Instance;
		}

		public void Release( bool p_Disposing )
		{
			if (Interlocked.Decrement(ref m_ReferenceCount) == 0)
			{
				if( p_Disposing )
				{
					if( DeviceDisposing != null )
					{
						DeviceDisposing( this, EventArgs.Empty );
					}

					m_GraphicsDevice.Dispose( );
				}

				m_GraphicsDevice = null;
			}
		}

		public void ResetDevice( int p_Width, int p_Height )
		{
			if( DeviceResetting != null )
			{
				DeviceResetting( this, EventArgs.Empty );
			}

			m_PresentationParameters.BackBufferWidth =
				Math.Max( m_PresentationParameters.BackBufferWidth, p_Width );
			m_PresentationParameters.BackBufferHeight =
				Math.Max( m_PresentationParameters.BackBufferHeight, p_Height );

			m_GraphicsDevice.Reset( m_PresentationParameters );

			if( DeviceReset != null )
			{
				DeviceReset( this, EventArgs.Empty );
			}
		}

		public GraphicsDevice GraphicsDevice
		{
			get
			{
				return m_GraphicsDevice;
			}
		}
	}
}
