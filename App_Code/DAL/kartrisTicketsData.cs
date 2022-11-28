

    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Xml.Serialization;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute("kartrisTicketsData", Namespace="http://tempuri.org/kartrisTicketsData.xsd", AnonymousType=true)]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlRootAttribute("kartrisTicketsData", Namespace="http://tempuri.org/kartrisTicketsData.xsd")]
    public partial class KartrisTicketsData
    {
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        private System.Collections.ObjectModel.Collection<KartrisTicketsDataTickets> _tickets;
        
        [System.Xml.Serialization.XmlElementAttribute("Tickets", Namespace="http://tempuri.org/kartrisTicketsData.xsd")]
        public System.Collections.ObjectModel.Collection<KartrisTicketsDataTickets> Tickets
        {
            get
            {
                return this._tickets;
            }
            private set
            {
                this._tickets = value;
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool TicketsSpecified
        {
            get
            {
                return (this.Tickets.Count != 0);
            }
        }
        
        public KartrisTicketsData()
        {
            this._tickets = new System.Collections.ObjectModel.Collection<KartrisTicketsDataTickets>();
            this._ticketTypes = new System.Collections.ObjectModel.Collection<KartrisTicketsDataTicketTypes>();
            this._ticketMessages = new System.Collections.ObjectModel.Collection<KartrisTicketsDataTicketMessages>();
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        private System.Collections.ObjectModel.Collection<KartrisTicketsDataTicketTypes> _ticketTypes;
        
        [System.Xml.Serialization.XmlElementAttribute("TicketTypes", Namespace="http://tempuri.org/kartrisTicketsData.xsd")]
        public System.Collections.ObjectModel.Collection<KartrisTicketsDataTicketTypes> TicketTypes
        {
            get
            {
                return this._ticketTypes;
            }
            private set
            {
                this._ticketTypes = value;
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool TicketTypesSpecified
        {
            get
            {
                return (this.TicketTypes.Count != 0);
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        private System.Collections.ObjectModel.Collection<KartrisTicketsDataTicketMessages> _ticketMessages;
        
        [System.Xml.Serialization.XmlElementAttribute("TicketMessages", Namespace="http://tempuri.org/kartrisTicketsData.xsd")]
        public System.Collections.ObjectModel.Collection<KartrisTicketsDataTicketMessages> TicketMessages
        {
            get
            {
                return this._ticketMessages;
            }
            private set
            {
                this._ticketMessages = value;
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool TicketMessagesSpecified
        {
            get
            {
                return (this.TicketMessages.Count != 0);
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute("KartrisTicketsDataTickets", Namespace="http://tempuri.org/kartrisTicketsData.xsd", AnonymousType=true)]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class KartrisTicketsDataTickets
    {
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("TIC_ID", Namespace="http://tempuri.org/kartrisTicketsData.xsd")]
        public long TIC_IDValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool TIC_IDValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<long> TIC_ID
        {
            get
            {
                if (this.TIC_IDValueSpecified)
                {
                    return this.TIC_IDValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.TIC_IDValue = value.GetValueOrDefault();
                this.TIC_IDValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("TIC_DateOpened", Namespace="http://tempuri.org/kartrisTicketsData.xsd", DataType="dateTime")]
        public System.DateTime TIC_DateOpenedValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool TIC_DateOpenedValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<System.DateTime> TIC_DateOpened
        {
            get
            {
                if (this.TIC_DateOpenedValueSpecified)
                {
                    return this.TIC_DateOpenedValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.TIC_DateOpenedValue = value.GetValueOrDefault();
                this.TIC_DateOpenedValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("TIC_DateClosed", Namespace="http://tempuri.org/kartrisTicketsData.xsd", DataType="dateTime")]
        public System.DateTime TIC_DateClosedValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool TIC_DateClosedValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<System.DateTime> TIC_DateClosed
        {
            get
            {
                if (this.TIC_DateClosedValueSpecified)
                {
                    return this.TIC_DateClosedValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.TIC_DateClosedValue = value.GetValueOrDefault();
                this.TIC_DateClosedValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(100)]
        [System.Xml.Serialization.XmlElementAttribute("TIC_Subject", Namespace="http://tempuri.org/kartrisTicketsData.xsd")]
        public string TIC_Subject { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(1)]
        [System.Xml.Serialization.XmlElementAttribute("TIC_Status", Namespace="http://tempuri.org/kartrisTicketsData.xsd")]
        public string TIC_Status { get; set; }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("TIC_TimeSpent", Namespace="http://tempuri.org/kartrisTicketsData.xsd")]
        public int TIC_TimeSpentValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool TIC_TimeSpentValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<int> TIC_TimeSpent
        {
            get
            {
                if (this.TIC_TimeSpentValueSpecified)
                {
                    return this.TIC_TimeSpentValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.TIC_TimeSpentValue = value.GetValueOrDefault();
                this.TIC_TimeSpentValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("TIC_UserID", Namespace="http://tempuri.org/kartrisTicketsData.xsd")]
        public int TIC_UserIDValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool TIC_UserIDValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<int> TIC_UserID
        {
            get
            {
                if (this.TIC_UserIDValueSpecified)
                {
                    return this.TIC_UserIDValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.TIC_UserIDValue = value.GetValueOrDefault();
                this.TIC_UserIDValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("TIC_LoginID", Namespace="http://tempuri.org/kartrisTicketsData.xsd")]
        public short TIC_LoginIDValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool TIC_LoginIDValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<short> TIC_LoginID
        {
            get
            {
                if (this.TIC_LoginIDValueSpecified)
                {
                    return this.TIC_LoginIDValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.TIC_LoginIDValue = value.GetValueOrDefault();
                this.TIC_LoginIDValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("TIC_SupportTicketTypeID", Namespace="http://tempuri.org/kartrisTicketsData.xsd")]
        public short TIC_SupportTicketTypeIDValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool TIC_SupportTicketTypeIDValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<short> TIC_SupportTicketTypeID
        {
            get
            {
                if (this.TIC_SupportTicketTypeIDValueSpecified)
                {
                    return this.TIC_SupportTicketTypeIDValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.TIC_SupportTicketTypeIDValue = value.GetValueOrDefault();
                this.TIC_SupportTicketTypeIDValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(200)]
        [System.Xml.Serialization.XmlElementAttribute("TIC_Tags", Namespace="http://tempuri.org/kartrisTicketsData.xsd")]
        public string TIC_Tags { get; set; }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute("KartrisTicketsDataTicketTypes", Namespace="http://tempuri.org/kartrisTicketsData.xsd", AnonymousType=true)]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class KartrisTicketsDataTicketTypes
    {
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("STT_ID", Namespace="http://tempuri.org/kartrisTicketsData.xsd")]
        public int STT_IDValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool STT_IDValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<int> STT_ID
        {
            get
            {
                if (this.STT_IDValueSpecified)
                {
                    return this.STT_IDValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.STT_IDValue = value.GetValueOrDefault();
                this.STT_IDValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(50)]
        [System.Xml.Serialization.XmlElementAttribute("STT_Name", Namespace="http://tempuri.org/kartrisTicketsData.xsd")]
        public string STT_Name { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(1)]
        [System.Xml.Serialization.XmlElementAttribute("STT_Level", Namespace="http://tempuri.org/kartrisTicketsData.xsd")]
        public string STT_Level { get; set; }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute("KartrisTicketsDataTicketMessages", Namespace="http://tempuri.org/kartrisTicketsData.xsd", AnonymousType=true)]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class KartrisTicketsDataTicketMessages
    {
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("STM_ID", Namespace="http://tempuri.org/kartrisTicketsData.xsd")]
        public long STM_IDValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool STM_IDValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<long> STM_ID
        {
            get
            {
                if (this.STM_IDValueSpecified)
                {
                    return this.STM_IDValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.STM_IDValue = value.GetValueOrDefault();
                this.STM_IDValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("AssignedID", Namespace="http://tempuri.org/kartrisTicketsData.xsd")]
        public short AssignedIDValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool AssignedIDValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<short> AssignedID
        {
            get
            {
                if (this.AssignedIDValueSpecified)
                {
                    return this.AssignedIDValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.AssignedIDValue = value.GetValueOrDefault();
                this.AssignedIDValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(50)]
        [System.Xml.Serialization.XmlElementAttribute("AssignedName", Namespace="http://tempuri.org/kartrisTicketsData.xsd")]
        public string AssignedName { get; set; }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("STM_DateCreated", Namespace="http://tempuri.org/kartrisTicketsData.xsd", DataType="dateTime")]
        public System.DateTime STM_DateCreatedValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool STM_DateCreatedValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<System.DateTime> STM_DateCreated
        {
            get
            {
                if (this.STM_DateCreatedValueSpecified)
                {
                    return this.STM_DateCreatedValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.STM_DateCreatedValue = value.GetValueOrDefault();
                this.STM_DateCreatedValueSpecified = value.HasValue;
            }
        }
        
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(2147483647)]
        [System.Xml.Serialization.XmlElementAttribute("STM_Text", Namespace="http://tempuri.org/kartrisTicketsData.xsd")]
        public string STM_Text { get; set; }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("STM_TicketID", Namespace="http://tempuri.org/kartrisTicketsData.xsd")]
        public long STM_TicketIDValue { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool STM_TicketIDValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<long> STM_TicketID
        {
            get
            {
                if (this.STM_TicketIDValueSpecified)
                {
                    return this.STM_TicketIDValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.STM_TicketIDValue = value.GetValueOrDefault();
                this.STM_TicketIDValueSpecified = value.HasValue;
            }
        }
    }

 