using System;
using System.Configuration;
using System.Reflection;
using System.Text;
using Castle.Model.Configuration;
using NHibernate;
using NHibernate.Cfg;

namespace Castle.Facilities.ActiveRecord
{
	public class NHibernateMappingEngine
	{
		private Configuration _nhibernate;
		private string mappingOpen = "<hibernate-mapping xmlns=\"urn:nhibernate-mapping-2.0\" {0}>";
		private string mappingClose = "</hibernate-mapping>";
		private string classOpen = "<class name=\"{0}\" {1}>";
		private string classClose = "</class>";
		private string tableAttribute = "table=\"{0}\" ";
		private string proxyAttribute = "proxy=\"{0}\" ";
		private string schemaAttribute = "schema=\"{0}\" ";
		private string idOpen = "<id {0}>";
		private string idClose = "</id>";
		private string nameAttribute = "name=\"{0}\" ";
		private string typeAttribute = "type=\"{0}\" ";
		private string classAttribute = "class=\"{0}\" ";
		private string generatorOpen = "<generator class=\"{0}\">";
		private string generatorClose = "</generator>";
		private string propertyOpen = "<property name=\"{0}\" {1}>";
		private string propertyClose = "</property>";
		private string updateAttribute = "update=\"{0}\" ";
		private string insertAttribute = "insert=\"{0}\" ";
		private string formulaAttribute = "formula=\"{0}\" ";
		private string columnAttribute = "column=\"{0}\" ";
		private string lengthAttribute = "length=\"{0}\" ";
		private string notNullAttribute = "not-null=\"{0}\" ";
		private string oneToOne = "<one-to-one name=\"{0}\" class=\"{1}\" {2}/>";
//		private string oneToMany = "<one-to-many class=\"{0}\" />";
		private string manyToOne = "<many-to-one name=\"{0}\" {1} />";
		private string cascadeAttribute = "cascade=\"{0}\" ";
		private string outerJoinAttribute = "outer-join=\"{0}\" ";
		private string accessAttribute = "access=\"{0}\" ";
		private string unsavedValueAttribute = "unsaved-value=\"{0}\" ";
		private string constrainedAttribute = "constrained=\"{0}\" ";
		private string mapOpen = "<map name=\"{0}\" {1}>";
		private string mapClose = "</map>";
		private string listOpen = "<list name=\"{0}\" {1}>";
		private string listClose = "</list>";
		private string setOpen = "<set name=\"{0}\" {1}>";
		private string setClose = "</set>";
		private string keyTag = "<key column=\"{0}\"/>";
		private string elementTag = "<element column=\"{0}\" type=\"{1}\"/>";
		private string indexTag = "<index column=\"{0}\" {1} />";
		private string lazyAttribute = "lazy=\"{0}\" ";
		private string inverseAttribute = "inverse=\"{0}\" ";
		private string sortAttribute = "sort=\"{0}\" ";
		private string orderByAttribute = "order-by=\"{0}\" ";
		private string whereAttribute = "where=\"{0}\" ";

		public void Init( IConfiguration engineConfig )
		{
			IConfiguration assemblies = engineConfig.Children["assemblies"];
			if( assemblies == null )
			{
				throw new ConfigurationException( "The ActiveRecord facility requires an 'assemblies' configuration." );
			}
			_nhibernate = new Configuration();

			foreach( IConfiguration asm in assemblies.Children )
			{
				RegisterActiveRecords( asm );
			}
		}

		public ISessionFactory SessionFactory
		{
			get { return _nhibernate.BuildSessionFactory(); }
		}

		private void RegisterActiveRecords( IConfiguration asm )
		{
			Assembly assembly = LoadAssembly( asm.Value );
			foreach( Type type in assembly.GetTypes() )
			{
				if( type.GetCustomAttributes( typeof( ActiveRecordAttribute ), false ) != null )
				{
					_nhibernate.AddClass( type );
					CreateMapping( type );
				}
			}
		}

		private void CreateMapping( Type type )
		{
			ActiveRecordAttribute ar = GetActiveRecord( type );
			if( ar != null )
			{
				StringBuilder xml = new StringBuilder( String.Format( mappingOpen, "" ) );

				string table = ( ar.Table == null ? "" : String.Format( tableAttribute, ar.Table ) );
				string schema = ( ar.Schema == null ? "" : String.Format( schemaAttribute, ar.Schema ) );
				string proxy = ( ar.Proxy == null ? "" : String.Format( proxyAttribute, ar.Proxy ) );

				xml.AppendFormat( classOpen, type.AssemblyQualifiedName, table + schema + proxy );

				AddMappedProperties( xml, type.GetProperties() );

				xml.Append( classClose ).Append( mappingClose );

				_nhibernate.AddXmlString( xml.ToString() );
			}
		}

