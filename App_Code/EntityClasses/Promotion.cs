using System;

namespace Kartris
{
    /// <summary>
    ///     ''' A promotion that can be added to a basket to affect the items in the basket such as discount all items by 10% etc.
    ///     ''' </summary>
    ///     ''' <remarks></remarks>
    [Serializable()]
    public class Promotion
    {
        private int _ID;
        private DateTime _StartDate;
        private DateTime _EndDate;
        private int _Live;
        private double _OrderByValue;
        private double _MaxQuantity;
        private string _PartNo;
        private string _Type;
        private decimal _Value;
        private string _ItemType;
        private int _ItemID;
        private string _ItemName;
        private string _PromoText;

        /// <summary>
        ///         ''' The database idenitifer for the promotion.
        ///         ''' </summary>
        ///         ''' <value></value>
        ///         ''' <returns></returns>
        ///         ''' <remarks></remarks>
        public int ID
        {
            get
            {
                return _ID;
            }
            set
            {
                _ID = value;
            }
        }

        /// <summary>
        ///         ''' Date from which the promotion is active. The promotion is not active before this date
        ///         ''' </summary>
        ///         ''' <value></value>
        ///         ''' <returns></returns>
        ///         ''' <remarks></remarks>
        public DateTime StartDate
        {
            get
            {
                return _StartDate;
            }
            set
            {
                _StartDate = value;
            }
        }

        /// <summary>
        ///         ''' Date to which the promotion is active. The promotion is not active after this date
        ///         ''' </summary>
        ///         ''' <value></value>
        ///         ''' <returns></returns>
        ///         ''' <remarks></remarks>
        public DateTime EndDate
        {
            get
            {
                return _EndDate;
            }
            set
            {
                _EndDate = value;
            }
        }

        /// <summary>
        ///         ''' Promotion is active
        ///         ''' </summary>
        ///         ''' <value></value>
        ///         ''' <returns></returns>
        ///         ''' <remarks></remarks>
        public int Live
        {
            get
            {
                return _Live;
            }
            set
            {
                _Live = value;
            }
        }

        public double OrderByValue
        {
            get
            {
                return _OrderByValue;
            }
            set
            {
                _OrderByValue = value;
            }
        }

        public double MaxQuantity
        {
            get
            {
                return _MaxQuantity;
            }
            set
            {
                _MaxQuantity = value;
            }
        }

        public string PartNo
        {
            get
            {
                return _PartNo;
            }
            set
            {
                _PartNo = value;
            }
        }

        public string Type
        {
            get
            {
                return _Type;
            }
            set
            {
                _Type = value;
            }
        }

        public decimal Value
        {
            get
            {
                return _Value;
            }
            set
            {
                _Value = value;
            }
        }

        public string ItemType
        {
            get
            {
                return _ItemType;
            }
            set
            {
                _ItemType = value;
            }
        }

        public int ItemID
        {
            get
            {
                return _ItemID;
            }
            set
            {
                _ItemID = value;
            }
        }

        public string ItemName
        {
            get
            {
                return _ItemName;
            }
            set
            {
                _ItemName = value;
            }
        }

        public string PromoText
        {
            get
            {
                return _PromoText;
            }
            set
            {
                _PromoText = value;
            }
        }
    }
}
