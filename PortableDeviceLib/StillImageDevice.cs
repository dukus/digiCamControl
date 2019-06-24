using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using PortableDeviceApiLib;

namespace PortableDeviceLib
{

    public class StillImageDevice : PortableDevice
    {
        public delegate void TransferCallback(uint total, uint current);

        public StillImageDevice(string deviceId)
            : base(deviceId)
        {

        }

        public uint ResetDevice()
        {
            IPortableDeviceValues commandValues =
                (IPortableDeviceValues) new PortableDeviceTypesLib.PortableDeviceValuesClass();
            IPortableDeviceValues results;

            commandValues.SetGuidValue(PortableDevicePKeys.WPD_PROPERTY_COMMON_COMMAND_CATEGORY,
                PortableDevicePKeys.WPD_COMMAND_COMMON_RESET_DEVICE.fmtid);
            commandValues.SetUnsignedIntegerValue(PortableDevicePKeys.WPD_PROPERTY_COMMON_COMMAND_ID,
                PortableDevicePKeys.WPD_COMMAND_COMMON_RESET_DEVICE.pid);

            this.portableDeviceClass.SendCommand(0, commandValues, out results);

            try
            {
                int iValue = 0;
                results.GetErrorValue(PortableDevicePKeys.WPD_PROPERTY_COMMON_HRESULT, out iValue);
                if (iValue != 0)
                    return (uint)iValue;
                uint pValue = 0;
                results.GetUnsignedIntegerValue(ref PortableDevicePKeys.WPD_PROPERTY_MTP_EXT_RESPONSE_CODE, out pValue);
                if (pValue != 0)
                    return pValue;
            }
            catch (Exception)
            {
            }
            return 0;
        }

        public uint ExecuteWithNoData(uint code, params uint[] parameters)
        {
            IPortableDeviceValues commandValues =
              (IPortableDeviceValues)new PortableDeviceTypesLib.PortableDeviceValuesClass();
            IPortableDevicePropVariantCollection propVariant =
              (IPortableDevicePropVariantCollection)new PortableDeviceTypesLib.PortableDevicePropVariantCollection();
            IPortableDeviceValues results;

            //commandValues.SetGuidValue(ref PortableDevicePKeys.WPD_PROPERTY_COMMON_COMMAND_CATEGORY, ref command.fmtid);
            commandValues.SetGuidValue(PortableDevicePKeys.WPD_PROPERTY_COMMON_COMMAND_CATEGORY,
                                       PortableDevicePKeys.WPD_COMMAND_MTP_EXT_EXECUTE_COMMAND_WITHOUT_DATA_PHASE.fmtid);
            commandValues.SetUnsignedIntegerValue(PortableDevicePKeys.WPD_PROPERTY_COMMON_COMMAND_ID,
                                                  PortableDevicePKeys.WPD_COMMAND_MTP_EXT_EXECUTE_COMMAND_WITHOUT_DATA_PHASE.
                                                    pid);

            foreach (uint parameter in parameters)
            {
                tag_inner_PROPVARIANT vparam = new tag_inner_PROPVARIANT();
                UintToPropVariant(parameter, out vparam);
                propVariant.Add(ref vparam);
            }

            commandValues.SetUnsignedIntegerValue(ref PortableDevicePKeys.WPD_PROPERTY_MTP_EXT_OPERATION_CODE, code);

            commandValues.SetIPortableDevicePropVariantCollectionValue(ref PortableDevicePKeys.WPD_PROPERTY_MTP_EXT_OPERATION_PARAMS, propVariant);

            // According to documentation, first parameter should be 0 (see http://msdn.microsoft.com/en-us/library/dd375691%28v=VS.85%29.aspx)
            this.portableDeviceClass.SendCommand(0, commandValues, out results);
            //int pvalue = 0;
            try
            {
                int iValue = 0;
                results.GetErrorValue(PortableDevicePKeys.WPD_PROPERTY_COMMON_HRESULT, out iValue);
                if (iValue != 0)
                    return (uint)iValue;
                uint pValue = 0;
                results.GetUnsignedIntegerValue(ref PortableDevicePKeys.WPD_PROPERTY_MTP_EXT_RESPONSE_CODE, out pValue);
                if (pValue != 0)
                    return pValue;
            }
            catch (Exception)
            {
            }
            //results.GetSignedIntegerValue(ref PortableDevicePKeys.WPD_PROPERTY_MTP_EXT_RESPONSE_CODE, out pvalue);
            return 0;
        }

