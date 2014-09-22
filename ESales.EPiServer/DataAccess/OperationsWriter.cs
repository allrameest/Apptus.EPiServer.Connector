using System;
using System.IO;
using System.Xml;
using Apptus.ESales.EPiServer.Import;

namespace Apptus.ESales.EPiServer.DataAccess
{
    internal class OperationsWriter : IOperationsWriter
    {
        private bool _disposed;
        private bool _cleared;
        private bool _removed;
        private readonly FileHelper _fileHelper;
        private readonly Lazy<XmlWriter> _addWriter;
        private readonly Lazy<XmlWriter> _fullWriter;
        private readonly Lazy<XmlWriter> _delWriter;

        public OperationsWriter( IFileSystem fileSystem, FileHelper fileHelper )
        {
            _fileHelper = fileHelper;
            _disposed = false;
            _cleared = false;
            _removed = false;
            _fullWriter = new Lazy<XmlWriter>(
                () => XmlWriter.Create( fileSystem.Open( fileHelper.ProductsFile, FileMode.CreateNew, FileAccess.Write ),
                                        new XmlWriterSettings
                                            {
                                                Indent = false,
                                                OmitXmlDeclaration = false,
                                                CloseOutput = true
                                            } ) );
            _addWriter = new Lazy<XmlWriter>(
                () => XmlWriter.Create( fileSystem.Open( fileHelper.ProductsAddFile, FileMode.CreateNew, FileAccess.Write ),
                                        new XmlWriterSettings
                                            {
                                                CheckCharacters = true,
                                                Indent = false,
                                                OmitXmlDeclaration = true,
                                                CloseOutput = true
                                            } ) );
            _delWriter = new Lazy<XmlWriter>(
                () => XmlWriter.Create( fileSystem.Open( fileHelper.ProductsDelFile, FileMode.CreateNew, FileAccess.Write ),
                                        new XmlWriterSettings
                                            {
                                                Indent = false,
                                                OmitXmlDeclaration = true,
                                                CloseOutput = true
                                            } ) );
        }

        public void Add( IEntity entity )
        {
            if ( entity == null )
            {
                throw new ArgumentNullException( "entity" );
            }

            if ( !_addWriter.IsValueCreated )
            {
                _addWriter.Value.WriteStartDocument();
                _addWriter.Value.WriteStartElement( "add" );
            }

            var writer = _addWriter.Value;
            writer.WriteStartElement( entity.Name );
            foreach ( var attribute in entity )
            {
                writer.WriteElementString( attribute.Name, attribute.Value );
            }
            writer.WriteEndElement();
        }

        public void Remove( IEntity document )
        {
            if ( document == null )
            {
                throw new ArgumentNullException( "document" );
            }
            if ( _cleared )
            {
                throw new InvalidOperationException( "Clear and remove can not be called in the same import." );
            }
            _removed = true;

            if ( !_delWriter.IsValueCreated )
            {
                _delWriter.Value.WriteStartDocument();
                _delWriter.Value.WriteStartElement( "remove" );
            }

            var writer = _delWriter.Value;
            writer.WriteStartElement( document.Name );
            writer.WriteElementString( document.Key.Name, document.Key.Value );
            writer.WriteEndElement();
        }

        public void Clear( string name )
        {
            if ( name == null )
            {
                throw new ArgumentNullException( "name" );
            }
            if ( _removed )
            {
                throw new InvalidOperationException( "Clear and remove can not be called in the same import." );
            }
            _cleared = true;

            if ( !_delWriter.IsValueCreated )
            {
                var writer = _delWriter.Value;
                writer.WriteStartDocument();
                writer.WriteStartElement( "clear" );
                writer.WriteElementString( name, "" );
                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
        }

        public void Dispose()
        {
            if ( _disposed )
            {
                return;
            }

            if ( _delWriter.IsValueCreated )
            {
                if ( _removed )
                {
                    ClosePartWriter( _delWriter.Value );
                }
                _delWriter.Value.Close();
            }

            if ( _addWriter.IsValueCreated )
            {
                ClosePartWriter( _addWriter.Value );
                _addWriter.Value.Close();
            }

            using ( var delReader = _delWriter.IsValueCreated ? XmlReader.Create( _fileHelper.ProductsDelFile ) : XmlReader.Create( new StringReader( "<remove/>" ) ) )
            using ( var addReader = _addWriter.IsValueCreated ? XmlReader.Create( _fileHelper.ProductsAddFile ) : XmlReader.Create( new StringReader( "<add/>" ) ) )
            using ( var fullWriter = _fullWriter.Value )
            {
                fullWriter.WriteStartDocument();
                fullWriter.WriteStartElement( "operations" );
                fullWriter.WriteNode( delReader, false );
                fullWriter.WriteNode( addReader, false );
                fullWriter.WriteEndElement();
                fullWriter.WriteEndDocument();
            }

            File.Delete( _fileHelper.ProductsDelFile );
            File.Delete( _fileHelper.ProductsAddFile );

            _disposed = true;
        }

        private static void ClosePartWriter( XmlWriter writer )
        {
            writer.WriteEndElement();
            writer.WriteEndDocument();
        }
    }
}