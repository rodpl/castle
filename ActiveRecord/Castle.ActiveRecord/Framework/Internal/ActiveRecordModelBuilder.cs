// Copyright 2004-2005 Castle Project - http://www.castleproject.org/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace Castle.ActiveRecord.Framework.Internal
{
	using System;
	using System.Reflection;


	public class ActiveRecordModelBuilder
	{
		private static readonly BindingFlags DefaultBindingFlags = BindingFlags.DeclaredOnly|BindingFlags.Public|BindingFlags.Instance;
		private static readonly BindingFlags FieldDefaultBindingFlags = BindingFlags.DeclaredOnly|BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.Instance;

		private readonly ActiveRecordModelCollection coll = new ActiveRecordModelCollection();

		public ActiveRecordModelBuilder()
		{
		}

		public ActiveRecordModel Create(Type type)
		{
			if (type == null) throw new ArgumentNullException("type");

			if (type.IsDefined( typeof(ActiveRecordSkip), false )) return null;

			ActiveRecordModel model = new ActiveRecordModel(type);

			coll.Add(model);

			PopulateModel(model, type);

			ActiveRecordBase._Register( type, model );

			return model;
		}

		public ActiveRecordModelCollection Models
		{
			get { return coll; }
		}

		private void PopulateModel(ActiveRecordModel model, Type type)
		{
			ProcessActiveRecordAttribute(type, model);

			ProcessJoinedBaseAttribute(type, model);

			ProcessProperties(type, model);

			ProcessFields(type, model);
		}

		private void ProcessJoinedBaseAttribute(Type type, ActiveRecordModel model)
		{
			model.IsJoinedSubClassBase = type.IsDefined( typeof(JoinedBaseAttribute), false );
		}

		private void PopulateActiveRecordAttribute(ActiveRecordAttribute attribute, ActiveRecordModel model)
		{
			model.ActiveRecordAtt = attribute;

			if (attribute.DiscriminatorColumn != null)
			{
				model.IsDiscriminatorBase = true;

				if (attribute.DiscriminatorValue == null)
				{
					throw new ActiveRecordException( 
						String.Format("You must specify a discriminator value for the type {0}", model.Type.FullName) );
				}
			}
			else if (attribute.DiscriminatorType != null)
			{
				throw new ActiveRecordException( 
					String.Format("The usage of DiscriminatorType for {0} is meaningless", model.Type.FullName) );
			}

			if (model.ActiveRecordAtt.Table == null)
			{
				model.ActiveRecordAtt.Table = model.Type.Name;
			}
		}

		private void ProcessFields(Type type, ActiveRecordModel model)
		{
			FieldInfo[] fields = type.GetFields( FieldDefaultBindingFlags );

			foreach( FieldInfo field in fields )
			{

				if (field.IsDefined( typeof(FieldAttribute), false ))
				{
					FieldAttribute fieldAtt = field.GetCustomAttributes( typeof(FieldAttribute), false )[0] as FieldAttribute;

					model.Fields.Add( new FieldModel( field, fieldAtt ) );
				}
			}
		}


		private void ProcessProperties(Type type, ActiveRecordModel model)
		{
			PropertyInfo[] props = type.GetProperties( DefaultBindingFlags );

			foreach( PropertyInfo prop in props )
			{
				bool isArProperty = false;

				// Validation

				object[] valAtts = prop.GetCustomAttributes( typeof(AbstractValidationAttribute), true ); 

				foreach(AbstractValidationAttribute valAtt in valAtts)
				{
					valAtt.Validator.Initialize( prop );

					model.Validators.Add( valAtt.Validator );
				}

				if (prop.IsDefined( typeof(PrimaryKeyAttribute), false ))
				{
					PrimaryKeyAttribute propAtt = prop.GetCustomAttributes( typeof(PrimaryKeyAttribute), false )[0] as PrimaryKeyAttribute;
					isArProperty = true;

					model.Ids.Add( new PrimaryKeyModel( prop, propAtt ) );
				}
				else if (prop.IsDefined( typeof(PropertyAttribute), false ))
				{
					PropertyAttribute propAtt = prop.GetCustomAttributes( typeof(PropertyAttribute), false )[0] as PropertyAttribute;
					isArProperty = true;

					model.Properties.Add( new PropertyModel( prop, propAtt ) );
				}
				else if (prop.IsDefined( typeof(NestedAttribute), false ))
				{
					NestedAttribute propAtt = prop.GetCustomAttributes( typeof(NestedAttribute), false )[0] as NestedAttribute;
					isArProperty = true;

					ActiveRecordModel nestedModel = new ActiveRecordModel(prop.PropertyType);

					nestedModel.IsNestedType = true;

					ProcessProperties(prop.PropertyType, nestedModel);

					model.Components.Add( new NestedModel( prop, propAtt, nestedModel ) );
				}
				else if (prop.IsDefined( typeof(JoinedKeyAttribute), false ))
				{
					JoinedKeyAttribute propAtt = prop.GetCustomAttributes( typeof(JoinedKeyAttribute), false )[0] as JoinedKeyAttribute;
					isArProperty = true;

					if (model.Key != null)
					{
						throw new ActiveRecordException("You can't specify more than one JoinedKeyAttribute. " + 
							"Check type " + model.Type.FullName);
					}

					model.Key = new KeyModel( prop, propAtt );
				}
				else if (prop.IsDefined( typeof(VersionAttribute), false ))
				{
					VersionAttribute propAtt = prop.GetCustomAttributes( typeof(VersionAttribute), false )[0] as VersionAttribute;
					isArProperty = true;

					if (model.Version != null)
					{
						throw new ActiveRecordException("You can't specify more than one VersionAttribute. " + 
							"Check type " + model.Type.FullName);
					}

					model.Version = new VersionModel( prop, propAtt );
				}
				else if (prop.IsDefined( typeof(TimestampAttribute), false ))
				{
					TimestampAttribute propAtt = prop.GetCustomAttributes( typeof(TimestampAttribute), false )[0] as TimestampAttribute;
					isArProperty = true;

					if (model.Timestamp != null)
					{
						throw new ActiveRecordException("You can't specify more than one TimestampAttribute. " + 
							"Check type " + model.Type.FullName);
					}

					model.Timestamp = new TimestampModel( prop, propAtt );
				}
				// Relations
				else if (prop.IsDefined( typeof(OneToOneAttribute), false ))
				{
					OneToOneAttribute propAtt = prop.GetCustomAttributes( typeof(OneToOneAttribute), false )[0] as OneToOneAttribute;
					isArProperty = true;

					model.OneToOnes.Add(new OneToOneModel( prop, propAtt ));
				}
				else if (prop.IsDefined( typeof(BelongsToAttribute), false ))
				{
					BelongsToAttribute propAtt = prop.GetCustomAttributes( typeof(BelongsToAttribute), false )[0] as BelongsToAttribute;
					isArProperty = true;

					model.BelongsTo.Add(new BelongsToModel( prop, propAtt ));
				}
				else if (prop.IsDefined( typeof(HasManyAttribute), false ))
				{
					HasManyAttribute propAtt = prop.GetCustomAttributes( typeof(HasManyAttribute), false )[0] as HasManyAttribute;
					isArProperty = true;

					model.HasMany.Add(new HasManyModel( prop, propAtt ));
				}
				else if (prop.IsDefined( typeof(HasAndBelongsToManyAttribute), false ))
				{
					HasAndBelongsToManyAttribute propAtt = prop.GetCustomAttributes( typeof(HasAndBelongsToManyAttribute), false )[0] as HasAndBelongsToManyAttribute;
					isArProperty = true;

					model.HasAndBelongsToMany.Add(new HasAndBelongsToManyModel( prop, propAtt ));
				}

				if (prop.IsDefined( typeof(CollectionIDAttribute), false ))
				{
					CollectionIDAttribute propAtt = prop.GetCustomAttributes( typeof(CollectionIDAttribute), false )[0] as CollectionIDAttribute;

					model.CollectionIDs.Add(new CollectionIDModel( prop, propAtt ));
				}
				if (prop.IsDefined( typeof(HiloAttribute), false ))
				{
					HiloAttribute propAtt = prop.GetCustomAttributes( typeof(HiloAttribute), false )[0] as HiloAttribute;

					model.Hilos.Add(new HiloModel( prop, propAtt ));
				}

				if (!isArProperty)
				{
					model.NotMappedProperties.Add(prop);
				}
			}
		}

		private void ProcessActiveRecordAttribute(Type type, ActiveRecordModel model)
		{
			object[] attrs = type.GetCustomAttributes( typeof(ActiveRecordAttribute), false );
	
			if (attrs.Length == 0)
			{
				throw new ActiveRecordException( String.Format("Type {0} is not using the ActiveRecordAttribute, which is obligatory.", type.FullName) );
			}
	
			ActiveRecordAttribute arAttribute = attrs[0] as ActiveRecordAttribute;
	
			PopulateActiveRecordAttribute(arAttribute, model);
		}
	}
}
