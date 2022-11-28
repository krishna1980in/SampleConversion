

    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Xml.Serialization;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute("kartrisStatisticsData", Namespace="http://tempuri.org/kartrisStatisticsData.xsd", AnonymousType=true)]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlRootAttribute("kartrisStatisticsData", Namespace="http://tempuri.org/kartrisStatisticsData.xsd")]
    public partial class KartrisStatisticsData
    {
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        private System.Collections.ObjectModel.Collection<KartrisStatisticsDataStatistics> _statistics;
        
        [System.Xml.Serialization.XmlElementAttribute("Statistics", Namespace="http://tempuri.org/kartrisStatisticsData.xsd")]
        public System.Collections.ObjectModel.Collection<KartrisStatisticsDataStatistics> Statistics
        {
            get
            {
                return this._statistics;
            }
            private set
            {
                this._statistics = value;
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool StatisticsSpecified
        {
            get
            {
                return (this.Statistics.Count != 0);
            }
        }
        
        public KartrisStatisticsData()
        {
            this._statistics = new System.Collections.ObjectModel.Collection<KartrisStatisticsDataStatistics>();
            this._searchStatistics = new System.Collections.ObjectModel.Collection<KartrisStatisticsDataSearchStatistics>();
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        private System.Collections.ObjectModel.Collection<KartrisStatisticsDataSearchStatistics> _searchStatistics;
        
        [System.Xml.Serialization.XmlElementAttribute("SearchStatistics", Namespace="http://tempuri.org/kartrisStatisticsData.xsd")]
        public System.Collections.ObjectModel.Collection<KartrisStatisticsDataSearchStatistics> SearchStatistics
        {
            get
            {
                return this._searchStatistics;
            }
            private set
            {
                this._searchStatistics = value;
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool SearchStatisticsSpecified
        {
            get
            {
                return (this.SearchStatistics.Count != 0);
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute("KartrisStatisticsDataStatistics", Namespace="http://tempuri.org/kartrisStatisticsData.xsd", AnonymousType=true)]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class KartrisStatisticsDataStatistics
    {
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("ST_ID", Namespace="http://tempuri.org/kartrisStatisticsData.xsd")]
        public long ST_IDValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool ST_IDValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<long> ST_ID
        {
            get
            {
                if (this.ST_IDValueSpecified)
                {
                    return this.ST_IDValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.ST_IDValue = value.GetValueOrDefault();
                this.ST_IDValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(1)]
        [System.Xml.Serialization.XmlElementAttribute("ST_Type", Namespace="http://tempuri.org/kartrisStatisticsData.xsd")]
        public string ST_Type { get; set; }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("ST_ItemParentID", Namespace="http://tempuri.org/kartrisStatisticsData.xsd")]
        public long ST_ItemParentIDValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool ST_ItemParentIDValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<long> ST_ItemParentID
        {
            get
            {
                if (this.ST_ItemParentIDValueSpecified)
                {
                    return this.ST_ItemParentIDValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.ST_ItemParentIDValue = value.GetValueOrDefault();
                this.ST_ItemParentIDValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("ST_ItemID", Namespace="http://tempuri.org/kartrisStatisticsData.xsd")]
        public long ST_ItemIDValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool ST_ItemIDValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<long> ST_ItemID
        {
            get
            {
                if (this.ST_ItemIDValueSpecified)
                {
                    return this.ST_ItemIDValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.ST_ItemIDValue = value.GetValueOrDefault();
                this.ST_ItemIDValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("ST_Date", Namespace="http://tempuri.org/kartrisStatisticsData.xsd", DataType="dateTime")]
        public System.DateTime ST_DateValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool ST_DateValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<System.DateTime> ST_Date
        {
            get
            {
                if (this.ST_DateValueSpecified)
                {
                    return this.ST_DateValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.ST_DateValue = value.GetValueOrDefault();
                this.ST_DateValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(50)]
        [System.Xml.Serialization.XmlElementAttribute("ST_IP", Namespace="http://tempuri.org/kartrisStatisticsData.xsd")]
        public string ST_IP { get; set; }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute("KartrisStatisticsDataSearchStatistics", Namespace="http://tempuri.org/kartrisStatisticsData.xsd", AnonymousType=true)]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class KartrisStatisticsDataSearchStatistics
    {
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("SS_ID", Namespace="http://tempuri.org/kartrisStatisticsData.xsd")]
        public long SS_IDValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool SS_IDValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<long> SS_ID
        {
            get
            {
                if (this.SS_IDValueSpecified)
                {
                    return this.SS_IDValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.SS_IDValue = value.GetValueOrDefault();
                this.SS_IDValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(150)]
        [System.Xml.Serialization.XmlElementAttribute("SS_Keyword", Namespace="http://tempuri.org/kartrisStatisticsData.xsd")]
        public string SS_Keyword { get; set; }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("SS_Year", Namespace="http://tempuri.org/kartrisStatisticsData.xsd")]
        public short SS_YearValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool SS_YearValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<short> SS_Year
        {
            get
            {
                if (this.SS_YearValueSpecified)
                {
                    return this.SS_YearValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.SS_YearValue = value.GetValueOrDefault();
                this.SS_YearValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("SS_Month", Namespace="http://tempuri.org/kartrisStatisticsData.xsd")]
        public byte SS_MonthValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool SS_MonthValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<byte> SS_Month
        {
            get
            {
                if (this.SS_MonthValueSpecified)
                {
                    return this.SS_MonthValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.SS_MonthValue = value.GetValueOrDefault();
                this.SS_MonthValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("SS_Day", Namespace="http://tempuri.org/kartrisStatisticsData.xsd")]
        public byte SS_DayValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool SS_DayValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<byte> SS_Day
        {
            get
            {
                if (this.SS_DayValueSpecified)
                {
                    return this.SS_DayValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.SS_DayValue = value.GetValueOrDefault();
                this.SS_DayValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("SS_Searches", Namespace="http://tempuri.org/kartrisStatisticsData.xsd")]
        public short SS_SearchesValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool SS_SearchesValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<short> SS_Searches
        {
            get
            {
                if (this.SS_SearchesValueSpecified)
                {
                    return this.SS_SearchesValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.SS_SearchesValue = value.GetValueOrDefault();
                this.SS_SearchesValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("SS_Date", Namespace="http://tempuri.org/kartrisStatisticsData.xsd", DataType="dateTime")]
        public System.DateTime SS_DateValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool SS_DateValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<System.DateTime> SS_Date
        {
            get
            {
                if (this.SS_DateValueSpecified)
                {
                    return this.SS_DateValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.SS_DateValue = value.GetValueOrDefault();
                this.SS_DateValueSpecified = value.HasValue;
            }
        }
    }

 