        public MTPDataResponse ExecuteReadBigData(uint code,Stream stream, TransferCallback callback, params uint[] parameters)
        {
            MTPDataResponse res = new MTPDataResponse();
            // source: http://msdn.microsoft.com/en-us/library/windows/desktop/ff384843(v=vs.85).aspx
            // and view-source:http://www.experts-exchange.com/Programming/Languages/C_Sharp/Q_26860397.html
            // error codes http://msdn.microsoft.com/en-us/library/windows/desktop/dd319335(v=vs.85).aspx
            byte[] imgdate = new byte[8];

            IPortableDeviceValues commandValues = (IPortableDeviceValues)new PortableDeviceTypesLib.PortableDeviceValuesClass();
            IPortableDeviceValues pParameters = (IPortableDeviceValues)new PortableDeviceTypesLib.PortableDeviceValues();

            IPortableDevicePropVariantCollection propVariant =
              (IPortableDevicePropVariantCollection)new PortableDeviceTypesLib.PortableDevicePropVariantCollection();
            IPortableDeviceValues pResults;

            //commandValues.SetGuidValue(ref PortableDevicePKeys.WPD_PROPERTY_COMMON_COMMAND_CATEGORY, ref command.fmtid);
            commandValues.SetGuidValue(PortableDevicePKeys.WPD_PROPERTY_COMMON_COMMAND_CATEGORY,
                                             PortableDevicePKeys.WPD_COMMAND_MTP_EXT_EXECUTE_COMMAND_WITH_DATA_TO_READ.fmtid);
            commandValues.SetUnsignedIntegerValue(PortableDevicePKeys.WPD_PROPERTY_COMMON_COMMAND_ID,
                                   PortableDevicePKeys.WPD_COMMAND_MTP_EXT_EXECUTE_COMMAND_WITH_DATA_TO_READ.pid);
            commandValues.SetBufferValue(ref PortableDevicePKeys.WPD_PROPERTY_MTP_EXT_TRANSFER_DATA, ref imgdate[0], (uint)imgdate.Length);

            foreach (uint parameter in parameters)
            {
                tag_inner_PROPVARIANT vparam = new tag_inner_PROPVARIANT();
                UintToPropVariant(parameter, out vparam);
                propVariant.Add(ref vparam);

            }

            commandValues.SetIPortableDevicePropVariantCollectionValue(ref PortableDevicePKeys.WPD_PROPERTY_MTP_EXT_OPERATION_PARAMS, propVariant);
            commandValues.SetUnsignedIntegerValue(ref PortableDevicePKeys.WPD_PROPERTY_MTP_EXT_OPERATION_CODE, (uint)code);

            // According to documentation, first parameter should be 0 (see http://msdn.microsoft.com/en-us/library/dd375691%28v=VS.85%29.aspx)
            this.portableDeviceClass.SendCommand(0, commandValues, out pResults);

            try
            {
                int pValue = 0;
                pResults.GetErrorValue(PortableDevicePKeys.WPD_PROPERTY_COMMON_HRESULT, out pValue);
                if (pValue != 0)
                {
                    // check if the device is busy, and after 100 ms seconds try again 
                    if (((uint)pValue) == PortableDeviceErrorCodes.ERROR_BUSY)
                    {
                        Thread.Sleep(50);
                        return ExecuteReadBigData(code, stream, callback, parameters);
                    }
                }
            }
            catch (Exception)
            {
            }
            //string pwszContext = string.Empty;
            //pResults.GetStringValue(ref PortableDevicePKeys.WPD_PROPERTY_MTP_EXT_TRANSFER_CONTEXT, out pwszContext);
            //uint cbReportedDataSize = 0;
            //pResults.GetUnsignedIntegerValue(ref PortableDevicePKeys.WPD_PROPERTY_MTP_EXT_TRANSFER_TOTAL_DATA_SIZE, out cbReportedDataSize);


            uint tmpBufferSize = 0;
            uint tmpTransferSize = 0;
            string tmpTransferContext = string.Empty;
            {
                pResults.GetStringValue(ref PortableDevicePKeys.WPD_PROPERTY_MTP_EXT_TRANSFER_CONTEXT, out tmpTransferContext);
                pResults.GetUnsignedIntegerValue(ref PortableDevicePKeys.WPD_PROPERTY_MTP_EXT_TRANSFER_TOTAL_DATA_SIZE, out tmpBufferSize);
                pResults.GetUnsignedIntegerValue(ref PortableDevicePKeys.WPD_PROPERTY_MTP_EXT_OPTIMAL_TRANSFER_BUFFER_SIZE, out tmpTransferSize);

                try
                {
                    int pValue;
                    pResults.GetErrorValue(PortableDevicePKeys.WPD_PROPERTY_COMMON_HRESULT, out pValue);
                    if (pValue != 0)
                    {
                        return null;
                    }
                }
                catch
                {
                }
            }


            pParameters.Clear();
            pResults.Clear();
            uint offset = 0;
            res.Data = null;
            bool cont = true;

            do
            {
                if (offset + tmpTransferSize >= tmpBufferSize)
                {
                    cont = false;
                    tmpTransferSize = (uint)(tmpBufferSize - offset);
                }

                byte[] tmpData = new byte[(int)tmpTransferSize];
                pParameters.SetGuidValue(PortableDevicePKeys.WPD_PROPERTY_COMMON_COMMAND_CATEGORY,
                                         PortableDevicePKeys.WPD_COMMAND_MTP_EXT_READ_DATA.fmtid);
                pParameters.SetUnsignedIntegerValue(ref PortableDevicePKeys.WPD_PROPERTY_COMMON_COMMAND_ID,
                                                    PortableDevicePKeys.WPD_COMMAND_MTP_EXT_READ_DATA.pid);
                pParameters.SetStringValue(ref PortableDevicePKeys.WPD_PROPERTY_MTP_EXT_TRANSFER_CONTEXT, tmpTransferContext);
                pParameters.SetBufferValue(ref PortableDevicePKeys.WPD_PROPERTY_MTP_EXT_TRANSFER_DATA, ref tmpData[0],
                                           (uint)tmpTransferSize);
                pParameters.SetUnsignedIntegerValue(ref PortableDevicePKeys.WPD_PROPERTY_MTP_EXT_TRANSFER_NUM_BYTES_TO_READ,
                                                    (uint)tmpTransferSize);
                pParameters.SetIPortableDevicePropVariantCollectionValue(
                  ref PortableDevicePKeys.WPD_PROPERTY_MTP_EXT_OPERATION_PARAMS, propVariant);


                portableDeviceClass.SendCommand(0, pParameters, out pResults);


                uint cbBytesRead = 0;

                try
                {
                    int pValue = 0;
                    pResults.GetErrorValue(PortableDevicePKeys.WPD_PROPERTY_COMMON_HRESULT, out pValue);
                    if (pValue != 0)
                    {
                        res.ErrorCode = (uint)pValue;
                        return res;
                    }
                }
                catch (Exception)
                {
                }

                callback(tmpBufferSize, offset);

                GCHandle pinnedArray = GCHandle.Alloc(imgdate, GCHandleType.Pinned);
                IntPtr ptr = pinnedArray.AddrOfPinnedObject();

                uint dataread = 0;
                pResults.GetUnsignedIntegerValue(ref PortableDevicePKeys.WPD_PROPERTY_MTP_EXT_TRANSFER_NUM_BYTES_READ,
                                                 out dataread);
                pResults.GetBufferValue(ref PortableDevicePKeys.WPD_PROPERTY_MTP_EXT_TRANSFER_DATA, ptr, out cbBytesRead);

                IntPtr tmpPtr = new IntPtr(Marshal.ReadInt64(ptr));

                byte[] buffer = new byte[(int)cbBytesRead];
                Marshal.Copy(tmpPtr, buffer, 0, (int)cbBytesRead);
                stream.Write(buffer, 0, (int)cbBytesRead);


                //for (int i = 0; i < cbBytesRead; i++)
                //{
                //    res.Data[offset + i] = Marshal.ReadByte(tmpPtr, i);
                //}

                Marshal.FreeHGlobal(tmpPtr);
                pinnedArray.Free();

                offset += cbBytesRead;
            } while (cont);

            pParameters.Clear();
            pResults.Clear();
            {
                pParameters.SetGuidValue(PortableDevicePKeys.WPD_PROPERTY_COMMON_COMMAND_CATEGORY, PortableDevicePKeys.WPD_COMMAND_MTP_EXT_END_DATA_TRANSFER.fmtid);
                pParameters.SetUnsignedIntegerValue(ref PortableDevicePKeys.WPD_PROPERTY_COMMON_COMMAND_ID, PortableDevicePKeys.WPD_COMMAND_MTP_EXT_END_DATA_TRANSFER.pid);
                pParameters.SetStringValue(PortableDevicePKeys.WPD_PROPERTY_MTP_EXT_TRANSFER_CONTEXT, tmpTransferContext);
            }

            portableDeviceClass.SendCommand(0, pParameters, out pResults);

            try
            {
                int tmpResult = 0;

                pResults.GetErrorValue(ref PortableDevicePKeys.WPD_PROPERTY_COMMON_HRESULT, out tmpResult);
                if (tmpResult != 0)
                {

                }
            }
            catch
            {
            }
            return res;
        }

