using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrawberryCloudServer.DataBase
{
    class DbInit
    {
        private string strConn = @"Data Source=D:\cloud.db;";

        public void WriteTable()
        {

            using (SQLiteConnection conn = new SQLiteConnection(strConn))
            {
                conn.Open();
                string sql = "CREATE TABLE user(" +
                             "sid integer PRIMARY KEY AUTOINCREMENT," +
                             "name varchar(30) not null," +
                             "password varchar(20) not null)";

                SQLiteCommand cmd = new SQLiteCommand(sql, conn);
                cmd.ExecuteNonQuery();
                cmd.Dispose();
            }
                

        }
    }
}
