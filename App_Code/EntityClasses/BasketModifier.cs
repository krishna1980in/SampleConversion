using System;

namespace Kartris
{
    /// <summary>
    ///     ''' Used to modify the entirety of a basket according to any rule set
    ///     ''' </summary>
    ///     ''' <remarks>Used to apply coupons, promotions, shipping etc. to a basket.</remarks>
    [Serializable()]
    public class BasketModifier
    {
        private double _ExTax;
        private double _IncTax;
        private double _TaxRate;

        public double ExTax
        {
            get
            {
                return Math.Round(_ExTax, 2);
            }
            set
            {
                _ExTax = value;
            }
        }

        public double IncTax
        {
            get
            {
                return Math.Round(_IncTax, 2);
            }
            set
            {
                _IncTax = value;
            }
        }

        public double TaxRate
        {
            get
            {
                int numRounding = 0;
                if (ConfigurationManager.AppSettings("TaxRegime").ToLower != "us" & ConfigurationManager.AppSettings("TaxRegime").ToLower != "simple")
                    numRounding = 4;
                else
                    numRounding = 6;
                return Math.Round(_TaxRate, numRounding);
            }
            set
            {
                _TaxRate = value;
            }
        }

        public double TaxAmount
        {
            get
            {
                int numRounding = 0;
                if (ConfigurationManager.AppSettings("TaxRegime").ToLower != "us" & ConfigurationManager.AppSettings("TaxRegime").ToLower != "simple")
                    numRounding = 2;
                else
                    numRounding = 4;
                return Math.Round(IncTax - ExTax, numRounding);
            }
        }
    }
}
