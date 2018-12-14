using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using Canon.Eos.Framework.Helper;
using Canon.Eos.Framework.Internal.SDK;

namespace Canon.Eos.Framework
{
    public abstract class EosObject : EosDisposable
    {
        private readonly IntPtr _handle;
        protected object _locker = new object();

        internal EosObject(IntPtr handle)
        {
            _handle = handle;
        }

        public IntPtr Handle
        {
            get { return _handle; }
        }

        /// <summary>
        /// Gets the artist.
        /// </summary>
        [EosProperty(Edsdk.PropID_Artist)]
        public string Artist
        {
            get { return this.GetPropertyStringData(Edsdk.PropID_Artist); }
        }

        /// <summary>
        /// Gets the copyright.
        /// </summary>
        [EosProperty(Edsdk.PropID_Copyright)]
        public string Copyright
        {
            get { return this.GetPropertyStringData(Edsdk.PropID_Copyright); }
        }

        /// <summary>
        /// Gets the focus.
        /// </summary>
        [EosProperty(Edsdk.PropID_FocusInfo)]
        public EosFocus Focus
        {
            get 
            {   return EosFocus.Create(this.GetPropertyStruct<Edsdk.EdsFocusInfo>(Edsdk.PropID_FocusInfo, 
                    Edsdk.EdsDataType.FocusInfo));
            }
        }

        /// <summary>
        /// Gets the firmware version.
        /// </summary>
        [EosProperty(Edsdk.PropID_FirmwareVersion)]
        public string FirmwareVersion
        {
            get { return this.GetPropertyStringData(Edsdk.PropID_FirmwareVersion); }
        }

        /// <summary>
        /// Gets the name of the owner.
        /// </summary>
        /// <value>
        /// The name of the owner.
        /// </value>
        [EosProperty(Edsdk.PropID_OwnerName)]
        public string OwnerName
        {
            get { return this.GetPropertyStringData(Edsdk.PropID_OwnerName); }
        }

        /// <summary>
        /// Gets the name of the product.
        /// </summary>
        /// <value>
        /// The name of the product.
        /// </value>
        [EosProperty(Edsdk.PropID_ProductName)]
        public string ProductName
        {
            get { return this.GetPropertyStringData(Edsdk.PropID_ProductName); }
        }

        /// <summary>
        /// Gets the serial number.
        /// </summary>
        [EosProperty(Edsdk.PropID_BodyIDEx)]
        public string SerialNumber
        {
            get { return this.GetPropertyStringData(Edsdk.PropID_BodyIDEx); }
        }

        /// <summary>
        /// Gets or sets the white balance.
        /// </summary>
        /// <value>
        /// The white balance.
        /// </value>
        [EosProperty(Edsdk.PropID_WhiteBalance)]
        public EosWhiteBalance WhiteBalance
        {
            get { return (EosWhiteBalance)this.GetPropertyIntegerData(Edsdk.PropID_WhiteBalance); }
            set { this.SetPropertyIntegerData(Edsdk.PropID_WhiteBalance, (long)value); }
        }
        
        protected internal override void DisposeUnmanaged()
        {
            if(_handle != IntPtr.Zero)
                Edsdk.EdsRelease(_handle);
            base.DisposeUnmanaged();
        }

        protected virtual void ExecuteSetter(Action action) 
        {
            lock (_locker)
            {
                action();
            }
        }

        protected virtual TResult ExecuteGetter<TResult>(Func<TResult> function)
        {
            return function();
        }
        

        internal int GetPropertyDataSize(uint propertyId, Edsdk.EdsDataType expectedDataType)
        {
            return this.ExecuteGetter(() =>
            {
                int dataSize;
                Edsdk.EdsDataType dataType;
                Util.Assert(Edsdk.EdsGetPropertySize(this.Handle, propertyId, 0, out dataType, out dataSize),
                    "Failed to get property size.", propertyId);
                //Util.AssertIf(expectedDataType != dataType, "DataType mismatch: Expected <{0}>, Actual <{1}>", expectedDataType, dataType);
                return dataSize;
            });
        }

        internal T GetPropertyStruct<T>(uint propertyId, Edsdk.EdsDataType expectedDataType) where T: struct
        {
            return this.ExecuteGetter(() =>
            {
                var dataSize = this.GetPropertyDataSize(propertyId, expectedDataType);
                var ptr = Marshal.AllocHGlobal(dataSize);
                try
                {
                    Util.Assert(Edsdk.EdsGetPropertyData(this.Handle, propertyId, 0, dataSize, ptr),
                        "Failed to get required struct.", propertyId);
                    return (T)Marshal.PtrToStructure(ptr, typeof(T));
                }
                finally
                {
                    Marshal.FreeHGlobal(ptr);
                }
            });
        }

