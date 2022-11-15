// ========================================================================
// Kartris - www.kartris.com
// Copyright 2021 CACTUSOFT

// GNU GENERAL PUBLIC LICENSE v2
// This program is free software distributed under the GPL without any
// warranty.
// www.gnu.org/licenses/gpl-2.0.html

// KARTRIS COMMERCIAL LICENSE
// If a valid license.config issued by Cactusoft is present, the KCL
// overrides the GPL v2.
// www.kartris.com/t-Kartris-Commercial-License.aspx
// ========================================================================
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;

using MailChimp;
using MailChimp.Net;
using MailChimp.Net.Core;
using MailChimp.Net.Interfaces;
using MailChimp.Net.Models;
using System.Collections.ObjectModel;
using System.Web.Script.Serialization;
using System.Xml.Serialization;
using Kartris;


// ========================================================================
// See https://github.com/brandonseydel/MailChimp.Net
// 
// Rather than reinvent the wheel, we're using Brandon Seydel's excellent
// MailChimp v3 API NuGet package.
// 
// ========================================================================
public class MailChimpBLL
{
    private string listId = KartSettingsManager.GetKartConfig("general.mailchimp.listid");
    private string apiKey = KartSettingsManager.GetKartConfig("general.mailchimp.apikey");
    private string apiUrl = KartSettingsManager.GetKartConfig("general.mailchimp.apiurl");
    private string mcStoreId = KartSettingsManager.GetKartConfig("general.mailchimp.storeid");
    public IMailChimpManager manager;
    private KartrisMemberShipUser currentLoggedUser;
    private Basket kartrisBasket;
    private string kartrisCurrencyCode;

    /// <summary>
    ///     ''' Create a new action
    ///     ''' if user not logged in
    ///     ''' </summary>
    public MailChimpBLL()
    {
        apiKey = KartSettingsManager.GetKartConfig("general.mailchimp.apikey");
        apiUrl = KartSettingsManager.GetKartConfig("general.mailchimp.apiurl");
        manager = new MailChimpManager(apiKey);
        kartrisCurrencyCode = CurrenciesBLL.CurrencyCode(CurrenciesBLL.GetDefaultCurrency());
        currentLoggedUser = null;
        kartrisBasket = new Basket();
    }

    /// <summary>
    ///     ''' Create a new action
    ///     ''' for logged in user
    ///     ''' </summary>
    public MailChimpBLL(KartrisMemberShipUser user, string currencyCode)
    {
        apiKey = KartSettingsManager.GetKartConfig("general.mailchimp.apikey");
        apiUrl = KartSettingsManager.GetKartConfig("general.mailchimp.apiurl");
        manager = new MailChimpManager(apiKey);
        currentLoggedUser = user;
        this.kartrisCurrencyCode = currencyCode;
    }

    /// <summary>
    ///     ''' Create a new action
    ///     ''' logged in user with basket
    ///     ''' </summary>
    public MailChimpBLL(ref KartrisMemberShipUser user, ref Basket basket, ref string currencyCode)
    {
        manager = new MailChimpManager(apiKey);
        currentLoggedUser = user;
        kartrisBasket = basket;
        this.kartrisCurrencyCode = currencyCode;
    }

    /// <summary>
    ///     ''' Get customer using Kartris customer ID
    ///     ''' from mailchimp account
    ///     ''' </summary>
    public async Task<Customer> GetCustomer(int kartrisUserId)
    {
        Customer customer = null/* TODO Change to default(_) if this is not a reference type */;
        try
        {
            customer = manager.ECommerceStores.Customers(mcStoreId).GetAsync(kartrisUserId).Result;
        }
        catch (Exception ex)
        {
            CkartrisFormatErrors.LogError("MailchimpBLL GetCustomer ");
            CkartrisFormatErrors.LogError("MailchimpBLL GetCustomer ex:" + ex.Message);
            Debug.Print(ex.Message);
        }
        return customer;
    }

