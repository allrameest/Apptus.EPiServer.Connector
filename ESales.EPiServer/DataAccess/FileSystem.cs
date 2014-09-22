using System.IO;

namespace Apptus.ESales.EPiServer.DataAccess
{
    internal class FileSystem : IFileSystem
    {
        public Stream Open( string path, FileMode mode, FileAccess access )
        {
            var file = new FileInfo( path );
            CreateDirectoryIfMissing( file );
            return file.Open( mode, access );
        }

        public void Copy( string source, string destination )
        {
            File.Copy( source, destination );
        }

        private static void CreateDirectoryIfMissing( FileInfo file )
        {
            var directory = file.Directory;
            if ( directory != null )
            {
                directory.Create();
            }
        }
    }
}