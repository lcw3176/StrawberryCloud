using System.Data.SQLite;

namespace StrawberryCloudServer.DataBase
{
    class DbWrite
    {
        private string strConn = @"Data Source=D:\cloud.db";

        public void SetUser(string userId, string userPw)
        {
            using (SQLiteConnection conn = new SQLiteConnection(strConn))
            {
                conn.Open();
                string sql = string.Format("INSERT INTO user(name, password) " +
                                           "VALUES('{0}', '{1}')", userId, userPw);

                SQLiteCommand cmd = new SQLiteCommand(sql, conn);
                cmd.ExecuteNonQuery();
                cmd.Dispose();
            }
        }
    }
}
