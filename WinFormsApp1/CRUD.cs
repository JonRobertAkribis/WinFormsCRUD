using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Windows.Forms;
using Npgsql;

namespace WinFormsApp1
{
    class CRUD
    {
        private static string getConnectionString()
        {



            ///////////////jon modified
            var cs = "Server=localhost;Port=5432;Database=testDB;Username=postgres;Password=root;";


            string conString = cs;
            return conString;
        }

        public static NpgsqlConnection con = new NpgsqlConnection(getConnectionString());
        public static NpgsqlCommand cmd = default(NpgsqlCommand);
        public static string sql = string.Empty;

        public static DataTable PerformCRUD(NpgsqlCommand com)
        {
            NpgsqlDataAdapter da = default(NpgsqlDataAdapter);
            DataTable dt = new DataTable();

            try
            {
                da = new NpgsqlDataAdapter();
                da.SelectCommand = com;
                da.Fill(dt);

                return dt;
            }
            catch (Exception Ex)
            {
                MessageBox.Show("An error occured: " + Ex.Message, "Perfrom CRUD fail", MessageBoxButtons.OK, MessageBoxIcon.Error);
                dt = null;
                
            }
            return dt;
        }
    }
}
