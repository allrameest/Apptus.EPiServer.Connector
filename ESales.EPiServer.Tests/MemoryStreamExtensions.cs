using System.IO;
using System.Linq;

namespace ESales.EPiServer.Tests
{
    internal static class MemoryStreamExtensions
    {
        public static bool IsOpen( this MemoryStream memoryStream )
        {
            return memoryStream.CanRead;
        }

        public static byte[] ToArrayWithData( this MemoryStream memoryStream )
        {
            var bytes = memoryStream.ToArray();
            int lastIndexWithData;
            for ( lastIndexWithData = bytes.Length - 1; lastIndexWithData >= 0; lastIndexWithData-- )
            {
                if ( bytes[lastIndexWithData] != 0 )
                {
                    break;
                }
            }
            return bytes.Take( lastIndexWithData + 1 ).ToArray();
        }

        public static Stream CreateNewOrGetExistingStream( this MemoryStream memoryStream )
        {
            if ( !memoryStream.IsOpen() )
            {
                memoryStream = new MemoryStream( memoryStream.ToArrayWithData() );
            }
            return memoryStream;
        }
    }
}
