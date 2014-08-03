using System;
using System.Collections.Generic;

namespace BloodBulletEditor
{
	public class ServiceContainer : IServiceProvider
	{
		Dictionary< Type, object > m_Services =
			new Dictionary< Type, object >( );

		public void AddService< T >( T p_Service )
		{
			m_Services.Add( typeof( T ), p_Service );
		}

		public object GetService( Type p_ServiceType )
		{
			object Service;

			m_Services.TryGetValue( p_ServiceType, out Service );

			return Service;
		}
	}
}
