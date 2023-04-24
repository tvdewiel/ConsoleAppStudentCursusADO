using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleAppStudentCursusADO
{
    public class Klas
    {
        public Klas(string klasnaam)
        {
            this.Klasnaam = klasnaam;
        }

        public Klas(int id, string klasnaam)
        {
            Id = id;
            this.Klasnaam = klasnaam;
        }

        public int Id { get; set; }
        public string Klasnaam { get; set; }
        public string Lokaal { get; set; }
    }
}