        public MTPDataResponse ExecuteReadData(uint code, params uint[] parameters)
        {
            IPortableDevicePropVariantCollection propVariant =
              (IPortableDevicePropVariantCollection)new PortableDeviceTypesLib.PortableDevicePropVariantCollection();

            foreach (uint parameter in parameters)
            {
                tag_inner_PROPVARIANT vparam1 = new tag_inner_PROPVARIANT();
                UintToPropVariant(parameter, out vparam1);
                propVariant.Add(ref vparam1);

            }
            return ExecuteReadData(code, propVariant);
        }

        private MTPDataResponse ExecuteReadData(uint code, IPortableDevicePropVariantCollection propVariant)
        {
            MTPDataResponse resp = new MTPDataResponse();

            // source: http://msdn.microsoft.com/en-us/library/windows/desktop/ff384843(v=vs.85).aspx
            // and view-source:http://www.experts-exchange.com/Programming/Languages/C_Sharp/Q_26860397.html
            // error codes http://msdn.microsoft.com/en-us/library/windows/desktop/dd319335(v=vs.85).aspx
            byte[] imgdate = new byte[8];

            IPortableDeviceValues commandValues = (IPortableDeviceValues)new PortableDeviceTypesLib.PortableDeviceValuesClass();
            IPortableDeviceValues pParameters = (IPortableDeviceValues)new PortableDeviceTypesLib.PortableDeviceValues();

            IPortableDeviceValues pResults;

            //commandValues.SetGuidValue(ref PortableDevicePKeys.WPD_PROPERTY_COMMON_COMMAND_CATEGORY, ref command.fmtid);
            commandValues.SetGuidValue(PortableDevicePKeys.WPD_PROPERTY_COMMON_COMMAND_CATEGORY,
                                             PortableDevicePKeys.WPD_COMMAND_MTP_EXT_EXECUTE_COMMAND_WITH_DATA_TO_READ.fmtid);
            commandValues.SetUnsignedIntegerValue(PortableDevicePKeys.WPD_PROPERTY_COMMON_COMMAND_ID,
                                   PortableDevicePKeys.WPD_COMMAND_MTP_EXT_EXECUTE_COMMAND_WITH_DATA_TO_READ.pid);
            commandValues.SetBufferValue(ref PortableDevicePKeys.WPD_PROPERTY_MTP_EXT_TRANSFER_DATA, ref imgdate[0], (uint)imgdate.Length);



            commandValues.SetIPortableDevicePropVariantCollectionValue(ref PortableDevicePKeys.WPD_PROPERTY_MTP_EXT_OPERATION_PARAMS, propVariant);
            commandValues.SetUnsignedIntegerValue(ref PortableDevicePKeys.WPD_PROPERTY_MTP_EXT_OPERATION_CODE, (uint)code);

            // According to documentation, first parameter should be 0 (see http://msdn.microsoft.com/en-us/library/dd375691%28v=VS.85%29.aspx)
            this.portableDeviceClass.SendCommand(0, commandValues, out pResults);

            try
            {
                int pValue;
                pResults.GetErrorValue(PortableDevicePKeys.WPD_PROPERTY_COMMON_HRESULT, out pValue);
                if (pValue != 0)
                {
                    resp.ErrorCode = (uint)pValue;
                    return resp;
                }
            }
            catch (Exception)
            {
            }

            uint tmpBufferSize = 0;
            uint tmpTransferSize = 0;
            string tmpTransferContext = string.Empty;
            {
                pResults.GetStringValue(ref PortableDevicePKeys.WPD_PROPERTY_MTP_EXT_TRANSFER_CONTEXT, out tmpTransferContext);
                pResults.GetUnsignedIntegerValue(ref PortableDevicePKeys.WPD_PROPERTY_MTP_EXT_TRANSFER_TOTAL_DATA_SIZE, out tmpBufferSize);
                pResults.GetUnsignedIntegerValue(ref PortableDevicePKeys.WPD_PROPERTY_MTP_EXT_OPTIMAL_TRANSFER_BUFFER_SIZE, out tmpTransferSize);

            }

            pParameters.Clear();
            pResults.Clear();

            byte[] tmpData = new byte[(int)tmpTransferSize];
            pParameters.SetGuidValue(PortableDevicePKeys.WPD_PROPERTY_COMMON_COMMAND_CATEGORY, PortableDevicePKeys.WPD_COMMAND_MTP_EXT_READ_DATA.fmtid);
            pParameters.SetUnsignedIntegerValue(ref PortableDevicePKeys.WPD_PROPERTY_COMMON_COMMAND_ID, PortableDevicePKeys.WPD_COMMAND_MTP_EXT_READ_DATA.pid);
            pParameters.SetStringValue(ref PortableDevicePKeys.WPD_PROPERTY_MTP_EXT_TRANSFER_CONTEXT, tmpTransferContext);
            pParameters.SetBufferValue(ref PortableDevicePKeys.WPD_PROPERTY_MTP_EXT_TRANSFER_DATA, ref tmpData[0], (uint)tmpTransferSize);
            pParameters.SetUnsignedIntegerValue(ref PortableDevicePKeys.WPD_PROPERTY_MTP_EXT_TRANSFER_NUM_BYTES_TO_READ, (uint)tmpTransferSize);
            pParameters.SetIPortableDevicePropVariantCollectionValue(ref PortableDevicePKeys.WPD_PROPERTY_MTP_EXT_OPERATION_PARAMS, propVariant);


            portableDeviceClass.SendCommand(0, pParameters, out pResults);


            uint cbBytesRead = 0;

            try
            {
                int pValue;
                pResults.GetErrorValue(PortableDevicePKeys.WPD_PROPERTY_COMMON_HRESULT, out pValue);
                if (pValue != 0)
                {
                    resp.ErrorCode = (uint)pValue;
                    return resp;
                }
            }
            catch (Exception)
            {
            }
            // 24,142,174,9
            // 18, 8E  
            GCHandle pinnedArray = GCHandle.Alloc(imgdate, GCHandleType.Pinned);
            IntPtr ptr = pinnedArray.AddrOfPinnedObject();

            uint dataread = 0;
            pResults.GetUnsignedIntegerValue(ref PortableDevicePKeys.WPD_PROPERTY_MTP_EXT_TRANSFER_NUM_BYTES_READ, out dataread);
            pResults.GetBufferValue(ref PortableDevicePKeys.WPD_PROPERTY_MTP_EXT_TRANSFER_DATA, ptr, out cbBytesRead);


            IntPtr tmpPtr = new IntPtr(Marshal.ReadInt64(ptr));
            resp.Data = new byte[(int)cbBytesRead];
            Marshal.Copy(tmpPtr, resp.Data, 0, (int)cbBytesRead);
            //for (int i = 0; i < cbBytesRead; i++)
            //{
            //    resp.Data[i] = Marshal.ReadByte(tmpPtr, i);
            //}

            Marshal.FreeHGlobal(tmpPtr);
            pinnedArray.Free();

            pParameters.Clear();
            pResults.Clear();
            {
                pParameters.SetGuidValue(PortableDevicePKeys.WPD_PROPERTY_COMMON_COMMAND_CATEGORY, PortableDevicePKeys.WPD_COMMAND_MTP_EXT_END_DATA_TRANSFER.fmtid);
                pParameters.SetUnsignedIntegerValue(ref PortableDevicePKeys.WPD_PROPERTY_COMMON_COMMAND_ID, PortableDevicePKeys.WPD_COMMAND_MTP_EXT_END_DATA_TRANSFER.pid);
                pParameters.SetStringValue(PortableDevicePKeys.WPD_PROPERTY_MTP_EXT_TRANSFER_CONTEXT, tmpTransferContext);
            }

            portableDeviceClass.SendCommand(0, pParameters, out pResults);



            try
            {
                int pValue;
                pResults.GetErrorValue(PortableDevicePKeys.WPD_PROPERTY_COMMON_HRESULT, out pValue);
                if (pValue != 0)
                {
                    resp.ErrorCode = (uint)pValue;
                    return resp;
                }

                uint iValue;
                pResults.GetUnsignedIntegerValue(ref PortableDevicePKeys.WPD_PROPERTY_MTP_EXT_RESPONSE_CODE, out iValue);
                if (iValue != 0)
                {
                    resp.ErrorCode = iValue;
                    return resp;
                }
            }
            catch
            {
            }
            return resp;
        }

