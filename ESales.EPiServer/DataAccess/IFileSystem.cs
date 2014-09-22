using System.IO;

namespace Apptus.ESales.EPiServer.DataAccess
{
    internal interface IFileSystem
    {
        Stream Open( string path, FileMode mode, FileAccess access );
        void Copy( string source, string destination );
    }
}