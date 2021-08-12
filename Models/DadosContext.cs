using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

namespace Estacionamento.Models
{
    public class DadosContext
    {

        private readonly string _connectionString;
        public DadosContext(IConfiguration configuration)
        {
        _connectionString = configuration.GetConnectionString("Default");
        }
       

        public List<T> RetornarLista<T>(string procedure, MySqlParameter[] parametros) where T : class, new()
        {
            
            Type type = typeof(T);
            List<T> lista = new List<T>();
            ConstructorInfo ctor = type.GetConstructor(Type.EmptyTypes);
            if (ctor == null) {
                throw new InvalidOperationException($"Type {type.Name} does not have a default constructor.");
            }
            
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                using (MySqlCommand  cmd = new MySqlCommand (procedure, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddRange(parametros);
                    conn.Open();
                    using(MySqlDataReader  rdr =  cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            T newInst = (T)ctor.Invoke(null);
                            for (int i = 0; i < rdr.FieldCount; i++) 
                            {
                                string propName = rdr.GetName(i);
                                PropertyInfo propInfo = type.GetProperty(propName);
                                if (propInfo != null) 
                                {
                                    object value = rdr.GetValue(i);
                                    if (value == DBNull.Value) {
                                        propInfo.SetValue(newInst, null);
                                    } else {
                                        propInfo.SetValue(newInst, value);
                                    }
                                }
                            }
                            lista.Add(newInst);                        
                        }
                    }
                    conn.Close();
                }
            }
            return lista;
        }

        public T ListarObjeto<T>(string procedure, MySqlParameter[] parametros) where T : class, new()
        {
            
            Type type = typeof(T);
            ConstructorInfo ctor = type.GetConstructor(Type.EmptyTypes);
            if (ctor == null) {
                throw new InvalidOperationException($"Type {type.Name} does not have a default constructor.");
            }
            T newInst = (T)ctor.Invoke(null);
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                using (MySqlCommand  cmd = new MySqlCommand (procedure, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddRange(parametros);
                    conn.Open();
                    using(MySqlDataReader  rdr =  cmd.ExecuteReader())
                    {
                        
                        while (rdr.Read())
                        {
                            
                            for (int i = 0; i < rdr.FieldCount; i++) 
                            {
                                string propName = rdr.GetName(i);
                                PropertyInfo propInfo = type.GetProperty(propName);
                                if (propInfo != null) 
                                {
                                    object value = rdr.GetValue(i);
                                    if (value == DBNull.Value) {
                                        propInfo.SetValue(newInst, null);
                                    } else {
                                        propInfo.SetValue(newInst, value);
                                    }
                                }
                            }
                                                    
                        }
                        conn.Close();
                       
                    }
                }
            }
            return newInst;
        }

    }
}