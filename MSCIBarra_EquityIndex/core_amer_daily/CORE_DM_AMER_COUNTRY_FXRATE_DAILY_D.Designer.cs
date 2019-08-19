// ------------------------------------------------------------------------------
//  <auto-generated>
//    Generated by Xsd2Code. Version 3.4.0.38967
//    <NameSpace>MSCIBarra_EquityIndex.core_amer_daily</NameSpace><Collection>List</Collection><codeType>CSharp</codeType><EnableDataBinding>False</EnableDataBinding><EnableLazyLoading>False</EnableLazyLoading><TrackingChangesEnable>False</TrackingChangesEnable><GenTrackingClasses>False</GenTrackingClasses><HidePrivateFieldInIDE>False</HidePrivateFieldInIDE><EnableSummaryComment>False</EnableSummaryComment><VirtualProp>False</VirtualProp><IncludeSerializeMethod>False</IncludeSerializeMethod><UseBaseClass>False</UseBaseClass><GenBaseClass>False</GenBaseClass><GenerateCloneMethod>False</GenerateCloneMethod><GenerateDataContracts>False</GenerateDataContracts><CodeBaseTag>Net40</CodeBaseTag><SerializeMethodName>Serialize</SerializeMethodName><DeserializeMethodName>Deserialize</DeserializeMethodName><SaveToFileMethodName>SaveToFile</SaveToFileMethodName><LoadFromFileMethodName>LoadFromFile</LoadFromFileMethodName><GenerateXMLAttributes>True</GenerateXMLAttributes><EnableEncoding>False</EnableEncoding><AutomaticProperties>False</AutomaticProperties><GenerateShouldSerialize>False</GenerateShouldSerialize><DisableDebug>False</DisableDebug><PropNameSpecified>Default</PropNameSpecified><Encoder>UTF8</Encoder><CustomUsings></CustomUsings><ExcludeIncludedTypes>False</ExcludeIncludedTypes><EnableInitializeFields>True</EnableInitializeFields>
//  </auto-generated>
// ------------------------------------------------------------------------------
namespace MSCIBarra_EquityIndex.core_amer_daily {
    using System;
    using System.Diagnostics;
    using System.Xml.Serialization;
    using System.Collections;
    using System.Xml.Schema;
    using System.ComponentModel;
    using System.Collections.Generic;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://xml.mscibarra.com/ns/msci/deal/D_5A")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="http://xml.mscibarra.com/ns/msci/deal/D_5A", IsNullable=false)]
    public partial class package_D_5A {
        
        private List<package_D_5AEntry> dataset_D_5AField;
        
        public package_D_5A() {
            this.dataset_D_5AField = new List<package_D_5AEntry>();
        }
        
        [System.Xml.Serialization.XmlArrayAttribute(Order=0)]
        [System.Xml.Serialization.XmlArrayItemAttribute("entry", IsNullable=false)]
        public List<package_D_5AEntry> dataset_D_5A {
            get {
                return this.dataset_D_5AField;
            }
            set {
                this.dataset_D_5AField = value;
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://xml.mscibarra.com/ns/msci/deal/D_5A")]
    public partial class package_D_5AEntry {
        
        private System.DateTime calc_dateField;
        
        private string iSO_currency_symbolField;
        
        private decimal spot_fx_eod00dField;
        
        private decimal forward_fx_30_eod00dField;
        
        private bool forward_fx_30_eod00dFieldSpecified;
        
        private decimal covering_costField;
        
        private bool covering_costFieldSpecified;
        
        [System.Xml.Serialization.XmlAttributeAttribute(DataType="date")]
        public System.DateTime calc_date {
            get {
                return this.calc_dateField;
            }
            set {
                this.calc_dateField = value;
            }
        }
        
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string ISO_currency_symbol {
            get {
                return this.iSO_currency_symbolField;
            }
            set {
                this.iSO_currency_symbolField = value;
            }
        }
        
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal spot_fx_eod00d {
            get {
                return this.spot_fx_eod00dField;
            }
            set {
                this.spot_fx_eod00dField = value;
            }
        }
        
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal forward_fx_30_eod00d {
            get {
                return this.forward_fx_30_eod00dField;
            }
            set {
                this.forward_fx_30_eod00dField = value;
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool forward_fx_30_eod00dSpecified {
            get {
                return this.forward_fx_30_eod00dFieldSpecified;
            }
            set {
                this.forward_fx_30_eod00dFieldSpecified = value;
            }
        }
        
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal covering_cost {
            get {
                return this.covering_costField;
            }
            set {
                this.covering_costField = value;
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool covering_costSpecified {
            get {
                return this.covering_costFieldSpecified;
            }
            set {
                this.covering_costFieldSpecified = value;
            }
        }
    }
}
