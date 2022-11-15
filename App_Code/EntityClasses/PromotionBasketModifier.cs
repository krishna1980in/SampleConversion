using System;


namespace Kartris
{
    /// <summary>
    ///     ''' Contains a promotion item and is used to modify a basket with the contents of the promotion
    ///     ''' </summary>
    ///     ''' <remarks>A promotion may define that all items over $100 are discounted by 10%, this modifier is responsible for actioning the details of the promotion upon the basket</remarks>
    [Serializable()]
    public class PromotionBasketModifier
    {
        private int _PromotionID;
        private string _Name;
        private decimal _ExTax;
        private decimal _IncTax;
        private decimal _TaxAmount;
        private decimal _TaxRate;
        private double _Quantity;
        private decimal _TotalExTax;
        private decimal _TotalIncTax;
        private bool _isFixedValuePromo;
        private bool _ApplyTax;

        /// <summary>
        ///         ''' The database identifier number for the promotion
        ///         ''' </summary>
        ///         ''' <value></value>
        ///         ''' <returns></returns>
        ///         ''' <remarks></remarks>
        public int PromotionID
        {
            get
            {
                return _PromotionID;
            }
            set
            {
                _PromotionID = value;
            }
        }

        /// <summary>
        ///         ''' Promotion name
        ///         ''' </summary>
        ///         ''' <value></value>
        ///         ''' <returns></returns>
        ///         ''' <remarks></remarks>
        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                _Name = value;
            }
        }

        public double ExTax
        {
            get
            {
                return (_ExTax);
            }
            set
            {
                _ExTax = (value);
            }
        }

        public decimal IncTax
        {
            get
            {
                return _IncTax;
            }
            set
            {
                _IncTax = (value);
            }
        }

        public decimal TaxAmount
        {
            get
            {
                return Interaction.IIf(!(ApplyTax), 0, (ExTax * ComputedTaxRate));
            }
            set
            {
                _TaxAmount = value;
            }
        }

        public decimal ComputedTaxRate
        {
            get
            {
                return _TaxRate;
            }
            set
            {
                _TaxRate = value;
            }
        }

        public double Quantity
        {
            get
            {
                return _Quantity;
            }
            set
            {
                _Quantity = value;
            }
        }

        public decimal TotalExTax
        {
            get
            {
                return _TotalExTax;
            }
            set
            {
                _TotalExTax = value;
            }
        }

        public decimal TotalIncTax
        {
            get
            {
                return _TotalIncTax;
            }
            set
            {
                _TotalIncTax = value;
            }
        }

        public bool isFixedValuePromo
        {
            get
            {
                return _isFixedValuePromo;
            }
            set
            {
                _isFixedValuePromo = value;
            }
        }

        public bool ApplyTax
        {
            get
            {
                return _ApplyTax;
            }
            set
            {
                _ApplyTax = value;
            }
        }
    }
}
