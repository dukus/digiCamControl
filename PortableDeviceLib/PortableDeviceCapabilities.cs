#region License
/*
PortableDeviceCapabilities.cs
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

using PortableDeviceApiLib;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Reflection;
using PortableDeviceLib.Model;

namespace PortableDeviceLib
{
    /// <summary>
    /// Represent the device capabilities
    /// </summary>
    public class PortableDeviceCapabilities
    {

        private Dictionary<Guid, FunctionalCategory> functionalCategories;
        private Dictionary<string, _tagpropertykey> commands;
        private Dictionary<PortableDeviceEventDescription, _tagpropertykey> events;

        /// <summary>
        /// Default constructor
        /// </summary>
        internal PortableDeviceCapabilities()
        {
            this.functionalCategories = new Dictionary<Guid, FunctionalCategory>();
            this.commands = new Dictionary<string, _tagpropertykey>();
            this.events = new Dictionary<PortableDeviceEventDescription, _tagpropertykey>();
        }

        /// <summary>
        /// Gets all functional categories
        /// </summary>
        public IEnumerable<FunctionalCategory> FunctionalCategories
        {
            get
            {
                return this.functionalCategories.Values;
            }
        }

        /// <summary>
        /// Gets commands from devices
        /// </summary>
        public IEnumerable<string> Commands
        {
            get
            {
                return this.commands.Keys;
            }
        }

        /// <summary>
        /// Gets supported events
        /// </summary>
        public IEnumerable<PortableDeviceEventDescription> Events
        {
            get
            {
                return this.events.Keys;
            }
        }

        /// <summary>
        /// Extract device capabilities
        /// </summary>
        /// <param name="portableDeviceClass"></param>
        internal void ExtractDeviceCapabilities(PortableDeviceClass portableDeviceClass)
        {
            if (portableDeviceClass == null)
                throw new ArgumentNullException("portableDeviceClass");

            try
            {
                PortableDeviceApiLib.IPortableDeviceCapabilities capabilities;
                portableDeviceClass.Capabilities(out capabilities);

                if (capabilities == null)
                {
                    Trace.WriteLine("Cannot extract capabilities from device");
                    throw new PortableDeviceException("Cannot extract capabilities from device");
                }

                IPortableDevicePropVariantCollection functionalCategories;
                capabilities.GetFunctionalCategories(out functionalCategories);

                if (functionalCategories == null)
                {
                    throw new PortableDeviceException("Failed to extract functionnal categories");
                }

                uint countCategories = 1;
                functionalCategories.GetCount(ref countCategories);
                tag_inner_PROPVARIANT values = new tag_inner_PROPVARIANT();

                PortableDeviceApiLib.IPortableDeviceValues pValues = (PortableDeviceApiLib.IPortableDeviceValues)new PortableDeviceTypesLib.PortableDeviceValuesClass();
                string categoryName;
                Guid currentGuid;
                for (uint i = 0; i < countCategories; i++)
                {
                    functionalCategories.GetAt(i, ref values);

                    pValues.SetValue(ref PortableDevicePKeys.WPD_COMMAND_CAPABILITIES_GET_SUPPORTED_FUNCTIONAL_CATEGORIES, ref values);
                    pValues.GetStringValue(ref PortableDevicePKeys.WPD_COMMAND_CAPABILITIES_GET_SUPPORTED_FUNCTIONAL_CATEGORIES, out categoryName);
                    currentGuid = new Guid(categoryName);
                    this.functionalCategories.Add(currentGuid, new FunctionalCategory(
                        portableDeviceClass,
                        currentGuid,
                        PortableDeviceHelpers.GetKeyNameFromGuid(currentGuid)));

                }

            }
            catch (Exception ex)
            {
                throw new PortableDeviceException("Error on extract device capabilities", ex);
            }
        }

        /// <summary>
        /// Extract command supported by device
        /// </summary>
        /// <param name="portableDeviceClass"></param>
        internal void ExtractCommands(PortableDeviceClass portableDeviceClass)
        {
            PortableDeviceApiLib.IPortableDeviceCapabilities capabilities;
            portableDeviceClass.Capabilities(out capabilities);

            PortableDeviceApiLib.IPortableDeviceKeyCollection values;
            capabilities.GetSupportedCommands(out values);

            _tagpropertykey key = new _tagpropertykey();
            _tagpropertykey tt;
            string currentName;

            uint count = 1;
            values.GetCount(ref count);
            for (uint i = 0; i < count; i++)
            {
                values.GetAt(i, ref key);

                currentName = string.Empty;
                foreach (FieldInfo fi in typeof(PortableDevicePKeys).GetFields())
                {
                    tt = (_tagpropertykey)fi.GetValue(null);
                    if (key.fmtid == tt.fmtid && key.pid == tt.pid)
                        currentName = fi.Name;
                }

                if (!string.IsNullOrEmpty(currentName))
                    this.commands.Add(currentName, key);
                else
                    this.commands.Add(key.pid + " " + key.fmtid, key);

            }
        }

        /// <summary>
        /// Extract event supported by device
        /// </summary>
        /// <param name="portableDeviceClass"></param>
        internal void ExtractEvents(PortableDeviceClass portableDeviceClass)
        {
            PortableDeviceApiLib.IPortableDeviceCapabilities capabilities;
            portableDeviceClass.Capabilities(out capabilities);

            PortableDeviceApiLib.IPortableDevicePropVariantCollection events;
            capabilities.GetSupportedEvents(out events);

            uint countEvents = 0;
            events.GetCount(ref countEvents);

            PortableDeviceApiLib.IPortableDeviceValues pValues = (PortableDeviceApiLib.IPortableDeviceValues)new PortableDeviceTypesLib.PortableDeviceValuesClass();
            tag_inner_PROPVARIANT evt = new tag_inner_PROPVARIANT();

            Guid eventName;
            IPortableDeviceValues eventOptions;
            PortableDeviceEventDescription eventDescription;

            for (uint i = 0; i < countEvents; i++)
            {
                events.GetAt(i, ref evt);
                pValues.SetValue(ref PortableDevicePKeys.WPD_EVENT_PARAMETER_EVENT_ID, ref evt);
                pValues.GetGuidValue(ref PortableDevicePKeys.WPD_EVENT_PARAMETER_EVENT_ID, out eventName);

                eventDescription = new PortableDeviceEventDescription(eventName, PortableDeviceHelpers.GetKeyNameFromGuid(eventName));

                //Retrieve options
                try
                { // Event option isn't always present, so ...
                    eventOptions = (PortableDeviceApiLib.IPortableDeviceValues)new PortableDeviceTypesLib.PortableDeviceValuesClass();
                    capabilities.GetEventOptions(ref eventName, out eventOptions);

                   

                    //eventOptions.GetBoolValue(ref PortableDevicePKeys.WPD_EVENT_OPTION_IS_AUTOPLAY_EVENT, out isAutoPlayEvent);
                    //eventOptions.GetBoolValue(ref PortableDevicePKeys.WPD_EVENT_OPTION_IS_BROADCAST_EVENT, out isBroadcastEvent);

                    //eventDescription.AddOptions(new PortableDeviceEventOption()
                    //{
                    //    Guid = PortableDevicePKeys.WPD_EVENT_OPTION_IS_BROADCAST_EVENT.fmtid,
                    //    Name = PortableDeviceHelpers.GetKeyNameFromGuid(PortableDevicePKeys.WPD_EVENT_OPTION_IS_BROADCAST_EVENT.fmtid),
                    //    Value = isBroadcastEvent,
                    //    ValueType = TypeCode.Boolean
                    //});

                    //eventDescription.AddOptions(new PortableDeviceEventOption()
                    //{
                    //    Guid = PortableDevicePKeys.WPD_EVENT_OPTION_IS_AUTOPLAY_EVENT.fmtid,
                    //    Name = PortableDeviceHelpers.GetKeyNameFromGuid(PortableDevicePKeys.WPD_EVENT_OPTION_IS_AUTOPLAY_EVENT.fmtid),
                    //    Value = isAutoPlayEvent,
                    //    ValueType = TypeCode.Boolean
                    //});
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    Debug.WriteLine(ex.StackTrace);
                }
                
                this.events.Add(eventDescription, PortableDevicePKeys.WPD_EVENT_PARAMETER_EVENT_ID);
            }
        }
    }
}
