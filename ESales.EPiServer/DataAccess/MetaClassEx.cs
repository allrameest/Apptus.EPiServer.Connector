using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mediachase.MetaDataPlus.Configurator;

namespace Apptus.ESales.EPiServer.DataAccess
{
    internal class MetaClassEx : IEnumerable<MetaFieldEx>
    {
        private readonly IEnumerable<MetaFieldEx> _fields;

        public MetaClassEx( int id, string name, IEnumerable<MetaFieldEx> fields )
        {
            _fields = fields ?? Enumerable.Empty<MetaFieldEx>();
            Id = id;
            Name = name;
        }

        public MetaClassEx( MetaClass c ) : this( c.Id, c.Name, c.MetaFields.Cast<MetaField>().Select( f => new MetaFieldEx( f ) ) )
        {
        }

        public int Id { get; private set; }
        public string Name { get; private set; }

        public IEnumerator<MetaFieldEx> GetEnumerator()
        {
            return _fields.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
