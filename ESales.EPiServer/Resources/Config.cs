﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18444
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System.Xml.Serialization;

// 
// This source code was auto-generated by xsd, Version=4.0.30319.1.
// 


/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
[System.Xml.Serialization.XmlRootAttribute(Namespace="", IsNullable=false)]
public partial class configuration {
    
    private attribute[] attributesField;
    
    private attribute[] product_attributesField;
    
    private attribute[] ad_attributesField;
    
    private search_refinements search_refinementsField;
    
    private format[] formatsField;
    
    private tokenization[] tokenizationsField;
    
    private normalization[] normalizationsField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlArrayItemAttribute("attribute", IsNullable=false)]
    public attribute[] attributes {
        get {
            return this.attributesField;
        }
        set {
            this.attributesField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlArrayItemAttribute("attribute", IsNullable=false)]
    public attribute[] product_attributes {
        get {
            return this.product_attributesField;
        }
        set {
            this.product_attributesField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlArrayItemAttribute("attribute", IsNullable=false)]
    public attribute[] ad_attributes {
        get {
            return this.ad_attributesField;
        }
        set {
            this.ad_attributesField = value;
        }
    }
    
    /// <remarks/>
    public search_refinements search_refinements {
        get {
            return this.search_refinementsField;
        }
        set {
            this.search_refinementsField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlArrayItemAttribute("format", IsNullable=false)]
    public format[] formats {
        get {
            return this.formatsField;
        }
        set {
            this.formatsField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlArrayItemAttribute("tokenization", IsNullable=false)]
    public tokenization[] tokenizations {
        get {
            return this.tokenizationsField;
        }
        set {
            this.tokenizationsField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlArrayItemAttribute("normalization", IsNullable=false)]
    public normalization[] normalizations {
        get {
            return this.normalizationsField;
        }
        set {
            this.normalizationsField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
[System.Xml.Serialization.XmlRootAttribute(Namespace="", IsNullable=false)]
public partial class attribute {
    
    private filter_attribute[] filter_attributesField;
    
    private search_attribute[] search_attributesField;
    
    private sort_attribute[] sort_attributesField;
    
    private present presentField;
    
    private string nameField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlArrayItemAttribute("filter_attribute", IsNullable=false)]
    public filter_attribute[] filter_attributes {
        get {
            return this.filter_attributesField;
        }
        set {
            this.filter_attributesField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlArrayItemAttribute("search_attribute", IsNullable=false)]
    public search_attribute[] search_attributes {
        get {
            return this.search_attributesField;
        }
        set {
            this.search_attributesField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlArrayItemAttribute("sort_attribute", IsNullable=false)]
    public sort_attribute[] sort_attributes {
        get {
            return this.sort_attributesField;
        }
        set {
            this.sort_attributesField = value;
        }
    }
    
    /// <remarks/>
    public present present {
        get {
            return this.presentField;
        }
        set {
            this.presentField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute(DataType="NCName")]
    public string name {
        get {
            return this.nameField;
        }
        set {
            this.nameField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
[System.Xml.Serialization.XmlRootAttribute(Namespace="", IsNullable=false)]
public partial class filter_attribute {
    
    private format formatField;
    
    private tokenization tokenizationField;
    
    private type typeField;
    
    private string nameField;
    
    /// <remarks/>
    public format format {
        get {
            return this.formatField;
        }
        set {
            this.formatField = value;
        }
    }
    
    /// <remarks/>
    public tokenization tokenization {
        get {
            return this.tokenizationField;
        }
        set {
            this.tokenizationField = value;
        }
    }
    
    /// <remarks/>
    public type type {
        get {
            return this.typeField;
        }
        set {
            this.typeField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute(DataType="NCName")]
    public string name {
        get {
            return this.nameField;
        }
        set {
            this.nameField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
[System.Xml.Serialization.XmlRootAttribute(Namespace="", IsNullable=false)]
public partial class format {
    
    private rule[] rulesField;
    
    private string nameField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlArrayItemAttribute("rule", IsNullable=false)]
    public rule[] rules {
        get {
            return this.rulesField;
        }
        set {
            this.rulesField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string name {
        get {
            return this.nameField;
        }
        set {
            this.nameField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
[System.Xml.Serialization.XmlRootAttribute(Namespace="", IsNullable=false)]
public partial class rule {
    
    private string typeField;
    
    private string classField;
    
    private string definitionField;
    
    private int priorityField;
    
    /// <remarks/>
    public string type {
        get {
            return this.typeField;
        }
        set {
            this.typeField = value;
        }
    }
    
    /// <remarks/>
    public string @class {
        get {
            return this.classField;
        }
        set {
            this.classField = value;
        }
    }
    
    /// <remarks/>
    public string definition {
        get {
            return this.definitionField;
        }
        set {
            this.definitionField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public int priority {
        get {
            return this.priorityField;
        }
        set {
            this.priorityField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
[System.Xml.Serialization.XmlRootAttribute(Namespace="", IsNullable=false)]
public partial class tokenization {
    
    private tokenizationProduct productField;
    
    private tokenizationQuery queryField;
    
    private string nameField;
    
    /// <remarks/>
    public tokenizationProduct product {
        get {
            return this.productField;
        }
        set {
            this.productField = value;
        }
    }
    
    /// <remarks/>
    public tokenizationQuery query {
        get {
            return this.queryField;
        }
        set {
            this.queryField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string name {
        get {
            return this.nameField;
        }
        set {
            this.nameField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
public partial class tokenizationProduct {
    
    private @default defaultField;
    
    private localized[] localizedField;
    
    /// <remarks/>
    public @default @default {
        get {
            return this.defaultField;
        }
        set {
            this.defaultField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("localized")]
    public localized[] localized {
        get {
            return this.localizedField;
        }
        set {
            this.localizedField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
[System.Xml.Serialization.XmlRootAttribute(Namespace="", IsNullable=false)]
public partial class @default {
    
    private rule[] rulesField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlArrayItemAttribute("rule", IsNullable=false)]
    public rule[] rules {
        get {
            return this.rulesField;
        }
        set {
            this.rulesField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
[System.Xml.Serialization.XmlRootAttribute(Namespace="", IsNullable=false)]
public partial class localized {
    
    private rule[] rulesField;
    
    private string localeField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlArrayItemAttribute("rule", IsNullable=false)]
    public rule[] rules {
        get {
            return this.rulesField;
        }
        set {
            this.rulesField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string locale {
        get {
            return this.localeField;
        }
        set {
            this.localeField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
public partial class tokenizationQuery {
    
    private @default defaultField;
    
    private localized[] localizedField;
    
    /// <remarks/>
    public @default @default {
        get {
            return this.defaultField;
        }
        set {
            this.defaultField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("localized")]
    public localized[] localized {
        get {
            return this.localizedField;
        }
        set {
            this.localizedField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
[System.SerializableAttribute()]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
[System.Xml.Serialization.XmlRootAttribute(Namespace="", IsNullable=false)]
public enum type {
    
    /// <remarks/>
    boolean,
    
    /// <remarks/>
    @byte,
    
    /// <remarks/>
    @short,
    
    /// <remarks/>
    @int,
    
    /// <remarks/>
    @long,
    
    /// <remarks/>
    @float,
    
    /// <remarks/>
    @double,
    
    /// <remarks/>
    @string,
    
    /// <remarks/>
    xml,
    
    /// <remarks/>
    [System.Xml.Serialization.XmlEnumAttribute("time point")]
    timepoint,
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
[System.Xml.Serialization.XmlRootAttribute(Namespace="", IsNullable=false)]
public partial class search_attribute {
    
    private format formatField;
    
    private bool match_suffixField;
    
    private bool suggestField;
    
    private string localeField;
    
    private string nameField;
    
    /// <remarks/>
    public format format {
        get {
            return this.formatField;
        }
        set {
            this.formatField = value;
        }
    }
    
    /// <remarks/>
    public bool match_suffix {
        get {
            return this.match_suffixField;
        }
        set {
            this.match_suffixField = value;
        }
    }
    
    /// <remarks/>
    public bool suggest {
        get {
            return this.suggestField;
        }
        set {
            this.suggestField = value;
        }
    }
    
    /// <remarks/>
    public string locale {
        get {
            return this.localeField;
        }
        set {
            this.localeField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute(DataType="NCName")]
    public string name {
        get {
            return this.nameField;
        }
        set {
            this.nameField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
[System.Xml.Serialization.XmlRootAttribute(Namespace="", IsNullable=false)]
public partial class sort_attribute {
    
    private normalization normalizationField;
    
    private type typeField;
    
    private string nameField;
    
    /// <remarks/>
    public normalization normalization {
        get {
            return this.normalizationField;
        }
        set {
            this.normalizationField = value;
        }
    }
    
    /// <remarks/>
    public type type {
        get {
            return this.typeField;
        }
        set {
            this.typeField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute(DataType="NCName")]
    public string name {
        get {
            return this.nameField;
        }
        set {
            this.nameField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
[System.Xml.Serialization.XmlRootAttribute(Namespace="", IsNullable=false)]
public partial class normalization {
    
    private @default defaultField;
    
    private localized[] localizedField;
    
    private string nameField;
    
    /// <remarks/>
    public @default @default {
        get {
            return this.defaultField;
        }
        set {
            this.defaultField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("localized")]
    public localized[] localized {
        get {
            return this.localizedField;
        }
        set {
            this.localizedField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string name {
        get {
            return this.nameField;
        }
        set {
            this.nameField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
[System.Xml.Serialization.XmlRootAttribute(Namespace="", IsNullable=false)]
public partial class present {
    
    private bool xmlField;
    
    private bool xmlFieldSpecified;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public bool xml {
        get {
            return this.xmlField;
        }
        set {
            this.xmlField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlIgnoreAttribute()]
    public bool xmlSpecified {
        get {
            return this.xmlFieldSpecified;
        }
        set {
            this.xmlFieldSpecified = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
[System.Xml.Serialization.XmlRootAttribute(Namespace="", IsNullable=false)]
public partial class search_refinements {
    
    private search_refinementsStrict strictField;
    
    private search_refinementsLoose looseField;
    
    /// <remarks/>
    public search_refinementsStrict strict {
        get {
            return this.strictField;
        }
        set {
            this.strictField = value;
        }
    }
    
    /// <remarks/>
    public search_refinementsLoose loose {
        get {
            return this.looseField;
        }
        set {
            this.looseField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
public partial class search_refinementsStrict {
    
    private tokenization tokenizationField;
    
    /// <remarks/>
    public tokenization tokenization {
        get {
            return this.tokenizationField;
        }
        set {
            this.tokenizationField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
public partial class search_refinementsLoose {
    
    private tokenization tokenizationField;
    
    /// <remarks/>
    public tokenization tokenization {
        get {
            return this.tokenizationField;
        }
        set {
            this.tokenizationField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
[System.Xml.Serialization.XmlRootAttribute(Namespace="", IsNullable=false)]
public partial class attributes {
    
    private attribute[] attributeField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("attribute")]
    public attribute[] attribute {
        get {
            return this.attributeField;
        }
        set {
            this.attributeField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
[System.Xml.Serialization.XmlRootAttribute(Namespace="", IsNullable=false)]
public partial class product_attributes {
    
    private attribute[] attributeField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("attribute")]
    public attribute[] attribute {
        get {
            return this.attributeField;
        }
        set {
            this.attributeField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
[System.Xml.Serialization.XmlRootAttribute(Namespace="", IsNullable=false)]
public partial class ad_attributes {
    
    private attribute[] attributeField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("attribute")]
    public attribute[] attribute {
        get {
            return this.attributeField;
        }
        set {
            this.attributeField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
[System.Xml.Serialization.XmlRootAttribute(Namespace="", IsNullable=false)]
public partial class filter_attributes {
    
    private filter_attribute[] filter_attributeField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("filter_attribute")]
    public filter_attribute[] filter_attribute {
        get {
            return this.filter_attributeField;
        }
        set {
            this.filter_attributeField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
[System.Xml.Serialization.XmlRootAttribute(Namespace="", IsNullable=false)]
public partial class search_attributes {
    
    private search_attribute[] search_attributeField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("search_attribute")]
    public search_attribute[] search_attribute {
        get {
            return this.search_attributeField;
        }
        set {
            this.search_attributeField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
[System.Xml.Serialization.XmlRootAttribute(Namespace="", IsNullable=false)]
public partial class sort_attributes {
    
    private sort_attribute[] sort_attributeField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("sort_attribute")]
    public sort_attribute[] sort_attribute {
        get {
            return this.sort_attributeField;
        }
        set {
            this.sort_attributeField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
[System.Xml.Serialization.XmlRootAttribute(Namespace="", IsNullable=false)]
public partial class formats {
    
    private format[] formatField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("format")]
    public format[] format {
        get {
            return this.formatField;
        }
        set {
            this.formatField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
[System.Xml.Serialization.XmlRootAttribute(Namespace="", IsNullable=false)]
public partial class tokenizations {
    
    private tokenization[] tokenizationField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("tokenization")]
    public tokenization[] tokenization {
        get {
            return this.tokenizationField;
        }
        set {
            this.tokenizationField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
[System.Xml.Serialization.XmlRootAttribute(Namespace="", IsNullable=false)]
public partial class normalizations {
    
    private normalization[] normalizationField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("normalization")]
    public normalization[] normalization {
        get {
            return this.normalizationField;
        }
        set {
            this.normalizationField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
[System.Xml.Serialization.XmlRootAttribute(Namespace="", IsNullable=false)]
public partial class rules {
    
    private rule[] ruleField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("rule")]
    public rule[] rule {
        get {
            return this.ruleField;
        }
        set {
            this.ruleField = value;
        }
    }
}
