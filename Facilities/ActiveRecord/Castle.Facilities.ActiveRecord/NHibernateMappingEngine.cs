//using System;
//using System.Collections;
//using System.Configuration;
//using System.Reflection;
//using System.Text;
//using Castle.Model.Configuration;
//using NHibernate.Cfg;
//
//namespace Castle.Facilities.ActiveRecord
//{
//	public class NHibernateMappingEngine
//	{
//		private Configuration _nhibernate;
//		private string mappingOpen = "<hibernate-mapping xmlns=\"urn:nhibernate-mapping-2.0\" {0}>";
//		private string mappingClose = "</hibernate-mapping>";
//		private string classOpen = "<class name=\"{0}\" {1}>";
//		private string classClose = "</class>";
//		private string tableAttribute = "table=\"{0}\" ";
//		private string proxyAttribute = "proxy=\"{0}\" ";
//		private string schemaAttribute = "schema=\"{0}\" ";
//		private string idOpen = "<id {0}>";
//		private string idClose = "</id>";
//		private string nameAttribute = "name=\"{0}\" ";
//		private string typeAttribute = "type=\"{0}\" ";
//		private string classAttribute = "class=\"{0}\" ";
//		private string generatorOpen = "<generator class=\"{0}\">";
//		private string generatorClose = "</generator>";
//		private string propertyOpen = "<property name=\"{0}\" {1}>";
//		private string propertyClose = "</property>";
//		private string columnAttribute = "column=\"{0}\" ";
//		private string lengthAttribute = "length=\"{0}\" ";
//		private string notNullAttribute = "not-null=\"{0}\" ";
//		private string oneToMany = "<one-to-many class=\"{0}\" />";
//		private string accessAttribute = "access=\"{0}\" ";
//		private string unsavedValueAttribute = "unsaved-value=\"{0}\" ";
//
//		public void Init( IConfiguration engineConfig )
//		{
//			IConfiguration assemblies = engineConfig.Children["assemblies"];
//			if( assemblies == null )
//			{
//				throw new ConfigurationException( "The ActiveRecord facility requires an 'assemblies' configuration." );
//			}
//			_nhibernate = new Configuration();
//
//			foreach( IConfiguration asm in assemblies.Children )
//			{
//				RegisterActiveRecords( asm );
//			}
//		}
//
//		private void RegisterActiveRecords( IConfiguration asm )
//		{
//			Assembly assembly = LoadAssembly( asm.Value );
//			foreach( Type type in assembly.GetTypes() )
//			{
//				if( type.GetCustomAttributes( typeof( ActiveRecordAttribute ), false ) != null )
//				{
//					CreateMapping( type );
//				}
//			}
//		}
//
//		private void CreateMapping( Type type )
//		{
//			ActiveRecordAttribute ar = GetActiveRecord( type );
//			if( ar != null )
//			{
//				StringBuilder xml = new StringBuilder( String.Format( mappingOpen, "" ) );
//
//				string table = ( ar.Table == null ? "" : String.Format( tableAttribute, ar.Table ) );
//				string schema = ( ar.Schema == null ? "" : String.Format( schemaAttribute, ar.Schema ) );
//				string proxy = ( ar.Proxy == null ? "" : String.Format( proxyAttribute, ar.Proxy ) );
//
//				xml.AppendFormat( classOpen, type.AssemblyQualifiedName, table + schema + proxy );
//
//				AddMappedProperties( xml, type.GetProperties() );
//
//				xml.Append( classClose ).Append( mappingClose );
//			}
//			
//		}
//
//		private void AddMappedProperties( StringBuilder builder, PropertyInfo[] props )
//		{
//			foreach( PropertyInfo prop in props )
//			{
//				object[] attributes = prop.GetCustomAttributes( false );
//				foreach( object attribute in attributes )
//				{
//					PrimaryKeyAttribute pk = attribute as PrimaryKeyAttribute;
//					if( pk != null )
//					{
//						string name = ( pk.Name == null ? "" : String.Format( nameAttribute, pk.Name ) );
//						string column = ( pk.Column == null ? "" : String.Format( columnAttribute, pk.Column ) );
//						string type = ( pk.Type == null ? "" : String.Format( typeAttribute, pk.Type ) );
//						string unsavedValue = ( pk.UnsavedValue == null ? "" : String.Format( unsavedValueAttribute, pk.UnsavedValue ) );
//						string access = ( pk.Access == null ? "" : String.Format( accessAttribute, pk.Access ) );
//
//						builder.AppendFormat( idOpen, name, type, column, unsavedValue, access );
//
//						if( pk.Generator != PrimaryKeyType.None )
//						{
//							builder.AppendFormat( generatorOpen, pk.Generator.ToString(), generatorClose );
//						}
//
//						builder.Append( idClose );
//					}
//				}
//			}
//		}
//
//		private ActiveRecordAttribute GetActiveRecord( MemberInfo klass )
//		{
//			foreach( Attribute attribute in klass.GetCustomAttributes( false ) )
//			{
//				if( attribute is ActiveRecordAttribute )
//				{
//					return attribute as ActiveRecordAttribute;
//				}
//			}
//			return null;
//		}
//
//		private Assembly LoadAssembly( string assembly )
//		{
//			try
//			{
//				return Assembly.Load( assembly );
//			}
//			catch( Exception e )
//			{
//				throw new ConfigurationException( String.Format( "Assembly '{0}' could not be loaded.", assembly ), e );
//			}
//		}
//	}
//}