// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
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

namespace Castle.Components.DictionaryAdapter
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Collections.Specialized;
	using System.ComponentModel;
	using System.Reflection;
	using System.Reflection.Emit;
	using System.Text;
	using System.Threading;

	/// <summary>
	/// Uses Reflection.Emit to expose the properties of a dictionary
	/// through a dynamic implementation of a typed interface.
	/// </summary>
	public class DictionaryAdapterFactory : IDictionaryAdapterFactory
	{
		private readonly IDictionary<Assembly, string> assembliesNames;

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="DictionaryAdapterFactory"/> class.
		/// </summary>
		public DictionaryAdapterFactory()
		{
			assembliesNames = new Dictionary<Assembly, string>();
		}

		#endregion

		#region IDictionaryAdapterFactory

		/// <summary>
		/// Gets a typed adapter bound to the <see cref="IDictionary"/>.
		/// </summary>
		/// <typeparam name="T">The typed interface.</typeparam>
		/// <param name="dictionary">The underlying source of properties.</param>
		/// <returns>An implementation of the typed interface bound to the dictionary.</returns>
		/// <remarks>
		/// The type represented by T must be an interface with properties.
		/// </remarks>
		public T GetAdapter<T>(IDictionary dictionary)
		{
			return (T) GetAdapter(typeof(T), dictionary);
		}

		/// <summary>
		/// Gets a typed adapter bound to the <see cref="IDictionary"/>.
		/// </summary>
		/// <param name="type">The typed interface.</param>
		/// <param name="dictionary">The underlying source of properties.</param>
		/// <returns>An implementation of the typed interface bound to the dictionary.</returns>
		/// <remarks>
		/// The type represented by T must be an interface with properties.
		/// </remarks>
		public object GetAdapter(Type type, IDictionary dictionary)
		{
			return InternalGetAdapter(type, dictionary, null);
		}

		/// <summary>
		/// Gets a typed adapter bound to the <see cref="IDictionary"/>.
		/// </summary>
		/// <param name="type">The typed interface.</param>
		/// <param name="dictionary">The underlying source of properties.</param>
		/// <param name="descriptor">The property descriptor.</param>
		/// <returns>An implementation of the typed interface bound to the dictionary.</returns>
		/// <remarks>
		/// The type represented by T must be an interface with properties.
		/// </remarks>
		public object GetAdapter(Type type, IDictionary dictionary, PropertyDescriptor descriptor)
		{
			return InternalGetAdapter(type, dictionary, descriptor);
		}

		/// <summary>
		/// Gets a typed adapter bound to the <see cref="NameValueCollection"/>.
		/// </summary>
		/// <typeparam name="T">The typed interface.</typeparam>
		/// <param name="nameValues">The underlying source of properties.</param>
		/// <returns>An implementation of the typed interface bound to the namedValues.</returns>
		/// <remarks>
		/// The type represented by T must be an interface with properties.
		/// </remarks>
		public T GetAdapter<T>(NameValueCollection nameValues)
		{
			return GetAdapter<T>(new NameValueCollectionAdapter(nameValues));
		}

		/// <summary>
		/// Gets a typed adapter bound to the <see cref="NameValueCollection"/>.
		/// </summary>
		/// <param name="type">The typed interface.</param>
		/// <param name="nameValues">The underlying source of properties.</param>
		/// <returns>An implementation of the typed interface bound to the namedValues.</returns>
		/// <remarks>
		/// The type represented by T must be an interface with properties.
		/// </remarks>
		public object GetAdapter(Type type, NameValueCollection nameValues)
		{
			return GetAdapter(type, new NameValueCollectionAdapter(nameValues));
		}

		#endregion

		#region Dynamic Type Generation

		private object InternalGetAdapter(Type type, IDictionary dictionary, PropertyDescriptor descriptor)
		{
			if (!type.IsInterface)
			{
				throw new ArgumentException("Only interfaces can be adapted");
			}

			var appDomain = Thread.GetDomain();
			var adapterAssemblyName = GetAdapterAssemblyName(type);
			var adapterAssembly = GetExistingAdapterAssembly(appDomain, adapterAssemblyName);

			if (adapterAssembly == null)
			{
				var typeBuilder = CreateTypeBuilder(type, appDomain, adapterAssemblyName);
				adapterAssembly = CreateAdapterAssembly(type, typeBuilder);
			}

			return GetExistingAdapter(type, adapterAssembly, dictionary, descriptor);
		}

		private static TypeBuilder CreateTypeBuilder(Type type, AppDomain appDomain, String adapterAssemblyName)
		{
			var assemblyName = new AssemblyName(adapterAssemblyName);
			var assemblyBuilder = appDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
			var moduleBuilder = assemblyBuilder.DefineDynamicModule(adapterAssemblyName);
			return CreateAdapterType(type, moduleBuilder);
		}

		private static TypeBuilder CreateAdapterType(Type type, ModuleBuilder moduleBuilder)
		{
			var typeBuilder = moduleBuilder.DefineType(GetAdapterFullTypeName(type),
				TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.BeforeFieldInit);
			typeBuilder.AddInterfaceImplementation(type);
			typeBuilder.SetParent(typeof(DictionaryAdapterBase));
			return typeBuilder;
		}

		private static Assembly CreateAdapterAssembly(Type type, TypeBuilder typeBuilder)
		{
			var propertyMapField = typeBuilder.DefineField("propertyMap", typeof(Dictionary<String, PropertyDescriptor>),
														   FieldAttributes.Private | FieldAttributes.Static);
			CreateAdapterConstructor(type, typeBuilder);

			var propertyMap = GetPropertyDescriptors(type);

			CreateDictionaryAdapterMeta(typeBuilder, propertyMapField, propertyMap);

			foreach (var descriptor in propertyMap)
			{
				CreateAdapterProperty(typeBuilder, propertyMapField, descriptor.Value);
			}

			var adapterType = typeBuilder.CreateType();
			adapterType.InvokeMember("propertyMap", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.SetField,
			                         null, null, new object[] { propertyMap });

			return typeBuilder.Assembly;
		}

		#endregion

		#region CreateAdapterConstructor

		private static void CreateAdapterConstructor(Type type, TypeBuilder typeBuilder)
		{
			var constructorBuilder = typeBuilder.DefineConstructor(
				MethodAttributes.Public | MethodAttributes.HideBySig, CallingConventions.Standard,
				new [] { typeof(Type), typeof(DictionaryAdapterFactory), 
						 typeof(IDictionary), typeof(IDictionaryKeyBuilder) });

			constructorBuilder.DefineParameter(1, ParameterAttributes.None, "type");
			constructorBuilder.DefineParameter(2, ParameterAttributes.None, "factory");
			constructorBuilder.DefineParameter(3, ParameterAttributes.None, "dictionary");
			constructorBuilder.DefineParameter(4, ParameterAttributes.None, "descriptor");

			var ilGenerator = constructorBuilder.GetILGenerator();

			var baseType = typeof(DictionaryAdapterBase);
			var baseConstructorInfo = baseType.GetConstructors()[0];

			ilGenerator.Emit(OpCodes.Ldarg_0);
			ilGenerator.Emit(OpCodes.Ldarg_1);
			ilGenerator.Emit(OpCodes.Ldarg_2);
			ilGenerator.Emit(OpCodes.Ldarg_3);
			ilGenerator.Emit(OpCodes.Ldarg_S, 4);
			ilGenerator.Emit(OpCodes.Call, baseConstructorInfo);

			var initializers = AttributesUtil.GetTypeAttributes<DictionaryInitializeAttribute>(type);

			if (initializers != null) foreach (var initializer in initializers)
			{
				// new DictionaryInitializer().Initialize(this)
				ilGenerator.Emit(OpCodes.Newobj, initializer.Constructor);
				ilGenerator.Emit(OpCodes.Ldarg_0);
				ilGenerator.Emit(OpCodes.Callvirt, DictionaryInitialize);
			}

			ilGenerator.Emit(OpCodes.Ret);
		}

		#endregion

		#region CreateDictionaryAdapterMeta

		private static void CreateDictionaryAdapterMeta(TypeBuilder typeBuilder, FieldInfo propertyMapField,
														IDictionary<String, PropertyDescriptor> propertyMap)
		{
			CreateDictionaryAdapterMetaProperty(typeBuilder, MetaPropertiesProp, propertyMapField);

			var methodAttribs = MethodAttributes.Public    | MethodAttributes.HideBySig |
								MethodAttributes.ReuseSlot | MethodAttributes.Virtual;

			// override IDictionaryAdapter.FetchProperties()
			var methodBuilder = typeBuilder.DefineMethod(MetaFetchProperties.Name, methodAttribs,
														 MetaFetchProperties.ReturnType, null);

			var iLGenerator = methodBuilder.GetILGenerator();
			foreach (var descriptor in propertyMap.Values)
			{
				iLGenerator.Emit(OpCodes.Ldarg_0);
				iLGenerator.Emit(OpCodes.Call, descriptor.Property.GetGetMethod());
				iLGenerator.Emit(OpCodes.Pop);
			}
			iLGenerator.Emit(OpCodes.Ret);

			typeBuilder.DefineMethodOverride(methodBuilder, MetaFetchProperties);
		}

		private static void CreateDictionaryAdapterMetaProperty(TypeBuilder typeBuilder, PropertyInfo metaProp, 
																FieldInfo metaField)
		{
			var propAttribs = MethodAttributes.Public    | MethodAttributes.SpecialName |
							  MethodAttributes.HideBySig | MethodAttributes.ReuseSlot |
							  MethodAttributes.Virtual   | MethodAttributes.Final;

			var getMethodBuilder = typeBuilder.DefineMethod("get_" + metaProp.Name,
														    propAttribs, metaProp.PropertyType, null);

			var getILGenerator = getMethodBuilder.GetILGenerator();
			if (metaField.IsStatic)
			{
				getILGenerator.Emit(OpCodes.Ldsfld, metaField);
			}
			else
			{
				getILGenerator.Emit(OpCodes.Ldarg_0);
				getILGenerator.Emit(OpCodes.Ldfld, metaField);
			}
			getILGenerator.Emit(OpCodes.Ret);

			typeBuilder.DefineMethodOverride(getMethodBuilder, metaProp.GetGetMethod());
		}

		#endregion

		#region CreateAdapterProperty

		private static void CreateAdapterProperty(TypeBuilder typeBuilder, FieldInfo propertyMapField, 
												  PropertyDescriptor descriptor)
		{
			var property = descriptor.Property;
			var propertyBuilder = typeBuilder.DefineProperty(property.Name, property.Attributes, property.PropertyType, null);

			var propAttribs = MethodAttributes.Public | MethodAttributes.SpecialName |
							  MethodAttributes.HideBySig | MethodAttributes.Virtual;

			if (property.CanRead)
			{
				CreatePropertyGetMethod(typeBuilder, propertyMapField, propertyBuilder, descriptor, propAttribs);
			}

			if (property.CanWrite)
			{
				CreatePropertySetMethod(typeBuilder, propertyMapField, propertyBuilder, descriptor, propAttribs);
			}
		}

		private static void PreparePropertyMethod(
			PropertyDescriptor descriptor, FieldInfo propertyMapField,
			ILGenerator propILGenerator, out LocalBuilder descriptorLocal)
		{
			propILGenerator.DeclareLocal(typeof(String));
			propILGenerator.DeclareLocal(typeof(object));
			descriptorLocal = propILGenerator.DeclareLocal(typeof(PropertyDescriptor));

			// key = propertyInfo.Name
			propILGenerator.Emit(OpCodes.Ldstr, descriptor.PropertyName);
			propILGenerator.Emit(OpCodes.Stloc_0);

			// descriptor = propertyMap[key]
			propILGenerator.Emit(OpCodes.Ldsfld, propertyMapField);
			propILGenerator.Emit(OpCodes.Ldstr, descriptor.PropertyName);
			propILGenerator.Emit(OpCodes.Callvirt, PropertyMapGetItem);
			propILGenerator.Emit(OpCodes.Stloc_S, descriptorLocal);
		}

		#endregion

		#region CreatePropertyGetMethod

		private static void CreatePropertyGetMethod(
			TypeBuilder typeBuilder, FieldInfo propertyMapField, PropertyBuilder propertyBuilder, 
			PropertyDescriptor descriptor, MethodAttributes propAttribs)
		{
			var getMethodBuilder = typeBuilder.DefineMethod("get_" + descriptor.PropertyName, propAttribs,
															descriptor.PropertyType, null);

			var getILGenerator = getMethodBuilder.GetILGenerator();

			var returnDefault = getILGenerator.DefineLabel();
			var storeResult = getILGenerator.DefineLabel();
			var loadResult = getILGenerator.DefineLabel();
			LocalBuilder descriptorLocal;

			PreparePropertyMethod(descriptor, propertyMapField, getILGenerator, out descriptorLocal);

			var result = getILGenerator.DeclareLocal(descriptor.PropertyType);

			// value = GetProperty(key, value)
			getILGenerator.Emit(OpCodes.Ldarg_0);
			getILGenerator.Emit(OpCodes.Ldloc_0);
			getILGenerator.Emit(OpCodes.Callvirt, MetaGetProperty);
			getILGenerator.Emit(OpCodes.Stloc_1);

			// if (value == null) return null
			getILGenerator.Emit(OpCodes.Ldloc_1);
			getILGenerator.Emit(OpCodes.Brfalse_S, returnDefault);

			// return (propertyInfo.PropertyType) value
			getILGenerator.Emit(OpCodes.Ldloc_1);
			getILGenerator.Emit(OpCodes.Unbox_Any, descriptor.PropertyType);
			getILGenerator.Emit(OpCodes.Br_S, storeResult);

			getILGenerator.MarkLabel(returnDefault);
			getILGenerator.Emit(OpCodes.Ldloca_S, result);
			getILGenerator.Emit(OpCodes.Initobj, descriptor.PropertyType);
			getILGenerator.Emit(OpCodes.Br_S, loadResult);

			getILGenerator.MarkLabel(storeResult);
			getILGenerator.Emit(OpCodes.Stloc_S, result);

			getILGenerator.MarkLabel(loadResult);
			getILGenerator.Emit(OpCodes.Ldloc_S, result);

			getILGenerator.Emit(OpCodes.Ret);

			propertyBuilder.SetGetMethod(getMethodBuilder);
		}

		#endregion

		#region CreatePropertySetMethod

		private static void CreatePropertySetMethod(
			TypeBuilder typeBuilder, FieldInfo propertyMapField, PropertyBuilder propertyBuilder,
			PropertyDescriptor descriptor, MethodAttributes propAttribs)
		{
			var setMethodBuilder = typeBuilder.DefineMethod("set_" + descriptor.PropertyName, propAttribs, null,
															new[] {descriptor.PropertyType});

			var setILGenerator = setMethodBuilder.GetILGenerator();
			LocalBuilder descriptorLocal;

			PreparePropertyMethod(descriptor, propertyMapField, setILGenerator, out descriptorLocal);

			setILGenerator.Emit(OpCodes.Ldarg_1);
			if (descriptor.PropertyType.IsValueType)
			{
				setILGenerator.Emit(OpCodes.Box, descriptor.PropertyType);
			}
			setILGenerator.Emit(OpCodes.Stloc_1);

			// ignore = SetProperty(key, ref value)
			setILGenerator.Emit(OpCodes.Ldarg_0);
			setILGenerator.Emit(OpCodes.Ldloc_0);
			setILGenerator.Emit(OpCodes.Ldloca_S, 1);
			setILGenerator.Emit(OpCodes.Callvirt, MetaSetProperty);
			setILGenerator.Emit(OpCodes.Pop);
			setILGenerator.Emit(OpCodes.Ret);

			propertyBuilder.SetSetMethod(setMethodBuilder);
		}

		#endregion

		#region Property Descriptors

		private static Dictionary<String, PropertyDescriptor> GetPropertyDescriptors(Type type)
		{
			var propertyMap = new Dictionary<String, PropertyDescriptor>();
			var typeGetters = GetOrderedBehaviors<IDictionaryPropertyGetter>(type);
			var typeSetters = GetOrderedBehaviors<IDictionaryPropertySetter>(type);

			RecursivelyDiscoverProperties(type, property =>
			{
				var descriptor = new PropertyDescriptor(property);

				descriptor.AddKeyBuilders(GetOrderedBehaviors<IDictionaryKeyBuilder>(property));
				descriptor.AddKeyBuilders(GetOrderedBehaviors<IDictionaryKeyBuilder>(property.ReflectedType));

				descriptor.AddGetters(GetOrderedBehaviors<IDictionaryPropertyGetter>(property));
				descriptor.AddGetters(typeGetters);
				AddDefaultGetter(descriptor);

				descriptor.AddSetters(GetOrderedBehaviors<IDictionaryPropertySetter>(property));
				descriptor.AddSetters(typeSetters);

				PropertyDescriptor existingDescriptor;
				if (propertyMap.TryGetValue(property.Name, out existingDescriptor))
				{
					var existingProperty = existingDescriptor.Property;
					if (existingProperty.PropertyType == property.PropertyType)
					{
						if (property.CanRead && property.CanWrite)
						{
							propertyMap[property.Name] = descriptor;
						}
						return;
					}
				}
	
				propertyMap.Add(property.Name, descriptor);
			});

			return propertyMap;
		}

		private static List<T> GetOrderedBehaviors<T>(Type type) where T : IDictionaryBehavior
		{
			var behaviors = AttributesUtil.GetTypeAttributes<T>(type);
			if (behaviors != null)
			{
				behaviors.Sort(DictionaryBehaviorComparer<T>.Instance);
			}
			return behaviors;
		}

		private static List<T> GetOrderedBehaviors<T>(MemberInfo member) where T : IDictionaryBehavior
		{
			var behaviors = AttributesUtil.GetAttributes<T>(member);
			if (behaviors != null)
			{
				behaviors.Sort(DictionaryBehaviorComparer<T>.Instance);
			}
			return behaviors;
		}

		private static void RecursivelyDiscoverProperties(Type currentType, Action<PropertyInfo> onProperty)
		{
			var types = new List<Type>();
			types.Add(currentType);
			types.AddRange(currentType.GetInterfaces());

			foreach (Type type in types)
			{
				if (type == typeof(IDataErrorInfo)) continue;
				foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
				{
					onProperty(property);
				}
			}
		}

		private static void AddDefaultGetter(PropertyDescriptor descriptor)
		{
			if (descriptor.TypeConverter != null)
			{
				descriptor.AddGetter(new DefaultPropertyGetter(descriptor.TypeConverter));
			}
		}

		#endregion

		#region Assembly Support 

		private string GetAdapterAssemblyName(Type type)
		{
			return string.Concat(GetAssemblyName( type.Assembly ), ".",
				GetSafeTypeFullName(type), ".DictionaryAdapter" );
		}

		private string GetAssemblyName(Assembly assembly)
		{
			string assemblyName;
			if (!assembliesNames.TryGetValue(assembly, out assemblyName))
			{
				assemblyName = assembly.GetName().Name;
				assembliesNames[assembly] = assemblyName;
			}

			return assemblyName;
		}

		private static String GetAdapterFullTypeName(Type type)
		{
			return type.Namespace + "." + GetAdapterTypeName(type);
		}

		private static String GetAdapterTypeName(Type type)
		{
			return GetSafeTypeName(type).Substring(1) + "DictionaryAdapter";
		}

		public static string GetSafeTypeFullName(Type type)
		{
			if (type.IsGenericTypeDefinition)
			{
				return type.FullName.Replace("`", "_");
			}

			if (type.IsGenericType)
			{
				var name = new StringBuilder();
				if (!string.IsNullOrEmpty(type.Namespace))
				{
					name.Append(type.Namespace).Append(".");
				}

				AppendGenericTypeName(type, name);
				return name.ToString();
			}

			return type.FullName;
		}

		public static string GetSafeTypeName(Type type)
		{
			if (type.IsGenericTypeDefinition)
			{
				return type.Name.Replace("`", "_");
			}

			if (type.IsGenericType)
			{
				var name = new StringBuilder();
				AppendGenericTypeName(type, name);
				return name.ToString();
			}

			return type.Name;
		}

		private static void AppendGenericTypeName(Type type, StringBuilder sb)
		{
			// Replace back tick preceding parameter count with _ List`1 => List_1
			sb.Append(type.Name.Replace("`", "_"));

			// Append safe full name of each type argument, separated by _
			foreach (var argument in type.GetGenericArguments())
			{
				sb.Append("_").Append(GetSafeTypeFullName(argument).Replace(".", "_"));
			}
		}

		private object GetExistingAdapter(Type type, Assembly assembly, IDictionary dictionary,
		                                  PropertyDescriptor descriptor)
		{
			var adapterFullTypeName = GetAdapterFullTypeName(type);
			return Activator.CreateInstance(assembly.GetType(adapterFullTypeName, true),
			                                type, this, dictionary, descriptor);
		}

		private Assembly GetExistingAdapterAssembly(AppDomain appDomain, string assemblyName)
		{
			return Array.Find(appDomain.GetAssemblies(), assembly => GetAssemblyName(assembly) == assemblyName );
		}

		#endregion

		#region Reflection Cache

		private static readonly MethodInfo DictionaryGetItem =
			typeof(IDictionary).GetMethod("get_Item", new[] {typeof(Object)});

		private static readonly MethodInfo DictionarySetItem =
			typeof(IDictionary).GetMethod("set_Item", new[] {typeof(Object), typeof(Object)});

		private static readonly MethodInfo PropertyMapGetItem =
			typeof(Dictionary<String, object[]>).GetMethod("get_Item", new[] {typeof(String)});

		private static readonly MethodInfo DescriptorGetKey =
			typeof(PropertyDescriptor).GetMethod("GetKey");

		private static readonly MethodInfo DescriptorGetValue =
			typeof(PropertyDescriptor).GetMethod("GetPropertyValue");

		private static readonly MethodInfo DescriptorSetValue =
			typeof(PropertyDescriptor).GetMethod("SetPropertyValue");

		private static readonly PropertyInfo MetaDictionaryProp =
			typeof(IDictionaryAdapter).GetProperty("Dictionary");

		private static readonly PropertyInfo MetaPropertiesProp =
			typeof(IDictionaryAdapter).GetProperty("Properties");

		private static readonly MethodInfo MetaGetProperty =
			typeof(IDictionaryAdapter).GetMethod("GetProperty");

		private static readonly MethodInfo MetaSetProperty =
			typeof(IDictionaryAdapter).GetMethod("SetProperty");

		private static readonly MethodInfo MetaFetchProperties =
			typeof(IDictionaryAdapter).GetMethod("FetchProperties");

		private static readonly MethodInfo DictionaryInitialize =
			typeof(IDictionaryInitializer).GetMethod("Initialize");

		#endregion
	}
}
