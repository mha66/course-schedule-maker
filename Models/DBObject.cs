using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseScheduleMaker.Models
{
    public class DBObject
    {
        public int Id { get; set; }
        public virtual string? Name { get; set; }

        public DBObject()
        {
            Id = -1;
        }

        public DBObject(int id)
        {
            Id = id;
        }

        public DBObject(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }
        public DBObject(int id, string name)
        {
            Id = id;
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        public DBObject(DBObject dbObject)
        {
            Id = dbObject.Id;
            Name = dbObject.Name ?? throw new ArgumentNullException(nameof(dbObject.Name));
        }

        public override string? ToString()
        {
            return $"ID:{Id}, Name:{Name}";
        }
    }
}
