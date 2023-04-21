using System.Data.SqlClient;

namespace PasswordLink.Data
{
    public class PasswordLinkManager
    {
        private string _connectionString { get; set; }

        public PasswordLinkManager(string connectionString)
        {
            _connectionString = connectionString;
        }

        public int Add(string fileName, string password)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"insert into Images(FileName, Password, Views)
                                values(@fileName, @passWord, @views) SELECT SCOPE_IDENTITY()";
            cmd.Parameters.AddWithValue("@fileName", fileName);
            cmd.Parameters.AddWithValue("@passWord", password);
            cmd.Parameters.AddWithValue("@views", 0);
            connection.Open();
            return (int)(decimal)cmd.ExecuteScalar();

        }
        public Images GetImage(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"select * from Images where Id = @id";
            cmd.Parameters.AddWithValue("@id", id);

            connection.Open();
            var reader = cmd.ExecuteReader();


            if (!reader.Read())
            {
                return null;
            }
            Images image = new Images();
            image.Id = (int)reader["Id"];
            image.FileName = (string)reader["FileName"];
            image.Password = (string)reader["Password"];
            image.View = (int)reader["Views"];

            return image;
            }
        public void UpdateView(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"Update Images set Views = Views + 1 WHERE Id = @id";
                       
            cmd.Parameters.AddWithValue("@id", id);
            connection.Open();
          
            Images image = new Images();
            cmd.ExecuteNonQuery();
            
                     
        }
        public int GetView(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"select Views from Images where Id = @id";
            cmd.Parameters.AddWithValue("@id", id);

            connection.Open();
            var reader = cmd.ExecuteReader();

            Images image = new Images();
            while (reader.Read())
            {
                image.View = (int)reader["Views"];
            }
           
           
            return image.View;
        }

    }


}
       
   


