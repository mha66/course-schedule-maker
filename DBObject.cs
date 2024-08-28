using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseScheduleMaker
{
    internal class DBObject
    {
        public int Id { get; set; }
        public virtual string? Name { get; set; }

        public DBObject() 
        {
            Id = -1;
            Name = "database object";
        }

        public DBObject(int id)
        {
            Id = id;
        }

        public DBObject(int id, string name)
        {
            Id = id;
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        public override string? ToString()
        {
            return $"ID:{Id}, Name:{Name}";
        }
    }
}
