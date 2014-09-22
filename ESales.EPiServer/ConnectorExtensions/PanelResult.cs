using System.Collections.Generic;
using Apptus.ESales.Connector;

namespace Apptus.ESales.EPiServer.ConnectorExtensions
{
    public class PanelResult<T> where T : Result
    {

        public PanelResult(IDictionary<string, string> attributes, T result, string ticket)
        {
            Attributes = attributes;
            Result = result;
            Ticket = ticket;
            ErrorMessage = null;
        }

        public PanelResult(string errorMessage )
        {
            Attributes = null;
            Result = null;
            Ticket = null;
            ErrorMessage = errorMessage;
        }

        public IDictionary<string, string> Attributes { get; private set; }

        public T Result { get; private set; }

        public string Ticket { get; private set; }

        public string ErrorMessage { get; private set; }

        public bool HasError
        {
            get { return ErrorMessage != null; }
        }
    }
}
