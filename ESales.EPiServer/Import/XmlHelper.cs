using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Apptus.ESales.EPiServer.Import
{

    // =============================================
    //disable warning about no XML comments
    #pragma warning disable 1591
    #region product
    ///<summary>
    /// This class is a helper class that needs to be public for XML deserialization.
    ///</summary>
    [Serializable]
    [XmlRoot(ElementName = "product", Namespace = "")]
    public class ESalesProduct
    {
        private String _pk;

        [XmlElement(ElementName = "product_key")]
        public String ProductKey

        {
            get
            {
                return _pk;
            }
            set
            {
                _pk = value;
            }
        }

    }
    #endregion

    // =============================================
    ///<summary>
    /// This class is a helper class that needs to be public for XML deserialization.
    ///</summary>
    public class ESalesRangeValue
    {

        public ESalesRangeValue()
        {
            Descriptions = new ESalesDescriptions();
            Numeric = true;
            LowerBound = -2147483647;
            UpperBound = 2147483647;
        }

        [XmlAttribute("key")]
        public string Key
        { get; set; }

        [XmlAttribute("numeric")]
        public bool Numeric
        { get; set; }

        [XmlAttribute("lowerbound")]
        public decimal LowerBound
        { get; set; }

        [XmlAttribute("upperbound")]
        public decimal UpperBound
        { get; set; }

        [XmlElement("Descriptions")]
        public ESalesDescriptions Descriptions
        { get; set; }

    }

    ///<summary>
    /// This class is a helper class that needs to be public for XML deserialization.
    ///</summary>
    public class ESalesPriceRangeValue
    {

        public ESalesPriceRangeValue()
        {
            Descriptions = new ESalesDescriptions();
            LowerBound = -2147483647;
            UpperBound = 2147483647;
            LowerBoundIncluded = false;
            UpperBoundIncluded = true;

        }

        [XmlAttribute("key")]
        public string Key
        { get; set; }

        [XmlAttribute("currency")]
        public string Currency
        { get; set; }

        [XmlAttribute("lowerbound")]
        public decimal LowerBound
        { get; set; }

        [XmlAttribute("upperbound")]
        public decimal UpperBound
        { get; set; }

        [XmlAttribute("lowerboundincluded")]
        public bool LowerBoundIncluded
        { get; set; }

        [XmlAttribute("upperboundincluded")]
        public bool UpperBoundIncluded
        { get; set; }

        [XmlElement("Descriptions")]
        public ESalesDescriptions Descriptions
        { get; set; }

    }

    ///<summary>
    /// This class is a helper class that needs to be public for XML deserialization.
    ///</summary>
    public class ESalesSimpleValue
    {

        public ESalesSimpleValue()
        {
            Descriptions = new ESalesDescriptions();
        }

        [XmlAttribute("key")]
        public string Key
        { get; set; }

        [XmlAttribute("value")]
        public string Value
        { get; set; }

        [XmlElement("Descriptions")]
        public ESalesDescriptions Descriptions
        { get; set; }

    }

    ///<summary>
    /// This class is a helper class that needs to be public for XML deserialization.
    ///</summary>
    public class ESalesValues
    {

        public ESalesValues()
        {
            SimpleValue = new List<ESalesSimpleValue>();
            PriceRangeValue = new List<ESalesPriceRangeValue>();
            RangeValue = new List<ESalesRangeValue>();
        }

        [XmlElement("SimpleValue")]
        public List<ESalesSimpleValue> SimpleValue
        { get; set; }

        [XmlElement("PriceRangeValue")]
        public List<ESalesPriceRangeValue> PriceRangeValue
        { get; set; }

        [XmlElement("RangeValue")]
        public List<ESalesRangeValue> RangeValue
        { get; set; }

    }

    ///<summary>
    /// This class is a helper class that needs to be public for XML deserialization.
    ///</summary>
    public class ESalesDescription
    {

        [XmlAttribute("locale")]
        public string Locale
        { get; set; }

        [XmlText]
        public string Value
        { get; set; }

    }

    ///<summary>
    /// This class is a helper class that needs to be public for XML deserialization.
    ///</summary>
    public class ESalesDescriptions
    {

        public ESalesDescriptions()
        {
            Description = new List<ESalesDescription>();
        }

        [XmlAttribute("defaultLocale")]
        public string DefaultLocale
        { get; set; }

        [XmlElement("Description")]
        public List<ESalesDescription> Description
        { get; set; }

    }

    ///<summary>
    /// This class is a helper class that needs to be public for XML deserialization.
    ///</summary>
    public class ESalesFilter
    {

        public ESalesFilter()
        {
            Descriptions = new ESalesDescriptions();
            Values = new ESalesValues();
        }

        [XmlAttribute("field")]
        public string Field
        { get; set; }

        [XmlElement("Descriptions")]
        public ESalesDescriptions Descriptions
        { get; set; }

        [XmlElement("Values")]
        public ESalesValues Values
        { get; set; }

    }

    ///<summary>
    /// This class is a helper class that needs to be public for XML deserialization.
    ///</summary>
    public class ESalesSearchFilters
    {

        public ESalesSearchFilters()
        {
            Filter = new List<ESalesFilter>();
        }

        [XmlElement("Filter")]
        public List<ESalesFilter> Filter
        { get; set; }

    }

    ///<summary>
    /// This class is a helper class that needs to be public for XML deserialization.
    ///</summary>
    [Serializable]
    [XmlRoot(ElementName = "SearchConfig")]
    public class ESalesSearchConfig
    {

        [XmlElement("SearchFilters")]
        public ESalesSearchFilters SearchFilters
        { get; set; }

    }

    ///<summary>
    /// This class is a helper class that needs to be public for XML deserialization.
    ///</summary>
    public class ESalesFileDataElement
    {

        public ESalesFileDataElement(string name, string value)
        {
            Value = new List<String>();
            Name = name;
            Value.Add(value);
        }

        public string Name
        { get; set; }

        public List<String> Value
        { get; set; }

    }

    ///<summary>
    /// This class is a helper class that needs to be public for XML deserialization.
    ///</summary>
    public class ESalesFilterKeyValuePair
    {

        public ESalesFilterKeyValuePair(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public string Name
        { get; set; }

        public string Value
        { get; set; }
    }

    #pragma warning restore 1591
}