    /// <summary>
    ///     ''' Add cart to mailchimp and tagged for
    ///     ''' right customer
    ///     ''' </summary>
    public async Task<string> AddCartToCustomerToStore(int orderId = 0)
    {
        Store mcStore = new Store();
        string toReturn = "";
        try
        {
            mcStore = manager.ECommerceStores.GetAsync(mcStoreId).Result;
        }
        catch (Exception ex)
        {
            if (ex.Message.Contains("Unable to find the resource at"))
                mcStore = null/* TODO Change to default(_) if this is not a reference type */;
        }

        try
        {
            if ((mcStore == null))
                mcStore = await this.AddStore(mcStoreId).ConfigureAwait(false);

            Customer customer;
            try
            {
                customer = manager.ECommerceStores.Customers(mcStore.Id).GetAsync(currentLoggedUser.ID).Result;
            }
            catch (Exception ex)
            {
                customer = null/* TODO Change to default(_) if this is not a reference type */;
            }

            if (customer == null)
                customer = await this.AddCustomer().ConfigureAwait(false);

            if (customer != null)
            {
                Cart cart = await AddCart(customer, orderId).ConfigureAwait(false);
                if (cart != null)
                    toReturn = cart.Id;
            }
        }
        catch (Exception ex)
        {
            // Debug.Print(ex.Message)
            // Log the error
            try
            {
                CkartrisFormatErrors.LogError("MailchimpBLL AddCartToCustomerToStore(1): ");
                CkartrisFormatErrors.LogError("MailchimpBLL AddCartToCustomerToStore(2): " + ex.Message);
            }
            catch (Exception ex2)
            {
            }
        }
        return toReturn;
    }

    // Public Async Function AddUpdateMember() As Task(Of Member)
    // Try
    // Dim userFullName As String = ""
    // userFullName = UsersBLL.GetNameandEUVAT(currentLoggedUser.ID).Split("|||")(0)
    // Dim userNamesArray() As String = userFullName.Split(" ")
    // 'Use the Status property if updating an existing member
    // Dim member As Member = New Member With {.Id = currentLoggedUser.ID,
    // .EmailAddress = currentLoggedUser.Email,
    // .StatusIfNew = Status.Subscribed}
    // member.MergeFields.Add("FNAME", userNamesArray(0))
    // If (userNamesArray.Length > 1) Then
    // member.MergeFields.Add("LNAME", userNamesArray(1))
    // End If

    // Dim taskResult As Member = Await manager.Members.AddOrUpdateAsync(listId, member).ConfigureAwait(False)
    // Return taskResult
    // Catch ex As Exception
    // Debug.Print(ex.Message)
    // End Try
    // End Function

    /// <summary>
    ///     ''' Add customer record to mailchimp
    ///     ''' </summary>
    public async Task<Customer> AddCustomer()
    {
        try
        {
            string userFullName = "";
            UsersBLL objUsersBLL = new UsersBLL();
            userFullName = objUsersBLL.GetNameandEUVAT(currentLoggedUser.ID).Split("|||")(0);
            string[] userNamesArray = userFullName.Split(" ");
            // Use the Status property if updating an existing member
            Customer customer = new Customer()
            {
                Id = currentLoggedUser.ID,
                EmailAddress = currentLoggedUser.Email,
                OptInStatus = true
            };
            customer.FirstName = userNamesArray[0];
            if ((userNamesArray.Length > 1))
                customer.LastName = userNamesArray[1];

            Customer taskResult = await manager.ECommerceStores.Customers(mcStoreId).AddAsync(customer).ConfigureAwait(false);
            return taskResult;
        }
        catch (Exception ex)
        {
            // Log the error
            try
            {
                CkartrisFormatErrors.LogError("MailchimpBLL AddCustomer: " + ex.Message);
            }
            catch (Exception ex2)
            {
            }


            // Avoid warnings when building, they tend to confuse people
            return null/* TODO Change to default(_) if this is not a reference type */;
        }
    }

