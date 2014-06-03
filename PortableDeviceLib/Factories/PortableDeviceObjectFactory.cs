using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PortableDeviceLib.Model;

namespace PortableDeviceLib.Factories
{
    /// <summary>
    /// Represent a factory that can build <see cref="PortableDeviceObject"/>
    /// You can register new method to handle new object types
    /// </summary>
    public class PortableDeviceObjectFactory
    {
        private static PortableDeviceObjectFactory instance;
        private Dictionary<Guid, Func<string, string, string , string, PortableDeviceObject>> factoryMethods;

        /// <summary>
        /// Initialize a new instance of the <see cref="PortableDeviceObjectFactory"/> class
        /// This is a private constructor to support Singleton pattern
        /// </summary>
        private PortableDeviceObjectFactory()
        {
            this.factoryMethods = new Dictionary<Guid, Func<string, string, string, string, PortableDeviceObject>>();

            // Register know object type
            this.RegisterNewFactoryMethod(PortableDeviceGuids.WPD_CONTENT_TYPE_FOLDER, new Func<string, string, string, string, PortableDeviceObject>(CreateFolderObject));
            this.RegisterNewFactoryMethod(PortableDeviceGuids.WPD_CONTENT_TYPE_FUNCTIONAL_OBJECT, new Func<string, string, string, string, PortableDeviceObject>(CreateFunctionalObject));
        }

        #region Properties

        /// <summary>
        /// Gets the unique instance of factory
        /// </summary>
        public static PortableDeviceObjectFactory Instance
        {
            get
            {
                if (instance == null)
                    instance = new PortableDeviceObjectFactory();
                return instance;
            }
        }

        #endregion

        #region Public functions

        /// <summary>
        /// Register a new factory method that enable create new object type
        /// </summary>
        /// <param name="handledType"></param>
        /// <param name="method"></param>
        public void RegisterNewFactoryMethod(Guid handledType, Func<string, string, string, string, PortableDeviceObject> method)
        {
            if (handledType == Guid.Empty)
                throw new ArgumentException("handledType cann't be Guid.Empty", "handledType");
            if (method == null)
                throw new ArgumentNullException("method");

            if (this.factoryMethods.ContainsKey(handledType))
                throw new ArgumentException(string.Format("Guid {0} is already registered", handledType), "handledType");

            this.factoryMethods.Add(handledType, method);
        }

        /// <summary>
        /// Create a new instance of a portableDeviceObject or derived from type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public PortableDeviceObject CreateInstance(Guid type, string id, string name, string contentType, string format)
        {
            if (this.factoryMethods.ContainsKey(type))
                return this.factoryMethods[type](id, name, contentType, format);
            else
                return this.CreateGenericObject(id, name, contentType, format);
        }

        #endregion

        #region Private functions

        private PortableDeviceObject CreateFunctionalObject(string id, string name, string contentType, string format)
        {
            var obj = new PortableDeviceFonctionalObject(id);
            this.InitializeInstance(obj, name, contentType, format);
            return obj;
        }

        private PortableDeviceObject CreateFolderObject(string id, string name, string contentType, string format)
        {
            var obj = new PortableDeviceFolderObject(id);
            this.InitializeInstance(obj, name, contentType, format);
            return obj;
        }

        private PortableDeviceObject CreateGenericObject(string id, string name, string contentType, string format)
        {
            var obj = new PortableDeviceObject(id);
            this.InitializeInstance(obj, name, contentType, format);
            return obj;
        }

        private void InitializeInstance(PortableDeviceObject obj, string name, string contentType, string format)
        {
            obj.Name = name;
            obj.ContentType = contentType;
            obj.Format = format;
        }

        #endregion

    }
}
