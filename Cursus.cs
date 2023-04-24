using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleAppStudentCursusADO
{
    public class Cursus
    {
        public Cursus(string cursusnaam)
        {
            this.Cursusnaam = cursusnaam;
        }
        public Cursus(int id, string cursusnaam)
        {
            Id = id;
            this.Cursusnaam = cursusnaam;
        }
        public int Id { get; set; }
        public string Cursusnaam { get; set; }
        public List<Student> Studenten { get; set; }=new List<Student>();
        public override string ToString()
        {
            return $"{Id},{Cursusnaam}";
        }
    }
}
