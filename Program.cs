namespace ConsoleAppStudentCursusADO
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
            string connectionString = @"Data Source=NB21-6CDPYD3\SQLEXPRESS;Initial Catalog=studentenCursussen;Integrated Security=True";
            Databeheer db = new Databeheer(connectionString);
            //Cursus c1 = new Cursus("c++");
            //Cursus c2 = new Cursus("fortran");
            //db.VoegCursusToe(c1);
            //db.VoegCursusToe(c2);
            //var c = db.GeefCursus(2);
            //Console.WriteLine(c);
            //var cs = db.GeefCursussen();
            ////foreach(Cursus cu in cs) Console.WriteLine(cu);
            //Klas klas = new Klas(1, "AKlas");
            //Student student = new Student("jos", klas);
            //student.Cursussen = cs;
            //db.VoegStudentMetCursussenToe(student);
            //Klas k = new Klas("KlasD");
            //k.Lokaal = "grote zaal";
            //db.VoegKlasToe(k);
            //Klas kk = db.GeefKlas(4);

            //Klas k1 = new Klas("S-klas");
            //k1.Lokaal = "2.047";
            //db.VoegKlasToe(k1);
            //var klassen = db.GeefKlassen();
            var x = db.ZoekStudentenExtra("j");
        }
    }
}