    /// <summary>
    ///     ''' Add product to mailchimp
    ///     ''' </summary>
    public async Task<Product> AddProduct(BasketItem basketItem)
    {
        try
        {
            Product product = null/* TODO Change to default(_) if this is not a reference type */;
            Variant productVariant;
            List<Variant> listVariants = new List<Variant>();
            try
            {
                product = manager.ECommerceStores.Products(mcStoreId).GetAsync(basketItem.ProductID).Result;
            }
            catch (Exception ex)
            {
                CkartrisFormatErrors.LogError("MailchimpBLL Product trycatch ");
                CkartrisFormatErrors.LogError("MailchimpBLL Product trycatch :" + ex.Message);
                if (ex.Message.Contains("A product with the provided ID already"))
                    product = null/* TODO Change to default(_) if this is not a reference type */;
            }

            double itemprice;
            if (basketItem.PricesIncTax == true)
                itemprice = basketItem.IR_PricePerItem;
            else
                itemprice = Math.Round((basketItem.IR_PricePerItem * (1 + basketItem.IR_TaxPerItem)), 2);

            string imageUrl = CkartrisBLL.WebShopURL + "Image.aspx?strItemType=p&numMaxHeight=100&numMaxWidth=100&numItem=" + basketItem.ProductID;
            if (product == null)
            {
                productVariant = new Variant()
                {
                    Id = basketItem.ProductID,
                    Title = basketItem.Name,
                    Price = itemprice,
                    ImageUrl = imageUrl
                };

                listVariants.Add(productVariant);
                product = new Product()
                {
                    Id = basketItem.ProductID,
                    Title = basketItem.Name,
                    Variants = listVariants,
                    ImageUrl = imageUrl
                };
                Product taskResult = manager.ECommerceStores.Products(mcStoreId).AddAsync(product).Result;
                return taskResult;
            }
            else
            {
                bool modified = false;
                if (product.Title != basketItem.Name)
                {
                    product.Title = basketItem.Name;
                    product.Variants.First().Title = basketItem.Name;
                    modified = true;
                }
                if (product.ImageUrl != imageUrl)
                {
                    product.ImageUrl = imageUrl;
                    product.Variants.First().ImageUrl = imageUrl;
                    modified = true;
                }
                if (basketItem.PricesIncTax == true)
                {
                    if (product.Variants.First().Price != basketItem.IR_PricePerItem)
                    {
                        product.Variants.First().Price = basketItem.IR_PricePerItem;
                        modified = true;
                    }
                }
                else
                {
                    itemprice = Math.Round((basketItem.IR_PricePerItem * (1 + basketItem.IR_TaxPerItem)), 2);
                    if (product.Variants.First().Price != itemprice)
                    {
                        product.Variants.First().Price = itemprice;
                        modified = true;
                    }
                }

                if (modified)
                {
                    Product taskResult = null/* TODO Change to default(_) if this is not a reference type */;
                    try
                    {
                        taskResult = manager.ECommerceStores.Products(mcStoreId).UpdateAsync(basketItem.ProductID, product).Result;
                    }
                    catch (Exception ex)
                    {
                        CkartrisFormatErrors.LogError("MailchimpBLL Product try catch modified");
                    }
                    return taskResult;
                }
                else
                    return product;
            }
            return null/* TODO Change to default(_) if this is not a reference type */;
        }
        catch (Exception ex)
        {
            // Debug.Print(ex.Message)
            // Log the error
            try
            {
                CkartrisFormatErrors.LogError("MailchimpBLL AddProduct: " + ex.Message);
            }
            catch (Exception ex2)
            {
            }

            return null/* TODO Change to default(_) if this is not a reference type */;
        }
    }

