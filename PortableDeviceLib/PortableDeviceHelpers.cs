#region License
/*
PortableDeviceHelpers.cs
Copyright (C) 2009 Vincent Lainé
 
This library is free software; you can redistribute it and/or
modify it under the terms of the GNU Lesser General Public
License as published by the Free Software Foundation; either
version 2.1 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public
License along with this library; if not, write to the Free Software
Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
*/
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Reflection;

namespace PortableDeviceLib
{

    // Nested Types
    [StructLayout(LayoutKind.Explicit, Size = 16)]
    public struct PropVariant
    {
        [FieldOffset(0)]
        public short variantType;
        [FieldOffset(8)]
        public IntPtr pointerValue;
        [FieldOffset(8)]
        public byte byteValue;
        [FieldOffset(8)]
        public long longValue;
        [FieldOffset(8)]
        public double dateValue;
        [FieldOffset(8)]
        public short boolValue;
    } 


    internal static class PortableDeviceHelpers
    {
        public static int VT_DATE;
        public static int VT_LPWSTR;
        public static int VT_UI4;
        public static int VT_UI8;
        public static int VT_UINT;

        private static Dictionary<string, PortableDeviceApiLib._tagpropertykey> _values;
        private static Dictionary<Guid, string> _portableDeviceGuidWithName;

        static PortableDeviceHelpers()
        {
            VT_LPWSTR = 0x1f;
            VT_DATE = 7;
            VT_UI4 = 0x13;
            VT_UINT = 0x17;
            VT_UI8 = 0x15;

            _values = new Dictionary<string, PortableDeviceApiLib._tagpropertykey>();
            _values.Add(MTPConstants.OBJECT_PLAY_COUNT, PortableDevicePKeys.WPD_MEDIA_USE_COUNT);
            _values.Add(MTPConstants.OBJECT_GENRE, PortableDevicePKeys.WPD_MEDIA_GENRE);
            _values.Add(MTPConstants.OBJECT_STAR_RATING, PortableDevicePKeys.WPD_MEDIA_STAR_RATING);
            _values.Add(MTPConstants.OBJECT_ARTIST, PortableDevicePKeys.WPD_MEDIA_ARTIST);
            _values.Add(MTPConstants.OBJECT_ALBUM_ARTIST, PortableDevicePKeys.WPD_MEDIA_ALBUM_ARTIST);
            _values.Add(MTPConstants.OBJECT_SUBSCRIPTION_CONTENT_ID, PortableDevicePKeys.WPD_MEDIA_SUBSCRIPTION_CONTENT_ID);
            _values.Add(MTPConstants.OBJECT_EFFECTIVE_RATING, PortableDevicePKeys.WPD_MEDIA_USER_EFFECTIVE_RATING);
            _values.Add(MTPConstants.OBJECT_ALBUM, PortableDevicePKeys.WPD_MUSIC_ALBUM);
            _values.Add(MTPConstants.OBJECT_TRACK, PortableDevicePKeys.WPD_MUSIC_TRACK);
            _values.Add(MTPConstants.OBJECT_ID, PortableDevicePKeys.WPD_OBJECT_ID);
            _values.Add(MTPConstants.OBJECT_SIZE, PortableDevicePKeys.WPD_OBJECT_SIZE);
            _values.Add(MTPConstants.OBJECT_DURATION, PortableDevicePKeys.WPD_MEDIA_DURATION);
            _values.Add(MTPConstants.OBJECT_TITLE, PortableDevicePKeys.WPD_MEDIA_TITLE);
            _values.Add(MTPConstants.OBJECT_DATE_AUTHORED, PortableDevicePKeys.WPD_OBJECT_DATE_AUTHORED);
            _values.Add(MTPConstants.OBJECT_DATE_CREATED, PortableDevicePKeys.WPD_OBJECT_DATE_CREATED);
            _values.Add(MTPConstants.OBJECT_MODIFIED, PortableDevicePKeys.WPD_OBJECT_DATE_MODIFIED);
            _values.Add(MTPConstants.OBJECT_FORMAT, PortableDevicePKeys.WPD_OBJECT_FORMAT);
            _values.Add(MTPConstants.OBJECT_DRM_PROTECTED, PortableDevicePKeys.WPD_OBJECT_IS_DRM_PROTECTED);
            _values.Add(MTPConstants.OBJECT_FILENAME, PortableDevicePKeys.WPD_OBJECT_ORIGINAL_FILE_NAME);
            _values.Add(MTPConstants.OBJECT_NAME, PortableDevicePKeys.WPD_OBJECT_NAME);
            _values.Add(MTPConstants.OBJECT_PERSISTENT_ID, PortableDevicePKeys.WPD_OBJECT_PERSISTENT_UNIQUE_ID);
            _values.Add(MTPConstants.OBJECT_TOTAL_BITRATE, PortableDevicePKeys.WPD_MEDIA_TOTAL_BITRATE);
            _values.Add(MTPConstants.OBJECT_SKIP_COUNT, PortableDevicePKeys.WPD_MEDIA_SKIP_COUNT);
            _values.Add(MTPConstants.OBJECT_SAMPLE_RATE, PortableDevicePKeys.WPD_MEDIA_SAMPLE_RATE);
            _values.Add(MTPConstants.OBJECT_RELEASE_DATE, PortableDevicePKeys.WPD_MEDIA_RELEASE_DATE);
            _values.Add(MTPConstants.OBJECT_LAST_BUILD_DATE, PortableDevicePKeys.WPD_MEDIA_LAST_BUILD_DATE);
            _values.Add(MTPConstants.OBJECT_LAST_ACCESSED_TIME, PortableDevicePKeys.WPD_MEDIA_LAST_ACCESSED_TIME);
            _values.Add(MTPConstants.OBJECT_BITRATE_TYPE, PortableDevicePKeys.WPD_MEDIA_BITRATE_TYPE);
            _values.Add(MTPConstants.OBJECT_ALBUM_ART, PortableDevicePKeys.WPD_RESOURCE_ALBUM_ART);
            _values.Add(MTPConstants.DEVICE_FIRMWARE_VERSION, PortableDevicePKeys.WPD_DEVICE_FIRMWARE_VERSION);
            _values.Add(MTPConstants.DEVICE_FRIENDLY_NAME, PortableDevicePKeys.WPD_DEVICE_FRIENDLY_NAME);
            _values.Add(MTPConstants.DEVICE_MANUFACTURER, PortableDevicePKeys.WPD_DEVICE_MANUFACTURER);
            _values.Add(MTPConstants.DEVICE_MODEL, PortableDevicePKeys.WPD_DEVICE_MODEL);
            _values.Add(MTPConstants.DEVICE_BATTERY_LEVEL, PortableDevicePKeys.WPD_DEVICE_POWER_LEVEL);
            _values.Add(MTPConstants.DEVICE_POWER_SOURCE, PortableDevicePKeys.WPD_DEVICE_POWER_SOURCE);
            _values.Add(MTPConstants.DEVICE_SERIAL_NUMBER, PortableDevicePKeys.WPD_DEVICE_SERIAL_NUMBER);
            _values.Add(MTPConstants.OBJECT_CONTENT_TYPE, PortableDevicePKeys.WPD_OBJECT_CONTENT_TYPE);

            _portableDeviceGuidWithName = MakeGlobalDictionary();
        }

