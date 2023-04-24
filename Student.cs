using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleAppStudentCursusADO
{
    public class Student
    {
        public Student(string studentnaam, Klas klas)
        {
            Studentnaam = studentnaam;
            Klas = klas;
        }

        public Student(int id, string studentnaam, Klas klas)
        {
            Id = id;
            Studentnaam = studentnaam;
            Klas = klas;
        }

        public int Id { get; set; }
        public string Studentnaam { get; set; }
        public Klas Klas { get; set; }
        public List<Cursus> Cursussen { get; set; }=new List<Cursus>();
    }
}
