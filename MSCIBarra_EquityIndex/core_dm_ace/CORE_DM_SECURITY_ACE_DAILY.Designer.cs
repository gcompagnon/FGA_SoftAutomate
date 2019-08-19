// ------------------------------------------------------------------------------
//  <auto-generated>
//    Generated by Xsd2Code. Version 3.4.0.38967
//    <NameSpace>MSCIBarra_EquityIndex.core_dm_ace</NameSpace><Collection>List</Collection><codeType>CSharp</codeType><EnableDataBinding>False</EnableDataBinding><EnableLazyLoading>False</EnableLazyLoading><TrackingChangesEnable>False</TrackingChangesEnable><GenTrackingClasses>False</GenTrackingClasses><HidePrivateFieldInIDE>False</HidePrivateFieldInIDE><EnableSummaryComment>False</EnableSummaryComment><VirtualProp>False</VirtualProp><IncludeSerializeMethod>False</IncludeSerializeMethod><UseBaseClass>False</UseBaseClass><GenBaseClass>False</GenBaseClass><GenerateCloneMethod>False</GenerateCloneMethod><GenerateDataContracts>False</GenerateDataContracts><CodeBaseTag>Net40</CodeBaseTag><SerializeMethodName>Serialize</SerializeMethodName><DeserializeMethodName>Deserialize</DeserializeMethodName><SaveToFileMethodName>SaveToFile</SaveToFileMethodName><LoadFromFileMethodName>LoadFromFile</LoadFromFileMethodName><GenerateXMLAttributes>True</GenerateXMLAttributes><EnableEncoding>False</EnableEncoding><AutomaticProperties>False</AutomaticProperties><GenerateShouldSerialize>False</GenerateShouldSerialize><DisableDebug>False</DisableDebug><PropNameSpecified>Default</PropNameSpecified><Encoder>UTF8</Encoder><CustomUsings></CustomUsings><ExcludeIncludedTypes>False</ExcludeIncludedTypes><EnableInitializeFields>True</EnableInitializeFields>
//  </auto-generated>
// ------------------------------------------------------------------------------
namespace MSCIBarra_EquityIndex.core_dm_ace {
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
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://xml.mscibarra.com/ns/msci/deal/DM_ACE")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="http://xml.mscibarra.com/ns/msci/deal/DM_ACE", IsNullable=false)]
    public partial class package_DM_ACE {
        
        private List<package_DM_ACEEntry> dataset_DAILY_FILE_DMField;
        
        public package_DM_ACE() {
            this.dataset_DAILY_FILE_DMField = new List<package_DM_ACEEntry>();
        }
        
