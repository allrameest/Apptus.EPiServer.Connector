using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using Apptus.ESales.Connector;

namespace Apptus.ESales.EPiServer.Web.Controls
{
    public class PanelHierarchicalEnumerable : IHierarchicalEnumerable, IEnumerable<PanelHierarchyData>
    {

        private readonly int _indent;
        private readonly PanelHierarchyData _parentZone;
        private readonly IList<PanelContent> _panels;

        internal PanelHierarchicalEnumerable() : this(null, new PanelContent[] {}) {}

        internal PanelHierarchicalEnumerable(PanelContent panel) : this(null, new[]{panel}) { }

        internal PanelHierarchicalEnumerable(PanelHierarchyData parentZone, PanelContent panel) : this(parentZone, new[] {panel}) { }

        internal PanelHierarchicalEnumerable(PanelHierarchyData parentZone, IList<PanelContent> panels)
        {
            _indent = parentZone == null ? 0 : parentZone.Indent + 1;
            _parentZone = parentZone;
            _panels = panels;
        }

        public IEnumerator<PanelHierarchyData> GetEnumerator()
        {
            return _panels.Select(panel => new PanelHierarchyData(_parentZone, panel, Indent)).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IHierarchyData GetHierarchyData(object enumeratedItem)
        {
            if (!(enumeratedItem is PanelHierarchyData))
            {
                throw new ArgumentException("Enumerated item is not a PanelHierarchyData object");
            }
            return enumeratedItem as PanelHierarchyData;
        }

        public IHierarchyData Parent
        {
            get { return _parentZone; }
        }

        public int Indent
        {
            get { return _indent; }
        }
    }
}