    /// <summary>
    ///     ''' Add cart to mailchimp
    ///     ''' </summary>
    public async Task<Cart> AddCart(Customer customer, int orderId)
    {
        try
        {
            string idSufix = orderId;
            if (orderId == 0)
            {
                var timestamp = System.Convert.ToInt64(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds);
                idSufix = customer.Id + "_" + timestamp;
            }

            CurrencyCode currencyCodeEnum = (CurrencyCode)System.Enum.Parse(typeof(CurrencyCode), this.kartrisCurrencyCode);
            Cart cart = new Cart()
            {
                Id = "cart_" + idSufix,
                Customer = new Customer() { Id = customer.Id, OptInStatus = true },
                CurrencyCode = currencyCodeEnum,
                OrderTotal = kartrisBasket.FinalPriceIncTax,
                CheckoutUrl = CkartrisBLL.WebShopURL.ToLower + "Checkout.aspx",
                Lines = new Collection<Line>()
            };
            Product product;
            double itemprice;
            for (int counter = 0; counter <= kartrisBasket.BasketItems.Count - 1; counter++)
            {
                product = await AddProduct(kartrisBasket.BasketItems(counter));

                if (kartrisBasket.BasketItems(counter).PricesIncTax == true)
                    itemprice = (kartrisBasket.BasketItems(counter).IR_PricePerItem);
                else
                    itemprice = Math.Round((kartrisBasket.BasketItems(counter).IR_PricePerItem * (1 + kartrisBasket.BasketItems(counter).IR_TaxPerItem)), 2);

                cart.Lines.Add(new Line()
                {
                    Id = "cart_" + idSufix + "_l" + counter,
                    ProductId = kartrisBasket.BasketItems(counter).ProductID,
                    ProductTitle = kartrisBasket.BasketItems(counter).Name,
                    ProductVariantId = kartrisBasket.BasketItems(counter).ProductID,
                    ProductVariantTitle = kartrisBasket.BasketItems(counter).Name,
                    Quantity = kartrisBasket.BasketItems(counter).Quantity,
                    Price = itemprice
                });
            }

            Cart taskResult = manager.ECommerceStores.Carts(mcStoreId).AddAsync(cart).Result;

            return taskResult;
        }
        catch (Exception ex)
        {
            // Debug.Print(ex.Message)
            // Log the error
            var trace = new System.Diagnostics.StackTrace(ex, true);
            try
            {
                CkartrisFormatErrors.LogError("MailchimpBLL AddCart stacktrace: " + ex.StackTrace + Constants.vbCrLf + "Error in AddCart - Line number:" + trace.GetFrame(0).GetFileLineNumber().ToString());
                CkartrisFormatErrors.LogError("MailchimpBLL AddCart: " + ex.Message);
            }
            catch (Exception ex2)
            {
            }


            // Avoid build warnings
            return null/* TODO Change to default(_) if this is not a reference type */;
        }
    }

    /// <summary>
    ///     ''' Delete cart
    ///     ''' </summary>
    public async Task<bool> DeleteCart(string cartId)
    {
        bool result = false;
        try
        {
            await manager.ECommerceStores.Carts(mcStoreId).DeleteAsync(cartId).ConfigureAwait(false);
            result = true;

            // Deleting XML Files
            string xmlPath = Path.Combine(HttpRuntime.AppDomainAppPath, @"Uploads\\Mailchimp\\XmlStoring");
            int orderId = 0;
            if (cartId.Contains("cart"))
            {
                string[] cartIdSplit = cartId.Split(new string[] { "cart_" }, StringSplitOptions.None);
                orderId = int.Parse(cartIdSplit[1]);
            }
            else
                orderId = int.Parse(cartId);
            string xmlFilePath = xmlPath + @"\" + orderId.ToString() + "_basket.config";
            string orderXmlFilePath = xmlPath + @"\" + orderId.ToString() + "_order.config";

            if (File.Exists(xmlFilePath))
                File.Delete(xmlFilePath);
            if (File.Exists(orderXmlFilePath))
                File.Delete(orderXmlFilePath);
        }
        catch (Exception ex)
        {
            if (ex.Message.Contains("Unable to find"))
                result = true;
            else
                result = false;
            // Debug.Print(ex.Message)
            // Log the error
            try
            {
                CkartrisFormatErrors.LogError("MailchimpBLL DeleteCart: " + ex.Message);
            }
            catch (Exception ex2)
            {
            }
        }
        return result;
    }

