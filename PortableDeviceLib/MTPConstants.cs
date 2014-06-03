#region License
/*
MTPConstants.cs
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
using System.Text;
using System.Runtime.InteropServices;

namespace PortableDeviceLib
{
    class MTPConstants
    {
        public static string DEVICE;
        public static string DEVICE_BATTERY_LEVEL;
        public static string DEVICE_FIRMWARE_VERSION;
        public static string DEVICE_FRIENDLY_NAME;
        public static string DEVICE_MANUFACTURER;
        public static string DEVICE_MODEL;
        public static string DEVICE_POWER_SOURCE;
        public static string DEVICE_SERIAL_NUMBER;
        public static string HISTORY_DIRECTORY;
        public static string MUSIC;
        public static string OBJECT_ALBUM;
        public static string OBJECT_ALBUM_ART;
        public static string OBJECT_ALBUM_ARTIST;
        public static string OBJECT_ARTIST;
        public static string OBJECT_BITRATE_TYPE;
        public static string OBJECT_CONTENT_TYPE;
        public static string OBJECT_DATE_AUTHORED;
        public static string OBJECT_DATE_CREATED;
        public static string OBJECT_DRM_PROTECTED;
        public static string OBJECT_DURATION;
        public static string OBJECT_EFFECTIVE_RATING;
        public static string OBJECT_FILENAME;
        public static string OBJECT_FORMAT;
        public static string OBJECT_GENRE;
        public static string OBJECT_ID;
        public static string OBJECT_LAST_ACCESSED_TIME;
        public static string OBJECT_LAST_BUILD_DATE;
        public static string OBJECT_MODIFIED;
        public static string OBJECT_NAME;
        public static string OBJECT_PERSISTENT_ID;
        public static string OBJECT_PLAY_COUNT;
        public static string OBJECT_RELEASE_DATE;
        public static string OBJECT_SAMPLE_RATE;
        public static string OBJECT_SIZE;
        public static string OBJECT_SKIP_COUNT;
        public static string OBJECT_STAR_RATING;
        public static string OBJECT_SUBSCRIPTION_CONTENT_ID;
        public static string OBJECT_TITLE;
        public static string OBJECT_TOTAL_BITRATE;
        public static string OBJECT_TRACK;
        public static string PLAYLIST_FOLDER;
        public static string PLAYLISTS;

        // Methods
        static MTPConstants()
        {
            DEVICE_FRIENDLY_NAME = "Device Friendly Name";
            DEVICE_MODEL = "Device Model";
            DEVICE_BATTERY_LEVEL = "Device Power Level";
            DEVICE_MANUFACTURER = "Device Manufacturer";
            DEVICE_FIRMWARE_VERSION = "Device Firmware Version";
            DEVICE_POWER_SOURCE = "Device Power Source";
            DEVICE_SERIAL_NUMBER = "Device Serial Number";
            OBJECT_ID = "Object ID";
            OBJECT_TITLE = "Title";
            OBJECT_ARTIST = "Artist";
            OBJECT_PLAY_COUNT = "Play Count";
            OBJECT_GENRE = "Genre";
            OBJECT_STAR_RATING = "Star Rating";
            OBJECT_ALBUM_ARTIST = "Album Artist";
            OBJECT_ALBUM = "Album";
            OBJECT_SUBSCRIPTION_CONTENT_ID = "Subscription Content ID";
            OBJECT_EFFECTIVE_RATING = "Effective Rating";
            OBJECT_TRACK = "Track";
            OBJECT_SIZE = "Object Size";
            OBJECT_DURATION = "Duration";
            OBJECT_CONTENT_TYPE = "Content Type";
            OBJECT_DATE_AUTHORED = "Date Authored";
            OBJECT_FORMAT = "Object Format";
            OBJECT_DRM_PROTECTED = "DRM Protected";
            OBJECT_FILENAME = "Filename";
            OBJECT_NAME = "Object Name";
            OBJECT_PERSISTENT_ID = "Persistent ID";
            OBJECT_TOTAL_BITRATE = "Total Bitrate";
            OBJECT_SKIP_COUNT = "Skip Count";
            OBJECT_SAMPLE_RATE = "Sample Rate";
            OBJECT_RELEASE_DATE = "Release Date";
            OBJECT_LAST_BUILD_DATE = "Last Build Date";
            OBJECT_LAST_ACCESSED_TIME = "Last Accessed Time";
            OBJECT_BITRATE_TYPE = "Bitrate Type";
            OBJECT_ALBUM_ART = "Album Art";
            OBJECT_DATE_CREATED = "Date Created";
            OBJECT_MODIFIED = "Modified";
        }

    }
}
