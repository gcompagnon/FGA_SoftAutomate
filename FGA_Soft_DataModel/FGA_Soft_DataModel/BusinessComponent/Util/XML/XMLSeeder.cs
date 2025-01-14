﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace FGABusinessComponent.BusinessComponent.Util.XML
{
    /// <summary>
    /// Classe utilitaire pour traiter une initialisation des Entity Framework code first en XML
    /// voir http://codefirstseeder.codeplex.com/SourceControl/changeset/view/13889#238387
    /// </summary>
    public class XmlSeeder
    {
        private readonly IDictionary<string, object> _keys;

        public XmlSeeder()
        {
            _keys = new Dictionary<string, object>();
        }


        /// <summary>
        /// Seeds a newly created database with data from an XML file.
        /// This should be called from within the Seed Method of an implementation of IDatabaseInitializer
        /// </summary>
        /// <typeparam name="TContext">Type derived from DbContext</typeparam>
        /// <param name="context">Your context instance</param>
        /// <param name="xmlFilename">Seed data file name. If null, {YourContextClassShortName}.xml will be used</param>
        public void Seed<TContext>( TContext context, string xmlFilename = null ) where TContext : DbContext
        {
            Type contextType = typeof ( TContext );

            string directory = Path.GetDirectoryName( Assembly.GetExecutingAssembly().CodeBase );
            string fileName = xmlFilename ?? String.Format( "{0}.xml", contextType.Name );

            string filePath = Path.Combine( directory, fileName );

            IEnumerable<XElement> dbSetElements = XElement.Load( filePath ).Elements();

            foreach ( XElement dbSetElement in dbSetElements )
            {
                PropertyInfo dbSetProperty = contextType.GetProperty( dbSetElement.Name.LocalName );
                if ( dbSetProperty == null )
                {
                    throw new Exception( String.Format( "Property \"{0}\" not found in context: \"{1}\"", dbSetElement.Name.LocalName, contextType.FullName ) );
                }

                // Get the type of the IDbSet<Entity> 
                Type iDbSetType = dbSetProperty.PropertyType.GetInterfaces().FirstOrDefault( x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof ( IDbSet<> ) );
                if ( iDbSetType == null )
                {
                    throw new Exception( String.Format( "Property \"{0}\" in context \"{1}\" is not a DbSet<T>", dbSetElement.Name.LocalName, contextType.FullName ) );
                }

                // Get the first and only generic argument type of IDbSet<>. This is our entity type.
                Type entityType = iDbSetType.GetGenericArguments().First();

                IEnumerable entities = CreateEntityList( entityType, dbSetElement );

                object dbSet = dbSetProperty.GetValue( context, null );

                // Since we can't cast dbSet to IDbSet<T> (T is unknown until runtime), we must use refection to invoke the 'Add' method
                MethodInfo addMethod = dbSetProperty.PropertyType.GetMethod( "Add", new[] {entityType} );
                foreach ( object entity in entities )
                {
                    addMethod.Invoke( dbSet, new[] {entity} );
                }
            }
        }

        /// <summary>
        /// Creates a generic List of entities to be assigned to a parent entity navigation property or added to a DbSet
        /// </summary>
        /// <param name="type">Type of the List members</param>
        /// <param name="element">XML element containing child elements for this list</param>
        /// <returns>Although return type is IEumerable, the underlying type is actually a generic List so the returned value can be assigned to navigation properties</returns>
        private IEnumerable CreateEntityList( Type type, XContainer element )
        {
            // Create a new List<T> where typeof(T) == type
            object list = Activator.CreateInstance( typeof ( List<> ).MakeGenericType( type ) );

            // Since we can't cast list to List<T> (T is unknown until runtime), we must use refection to invoke the 'Add' method
            MethodInfo addMethod = list.GetType().GetMethod( "Add", new[] {type} );

            // Create and add each child entity to the list
            foreach ( XElement childElement in element.Elements() )
            {
                addMethod.Invoke( list, new[] {CreateEntity( type, childElement )} );
            }

            return list as IEnumerable;
        }


        /// <summary>
        /// Instantiates an entity and populates its properties
        /// </summary>
        /// <param name="type">entity type</param>
        /// <param name="element">XML element for entity</param>
        /// <returns></returns>
        private object CreateEntity( Type type, XElement element )
        {
            // if usekey attribute is defined, we retrieve an existing entity from the dictionary instead of creating a new one
            XAttribute useKeyAttribute = element.Attribute( "usekey" );
            if ( useKeyAttribute != null )
            {
                if ( !_keys.ContainsKey( useKeyAttribute.Value ) )
                {
                    throw new Exception( String.Format( "The key \"{0}\" was not found. You must define the entity with the key above any reference to it", useKeyAttribute.Value ) );
                }
                return _keys[useKeyAttribute.Value];
            }
            // if type attribute is defined, we take the constructor of that type name (usefull for inheritance)
            XAttribute typeAttribute = element.Attribute( "type" );
            if ( typeAttribute != null )
            {
                type = Type.GetType(typeAttribute.Value);
            }
            // Get parameterless constructor
            ConstructorInfo ctor = type.GetConstructor( new Type[] {} );

            if ( ctor == null )
            {
                throw new Exception( String.Format( "\"{0}\" does not have a parameterless constructor", type.FullName ) );
            }

            // Invoke the contructor to create a new object
            object entity = ctor.Invoke( new object[] {} );

            if (element.HasElements)
            {
                foreach (XElement propertyElement in element.Elements())
                {
                    PropertyInfo property = type.GetProperty(propertyElement.Name.LocalName);

                    if (property == null)
                    {
                        throw new Exception(String.Format("\"{0}\" does not contain property \"{1}\"", type.FullName, propertyElement.Name.LocalName));
                    }

                    property.SetValue(entity, GetPropertyValue(property.PropertyType, propertyElement), null);
                }
            }
            else // no Nodes, only one value
            { 
                PropertyInfo property = type.GetProperty(element.Name.LocalName);
                if (property == null)
                {
                    // use the string cast of the type class
                    try
                    {
                        object value = element.Value;

                        MethodInfo implicitCastMethod = type.GetMethod("op_Implicit",BindingFlags.Static | BindingFlags.Public);
                        if (implicitCastMethod != null)
                            entity = implicitCastMethod.Invoke(null, new object[] { value });
                        else
                        {
                            MethodInfo explicitCastMethod = type.GetMethod("op_Explicit", BindingFlags.Static | BindingFlags.Public);
                            entity = explicitCastMethod.Invoke(null, new object[] { value });
                        }                        
                    }
                    catch (Exception e)
                    {
                        throw new Exception(String.Format("\"{0}\" does not contain property \"{1}\" or not string casting", type.FullName, element.Name.LocalName));
                    }
                }
                else
                {
                    property.SetValue(entity, GetPropertyValue(property.PropertyType, element.Value), null);
                }
            }

            // if key attribute is defined, save this entity to the dictionary to we can add it to a many-to-many relationships later
            XAttribute keyAttribute = element.Attribute( "key" );
            if ( keyAttribute != null )
            {
                _keys.Add( keyAttribute.Value, entity );
            }

            return entity;
        }


        /// <summary>
        /// Creates an entity's property value. May be simple type or navigation property
        /// </summary>
        /// <param name="propertyType">Target property type</param>
        /// <param name="propertyElement">XML element for property</param>
        /// <returns></returns>
        private object GetPropertyValue( Type propertyType, XElement propertyElement )
        {
            // if simple type, return converted string
            if ( propertyType.IsValueType || propertyType == typeof ( string ) || propertyType == typeof ( byte[] ) )
            {
                return ConvertToType( propertyType, propertyElement.Value );
            }

            // if this is a "to-many" navigation property, return the list of children
            Type iEnumerableType = propertyType.GetInterfaces().FirstOrDefault( x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof ( IEnumerable<> ) );
            if ( iEnumerableType != null )
            {
                Type type = iEnumerableType.GetGenericArguments().First();
                return CreateEntityList( type, propertyElement );
            }

            // if this is a "to-one" navigation property, create the single entity
            return CreateEntity( propertyType, propertyElement );
        }

        /// <summary>
        /// Creates an entity's value of the correct given type (string parser)
        /// </summary>
        /// <param name="propertyType">Target property type</param>
        /// <param name="propertyValue">value of the element</param>
        /// <returns></returns>
        private object GetPropertyValue(Type propertyType, String propertyValue)
        {
            return ConvertToType(propertyType, propertyValue);
        }


        /// <summary>
        /// Simple type converter to convert string values to the entity's property type
        /// </summary>
        /// <param name="targetType">Target property type</param>
        /// <param name="value">String value from XML</param>
        /// <returns></returns>
        private static object ConvertToType( Type targetType, string value )
        {
            return Convert.ChangeType(value, targetType);
        //    try
        //    {
        //        if ( targetType == typeof ( string ) )
        //        {
        //            return value;
        //        }

        //        if ( targetType == typeof ( Int32 ) || targetType == typeof ( Int32? ) )
        //        {
        //            return Int32.Parse( value );
        //        }

        //        if ( targetType == typeof ( Int16 ) || targetType == typeof ( Int16? ) )
        //        {
        //            return Int16.Parse( value );
        //        }

        //        if ( targetType == typeof ( Int64 ) || targetType == typeof ( Int64? ) )
        //        {
        //            return Int64.Parse( value );
        //        }

        //        if ( targetType == typeof ( Boolean ) || targetType == typeof ( Boolean? ) )
        //        {
        //            return Boolean.Parse( value );
        //        }

        //        if ( targetType == typeof ( Decimal ) || targetType == typeof ( Decimal? ) )
        //        {
        //            return Decimal.Parse( value );
        //        }

        //        if ( targetType == typeof ( Single ) || targetType == typeof ( Single? ) )
        //        {
        //            return Single.Parse( value );
        //        }

        //        if ( targetType == typeof ( Double ) || targetType == typeof ( Double? ) )
        //        {
        //            return Double.Parse( value );
        //        }

        //        if ( targetType == typeof ( DateTime ) || targetType == typeof ( DateTime? ) )
        //        {
        //            return DateTime.Parse( value );
        //        }

        //        if ( targetType == typeof ( Guid ) || targetType == typeof ( Guid? ) )
        //        {
        //            return Guid.Parse( value );
        //        }

        //        if ( targetType == typeof ( Byte ) || targetType == typeof ( Byte? ) )
        //        {
        //            return Byte.Parse( value );
        //        }

        //        if ( targetType == typeof ( SByte ) || targetType == typeof ( SByte? ) )
        //        {
        //            return SByte.Parse( value );
        //        }

        //        if ( targetType == typeof ( Byte[] ) )
        //        {
        //            var hexRegEx = new Regex( "[^A-Fa-f0-9]" );
        //            string hexString = hexRegEx.Replace( value, String.Empty );

        //            var bytes = new byte[hexString.Length / 2];
        //            for ( int i = 0; i < hexString.Length; i += 2 )
        //            {
        //                bytes[i / 2] = Convert.ToByte( hexString.Substring( i, 2 ), 16 );
        //            }
        //            return bytes;
        //        }

        //        if ( targetType == typeof ( DateTimeOffset ) || targetType == typeof ( DateTimeOffset? ) )
        //        {
        //            return DateTimeOffset.Parse( value );
        //        }

        //        if ( targetType == typeof ( TimeSpan ) || targetType == typeof ( TimeSpan? ) )
        //        {
        //            return TimeSpan.Parse( value );
        //        }

        //        throw new Exception( String.Format( "Can't convert to type \"{0}\"", targetType.FullName ) );
        //    }
        //    catch ( Exception ex )
        //    {
        //        throw new Exception( String.Format( "Error converting \"{0}\" to type \"{1}\". See inner exception.", value, targetType.FullName ), ex );
        //    }
        }
    }
}