    /// <summary>
    ///     ''' Delete order
    ///     ''' </summary>
    public async Task<bool> DeleteOrder(string orderId)
    {
        bool result = false;
        try
        {
            await manager.ECommerceStores.Orders(mcStoreId).DeleteAsync(orderId).ConfigureAwait(false);
            result = true;
        }
        catch (Exception ex)
        {
            if (ex.Message.Contains("Unable to find"))
                result = true;
            else
                result = false;
            // Debug.Print(ex.Message)
            // Log the error
            try
            {
                CkartrisFormatErrors.LogError("MailchimpBLL DeleteOrder: " + ex.Message);
            }
            catch (Exception ex2)
            {
            }
        }
        return result;
    }

    /// <summary>
    ///     ''' Add order
    ///     ''' </summary>
    public async Task<Order> AddOrder(Customer customer, string cartId)
    {
        try
        {
            string timestamp = "";
            if ((IsDBNull(CkartrisDisplayFunctions.NowOffset())))
                timestamp = CkartrisDisplayFunctions.NowOffset().ToString("yyyy-MM-dd HH:mm");
            else
                timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm");

            if (kartrisBasket == null)
            {
                int orderId = 0;
                if (cartId.Contains("cart"))
                {
                    string[] cartIdSplit = cartId.Split(new string[] { "cart_" }, StringSplitOptions.None);
                    orderId = int.Parse(cartIdSplit[1]);
                }
                else
                    orderId = int.Parse(cartId);

                string xmlPath = Path.Combine(System.Web.HttpContext.Current.Request.PhysicalApplicationPath, @"Uploads\\Mailchimp\\XmlStoring");
                string xmlFilePath = xmlPath + @"\" + orderId.ToString() + "_basket.config";

                // Open the file to read from.
                string readText = File.ReadAllText(xmlFilePath);

                Kartris.Basket objBasket = Payment.Deserialize(readText, typeof(Kartris.Basket));
                kartrisBasket = objBasket;
            }

            CurrencyCode currencyCodeEnum = (CurrencyCode)System.Enum.Parse(typeof(CurrencyCode), this.kartrisCurrencyCode);

            Order order = new Order()
            {
                Id = "order_" + cartId,
                Customer = new Customer() { Id = customer.Id },
                CurrencyCode = currencyCodeEnum,
                OrderTotal = kartrisBasket.FinalPriceIncTax,
                ProcessedAtForeign = timestamp,
                UpdatedAtForeign = timestamp,
                Lines = new Collection<Line>()
            };
            Product product;
            double itemprice;

            for (int counter = 0; counter <= kartrisBasket.BasketItems.Count - 1; counter++)
            {
                product = await AddProduct(kartrisBasket.BasketItems(counter));

                if (kartrisBasket.BasketItems(counter).PricesIncTax == true)
                    itemprice = (kartrisBasket.BasketItems(counter).IR_PricePerItem);
                else
                    itemprice = Math.Round((kartrisBasket.BasketItems(counter).IR_PricePerItem * (1 + kartrisBasket.BasketItems(counter).IR_TaxPerItem)), 2);

                order.Lines.Add(new Line()
                {
                    Id = "order_" + cartId + "_l" + counter,
                    ProductId = kartrisBasket.BasketItems(counter).ProductID,
                    ProductTitle = kartrisBasket.BasketItems(counter).Name,
                    ProductVariantId = kartrisBasket.BasketItems(counter).ProductID,
                    ProductVariantTitle = kartrisBasket.BasketItems(counter).Name,
                    Quantity = kartrisBasket.BasketItems(counter).Quantity,
                    Price = itemprice
                });
            }
            Order taskResult = await manager.ECommerceStores.Orders(mcStoreId).AddAsync(order).ConfigureAwait(false);

            if (taskResult != null)
            {
                customer.OrdersCount = customer.OrdersCount + 1;
                customer.OptInStatus = true;
                Customer updateCustomer = await manager.ECommerceStores.Customers(mcStoreId).UpdateAsync(customer.Id, customer).ConfigureAwait(false);
            }

            return taskResult;
        }
        catch (Exception ex)
        {
            // Log the error
            try
            {
                CkartrisFormatErrors.LogError("MailchimpBLL AddOrder: " + ex.Message);
                var trace = new System.Diagnostics.StackTrace(ex, true);
                CkartrisFormatErrors.LogError("MailchimpBLL AddOrder stacktrace: " + ex.StackTrace + Constants.vbCrLf + "Error in AddOrder - Line number:" + trace.GetFrame(0).GetFileLineNumber().ToString());
            }
            catch (Exception ex2)
            {
            }
        }
        return null/* TODO Change to default(_) if this is not a reference type */;
    }