		private void AddMappedProperties( StringBuilder builder, PropertyInfo[] props )
		{
			foreach( PropertyInfo prop in props )
			{
				object[] attributes = prop.GetCustomAttributes( false );
				foreach( object attribute in attributes )
				{
					PrimaryKeyAttribute pk = attribute as PrimaryKeyAttribute;
					if( pk != null )
					{
						AddPrimaryKeyMapping( prop, pk, builder );

						continue;
					}
					PropertyAttribute property = attribute as PropertyAttribute;
					if( property != null )
					{
						AddPropertyMapping( property, prop, builder );

						continue;
					}
					BelongsToAttribute belongs = attribute as BelongsToAttribute;
					if( belongs != null )
					{
						AddManyToOneMapping( prop, belongs, builder );

						continue;
					}
					HasManyAttribute hasmany = attribute as HasManyAttribute;
					if( hasmany != null )
					{
						if( hasmany.Key != null && hasmany.Index == null )
						{
							AddMapMapping( prop, hasmany, builder );
						}
						else if( hasmany.Index != null && hasmany.Key == null )
						{
							AddListMapping( prop, hasmany, builder );
						}
						else
						{
							AddSetMapping( prop, hasmany, builder );
						}
						continue;
					}
					HasOneAttribute hasone = attribute as HasOneAttribute;
					if( hasone != null )
					{
						Type otherType = hasone.MapType;
						object[] otherAttributes = otherType.GetCustomAttributes( typeof( BelongsToAttribute ), false );
						BelongsToAttribute inverse = null;
						foreach( object o in otherAttributes )
						{
							if( o is BelongsToAttribute )
							{
								inverse = o as BelongsToAttribute;
								break;
							}
						}
						if( inverse != null && inverse.Type == prop.DeclaringType )
						{
							AddOneToOneMapping( prop, hasone, builder );
						}
						// throw exception if no BelongsToAttribute?
						continue;
					}
				}
			}
		}

		private void AddOneToOneMapping( PropertyInfo prop, HasOneAttribute hasone, StringBuilder builder )
		{
			string name = prop.Name;
			string klass = String.Format( classAttribute, prop.PropertyType.Name );
			string cascade = ( hasone.Cascade == null ? "" : String.Format( cascadeAttribute, hasone.Cascade ) );
			string outer = ( hasone.OuterJoin == null ? "" : String.Format( outerJoinAttribute, hasone.OuterJoin ) );
			string constrained = ( hasone.Constrained == null ? "" : String.Format( constrainedAttribute, hasone.Constrained ) );
			builder.AppendFormat( oneToOne, name, klass, cascade + outer + constrained );
		}

		private void AddSetMapping( PropertyInfo prop, HasManyAttribute hasmany, StringBuilder builder )
		{
			string name = prop.Name;
			string table = ( hasmany.Table == null ? "" : String.Format( tableAttribute, hasmany.Table ) );
			string schema = ( hasmany.Schema == null ? "" : String.Format( schemaAttribute, hasmany.Schema ) );
			string lazy = ( hasmany.Lazy == null ? "" : String.Format( lazyAttribute, hasmany.Lazy ) );
			string inverse = ( hasmany.Inverse == null ? "" : String.Format( inverseAttribute, hasmany.Inverse ) );
			string cascade = ( hasmany.Cascade == null ? "" : String.Format( cascadeAttribute, hasmany.Cascade ) );
			string sort = ( hasmany.Sort == null ? "" : String.Format( sortAttribute, hasmany.Sort ) );
			string orderBy = ( hasmany.OrderBy == null ? "" : String.Format( orderByAttribute, hasmany.OrderBy ) );
			string where = ( hasmany.Where == null ? "" : String.Format( whereAttribute, hasmany.Where ) );

			builder.AppendFormat( setOpen, name, table + schema + lazy + inverse + cascade + sort + orderBy + where );

			Type otherType = hasmany.MapType;
			PropertyInfo elementProp = otherType.GetProperty( hasmany.Key );
			if( elementProp != null )
			{
				builder.AppendFormat( keyTag, hasmany.Key );
				PropertyInfo indexProp = otherType.GetProperty( hasmany.Index );
				if( indexProp != null )
				{
					string type = String.Format( typeAttribute, indexProp.Name );
					builder.AppendFormat( indexTag, hasmany.Index, type );
				}
				string column = null;
				object[] elementAttributes = elementProp.GetCustomAttributes( false );
				foreach( object attribute in elementAttributes )
				{
					if( attribute is PropertyAttribute )
					{
						column = ( attribute as PropertyAttribute ).Column;
					}
					else if( attribute is PrimaryKeyAttribute )
					{
						column = ( attribute as PrimaryKeyAttribute ).Column;
					}
					else if ( attribute is BelongsToAttribute )
					{
						column = ( attribute as BelongsToAttribute ).Column;
					}
				}
				if( column != null )
				{
					builder.AppendFormat( elementTag, column, otherType.Name );
				}
			}
			builder.Append( setClose );
		}