        [System.Xml.Serialization.XmlArrayAttribute(Order=0)]
        [System.Xml.Serialization.XmlArrayItemAttribute("entry", IsNullable=false)]
        public List<package_DM_ACEEntry> dataset_DAILY_FILE_DM {
            get {
                return this.dataset_DAILY_FILE_DMField;
            }
            set {
                this.dataset_DAILY_FILE_DMField = value;
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://xml.mscibarra.com/ns/msci/deal/DM_ACE")]
    public partial class package_DM_ACEEntry {
        
        private string statusField;
        
        private decimal event_idField;
        
        private bool event_idFieldSpecified;
        
        private System.DateTime ex_dateField;
        
        private bool ex_dateFieldSpecified;
        
        private System.DateTime last_updated_dateField;
        
        private bool last_updated_dateFieldSpecified;
        
        private System.DateTime first_entry_dateField;
        
        private bool first_entry_dateFieldSpecified;
        
        private string current_countryField;
        
        private string new_countryField;
        
        private string current_sec_nameField;
        
        private string new_sec_nameField;
        
        private string msci_sec_codeField;
        
        private decimal timeserie_codeField;
        
        private bool timeserie_codeFieldSpecified;
        
        private string sedolField;
        
        private string isinField;
        
        private string bb_tickersField;
        
        private string event_typeField;
        
        private string event_descField;
        
        private string correction_commentField;
        
        private string termsField;
        
        private decimal text_ann_flagField;
        
        private bool text_ann_flagFieldSpecified;
        
        private decimal current_eod_nosField;
        
        private bool current_eod_nosFieldSpecified;
        
        private decimal new_eod_nosField;
        
        private bool new_eod_nosFieldSpecified;
        
        private decimal current_closing_nosField;
        
        private bool current_closing_nosFieldSpecified;
        
        private decimal new_closing_nosField;
        
        private bool new_closing_nosFieldSpecified;
        
        private string paf_formulaField;
        
        private decimal pafField;
        
        private bool pafFieldSpecified;
        
        private string current_ISO_currency_symbolField;
        
        private string new_ISO_currency_symbolField;
        
        private string current_gics_subind_codeField;
        
        private string new_gics_subindustry_codeField;
        
        private string current_gics_subind_nameField;
        
        private string new_gics_subind_nameField;
        
        private decimal current_fifField;
        
        private bool current_fifFieldSpecified;
        
        private decimal new_fifField;
        
        private bool new_fifFieldSpecified;
        
        private decimal current_difField;
        
        private bool current_difFieldSpecified;
        
        private decimal new_difField;
        
        private bool new_difFieldSpecified;
        
        private decimal current_lifField;
        
        private bool current_lifFieldSpecified;
        
        private decimal new_lifField;
        
        private bool new_lifFieldSpecified;
        
        private decimal current_folField;
        
        private bool current_folFieldSpecified;
        
        private decimal new_folField;
        
        private bool new_folFieldSpecified;
        
        private decimal current_std_flagField;
        
        private bool current_std_flagFieldSpecified;
        
        private decimal new_std_flagField;
        
        private bool new_std_flagFieldSpecified;
        
        private decimal current_std_dom_flagField;
        
        private bool current_std_dom_flagFieldSpecified;
        
        private decimal new_std_dom_flagField;
        
        private bool new_std_dom_flagFieldSpecified;
        
        private decimal current_large_dom_flagField;
        
        private bool current_large_dom_flagFieldSpecified;
        
        private decimal new_large_dom_flagField;
        
        private bool new_large_dom_flagFieldSpecified;
        
        private decimal current_mid_dom_flagField;
        
        private bool current_mid_dom_flagFieldSpecified;
        
        private decimal new_mid_dom_flagField;
        
        private bool new_mid_dom_flagFieldSpecified;
        
        private decimal current_large_flagField;
        
        private bool current_large_flagFieldSpecified;
        
        private decimal new_large_flagField;
        
        private bool new_large_flagFieldSpecified;
        
        private decimal current_mid_flagField;
        
        private bool current_mid_flagFieldSpecified;
        
        private decimal new_mid_flagField;
        
        private bool new_mid_flagFieldSpecified;
        
        private decimal dM_universe_flagField;
        
        private bool dM_universe_flagFieldSpecified;
        
        private decimal eM_universe_flagField;
        
        private bool eM_universe_flagFieldSpecified;
        
        private decimal fM_universe_flagField;
        
        private bool fM_universe_flagFieldSpecified;
        
        private decimal current_std_IIFField;
        
        private bool current_std_IIFFieldSpecified;
        
        private decimal new_std_IIFField;
        
        private bool new_std_IIFFieldSpecified;
        
        private decimal current_group_entity_codeField;
        
        private bool current_group_entity_codeFieldSpecified;
        
        private decimal new_group_entity_codeField;
        
        private bool new_group_entity_codeFieldSpecified;
        
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string status {
            get {
                return this.statusField;
            }
            set {
                this.statusField = value;
            }
        }
        
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal event_id {
            get {
                return this.event_idField;
            }
            set {
                this.event_idField = value;
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool event_idSpecified {
            get {
                return this.event_idFieldSpecified;
            }
            set {
                this.event_idFieldSpecified = value;
            }
        }
        
        [System.Xml.Serialization.XmlAttributeAttribute(DataType="date")]
        public System.DateTime ex_date {
            get {
                return this.ex_dateField;
            }
            set {
                this.ex_dateField = value;
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool ex_dateSpecified {
            get {
                return this.ex_dateFieldSpecified;
            }
            set {
                this.ex_dateFieldSpecified = value;
            }
        }
        
        [System.Xml.Serialization.XmlAttributeAttribute(DataType="date")]
        public System.DateTime last_updated_date {
            get {
                return this.last_updated_dateField;
            }
            set {
                this.last_updated_dateField = value;
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool last_updated_dateSpecified {
            get {
                return this.last_updated_dateFieldSpecified;
            }
            set {
                this.last_updated_dateFieldSpecified = value;
            }
        }
        
        [System.Xml.Serialization.XmlAttributeAttribute(DataType="date")]
        public System.DateTime first_entry_date {
            get {
                return this.first_entry_dateField;
            }
            set {
                this.first_entry_dateField = value;
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool first_entry_dateSpecified {
            get {
                return this.first_entry_dateFieldSpecified;
            }
            set {
                this.first_entry_dateFieldSpecified = value;
            }
        }
        
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string current_country {
            get {
                return this.current_countryField;
            }
            set {
                this.current_countryField = value;
            }
        }
        
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string new_country {
            get {
                return this.new_countryField;
            }
            set {
                this.new_countryField = value;
            }
        }
        
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string current_sec_name {
            get {
                return this.current_sec_nameField;
            }
            set {
                this.current_sec_nameField = value;
            }
        }
        
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string new_sec_name {
            get {
                return this.new_sec_nameField;
            }
            set {
                this.new_sec_nameField = value;
            }
        }
        
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string msci_sec_code {
            get {
                return this.msci_sec_codeField;
            }
            set {
                this.msci_sec_codeField = value;
            }
        }
        
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal timeserie_code {
            get {
                return this.timeserie_codeField;
            }
            set {
                this.timeserie_codeField = value;
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool timeserie_codeSpecified {
            get {
                return this.timeserie_codeFieldSpecified;
            }
            set {
                this.timeserie_codeFieldSpecified = value;
            }
        }
        
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string sedol {
            get {
                return this.sedolField;
            }
            set {
                this.sedolField = value;
            }
        }
        
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string isin {
            get {
                return this.isinField;
            }
            set {
                this.isinField = value;
            }
        }
        
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string bb_tickers {
            get {
                return this.bb_tickersField;
            }
            set {
                this.bb_tickersField = value;
            }
        }
        
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string event_type {
            get {
                return this.event_typeField;
            }
            set {
                this.event_typeField = value;
            }
        }
        
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string event_desc {
            get {
                return this.event_descField;
            }
            set {
                this.event_descField = value;
            }
        }
        
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string correction_comment {
            get {
                return this.correction_commentField;
            }
            set {
                this.correction_commentField = value;
            }
        }
        
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string terms {
            get {
                return this.termsField;
            }
            set {
                this.termsField = value;
            }
        }
        
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal text_ann_flag {
            get {
                return this.text_ann_flagField;
            }
            set {
                this.text_ann_flagField = value;
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool text_ann_flagSpecified {
            get {
                return this.text_ann_flagFieldSpecified;
            }
            set {
                this.text_ann_flagFieldSpecified = value;
            }
        }
        
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal current_eod_nos {
            get {
                return this.current_eod_nosField;
            }
            set {
                this.current_eod_nosField = value;
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool current_eod_nosSpecified {
            get {
                return this.current_eod_nosFieldSpecified;
            }
            set {
                this.current_eod_nosFieldSpecified = value;
            }
        }
        
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal new_eod_nos {
            get {
                return this.new_eod_nosField;
            }
            set {
                this.new_eod_nosField = value;
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool new_eod_nosSpecified {
            get {
                return this.new_eod_nosFieldSpecified;
            }
            set {
                this.new_eod_nosFieldSpecified = value;
            }
        }
        
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal current_closing_nos {
            get {
                return this.current_closing_nosField;
            }
            set {
                this.current_closing_nosField = value;
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool current_closing_nosSpecified {
            get {
                return this.current_closing_nosFieldSpecified;
            }
            set {
                this.current_closing_nosFieldSpecified = value;
            }
        }
        
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal new_closing_nos {
            get {
                return this.new_closing_nosField;
            }
            set {
                this.new_closing_nosField = value;
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool new_closing_nosSpecified {
            get {
                return this.new_closing_nosFieldSpecified;
            }
            set {
                this.new_closing_nosFieldSpecified = value;
            }
        }
        
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string paf_formula {
            get {
                return this.paf_formulaField;
            }
            set {
                this.paf_formulaField = value;
            }
        }
        
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal paf {
            get {
                return this.pafField;
            }
            set {
                this.pafField = value;
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool pafSpecified {
            get {
                return this.pafFieldSpecified;
            }
            set {
                this.pafFieldSpecified = value;
            }
        }
        
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string current_ISO_currency_symbol {
            get {
                return this.current_ISO_currency_symbolField;
            }
            set {
                this.current_ISO_currency_symbolField = value;
            }
        }
        
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string new_ISO_currency_symbol {
            get {
                return this.new_ISO_currency_symbolField;
            }
            set {
                this.new_ISO_currency_symbolField = value;
            }
        }
        
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string current_gics_subind_code {
            get {
                return this.current_gics_subind_codeField;
            }
            set {
                this.current_gics_subind_codeField = value;
            }
        }
        
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string new_gics_subindustry_code {
            get {
                return this.new_gics_subindustry_codeField;
            }
            set {
                this.new_gics_subindustry_codeField = value;
            }
        }
        
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string current_gics_subind_name {
            get {
                return this.current_gics_subind_nameField;
            }
            set {
                this.current_gics_subind_nameField = value;
            }
        }
        
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string new_gics_subind_name {
            get {
                return this.new_gics_subind_nameField;
            }
            set {
                this.new_gics_subind_nameField = value;
            }
        }
        
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal current_fif {
            get {
                return this.current_fifField;
            }
            set {
                this.current_fifField = value;
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool current_fifSpecified {
            get {
                return this.current_fifFieldSpecified;
            }
            set {
                this.current_fifFieldSpecified = value;
            }
        }
        
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal new_fif {
            get {
                return this.new_fifField;
            }
            set {
                this.new_fifField = value;
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool new_fifSpecified {
            get {
                return this.new_fifFieldSpecified;
            }
            set {
                this.new_fifFieldSpecified = value;
            }
        }
        
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal current_dif {
            get {
                return this.current_difField;
            }
            set {
                this.current_difField = value;
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool current_difSpecified {
            get {
                return this.current_difFieldSpecified;
            }
            set {
                this.current_difFieldSpecified = value;
            }
        }
        
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal new_dif {
            get {
                return this.new_difField;
            }
            set {
                this.new_difField = value;
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool new_difSpecified {
            get {
                return this.new_difFieldSpecified;
            }
            set {
                this.new_difFieldSpecified = value;
            }
        }
        
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal current_lif {
            get {
                return this.current_lifField;
            }
            set {
                this.current_lifField = value;
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool current_lifSpecified {
            get {
                return this.current_lifFieldSpecified;
            }
            set {
                this.current_lifFieldSpecified = value;
            }
        }
        
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal new_lif {
            get {
                return this.new_lifField;
            }
            set {
                this.new_lifField = value;
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool new_lifSpecified {
            get {
                return this.new_lifFieldSpecified;
            }
            set {
                this.new_lifFieldSpecified = value;
            }
        }
        
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal current_fol {
            get {
                return this.current_folField;
            }
            set {
                this.current_folField = value;
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool current_folSpecified {
            get {
                return this.current_folFieldSpecified;
            }
            set {
                this.current_folFieldSpecified = value;
            }
        }
        
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal new_fol {
            get {
                return this.new_folField;
            }
            set {
                this.new_folField = value;
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool new_folSpecified {
            get {
                return this.new_folFieldSpecified;
            }
            set {
                this.new_folFieldSpecified = value;
            }
        }
        
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal current_std_flag {
            get {
                return this.current_std_flagField;
            }
            set {
                this.current_std_flagField = value;
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool current_std_flagSpecified {
            get {
                return this.current_std_flagFieldSpecified;
            }
            set {
                this.current_std_flagFieldSpecified = value;
            }
        }
        
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal new_std_flag {
            get {
                return this.new_std_flagField;
            }
            set {
                this.new_std_flagField = value;
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool new_std_flagSpecified {
            get {
                return this.new_std_flagFieldSpecified;
            }
            set {
                this.new_std_flagFieldSpecified = value;
            }
        }
        
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal current_std_dom_flag {
            get {
                return this.current_std_dom_flagField;
            }
            set {
                this.current_std_dom_flagField = value;
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool current_std_dom_flagSpecified {
            get {
                return this.current_std_dom_flagFieldSpecified;
            }
            set {
                this.current_std_dom_flagFieldSpecified = value;
            }
        }
        
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal new_std_dom_flag {
            get {
                return this.new_std_dom_flagField;
            }
            set {
                this.new_std_dom_flagField = value;
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool new_std_dom_flagSpecified {
            get {
                return this.new_std_dom_flagFieldSpecified;
            }
            set {
                this.new_std_dom_flagFieldSpecified = value;
            }
        }
        
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal current_large_dom_flag {
            get {
                return this.current_large_dom_flagField;
            }
            set {
                this.current_large_dom_flagField = value;
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool current_large_dom_flagSpecified {
            get {
                return this.current_large_dom_flagFieldSpecified;
            }
            set {
                this.current_large_dom_flagFieldSpecified = value;
            }
        }
        
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal new_large_dom_flag {
            get {
                return this.new_large_dom_flagField;
            }
            set {
                this.new_large_dom_flagField = value;
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool new_large_dom_flagSpecified {
            get {
                return this.new_large_dom_flagFieldSpecified;
            }
            set {
                this.new_large_dom_flagFieldSpecified = value;
            }
        }
        
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal current_mid_dom_flag {
            get {
                return this.current_mid_dom_flagField;
            }
            set {
                this.current_mid_dom_flagField = value;
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool current_mid_dom_flagSpecified {
            get {
                return this.current_mid_dom_flagFieldSpecified;
            }
            set {
                this.current_mid_dom_flagFieldSpecified = value;
            }
        }
        
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal new_mid_dom_flag {
            get {
                return this.new_mid_dom_flagField;
            }
            set {
                this.new_mid_dom_flagField = value;
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool new_mid_dom_flagSpecified {
            get {
                return this.new_mid_dom_flagFieldSpecified;
            }
            set {
                this.new_mid_dom_flagFieldSpecified = value;
            }
        }
        
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal current_large_flag {
            get {
                return this.current_large_flagField;
            }
            set {
                this.current_large_flagField = value;
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool current_large_flagSpecified {
            get {
                return this.current_large_flagFieldSpecified;
            }
            set {
                this.current_large_flagFieldSpecified = value;
            }
        }
        
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal new_large_flag {
            get {
                return this.new_large_flagField;
            }
            set {
                this.new_large_flagField = value;
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool new_large_flagSpecified {
            get {
                return this.new_large_flagFieldSpecified;
            }
            set {
                this.new_large_flagFieldSpecified = value;
            }
        }
        
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal current_mid_flag {
            get {
                return this.current_mid_flagField;
            }
            set {
                this.current_mid_flagField = value;
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool current_mid_flagSpecified {
            get {
                return this.current_mid_flagFieldSpecified;
            }
            set {
                this.current_mid_flagFieldSpecified = value;
            }
        }
        
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal new_mid_flag {
            get {
                return this.new_mid_flagField;
            }
            set {
                this.new_mid_flagField = value;
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool new_mid_flagSpecified {
            get {
                return this.new_mid_flagFieldSpecified;
            }
            set {
                this.new_mid_flagFieldSpecified = value;
            }
        }
        
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal DM_universe_flag {
            get {
                return this.dM_universe_flagField;
            }
            set {
                this.dM_universe_flagField = value;
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool DM_universe_flagSpecified {
            get {
                return this.dM_universe_flagFieldSpecified;
            }
            set {
                this.dM_universe_flagFieldSpecified = value;
            }
        }
        
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal EM_universe_flag {
            get {
                return this.eM_universe_flagField;
            }
            set {
                this.eM_universe_flagField = value;
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool EM_universe_flagSpecified {
            get {
                return this.eM_universe_flagFieldSpecified;
            }
            set {
                this.eM_universe_flagFieldSpecified = value;
            }
        }
        
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal FM_universe_flag {
            get {
                return this.fM_universe_flagField;
            }
            set {
                this.fM_universe_flagField = value;
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool FM_universe_flagSpecified {
            get {
                return this.fM_universe_flagFieldSpecified;
            }
            set {
                this.fM_universe_flagFieldSpecified = value;
            }
        }
        
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal current_std_IIF {
            get {
                return this.current_std_IIFField;
            }
            set {
                this.current_std_IIFField = value;
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool current_std_IIFSpecified {
            get {
                return this.current_std_IIFFieldSpecified;
            }
            set {
                this.current_std_IIFFieldSpecified = value;
            }
        }
        
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal new_std_IIF {
            get {
                return this.new_std_IIFField;
            }
            set {
                this.new_std_IIFField = value;
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool new_std_IIFSpecified {
            get {
                return this.new_std_IIFFieldSpecified;
            }
            set {
                this.new_std_IIFFieldSpecified = value;
            }
        }
        
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal current_group_entity_code {
            get {
                return this.current_group_entity_codeField;
            }
            set {
                this.current_group_entity_codeField = value;
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool current_group_entity_codeSpecified {
            get {
                return this.current_group_entity_codeFieldSpecified;
            }
            set {
                this.current_group_entity_codeFieldSpecified = value;
            }
        }
        
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal new_group_entity_code {
            get {
                return this.new_group_entity_codeField;
            }
            set {
                this.new_group_entity_codeField = value;
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool new_group_entity_codeSpecified {
            get {
                return this.new_group_entity_codeFieldSpecified;
            }
            set {
                this.new_group_entity_codeFieldSpecified = value;
            }
        }
    }
}