    /// <summary>
    ///     ''' Add order
    ///     ''' </summary>
    public async Task<Order> AddOrderByCustomerId(int customerId, string cartId)
    {
        try
        {
            double O_TotalPriceGateway = 0;
            int orderId = 0;
            string timestamp = "";
            if ((IsDBNull(CkartrisDisplayFunctions.NowOffset())))
                timestamp = CkartrisDisplayFunctions.NowOffset().ToString("yyyy-MM-dd HH:mm");
            else
                timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm");

            if (kartrisBasket == null)
            {
                if (cartId.Contains("cart"))
                {
                    string[] cartIdSplit = cartId.Split(new string[] { "cart_" }, StringSplitOptions.None);
                    orderId = int.Parse(cartIdSplit[1]);
                }
                else
                    orderId = int.Parse(cartId);

                string xmlPath = Path.Combine(System.Web.HttpContext.Current.Request.PhysicalApplicationPath, @"Uploads\\Mailchimp\\XmlStoring");
                string xmlFilePath = xmlPath + @"\" + orderId.ToString() + "_basket.config";

                string orderXmlFilePath = xmlPath + @"\" + orderId.ToString() + "_order.config";

                // Open the file to read from.
                string readText = File.ReadAllText(xmlFilePath);
                string orderReadText = File.ReadAllText(orderXmlFilePath);
                try
                {
                    Kartris.Basket objBasket = Payment.Deserialize(readText, typeof(Kartris.Basket));
                    kartrisBasket = objBasket;

                    var xmlElem = XElement.Parse(orderReadText);

                    xmlElem = xmlElem.Element("Amount");
                    O_TotalPriceGateway = double.Parse(xmlElem.Value);
                }
                catch (Exception ex)
                {
                    CkartrisFormatErrors.LogError("MailchimpBLL XML catch");
                    CkartrisFormatErrors.LogError("MailchimpBLL XML catch: " + ex.Message);
                }
            }


            CurrencyCode currencyCodeEnum = (CurrencyCode)System.Enum.Parse(typeof(CurrencyCode), this.kartrisCurrencyCode);

            Order order = new Order()
            {
                Id = "order_" + cartId,
                Customer = new Customer() { Id = customerId },
                CurrencyCode = currencyCodeEnum,
                OrderTotal = O_TotalPriceGateway,
                ProcessedAtForeign = timestamp,
                UpdatedAtForeign = timestamp,
                Lines = new Collection<Line>()
            };
            Product product;
            double itemprice;

            for (int counter = 0; counter <= kartrisBasket.BasketItems.Count - 1; counter++)
            {
                try
                {
                    product = await AddProduct(kartrisBasket.BasketItems(counter));
                }
                catch (Exception ex)
                {
                    CkartrisFormatErrors.LogError("MailchimpBLL AddOrderByCustomerId addproduct");
                }

                try
                {
                    if (kartrisBasket.BasketItems(counter).PricesIncTax == true)
                        itemprice = (kartrisBasket.BasketItems(counter).IR_PricePerItem);
                    else
                        itemprice = Math.Round((kartrisBasket.BasketItems(counter).IR_PricePerItem * (1 + kartrisBasket.BasketItems(counter).IR_TaxPerItem)), 2);

                    if (itemprice > 0)
                        order.Lines.Add(new Line()
                        {
                            Id = "order_" + cartId + "_l" + counter,
                            ProductId = kartrisBasket.BasketItems(counter).ProductID,
                            ProductTitle = kartrisBasket.BasketItems(counter).Name,
                            ProductVariantId = kartrisBasket.BasketItems(counter).ProductID,
                            ProductVariantTitle = kartrisBasket.BasketItems(counter).Name,
                            Quantity = kartrisBasket.BasketItems(counter).Quantity,
                            Price = itemprice
                        });
                }
                catch (Exception ex)
                {
                    CkartrisFormatErrors.LogError("MailchimpBLL AddOrderByCustomerId orderline add : " + ex.Message);
                    var trace = new System.Diagnostics.StackTrace(ex, true);
                    CkartrisFormatErrors.LogError("MailchimpBLL AddOrderByCustomerId  orderline addstacktrace: " + ex.StackTrace + Constants.vbCrLf + "Error in AddOrder - Line number:" + trace.GetFrame(0).GetFileLineNumber().ToString());
                }
            }
            Order taskResult = manager.ECommerceStores.Orders(mcStoreId).AddAsync(order).Result;

            Customer mcCustomer = manager.ECommerceStores.Customers(mcStoreId).GetAsync(customerId).Result;


            if (taskResult != null)
            {
                mcCustomer.OrdersCount = mcCustomer.OrdersCount + 1;
                mcCustomer.OptInStatus = true;
                Customer updateCustomer = await manager.ECommerceStores.Customers(mcStoreId).UpdateAsync(customerId, mcCustomer).ConfigureAwait(false);
            }

            return taskResult;
        }
        catch (Exception ex)
        {
            // Debug.Print(ex.Message)
            // Log the error
            try
            {
                CkartrisFormatErrors.LogError("MailchimpBLL AddOrderByCustomerId: " + ex.Message);
                var trace = new System.Diagnostics.StackTrace(ex, true);
                CkartrisFormatErrors.LogError("MailchimpBLL AddOrderByCustomerId stacktrace: " + ex.StackTrace + Constants.vbCrLf + "Error in AddOrder - Line number:" + trace.GetFrame(0).GetFileLineNumber().ToString());
            }
            catch (Exception ex2)
            {
            }
        }
        return null/* TODO Change to default(_) if this is not a reference type */;
    }

