using System.Data.SqlClient;
using NUnit.Framework;

namespace Test
{
    [SetUpFixture]
    class TestDb
    {
        public static SqlConnection Connection;

        [SetUp]
        public void Connect()
        {
            var connectionString = @"Data Source=(localdb)\v11.0;Integrated Security=true;AttachDbFileName=|DataDirectory|\Database1.mdf";
//            const string connectionString = "Server=remote-pc\\sqlexpress;Trusted_Connection=True;";
            Connection = new SqlConnection(connectionString);
            Connection.Open();
        }

        [TearDown]
        public void Disconnect()
        {
            Connection.Close();
            Connection.Dispose();
        }
    }
}
