

    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Xml.Serialization;


    [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.97.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute("PurchaseOrderType", Namespace="http://tempuri.org/PurchaseOrderSchema.xsd")]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlRootAttribute("PurchaseOrder", Namespace="http://tempuri.org/PurchaseOrderSchema.xsd")]
    public partial class PurchaseOrderType
    {

    [System.Xml.Serialization.XmlIgnoreAttribute()]
    private System.Collections.ObjectModel.Collection
        _shipTo;

        [System.Xml.Serialization.XmlElementAttribute("ShipTo", Namespace="http://tempuri.org/PurchaseOrderSchema.xsd")]
        public System.Collections.ObjectModel.Collection
            ShipTo
            {
            get
            {
            return this._shipTo;
            }
            }

            public PurchaseOrderType()
            {
            this._shipTo = new System.Collections.ObjectModel.Collection
                ();
                }

                [System.Xml.Serialization.XmlElementAttribute("BillTo", Namespace="http://tempuri.org/PurchaseOrderSchema.xsd")]
                public USAddress BillTo { get; set; }

                [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
                [System.Xml.Serialization.XmlAttributeAttribute("OrderDate", Form=System.Xml.Schema.XmlSchemaForm.Unqualified, DataType="date")]
                public System.DateTime OrderDateValue { get; set; }

                [System.Xml.Serialization.XmlIgnoreAttribute()]
                [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
                public bool OrderDateValueSpecified { get; set; }

                [System.Xml.Serialization.XmlIgnoreAttribute()]
                public System.Nullable
                OrderDate
                {
                    get
                    {
                     if (this.OrderDateValueSpecified)
                     {
                      return this.OrderDateValue;
                     }
                     else
                     {
                      return null;
                     }
                    }
                    set
                    {
                      this.OrderDateValue = value.GetValueOrDefault();
                      this.OrderDateValueSpecified = value.HasValue;
                     }
                    }
                 }

                    [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.97.0")]
                    [System.SerializableAttribute()]
                    [System.Xml.Serialization.XmlTypeAttribute("USAddress", Namespace="http://tempuri.org/PurchaseOrderSchema.xsd")]
                    [System.ComponentModel.DesignerCategoryAttribute("code")]
                    public partial class USAddress
                    {

                        [System.Xml.Serialization.XmlElementAttribute("name", Namespace="http://tempuri.org/PurchaseOrderSchema.xsd", DataType="string")]
                        public string Name { get; set; }

                        [System.Xml.Serialization.XmlElementAttribute("street", Namespace="http://tempuri.org/PurchaseOrderSchema.xsd", DataType="string")]
                        public string Street { get; set; }

                        [System.Xml.Serialization.XmlElementAttribute("city", Namespace="http://tempuri.org/PurchaseOrderSchema.xsd", DataType="string")]
                        public string City { get; set; }

                        [System.Xml.Serialization.XmlElementAttribute("state", Namespace="http://tempuri.org/PurchaseOrderSchema.xsd", DataType="string")]
                        public string State { get; set; }

                        [System.Xml.Serialization.XmlElementAttribute("zip", Namespace="http://tempuri.org/PurchaseOrderSchema.xsd", DataType="integer")]
                        public string Zip { get; set; }

                        [System.Xml.Serialization.XmlIgnoreAttribute()]
                        private string _country = "US";

                        [System.ComponentModel.DefaultValueAttribute("US")]
                        [System.Xml.Serialization.XmlAttributeAttribute("country", Form=System.Xml.Schema.XmlSchemaForm.Unqualified, DataType="NMTOKEN")]
                        public string Country
                        {
                            get
                            {
                            return this._country;
                            }
                            set
                            {
                            this._country = value;
                            }
                       }
                    }
      