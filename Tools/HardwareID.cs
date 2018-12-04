using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace EnroladorStandAlone
{
    public class HardwareID
    {
        #region CPUId
        [ReadOnly(true)]
        static string CPUId
        {
            get
            {
                //Uses first CPU identifier available in order of preference
                //Don't get all identifiers, as it is very time consuming
                string result = GetId("Win32_Processor", "UniqueId");
                if (string.IsNullOrEmpty(result))//If no UniqueID, use ProcessorID
                {
                    result = GetId("Win32_Processor", "ProcessorId");

                    if (string.IsNullOrEmpty(result))//If no ProcessorId, use Name
                    {
                        result = GetId("Win32_Processor", "Name");

                        if (string.IsNullOrEmpty(result))//If no Name, use Manufacturer
                            result = GetId("Win32_Processor", "Manufacturer");
                        //Add clock speed for extra security
                        result += GetId("Win32_Processor", "MaxClockSpeed");

                    }
                }
                if (!string.IsNullOrEmpty(result))
                    return GetCryptedString(result);
                else
                    return string.Empty;
            }
        }
        #endregion

        #region BiosId
        [ReadOnly(true)]
        static string BiosId
        {
            get
            {
                string value = string.Concat(
                        GetId("Win32_BIOS", "Manufacturer"),
                        GetId("Win32_BIOS", "SMBIOSBIOSVersion"),
                        GetId("Win32_BIOS", "IdentificationCode"),
                        GetId("Win32_BIOS", "SerialNumber"),
                        GetId("Win32_BIOS", "ReleaseDate"),
                        GetId("Win32_BIOS", "Version"));
                return GetCryptedString(value);
            }
        }
        #endregion

        #region HardDiskId
        [ReadOnly(true)]
        static string HardDiskId
        {
            get
            {
                string value = string.Concat(
                    GetId("Win32_DiskDrive", "Model"),
                    GetId("Win32_DiskDrive", "Manufacturer"),
                    GetId("Win32_DiskDrive", "Signature"),
                    GetId("Win32_DiskDrive", "TotalHeads"));
                return GetCryptedString(value);
            }
        }
        #endregion

        #region BaseBoardId
        [ReadOnly(true)]
        static string BaseBoardId
        {
            get
            {
                string value = string.Concat(
                    GetId("Win32_BaseBoard", "Model"),
                    GetId("Win32_BaseBoard", "Manufacturer"),
                    GetId("Win32_BaseBoard", "Name"),
                    GetId("Win32_BaseBoard", "SerialNumber"));
                return GetCryptedString(value);
            }
        }
        #endregion

        #region VideoControllerId
        [ReadOnly(true)]
        static string VideoControllerId
        {
            get
            {
                string value = string.Concat(
                    GetId("Win32_VideoController", "DriverVersion"),
                    GetId("Win32_VideoController", "Name"));
                return GetCryptedString(value);
            }
        }
        #endregion

        #region MacAddressId
        [ReadOnly(true)]
        static string MacAddressId
        {
            get
            {
                string value = GetId("Win32_NetworkAdapterConfiguration", "MACAddress", "IPEnabled");
                if (value != string.Empty)
                    return GetCryptedString(value);
                return string.Empty;
            }
        }
        #endregion

        #region BaseHardwareFingerPrint

        private static Guid _hardwareFingerPrint;
        public static Guid GetBaseHardwareFingerPrint()
        {
            if (_hardwareFingerPrint == Guid.Empty)
            {
                string value = string.Concat(BaseBoardId, CPUId, MacAddressId);
                value = GetCryptedString(value);
                value = PackToMd5(value);
                _hardwareFingerPrint = new Guid(value);
            }
            return _hardwareFingerPrint;
        }
        #endregion


        #region GetId
        /// <summary>
        /// Obtener el valor de una propiedad de una clase WMI
        /// </summary>
        /// <see cref="http://msdn.microsoft.com/es-es/library/system.management.managementclass(v=vs.80).aspx"/>
        /// <seealso cref="http://msdn.microsoft.com/en-us/library/system.management.managementobject.aspx"/>
        /// <param name="wmiClass">Nombre de la clase WMI</param>
        /// <param name="wmiProperty">Nombre de la propiedad</param>
        /// <returns>Valor de tipo String que representa el valor de la propiedad.</returns>
        static string GetId(string wmiClass, string wmiProperty)
        {
            string result = string.Empty;

            try
            {
                System.Management.ManagementClass mc = new System.Management.ManagementClass(wmiClass);
                System.Management.ManagementObjectCollection moc = mc.GetInstances();
                foreach (System.Management.ManagementObject mo in moc)
                {
                    if (string.IsNullOrEmpty(result))
                    {
                        try
                        {
                            result = mo[wmiProperty].ToString();
                            break;
                        }
                        catch { continue; }
                    }

                }
            }
            catch { result = string.Empty; }

            return result;
        }

        public static string GetId(string wmiClass, string wmiProperty, string wmiThisMustBeTrue)
        {
            string result = string.Empty;

            try
            {
                System.Management.ManagementClass mc = new System.Management.ManagementClass(wmiClass);
                System.Management.ManagementObjectCollection moc = mc.GetInstances();
                foreach (System.Management.ManagementObject mo in moc)
                {
                    if (mo[wmiThisMustBeTrue].ToString().Equals("True") && string.IsNullOrEmpty(result))
                    {
                        try
                        {

                            result = mo[wmiProperty].ToString();
                            break;

                        }
                        catch { continue; }
                    }

                }
            }
            catch { result = string.Empty; }

            return result;
        }
        #endregion

        #region GetCryptedString
        /// <summary>
        /// Codifica una cadena (texto plano) usando el algoritmo SHA512.
        /// </summary>
        /// <see cref="http://msdn.microsoft.com/en-us/library/system.security.cryptography.sha512.aspx"/>
        /// <param name="plain">La cadena, en texto plano, que se desea codificar.</param>
        /// <returns>Retorna un valor de tipo String del valor codificado en formato hexadecimal.</returns>
        static string GetCryptedString(string plain)
        {
            string result = string.Empty;

            try
            {
                plain = plain.Trim();

                using (System.Security.Cryptography.SHA512 sha512 = System.Security.Cryptography.SHA512.Create())
                {
                    byte[] data = sha512.ComputeHash(Encoding.UTF8.GetBytes(plain));
                    for (int i = 0; i < data.Length; i++)
                        result += data[i].ToString("x2");
                }
            }
            catch { result = string.Empty; }

            return result;
        }
        #endregion

        #region PackToMd5
        static string PackToMd5(string value)
        {
            string result = string.Empty;
            try
            {
                value = value.Trim();

                using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
                {
                    byte[] data = md5.ComputeHash(Encoding.UTF8.GetBytes(value));
                    for (int i = 0; i < data.Length; i++)
                        result += data[i].ToString("x2");
                }
            }
            catch (Exception) { result = string.Empty; }



            return result;
        }
        #endregion
    }
}
