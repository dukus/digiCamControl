
//  Microsoft (R) .NET Framework IL Disassembler.  Version 3.5.30729.1
//  Copyright (c) Microsoft Corporation.  All rights reserved.



// Metadata version: v2.0.50727
.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 2:0:0:0
}
.assembly Interop.PortableDeviceApiLib
{
  .custom instance void [mscorlib]System.Runtime.InteropServices.ImportedFromTypeLibAttribute::.ctor(string) = ( 01 00 14 50 6F 72 74 61 62 6C 65 44 65 76 69 63   // ...PortableDevic
                                                                                                                 65 41 70 69 4C 69 62 00 00 )                      // eApiLib..
  .custom instance void [mscorlib]System.Runtime.InteropServices.TypeLibVersionAttribute::.ctor(int32,
                                                                                                int32) = ( 01 00 01 00 00 00 00 00 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.InteropServices.GuidAttribute::.ctor(string) = ( 01 00 24 31 66 30 30 31 33 33 32 2D 31 61 35 37   // ..$1f001332-1a57
                                                                                                  2D 34 39 33 34 2D 62 65 33 31 2D 61 66 66 63 39   // -4934-be31-affc9
                                                                                                  39 66 34 65 65 30 61 00 00 )                      // 9f4ee0a..
  .hash algorithm 0x00008004
  .ver 1:0:0:0
}
.module PortableDeviceApiLib.dll
// MVID: {4EC336A7-C20B-4603-86CC-4EEDAE6D5AB4}
.imagebase 0x00400000
.file alignment 0x00001000
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x00580000


// =============== CLASS MEMBERS DECLARATION ===================

.class public auto ansi import PortableDeviceApiLib.PortableDeviceClass
       extends [mscorlib]System.Object
       implements PortableDeviceApiLib.IPortableDevice,
                  PortableDeviceApiLib.PortableDevice
{
  .custom instance void [mscorlib]System.Runtime.InteropServices.ClassInterfaceAttribute::.ctor(int16) = ( 01 00 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.InteropServices.TypeLibTypeAttribute::.ctor(int16) = ( 01 00 02 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.InteropServices.GuidAttribute::.ctor(string) = ( 01 00 24 37 32 38 41 32 31 43 35 2D 33 44 39 45   // ..$728A21C5-3D9E
                                                                                                  2D 34 38 44 37 2D 39 38 31 30 2D 38 36 34 38 34   // -48D7-9810-86484
                                                                                                  38 46 30 46 34 30 34 00 00 )                      // 8F0F404..
  .method public specialname rtspecialname 
          instance void  .ctor() runtime managed internalcall
  {
  } // end of method PortableDeviceClass::.ctor

  .method public hidebysig newslot virtual 
          instance void  Open([in] string  marshal( lpwstr) pszPnPDeviceID,
                              [in] class PortableDeviceApiLib.IPortableDeviceValues  marshal( interface ) pClientInfo) runtime managed internalcall
  {
    .override PortableDeviceApiLib.IPortableDevice::Open
  } // end of method PortableDeviceClass::Open

  .method public hidebysig newslot virtual 
          instance void  SendCommand([in] uint32 dwFlags,
                                     [in] class PortableDeviceApiLib.IPortableDeviceValues  marshal( interface ) pParameters,
                                     [out] class PortableDeviceApiLib.IPortableDeviceValues&  marshal( interface ) ppResults) runtime managed internalcall
  {
    .override PortableDeviceApiLib.IPortableDevice::SendCommand
  } // end of method PortableDeviceClass::SendCommand

  .method public hidebysig newslot virtual 
          instance void  Content([out] class PortableDeviceApiLib.IPortableDeviceContent&  marshal( interface ) ppContent) runtime managed internalcall
  {
    .override PortableDeviceApiLib.IPortableDevice::Content
  } // end of method PortableDeviceClass::Content

  .method public hidebysig newslot virtual 
          instance void  Capabilities([out] class PortableDeviceApiLib.IPortableDeviceCapabilities&  marshal( interface ) ppCapabilities) runtime managed internalcall
  {
    .override PortableDeviceApiLib.IPortableDevice::Capabilities
  } // end of method PortableDeviceClass::Capabilities

  .method public hidebysig newslot virtual 
          instance void  Cancel() runtime managed internalcall
  {
    .override PortableDeviceApiLib.IPortableDevice::Cancel
  } // end of method PortableDeviceClass::Cancel

  .method public hidebysig newslot virtual 
          instance void  Close() runtime managed internalcall
  {
    .override PortableDeviceApiLib.IPortableDevice::Close
  } // end of method PortableDeviceClass::Close

  .method public hidebysig newslot virtual 
          instance void  Advise([in] uint32 dwFlags,
                                [in] class PortableDeviceApiLib.IPortableDeviceEventCallback  marshal( interface ) pCallback,
                                [in] class PortableDeviceApiLib.IPortableDeviceValues  marshal( interface ) pParameters,
                                [out] string&  marshal( lpwstr) ppszCookie) runtime managed internalcall
  {
    .override PortableDeviceApiLib.IPortableDevice::Advise
  } // end of method PortableDeviceClass::Advise

  .method public hidebysig newslot virtual 
          instance void  Unadvise([in] string  marshal( lpwstr) pszCookie) runtime managed internalcall
  {
    .override PortableDeviceApiLib.IPortableDevice::Unadvise
  } // end of method PortableDeviceClass::Unadvise

  .method public hidebysig newslot virtual 
          instance void  GetPnPDeviceID([out] string&  marshal( lpwstr) ppszPnPDeviceID) runtime managed internalcall
  {
    .override PortableDeviceApiLib.IPortableDevice::GetPnPDeviceID
  } // end of method PortableDeviceClass::GetPnPDeviceID

} // end of class PortableDeviceApiLib.PortableDeviceClass

.class interface public abstract auto ansi import PortableDeviceApiLib.PortableDevice
       implements PortableDeviceApiLib.IPortableDevice
{
  .custom instance void [mscorlib]System.Runtime.InteropServices.CoClassAttribute::.ctor(class [mscorlib]System.Type) = ( 01 00 28 50 6F 72 74 61 62 6C 65 44 65 76 69 63   // ..(PortableDevic
                                                                                                                          65 41 70 69 4C 69 62 2E 50 6F 72 74 61 62 6C 65   // eApiLib.Portable
                                                                                                                          44 65 76 69 63 65 43 6C 61 73 73 00 00 )          // DeviceClass..
  .custom instance void [mscorlib]System.Runtime.InteropServices.GuidAttribute::.ctor(string) = ( 01 00 24 36 32 35 45 32 44 46 38 2D 36 33 39 32   // ..$625E2DF8-6392
                                                                                                  2D 34 43 46 30 2D 39 41 44 31 2D 33 43 46 41 35   // -4CF0-9AD1-3CFA5
                                                                                                  46 31 37 37 37 35 43 00 00 )                      // F17775C..
} // end of class PortableDeviceApiLib.PortableDevice

.class interface public abstract auto ansi import PortableDeviceApiLib.IPortableDevice
{
  .custom instance void [mscorlib]System.Runtime.InteropServices.InterfaceTypeAttribute::.ctor(int16) = ( 01 00 01 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.InteropServices.GuidAttribute::.ctor(string) = ( 01 00 24 36 32 35 45 32 44 46 38 2D 36 33 39 32   // ..$625E2DF8-6392
                                                                                                  2D 34 43 46 30 2D 39 41 44 31 2D 33 43 46 41 35   // -4CF0-9AD1-3CFA5
                                                                                                  46 31 37 37 37 35 43 00 00 )                      // F17775C..
  .method public hidebysig newslot abstract virtual 
          instance void  Open([in] string  marshal( lpwstr) pszPnPDeviceID,
                              [in] class PortableDeviceApiLib.IPortableDeviceValues  marshal( interface ) pClientInfo) runtime managed internalcall
  {
  } // end of method IPortableDevice::Open

  .method public hidebysig newslot abstract virtual 
          instance void  SendCommand([in] uint32 dwFlags,
                                     [in] class PortableDeviceApiLib.IPortableDeviceValues  marshal( interface ) pParameters,
                                     [out] class PortableDeviceApiLib.IPortableDeviceValues&  marshal( interface ) ppResults) runtime managed internalcall
  {
  } // end of method IPortableDevice::SendCommand

  .method public hidebysig newslot abstract virtual 
          instance void  Content([out] class PortableDeviceApiLib.IPortableDeviceContent&  marshal( interface ) ppContent) runtime managed internalcall
  {
  } // end of method IPortableDevice::Content

  .method public hidebysig newslot abstract virtual 
          instance void  Capabilities([out] class PortableDeviceApiLib.IPortableDeviceCapabilities&  marshal( interface ) ppCapabilities) runtime managed internalcall
  {
  } // end of method IPortableDevice::Capabilities

  .method public hidebysig newslot abstract virtual 
          instance void  Cancel() runtime managed internalcall
  {
  } // end of method IPortableDevice::Cancel

  .method public hidebysig newslot abstract virtual 
          instance void  Close() runtime managed internalcall
  {
  } // end of method IPortableDevice::Close

  .method public hidebysig newslot abstract virtual 
          instance void  Advise([in] uint32 dwFlags,
                                [in] class PortableDeviceApiLib.IPortableDeviceEventCallback  marshal( interface ) pCallback,
                                [in] class PortableDeviceApiLib.IPortableDeviceValues  marshal( interface ) pParameters,
                                [out] string&  marshal( lpwstr) ppszCookie) runtime managed internalcall
  {
  } // end of method IPortableDevice::Advise

  .method public hidebysig newslot abstract virtual 
          instance void  Unadvise([in] string  marshal( lpwstr) pszCookie) runtime managed internalcall
  {
  } // end of method IPortableDevice::Unadvise

  .method public hidebysig newslot abstract virtual 
          instance void  GetPnPDeviceID([out] string&  marshal( lpwstr) ppszPnPDeviceID) runtime managed internalcall
  {
  } // end of method IPortableDevice::GetPnPDeviceID

} // end of class PortableDeviceApiLib.IPortableDevice

.class interface public abstract auto ansi import PortableDeviceApiLib.IPortableDeviceValues
{
  .custom instance void [mscorlib]System.Runtime.InteropServices.GuidAttribute::.ctor(string) = ( 01 00 24 36 38 34 38 46 36 46 32 2D 33 31 35 35   // ..$6848F6F2-3155
                                                                                                  2D 34 46 38 36 2D 42 36 46 35 2D 32 36 33 45 45   // -4F86-B6F5-263EE
                                                                                                  45 41 42 33 31 34 33 00 00 )                      // EAB3143..
  .custom instance void [mscorlib]System.Runtime.InteropServices.ComConversionLossAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.InteropServices.InterfaceTypeAttribute::.ctor(int16) = ( 01 00 01 00 00 00 ) 
  .method public hidebysig newslot abstract virtual 
          instance void  GetCount([in] uint32& pcelt) runtime managed internalcall
  {
  } // end of method IPortableDeviceValues::GetCount

  .method public hidebysig newslot abstract virtual 
          instance void  GetAt([in] uint32 index,
                               [in][out] valuetype PortableDeviceApiLib._tagpropertykey& pKey,
                               [in][out] valuetype PortableDeviceApiLib.tag_inner_PROPVARIANT& pValue) runtime managed internalcall
  {
  } // end of method IPortableDeviceValues::GetAt

  .method public hidebysig newslot abstract virtual 
          instance void  SetValue([in] valuetype PortableDeviceApiLib._tagpropertykey& key,
                                  [in] valuetype PortableDeviceApiLib.tag_inner_PROPVARIANT& pValue) runtime managed internalcall
  {
  } // end of method IPortableDeviceValues::SetValue

  .method public hidebysig newslot abstract virtual 
          instance void  GetValue([in] valuetype PortableDeviceApiLib._tagpropertykey& key,
                                  [out] valuetype PortableDeviceApiLib.tag_inner_PROPVARIANT& pValue) runtime managed internalcall
  {
  } // end of method IPortableDeviceValues::GetValue

  .method public hidebysig newslot abstract virtual 
          instance void  SetStringValue([in] valuetype PortableDeviceApiLib._tagpropertykey& key,
                                        [in] string  marshal( lpwstr) Value) runtime managed internalcall
  {
  } // end of method IPortableDeviceValues::SetStringValue

  .method public hidebysig newslot abstract virtual 
          instance void  GetStringValue([in] valuetype PortableDeviceApiLib._tagpropertykey& key,
                                        [out] string&  marshal( lpwstr) pValue) runtime managed internalcall
  {
  } // end of method IPortableDeviceValues::GetStringValue

  .method public hidebysig newslot abstract virtual 
          instance void  SetUnsignedIntegerValue([in] valuetype PortableDeviceApiLib._tagpropertykey& key,
                                                 [in] uint32 Value) runtime managed internalcall
  {
  } // end of method IPortableDeviceValues::SetUnsignedIntegerValue

  .method public hidebysig newslot abstract virtual 
          instance void  GetUnsignedIntegerValue([in] valuetype PortableDeviceApiLib._tagpropertykey& key,
                                                 [out] uint32& pValue) runtime managed internalcall
  {
  } // end of method IPortableDeviceValues::GetUnsignedIntegerValue

  .method public hidebysig newslot abstract virtual 
          instance void  SetSignedIntegerValue([in] valuetype PortableDeviceApiLib._tagpropertykey& key,
                                               [in] int32 Value) runtime managed internalcall
  {
  } // end of method IPortableDeviceValues::SetSignedIntegerValue

  .method public hidebysig newslot abstract virtual 
          instance void  GetSignedIntegerValue([in] valuetype PortableDeviceApiLib._tagpropertykey& key,
                                               [out] int32& pValue) runtime managed internalcall
  {
  } // end of method IPortableDeviceValues::GetSignedIntegerValue

  .method public hidebysig newslot abstract virtual 
          instance void  SetUnsignedLargeIntegerValue([in] valuetype PortableDeviceApiLib._tagpropertykey& key,
                                                      [in] uint64 Value) runtime managed internalcall
  {
  } // end of method IPortableDeviceValues::SetUnsignedLargeIntegerValue

  .method public hidebysig newslot abstract virtual 
          instance void  GetUnsignedLargeIntegerValue([in] valuetype PortableDeviceApiLib._tagpropertykey& key,
                                                      [out] uint64& pValue) runtime managed internalcall
  {
  } // end of method IPortableDeviceValues::GetUnsignedLargeIntegerValue

  .method public hidebysig newslot abstract virtual 
          instance void  SetSignedLargeIntegerValue([in] valuetype PortableDeviceApiLib._tagpropertykey& key,
                                                    [in] int64 Value) runtime managed internalcall
  {
  } // end of method IPortableDeviceValues::SetSignedLargeIntegerValue

  .method public hidebysig newslot abstract virtual 
          instance void  GetSignedLargeIntegerValue([in] valuetype PortableDeviceApiLib._tagpropertykey& key,
                                                    [out] int64& pValue) runtime managed internalcall
  {
  } // end of method IPortableDeviceValues::GetSignedLargeIntegerValue

  .method public hidebysig newslot abstract virtual 
          instance void  SetFloatValue([in] valuetype PortableDeviceApiLib._tagpropertykey& key,
                                       [in] float32 Value) runtime managed internalcall
  {
  } // end of method IPortableDeviceValues::SetFloatValue

  .method public hidebysig newslot abstract virtual 
          instance void  GetFloatValue([in] valuetype PortableDeviceApiLib._tagpropertykey& key,
                                       [out] float32& pValue) runtime managed internalcall
  {
  } // end of method IPortableDeviceValues::GetFloatValue

  .method public hidebysig newslot abstract virtual 
          instance void  SetErrorValue([in] valuetype PortableDeviceApiLib._tagpropertykey& key,
                                       [in] int32  marshal( error) Value) runtime managed internalcall
  {
  } // end of method IPortableDeviceValues::SetErrorValue

  .method public hidebysig newslot abstract virtual 
          instance void  GetErrorValue([in] valuetype PortableDeviceApiLib._tagpropertykey& key,
                                       [out] int32&  marshal( error) pValue) runtime managed internalcall
  {
  } // end of method IPortableDeviceValues::GetErrorValue

  .method public hidebysig newslot abstract virtual 
          instance void  SetKeyValue([in] valuetype PortableDeviceApiLib._tagpropertykey& key,
                                     [in] valuetype PortableDeviceApiLib._tagpropertykey& Value) runtime managed internalcall
  {
  } // end of method IPortableDeviceValues::SetKeyValue

  .method public hidebysig newslot abstract virtual 
          instance void  GetKeyValue([in] valuetype PortableDeviceApiLib._tagpropertykey& key,
                                     [out] valuetype PortableDeviceApiLib._tagpropertykey& pValue) runtime managed internalcall
  {
  } // end of method IPortableDeviceValues::GetKeyValue

  .method public hidebysig newslot abstract virtual 
          instance void  SetBoolValue([in] valuetype PortableDeviceApiLib._tagpropertykey& key,
                                      [in] int32 Value) runtime managed internalcall
  {
  } // end of method IPortableDeviceValues::SetBoolValue

  .method public hidebysig newslot abstract virtual 
          instance void  GetBoolValue([in] valuetype PortableDeviceApiLib._tagpropertykey& key,
                                      [out] int32& pValue) runtime managed internalcall
  {
  } // end of method IPortableDeviceValues::GetBoolValue

  .method public hidebysig newslot abstract virtual 
          instance void  SetIUnknownValue([in] valuetype PortableDeviceApiLib._tagpropertykey& key,
                                          [in] object  marshal( iunknown ) pValue) runtime managed internalcall
  {
  } // end of method IPortableDeviceValues::SetIUnknownValue

  .method public hidebysig newslot abstract virtual 
          instance void  GetIUnknownValue([in] valuetype PortableDeviceApiLib._tagpropertykey& key,
                                          [out] object&  marshal( iunknown ) ppValue) runtime managed internalcall
  {
  } // end of method IPortableDeviceValues::GetIUnknownValue

  .method public hidebysig newslot abstract virtual 
          instance void  SetGuidValue([in] valuetype PortableDeviceApiLib._tagpropertykey& key,
                                      [in] valuetype [mscorlib]System.Guid& Value) runtime managed internalcall
  {
  } // end of method IPortableDeviceValues::SetGuidValue

  .method public hidebysig newslot abstract virtual 
          instance void  GetGuidValue([in] valuetype PortableDeviceApiLib._tagpropertykey& key,
                                      [out] valuetype [mscorlib]System.Guid& pValue) runtime managed internalcall
  {
  } // end of method IPortableDeviceValues::GetGuidValue

  .method public hidebysig newslot abstract virtual 
          instance void  SetBufferValue([in] valuetype PortableDeviceApiLib._tagpropertykey& key,
                                        [in] uint8& pValue,
                                        [in] uint32 cbValue) runtime managed internalcall
  {
  } // end of method IPortableDeviceValues::SetBufferValue

  .method public hidebysig newslot abstract virtual 
          instance void  GetBufferValue([in] valuetype PortableDeviceApiLib._tagpropertykey& key,
                                        [out] native int ppValue,
                                        [out] uint32& pcbValue) runtime managed internalcall
  {
  } // end of method IPortableDeviceValues::GetBufferValue

  .method public hidebysig newslot abstract virtual 
          instance void  SetIPortableDeviceValuesValue([in] valuetype PortableDeviceApiLib._tagpropertykey& key,
                                                       [in] class PortableDeviceApiLib.IPortableDeviceValues  marshal( interface ) pValue) runtime managed internalcall
  {
  } // end of method IPortableDeviceValues::SetIPortableDeviceValuesValue

  .method public hidebysig newslot abstract virtual 
          instance void  GetIPortableDeviceValuesValue([in] valuetype PortableDeviceApiLib._tagpropertykey& key,
                                                       [out] class PortableDeviceApiLib.IPortableDeviceValues&  marshal( interface ) ppValue) runtime managed internalcall
  {
  } // end of method IPortableDeviceValues::GetIPortableDeviceValuesValue

  .method public hidebysig newslot abstract virtual 
          instance void  SetIPortableDevicePropVariantCollectionValue([in] valuetype PortableDeviceApiLib._tagpropertykey& key,
                                                                      [in] class PortableDeviceApiLib.IPortableDevicePropVariantCollection  marshal( interface ) pValue) runtime managed internalcall
  {
  } // end of method IPortableDeviceValues::SetIPortableDevicePropVariantCollectionValue

  .method public hidebysig newslot abstract virtual 
          instance void  GetIPortableDevicePropVariantCollectionValue([in] valuetype PortableDeviceApiLib._tagpropertykey& key,
                                                                      [out] class PortableDeviceApiLib.IPortableDevicePropVariantCollection&  marshal( interface ) ppValue) runtime managed internalcall
  {
  } // end of method IPortableDeviceValues::GetIPortableDevicePropVariantCollectionValue

  .method public hidebysig newslot abstract virtual 
          instance void  SetIPortableDeviceKeyCollectionValue([in] valuetype PortableDeviceApiLib._tagpropertykey& key,
                                                              [in] class PortableDeviceApiLib.IPortableDeviceKeyCollection  marshal( interface ) pValue) runtime managed internalcall
  {
  } // end of method IPortableDeviceValues::SetIPortableDeviceKeyCollectionValue

  .method public hidebysig newslot abstract virtual 
          instance void  GetIPortableDeviceKeyCollectionValue([in] valuetype PortableDeviceApiLib._tagpropertykey& key,
                                                              [out] class PortableDeviceApiLib.IPortableDeviceKeyCollection&  marshal( interface ) ppValue) runtime managed internalcall
  {
  } // end of method IPortableDeviceValues::GetIPortableDeviceKeyCollectionValue

  .method public hidebysig newslot abstract virtual 
          instance void  SetIPortableDeviceValuesCollectionValue([in] valuetype PortableDeviceApiLib._tagpropertykey& key,
                                                                 [in] class PortableDeviceApiLib.IPortableDeviceValuesCollection  marshal( interface ) pValue) runtime managed internalcall
  {
  } // end of method IPortableDeviceValues::SetIPortableDeviceValuesCollectionValue

  .method public hidebysig newslot abstract virtual 
          instance void  GetIPortableDeviceValuesCollectionValue([in] valuetype PortableDeviceApiLib._tagpropertykey& key,
                                                                 [out] class PortableDeviceApiLib.IPortableDeviceValuesCollection&  marshal( interface ) ppValue) runtime managed internalcall
  {
  } // end of method IPortableDeviceValues::GetIPortableDeviceValuesCollectionValue

  .method public hidebysig newslot abstract virtual 
          instance void  RemoveValue([in] valuetype PortableDeviceApiLib._tagpropertykey& key) runtime managed internalcall
  {
  } // end of method IPortableDeviceValues::RemoveValue

  .method public hidebysig newslot abstract virtual 
          instance void  CopyValuesFromPropertyStore([in] class PortableDeviceApiLib.IPropertyStore  marshal( interface ) pStore) runtime managed internalcall
  {
  } // end of method IPortableDeviceValues::CopyValuesFromPropertyStore

  .method public hidebysig newslot abstract virtual 
          instance void  CopyValuesToPropertyStore([in] class PortableDeviceApiLib.IPropertyStore  marshal( interface ) pStore) runtime managed internalcall
  {
  } // end of method IPortableDeviceValues::CopyValuesToPropertyStore

  .method public hidebysig newslot abstract virtual 
          instance void  Clear() runtime managed internalcall
  {
  } // end of method IPortableDeviceValues::Clear

} // end of class PortableDeviceApiLib.IPortableDeviceValues

.class public sequential ansi sealed beforefieldinit PortableDeviceApiLib._tagpropertykey
       extends [mscorlib]System.ValueType
{
  .pack 4
  .size 0
  .field public valuetype [mscorlib]System.Guid fmtid
  .field public uint32 pid
} // end of class PortableDeviceApiLib._tagpropertykey

.class public sequential ansi sealed beforefieldinit PortableDeviceApiLib.tag_inner_PROPVARIANT
       extends [mscorlib]System.ValueType
{
  .pack 8
  .size 0
  .field public uint16 vt
  .field public uint8 wReserved1
  .field public uint8 wReserved2
  .field public uint32 wReserved3
  .field public valuetype PortableDeviceApiLib.__MIDL___MIDL_itf_PortableDeviceApi_0001_0000_0001 __MIDL____MIDL_itf_PortableDeviceApi_0001_00000001
} // end of class PortableDeviceApiLib.tag_inner_PROPVARIANT

.class public explicit ansi sealed PortableDeviceApiLib.__MIDL___MIDL_itf_PortableDeviceApi_0001_0000_0001
       extends [mscorlib]System.ValueType
{
  .pack 8
  .size 8
} // end of class PortableDeviceApiLib.__MIDL___MIDL_itf_PortableDeviceApi_0001_0000_0001

.class public sequential ansi sealed beforefieldinit PortableDeviceApiLib._LARGE_INTEGER
       extends [mscorlib]System.ValueType
{
  .pack 8
  .size 0
  .field public int64 QuadPart
} // end of class PortableDeviceApiLib._LARGE_INTEGER

.class public sequential ansi sealed beforefieldinit PortableDeviceApiLib._ULARGE_INTEGER
       extends [mscorlib]System.ValueType
{
  .pack 8
  .size 0
  .field public uint64 QuadPart
} // end of class PortableDeviceApiLib._ULARGE_INTEGER

.class public sequential ansi sealed beforefieldinit PortableDeviceApiLib._FILETIME
       extends [mscorlib]System.ValueType
{
  .pack 4
  .size 0
  .field public uint32 dwLowDateTime
  .field public uint32 dwHighDateTime
} // end of class PortableDeviceApiLib._FILETIME

.class public sequential ansi sealed beforefieldinit PortableDeviceApiLib.tagCLIPDATA
       extends [mscorlib]System.ValueType
{
  .pack 4
  .size 0
  .custom instance void [mscorlib]System.Runtime.InteropServices.ComConversionLossAttribute::.ctor() = ( 01 00 00 00 ) 
  .field public uint32 cbSize
  .field public int32 ulClipFmt
  .field public native int pClipData
  .custom instance void [mscorlib]System.Runtime.InteropServices.ComConversionLossAttribute::.ctor() = ( 01 00 00 00 ) 
} // end of class PortableDeviceApiLib.tagCLIPDATA

.class public sequential ansi sealed beforefieldinit PortableDeviceApiLib.tagBSTRBLOB
       extends [mscorlib]System.ValueType
{
  .pack 4
  .size 0
  .custom instance void [mscorlib]System.Runtime.InteropServices.ComConversionLossAttribute::.ctor() = ( 01 00 00 00 ) 
  .field public uint32 cbSize
  .field public native int pData
  .custom instance void [mscorlib]System.Runtime.InteropServices.ComConversionLossAttribute::.ctor() = ( 01 00 00 00 ) 
} // end of class PortableDeviceApiLib.tagBSTRBLOB

.class public sequential ansi sealed beforefieldinit PortableDeviceApiLib.tagBLOB
       extends [mscorlib]System.ValueType
{
  .pack 4
  .size 0
  .custom instance void [mscorlib]System.Runtime.InteropServices.ComConversionLossAttribute::.ctor() = ( 01 00 00 00 ) 
  .field public uint32 cbSize
  .field public native int pBlobData
  .custom instance void [mscorlib]System.Runtime.InteropServices.ComConversionLossAttribute::.ctor() = ( 01 00 00 00 ) 
} // end of class PortableDeviceApiLib.tagBLOB

.class interface public abstract auto ansi import PortableDeviceApiLib.IStream
       implements PortableDeviceApiLib.ISequentialStream
{
  .custom instance void [mscorlib]System.Runtime.InteropServices.InterfaceTypeAttribute::.ctor(int16) = ( 01 00 01 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.InteropServices.GuidAttribute::.ctor(string) = ( 01 00 24 30 30 30 30 30 30 30 43 2D 30 30 30 30   // ..$0000000C-0000
                                                                                                  2D 30 30 30 30 2D 43 30 30 30 2D 30 30 30 30 30   // -0000-C000-00000
                                                                                                  30 30 30 30 30 34 36 00 00 )                      // 0000046..
  .method public hidebysig newslot abstract virtual 
          instance void  RemoteRead([out] uint8& pv,
                                    [in] uint32 cb,
                                    [out] uint32& pcbRead) runtime managed internalcall
  {
  } // end of method IStream::RemoteRead

  .method public hidebysig newslot abstract virtual 
          instance void  RemoteWrite([in] uint8& pv,
                                     [in] uint32 cb,
                                     [out] uint32& pcbWritten) runtime managed internalcall
  {
  } // end of method IStream::RemoteWrite

  .method public hidebysig newslot abstract virtual 
          instance void  RemoteSeek([in] valuetype PortableDeviceApiLib._LARGE_INTEGER dlibMove,
                                    [in] uint32 dwOrigin,
                                    [out] valuetype PortableDeviceApiLib._ULARGE_INTEGER& plibNewPosition) runtime managed internalcall
  {
  } // end of method IStream::RemoteSeek

  .method public hidebysig newslot abstract virtual 
          instance void  SetSize([in] valuetype PortableDeviceApiLib._ULARGE_INTEGER libNewSize) runtime managed internalcall
  {
  } // end of method IStream::SetSize

  .method public hidebysig newslot abstract virtual 
          instance void  RemoteCopyTo([in] class PortableDeviceApiLib.IStream  marshal( interface ) pstm,
                                      [in] valuetype PortableDeviceApiLib._ULARGE_INTEGER cb,
                                      [out] valuetype PortableDeviceApiLib._ULARGE_INTEGER& pcbRead,
                                      [out] valuetype PortableDeviceApiLib._ULARGE_INTEGER& pcbWritten) runtime managed internalcall
  {
  } // end of method IStream::RemoteCopyTo

  .method public hidebysig newslot abstract virtual 
          instance void  Commit([in] uint32 grfCommitFlags) runtime managed internalcall
  {
  } // end of method IStream::Commit

  .method public hidebysig newslot abstract virtual 
          instance void  Revert() runtime managed internalcall
  {
  } // end of method IStream::Revert

  .method public hidebysig newslot abstract virtual 
          instance void  LockRegion([in] valuetype PortableDeviceApiLib._ULARGE_INTEGER libOffset,
                                    [in] valuetype PortableDeviceApiLib._ULARGE_INTEGER cb,
                                    [in] uint32 dwLockType) runtime managed internalcall
  {
  } // end of method IStream::LockRegion

  .method public hidebysig newslot abstract virtual 
          instance void  UnlockRegion([in] valuetype PortableDeviceApiLib._ULARGE_INTEGER libOffset,
                                      [in] valuetype PortableDeviceApiLib._ULARGE_INTEGER cb,
                                      [in] uint32 dwLockType) runtime managed internalcall
  {
  } // end of method IStream::UnlockRegion

  .method public hidebysig newslot abstract virtual 
          instance void  Stat([out] valuetype PortableDeviceApiLib.tagSTATSTG& pstatstg,
                              [in] uint32 grfStatFlag) runtime managed internalcall
  {
  } // end of method IStream::Stat

  .method public hidebysig newslot abstract virtual 
          instance void  Clone([out] class PortableDeviceApiLib.IStream&  marshal( interface ) ppstm) runtime managed internalcall
  {
  } // end of method IStream::Clone

} // end of class PortableDeviceApiLib.IStream

.class interface public abstract auto ansi import PortableDeviceApiLib.ISequentialStream
{
  .custom instance void [mscorlib]System.Runtime.InteropServices.GuidAttribute::.ctor(string) = ( 01 00 24 30 43 37 33 33 41 33 30 2D 32 41 31 43   // ..$0C733A30-2A1C
                                                                                                  2D 31 31 43 45 2D 41 44 45 35 2D 30 30 41 41 30   // -11CE-ADE5-00AA0
                                                                                                  30 34 34 37 37 33 44 00 00 )                      // 044773D..
  .custom instance void [mscorlib]System.Runtime.InteropServices.InterfaceTypeAttribute::.ctor(int16) = ( 01 00 01 00 00 00 ) 
  .method public hidebysig newslot abstract virtual 
          instance void  RemoteRead([out] uint8& pv,
                                    [in] uint32 cb,
                                    [out] uint32& pcbRead) runtime managed internalcall
  {
  } // end of method ISequentialStream::RemoteRead

  .method public hidebysig newslot abstract virtual 
          instance void  RemoteWrite([in] uint8& pv,
                                     [in] uint32 cb,
                                     [out] uint32& pcbWritten) runtime managed internalcall
  {
  } // end of method ISequentialStream::RemoteWrite

} // end of class PortableDeviceApiLib.ISequentialStream

.class public sequential ansi sealed beforefieldinit PortableDeviceApiLib.tagSTATSTG
       extends [mscorlib]System.ValueType
{
  .pack 8
  .size 0
  .field public  marshal( lpwstr) string pwcsName
  .field public uint32 'type'
  .field public valuetype PortableDeviceApiLib._ULARGE_INTEGER cbSize
  .field public valuetype PortableDeviceApiLib._FILETIME mtime
  .field public valuetype PortableDeviceApiLib._FILETIME ctime
  .field public valuetype PortableDeviceApiLib._FILETIME atime
  .field public uint32 grfMode
  .field public uint32 grfLocksSupported
  .field public valuetype [mscorlib]System.Guid 'clsid'
  .field public uint32 grfStateBits
  .field public uint32 reserved
} // end of class PortableDeviceApiLib.tagSTATSTG

.class interface public abstract auto ansi import PortableDeviceApiLib.IStorage
{
  .custom instance void [mscorlib]System.Runtime.InteropServices.GuidAttribute::.ctor(string) = ( 01 00 24 30 30 30 30 30 30 30 42 2D 30 30 30 30   // ..$0000000B-0000
                                                                                                  2D 30 30 30 30 2D 43 30 30 30 2D 30 30 30 30 30   // -0000-C000-00000
                                                                                                  30 30 30 30 30 34 36 00 00 )                      // 0000046..
  .custom instance void [mscorlib]System.Runtime.InteropServices.InterfaceTypeAttribute::.ctor(int16) = ( 01 00 01 00 00 00 ) 
  .method public hidebysig newslot abstract virtual 
          instance void  CreateStream([in] string  marshal( lpwstr) pwcsName,
                                      [in] uint32 grfMode,
                                      [in] uint32 reserved1,
                                      [in] uint32 reserved2,
                                      [out] class PortableDeviceApiLib.IStream&  marshal( interface ) ppstm) runtime managed internalcall
  {
  } // end of method IStorage::CreateStream

  .method public hidebysig newslot abstract virtual 
          instance void  RemoteOpenStream([in] string  marshal( lpwstr) pwcsName,
                                          [in] uint32 cbReserved1,
                                          [in] uint8& reserved1,
                                          [in] uint32 grfMode,
                                          [in] uint32 reserved2,
                                          [out] class PortableDeviceApiLib.IStream&  marshal( interface ) ppstm) runtime managed internalcall
  {
  } // end of method IStorage::RemoteOpenStream

  .method public hidebysig newslot abstract virtual 
          instance void  CreateStorage([in] string  marshal( lpwstr) pwcsName,
                                       [in] uint32 grfMode,
                                       [in] uint32 reserved1,
                                       [in] uint32 reserved2,
                                       [out] class PortableDeviceApiLib.IStorage&  marshal( interface ) ppstg) runtime managed internalcall
  {
  } // end of method IStorage::CreateStorage

  .method public hidebysig newslot abstract virtual 
          instance void  OpenStorage([in] string  marshal( lpwstr) pwcsName,
                                     [in] class PortableDeviceApiLib.IStorage  marshal( interface ) pstgPriority,
                                     [in] uint32 grfMode,
                                     [in] valuetype PortableDeviceApiLib.tagRemSNB& snbExclude,
                                     [in] uint32 reserved,
                                     [out] class PortableDeviceApiLib.IStorage&  marshal( interface ) ppstg) runtime managed internalcall
  {
    .param [4]
    .custom instance void [mscorlib]System.Runtime.InteropServices.ComAliasNameAttribute::.ctor(string) = ( 01 00 1C 50 6F 72 74 61 62 6C 65 44 65 76 69 63   // ...PortableDevic
                                                                                                            65 41 70 69 4C 69 62 2E 77 69 72 65 53 4E 42 00   // eApiLib.wireSNB.
                                                                                                            00 ) 
  } // end of method IStorage::OpenStorage

  .method public hidebysig newslot abstract virtual 
          instance void  RemoteCopyTo([in] uint32 ciidExclude,
                                      [in] valuetype [mscorlib]System.Guid& rgiidExclude,
                                      [in] valuetype PortableDeviceApiLib.tagRemSNB& snbExclude,
                                      [in] class PortableDeviceApiLib.IStorage  marshal( interface ) pstgDest) runtime managed internalcall
  {
    .param [3]
    .custom instance void [mscorlib]System.Runtime.InteropServices.ComAliasNameAttribute::.ctor(string) = ( 01 00 1C 50 6F 72 74 61 62 6C 65 44 65 76 69 63   // ...PortableDevic
                                                                                                            65 41 70 69 4C 69 62 2E 77 69 72 65 53 4E 42 00   // eApiLib.wireSNB.
                                                                                                            00 ) 
  } // end of method IStorage::RemoteCopyTo

  .method public hidebysig newslot abstract virtual 
          instance void  MoveElementTo([in] string  marshal( lpwstr) pwcsName,
                                       [in] class PortableDeviceApiLib.IStorage  marshal( interface ) pstgDest,
                                       [in] string  marshal( lpwstr) pwcsNewName,
                                       [in] uint32 grfFlags) runtime managed internalcall
  {
  } // end of method IStorage::MoveElementTo

  .method public hidebysig newslot abstract virtual 
          instance void  Commit([in] uint32 grfCommitFlags) runtime managed internalcall
  {
  } // end of method IStorage::Commit

  .method public hidebysig newslot abstract virtual 
          instance void  Revert() runtime managed internalcall
  {
  } // end of method IStorage::Revert

  .method public hidebysig newslot abstract virtual 
          instance void  RemoteEnumElements([in] uint32 reserved1,
                                            [in] uint32 cbReserved2,
                                            [in] uint8& reserved2,
                                            [in] uint32 reserved3,
                                            [out] class PortableDeviceApiLib.IEnumSTATSTG&  marshal( interface ) ppenum) runtime managed internalcall
  {
  } // end of method IStorage::RemoteEnumElements

  .method public hidebysig newslot abstract virtual 
          instance void  DestroyElement([in] string  marshal( lpwstr) pwcsName) runtime managed internalcall
  {
  } // end of method IStorage::DestroyElement

  .method public hidebysig newslot abstract virtual 
          instance void  RenameElement([in] string  marshal( lpwstr) pwcsOldName,
                                       [in] string  marshal( lpwstr) pwcsNewName) runtime managed internalcall
  {
  } // end of method IStorage::RenameElement

  .method public hidebysig newslot abstract virtual 
          instance void  SetElementTimes([in] string  marshal( lpwstr) pwcsName,
                                         [in] valuetype PortableDeviceApiLib._FILETIME& pctime,
                                         [in] valuetype PortableDeviceApiLib._FILETIME& patime,
                                         [in] valuetype PortableDeviceApiLib._FILETIME& pmtime) runtime managed internalcall
  {
  } // end of method IStorage::SetElementTimes

  .method public hidebysig newslot abstract virtual 
          instance void  SetClass([in] valuetype [mscorlib]System.Guid& 'clsid') runtime managed internalcall
  {
  } // end of method IStorage::SetClass

  .method public hidebysig newslot abstract virtual 
          instance void  SetStateBits([in] uint32 grfStateBits,
                                      [in] uint32 grfMask) runtime managed internalcall
  {
  } // end of method IStorage::SetStateBits

  .method public hidebysig newslot abstract virtual 
          instance void  Stat([out] valuetype PortableDeviceApiLib.tagSTATSTG& pstatstg,
                              [in] uint32 grfStatFlag) runtime managed internalcall
  {
  } // end of method IStorage::Stat

} // end of class PortableDeviceApiLib.IStorage

.class public sequential ansi sealed beforefieldinit PortableDeviceApiLib.tagRemSNB
       extends [mscorlib]System.ValueType
{
  .pack 4
  .size 0
  .custom instance void [mscorlib]System.Runtime.InteropServices.ComConversionLossAttribute::.ctor() = ( 01 00 00 00 ) 
  .field public uint32 ulCntStr
  .field public uint32 ulCntChar
  .field public native int rgString
  .custom instance void [mscorlib]System.Runtime.InteropServices.ComConversionLossAttribute::.ctor() = ( 01 00 00 00 ) 
} // end of class PortableDeviceApiLib.tagRemSNB

.class interface public abstract auto ansi import PortableDeviceApiLib.IEnumSTATSTG
{
  .custom instance void [mscorlib]System.Runtime.InteropServices.GuidAttribute::.ctor(string) = ( 01 00 24 30 30 30 30 30 30 30 44 2D 30 30 30 30   // ..$0000000D-0000
                                                                                                  2D 30 30 30 30 2D 43 30 30 30 2D 30 30 30 30 30   // -0000-C000-00000
                                                                                                  30 30 30 30 30 34 36 00 00 )                      // 0000046..
  .custom instance void [mscorlib]System.Runtime.InteropServices.InterfaceTypeAttribute::.ctor(int16) = ( 01 00 01 00 00 00 ) 
  .method public hidebysig newslot abstract virtual 
          instance void  RemoteNext([in] uint32 celt,
                                    [out] valuetype PortableDeviceApiLib.tagSTATSTG& rgelt,
                                    [out] uint32& pceltFetched) runtime managed internalcall
  {
  } // end of method IEnumSTATSTG::RemoteNext

  .method public hidebysig newslot abstract virtual 
          instance void  Skip([in] uint32 celt) runtime managed internalcall
  {
  } // end of method IEnumSTATSTG::Skip

  .method public hidebysig newslot abstract virtual 
          instance void  Reset() runtime managed internalcall
  {
  } // end of method IEnumSTATSTG::Reset

  .method public hidebysig newslot abstract virtual 
          instance void  Clone([out] class PortableDeviceApiLib.IEnumSTATSTG&  marshal( interface ) ppenum) runtime managed internalcall
  {
  } // end of method IEnumSTATSTG::Clone

} // end of class PortableDeviceApiLib.IEnumSTATSTG

.class public sequential ansi sealed beforefieldinit PortableDeviceApiLib.tagVersionedStream
       extends [mscorlib]System.ValueType
{
  .pack 4
  .size 0
  .field public valuetype [mscorlib]System.Guid guidVersion
  .field public  marshal( interface ) class PortableDeviceApiLib.IStream pStream
} // end of class PortableDeviceApiLib.tagVersionedStream

.class public sequential ansi sealed beforefieldinit PortableDeviceApiLib._wireSAFEARRAY
       extends [mscorlib]System.ValueType
{
  .pack 4
  .size 0
  .custom instance void [mscorlib]System.Runtime.InteropServices.ComConversionLossAttribute::.ctor() = ( 01 00 00 00 ) 
  .field public uint16 cDims
  .field public uint16 fFeatures
  .field public uint32 cbElements
  .field public uint32 cLocks
  .field public valuetype PortableDeviceApiLib._wireSAFEARRAY_UNION uArrayStructs
  .field public native int rgsabound
  .custom instance void [mscorlib]System.Runtime.InteropServices.ComConversionLossAttribute::.ctor() = ( 01 00 00 00 ) 
} // end of class PortableDeviceApiLib._wireSAFEARRAY

.class public sequential ansi sealed beforefieldinit PortableDeviceApiLib._wireSAFEARRAY_UNION
       extends [mscorlib]System.ValueType
{
  .pack 4
  .size 0
  .field public uint32 sfType
  .field public valuetype PortableDeviceApiLib.__MIDL_IOleAutomationTypes_0001 u
} // end of class PortableDeviceApiLib._wireSAFEARRAY_UNION

.class public explicit ansi sealed PortableDeviceApiLib.__MIDL_IOleAutomationTypes_0001
       extends [mscorlib]System.ValueType
{
  .pack 4
  .size 24
} // end of class PortableDeviceApiLib.__MIDL_IOleAutomationTypes_0001

.class public sequential ansi sealed beforefieldinit PortableDeviceApiLib._wireSAFEARR_BSTR
       extends [mscorlib]System.ValueType
{
  .pack 4
  .size 0
  .custom instance void [mscorlib]System.Runtime.InteropServices.ComConversionLossAttribute::.ctor() = ( 01 00 00 00 ) 
  .field public uint32 Size
  .field public native int aBstr
  .custom instance void [mscorlib]System.Runtime.InteropServices.ComConversionLossAttribute::.ctor() = ( 01 00 00 00 ) 
} // end of class PortableDeviceApiLib._wireSAFEARR_BSTR

.class public sequential ansi sealed beforefieldinit PortableDeviceApiLib._FLAGGED_WORD_BLOB
       extends [mscorlib]System.ValueType
{
  .pack 4
  .size 0
  .custom instance void [mscorlib]System.Runtime.InteropServices.ComConversionLossAttribute::.ctor() = ( 01 00 00 00 ) 
  .field public uint32 fFlags
  .field public uint32 clSize
  .field public native int asData
  .custom instance void [mscorlib]System.Runtime.InteropServices.ComConversionLossAttribute::.ctor() = ( 01 00 00 00 ) 
} // end of class PortableDeviceApiLib._FLAGGED_WORD_BLOB

.class public sequential ansi sealed beforefieldinit PortableDeviceApiLib._wireSAFEARR_UNKNOWN
       extends [mscorlib]System.ValueType
{
  .pack 4
  .size 0
  .custom instance void [mscorlib]System.Runtime.InteropServices.ComConversionLossAttribute::.ctor() = ( 01 00 00 00 ) 
  .field public uint32 Size
  .field public native int apUnknown
  .custom instance void [mscorlib]System.Runtime.InteropServices.ComConversionLossAttribute::.ctor() = ( 01 00 00 00 ) 
} // end of class PortableDeviceApiLib._wireSAFEARR_UNKNOWN

.class public sequential ansi sealed beforefieldinit PortableDeviceApiLib._wireSAFEARR_DISPATCH
       extends [mscorlib]System.ValueType
{
  .pack 4
  .size 0
  .custom instance void [mscorlib]System.Runtime.InteropServices.ComConversionLossAttribute::.ctor() = ( 01 00 00 00 ) 
  .field public uint32 Size
  .field public native int apDispatch
  .custom instance void [mscorlib]System.Runtime.InteropServices.ComConversionLossAttribute::.ctor() = ( 01 00 00 00 ) 
} // end of class PortableDeviceApiLib._wireSAFEARR_DISPATCH

.class public sequential ansi sealed beforefieldinit PortableDeviceApiLib._wireSAFEARR_VARIANT
       extends [mscorlib]System.ValueType
{
  .pack 4
  .size 0
  .custom instance void [mscorlib]System.Runtime.InteropServices.ComConversionLossAttribute::.ctor() = ( 01 00 00 00 ) 
  .field public uint32 Size
  .field public native int aVariant
  .custom instance void [mscorlib]System.Runtime.InteropServices.ComConversionLossAttribute::.ctor() = ( 01 00 00 00 ) 
} // end of class PortableDeviceApiLib._wireSAFEARR_VARIANT

.class public sequential ansi sealed beforefieldinit PortableDeviceApiLib._wireVARIANT
       extends [mscorlib]System.ValueType
{
  .pack 8
  .size 0
  .field public uint32 clSize
  .field public uint32 rpcReserved
  .field public uint16 vt
  .field public uint16 wReserved1
  .field public uint16 wReserved2
  .field public uint16 wReserved3
  .field public valuetype PortableDeviceApiLib.__MIDL_IOleAutomationTypes_0004 __MIDL__IOleAutomationTypes0002
} // end of class PortableDeviceApiLib._wireVARIANT

.class public explicit ansi sealed PortableDeviceApiLib.__MIDL_IOleAutomationTypes_0004
       extends [mscorlib]System.ValueType
{
  .pack 8
  .size 16
} // end of class PortableDeviceApiLib.__MIDL_IOleAutomationTypes_0004

.class public sequential ansi sealed beforefieldinit PortableDeviceApiLib._wireBRECORD
       extends [mscorlib]System.ValueType
{
  .pack 4
  .size 0
  .custom instance void [mscorlib]System.Runtime.InteropServices.ComConversionLossAttribute::.ctor() = ( 01 00 00 00 ) 
  .field public uint32 fFlags
  .field public uint32 clSize
  .field public  marshal( interface ) class PortableDeviceApiLib.IRecordInfo pRecInfo
  .field public native int pRecord
  .custom instance void [mscorlib]System.Runtime.InteropServices.ComConversionLossAttribute::.ctor() = ( 01 00 00 00 ) 
} // end of class PortableDeviceApiLib._wireBRECORD

.class interface public abstract auto ansi import PortableDeviceApiLib.IRecordInfo
{
  .custom instance void [mscorlib]System.Runtime.InteropServices.GuidAttribute::.ctor(string) = ( 01 00 24 30 30 30 30 30 30 32 46 2D 30 30 30 30   // ..$0000002F-0000
                                                                                                  2D 30 30 30 30 2D 43 30 30 30 2D 30 30 30 30 30   // -0000-C000-00000
                                                                                                  30 30 30 30 30 34 36 00 00 )                      // 0000046..
  .custom instance void [mscorlib]System.Runtime.InteropServices.InterfaceTypeAttribute::.ctor(int16) = ( 01 00 01 00 00 00 ) 
  .method public hidebysig newslot abstract virtual 
          instance void  RecordInit([out] native int pvNew) runtime managed internalcall
  {
  } // end of method IRecordInfo::RecordInit

  .method public hidebysig newslot abstract virtual 
          instance void  RecordClear([in] native int pvExisting) runtime managed internalcall
  {
  } // end of method IRecordInfo::RecordClear

  .method public hidebysig newslot abstract virtual 
          instance void  RecordCopy([in] native int pvExisting,
                                    [out] native int pvNew) runtime managed internalcall
  {
  } // end of method IRecordInfo::RecordCopy

  .method public hidebysig newslot abstract virtual 
          instance void  GetGuid([out] valuetype [mscorlib]System.Guid& pguid) runtime managed internalcall
  {
  } // end of method IRecordInfo::GetGuid

  .method public hidebysig newslot abstract virtual 
          instance void  GetName([out] string&  marshal( bstr) pbstrName) runtime managed internalcall
  {
  } // end of method IRecordInfo::GetName

  .method public hidebysig newslot abstract virtual 
          instance void  GetSize([out] uint32& pcbSize) runtime managed internalcall
  {
  } // end of method IRecordInfo::GetSize

  .method public hidebysig newslot abstract virtual 
          instance void  GetTypeInfo([out] class [mscorlib]System.Type&  marshal( custom ("System.Runtime.InteropServices.CustomMarshalers.TypeToTypeInfoMarshaler, CustomMarshalers, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a",
                                     "")) ppTypeInfo) runtime managed internalcall
  {
  } // end of method IRecordInfo::GetTypeInfo

  .method public hidebysig newslot abstract virtual 
          instance void  GetField([in] native int pvData,
                                  [in] string  marshal( lpwstr) szFieldName,
                                  [out] object&  marshal( struct) pvarField) runtime managed internalcall
  {
  } // end of method IRecordInfo::GetField

  .method public hidebysig newslot abstract virtual 
          instance void  GetFieldNoCopy([in] native int pvData,
                                        [in] string  marshal( lpwstr) szFieldName,
                                        [out] object&  marshal( struct) pvarField,
                                        [out] native int& ppvDataCArray) runtime managed internalcall
  {
  } // end of method IRecordInfo::GetFieldNoCopy

  .method public hidebysig newslot abstract virtual 
          instance void  PutField([in] uint32 wFlags,
                                  [in][out] native int pvData,
                                  [in] string  marshal( lpwstr) szFieldName,
                                  [in] object&  marshal( struct) pvarField) runtime managed internalcall
  {
  } // end of method IRecordInfo::PutField

  .method public hidebysig newslot abstract virtual 
          instance void  PutFieldNoCopy([in] uint32 wFlags,
                                        [in][out] native int pvData,
                                        [in] string  marshal( lpwstr) szFieldName,
                                        [in] object&  marshal( struct) pvarField) runtime managed internalcall
  {
  } // end of method IRecordInfo::PutFieldNoCopy

  .method public hidebysig newslot abstract virtual 
          instance void  GetFieldNames([in][out] uint32& pcNames,
                                       [out] string&  marshal( bstr) rgBstrNames) runtime managed internalcall
  {
  } // end of method IRecordInfo::GetFieldNames

  .method public hidebysig newslot abstract virtual 
          instance int32  IsMatchingType([in] class PortableDeviceApiLib.IRecordInfo  marshal( interface ) pRecordInfo) runtime managed preservesig internalcall
  {
  } // end of method IRecordInfo::IsMatchingType

  .method public hidebysig newslot abstract virtual 
          instance native int  RecordCreate() runtime managed preservesig internalcall
  {
  } // end of method IRecordInfo::RecordCreate

  .method public hidebysig newslot abstract virtual 
          instance void  RecordCreateCopy([in] native int pvSource,
                                          [out] native int& ppvDest) runtime managed internalcall
  {
  } // end of method IRecordInfo::RecordCreateCopy

  .method public hidebysig newslot abstract virtual 
          instance void  RecordDestroy([in] native int pvRecord) runtime managed internalcall
  {
  } // end of method IRecordInfo::RecordDestroy

} // end of class PortableDeviceApiLib.IRecordInfo

.class interface public abstract auto ansi import PortableDeviceApiLib.ITypeInfo
{
  .custom instance void [mscorlib]System.Runtime.InteropServices.GuidAttribute::.ctor(string) = ( 01 00 24 30 30 30 32 30 34 30 31 2D 30 30 30 30   // ..$00020401-0000
                                                                                                  2D 30 30 30 30 2D 43 30 30 30 2D 30 30 30 30 30   // -0000-C000-00000
                                                                                                  30 30 30 30 30 34 36 00 00 )                      // 0000046..
  .custom instance void [mscorlib]System.Runtime.InteropServices.InterfaceTypeAttribute::.ctor(int16) = ( 01 00 01 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.InteropServices.ComConversionLossAttribute::.ctor() = ( 01 00 00 00 ) 
  .method public hidebysig newslot abstract virtual 
          instance void  RemoteGetTypeAttr([out] native int ppTypeAttr,
                                           [out] uint32& pDummy) runtime managed internalcall
  {
    .param [2]
    .custom instance void [mscorlib]System.Runtime.InteropServices.ComAliasNameAttribute::.ctor(string) = ( 01 00 1A 50 6F 72 74 61 62 6C 65 44 65 76 69 63   // ...PortableDevic
                                                                                                            65 41 70 69 4C 69 62 2E 44 57 4F 52 44 00 00 )    // eApiLib.DWORD..
  } // end of method ITypeInfo::RemoteGetTypeAttr

  .method public hidebysig newslot abstract virtual 
          instance void  GetTypeComp([out] class PortableDeviceApiLib.ITypeComp&  marshal( interface ) ppTComp) runtime managed internalcall
  {
  } // end of method ITypeInfo::GetTypeComp

  .method public hidebysig newslot abstract virtual 
          instance void  RemoteGetFuncDesc([in] uint32 index,
                                           [out] native int ppFuncDesc,
                                           [out] uint32& pDummy) runtime managed internalcall
  {
    .param [3]
    .custom instance void [mscorlib]System.Runtime.InteropServices.ComAliasNameAttribute::.ctor(string) = ( 01 00 1A 50 6F 72 74 61 62 6C 65 44 65 76 69 63   // ...PortableDevic
                                                                                                            65 41 70 69 4C 69 62 2E 44 57 4F 52 44 00 00 )    // eApiLib.DWORD..
  } // end of method ITypeInfo::RemoteGetFuncDesc

  .method public hidebysig newslot abstract virtual 
          instance void  RemoteGetVarDesc([in] uint32 index,
                                          [out] native int ppVarDesc,
                                          [out] uint32& pDummy) runtime managed internalcall
  {
    .param [3]
    .custom instance void [mscorlib]System.Runtime.InteropServices.ComAliasNameAttribute::.ctor(string) = ( 01 00 1A 50 6F 72 74 61 62 6C 65 44 65 76 69 63   // ...PortableDevic
                                                                                                            65 41 70 69 4C 69 62 2E 44 57 4F 52 44 00 00 )    // eApiLib.DWORD..
  } // end of method ITypeInfo::RemoteGetVarDesc

  .method public hidebysig newslot abstract virtual 
          instance void  RemoteGetNames([in] int32 memid,
                                        [out] string&  marshal( bstr) rgBstrNames,
                                        [in] uint32 cMaxNames,
                                        [out] uint32& pcNames) runtime managed internalcall
  {
  } // end of method ITypeInfo::RemoteGetNames

  .method public hidebysig newslot abstract virtual 
          instance void  GetRefTypeOfImplType([in] uint32 index,
                                              [out] uint32& pRefType) runtime managed internalcall
  {
  } // end of method ITypeInfo::GetRefTypeOfImplType

  .method public hidebysig newslot abstract virtual 
          instance void  GetImplTypeFlags([in] uint32 index,
                                          [out] int32& pImplTypeFlags) runtime managed internalcall
  {
  } // end of method ITypeInfo::GetImplTypeFlags

  .method public hidebysig newslot abstract virtual 
          instance void  LocalGetIDsOfNames() runtime managed internalcall
  {
  } // end of method ITypeInfo::LocalGetIDsOfNames

  .method public hidebysig newslot abstract virtual 
          instance void  LocalInvoke() runtime managed internalcall
  {
  } // end of method ITypeInfo::LocalInvoke

  .method public hidebysig newslot abstract virtual 
          instance void  RemoteGetDocumentation([in] int32 memid,
                                                [in] uint32 refPtrFlags,
                                                [out] string&  marshal( bstr) pbstrName,
                                                [out] string&  marshal( bstr) pBstrDocString,
                                                [out] uint32& pdwHelpContext,
                                                [out] string&  marshal( bstr) pBstrHelpFile) runtime managed internalcall
  {
  } // end of method ITypeInfo::RemoteGetDocumentation

  .method public hidebysig newslot abstract virtual 
          instance void  RemoteGetDllEntry([in] int32 memid,
                                           [in] valuetype PortableDeviceApiLib.tagINVOKEKIND invkind,
                                           [in] uint32 refPtrFlags,
                                           [out] string&  marshal( bstr) pBstrDllName,
                                           [out] string&  marshal( bstr) pbstrName,
                                           [out] uint16& pwOrdinal) runtime managed internalcall
  {
  } // end of method ITypeInfo::RemoteGetDllEntry

  .method public hidebysig newslot abstract virtual 
          instance void  GetRefTypeInfo([in] uint32 hreftype,
                                        [out] class [mscorlib]System.Type&  marshal( custom ("System.Runtime.InteropServices.CustomMarshalers.TypeToTypeInfoMarshaler, CustomMarshalers, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a",
                                        "")) ppTInfo) runtime managed internalcall
  {
  } // end of method ITypeInfo::GetRefTypeInfo

  .method public hidebysig newslot abstract virtual 
          instance void  LocalAddressOfMember() runtime managed internalcall
  {
  } // end of method ITypeInfo::LocalAddressOfMember

  .method public hidebysig newslot abstract virtual 
          instance void  RemoteCreateInstance([in] valuetype [mscorlib]System.Guid& riid,
                                              [out] object&  marshal( iunknown ) ppvObj) runtime managed internalcall
  {
  } // end of method ITypeInfo::RemoteCreateInstance

  .method public hidebysig newslot abstract virtual 
          instance void  GetMops([in] int32 memid,
                                 [out] string&  marshal( bstr) pBstrMops) runtime managed internalcall
  {
  } // end of method ITypeInfo::GetMops

  .method public hidebysig newslot abstract virtual 
          instance void  RemoteGetContainingTypeLib([out] class PortableDeviceApiLib.ITypeLib&  marshal( interface ) ppTLib,
                                                    [out] uint32& pIndex) runtime managed internalcall
  {
  } // end of method ITypeInfo::RemoteGetContainingTypeLib

  .method public hidebysig newslot abstract virtual 
          instance void  LocalReleaseTypeAttr() runtime managed internalcall
  {
  } // end of method ITypeInfo::LocalReleaseTypeAttr

  .method public hidebysig newslot abstract virtual 
          instance void  LocalReleaseFuncDesc() runtime managed internalcall
  {
  } // end of method ITypeInfo::LocalReleaseFuncDesc

  .method public hidebysig newslot abstract virtual 
          instance void  LocalReleaseVarDesc() runtime managed internalcall
  {
  } // end of method ITypeInfo::LocalReleaseVarDesc

} // end of class PortableDeviceApiLib.ITypeInfo

.class public sequential ansi sealed beforefieldinit PortableDeviceApiLib.tagTYPEATTR
       extends [mscorlib]System.ValueType
{
  .pack 4
  .size 0
  .field public valuetype [mscorlib]System.Guid guid
  .field public uint32 lcid
  .field public uint32 dwReserved
  .field public int32 memidConstructor
  .field public int32 memidDestructor
  .field public  marshal( lpwstr) string lpstrSchema
  .field public uint32 cbSizeInstance
  .field public valuetype PortableDeviceApiLib.tagTYPEKIND typekind
  .field public uint16 cFuncs
  .field public uint16 cVars
  .field public uint16 cImplTypes
  .field public uint16 cbSizeVft
  .field public uint16 cbAlignment
  .field public uint16 wTypeFlags
  .field public uint16 wMajorVerNum
  .field public uint16 wMinorVerNum
  .field public valuetype PortableDeviceApiLib.tagTYPEDESC tdescAlias
  .field public valuetype PortableDeviceApiLib.tagIDLDESC idldescType
} // end of class PortableDeviceApiLib.tagTYPEATTR

.class public auto ansi sealed PortableDeviceApiLib.tagTYPEKIND
       extends [mscorlib]System.Enum
{
  .field public specialname rtspecialname int32 value__
  .field public static literal valuetype PortableDeviceApiLib.tagTYPEKIND TKIND_ENUM = int32(0x00000000)
  .field public static literal valuetype PortableDeviceApiLib.tagTYPEKIND TKIND_RECORD = int32(0x00000001)
  .field public static literal valuetype PortableDeviceApiLib.tagTYPEKIND TKIND_MODULE = int32(0x00000002)
  .field public static literal valuetype PortableDeviceApiLib.tagTYPEKIND TKIND_INTERFACE = int32(0x00000003)
  .field public static literal valuetype PortableDeviceApiLib.tagTYPEKIND TKIND_DISPATCH = int32(0x00000004)
  .field public static literal valuetype PortableDeviceApiLib.tagTYPEKIND TKIND_COCLASS = int32(0x00000005)
  .field public static literal valuetype PortableDeviceApiLib.tagTYPEKIND TKIND_ALIAS = int32(0x00000006)
  .field public static literal valuetype PortableDeviceApiLib.tagTYPEKIND TKIND_UNION = int32(0x00000007)
  .field public static literal valuetype PortableDeviceApiLib.tagTYPEKIND TKIND_MAX = int32(0x00000008)
} // end of class PortableDeviceApiLib.tagTYPEKIND

.class public sequential ansi sealed beforefieldinit PortableDeviceApiLib.tagTYPEDESC
       extends [mscorlib]System.ValueType
{
  .pack 4
  .size 0
  .field public valuetype PortableDeviceApiLib.__MIDL_IOleAutomationTypes_0005 __MIDL__IOleAutomationTypes0004
  .field public uint16 vt
} // end of class PortableDeviceApiLib.tagTYPEDESC

.class public explicit ansi sealed PortableDeviceApiLib.__MIDL_IOleAutomationTypes_0005
       extends [mscorlib]System.ValueType
{
  .pack 4
  .size 4
} // end of class PortableDeviceApiLib.__MIDL_IOleAutomationTypes_0005

.class public sequential ansi sealed beforefieldinit PortableDeviceApiLib.tagARRAYDESC
       extends [mscorlib]System.ValueType
{
  .pack 4
  .size 0
  .custom instance void [mscorlib]System.Runtime.InteropServices.ComConversionLossAttribute::.ctor() = ( 01 00 00 00 ) 
  .field public valuetype PortableDeviceApiLib.tagTYPEDESC tdescElem
  .field public uint16 cDims
  .field public native int rgbounds
  .custom instance void [mscorlib]System.Runtime.InteropServices.ComConversionLossAttribute::.ctor() = ( 01 00 00 00 ) 
} // end of class PortableDeviceApiLib.tagARRAYDESC

.class public sequential ansi sealed beforefieldinit PortableDeviceApiLib.tagSAFEARRAYBOUND
       extends [mscorlib]System.ValueType
{
  .pack 4
  .size 0
  .field public uint32 cElements
  .field public int32 lLbound
} // end of class PortableDeviceApiLib.tagSAFEARRAYBOUND

.class public sequential ansi sealed beforefieldinit PortableDeviceApiLib.tagIDLDESC
       extends [mscorlib]System.ValueType
{
  .pack 4
  .size 0
  .field public uint32 dwReserved
  .custom instance void [mscorlib]System.Runtime.InteropServices.ComAliasNameAttribute::.ctor(string) = ( 01 00 1E 50 6F 72 74 61 62 6C 65 44 65 76 69 63   // ...PortableDevic
                                                                                                          65 41 70 69 4C 69 62 2E 55 4C 4F 4E 47 5F 50 54   // eApiLib.ULONG_PT
                                                                                                          52 00 00 )                                        // R..
  .field public uint16 wIDLFlags
} // end of class PortableDeviceApiLib.tagIDLDESC

.class interface public abstract auto ansi import PortableDeviceApiLib.ITypeComp
{
  .custom instance void [mscorlib]System.Runtime.InteropServices.ComConversionLossAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.InteropServices.GuidAttribute::.ctor(string) = ( 01 00 24 30 30 30 32 30 34 30 33 2D 30 30 30 30   // ..$00020403-0000
                                                                                                  2D 30 30 30 30 2D 43 30 30 30 2D 30 30 30 30 30   // -0000-C000-00000
                                                                                                  30 30 30 30 30 34 36 00 00 )                      // 0000046..
  .custom instance void [mscorlib]System.Runtime.InteropServices.InterfaceTypeAttribute::.ctor(int16) = ( 01 00 01 00 00 00 ) 
  .method public hidebysig newslot abstract virtual 
          instance void  RemoteBind([in] string  marshal( lpwstr) szName,
                                    [in] uint32 lHashVal,
                                    [in] uint16 wFlags,
                                    [out] class [mscorlib]System.Type&  marshal( custom ("System.Runtime.InteropServices.CustomMarshalers.TypeToTypeInfoMarshaler, CustomMarshalers, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a",
                                    "")) ppTInfo,
                                    [out] valuetype PortableDeviceApiLib.tagDESCKIND& pDescKind,
                                    [out] native int ppFuncDesc,
                                    [out] native int ppVarDesc,
                                    [out] class PortableDeviceApiLib.ITypeComp&  marshal( interface ) ppTypeComp,
                                    [out] uint32& pDummy) runtime managed internalcall
  {
    .param [9]
    .custom instance void [mscorlib]System.Runtime.InteropServices.ComAliasNameAttribute::.ctor(string) = ( 01 00 1A 50 6F 72 74 61 62 6C 65 44 65 76 69 63   // ...PortableDevic
                                                                                                            65 41 70 69 4C 69 62 2E 44 57 4F 52 44 00 00 )    // eApiLib.DWORD..
  } // end of method ITypeComp::RemoteBind

  .method public hidebysig newslot abstract virtual 
          instance void  RemoteBindType([in] string  marshal( lpwstr) szName,
                                        [in] uint32 lHashVal,
                                        [out] class [mscorlib]System.Type&  marshal( custom ("System.Runtime.InteropServices.CustomMarshalers.TypeToTypeInfoMarshaler, CustomMarshalers, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a",
                                        "")) ppTInfo) runtime managed internalcall
  {
  } // end of method ITypeComp::RemoteBindType

} // end of class PortableDeviceApiLib.ITypeComp

.class public auto ansi sealed PortableDeviceApiLib.tagDESCKIND
       extends [mscorlib]System.Enum
{
  .field public specialname rtspecialname int32 value__
  .field public static literal valuetype PortableDeviceApiLib.tagDESCKIND DESCKIND_NONE = int32(0x00000000)
  .field public static literal valuetype PortableDeviceApiLib.tagDESCKIND DESCKIND_FUNCDESC = int32(0x00000001)
  .field public static literal valuetype PortableDeviceApiLib.tagDESCKIND DESCKIND_VARDESC = int32(0x00000002)
  .field public static literal valuetype PortableDeviceApiLib.tagDESCKIND DESCKIND_TYPECOMP = int32(0x00000003)
  .field public static literal valuetype PortableDeviceApiLib.tagDESCKIND DESCKIND_IMPLICITAPPOBJ = int32(0x00000004)
  .field public static literal valuetype PortableDeviceApiLib.tagDESCKIND DESCKIND_MAX = int32(0x00000005)
} // end of class PortableDeviceApiLib.tagDESCKIND

.class public sequential ansi sealed beforefieldinit PortableDeviceApiLib.tagFUNCDESC
       extends [mscorlib]System.ValueType
{
  .pack 4
  .size 0
  .custom instance void [mscorlib]System.Runtime.InteropServices.ComConversionLossAttribute::.ctor() = ( 01 00 00 00 ) 
  .field public int32 memid
  .field public native int lprgscode
  .custom instance void [mscorlib]System.Runtime.InteropServices.ComConversionLossAttribute::.ctor() = ( 01 00 00 00 ) 
  .field public native int lprgelemdescParam
  .custom instance void [mscorlib]System.Runtime.InteropServices.ComConversionLossAttribute::.ctor() = ( 01 00 00 00 ) 
  .field public valuetype PortableDeviceApiLib.tagFUNCKIND funckind
  .field public valuetype PortableDeviceApiLib.tagINVOKEKIND invkind
  .field public valuetype PortableDeviceApiLib.tagCALLCONV 'callconv'
  .field public int16 cParams
  .field public int16 cParamsOpt
  .field public int16 oVft
  .field public int16 cScodes
  .field public valuetype PortableDeviceApiLib.tagELEMDESC elemdescFunc
  .field public uint16 wFuncFlags
} // end of class PortableDeviceApiLib.tagFUNCDESC

.class public sequential ansi sealed beforefieldinit PortableDeviceApiLib.tagELEMDESC
       extends [mscorlib]System.ValueType
{
  .pack 4
  .size 0
  .field public valuetype PortableDeviceApiLib.tagTYPEDESC tdesc
  .field public valuetype PortableDeviceApiLib.tagPARAMDESC paramdesc
} // end of class PortableDeviceApiLib.tagELEMDESC

.class public sequential ansi sealed beforefieldinit PortableDeviceApiLib.tagPARAMDESC
       extends [mscorlib]System.ValueType
{
  .pack 4
  .size 0
  .custom instance void [mscorlib]System.Runtime.InteropServices.ComConversionLossAttribute::.ctor() = ( 01 00 00 00 ) 
  .field public native int pparamdescex
  .custom instance void [mscorlib]System.Runtime.InteropServices.ComConversionLossAttribute::.ctor() = ( 01 00 00 00 ) 
  .field public uint16 wParamFlags
} // end of class PortableDeviceApiLib.tagPARAMDESC

.class public sequential ansi sealed beforefieldinit PortableDeviceApiLib.tagPARAMDESCEX
       extends [mscorlib]System.ValueType
{
  .pack 8
  .size 0
  .field public uint32 cBytes
  .field public  marshal( struct) object varDefaultValue
} // end of class PortableDeviceApiLib.tagPARAMDESCEX

.class public auto ansi sealed PortableDeviceApiLib.tagFUNCKIND
       extends [mscorlib]System.Enum
{
  .field public specialname rtspecialname int32 value__
  .field public static literal valuetype PortableDeviceApiLib.tagFUNCKIND FUNC_VIRTUAL = int32(0x00000000)
  .field public static literal valuetype PortableDeviceApiLib.tagFUNCKIND FUNC_PUREVIRTUAL = int32(0x00000001)
  .field public static literal valuetype PortableDeviceApiLib.tagFUNCKIND FUNC_NONVIRTUAL = int32(0x00000002)
  .field public static literal valuetype PortableDeviceApiLib.tagFUNCKIND FUNC_STATIC = int32(0x00000003)
  .field public static literal valuetype PortableDeviceApiLib.tagFUNCKIND FUNC_DISPATCH = int32(0x00000004)
} // end of class PortableDeviceApiLib.tagFUNCKIND

.class public auto ansi sealed PortableDeviceApiLib.tagINVOKEKIND
       extends [mscorlib]System.Enum
{
  .field public specialname rtspecialname int32 value__
  .field public static literal valuetype PortableDeviceApiLib.tagINVOKEKIND INVOKE_FUNC = int32(0x00000001)
  .field public static literal valuetype PortableDeviceApiLib.tagINVOKEKIND INVOKE_PROPERTYGET = int32(0x00000002)
  .field public static literal valuetype PortableDeviceApiLib.tagINVOKEKIND INVOKE_PROPERTYPUT = int32(0x00000004)
  .field public static literal valuetype PortableDeviceApiLib.tagINVOKEKIND INVOKE_PROPERTYPUTREF = int32(0x00000008)
} // end of class PortableDeviceApiLib.tagINVOKEKIND

.class public auto ansi sealed PortableDeviceApiLib.tagCALLCONV
       extends [mscorlib]System.Enum
{
  .field public specialname rtspecialname int32 value__
  .field public static literal valuetype PortableDeviceApiLib.tagCALLCONV CC_FASTCALL = int32(0x00000000)
  .field public static literal valuetype PortableDeviceApiLib.tagCALLCONV CC_CDECL = int32(0x00000001)
  .field public static literal valuetype PortableDeviceApiLib.tagCALLCONV CC_MSCPASCAL = int32(0x00000002)
  .field public static literal valuetype PortableDeviceApiLib.tagCALLCONV CC_PASCAL = int32(0x00000002)
  .field public static literal valuetype PortableDeviceApiLib.tagCALLCONV CC_MACPASCAL = int32(0x00000003)
  .field public static literal valuetype PortableDeviceApiLib.tagCALLCONV CC_STDCALL = int32(0x00000004)
  .field public static literal valuetype PortableDeviceApiLib.tagCALLCONV CC_FPFASTCALL = int32(0x00000005)
  .field public static literal valuetype PortableDeviceApiLib.tagCALLCONV CC_SYSCALL = int32(0x00000006)
  .field public static literal valuetype PortableDeviceApiLib.tagCALLCONV CC_MPWCDECL = int32(0x00000007)
  .field public static literal valuetype PortableDeviceApiLib.tagCALLCONV CC_MPWPASCAL = int32(0x00000008)
  .field public static literal valuetype PortableDeviceApiLib.tagCALLCONV CC_MAX = int32(0x00000009)
} // end of class PortableDeviceApiLib.tagCALLCONV

.class public sequential ansi sealed beforefieldinit PortableDeviceApiLib.tagVARDESC
       extends [mscorlib]System.ValueType
{
  .pack 4
  .size 0
  .field public int32 memid
  .field public  marshal( lpwstr) string lpstrSchema
  .field public valuetype PortableDeviceApiLib.__MIDL_IOleAutomationTypes_0006 __MIDL__IOleAutomationTypes0005
  .field public valuetype PortableDeviceApiLib.tagELEMDESC elemdescVar
  .field public uint16 wVarFlags
  .field public valuetype PortableDeviceApiLib.tagVARKIND varkind
} // end of class PortableDeviceApiLib.tagVARDESC

.class public explicit ansi sealed PortableDeviceApiLib.__MIDL_IOleAutomationTypes_0006
       extends [mscorlib]System.ValueType
{
  .pack 4
  .size 4
} // end of class PortableDeviceApiLib.__MIDL_IOleAutomationTypes_0006

.class public auto ansi sealed PortableDeviceApiLib.tagVARKIND
       extends [mscorlib]System.Enum
{
  .field public specialname rtspecialname int32 value__
  .field public static literal valuetype PortableDeviceApiLib.tagVARKIND VAR_PERINSTANCE = int32(0x00000000)
  .field public static literal valuetype PortableDeviceApiLib.tagVARKIND VAR_STATIC = int32(0x00000001)
  .field public static literal valuetype PortableDeviceApiLib.tagVARKIND VAR_CONST = int32(0x00000002)
  .field public static literal valuetype PortableDeviceApiLib.tagVARKIND VAR_DISPATCH = int32(0x00000003)
} // end of class PortableDeviceApiLib.tagVARKIND

.class interface public abstract auto ansi import PortableDeviceApiLib.ITypeLib
{
  .custom instance void [mscorlib]System.Runtime.InteropServices.GuidAttribute::.ctor(string) = ( 01 00 24 30 30 30 32 30 34 30 32 2D 30 30 30 30   // ..$00020402-0000
                                                                                                  2D 30 30 30 30 2D 43 30 30 30 2D 30 30 30 30 30   // -0000-C000-00000
                                                                                                  30 30 30 30 30 34 36 00 00 )                      // 0000046..
  .custom instance void [mscorlib]System.Runtime.InteropServices.InterfaceTypeAttribute::.ctor(int16) = ( 01 00 01 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.InteropServices.ComConversionLossAttribute::.ctor() = ( 01 00 00 00 ) 
  .method public hidebysig newslot abstract virtual 
          instance void  RemoteGetTypeInfoCount([out] uint32& pcTInfo) runtime managed internalcall
  {
  } // end of method ITypeLib::RemoteGetTypeInfoCount

  .method public hidebysig newslot abstract virtual 
          instance void  GetTypeInfo([in] uint32 index,
                                     [out] class [mscorlib]System.Type&  marshal( custom ("System.Runtime.InteropServices.CustomMarshalers.TypeToTypeInfoMarshaler, CustomMarshalers, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a",
                                     "")) ppTInfo) runtime managed internalcall
  {
  } // end of method ITypeLib::GetTypeInfo

  .method public hidebysig newslot abstract virtual 
          instance void  GetTypeInfoType([in] uint32 index,
                                         [out] valuetype PortableDeviceApiLib.tagTYPEKIND& pTKind) runtime managed internalcall
  {
  } // end of method ITypeLib::GetTypeInfoType

  .method public hidebysig newslot abstract virtual 
          instance void  GetTypeInfoOfGuid([in] valuetype [mscorlib]System.Guid& guid,
                                           [out] class [mscorlib]System.Type&  marshal( custom ("System.Runtime.InteropServices.CustomMarshalers.TypeToTypeInfoMarshaler, CustomMarshalers, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a",
                                           "")) ppTInfo) runtime managed internalcall
  {
  } // end of method ITypeLib::GetTypeInfoOfGuid

  .method public hidebysig newslot abstract virtual 
          instance void  RemoteGetLibAttr([out] native int ppTLibAttr,
                                          [out] uint32& pDummy) runtime managed internalcall
  {
    .param [2]
    .custom instance void [mscorlib]System.Runtime.InteropServices.ComAliasNameAttribute::.ctor(string) = ( 01 00 1A 50 6F 72 74 61 62 6C 65 44 65 76 69 63   // ...PortableDevic
                                                                                                            65 41 70 69 4C 69 62 2E 44 57 4F 52 44 00 00 )    // eApiLib.DWORD..
  } // end of method ITypeLib::RemoteGetLibAttr

  .method public hidebysig newslot abstract virtual 
          instance void  GetTypeComp([out] class PortableDeviceApiLib.ITypeComp&  marshal( interface ) ppTComp) runtime managed internalcall
  {
  } // end of method ITypeLib::GetTypeComp

  .method public hidebysig newslot abstract virtual 
          instance void  RemoteGetDocumentation([in] int32 index,
                                                [in] uint32 refPtrFlags,
                                                [out] string&  marshal( bstr) pbstrName,
                                                [out] string&  marshal( bstr) pBstrDocString,
                                                [out] uint32& pdwHelpContext,
                                                [out] string&  marshal( bstr) pBstrHelpFile) runtime managed internalcall
  {
  } // end of method ITypeLib::RemoteGetDocumentation

  .method public hidebysig newslot abstract virtual 
          instance void  RemoteIsName([in] string  marshal( lpwstr) szNameBuf,
                                      [in] uint32 lHashVal,
                                      [out] int32& pfName,
                                      [out] string&  marshal( bstr) pBstrLibName) runtime managed internalcall
  {
  } // end of method ITypeLib::RemoteIsName

  .method public hidebysig newslot abstract virtual 
          instance void  RemoteFindName([in] string  marshal( lpwstr) szNameBuf,
                                        [in] uint32 lHashVal,
                                        [out] class [mscorlib]System.Type&  marshal( custom ("System.Runtime.InteropServices.CustomMarshalers.TypeToTypeInfoMarshaler, CustomMarshalers, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a",
                                        "")) ppTInfo,
                                        [out] int32& rgMemId,
                                        [in][out] uint16& pcFound,
                                        [out] string&  marshal( bstr) pBstrLibName) runtime managed internalcall
  {
  } // end of method ITypeLib::RemoteFindName

  .method public hidebysig newslot abstract virtual 
          instance void  LocalReleaseTLibAttr() runtime managed internalcall
  {
  } // end of method ITypeLib::LocalReleaseTLibAttr

} // end of class PortableDeviceApiLib.ITypeLib

.class public sequential ansi sealed beforefieldinit PortableDeviceApiLib.tagTLIBATTR
       extends [mscorlib]System.ValueType
{
  .pack 4
  .size 0
  .field public valuetype [mscorlib]System.Guid guid
  .field public uint32 lcid
  .field public valuetype PortableDeviceApiLib.tagSYSKIND syskind
  .field public uint16 wMajorVerNum
  .field public uint16 wMinorVerNum
  .field public uint16 wLibFlags
} // end of class PortableDeviceApiLib.tagTLIBATTR

.class public auto ansi sealed PortableDeviceApiLib.tagSYSKIND
       extends [mscorlib]System.Enum
{
  .field public specialname rtspecialname int32 value__
  .field public static literal valuetype PortableDeviceApiLib.tagSYSKIND SYS_WIN16 = int32(0x00000000)
  .field public static literal valuetype PortableDeviceApiLib.tagSYSKIND SYS_WIN32 = int32(0x00000001)
  .field public static literal valuetype PortableDeviceApiLib.tagSYSKIND SYS_MAC = int32(0x00000002)
  .field public static literal valuetype PortableDeviceApiLib.tagSYSKIND SYS_WIN64 = int32(0x00000003)
} // end of class PortableDeviceApiLib.tagSYSKIND

.class public sequential ansi sealed beforefieldinit PortableDeviceApiLib._wireSAFEARR_BRECORD
       extends [mscorlib]System.ValueType
{
  .pack 4
  .size 0
  .custom instance void [mscorlib]System.Runtime.InteropServices.ComConversionLossAttribute::.ctor() = ( 01 00 00 00 ) 
  .field public uint32 Size
  .field public native int aRecord
  .custom instance void [mscorlib]System.Runtime.InteropServices.ComConversionLossAttribute::.ctor() = ( 01 00 00 00 ) 
} // end of class PortableDeviceApiLib._wireSAFEARR_BRECORD

.class public sequential ansi sealed beforefieldinit PortableDeviceApiLib._wireSAFEARR_HAVEIID
       extends [mscorlib]System.ValueType
{
  .pack 4
  .size 0
  .custom instance void [mscorlib]System.Runtime.InteropServices.ComConversionLossAttribute::.ctor() = ( 01 00 00 00 ) 
  .field public uint32 Size
  .field public native int apUnknown
  .custom instance void [mscorlib]System.Runtime.InteropServices.ComConversionLossAttribute::.ctor() = ( 01 00 00 00 ) 
  .field public valuetype [mscorlib]System.Guid iid
} // end of class PortableDeviceApiLib._wireSAFEARR_HAVEIID

.class public sequential ansi sealed beforefieldinit PortableDeviceApiLib._BYTE_SIZEDARR
       extends [mscorlib]System.ValueType
{
  .pack 4
  .size 0
  .custom instance void [mscorlib]System.Runtime.InteropServices.ComConversionLossAttribute::.ctor() = ( 01 00 00 00 ) 
  .field public uint32 clSize
  .field public native int pData
  .custom instance void [mscorlib]System.Runtime.InteropServices.ComConversionLossAttribute::.ctor() = ( 01 00 00 00 ) 
} // end of class PortableDeviceApiLib._BYTE_SIZEDARR

.class public sequential ansi sealed beforefieldinit PortableDeviceApiLib._SHORT_SIZEDARR
       extends [mscorlib]System.ValueType
{
  .pack 4
  .size 0
  .custom instance void [mscorlib]System.Runtime.InteropServices.ComConversionLossAttribute::.ctor() = ( 01 00 00 00 ) 
  .field public uint32 clSize
  .field public native int pData
  .custom instance void [mscorlib]System.Runtime.InteropServices.ComConversionLossAttribute::.ctor() = ( 01 00 00 00 ) 
} // end of class PortableDeviceApiLib._SHORT_SIZEDARR

.class public sequential ansi sealed beforefieldinit PortableDeviceApiLib._LONG_SIZEDARR
       extends [mscorlib]System.ValueType
{
  .pack 4
  .size 0
  .custom instance void [mscorlib]System.Runtime.InteropServices.ComConversionLossAttribute::.ctor() = ( 01 00 00 00 ) 
  .field public uint32 clSize
  .field public native int pData
  .custom instance void [mscorlib]System.Runtime.InteropServices.ComConversionLossAttribute::.ctor() = ( 01 00 00 00 ) 
} // end of class PortableDeviceApiLib._LONG_SIZEDARR

.class public sequential ansi sealed beforefieldinit PortableDeviceApiLib._HYPER_SIZEDARR
       extends [mscorlib]System.ValueType
{
  .pack 4
  .size 0
  .custom instance void [mscorlib]System.Runtime.InteropServices.ComConversionLossAttribute::.ctor() = ( 01 00 00 00 ) 
  .field public uint32 clSize
  .field public native int pData
  .custom instance void [mscorlib]System.Runtime.InteropServices.ComConversionLossAttribute::.ctor() = ( 01 00 00 00 ) 
} // end of class PortableDeviceApiLib._HYPER_SIZEDARR

.class public sequential ansi sealed beforefieldinit PortableDeviceApiLib.tagCAC
       extends [mscorlib]System.ValueType
{
  .pack 4
  .size 0
  .custom instance void [mscorlib]System.Runtime.InteropServices.ComConversionLossAttribute::.ctor() = ( 01 00 00 00 ) 
  .field public uint32 cElems
  .field public native int pElems
  .custom instance void [mscorlib]System.Runtime.InteropServices.ComConversionLossAttribute::.ctor() = ( 01 00 00 00 ) 
} // end of class PortableDeviceApiLib.tagCAC

.class public sequential ansi sealed beforefieldinit PortableDeviceApiLib.tagCAUB
       extends [mscorlib]System.ValueType
{
  .pack 4
  .size 0
  .custom instance void [mscorlib]System.Runtime.InteropServices.ComConversionLossAttribute::.ctor() = ( 01 00 00 00 ) 
  .field public uint32 cElems
  .field public native int pElems
  .custom instance void [mscorlib]System.Runtime.InteropServices.ComConversionLossAttribute::.ctor() = ( 01 00 00 00 ) 
} // end of class PortableDeviceApiLib.tagCAUB

.class public sequential ansi sealed beforefieldinit PortableDeviceApiLib.tagCAI
       extends [mscorlib]System.ValueType
{
  .pack 4
  .size 0
  .custom instance void [mscorlib]System.Runtime.InteropServices.ComConversionLossAttribute::.ctor() = ( 01 00 00 00 ) 
  .field public uint32 cElems
  .field public native int pElems
  .custom instance void [mscorlib]System.Runtime.InteropServices.ComConversionLossAttribute::.ctor() = ( 01 00 00 00 ) 
} // end of class PortableDeviceApiLib.tagCAI

.class public sequential ansi sealed beforefieldinit PortableDeviceApiLib.tagCAUI
       extends [mscorlib]System.ValueType
{
  .pack 4
  .size 0
  .custom instance void [mscorlib]System.Runtime.InteropServices.ComConversionLossAttribute::.ctor() = ( 01 00 00 00 ) 
  .field public uint32 cElems
  .field public native int pElems
  .custom instance void [mscorlib]System.Runtime.InteropServices.ComConversionLossAttribute::.ctor() = ( 01 00 00 00 ) 
} // end of class PortableDeviceApiLib.tagCAUI

.class public sequential ansi sealed beforefieldinit PortableDeviceApiLib.tagCAL
       extends [mscorlib]System.ValueType
{
  .pack 4
  .size 0
  .custom instance void [mscorlib]System.Runtime.InteropServices.ComConversionLossAttribute::.ctor() = ( 01 00 00 00 ) 
  .field public uint32 cElems
  .field public native int pElems
  .custom instance void [mscorlib]System.Runtime.InteropServices.ComConversionLossAttribute::.ctor() = ( 01 00 00 00 ) 
} // end of class PortableDeviceApiLib.tagCAL

.class public sequential ansi sealed beforefieldinit PortableDeviceApiLib.tagCAUL
       extends [mscorlib]System.ValueType
{
  .pack 4
  .size 0
  .custom instance void [mscorlib]System.Runtime.InteropServices.ComConversionLossAttribute::.ctor() = ( 01 00 00 00 ) 
  .field public uint32 cElems
  .field public native int pElems
  .custom instance void [mscorlib]System.Runtime.InteropServices.ComConversionLossAttribute::.ctor() = ( 01 00 00 00 ) 
} // end of class PortableDeviceApiLib.tagCAUL

.class public sequential ansi sealed beforefieldinit PortableDeviceApiLib.tagCAH
       extends [mscorlib]System.ValueType
{
  .pack 4
  .size 0
  .custom instance void [mscorlib]System.Runtime.InteropServices.ComConversionLossAttribute::.ctor() = ( 01 00 00 00 ) 
  .field public uint32 cElems
  .field public native int pElems
  .custom instance void [mscorlib]System.Runtime.InteropServices.ComConversionLossAttribute::.ctor() = ( 01 00 00 00 ) 
} // end of class PortableDeviceApiLib.tagCAH

.class public sequential ansi sealed beforefieldinit PortableDeviceApiLib.tagCAUH
       extends [mscorlib]System.ValueType
{
  .pack 4
  .size 0
  .custom instance void [mscorlib]System.Runtime.InteropServices.ComConversionLossAttribute::.ctor() = ( 01 00 00 00 ) 
  .field public uint32 cElems
  .field public native int pElems
  .custom instance void [mscorlib]System.Runtime.InteropServices.ComConversionLossAttribute::.ctor() = ( 01 00 00 00 ) 
} // end of class PortableDeviceApiLib.tagCAUH

.class public sequential ansi sealed beforefieldinit PortableDeviceApiLib.tagCAFLT
       extends [mscorlib]System.ValueType
{
  .pack 4
  .size 0
  .custom instance void [mscorlib]System.Runtime.InteropServices.ComConversionLossAttribute::.ctor() = ( 01 00 00 00 ) 
  .field public uint32 cElems
  .field public native int pElems
  .custom instance void [mscorlib]System.Runtime.InteropServices.ComConversionLossAttribute::.ctor() = ( 01 00 00 00 ) 
} // end of class PortableDeviceApiLib.tagCAFLT

.class public sequential ansi sealed beforefieldinit PortableDeviceApiLib.tagCADBL
       extends [mscorlib]System.ValueType
{
  .pack 4
  .size 0
  .custom instance void [mscorlib]System.Runtime.InteropServices.ComConversionLossAttribute::.ctor() = ( 01 00 00 00 ) 
  .field public uint32 cElems
  .field public native int pElems
  .custom instance void [mscorlib]System.Runtime.InteropServices.ComConversionLossAttribute::.ctor() = ( 01 00 00 00 ) 
} // end of class PortableDeviceApiLib.tagCADBL

.class public sequential ansi sealed beforefieldinit PortableDeviceApiLib.tagCABOOL
       extends [mscorlib]System.ValueType
{
  .pack 4
  .size 0
  .custom instance void [mscorlib]System.Runtime.InteropServices.ComConversionLossAttribute::.ctor() = ( 01 00 00 00 ) 
  .field public uint32 cElems
  .field public native int pElems
  .custom instance void [mscorlib]System.Runtime.InteropServices.ComConversionLossAttribute::.ctor() = ( 01 00 00 00 ) 
} // end of class PortableDeviceApiLib.tagCABOOL

.class public sequential ansi sealed beforefieldinit PortableDeviceApiLib.tagCASCODE
       extends [mscorlib]System.ValueType
{
  .pack 4
  .size 0
  .custom instance void [mscorlib]System.Runtime.InteropServices.ComConversionLossAttribute::.ctor() = ( 01 00 00 00 ) 
  .field public uint32 cElems
  .field public native int pElems
  .custom instance void [mscorlib]System.Runtime.InteropServices.ComConversionLossAttribute::.ctor() = ( 01 00 00 00 ) 
} // end of class PortableDeviceApiLib.tagCASCODE

.class public sequential ansi sealed beforefieldinit PortableDeviceApiLib.tagCACY
       extends [mscorlib]System.ValueType
{
  .pack 4
  .size 0
  .custom instance void [mscorlib]System.Runtime.InteropServices.ComConversionLossAttribute::.ctor() = ( 01 00 00 00 ) 
  .field public uint32 cElems
  .field public native int pElems
  .custom instance void [mscorlib]System.Runtime.InteropServices.ComConversionLossAttribute::.ctor() = ( 01 00 00 00 ) 
} // end of class PortableDeviceApiLib.tagCACY

.class public sequential ansi sealed beforefieldinit PortableDeviceApiLib.tagCADATE
       extends [mscorlib]System.ValueType
{
  .pack 4
  .size 0
  .custom instance void [mscorlib]System.Runtime.InteropServices.ComConversionLossAttribute::.ctor() = ( 01 00 00 00 ) 
  .field public uint32 cElems
  .field public native int pElems
  .custom instance void [mscorlib]System.Runtime.InteropServices.ComConversionLossAttribute::.ctor() = ( 01 00 00 00 ) 
} // end of class PortableDeviceApiLib.tagCADATE

.class public sequential ansi sealed beforefieldinit PortableDeviceApiLib.tagCAFILETIME
       extends [mscorlib]System.ValueType
{
  .pack 4
  .size 0
  .custom instance void [mscorlib]System.Runtime.InteropServices.ComConversionLossAttribute::.ctor() = ( 01 00 00 00 ) 
  .field public uint32 cElems
  .field public native int pElems
  .custom instance void [mscorlib]System.Runtime.InteropServices.ComConversionLossAttribute::.ctor() = ( 01 00 00 00 ) 
} // end of class PortableDeviceApiLib.tagCAFILETIME

.class public sequential ansi sealed beforefieldinit PortableDeviceApiLib.tagCACLSID
       extends [mscorlib]System.ValueType
{
  .pack 4
  .size 0
  .custom instance void [mscorlib]System.Runtime.InteropServices.ComConversionLossAttribute::.ctor() = ( 01 00 00 00 ) 
  .field public uint32 cElems
  .field public native int pElems
  .custom instance void [mscorlib]System.Runtime.InteropServices.ComConversionLossAttribute::.ctor() = ( 01 00 00 00 ) 
} // end of class PortableDeviceApiLib.tagCACLSID

.class public sequential ansi sealed beforefieldinit PortableDeviceApiLib.tagCACLIPDATA
       extends [mscorlib]System.ValueType
{
  .pack 4
  .size 0
  .custom instance void [mscorlib]System.Runtime.InteropServices.ComConversionLossAttribute::.ctor() = ( 01 00 00 00 ) 
  .field public uint32 cElems
  .field public native int pElems
  .custom instance void [mscorlib]System.Runtime.InteropServices.ComConversionLossAttribute::.ctor() = ( 01 00 00 00 ) 
} // end of class PortableDeviceApiLib.tagCACLIPDATA

.class public sequential ansi sealed beforefieldinit PortableDeviceApiLib.tagCABSTR
       extends [mscorlib]System.ValueType
{
  .pack 4
  .size 0
  .custom instance void [mscorlib]System.Runtime.InteropServices.ComConversionLossAttribute::.ctor() = ( 01 00 00 00 ) 
  .field public uint32 cElems
  .field public native int pElems
  .custom instance void [mscorlib]System.Runtime.InteropServices.ComConversionLossAttribute::.ctor() = ( 01 00 00 00 ) 
} // end of class PortableDeviceApiLib.tagCABSTR

.class public sequential ansi sealed beforefieldinit PortableDeviceApiLib.tagCABSTRBLOB
       extends [mscorlib]System.ValueType
{
  .pack 4
  .size 0
  .custom instance void [mscorlib]System.Runtime.InteropServices.ComConversionLossAttribute::.ctor() = ( 01 00 00 00 ) 
  .field public uint32 cElems
  .field public native int pElems
  .custom instance void [mscorlib]System.Runtime.InteropServices.ComConversionLossAttribute::.ctor() = ( 01 00 00 00 ) 
} // end of class PortableDeviceApiLib.tagCABSTRBLOB

.class public sequential ansi sealed beforefieldinit PortableDeviceApiLib.tagCALPSTR
       extends [mscorlib]System.ValueType
{
  .pack 4
  .size 0
  .custom instance void [mscorlib]System.Runtime.InteropServices.ComConversionLossAttribute::.ctor() = ( 01 00 00 00 ) 
  .field public uint32 cElems
  .field public native int pElems
  .custom instance void [mscorlib]System.Runtime.InteropServices.ComConversionLossAttribute::.ctor() = ( 01 00 00 00 ) 
} // end of class PortableDeviceApiLib.tagCALPSTR

.class public sequential ansi sealed beforefieldinit PortableDeviceApiLib.tagCALPWSTR
       extends [mscorlib]System.ValueType
{
  .pack 4
  .size 0
  .custom instance void [mscorlib]System.Runtime.InteropServices.ComConversionLossAttribute::.ctor() = ( 01 00 00 00 ) 
  .field public uint32 cElems
  .field public native int pElems
  .custom instance void [mscorlib]System.Runtime.InteropServices.ComConversionLossAttribute::.ctor() = ( 01 00 00 00 ) 
} // end of class PortableDeviceApiLib.tagCALPWSTR

.class public sequential ansi sealed beforefieldinit PortableDeviceApiLib.tagCAPROPVARIANT
       extends [mscorlib]System.ValueType
{
  .pack 4
  .size 0
  .custom instance void [mscorlib]System.Runtime.InteropServices.ComConversionLossAttribute::.ctor() = ( 01 00 00 00 ) 
  .field public uint32 cElems
  .field public native int pElems
  .custom instance void [mscorlib]System.Runtime.InteropServices.ComConversionLossAttribute::.ctor() = ( 01 00 00 00 ) 
} // end of class PortableDeviceApiLib.tagCAPROPVARIANT

.class interface public abstract auto ansi import PortableDeviceApiLib.IPortableDevicePropVariantCollection
{
  .custom instance void [mscorlib]System.Runtime.InteropServices.GuidAttribute::.ctor(string) = ( 01 00 24 38 39 42 32 45 34 32 32 2D 34 46 31 42   // ..$89B2E422-4F1B
                                                                                                  2D 34 33 31 36 2D 42 43 45 46 2D 41 34 34 41 46   // -4316-BCEF-A44AF
                                                                                                  45 41 38 33 45 42 33 00 00 )                      // EA83EB3..
  .custom instance void [mscorlib]System.Runtime.InteropServices.InterfaceTypeAttribute::.ctor(int16) = ( 01 00 01 00 00 00 ) 
  .method public hidebysig newslot abstract virtual 
          instance void  GetCount([in] uint32& pcElems) runtime managed internalcall
  {
  } // end of method IPortableDevicePropVariantCollection::GetCount

  .method public hidebysig newslot abstract virtual 
          instance void  GetAt([in] uint32 dwIndex,
                               [in] valuetype PortableDeviceApiLib.tag_inner_PROPVARIANT& pValue) runtime managed internalcall
  {
  } // end of method IPortableDevicePropVariantCollection::GetAt

  .method public hidebysig newslot abstract virtual 
          instance void  Add([in] valuetype PortableDeviceApiLib.tag_inner_PROPVARIANT& pValue) runtime managed internalcall
  {
  } // end of method IPortableDevicePropVariantCollection::Add

  .method public hidebysig newslot abstract virtual 
          instance void  GetType([out] uint16& pvt) runtime managed internalcall
  {
  } // end of method IPortableDevicePropVariantCollection::GetType

  .method public hidebysig newslot abstract virtual 
          instance void  ChangeType([in] uint16 vt) runtime managed internalcall
  {
  } // end of method IPortableDevicePropVariantCollection::ChangeType

  .method public hidebysig newslot abstract virtual 
          instance void  Clear() runtime managed internalcall
  {
  } // end of method IPortableDevicePropVariantCollection::Clear

  .method public hidebysig newslot abstract virtual 
          instance void  RemoveAt([in] uint32 dwIndex) runtime managed internalcall
  {
  } // end of method IPortableDevicePropVariantCollection::RemoveAt

} // end of class PortableDeviceApiLib.IPortableDevicePropVariantCollection

.class interface public abstract auto ansi import PortableDeviceApiLib.IPortableDeviceKeyCollection
{
  .custom instance void [mscorlib]System.Runtime.InteropServices.InterfaceTypeAttribute::.ctor(int16) = ( 01 00 01 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.InteropServices.GuidAttribute::.ctor(string) = ( 01 00 24 44 41 44 41 32 33 35 37 2D 45 30 41 44   // ..$DADA2357-E0AD
                                                                                                  2D 34 39 32 45 2D 39 38 44 42 2D 44 44 36 31 43   // -492E-98DB-DD61C
                                                                                                  35 33 42 41 33 35 33 00 00 )                      // 53BA353..
  .method public hidebysig newslot abstract virtual 
          instance void  GetCount([in] uint32& pcElems) runtime managed internalcall
  {
  } // end of method IPortableDeviceKeyCollection::GetCount

  .method public hidebysig newslot abstract virtual 
          instance void  GetAt([in] uint32 dwIndex,
                               [in] valuetype PortableDeviceApiLib._tagpropertykey& pKey) runtime managed internalcall
  {
  } // end of method IPortableDeviceKeyCollection::GetAt

  .method public hidebysig newslot abstract virtual 
          instance void  Add([in] valuetype PortableDeviceApiLib._tagpropertykey& key) runtime managed internalcall
  {
  } // end of method IPortableDeviceKeyCollection::Add

  .method public hidebysig newslot abstract virtual 
          instance void  Clear() runtime managed internalcall
  {
  } // end of method IPortableDeviceKeyCollection::Clear

  .method public hidebysig newslot abstract virtual 
          instance void  RemoveAt([in] uint32 dwIndex) runtime managed internalcall
  {
  } // end of method IPortableDeviceKeyCollection::RemoveAt

} // end of class PortableDeviceApiLib.IPortableDeviceKeyCollection

.class interface public abstract auto ansi import PortableDeviceApiLib.IPortableDeviceValuesCollection
{
  .custom instance void [mscorlib]System.Runtime.InteropServices.GuidAttribute::.ctor(string) = ( 01 00 24 36 45 33 46 32 44 37 39 2D 34 45 30 37   // ..$6E3F2D79-4E07
                                                                                                  2D 34 38 43 34 2D 38 32 30 38 2D 44 38 43 32 45   // -48C4-8208-D8C2E
                                                                                                  35 41 46 34 41 39 39 00 00 )                      // 5AF4A99..
  .custom instance void [mscorlib]System.Runtime.InteropServices.InterfaceTypeAttribute::.ctor(int16) = ( 01 00 01 00 00 00 ) 
  .method public hidebysig newslot abstract virtual 
          instance void  GetCount([in] uint32& pcElems) runtime managed internalcall
  {
  } // end of method IPortableDeviceValuesCollection::GetCount

  .method public hidebysig newslot abstract virtual 
          instance void  GetAt([in] uint32 dwIndex,
                               [out] class PortableDeviceApiLib.IPortableDeviceValues&  marshal( interface ) ppValues) runtime managed internalcall
  {
  } // end of method IPortableDeviceValuesCollection::GetAt

  .method public hidebysig newslot abstract virtual 
          instance void  Add([in] class PortableDeviceApiLib.IPortableDeviceValues  marshal( interface ) pValues) runtime managed internalcall
  {
  } // end of method IPortableDeviceValuesCollection::Add

  .method public hidebysig newslot abstract virtual 
          instance void  Clear() runtime managed internalcall
  {
  } // end of method IPortableDeviceValuesCollection::Clear

  .method public hidebysig newslot abstract virtual 
          instance void  RemoveAt([in] uint32 dwIndex) runtime managed internalcall
  {
  } // end of method IPortableDeviceValuesCollection::RemoveAt

} // end of class PortableDeviceApiLib.IPortableDeviceValuesCollection

.class interface public abstract auto ansi import PortableDeviceApiLib.IPropertyStore
{
  .custom instance void [mscorlib]System.Runtime.InteropServices.GuidAttribute::.ctor(string) = ( 01 00 24 38 38 36 44 38 45 45 42 2D 38 43 46 32   // ..$886D8EEB-8CF2
                                                                                                  2D 34 34 34 36 2D 38 44 30 32 2D 43 44 42 41 31   // -4446-8D02-CDBA1
                                                                                                  44 42 44 43 46 39 39 00 00 )                      // DBDCF99..
  .custom instance void [mscorlib]System.Runtime.InteropServices.InterfaceTypeAttribute::.ctor(int16) = ( 01 00 01 00 00 00 ) 
  .method public hidebysig newslot abstract virtual 
          instance void  GetCount([out] uint32& cProps) runtime managed internalcall
  {
  } // end of method IPropertyStore::GetCount

  .method public hidebysig newslot abstract virtual 
          instance void  GetAt([in] uint32 iProp,
                               [out] valuetype PortableDeviceApiLib._tagpropertykey& pKey) runtime managed internalcall
  {
  } // end of method IPropertyStore::GetAt

  .method public hidebysig newslot abstract virtual 
          instance void  GetValue([in] valuetype PortableDeviceApiLib._tagpropertykey& key,
                                  [out] valuetype PortableDeviceApiLib.tag_inner_PROPVARIANT& pv) runtime managed internalcall
  {
  } // end of method IPropertyStore::GetValue

  .method public hidebysig newslot abstract virtual 
          instance void  SetValue([in] valuetype PortableDeviceApiLib._tagpropertykey& key,
                                  [in] valuetype PortableDeviceApiLib.tag_inner_PROPVARIANT& propvar) runtime managed internalcall
  {
  } // end of method IPropertyStore::SetValue

  .method public hidebysig newslot abstract virtual 
          instance void  Commit() runtime managed internalcall
  {
  } // end of method IPropertyStore::Commit

} // end of class PortableDeviceApiLib.IPropertyStore

.class interface public abstract auto ansi import PortableDeviceApiLib.IPortableDeviceContent
{
  .custom instance void [mscorlib]System.Runtime.InteropServices.InterfaceTypeAttribute::.ctor(int16) = ( 01 00 01 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.InteropServices.GuidAttribute::.ctor(string) = ( 01 00 24 36 41 39 36 45 44 38 34 2D 37 43 37 33   // ..$6A96ED84-7C73
                                                                                                  2D 34 34 38 30 2D 39 39 33 38 2D 42 46 35 41 46   // -4480-9938-BF5AF
                                                                                                  34 37 37 44 34 32 36 00 00 )                      // 477D426..
  .method public hidebysig newslot abstract virtual 
          instance void  EnumObjects([in] uint32 dwFlags,
                                     [in] string  marshal( lpwstr) pszParentObjectID,
                                     [in] class PortableDeviceApiLib.IPortableDeviceValues  marshal( interface ) pFilter,
                                     [out] class PortableDeviceApiLib.IEnumPortableDeviceObjectIDs&  marshal( interface ) ppenum) runtime managed internalcall
  {
  } // end of method IPortableDeviceContent::EnumObjects

  .method public hidebysig newslot abstract virtual 
          instance void  Properties([out] class PortableDeviceApiLib.IPortableDeviceProperties&  marshal( interface ) ppProperties) runtime managed internalcall
  {
  } // end of method IPortableDeviceContent::Properties

  .method public hidebysig newslot abstract virtual 
          instance void  Transfer([out] class PortableDeviceApiLib.IPortableDeviceResources&  marshal( interface ) ppResources) runtime managed internalcall
  {
  } // end of method IPortableDeviceContent::Transfer

  .method public hidebysig newslot abstract virtual 
          instance void  CreateObjectWithPropertiesOnly([in] class PortableDeviceApiLib.IPortableDeviceValues  marshal( interface ) pValues,
                                                        [in][out] string&  marshal( lpwstr) ppszObjectID) runtime managed internalcall
  {
  } // end of method IPortableDeviceContent::CreateObjectWithPropertiesOnly

  .method public hidebysig newslot abstract virtual 
          instance void  CreateObjectWithPropertiesAndData([in] class PortableDeviceApiLib.IPortableDeviceValues  marshal( interface ) pValues,
                                                           [out] class PortableDeviceApiLib.IStream&  marshal( interface ) ppData,
                                                           [in][out] uint32& pdwOptimalWriteBufferSize,
                                                           [in][out] string&  marshal( lpwstr) ppszCookie) runtime managed internalcall
  {
  } // end of method IPortableDeviceContent::CreateObjectWithPropertiesAndData

  .method public hidebysig newslot abstract virtual 
          instance void  Delete([in] uint32 dwOptions,
                                [in] class PortableDeviceApiLib.IPortableDevicePropVariantCollection  marshal( interface ) pObjectIDs,
                                [in][out] class PortableDeviceApiLib.IPortableDevicePropVariantCollection&  marshal( interface ) ppResults) runtime managed internalcall
  {
  } // end of method IPortableDeviceContent::Delete

  .method public hidebysig newslot abstract virtual 
          instance void  GetObjectIDsFromPersistentUniqueIDs([in] class PortableDeviceApiLib.IPortableDevicePropVariantCollection  marshal( interface ) pPersistentUniqueIDs,
                                                             [out] class PortableDeviceApiLib.IPortableDevicePropVariantCollection&  marshal( interface ) ppObjectIDs) runtime managed internalcall
  {
  } // end of method IPortableDeviceContent::GetObjectIDsFromPersistentUniqueIDs

  .method public hidebysig newslot abstract virtual 
          instance void  Cancel() runtime managed internalcall
  {
  } // end of method IPortableDeviceContent::Cancel

  .method public hidebysig newslot abstract virtual 
          instance void  Move([in] class PortableDeviceApiLib.IPortableDevicePropVariantCollection  marshal( interface ) pObjectIDs,
                              [in] string  marshal( lpwstr) pszDestinationFolderObjectID,
                              [in][out] class PortableDeviceApiLib.IPortableDevicePropVariantCollection&  marshal( interface ) ppResults) runtime managed internalcall
  {
  } // end of method IPortableDeviceContent::Move

  .method public hidebysig newslot abstract virtual 
          instance void  Copy([in] class PortableDeviceApiLib.IPortableDevicePropVariantCollection  marshal( interface ) pObjectIDs,
                              [in] string  marshal( lpwstr) pszDestinationFolderObjectID,
                              [in][out] class PortableDeviceApiLib.IPortableDevicePropVariantCollection&  marshal( interface ) ppResults) runtime managed internalcall
  {
  } // end of method IPortableDeviceContent::Copy

} // end of class PortableDeviceApiLib.IPortableDeviceContent

.class interface public abstract auto ansi import PortableDeviceApiLib.IEnumPortableDeviceObjectIDs
{
  .custom instance void [mscorlib]System.Runtime.InteropServices.GuidAttribute::.ctor(string) = ( 01 00 24 31 30 45 43 45 39 35 35 2D 43 46 34 31   // ..$10ECE955-CF41
                                                                                                  2D 34 37 32 38 2D 42 46 41 30 2D 34 31 45 45 44   // -4728-BFA0-41EED
                                                                                                  46 31 42 42 46 31 39 00 00 )                      // F1BBF19..
  .custom instance void [mscorlib]System.Runtime.InteropServices.InterfaceTypeAttribute::.ctor(int16) = ( 01 00 01 00 00 00 ) 
  .method public hidebysig newslot abstract virtual 
          instance void  Next([in] uint32 cObjects,
                              [out] string&  marshal( lpwstr) pObjIDs,
                              [in][out] uint32& pcFetched) runtime managed internalcall
  {
  } // end of method IEnumPortableDeviceObjectIDs::Next

  .method public hidebysig newslot abstract virtual 
          instance void  Skip([in] uint32 cObjects) runtime managed internalcall
  {
  } // end of method IEnumPortableDeviceObjectIDs::Skip

  .method public hidebysig newslot abstract virtual 
          instance void  Reset() runtime managed internalcall
  {
  } // end of method IEnumPortableDeviceObjectIDs::Reset

  .method public hidebysig newslot abstract virtual 
          instance void  Clone([out] class PortableDeviceApiLib.IEnumPortableDeviceObjectIDs&  marshal( interface ) ppenum) runtime managed internalcall
  {
  } // end of method IEnumPortableDeviceObjectIDs::Clone

  .method public hidebysig newslot abstract virtual 
          instance void  Cancel() runtime managed internalcall
  {
  } // end of method IEnumPortableDeviceObjectIDs::Cancel

} // end of class PortableDeviceApiLib.IEnumPortableDeviceObjectIDs

.class interface public abstract auto ansi import PortableDeviceApiLib.IPortableDeviceProperties
{
  .custom instance void [mscorlib]System.Runtime.InteropServices.InterfaceTypeAttribute::.ctor(int16) = ( 01 00 01 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.InteropServices.GuidAttribute::.ctor(string) = ( 01 00 24 37 46 36 44 36 39 35 43 2D 30 33 44 46   // ..$7F6D695C-03DF
                                                                                                  2D 34 34 33 39 2D 41 38 30 39 2D 35 39 32 36 36   // -4439-A809-59266
                                                                                                  42 45 45 45 33 41 36 00 00 )                      // BEEE3A6..
  .method public hidebysig newslot abstract virtual 
          instance void  GetSupportedProperties([in] string  marshal( lpwstr) pszObjectID,
                                                [out] class PortableDeviceApiLib.IPortableDeviceKeyCollection&  marshal( interface ) ppKeys) runtime managed internalcall
  {
  } // end of method IPortableDeviceProperties::GetSupportedProperties

  .method public hidebysig newslot abstract virtual 
          instance void  GetPropertyAttributes([in] string  marshal( lpwstr) pszObjectID,
                                               [in] valuetype PortableDeviceApiLib._tagpropertykey& key,
                                               [out] class PortableDeviceApiLib.IPortableDeviceValues&  marshal( interface ) ppAttributes) runtime managed internalcall
  {
  } // end of method IPortableDeviceProperties::GetPropertyAttributes

  .method public hidebysig newslot abstract virtual 
          instance void  GetValues([in] string  marshal( lpwstr) pszObjectID,
                                   [in] class PortableDeviceApiLib.IPortableDeviceKeyCollection  marshal( interface ) pKeys,
                                   [out] class PortableDeviceApiLib.IPortableDeviceValues&  marshal( interface ) ppValues) runtime managed internalcall
  {
  } // end of method IPortableDeviceProperties::GetValues

  .method public hidebysig newslot abstract virtual 
          instance void  SetValues([in] string  marshal( lpwstr) pszObjectID,
                                   [in] class PortableDeviceApiLib.IPortableDeviceValues  marshal( interface ) pValues,
                                   [out] class PortableDeviceApiLib.IPortableDeviceValues&  marshal( interface ) ppResults) runtime managed internalcall
  {
  } // end of method IPortableDeviceProperties::SetValues

  .method public hidebysig newslot abstract virtual 
          instance void  Delete([in] string  marshal( lpwstr) pszObjectID,
                                [in] class PortableDeviceApiLib.IPortableDeviceKeyCollection  marshal( interface ) pKeys) runtime managed internalcall
  {
  } // end of method IPortableDeviceProperties::Delete

  .method public hidebysig newslot abstract virtual 
          instance void  Cancel() runtime managed internalcall
  {
  } // end of method IPortableDeviceProperties::Cancel

} // end of class PortableDeviceApiLib.IPortableDeviceProperties

.class interface public abstract auto ansi import PortableDeviceApiLib.IPortableDeviceResources
{
  .custom instance void [mscorlib]System.Runtime.InteropServices.GuidAttribute::.ctor(string) = ( 01 00 24 46 44 38 38 37 38 41 43 2D 44 38 34 31   // ..$FD8878AC-D841
                                                                                                  2D 34 44 31 37 2D 38 39 31 43 2D 45 36 38 32 39   // -4D17-891C-E6829
                                                                                                  43 44 42 36 39 33 34 00 00 )                      // CDB6934..
  .custom instance void [mscorlib]System.Runtime.InteropServices.InterfaceTypeAttribute::.ctor(int16) = ( 01 00 01 00 00 00 ) 
  .method public hidebysig newslot abstract virtual 
          instance void  GetSupportedResources([in] string  marshal( lpwstr) pszObjectID,
                                               [out] class PortableDeviceApiLib.IPortableDeviceKeyCollection&  marshal( interface ) ppKeys) runtime managed internalcall
  {
  } // end of method IPortableDeviceResources::GetSupportedResources

  .method public hidebysig newslot abstract virtual 
          instance void  GetResourceAttributes([in] string  marshal( lpwstr) pszObjectID,
                                               [in] valuetype PortableDeviceApiLib._tagpropertykey& key,
                                               [out] class PortableDeviceApiLib.IPortableDeviceValues&  marshal( interface ) ppResourceAttributes) runtime managed internalcall
  {
  } // end of method IPortableDeviceResources::GetResourceAttributes

  .method public hidebysig newslot abstract virtual 
          instance void  GetStream([in] string  marshal( lpwstr) pszObjectID,
                                   [in] valuetype PortableDeviceApiLib._tagpropertykey& key,
                                   [in] uint32 dwMode,
                                   [in][out] uint32& pdwOptimalBufferSize,
                                   [out] class PortableDeviceApiLib.IStream&  marshal( interface ) ppStream) runtime managed internalcall
  {
  } // end of method IPortableDeviceResources::GetStream

  .method public hidebysig newslot abstract virtual 
          instance void  Delete([in] string  marshal( lpwstr) pszObjectID,
                                [in] class PortableDeviceApiLib.IPortableDeviceKeyCollection  marshal( interface ) pKeys) runtime managed internalcall
  {
  } // end of method IPortableDeviceResources::Delete

  .method public hidebysig newslot abstract virtual 
          instance void  Cancel() runtime managed internalcall
  {
  } // end of method IPortableDeviceResources::Cancel

  .method public hidebysig newslot abstract virtual 
          instance void  CreateResource([in] class PortableDeviceApiLib.IPortableDeviceValues  marshal( interface ) pResourceAttributes,
                                        [out] class PortableDeviceApiLib.IStream&  marshal( interface ) ppData,
                                        [in][out] uint32& pdwOptimalWriteBufferSize,
                                        [in][out] string&  marshal( lpwstr) ppszCookie) runtime managed internalcall
  {
  } // end of method IPortableDeviceResources::CreateResource

} // end of class PortableDeviceApiLib.IPortableDeviceResources

.class interface public abstract auto ansi import PortableDeviceApiLib.IPortableDeviceCapabilities
{
  .custom instance void [mscorlib]System.Runtime.InteropServices.InterfaceTypeAttribute::.ctor(int16) = ( 01 00 01 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.InteropServices.GuidAttribute::.ctor(string) = ( 01 00 24 32 43 38 43 36 44 42 46 2D 45 33 44 43   // ..$2C8C6DBF-E3DC
                                                                                                  2D 34 30 36 31 2D 42 45 43 43 2D 38 35 34 32 45   // -4061-BECC-8542E
                                                                                                  38 31 30 44 31 32 36 00 00 )                      // 810D126..
  .method public hidebysig newslot abstract virtual 
          instance void  GetSupportedCommands([out] class PortableDeviceApiLib.IPortableDeviceKeyCollection&  marshal( interface ) ppCommands) runtime managed internalcall
  {
  } // end of method IPortableDeviceCapabilities::GetSupportedCommands

  .method public hidebysig newslot abstract virtual 
          instance void  GetCommandOptions([in] valuetype PortableDeviceApiLib._tagpropertykey& Command,
                                           [out] class PortableDeviceApiLib.IPortableDeviceValues&  marshal( interface ) ppOptions) runtime managed internalcall
  {
  } // end of method IPortableDeviceCapabilities::GetCommandOptions

  .method public hidebysig newslot abstract virtual 
          instance void  GetFunctionalCategories([out] class PortableDeviceApiLib.IPortableDevicePropVariantCollection&  marshal( interface ) ppCategories) runtime managed internalcall
  {
  } // end of method IPortableDeviceCapabilities::GetFunctionalCategories

  .method public hidebysig newslot abstract virtual 
          instance void  GetFunctionalObjects([in] valuetype [mscorlib]System.Guid& Category,
                                              [out] class PortableDeviceApiLib.IPortableDevicePropVariantCollection&  marshal( interface ) ppObjectIDs) runtime managed internalcall
  {
  } // end of method IPortableDeviceCapabilities::GetFunctionalObjects

  .method public hidebysig newslot abstract virtual 
          instance void  GetSupportedContentTypes([in] valuetype [mscorlib]System.Guid& Category,
                                                  [out] class PortableDeviceApiLib.IPortableDevicePropVariantCollection&  marshal( interface ) ppContentTypes) runtime managed internalcall
  {
  } // end of method IPortableDeviceCapabilities::GetSupportedContentTypes

  .method public hidebysig newslot abstract virtual 
          instance void  GetSupportedFormats([in] valuetype [mscorlib]System.Guid& ContentType,
                                             [out] class PortableDeviceApiLib.IPortableDevicePropVariantCollection&  marshal( interface ) ppFormats) runtime managed internalcall
  {
  } // end of method IPortableDeviceCapabilities::GetSupportedFormats

  .method public hidebysig newslot abstract virtual 
          instance void  GetSupportedFormatProperties([in] valuetype [mscorlib]System.Guid& Format,
                                                      [out] class PortableDeviceApiLib.IPortableDeviceKeyCollection&  marshal( interface ) ppKeys) runtime managed internalcall
  {
  } // end of method IPortableDeviceCapabilities::GetSupportedFormatProperties

  .method public hidebysig newslot abstract virtual 
          instance void  GetFixedPropertyAttributes([in] valuetype [mscorlib]System.Guid& Format,
                                                    [in] valuetype PortableDeviceApiLib._tagpropertykey& key,
                                                    [out] class PortableDeviceApiLib.IPortableDeviceValues&  marshal( interface ) ppAttributes) runtime managed internalcall
  {
  } // end of method IPortableDeviceCapabilities::GetFixedPropertyAttributes

  .method public hidebysig newslot abstract virtual 
          instance void  Cancel() runtime managed internalcall
  {
  } // end of method IPortableDeviceCapabilities::Cancel

  .method public hidebysig newslot abstract virtual 
          instance void  GetSupportedEvents([out] class PortableDeviceApiLib.IPortableDevicePropVariantCollection&  marshal( interface ) ppEvents) runtime managed internalcall
  {
  } // end of method IPortableDeviceCapabilities::GetSupportedEvents

  .method public hidebysig newslot abstract virtual 
          instance void  GetEventOptions([in] valuetype [mscorlib]System.Guid& Event,
                                         [out] class PortableDeviceApiLib.IPortableDeviceValues&  marshal( interface ) ppOptions) runtime managed internalcall
  {
  } // end of method IPortableDeviceCapabilities::GetEventOptions

} // end of class PortableDeviceApiLib.IPortableDeviceCapabilities

.class interface public abstract auto ansi import PortableDeviceApiLib.IPortableDeviceEventCallback
{
  .custom instance void [mscorlib]System.Runtime.InteropServices.InterfaceTypeAttribute::.ctor(int16) = ( 01 00 01 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.InteropServices.GuidAttribute::.ctor(string) = ( 01 00 24 41 38 37 39 32 41 33 31 2D 46 33 38 35   // ..$A8792A31-F385
                                                                                                  2D 34 39 33 43 2D 41 38 39 33 2D 34 30 46 36 34   // -493C-A893-40F64
                                                                                                  45 42 34 35 46 36 45 00 00 )                      // EB45F6E..
  .method public hidebysig newslot abstract virtual 
          instance void  OnEvent([in] class PortableDeviceApiLib.IPortableDeviceValues  marshal( interface ) pEventParameters) runtime managed internalcall
  {
  } // end of method IPortableDeviceEventCallback::OnEvent

} // end of class PortableDeviceApiLib.IPortableDeviceEventCallback

.class public auto ansi import PortableDeviceApiLib.PortableDeviceManagerClass
       extends [mscorlib]System.Object
       implements PortableDeviceApiLib.IPortableDeviceManager,
                  PortableDeviceApiLib.PortableDeviceManager
{
  .custom instance void [mscorlib]System.Runtime.InteropServices.GuidAttribute::.ctor(string) = ( 01 00 24 30 41 46 31 30 43 45 43 2D 32 45 43 44   // ..$0AF10CEC-2ECD
                                                                                                  2D 34 42 39 32 2D 39 35 38 31 2D 33 34 46 36 41   // -4B92-9581-34F6A
                                                                                                  45 30 36 33 37 46 33 00 00 )                      // E0637F3..
  .custom instance void [mscorlib]System.Runtime.InteropServices.ClassInterfaceAttribute::.ctor(int16) = ( 01 00 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.InteropServices.TypeLibTypeAttribute::.ctor(int16) = ( 01 00 02 00 00 00 ) 
  .method public specialname rtspecialname 
          instance void  .ctor() runtime managed internalcall
  {
  } // end of method PortableDeviceManagerClass::.ctor

  .method public hidebysig newslot virtual 
          instance void  GetDevices([in][out] string[] marshal(lpwstr[]) pPnPDeviceIDs,
                                    [in][out] uint32& pcPnPDeviceIDs) runtime managed internalcall
  {
    .override PortableDeviceApiLib.IPortableDeviceManager::GetDevices
  } // end of method PortableDeviceManagerClass::GetDevices

  .method public hidebysig newslot virtual 
          instance void  RefreshDeviceList() runtime managed internalcall
  {
    .override PortableDeviceApiLib.IPortableDeviceManager::RefreshDeviceList
  } // end of method PortableDeviceManagerClass::RefreshDeviceList

  .method public hidebysig newslot virtual 
          instance void  GetDeviceFriendlyName([in] string  marshal( lpwstr) pszPnPDeviceID,
                                               [in][out] uint16& pDeviceFriendlyName,
                                               [in][out] uint32& pcchDeviceFriendlyName) runtime managed internalcall
  {
    .override PortableDeviceApiLib.IPortableDeviceManager::GetDeviceFriendlyName
  } // end of method PortableDeviceManagerClass::GetDeviceFriendlyName

  .method public hidebysig newslot virtual 
          instance void  GetDeviceDescription([in] string  marshal( lpwstr) pszPnPDeviceID,
                                              [in][out] uint16& pDeviceDescription,
                                              [in][out] uint32& pcchDeviceDescription) runtime managed internalcall
  {
    .override PortableDeviceApiLib.IPortableDeviceManager::GetDeviceDescription
  } // end of method PortableDeviceManagerClass::GetDeviceDescription

  .method public hidebysig newslot virtual 
          instance void  GetDeviceManufacturer([in] string  marshal( lpwstr) pszPnPDeviceID,
                                               [in][out] uint16& pDeviceManufacturer,
                                               [in][out] uint32& pcchDeviceManufacturer) runtime managed internalcall
  {
    .override PortableDeviceApiLib.IPortableDeviceManager::GetDeviceManufacturer
  } // end of method PortableDeviceManagerClass::GetDeviceManufacturer

  .method public hidebysig newslot virtual 
          instance void  GetDeviceProperty([in] string  marshal( lpwstr) pszPnPDeviceID,
                                           [in] string  marshal( lpwstr) pszDevicePropertyName,
                                           [in][out] uint8& pData,
                                           [in][out] uint32& pcbData,
                                           [in][out] uint32& pdwType) runtime managed internalcall
  {
    .override PortableDeviceApiLib.IPortableDeviceManager::GetDeviceProperty
  } // end of method PortableDeviceManagerClass::GetDeviceProperty

  .method public hidebysig newslot virtual 
          instance void  GetPrivateDevices([in][out] string&  marshal( lpwstr) pPnPDeviceIDs,
                                           [in][out] uint32& pcPnPDeviceIDs) runtime managed internalcall
  {
    .override PortableDeviceApiLib.IPortableDeviceManager::GetPrivateDevices
  } // end of method PortableDeviceManagerClass::GetPrivateDevices

} // end of class PortableDeviceApiLib.PortableDeviceManagerClass

.class interface public abstract auto ansi import PortableDeviceApiLib.PortableDeviceManager
       implements PortableDeviceApiLib.IPortableDeviceManager
{
  .custom instance void [mscorlib]System.Runtime.InteropServices.GuidAttribute::.ctor(string) = ( 01 00 24 41 31 35 36 37 35 39 35 2D 34 43 32 46   // ..$A1567595-4C2F
                                                                                                  2D 34 35 37 34 2D 41 36 46 41 2D 45 43 45 46 39   // -4574-A6FA-ECEF9
                                                                                                  31 37 42 39 41 34 30 00 00 )                      // 17B9A40..
  .custom instance void [mscorlib]System.Runtime.InteropServices.CoClassAttribute::.ctor(class [mscorlib]System.Type) = ( 01 00 2F 50 6F 72 74 61 62 6C 65 44 65 76 69 63   // ../PortableDevic
                                                                                                                          65 41 70 69 4C 69 62 2E 50 6F 72 74 61 62 6C 65   // eApiLib.Portable
                                                                                                                          44 65 76 69 63 65 4D 61 6E 61 67 65 72 43 6C 61   // DeviceManagerCla
                                                                                                                          73 73 00 00 )                                     // ss..
} // end of class PortableDeviceApiLib.PortableDeviceManager

.class interface public abstract auto ansi import PortableDeviceApiLib.IPortableDeviceManager
{
  .custom instance void [mscorlib]System.Runtime.InteropServices.GuidAttribute::.ctor(string) = ( 01 00 24 41 31 35 36 37 35 39 35 2D 34 43 32 46   // ..$A1567595-4C2F
                                                                                                  2D 34 35 37 34 2D 41 36 46 41 2D 45 43 45 46 39   // -4574-A6FA-ECEF9
                                                                                                  31 37 42 39 41 34 30 00 00 )                      // 17B9A40..
  .custom instance void [mscorlib]System.Runtime.InteropServices.InterfaceTypeAttribute::.ctor(int16) = ( 01 00 01 00 00 00 ) 
  .method public hidebysig newslot abstract virtual 
          instance void  GetDevices([in][out] string[] marshal(lpwstr[]) pPnPDeviceIDs,
                                    [in][out] uint32& pcPnPDeviceIDs) runtime managed internalcall
  {
  } // end of method IPortableDeviceManager::GetDevices

  .method public hidebysig newslot abstract virtual 
          instance void  RefreshDeviceList() runtime managed internalcall
  {
  } // end of method IPortableDeviceManager::RefreshDeviceList

  .method public hidebysig newslot abstract virtual 
          instance void  GetDeviceFriendlyName([in] string  marshal( lpwstr) pszPnPDeviceID,
                                               [in][out] uint16& pDeviceFriendlyName,
                                               [in][out] uint32& pcchDeviceFriendlyName) runtime managed internalcall
  {
  } // end of method IPortableDeviceManager::GetDeviceFriendlyName

  .method public hidebysig newslot abstract virtual 
          instance void  GetDeviceDescription([in] string  marshal( lpwstr) pszPnPDeviceID,
                                              [in][out] uint16& pDeviceDescription,
                                              [in][out] uint32& pcchDeviceDescription) runtime managed internalcall
  {
  } // end of method IPortableDeviceManager::GetDeviceDescription

  .method public hidebysig newslot abstract virtual 
          instance void  GetDeviceManufacturer([in] string  marshal( lpwstr) pszPnPDeviceID,
                                               [in][out] uint16& pDeviceManufacturer,
                                               [in][out] uint32& pcchDeviceManufacturer) runtime managed internalcall
  {
  } // end of method IPortableDeviceManager::GetDeviceManufacturer

  .method public hidebysig newslot abstract virtual 
          instance void  GetDeviceProperty([in] string  marshal( lpwstr) pszPnPDeviceID,
                                           [in] string  marshal( lpwstr) pszDevicePropertyName,
                                           [in][out] uint8& pData,
                                           [in][out] uint32& pcbData,
                                           [in][out] uint32& pdwType) runtime managed internalcall
  {
  } // end of method IPortableDeviceManager::GetDeviceProperty

  .method public hidebysig newslot abstract virtual 
          instance void  GetPrivateDevices([in][out] string&  marshal( lpwstr) pPnPDeviceIDs,
                                           [in][out] uint32& pcPnPDeviceIDs) runtime managed internalcall
  {
  } // end of method IPortableDeviceManager::GetPrivateDevices

} // end of class PortableDeviceApiLib.IPortableDeviceManager


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
// WARNING: Created Win32 resource file pdapi2_0.res
