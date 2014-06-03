using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace PortableDeviceLib.Model
{
    /// <summary>
    /// Represent an object that can contains other objects
    /// </summary>
    public class PortableDeviceContainerObject : PortableDeviceObject
    {
        private ObservableCollection<PortableDeviceObject> childs;

        /// <summary>
        /// Initialize a new instance of the <see cref="PortableDeviceContainerObject"/> class
        /// </summary>
        /// <param name="id"></param>
        public PortableDeviceContainerObject(string id)
            : base(id)
        {
            this.childs = new ObservableCollection<PortableDeviceObject>();
            this.Childs = new ReadOnlyObservableCollection<PortableDeviceObject>(this.childs);
        }

        /// <summary>
        /// Gets the childs of this object
        /// </summary>
        public ReadOnlyObservableCollection<PortableDeviceObject> Childs
        {
            get;
            private set;
        }

        /// <summary>
        /// Add a child in collection
        /// </summary>
        /// <param name="child"></param>
        internal void AddChild(PortableDeviceObject child)
        {
            if (this.childs.Contains(child))
                return;
            this.childs.Add(child);
        }
    }
}