        public uint ExecuteWriteData(uint code, byte[] data, params uint[] parameters)
        {
            // source: http://msdn.microsoft.com/en-us/library/windows/desktop/ff384843(v=vs.85).aspx
            // and view-source:http://www.experts-exchange.com/Programming/Languages/C_Sharp/Q_26860397.html
            // error codes http://msdn.microsoft.com/en-us/library/windows/desktop/dd319335(v=vs.85).aspx
            //byte[] imgdate = new byte[8];

            IPortableDeviceValues commandValues = (IPortableDeviceValues)new PortableDeviceTypesLib.PortableDeviceValuesClass();
            IPortableDeviceValues pParameters = (IPortableDeviceValues)new PortableDeviceTypesLib.PortableDeviceValues();

            IPortableDevicePropVariantCollection propVariant =
              (IPortableDevicePropVariantCollection)new PortableDeviceTypesLib.PortableDevicePropVariantCollection();
            IPortableDeviceValues pResults;

            //commandValues.SetGuidValue(ref PortableDevicePKeys.WPD_PROPERTY_COMMON_COMMAND_CATEGORY, ref command.fmtid);
            commandValues.SetGuidValue(PortableDevicePKeys.WPD_PROPERTY_COMMON_COMMAND_CATEGORY,
                                             PortableDevicePKeys.WPD_COMMAND_MTP_EXT_EXECUTE_COMMAND_WITH_DATA_TO_WRITE.fmtid);
            commandValues.SetUnsignedIntegerValue(PortableDevicePKeys.WPD_PROPERTY_COMMON_COMMAND_ID,
                                   PortableDevicePKeys.WPD_COMMAND_MTP_EXT_EXECUTE_COMMAND_WITH_DATA_TO_WRITE.pid);
            commandValues.SetBufferValue(ref PortableDevicePKeys.WPD_PROPERTY_MTP_EXT_TRANSFER_DATA, ref data[0], (uint)data.Length);

            foreach (uint parameter in parameters)
            {
                tag_inner_PROPVARIANT vparam = new tag_inner_PROPVARIANT();
                UintToPropVariant(parameter, out vparam);
                propVariant.Add(ref vparam);
            }

            commandValues.SetIPortableDevicePropVariantCollectionValue(ref PortableDevicePKeys.WPD_PROPERTY_MTP_EXT_OPERATION_PARAMS, propVariant);
            commandValues.SetUnsignedLargeIntegerValue(ref PortableDevicePKeys.WPD_PROPERTY_MTP_EXT_TRANSFER_TOTAL_DATA_SIZE,
                                                       (ulong)data.Length);
            commandValues.SetUnsignedIntegerValue(ref PortableDevicePKeys.WPD_PROPERTY_MTP_EXT_OPERATION_CODE, (uint)code);

            // According to documentation, first parameter should be 0 (see http://msdn.microsoft.com/en-us/library/dd375691%28v=VS.85%29.aspx)
            this.portableDeviceClass.SendCommand(0, commandValues, out pResults);

            try
            {
                int pValue = 0;
                pResults.GetErrorValue(PortableDevicePKeys.WPD_PROPERTY_COMMON_HRESULT, out pValue);
                if (pValue != 0)
                {
                    return (uint)pValue;
                }
                //uint mtperror;
                //pResults.GetUnsignedIntegerValue(ref PortableDevicePKeys.WPD_PROPERTY_MTP_EXT_RESPONSE_CODE, out mtperror);
                //if (mtperror != 0)
                //  return mtperror;
            }
            catch (Exception)
            {
            }
            string tmpTransferContext = string.Empty;
            pResults.GetStringValue(ref PortableDevicePKeys.WPD_PROPERTY_MTP_EXT_TRANSFER_CONTEXT, out tmpTransferContext);


            pParameters.Clear();
            pResults.Clear();

            pParameters.SetGuidValue(PortableDevicePKeys.WPD_PROPERTY_COMMON_COMMAND_CATEGORY, PortableDevicePKeys.WPD_COMMAND_MTP_EXT_WRITE_DATA.fmtid);
            pParameters.SetUnsignedIntegerValue(ref PortableDevicePKeys.WPD_PROPERTY_COMMON_COMMAND_ID, PortableDevicePKeys.WPD_COMMAND_MTP_EXT_WRITE_DATA.pid);
            pParameters.SetStringValue(ref PortableDevicePKeys.WPD_PROPERTY_MTP_EXT_TRANSFER_CONTEXT, tmpTransferContext);
            pParameters.SetBufferValue(ref PortableDevicePKeys.WPD_PROPERTY_MTP_EXT_TRANSFER_DATA, ref data[0], (uint)data.Length);
            pParameters.SetUnsignedIntegerValue(ref PortableDevicePKeys.WPD_PROPERTY_MTP_EXT_TRANSFER_NUM_BYTES_TO_WRITE, (uint)data.Length);
            pParameters.SetIPortableDevicePropVariantCollectionValue(ref PortableDevicePKeys.WPD_PROPERTY_MTP_EXT_OPERATION_PARAMS, propVariant);


            portableDeviceClass.SendCommand(0, pParameters, out pResults);


            try
            {
                int pValue = 0;
                pResults.GetErrorValue(PortableDevicePKeys.WPD_PROPERTY_COMMON_HRESULT, out pValue);
                if (pValue != 0)
                    return (uint)pValue;
                //uint mtperror;
                //pResults.GetUnsignedIntegerValue(ref PortableDevicePKeys.WPD_PROPERTY_MTP_EXT_RESPONSE_CODE, out mtperror);
                //if (mtperror != 0)
                //  return mtperror;
            }
            catch (Exception)
            {
            }


            pParameters.Clear();
            pResults.Clear();
            {
                pParameters.SetGuidValue(PortableDevicePKeys.WPD_PROPERTY_COMMON_COMMAND_CATEGORY, PortableDevicePKeys.WPD_COMMAND_MTP_EXT_END_DATA_TRANSFER.fmtid);
                pParameters.SetUnsignedIntegerValue(ref PortableDevicePKeys.WPD_PROPERTY_COMMON_COMMAND_ID, PortableDevicePKeys.WPD_COMMAND_MTP_EXT_END_DATA_TRANSFER.pid);
                pParameters.SetStringValue(PortableDevicePKeys.WPD_PROPERTY_MTP_EXT_TRANSFER_CONTEXT, tmpTransferContext);
            }

            portableDeviceClass.SendCommand(0, pParameters, out pResults);


            try
            {
                int tmpResult = 0;

                pResults.GetErrorValue(ref PortableDevicePKeys.WPD_PROPERTY_COMMON_HRESULT, out tmpResult);
                if (tmpResult != 0)
                {
                    return (uint)tmpResult;
                }
                uint mtperror;
                pResults.GetUnsignedIntegerValue(ref PortableDevicePKeys.WPD_PROPERTY_MTP_EXT_RESPONSE_CODE, out mtperror);
                if (mtperror != 0)
                    return mtperror;
            }
            catch
            {
            }
            return 0;
        }