        internal void SetPropertyStruct<T>(uint propertyId,T data) where T : struct
        {
            this.ExecuteSetter(() =>
            {
                try
                {
                    Util.Assert(Edsdk.EdsSetPropertyData(this.Handle, propertyId, 0, Marshal.SizeOf((object)data), (object)data),
                        string.Format("Failed to set property string data: propertyId {0}, data {1}", propertyId, data),
                        propertyId, data);
                }
                finally
                {
                
                }
            });
        }
        public Edsdk.EdsPropertyDesc GetPropertyDescription(uint propertyId)
        {
            return this.ExecuteGetter(() =>
            {
                Edsdk.EdsPropertyDesc desc;
                Util.Assert(Edsdk.EdsGetPropertyDesc(this.Handle, propertyId, out desc),
                    string.Format("Failed to get property description for data: propertyId {0}", propertyId), propertyId);
                return desc;
            });
        }
        
        protected long GetPropertyIntegerData(uint propertyId)
        {
            return this.ExecuteGetter(() =>
            {
                uint data;
                Util.Assert(Edsdk.EdsGetPropertyData(this.Handle, propertyId, 0, out data),
                    string.Format("Failed to get property integer data: propertyId {0}", propertyId), propertyId);
                return data;
            });
        }

        protected Point GetPropertyPointData(uint propertyId)
        {
            var point = this.GetPropertyStruct<Edsdk.EdsPoint>(propertyId, Edsdk.EdsDataType.Point);
            return new Point { X = point.x, Y = point.y };
        }

        protected Size GetPropertySizeData(uint propertyId)
        {
            var point = this.GetPropertyStruct<Edsdk.EdsSize>(propertyId, Edsdk.EdsDataType.Point);
            return new Size { Width = point.width, Height = point.height };
        }

        protected Rectangle GetPropertyRectangleData(uint propertyId)
        {
            var rect = this.GetPropertyStruct<Edsdk.EdsRect>(propertyId, Edsdk.EdsDataType.Rect);
            return new Rectangle { X = rect.x, Y = rect.y, Height = rect.height, Width = rect.width };
        }

        protected long[] GetPropertyIntegerArrayData(uint propertyId)
        {
            return this.ExecuteGetter(() =>
            {
                var dataSize = this.GetPropertyDataSize(propertyId, Edsdk.EdsDataType.UInt32_Array);
                var ptr = Marshal.AllocHGlobal(dataSize);
                try
                {
                    Util.Assert(Edsdk.EdsGetPropertyData(this.Handle, propertyId, 0, dataSize, ptr),
                        "Failed to get required struct.", propertyId);

                    var signed = new int[dataSize / Marshal.SizeOf(typeof(uint))];
                    Marshal.Copy(ptr, signed, 0, signed.Length);
                    return signed.Select(i => (long)(uint)i).ToArray();
                }
                finally
                {
                    Marshal.FreeHGlobal(ptr);
                }
            });
        }

        protected string GetPropertyStringData(uint propertyId)
        {
            return this.ExecuteGetter(() =>
            {
                string data;
                Util.Assert(Edsdk.EdsGetPropertyData(this.Handle, propertyId, 0, out data),
                    string.Format("Failed to get property string data: propertyId {0}", propertyId), propertyId);
                return data;
            });
        }
        
        protected void SetPropertyIntegerData(uint propertyId, long data)
        {
            this.ExecuteSetter(() => Util.Assert(Edsdk.EdsSetPropertyData(this.Handle, propertyId, 0, Marshal.SizeOf(typeof(uint)), (uint)data),
                string.Format("Failed to set property integer data: propertyId {0}, data {1}", propertyId, data),
                propertyId, data));
        }

        public void SetPropertyIntegerArrayData(uint propertyId, uint[] data)
        {
            this.ExecuteSetter(() => Util.Assert(Edsdk.EdsSetPropertyData(this.Handle, propertyId, 0, Marshal.SizeOf(typeof(uint)) * data.Length, data),
                string.Format("Failed to set property integer array data: propertyId {0}, data {1}", propertyId, data),
                propertyId, data));
        }

        protected void SetPropertyStringData(uint propertyId, string data, int maxByteLength)
        {
            this.ExecuteSetter(() =>
            {
                var bytes = Util.ConvertStringToBytesWithNullByteAtEnd(data);
                if (bytes.Length > maxByteLength)
                    throw new ArgumentException(string.Format("'{0}' converted to bytes is longer than {1}.", data, maxByteLength), "data");

                Util.Assert(Edsdk.EdsSetPropertyData(this.Handle, propertyId, 0, bytes.Length, bytes),
                    string.Format("Failed to set property string data: propertyId {0}, data {1}", propertyId, data),
                    propertyId, data);
            });
        }
    }
}