		private void AddListMapping( PropertyInfo prop, HasManyAttribute hasmany, StringBuilder builder )
		{
			string name = prop.Name;
			string table = ( hasmany.Table == null ? "" : String.Format( tableAttribute, hasmany.Table ) );
			string schema = ( hasmany.Schema == null ? "" : String.Format( schemaAttribute, hasmany.Schema ) );
			string lazy = ( hasmany.Lazy == null ? "" : String.Format( lazyAttribute, hasmany.Lazy ) );
			string inverse = ( hasmany.Inverse == null ? "" : String.Format( inverseAttribute, hasmany.Inverse ) );
			string cascade = ( hasmany.Cascade == null ? "" : String.Format( cascadeAttribute, hasmany.Cascade ) );
			string sort = ( hasmany.Sort == null ? "" : String.Format( sortAttribute, hasmany.Sort ) );
			string orderBy = ( hasmany.OrderBy == null ? "" : String.Format( orderByAttribute, hasmany.OrderBy ) );
			string where = ( hasmany.Where == null ? "" : String.Format( whereAttribute, hasmany.Where ) );

			builder.AppendFormat( listOpen, name, table + schema + lazy + inverse + cascade + sort + orderBy + where );

			Type otherType = hasmany.MapType;
			PropertyInfo indexProp = otherType.GetProperty( hasmany.Index );
			if( indexProp != null )
			{
				string type = String.Format( typeAttribute, indexProp.Name );
				builder.AppendFormat( indexTag, hasmany.Index, type );
//				PropertyInfo elementProp = otherType.GetProperty( hasmany.Key );
//				if( elementProp != null )
//				{
//					builder.AppendFormat( keyTag, hasmany.Key );
//				}
				string column = null;
				object[] elementAttributes = indexProp.GetCustomAttributes( false );
				foreach( object attribute in elementAttributes )
				{
					if( attribute is PropertyAttribute )
					{
						column = ( attribute as PropertyAttribute ).Column;
					}
					else if( attribute is PrimaryKeyAttribute )
					{
						column = ( attribute as PrimaryKeyAttribute ).Column;
					}
					else if ( attribute is BelongsToAttribute )
					{
						column = ( attribute as BelongsToAttribute ).Column;
					}
				}
				if( column != null )
				{
					builder.AppendFormat( elementTag, column, otherType.Name );
				}
			}
			builder.Append( listClose );
		}

		private void AddMapMapping( PropertyInfo prop, HasManyAttribute hasmany, StringBuilder builder )
		{
			string name = prop.Name;
			string table = ( hasmany.Table == null ? "" : String.Format( tableAttribute, hasmany.Table ) );
			string schema = ( hasmany.Schema == null ? "" : String.Format( schemaAttribute, hasmany.Schema ) );
			string lazy = ( hasmany.Lazy == null ? "" : String.Format( lazyAttribute, hasmany.Lazy ) );
			string inverse = ( hasmany.Inverse == null ? "" : String.Format( inverseAttribute, hasmany.Inverse ) );
			string cascade = ( hasmany.Cascade == null ? "" : String.Format( cascadeAttribute, hasmany.Cascade ) );
			string sort = ( hasmany.Sort == null ? "" : String.Format( sortAttribute, hasmany.Sort ) );
			string orderBy = ( hasmany.OrderBy == null ? "" : String.Format( orderByAttribute, hasmany.OrderBy ) );
			string where = ( hasmany.Where == null ? "" : String.Format( whereAttribute, hasmany.Where ) );

			builder.AppendFormat( mapOpen, name, table + schema + lazy + inverse + cascade + sort + orderBy + where );

			Type otherType = hasmany.MapType;
			PropertyInfo elementProp = otherType.GetProperty( hasmany.Key );
			if( elementProp != null )
			{
				builder.AppendFormat( keyTag, hasmany.Key );
				string column = null;
				object[] elementAttributes = elementProp.GetCustomAttributes( false );
				foreach( object attribute in elementAttributes )
				{
					if( attribute is PropertyAttribute )
					{
						column = ( attribute as PropertyAttribute ).Column;
					}
					else if( attribute is PrimaryKeyAttribute )
					{
						column = ( attribute as PrimaryKeyAttribute ).Column;
					}
					else if ( attribute is BelongsToAttribute )
					{
						column = ( attribute as BelongsToAttribute ).Column;
					}
				}
				if( column != null )
				{
					builder.AppendFormat( elementTag, column, otherType.Name );
				}
			}
			builder.Append( mapClose );
		}

