using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MyIoC.Attributes;

namespace MyIoC
{
    public class Container
    {
        private readonly Dictionary<Type, Type> _registeredTypes;
        private readonly List<Type> _importConstructors;
        private readonly List<Type> _importProperty;

        public Container()
        {
            _importConstructors = new List<Type>();
            _importProperty = new List<Type>();
            _registeredTypes = new Dictionary<Type, Type>();
        }

        /// <summary>
        /// Register Assembly into container
        /// </summary>
        /// <param name="assembly"></param>
        public void AddAssembly(Assembly assembly)
        {
            if (assembly == null) throw new ArgumentNullException(nameof(assembly));

            foreach (var type in assembly.GetTypes())
                AddType(type);
        }

        /// <summary>
        /// Create Instansce with a given type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="constructorParams"></param>
        /// <returns></returns>
        public T CreateInstance<T>(params Type[] constructorParams)
        {
            return (T) CreateInstance(typeof(T), constructorParams);
        }

        /// <summary>
        /// Create Instansce with a given type
        /// </summary>
        /// <param name="type"></param>
        /// <param name="constructorParams"></param>
        /// <returns></returns>
        public object CreateInstance(Type type, params Type[] constructorParams)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));

            if (!_registeredTypes.ContainsKey(type))
                throw new NullReferenceException("We didn't find the key of type");

            if (_importConstructors.Contains(type))
                return InvokeConstructor(type, constructorParams);

            if (_importProperty.Contains(type))
                return CreateTypeWithResolvedProperties(type);

            return Activator.CreateInstance(_registeredTypes[type]);
        }

        /// <summary>
        /// Register and resolve type and base type into container
        /// </summary>
        /// <param name="type"></param>
        /// <param name="baseType"></param>
        /// <returns></returns>
        public void AddType(Type type, Type baseType)
        {
            if (type == null || baseType == null)
                throw new NullReferenceException("Type can't be null");

            if (!baseType.IsAssignableFrom(type))
                throw new ArgumentException("Type should be inherit of type two");

            _registeredTypes.Add(baseType, type);
        }

        /// <summary>
        /// Register and resolve type into container
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public void AddType(Type type)
        {
            if (type.GetProperties().Any(propertyInfo =>
                propertyInfo.CustomAttributes.Any(j => j.AttributeType == typeof(ImportAttribute))))
                _importProperty.Add(type);

            ValidateType(type);

            if (type.CustomAttributes.Any(i => i.AttributeType == typeof(ExportAttribute)))
            {
                ResolveExports(type);
                return;
            }

            if (type.CustomAttributes.Any(attribute => attribute.AttributeType == typeof(ImportConstructorAttribute)))
                _importConstructors.Add(type);

            if (!type.IsInterface)
                _registeredTypes.Add(type, type);
        }

        /// <summary>
        /// Add Export types in dictionary, check case with params in attribute constructors
        /// </summary>
        /// <param name="type"></param>
        private void ResolveExports(Type type)
        {
            var attribute = type.CustomAttributes
                .FirstOrDefault(attr => attr.AttributeType == typeof(ExportAttribute));

            if (attribute.ConstructorArguments.Count < 1)
            {
                _registeredTypes.Add(type, type);
            }
            else
            {
                AddType(type, (Type) attribute.ConstructorArguments[0].Value);
            }
        }

        /// <summary>
        /// Resolve properties type and create
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private object CreateTypeWithResolvedProperties(Type type)
        {
            var resultObject = Activator.CreateInstance(type);
            var properies = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);

            foreach (var property in properies)
            {
                property.SetValue(resultObject, Activator.CreateInstance(_registeredTypes[property.PropertyType]));
            }

            return resultObject;
        }

        /// <summary>
        /// If type is invalid throw new Exception
        /// </summary>
        /// <param name="type"></param>
        private void ValidateType(Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));

            var countOfAttributes = type.CustomAttributes.Count(attribute =>
                attribute.AttributeType == typeof(ExportAttribute)
                || attribute.AttributeType ==
                typeof(ImportConstructorAttribute));
            if (_importProperty.Contains(type))
                countOfAttributes++;

            if (countOfAttributes > 1)
                throw new Exception("In type should be only one attribute");
        }

        /// <summary>
        /// Looking for the required constructor and calling it
        /// </summary>
        /// <param name="type"></param>
        /// <param name="constructorParams"></param>
        /// <returns></returns>
        private object InvokeConstructor(Type type, params Type[] constructorParams)
        {
            var constructor = constructorParams.Length > 0
                ? type.GetConstructor(constructorParams.ToArray())
                : type.GetConstructors().FirstOrDefault();

            if (constructor == null)
                throw new Exception("We didn't find constructor");

            var parametrs = new List<object>();
            foreach (var parameter in constructor.GetParameters())
            {
                if (!_registeredTypes.ContainsKey(parameter.ParameterType))
                    throw new NullReferenceException("We didn't find the key of type");

                parametrs.Add(Activator.CreateInstance(_registeredTypes[parameter.ParameterType]));
            }

            return constructor.Invoke(parametrs.ToArray());
        }
    }
}