        private void StringToPropVariant(string value,
            out PortableDeviceApiLib.tag_inner_PROPVARIANT propvarValue)
        {
            // We'll use an IPortableDeviceValues object to transform the
            // string into a PROPVARIANT
            PortableDeviceApiLib.IPortableDeviceValues pValues =
                (PortableDeviceApiLib.IPortableDeviceValues)
                    new PortableDeviceTypesLib.PortableDeviceValuesClass();

            // We insert the string value into the IPortableDeviceValues object
            // using the SetStringValue method
            pValues.SetStringValue(ref PortableDevicePKeys.WPD_OBJECT_ID, value);

            // We then extract the string into a PROPVARIANT by using the 
            // GetValue method
            pValues.GetValue(ref PortableDevicePKeys.WPD_OBJECT_ID,
                                        out propvarValue);
        }

        private void UintToPropVariant(uint value,
        out PortableDeviceApiLib.tag_inner_PROPVARIANT propvarValue)
        {
            // We'll use an IPortableDeviceValues object to transform the
            // string into a PROPVARIANT
            PortableDeviceApiLib.IPortableDeviceValues pValues =
                (PortableDeviceApiLib.IPortableDeviceValues)
                    new PortableDeviceTypesLib.PortableDeviceValuesClass();

            // We insert the string value into the IPortableDeviceValues object
            // using the SetStringValue method
            pValues.SetUnsignedIntegerValue(ref PortableDevicePKeys.WPD_OBJECT_ID, (uint)value);

            // We then extract the string into a PROPVARIANT by using the 
            // GetValue method
            pValues.GetValue(ref PortableDevicePKeys.WPD_OBJECT_ID,
                                        out propvarValue);
        }

        private void uLongIntToPropVariant(ulong value, out PortableDeviceApiLib.tag_inner_PROPVARIANT propvarValue)
        {
            // We'll use an IPortableDeviceValues object to transform the
            // string into a PROPVARIANT
            PortableDeviceApiLib.IPortableDeviceValues pValues =
                (PortableDeviceApiLib.IPortableDeviceValues)
                    new PortableDeviceTypesLib.PortableDeviceValuesClass();

            // We insert the string value into the IPortableDeviceValues object
            // using the SetStringValue method
            pValues.SetUnsignedLargeIntegerValue(ref PortableDevicePKeys.WPD_OBJECT_ID, (ulong)value);

            // We then extract the string into a PROPVARIANT by using the 
            // GetValue method
            pValues.GetValue(ref PortableDevicePKeys.WPD_OBJECT_ID,
                                        out propvarValue);
        }
    }
}