		private void AddManyToOneMapping( PropertyInfo prop, BelongsToAttribute belongs, StringBuilder builder )
		{
			string name = String.Format( nameAttribute, prop.Name );
			string klass = String.Format( classAttribute, prop.PropertyType.Name );
			string column = ( belongs.Column == null ? "" : String.Format( columnAttribute, belongs.Column ) );
			string cascade = ( belongs.Cascade == null ? "" : String.Format( cascadeAttribute, belongs.Cascade ) );
			string outer = ( belongs.OuterJoin == null ? "" : String.Format( outerJoinAttribute, belongs.OuterJoin ) );
			string update = ( belongs.Update == null ? "" : String.Format( updateAttribute, belongs.Update ) );
			string insert = ( belongs.Insert == null ? "" : String.Format( insertAttribute, belongs.Insert ) );
	
			builder.AppendFormat( manyToOne, name, klass + column + cascade + outer + update + insert );
		}

		private void AddPropertyMapping( PropertyAttribute property, PropertyInfo prop, StringBuilder builder )
		{
			string column = ( property.Column == null ? "" : String.Format( columnAttribute, property.Column ) );
			string update = ( property.Update == null ? "" : String.Format( updateAttribute, property.Update ) );
			string insert = ( property.Insert == null ? "" : String.Format( insertAttribute, property.Insert ) );
			string formula = ( property.Formula == null ? "" : String.Format( formulaAttribute, property.Formula ) );
			string length = ( property.Length == null ? "" : String.Format( lengthAttribute, property.Length ) );
			string notNull = ( property.NotNull == null ? "" : String.Format( notNullAttribute, property.NotNull ) );
			string name = String.Format( nameAttribute, prop.Name );
			string type = String.Format( typeAttribute, prop.PropertyType.Name );
	
			builder.AppendFormat( propertyOpen, name, type + column + update + insert + formula + length + notNull );
			builder.Append( propertyClose );
		}

		private void AddPrimaryKeyMapping( PropertyInfo prop, PrimaryKeyAttribute pk, StringBuilder builder )
		{
			string name = String.Format( nameAttribute, prop.Name );
			string type = String.Format( typeAttribute, prop.PropertyType.Name );
			string column = ( pk.Column == null ? "" : String.Format( columnAttribute, pk.Column ) );
			string unsavedValue = ( pk.UnsavedValue == null ? "" : String.Format( unsavedValueAttribute, pk.UnsavedValue ) );
			string access = ( pk.Access == null ? "" : String.Format( accessAttribute, pk.Access ) );
	
			builder.AppendFormat( idOpen, name + type + column + unsavedValue + access );
	
			if( pk.Generator != PrimaryKeyType.None )
			{
				builder.AppendFormat( generatorOpen, pk.Generator.ToString(), generatorClose );
			}
	
			builder.Append( idClose );
		}

		private ActiveRecordAttribute GetActiveRecord( MemberInfo klass )
		{
			foreach( Attribute attribute in klass.GetCustomAttributes( false ) )
			{
				if( attribute is ActiveRecordAttribute )
				{
					return attribute as ActiveRecordAttribute;
				}
			}
			return null;
		}

		private Assembly LoadAssembly( string assembly )
		{
			try
			{
				return Assembly.Load( assembly );
			}
			catch( Exception e )
			{
				throw new ConfigurationException( String.Format( "Assembly '{0}' could not be loaded.", assembly ), e );
			}
		}
	}
}