using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace EnroladorWebServices
{
    public class EnroladorWebServices : IEnroladorWebServices
    {
		//private string connectionString() = ""; //"Pooling=false;Data Source=192.168.150.10;Initial Catalog=SCCSPArauco;User Id=EnroladorStandAlone;Password=Enrolador:2017;";

		private string connectionString()
		{
			return  Properties.Settings.Default.ConnectionString;
		}

        public string AccionActualizarHuella(Guid responsable, Guid oid, string data)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString()))
                using (SqlCommand comm = new SqlCommand("ESA_Actualizar_Huella", conn) { CommandType = CommandType.StoredProcedure })
                {
                    conn.Open();

                    comm.Parameters.Add("@LoggedUserOid", SqlDbType.UniqueIdentifier).Value = responsable;
                    comm.Parameters.Add("@Oid", SqlDbType.UniqueIdentifier).Value = oid;
                    comm.Parameters.Add("@Data", SqlDbType.VarChar).Value = data;
                    SqlParameter outParam = new SqlParameter("@Error", SqlDbType.NVarChar, -1);
                    outParam.Direction = ParameterDirection.Output;
                    comm.Parameters.Add(outParam);

                    comm.ExecuteNonQuery();

                    if (!(outParam.Value is DBNull) && !string.IsNullOrEmpty((string)outParam.Value))
                    {
                        return (string)outParam.Value;
                    }
                    return null;
                }
            }
            catch(Exception ex)
            {
                return ex.Message;
            }
        }

        public string AccionCaducarContrato(Guid responsable, Guid oid, DateTime? finVigencia)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString()))
                using (SqlCommand comm = new SqlCommand("ESA_Caducar_Contrato", conn) { CommandType = CommandType.StoredProcedure })
                {
                    conn.Open();

                    comm.Parameters.Add("@LoggedUserOid", SqlDbType.UniqueIdentifier).Value = responsable;
                    comm.Parameters.Add("@ContratoOid", SqlDbType.UniqueIdentifier).Value = oid;
                    if (finVigencia.HasValue)
                    {
                        comm.Parameters.Add("@FinVigencia", SqlDbType.DateTime).Value = finVigencia.Value;
                    }
                    else
                    {
                        comm.Parameters.Add("@FinVigencia", SqlDbType.DateTime).Value = DBNull.Value;
                    }
                    SqlParameter outParam = new SqlParameter("@Error", SqlDbType.NVarChar, -1);
                    outParam.Direction = ParameterDirection.Output;
                    comm.Parameters.Add(outParam);

                    comm.ExecuteNonQuery();

                    if (!(outParam.Value is DBNull) && !string.IsNullOrEmpty((string)outParam.Value))
                    {
                        return (string)outParam.Value;
                    }
                    return null;
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public string AccionCrearAsignacion(Guid responsable, Guid oid, Guid empleado, Guid dispositivo)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString()))
                using (SqlCommand comm = new SqlCommand("ESA_Crear_Autorizacion", conn) { CommandType = CommandType.StoredProcedure })
                {
                    conn.Open();

                    comm.Parameters.Add("@LoggedUserOid", SqlDbType.UniqueIdentifier).Value = responsable;
                    comm.Parameters.Add("@Oid", SqlDbType.UniqueIdentifier).Value = oid;
                    comm.Parameters.Add("@Empleado", SqlDbType.UniqueIdentifier).Value = empleado;
                    comm.Parameters.Add("@Dispositivo", SqlDbType.UniqueIdentifier).Value = dispositivo;
                    SqlParameter outParam = new SqlParameter("@Error", SqlDbType.NVarChar, -1);
                    outParam.Direction = ParameterDirection.Output;
                    comm.Parameters.Add(outParam);

                    comm.ExecuteNonQuery();

                    if (!(outParam.Value is DBNull) && !string.IsNullOrEmpty((string)outParam.Value))
                    {
                        return (string)outParam.Value;
                    }
                    return null;
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public string AccionCrearContrato(Guid responsable, Guid oid, Guid empleado, Guid empresa, Guid cuenta, Guid cargo, DateTime inicioVigencia, DateTime? finVigencia, string CodigoContrato)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString()))
                using (SqlCommand comm = new SqlCommand("ESA_Crear_Contrato", conn) { CommandType = CommandType.StoredProcedure })
                {
                    conn.Open();

                    comm.Parameters.Add("@LoggedUserOid", SqlDbType.UniqueIdentifier).Value = responsable;
                    comm.Parameters.Add("@Oid", SqlDbType.UniqueIdentifier).Value = oid;
                    comm.Parameters.Add("@Empleado", SqlDbType.UniqueIdentifier).Value = empleado;
                    comm.Parameters.Add("@Empresa", SqlDbType.UniqueIdentifier).Value = empresa;
                    comm.Parameters.Add("@Cuenta", SqlDbType.UniqueIdentifier).Value = cuenta;
                    comm.Parameters.Add("@Cargo", SqlDbType.UniqueIdentifier).Value = cargo;
                    comm.Parameters.Add("@InicioVigencia", SqlDbType.DateTime).Value = inicioVigencia;
                    comm.Parameters.Add("@CodigoContrato", SqlDbType.VarChar).Value = CodigoContrato.Length > 100 ? CodigoContrato.Substring(0,100) : CodigoContrato;

                    if (finVigencia.HasValue)
                    {
                        comm.Parameters.Add("@FinVigencia", SqlDbType.DateTime).Value = finVigencia.Value;
                    }
                    else
                    {
                        comm.Parameters.Add("@FinVigencia", SqlDbType.DateTime).Value = DBNull.Value;
                    }
                    SqlParameter outParam = new SqlParameter("@Error", SqlDbType.NVarChar, -1);
                    outParam.Direction = ParameterDirection.Output;
                    comm.Parameters.Add(outParam);

                    comm.ExecuteNonQuery();

                    if (!(outParam.Value is DBNull) && !string.IsNullOrEmpty((string)outParam.Value))
                    {
                        return (string)outParam.Value;
                    }
                    return null;
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public string AccionCrearEmpleado(Guid responsable, Guid oid, string RUT, string firstName, string lastName, int enrollID, string contraseña)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString()))
                using (SqlCommand comm = new SqlCommand("ESA_Crear_Empleado", conn) { CommandType = CommandType.StoredProcedure })
                {
                    conn.Open();

                    comm.Parameters.Add("@LoggedUserOid", SqlDbType.UniqueIdentifier).Value = responsable;
                    comm.Parameters.Add("@Oid", SqlDbType.UniqueIdentifier).Value = oid;
                    comm.Parameters.Add("@RUT", SqlDbType.VarChar).Value = RUT;
                    comm.Parameters.Add("@FirstName", SqlDbType.VarChar).Value = firstName;
                    comm.Parameters.Add("@LastName", SqlDbType.VarChar).Value = lastName;
                    comm.Parameters.Add("@EnrollID", SqlDbType.Int).Value = enrollID;
                    comm.Parameters.Add("@Contraseña", SqlDbType.NVarChar).Value = contraseña;
                    SqlParameter outParam = new SqlParameter("@Error", SqlDbType.NVarChar, -1);
                    outParam.Direction = ParameterDirection.Output;
                    comm.Parameters.Add(outParam);

                    comm.ExecuteNonQuery();

                    if (!(outParam.Value is DBNull) && !string.IsNullOrEmpty((string)outParam.Value))
                    {
                        return (string)outParam.Value;
                    }
                    return null;
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public string AccionCrearHuella(Guid responsable, Guid oid, Guid empleado, int tipoHuella, string data)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString()))
                using (SqlCommand comm = new SqlCommand("ESA_Crear_Huella", conn) { CommandType = CommandType.StoredProcedure })
                {
                    conn.Open();

                    comm.Parameters.Add("@LoggedUserOid", SqlDbType.UniqueIdentifier).Value = responsable;
                    comm.Parameters.Add("@Oid", SqlDbType.UniqueIdentifier).Value = oid;
                    comm.Parameters.Add("@Empleado", SqlDbType.UniqueIdentifier).Value = empleado;
                    comm.Parameters.Add("@TipoHuella", SqlDbType.Int).Value = tipoHuella;
                    comm.Parameters.Add("@Data", SqlDbType.VarChar).Value = data;
                    SqlParameter outParam = new SqlParameter("@Error", SqlDbType.NVarChar, -1);
                    outParam.Direction = ParameterDirection.Output;
                    comm.Parameters.Add(outParam);

                    comm.ExecuteNonQuery();

                    if (!(outParam.Value is DBNull) && !string.IsNullOrEmpty((string)outParam.Value))
                    {
                        return (string)outParam.Value;
                    }
                    return null;
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public string AccionModificarContraseña(Guid responsable, Guid oid, string contraseña)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString()))
                using (SqlCommand comm = new SqlCommand("", conn) { CommandType = CommandType.StoredProcedure })
                {
                    conn.Open();

                    if (string.IsNullOrEmpty(contraseña))
                    {
                        comm.CommandText = "ESA_Eliminar_Contraseña";
                        comm.Parameters.Add("@LoggedUserOid", SqlDbType.UniqueIdentifier).Value = responsable;
                        comm.Parameters.Add("@EmpleadoOid", SqlDbType.UniqueIdentifier).Value = oid;
                        SqlParameter outParam = new SqlParameter("@Error", SqlDbType.NVarChar);
                        outParam.Direction = ParameterDirection.Output;
                        outParam.Size = -1; // nvarchar(max)
                        comm.Parameters.Add(outParam);

                        comm.ExecuteNonQuery();

                        if (!(outParam.Value is DBNull) && !string.IsNullOrEmpty((string)outParam.Value))
                        {
                            return (string)outParam.Value;
                        }
                        return null;
                    }
                    else
                    {
                        comm.CommandText = "ESA_Crear_Contraseña";
                        comm.CommandType = CommandType.StoredProcedure;
                        comm.Parameters.Clear();
                        comm.Parameters.Add("@LoggedUserOid", SqlDbType.UniqueIdentifier).Value = responsable;
                        comm.Parameters.Add("@EmpleadoOid", SqlDbType.UniqueIdentifier).Value = oid;
                        comm.Parameters.Add("@Contraseña", SqlDbType.VarChar).Value = contraseña;
                        SqlParameter outParam = new SqlParameter("@Error", SqlDbType.NVarChar);
                        outParam.Direction = ParameterDirection.Output;
                        outParam.Size = -1; // nvarchar(max)
                        comm.Parameters.Add(outParam);

                        comm.ExecuteNonQuery();

                        if (!(outParam.Value is DBNull) && !string.IsNullOrEmpty((string)outParam.Value))
                        {
                            return (string)outParam.Value;
                        }
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public bool Identificar(Guid HWID)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString()))
                using (SqlCommand comm = new SqlCommand("ESA_Identificar", conn) { CommandType = CommandType.StoredProcedure })
                {
                    conn.Open();

                    comm.Parameters.Add("@HWID", SqlDbType.UniqueIdentifier).Value = HWID;
                    SqlParameter outParam = new SqlParameter("@Identificado", SqlDbType.Bit);
                    outParam.Direction = ParameterDirection.Output;
                    comm.Parameters.Add(outParam);

                    comm.ExecuteNonQuery();

                    if (!(outParam.Value is DBNull))
                    {
                        //lpg
                        return true;
                        //return (bool)outParam.Value;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception XD)
            {
                return false;
            }

        }

        public List<Tuple<Guid, string>> LeeCadena(Guid loggedUser)
        {
            string sql = string.Format("SELECT Oid, Nombre FROM ESA_Cadena WHERE Usuario = '{0}'", loggedUser);

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString()))
                using (SqlCommand comm = new SqlCommand("", conn))
                {
                    conn.Open();
                    List<Tuple<Guid, string>> res;

                    comm.CommandText = string.Format("SELECT COUNT(*) FROM ({0}) AS Tabla", sql);
                    int filas = (int)comm.ExecuteScalar();

                    comm.CommandText = sql;
                    SqlDataReader reader = null;
                    try
                    {
                        reader = comm.ExecuteReader();

                        res = new List<Tuple<Guid, string>>(filas);
                        while (reader.Read())
                        {
                            Guid Oid = reader.GetFieldValue<Guid>(0);
                            string Nombre = reader.GetFieldValue<string>(1);
                            res.Add(new Tuple<Guid, string>(Oid, Nombre));
                        }
                    }
                    finally
                    {
                        reader.Close();
                    }

                    if (res.Count == 0)
                    {
                        return null;
                    }
                    return res;
                }
            }
            catch (Exception XD)
            {
                return null;
            }
        }

        public List<Tuple<Guid, string, Guid>> LeeCargo(Guid loggedUser)
        {
            string sql = string.Format("SELECT Oid, Nombre, Empresa FROM ESA_Cargo WHERE Usuario = '{0}'", loggedUser);

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString()))
                using (SqlCommand comm = new SqlCommand("", conn))
                {
                    conn.Open();
                    List<Tuple<Guid, string, Guid>> res;

                    comm.CommandText = string.Format("SELECT COUNT(*) FROM ({0}) AS Tabla", sql);
                    int filas = (int)comm.ExecuteScalar();

                    comm.CommandText = sql;
                    SqlDataReader reader = null;
                    try
                    {
                        reader = comm.ExecuteReader();

                        res = new List<Tuple<Guid, string, Guid>>(filas);
                        while (reader.Read())
                        {
                            Guid Oid = reader.GetFieldValue<Guid>(0);
                            string Nombre = reader.GetFieldValue<string>(1);
                            Guid Empresa = reader.GetFieldValue<Guid>(2);
                            res.Add(new Tuple<Guid, string, Guid>(Oid, Nombre, Empresa));
                        }
                    }
                    finally
                    {
                        reader.Close();
                    }

                    if (res.Count == 0)
                    {
                        return null;
                    }
                    return res;
                }
            }
            catch (Exception XD)
            {
                return null;
            }
        }

        public List<Tuple<Guid, Guid, Guid, Guid, DateTime, DateTime?, Guid, Tuple<string>>> LeeContrato(Guid loggedUser)
        {
            string sql = string.Format("SELECT Oid, Empresa, Cuenta, Cargo, InicioVigencia, FinVigencia, Empleado, Codigo FROM ESA_Contrato WHERE /*(GETDATE() BETWEEN ISNULL(InicioVigencia, GETDATE()) AND ISNULL(FinVigencia, GETDATE())) AND */ Usuario = '{0}'", loggedUser);

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString()))
                using (SqlCommand comm = new SqlCommand("", conn))
                {
                    conn.Open();
                    List<Tuple<Guid, Guid, Guid, Guid, DateTime, DateTime?, Guid, Tuple<string>>> res;

                    comm.CommandText = string.Format("SELECT COUNT(*) FROM ({0}) AS Tabla", sql);
                    int filas = (int)comm.ExecuteScalar();

                    comm.CommandText = sql;
                    SqlDataReader reader = null;
                    try
                    {
                        reader = comm.ExecuteReader();

                        res = new List<Tuple<Guid, Guid, Guid, Guid, DateTime, DateTime?, Guid, Tuple<string>>>(filas);
                        int i = 0;
                        while (reader.Read())
                        {
                            try
                            {
                                Guid Oid = reader.GetFieldValue<Guid>(0);
                                Guid Empresa = reader.GetFieldValue<Guid>(1);
                                Guid Cuenta = reader.GetFieldValue<Guid>(2);
                                Guid Cargo = reader.GetFieldValue<Guid>(3);
                                DateTime InicioVigencia = reader.IsDBNull(4) ? DateTime.Today : reader.GetFieldValue<DateTime>(4);
                                DateTime? FinVigencia = null;
                                if (!reader.IsDBNull(5))
                                {
                                    FinVigencia = reader.GetFieldValue<DateTime>(5);
                                }
                                Guid Empleado = reader.GetFieldValue<Guid>(6);
                                var CodigoContrato = reader.IsDBNull(7) ? "SIN_CODIGO_CONTRATO" : reader.GetFieldValue<string>(7).ToString();

                                var tplContrato = new Tuple<string>(CodigoContrato);
                                res.Add(new Tuple<Guid, Guid, Guid, Guid, DateTime, DateTime?, Guid, Tuple<string>>(Oid, Empresa, Cuenta, Cargo, InicioVigencia, FinVigencia, Empleado, tplContrato));
                                i++;
                            }
                            catch (Exception Ex)
                            {
                                throw new Exception("Error cargando contratos", Ex);
                            }
                        }
                    }
                    finally
                    {
                        reader.Close();
                    }

                    if (res.Count == 0)
                    {
                        return null;
                    }
                    return res;
                }
            }
            catch (Exception XD)
            {
                return null;
            }
        }

        public List<Tuple<Guid, string, Guid, DateTime?>> LeeCuenta(Guid loggedUser)
        {
            //lpg string sql = string.Format("SELECT Oid, Nombre, Empresa FROM ESA_Cuenta WHERE Usuario = '{0}'", loggedUser);
            string sql = string.Format(@"SELECT EC.Oid, EC.Nombre, EC.Empresa, C.FechaUltimoCierre FROM ESA_Cuenta EC
                                                INNER JOIN Cuenta C ON C.Oid = EC.Oid WHERE EC.Usuario = '{0}'", loggedUser);

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString()))
                using (SqlCommand comm = new SqlCommand("", conn))
                {
                    conn.Open();
                    List<Tuple<Guid, string, Guid, DateTime?>> res;

                    comm.CommandText = string.Format("SELECT COUNT(*) FROM ({0}) AS Tabla", sql);
                    int filas = (int)comm.ExecuteScalar();

                    comm.CommandText = sql;
                    SqlDataReader reader = null;
                    try
                    {
                        reader = comm.ExecuteReader();

                        res = new List<Tuple<Guid, string, Guid, DateTime?>>(filas);
                        while (reader.Read())
                        {
                            Guid Oid = reader.GetFieldValue<Guid>(0);
                            string Nombre = reader.GetFieldValue<string>(1);
                            Guid Empresa = reader.GetFieldValue<Guid>(2);
                            DateTime? FechaUltimoCierre = !(reader[3] is DBNull) ? reader.GetFieldValue<DateTime?>(3) : null;
                            res.Add(new Tuple<Guid, string, Guid, DateTime?>(Oid, Nombre, Empresa, FechaUltimoCierre));
                        }
                    }
                    finally
                    {
                        reader.Close();
                    }

                    if (res.Count == 0)
                    {
                        return null;
                    }
                    return res;
                }
            }
            catch (Exception XD)
            {
                return null;
            }
        }

        public List<Tuple<Guid, string, string, int, Guid>> LeeDispositivo(Guid loggedUser)
        {
            string sql = string.Format("SELECT Oid, Nombre, Host, Puerto, Instalacion FROM ESA_Dispositivo WHERE Usuario = '{0}'", loggedUser);

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString()))
                using (SqlCommand comm = new SqlCommand("", conn))
                {
                    conn.Open();
                    List<Tuple<Guid, string, string, int, Guid>> res;

                    comm.CommandText = string.Format("SELECT COUNT(*) FROM ({0}) AS Tabla", sql);
                    int filas = (int)comm.ExecuteScalar();

                    comm.CommandText = sql;
                    SqlDataReader reader = null;
                    try
                    {
                        reader = comm.ExecuteReader();

                        res = new List<Tuple<Guid, string, string, int, Guid>>(filas);
                        while (reader.Read())
                        {
                            Guid Oid = reader.GetFieldValue<Guid>(0);
                            string Nombre = reader.GetFieldValue<string>(1);
                            string Host = reader.IsDBNull(2) ? "" : reader.GetFieldValue<string>(2);
                            int Puerto = reader.IsDBNull(3) ? 0 : reader.GetFieldValue<int>(3);
                            Guid Instalacion = reader.GetFieldValue<Guid>(4);
                            res.Add(new Tuple<Guid, string, string, int, Guid>(Oid, Nombre, Host, Puerto, Instalacion));
                        }
                    }
                    finally
                    {
                        reader.Close();
                    }

                    if (res.Count == 0)
                    {
                        return null;
                    }
                    return res;
                }
            }
            catch (Exception XD)
            {
                return null;
            }
        }

        public List<Tuple<Guid, int, string, bool, string, string>> LeeEmpleado()
        {
            //lpg
            string sql = "SELECT TOP 3000 Oid, EnrollID, RUT, Contraseña, Firstname, LastName FROM ESA_Empleado";

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString()))
                using (SqlCommand comm = new SqlCommand("", conn))
                {
                    conn.Open();
                    List<Tuple<Guid, int, string, bool, string, string>> res;

                    comm.CommandText = string.Format("SELECT COUNT(*) FROM ({0}) AS Tabla", sql);
                    int filas = (int)comm.ExecuteScalar();

                    comm.CommandText = sql;
                    SqlDataReader reader = null;
                    try
                    {
                        reader = comm.ExecuteReader();

                        res = new List<Tuple<Guid, int, string, bool, string, string>>(filas);
                        while (reader.Read())
                        {
                            Guid Oid = reader.GetFieldValue<Guid>(0);
                            int EnrollID = reader.GetFieldValue<int>(1);
                            string RUT = reader.GetFieldValue<string>(2);
                            bool Contraseña = reader.GetFieldValue<int>(3) == 1;
                            string FirstName = reader.GetFieldValue<string>(4);
                            string LastName = reader.GetFieldValue<string>(5);
                            res.Add(new Tuple<Guid, int, string, bool, string, string>(Oid, EnrollID, RUT, Contraseña, FirstName, LastName));
                        }
                    }
                    finally
                    {
                        reader.Close();
                    }

                    if (res.Count == 0)
                    {
                        return null;
                    }
                    return res;
                }
            }
            catch (Exception XD)
            {
                return null;
            }
        }

        public List<Tuple<Guid, Guid>> LeeEmpleadosDispositivos()
        {
            string sql = "SELECT Dispositivo,Empleado FROM ESA_PuedeMarcarEn";

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString()))
                using (SqlCommand comm = new SqlCommand("", conn))
                {
                    conn.Open();
                    List<Tuple<Guid, Guid>> res;

                    comm.CommandText = string.Format("SELECT COUNT(*) FROM ({0}) AS Tabla", sql);
                    int filas = (int)comm.ExecuteScalar();

                    comm.CommandText = sql;
                    SqlDataReader reader = null;
                    try
                    {
                        reader = comm.ExecuteReader();

                        res = new List<Tuple<Guid, Guid>>(filas);
                        while (reader.Read())
                        {
                            Guid Dispositivo = reader.GetFieldValue<Guid>(0);
                            Guid Empleado = reader.GetFieldValue<Guid>(1);
                            res.Add(new Tuple<Guid, Guid>(Dispositivo, Empleado));
                        }
                    }
                    finally
                    {
                        reader.Close();
                    }

                    if (res.Count == 0)
                    {
                        return null;
                    }
                    return res;
                }
            }
            catch (Exception XD)
            {
                return null;
            }
        }

        public List<Tuple<Guid, string>> LeeEmpresa(Guid loggedUser)
        {
            string sql = string.Format("SELECT Oid, Name FROM ESA_Empresa WHERE Usuario = '{0}'", loggedUser);

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString()))
                using (SqlCommand comm = new SqlCommand("", conn))
                {
                    conn.Open();
                    List<Tuple<Guid, string>> res;

                    comm.CommandText = string.Format("SELECT COUNT(*) FROM ({0}) AS Tabla", sql);
                    int filas = (int)comm.ExecuteScalar();

                    comm.CommandText = sql;
                    SqlDataReader reader = null;
                    try
                    {
                        reader = comm.ExecuteReader();

                        res = new List<Tuple<Guid, string>>(filas);
                        while (reader.Read())
                        {
                            Guid Oid = reader.GetFieldValue<Guid>(0);
                            string Nombre = reader.GetFieldValue<string>(1);
                            res.Add(new Tuple<Guid, string>(Oid, Nombre));
                        }
                    }
                    finally
                    {
                        reader.Close();
                    }

                    if (res.Count == 0)
                    {
                        return null;
                    }
                    return res;
                }
            }
            catch (Exception XD)
            {
                return null;
            }
        }

        public List<Tuple<Guid, int, Guid>> LeeHuella()
        {
            string sql = "SELECT Oid, TipoHuella, Empleado FROM ESA_Huella";

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString()))
                using (SqlCommand comm = new SqlCommand("", conn))
                {
                    conn.Open();
                    List<Tuple<Guid, int, Guid>> res;

                    comm.CommandText = string.Format("SELECT COUNT(*) FROM ({0}) AS Tabla", sql);
                    int filas = (int)comm.ExecuteScalar();

                    comm.CommandText = sql;
                    SqlDataReader reader = null;
                    try
                    {
                        reader = comm.ExecuteReader();

                        res = new List<Tuple<Guid, int, Guid>>(filas);
                        while (reader.Read())
                        {
                            Guid Oid = reader.GetFieldValue<Guid>(0);
                            int TipoHuella = reader.GetFieldValue<int>(1);
                            Guid Empleado = reader.GetFieldValue<Guid>(2);
                            res.Add(new Tuple<Guid, int, Guid>(Oid, TipoHuella, Empleado));
                        }
                    }
                    finally
                    {
                        reader.Close();
                    }

                    if (res.Count == 0)
                    {
                        return null;
                    }
                    return res;
                }
            }
            catch (Exception XD)
            {
                return null;
            }
        }

        public List<Tuple<Guid, int, string>> LeeHuellaUser(Guid loggedUser)
        {
            try
            {
                List<Tuple<Guid, int, string>> res = new List<Tuple<Guid, int, string>>();
                string sql = string.Format("SELECT Oid, TipoHuella, Data FROM ESA_Huella_Usuario WHERE Usuario = '{0}'", loggedUser);

                using (SqlConnection conn = new SqlConnection(connectionString()))
                using (SqlCommand comm = new SqlCommand(sql, conn))
                {
                    SqlDataReader reader = null;
                    try
                    {
                        conn.Open();
                        reader = comm.ExecuteReader();

                        while (reader.Read())
                        {
                            Guid Oid = reader.GetFieldValue<Guid>(0);
                            int TipoHuella = reader.GetFieldValue<int>(1);
                            string Data = reader.GetFieldValue<string>(2);
                            res.Add(new Tuple<Guid, int, string>(Oid, TipoHuella, Data));
                        }
                        return res;
                    }
                    finally
                    {
                        reader.Close();
                    }
                }
            }
            catch (Exception XD)
            {
                return null;
            }
        }

        public List<Tuple<Guid, string, Guid>> LeeInstalacion(Guid loggedUser)
        {
            string sql = string.Format("SELECT Oid, Nombre, Cadena FROM ESA_Instalacion WHERE Usuario = '{0}'", loggedUser);

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString()))
                using (SqlCommand comm = new SqlCommand("", conn))
                {
                    conn.Open();
                    List<Tuple<Guid, string, Guid>> res;

                    comm.CommandText = string.Format("SELECT COUNT(*) FROM ({0}) AS Tabla", sql);
                    int filas = (int)comm.ExecuteScalar();

                    comm.CommandText = sql;
                    SqlDataReader reader = null;
                    try
                    {
                        reader = comm.ExecuteReader();

                        res = new List<Tuple<Guid, string, Guid>>(filas);
                        while (reader.Read())
                        {
                            Guid Oid = reader.GetFieldValue<Guid>(0);
                            string Nombre = reader.GetFieldValue<string>(1);
                            Guid Cadena = reader.GetFieldValue<Guid>(2);
                            res.Add(new Tuple<Guid, string, Guid>(Oid, Nombre, Cadena));
                        }
                    }
                    finally
                    {
                        reader.Close();
                    }

                    if (res.Count == 0)
                    {
                        return null;
                    }
                    return res;
                }
            }
            catch (Exception XD)
            {
                return null;
            }
        }

        public Guid? Login(string user, string pass)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString()))
                using (SqlCommand comm = new SqlCommand("ESA_Login", conn) { CommandType = CommandType.StoredProcedure })
                {
                    conn.Open();

                    comm.Parameters.Add("@User", SqlDbType.NVarChar).Value = user;
                    comm.Parameters.Add("@Pass", SqlDbType.NVarChar).Value = pass;
                    SqlParameter outParam = new SqlParameter("@OidUsuario", SqlDbType.UniqueIdentifier);
                    outParam.Direction = ParameterDirection.Output;
                    comm.Parameters.Add(outParam);

                    comm.ExecuteNonQuery();

                    if (!(outParam.Value is DBNull))
                    {
                        return (Guid)outParam.Value;
                    }
                    return null;
                }
            }
            catch (Exception XD)
            {
                return null;
            }
        }

        public Tuple<string, string> Revalidar(Guid loggedUser)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString()))
                using (SqlCommand comm = new SqlCommand("ESA_Revalidar", conn) { CommandType = CommandType.StoredProcedure })
                {
                    conn.Open();

                    comm.Parameters.Add("@LoggedUserOid", SqlDbType.UniqueIdentifier).Value = loggedUser;
                    SqlParameter outParam1 = new SqlParameter("@User", SqlDbType.NVarChar, -1);
                    outParam1.Direction = ParameterDirection.Output;
                    comm.Parameters.Add(outParam1);
                    SqlParameter outParam2 = new SqlParameter("@Pass", SqlDbType.VarBinary, -1);
                    outParam2.Direction = ParameterDirection.Output;
                    comm.Parameters.Add(outParam2);

                    comm.ExecuteNonQuery();

                    if (!(outParam1.Value is DBNull) && !(outParam2.Value is DBNull))
                    {
                        return new Tuple<string, string>((string)outParam1.Value, ByteArrayToString((byte[])outParam2.Value));
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception XD)
            {
                return null;
            }
        }

        private string ByteArrayToString(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:X2}", b);
            return hex.ToString();
        }
    }
}