    /// <summary>
    ///     ''' Add store
    ///     ''' </summary>
    public async Task<Store> AddStore(string storeId, string storeName = "Kartris Store", string storeDomain = "www.kartris.com", string EmailAddress = "someemail@cactusoft.com")
    {
        try
        {
            CurrencyCode currencyCodeEnum = (CurrencyCode)System.Enum.Parse(typeof(CurrencyCode), this.kartrisCurrencyCode);
            var storeObj = new Store() { Id = storeId, ListId = listId, Name = storeName, Domain = storeDomain, EmailAddress = EmailAddress, CurrencyCode = currencyCodeEnum };
            Store taskResult = await manager.ECommerceStores.AddAsync(storeObj).ConfigureAwait(false);

            return taskResult;
        }
        catch (Exception ex)
        {
            // Debug.Print(ex.Message)
            // Log the error
            try
            {
                CkartrisFormatErrors.LogError("MailchimpBLL AddStore: " + ex.Message);
                throw ex;
            }
            catch (Exception ex2)
            {
            }
        }
        return null/* TODO Change to default(_) if this is not a reference type */;
    }

    /// <summary>
    ///     ''' Add mailinglist signup to mailchimp
    ///     ''' </summary>
    public async Task<Member> AddListSubscriber(string strEmail)
    {
        try
        {
            Member member = new Member()
            {
                EmailAddress = strEmail,
                Status = Status.Subscribed
            };
            string strListID = KartSettingsManager.GetKartConfig("general.mailchimp.mailinglistid");
            Member taskResult = await manager.Members.AddOrUpdateAsync(strListID, member).ConfigureAwait(false);
            return taskResult;
        }
        catch (Exception ex)
        {
            // Debug.Print(ex.Message)
            // Log the error
            try
            {
                CkartrisFormatErrors.LogError("MailchimpBLL AddListSubscriber: " + ex.Message);
                throw ex;
            }
            catch (Exception ex2)
            {
            }
        }
        return null/* TODO Change to default(_) if this is not a reference type */;
    }
}