        public static Dictionary<Guid, string> PortableDeviceGuidWithName
        {
            get
            {
                return _portableDeviceGuidWithName;
            }
        }

        public static string GetNameFromGuid(PortableDeviceApiLib._tagpropertykey key, PortableDeviceApiLib.tag_inner_PROPVARIANT values)
        {
            PortableDeviceApiLib.IPortableDeviceValues pValues = (PortableDeviceApiLib.IPortableDeviceValues)new PortableDeviceTypesLib.PortableDeviceValuesClass();
            string contentTypeName;

            pValues.SetValue(ref key, ref values);
            pValues.GetStringValue(ref key, out contentTypeName);

            return contentTypeName;
        }

        public static string GetKeyNameFromGuid(Guid guid)
        {
            if (PortableDeviceHelpers.PortableDeviceGuidWithName.ContainsKey(guid))
                return PortableDeviceHelpers.PortableDeviceGuidWithName[guid];
            else
                return guid.ToString();
        }

        internal static string GetKeyNameFromPropkey(PortableDeviceApiLib._tagpropertykey propertyKey)
        {
            foreach (KeyValuePair<string, PortableDeviceApiLib._tagpropertykey> de in _values)
            {
                if ((propertyKey.pid == de.Value.pid) && (propertyKey.fmtid == de.Value.fmtid))
                {
                    return (string)de.Key;
                }
            }

            return (propertyKey.pid.ToString() + " " + propertyKey.fmtid.ToString());

        }

        private static Dictionary<Guid, string> MakeGlobalDictionary()
        {
            Dictionary<Guid, string> dic = new Dictionary<Guid, string>();
            foreach (FieldInfo fi in typeof(PortableDeviceGuids).GetFields())
            {
                dic.Add((Guid)fi.GetValue(null), fi.Name);
            }

            return dic;
        }

        

    }
}
