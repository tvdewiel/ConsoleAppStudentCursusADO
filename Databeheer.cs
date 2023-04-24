using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleAppStudentCursusADO
{
    public class Databeheer
    {
        private string connectionString;

        public Databeheer(string connectionString)
        {
            this.connectionString = connectionString;
        }
        public void VoegCursusToe(Cursus c)
        {
            string sql = "INSERT INTO dbo.cursus(cursusnaam) VALUES (@cursusnaam)";
            using(SqlConnection connection= new SqlConnection(connectionString))
            using(SqlCommand command= connection.CreateCommand())
            {
                try
                {
                    connection.Open();
                    command.CommandText = sql;
                    command.Parameters.Add(new SqlParameter("@cursusnaam", SqlDbType.NVarChar));
                    command.Parameters["@cursusnaam"].Value = c.Cursusnaam;
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }
        public Cursus GeefCursus(int id)
        {
            string sql = "SELECT * FROM cursus WHERE id=@id";
            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = connection.CreateCommand())
            {
                try
                {
                    connection.Open();
                    command.CommandText = sql;
                    command.Parameters.AddWithValue("@id",id);
                    SqlDataReader reader= command.ExecuteReader();
                    reader.Read();
                    Cursus c = new Cursus((int)reader["id"], (string)reader["cursusnaam"]);
                    reader.Close();
                    return c;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    throw;
                }
            }
        }
        public List<Cursus> GeefCursussen()
        {
            List<Cursus> cursussen=new List<Cursus>();
            string sql = "SELECT * FROM cursus ";
            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = connection.CreateCommand())
            {
                try
                {
                    connection.Open();
                    command.CommandText = sql;                   
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        cursussen.Add(new Cursus((int)reader["id"], (string)reader["cursusnaam"]));
                    }
                    reader.Close();
                    return cursussen;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    throw;
                }
            }
        }
        public void VoegStudentMetCursussenToe(Student student)
        {
            string sql1 = @"insert into student(naam,klasid) output INSERTED.ID values(@naam,@klasid)";
            string sql2 = @"insert into student_cursus(cursusid,studentid) values(@cursusid,@studentid)";
            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command1 = connection.CreateCommand())
            using (SqlCommand command2 = connection.CreateCommand())
            {                
                connection.Open();
                SqlTransaction trans=connection.BeginTransaction();
                try
                {
                    command1.Transaction = trans;
                    command2.Transaction = trans;
                    command1.CommandText = sql1;
                    command1.Parameters.AddWithValue("@naam", student.Studentnaam);
                    command1.Parameters.AddWithValue("@klasid", student.Klas.Id);
                    int newStudentid=(int)command1.ExecuteScalar();
                    command2.CommandText = sql2;
                    command2.Parameters.AddWithValue("@studentid", newStudentid);
                    command2.Parameters.Add(new SqlParameter("@cursusid", SqlDbType.Int));
                    foreach (Cursus c in student.Cursussen)
                    {
                        command2.Parameters["@cursusid"].Value=c.Id;
                        command2.ExecuteNonQuery();
                    }
                    trans.Commit();
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                }
            }
        }
        public void VoegKlasToe(Klas klas)
        {
            string sql = "INSERT INTO dbo.klas(klasnaam,lokaal) VALUES(@klasnaam,@lokaal)";
            using(SqlConnection conn=new SqlConnection(connectionString))
            using(SqlCommand command=conn.CreateCommand())
            {
                try
                {
                    conn.Open();
                    command.CommandText = sql;
                    command.Parameters.AddWithValue("@klasnaam", klas.Klasnaam);
                    if (klas.Lokaal==null)
                        command.Parameters.AddWithValue("@lokaal", DBNull.Value);
                    else 
                        command.Parameters.AddWithValue("@lokaal", klas.Lokaal);
                    command.ExecuteNonQuery();
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }

        public List<Klas> GeefKlassen()
        {
            List<Klas> klassen = new List<Klas>();
            string sql = "SELECT * FROM dbo.Klas";
            using(SqlConnection conn=new SqlConnection(connectionString))
            using(SqlCommand command=conn.CreateCommand())
            {
                try
                {
                    conn.Open();
                    command.CommandText = sql;
                    SqlDataReader reader = command.ExecuteReader();
                    while(reader.Read())
                    {
                        Klas k = new Klas((string)reader["klasnaam"]);
                        if (!reader.IsDBNull(reader.GetOrdinal("lokaal")))
                            k.Lokaal = (string)reader["lokaal"];
                        else k.Lokaal = "tzal buiten te doen zijn";
                        klassen.Add(k);
                    }
                    reader.Close();
                }
                catch(Exception ex) { Console.WriteLine(ex.Message); }
            }

            return klassen;
        }
        public List<Student> ZoekStudenten(string text)
        {
            Dictionary<int,Student> studenten = new Dictionary<int, Student>();
            string sql = "select t1.*,t2.*,t3.cursusnaam,t4.klasnaam,t4.lokaal from student t1 left join student_cursus t2 on t1.id=t2.studentid left join cursus t3 on t3.id=t2.cursusid left join klas t4 on t4.id=t1.klasId where t1.naam like @text";
            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand command = conn.CreateCommand())
            {
                try
                {
                    conn.Open();
                    command.CommandText = sql;
                    command.Parameters.AddWithValue("@text", text+"%");
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        int studentId = (int)reader["id"];
                        if (!studenten.ContainsKey(studentId))
                        {
                            Klas k = new Klas((int)reader["klasid"],(string)reader["klasnaam"]);
                            if (!reader.IsDBNull(reader.GetOrdinal("lokaal")))
                                k.Lokaal = (string)reader["lokaal"];
                            else k.Lokaal = "tzal buiten te doen zijn";
                            Student s = new Student(studentId,(string)reader["naam"], k);
                            studenten.Add(studentId, s);
                        }
                        Cursus c = new Cursus((int)reader["cursusid"], (string)reader["cursusnaam"]);
                        studenten[studentId].Cursussen.Add(c);
                    }
                    reader.Close();
                }
                catch (Exception ex) { Console.WriteLine(ex.Message); }
            }
            return studenten.Values.ToList();
        }
        public List<Student> ZoekStudentenExtra(string text)
        {
            Dictionary<int, Student> studenten = new Dictionary<int, Student>();
            Dictionary<int,Cursus> cursussen=new Dictionary<int, Cursus>();
            string sql = "select t1.*,t2.*,t3.cursusnaam,t4.klasnaam,t4.lokaal from student t1 left join student_cursus t2 on t1.id=t2.studentid left join cursus t3 on t3.id=t2.cursusid left join klas t4 on t4.id=t1.klasId where t1.naam like @text";
            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand command = conn.CreateCommand())
            {
                try
                {
                    conn.Open();
                    command.CommandText = sql;
                    command.Parameters.AddWithValue("@text", text + "%");
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        int studentId = (int)reader["id"];
                        if (!studenten.ContainsKey(studentId))
                        {
                            Klas k = new Klas((int)reader["klasid"], (string)reader["klasnaam"]);
                            if (!reader.IsDBNull(reader.GetOrdinal("lokaal")))
                                k.Lokaal = (string)reader["lokaal"];
                            else k.Lokaal = "tzal buiten te doen zijn";
                            Student s = new Student(studentId, (string)reader["naam"], k);
                            studenten.Add(studentId, s);
                        }
                        int cursusid = (int)reader["cursusid"];
                        if (cursussen.ContainsKey(cursusid))
                        {
                            cursussen[cursusid].Studenten.Add(studenten[studentId]);
                        }
                        else
                        {
                            Cursus c = new Cursus(cursusid, (string)reader["cursusnaam"]);
                            cursussen.Add(cursusid, c);
                            studenten[studentId].Cursussen.Add(c);
                            c.Studenten.Add(studenten[studentId]);
                        }
                    }
                    reader.Close();
                }
                catch (Exception ex) { Console.WriteLine(ex.Message); }
            }

            return studenten.Values.ToList();
        }

        //public void VoegKlasToe(Klas klas)
        //{
        //    string sql = "INSERT INTO dbo.klas(klasnaam,lokaal) VALUES (@klasnaam,@lokaal)";
        //    using (SqlConnection connection = new SqlConnection(connectionString))
        //    using (SqlCommand command = connection.CreateCommand())
        //    {
        //        try
        //        {
        //            connection.Open();
        //            command.CommandText = sql;
        //            command.Parameters.AddWithValue("@klasnaam",klas.Klasnaam);
        //            if (klas.Lokaal==null)
        //                command.Parameters.AddWithValue("@lokaal",DBNull.Value);
        //            else
        //                command.Parameters.AddWithValue("@lokaal", klas.Lokaal);
        //            command.ExecuteNonQuery();
        //        }
        //        catch (Exception ex)
        //        {
        //            Console.WriteLine(ex.ToString());
        //        }
        //    }
        //}
        //public Klas GeefKlas(int id)
        //{
        //    string sql = "SELECT * FROM klas WHERE id=@id";
        //    using (SqlConnection connection = new SqlConnection(connectionString))
        //    using (SqlCommand command = connection.CreateCommand())
        //    {
        //        try
        //        {
        //            connection.Open();
        //            command.CommandText = sql;
        //            command.Parameters.AddWithValue("@id", id);
        //            SqlDataReader reader = command.ExecuteReader();
        //            reader.Read();
        //            Klas klas = new Klas((int)reader["id"], (string)reader["klasnaam"]);
        //            if (!reader.IsDBNull(reader.GetOrdinal("lokaal"))) klas.Lokaal = (string)reader["lokaal"];
        //            reader.Close();
        //            return klas;
        //        }
        //        catch (Exception ex)
        //        {
        //            Console.WriteLine(ex.ToString());
        //            throw;
        //        }
        //    }
        //}


    }
}
