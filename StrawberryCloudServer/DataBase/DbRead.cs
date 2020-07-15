using System.Data.SQLite;

namespace StrawberryCloudServer.DataBase
{
    class DbRead
    {
        private string strConn = @"Data Source=D:\cloud.db";

        public bool IsUser(string userId, string userPw)
        {
            using (SQLiteConnection conn = new SQLiteConnection(strConn))
            {
                conn.Open();
                string sql = string.Format("SELECT name FROM user " +
                                           "WHERE name = '{0}' " +
                                           "AND " +
                                           "password = '{1}'", userId, userPw);

                SQLiteCommand cmd = new SQLiteCommand(sql, conn);
                SQLiteDataReader reader = cmd.ExecuteReader();
                
                while(reader.Read())
                {
                    if (reader["name"] != null)
                    {

                        cmd.Dispose();
                        reader.Close();
                        return true;

                    }
                }

                cmd.Dispose();
                reader.Close();
                return false;
            }
        }
    }
}
