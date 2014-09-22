using Mediachase.MetaDataPlus.Configurator;
using Mediachase.Search.Extensions;

namespace Apptus.ESales.EPiServer.DataAccess
{
    internal class MetaFieldEx
    {
        public MetaFieldEx( string ns, string name, string friendlyName, string description, MetaDataType dataType, int length, bool allowNulls,
                            bool multiLanguageValue, bool allowSearch, bool isEncrypted, bool presentable, bool sortable, bool tokenized )
        {
            Namespace = ns;
            Name = name;
            FriendlyName = friendlyName;
            Description = description;
            DataType = dataType;
            Length = length;
            AllowNulls = allowNulls;
            MultiLanguageValue = multiLanguageValue;
            AllowSearch = allowSearch;
            IsEncrypted = isEncrypted;
            Presentable = presentable;
            Sortable = sortable;
            Tokenized = tokenized;
        }

        public MetaFieldEx( MetaField f )
            : this( f.Namespace, f.Name, f.FriendlyName, f.Description, f.DataType, f.Length, f.AllowNulls, f.MultiLanguageValue, f.SafeAllowSearch,
                    f.IsEncrypted, f.IsIndexStored(), f.IsIndexSortable(), f.IsIndexTokenized() )
        {
        }

        public string Namespace { get; private set; }
        public string Name { get; private set; }
        public string FriendlyName { get; private set; }
        public string Description { get; private set; }
        public MetaDataType DataType { get; private set; }
        public int Length { get; private set; }
        public bool AllowNulls { get; private set; }
        public bool MultiLanguageValue { get; private set; }
        public bool AllowSearch { get; private set; }
        public bool IsEncrypted { get; private set; }
        public bool Presentable { get; private set; }
        public bool Sortable { get; private set; }
        public bool Tokenized { get; private set; }
    }
}
