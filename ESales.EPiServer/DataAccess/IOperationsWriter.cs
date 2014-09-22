using System;
using Apptus.ESales.EPiServer.Import;

namespace Apptus.ESales.EPiServer.DataAccess
{
    internal interface IOperationsWriter : IDisposable
    {
        void Add( IEntity entity );
        void Remove( IEntity document );
        void Clear( string name );
    }
}