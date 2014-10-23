using System.Collections.Generic;

namespace Apptus.ESales.EPiServer.Import.Products
{
    public interface IBatchedProductConverter
    {
        IReadOnlyCollection<IEntity> Convert(IReadOnlyCollection<IEntity> entities);
    }
}