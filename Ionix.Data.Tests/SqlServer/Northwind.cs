

// This file was automatically generated.
// Do not make changes directly to this file - edit the template instead.
// 
// The following connection settings were used to generate this file
// 
//     Configuration file:     "ionixTests\App.config"
//     Connection String Name: "SqlConn"
//     Connection String:      "Data Source=.;Initial Catalog=NORTHWND;User Id=admin;password=**zapped**;"

// ReSharper disable RedundantUsingDirective
// ReSharper disable DoNotCallOverridableMethodsInConstructor
// ReSharper disable InconsistentNaming
// ReSharper disable PartialTypeWithSinglePart
// ReSharper disable PartialMethodWithSinglePart
// ReSharper disable RedundantNameQualifier

using System;
using Ionix.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ionix.Data.Tests.SqlServer
{

    // ************************************************************************
    // POCO classes

    [Table("Alphabeticallistofproducts")]
    public partial class Alphabeticallistofproducts
    {
        [DbSchema(IsNullable = false, IsKey = true)]
        public int ProductID { get; set; }

        [DbSchema(IsNullable = false, IsKey = true, MaxLength = 40)]
        public string ProductName { get; set; }

        [DbSchema(IsNullable = true)]
        public int? SupplierID { get; set; }

        [DbSchema(IsNullable = true)]
        public int? CategoryID { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 20)]
        public string QuantityPerUnit { get; set; }

        [DbSchema(IsNullable = true)]
        public decimal? UnitPrice { get; set; }

        [DbSchema(IsNullable = true)]
        public short? UnitsInStock { get; set; }

        [DbSchema(IsNullable = true)]
        public short? UnitsOnOrder { get; set; }

        [DbSchema(IsNullable = true)]
        public short? ReorderLevel { get; set; }

        [DbSchema(IsNullable = false, IsKey = true)]
        public bool Discontinued { get; set; }

        [DbSchema(IsNullable = false, IsKey = true, MaxLength = 15)]
        public string CategoryName { get; set; }

    }


    [Table("Categories")]
    public partial class Categories
    {
        [DbSchema(IsNullable = false, IsKey = true, DatabaseGeneratedOption = Ionix.Data.StoreGeneratedPattern.Identity)]
        public int CategoryID { get; set; }

        [DbSchema(IsNullable = false, MaxLength = 15)]
        public string CategoryName { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 1073741823)]
        public string Description { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 2147483647)]
        public byte[] Picture { get; set; }

    }


    [Table("CategorySalesfor1997")]
    public partial class CategorySalesfor1997
    {
        [DbSchema(IsNullable = false, IsKey = true, MaxLength = 15)]
        public string CategoryName { get; set; }

        [DbSchema(IsNullable = true)]
        public decimal? CategorySales { get; set; }

    }


    [Table("CurrentProductList")]
    public partial class CurrentProductList
    {
        [DbSchema(IsNullable = false, IsKey = true, DatabaseGeneratedOption = Ionix.Data.StoreGeneratedPattern.Identity)]
        public int ProductID { get; set; }

        [DbSchema(IsNullable = false, IsKey = true, MaxLength = 40)]
        public string ProductName { get; set; }

    }


    [Table("CustomerandSuppliersbyCity")]
    public partial class CustomerandSuppliersbyCity
    {
        [DbSchema(IsNullable = true, MaxLength = 15)]
        public string City { get; set; }

        [DbSchema(IsNullable = false, IsKey = true, MaxLength = 40)]
        public string CompanyName { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 30)]
        public string ContactName { get; set; }

        [DbSchema(IsNullable = false, IsKey = true, MaxLength = 9)]
        public string Relationship { get; set; }

    }


    [Table("CustomerDemographics")]
    public partial class CustomerDemographics
    {
        [DbSchema(IsNullable = false, IsKey = true, MaxLength = 10)]
        public string CustomerTypeID { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 1073741823)]
        public string CustomerDesc { get; set; }

    }


    [Table("Customers")]
    public partial class Customers
    {
        [DbSchema(IsNullable = false, IsKey = true, MaxLength = 5)]
        public string CustomerID { get; set; }

        [DbSchema(IsNullable = false, MaxLength = 40)]
        public string CompanyName { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 30)]
        public string ContactName { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 30)]
        public string ContactTitle { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 60)]
        public string Address { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 15)]
        public string City { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 15)]
        public string Region { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 10)]
        public string PostalCode { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 15)]
        public string Country { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 24)]
        public string Phone { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 24)]
        public string Fax { get; set; }

    }


    [Table("Employees")]
    public partial class Employees
    {
        [DbSchema(IsNullable = false, IsKey = true, DatabaseGeneratedOption = Ionix.Data.StoreGeneratedPattern.Identity)]
        public int EmployeeID { get; set; }

        [DbSchema(IsNullable = false, MaxLength = 20)]
        public string LastName { get; set; }

        [DbSchema(IsNullable = false, MaxLength = 10)]
        public string FirstName { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 30)]
        public string Title { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 25)]
        public string TitleOfCourtesy { get; set; }

        [DbSchema(IsNullable = false)]
        public DateTime? BirthDate { get; set; }

        [DbSchema(IsNullable = false)]
        public DateTime? HireDate { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 60)]
        public string Address { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 15)]
        public string City { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 15)]
        public string Region { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 10)]
        public string PostalCode { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 15)]
        public string Country { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 24)]
        public string HomePhone { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 4)]
        public string Extension { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 2147483647)]
        public byte[] Photo { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 1073741823)]
        public string Notes { get; set; }

        [DbSchema(IsNullable = true)]
        public int? ReportsTo { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 255)]
        public string PhotoPath { get; set; }

    }


    [Table("Invoices")]
    public partial class Invoices
    {
        [DbSchema(IsNullable = true, MaxLength = 40)]
        public string ShipName { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 60)]
        public string ShipAddress { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 15)]
        public string ShipCity { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 15)]
        public string ShipRegion { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 10)]
        public string ShipPostalCode { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 15)]
        public string ShipCountry { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 5)]
        public string CustomerID { get; set; }

        [DbSchema(IsNullable = false, IsKey = true, MaxLength = 40)]
        public string CustomerName { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 60)]
        public string Address { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 15)]
        public string City { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 15)]
        public string Region { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 10)]
        public string PostalCode { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 15)]
        public string Country { get; set; }

        [DbSchema(IsNullable = false, IsKey = true, MaxLength = 31)]
        public string Salesperson { get; set; }

        [DbSchema(IsNullable = false, IsKey = true)]
        public int OrderID { get; set; }

        [DbSchema(IsNullable = true)]
        public DateTime? OrderDate { get; set; }

        [DbSchema(IsNullable = true)]
        public DateTime? RequiredDate { get; set; }

        [DbSchema(IsNullable = true)]
        public DateTime? ShippedDate { get; set; }

        [DbSchema(IsNullable = false, IsKey = true, MaxLength = 40)]
        public string ShipperName { get; set; }

        [DbSchema(IsNullable = false, IsKey = true)]
        public int ProductID { get; set; }

        [DbSchema(IsNullable = false, IsKey = true, MaxLength = 40)]
        public string ProductName { get; set; }

        [DbSchema(IsNullable = false, IsKey = true)]
        public decimal UnitPrice { get; set; }

        [DbSchema(IsNullable = false, IsKey = true)]
        public short Quantity { get; set; }

        [DbSchema(IsNullable = false, IsKey = true)]
        public float Discount { get; set; }

        [DbSchema(IsNullable = true)]
        public decimal? ExtendedPrice { get; set; }

        [DbSchema(IsNullable = true)]
        public decimal? Freight { get; set; }

    }


    [Table("OrderDetails")]
    public partial class OrderDetails
    {
        [DbSchema(IsNullable = false, IsKey = true)]
        public int OrderID { get; set; }

        [DbSchema(IsNullable = false, IsKey = true)]
        public int ProductID { get; set; }

        [DbSchema(IsNullable = false, DefaultValue = "0m")]
        public decimal UnitPrice { get; set; }

        [DbSchema(IsNullable = false, DefaultValue = "1")]
        public short Quantity { get; set; }

        [DbSchema(IsNullable = false, DefaultValue = "0")]
        public float Discount { get; set; }

    }


    [Table("OrderDetailsExtended")]
    public partial class OrderDetailsExtended
    {
        [DbSchema(IsNullable = false, IsKey = true)]
        public int OrderID { get; set; }

        [DbSchema(IsNullable = false, IsKey = true)]
        public int ProductID { get; set; }

        [DbSchema(IsNullable = false, IsKey = true, MaxLength = 40)]
        public string ProductName { get; set; }

        [DbSchema(IsNullable = false, IsKey = true)]
        public decimal UnitPrice { get; set; }

        [DbSchema(IsNullable = false, IsKey = true)]
        public short Quantity { get; set; }

        [DbSchema(IsNullable = false, IsKey = true)]
        public float Discount { get; set; }

        [DbSchema(IsNullable = true)]
        public decimal? ExtendedPrice { get; set; }

    }


    [Table("Orders")]
    public partial class Orders
    {
        [DbSchema(IsNullable = false, IsKey = true, DatabaseGeneratedOption = Ionix.Data.StoreGeneratedPattern.Identity)]
        public int OrderID { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 5)]
        public string CustomerID { get; set; }

        [DbSchema(IsNullable = true)]
        public int? EmployeeID { get; set; }

        [DbSchema(IsNullable = true)]
        public DateTime? OrderDate { get; set; }

        [DbSchema(IsNullable = true)]
        public DateTime? RequiredDate { get; set; }

        [DbSchema(IsNullable = true)]
        public DateTime? ShippedDate { get; set; }

        [DbSchema(IsNullable = true)]
        public int? ShipVia { get; set; }

        [DbSchema(IsNullable = true, DefaultValue = "0m")]
        public decimal? Freight { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 40)]
        public string ShipName { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 60)]
        public string ShipAddress { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 15)]
        public string ShipCity { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 15)]
        public string ShipRegion { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 10)]
        public string ShipPostalCode { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 15)]
        public string ShipCountry { get; set; }

    }


    [Table("OrdersQry")]
    public partial class OrdersQry
    {
        [DbSchema(IsNullable = false, IsKey = true)]
        public int OrderID { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 5)]
        public string CustomerID { get; set; }

        [DbSchema(IsNullable = true)]
        public int? EmployeeID { get; set; }

        [DbSchema(IsNullable = true)]
        public DateTime? OrderDate { get; set; }

        [DbSchema(IsNullable = true)]
        public DateTime? RequiredDate { get; set; }

        [DbSchema(IsNullable = true)]
        public DateTime? ShippedDate { get; set; }

        [DbSchema(IsNullable = true)]
        public int? ShipVia { get; set; }

        [DbSchema(IsNullable = true)]
        public decimal? Freight { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 40)]
        public string ShipName { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 60)]
        public string ShipAddress { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 15)]
        public string ShipCity { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 15)]
        public string ShipRegion { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 10)]
        public string ShipPostalCode { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 15)]
        public string ShipCountry { get; set; }

        [DbSchema(IsNullable = false, IsKey = true, MaxLength = 40)]
        public string CompanyName { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 60)]
        public string Address { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 15)]
        public string City { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 15)]
        public string Region { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 10)]
        public string PostalCode { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 15)]
        public string Country { get; set; }

    }


    [Table("OrderSubtotals")]
    public partial class OrderSubtotals
    {
        [DbSchema(IsNullable = false, IsKey = true)]
        public int OrderID { get; set; }

        [DbSchema(IsNullable = true)]
        public decimal? Subtotal { get; set; }

    }


    [Table("Products")]
    public partial class Products
    {
        [DbSchema(IsNullable = false, IsKey = true, DatabaseGeneratedOption = Ionix.Data.StoreGeneratedPattern.Identity)]
        public int ProductID { get; set; }

        [DbSchema(IsNullable = false, MaxLength = 40)]
        public string ProductName { get; set; }

        [DbSchema(IsNullable = true)]
        public int? SupplierID { get; set; }

        [DbSchema(IsNullable = true)]
        public int? CategoryID { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 20)]
        public string QuantityPerUnit { get; set; }

        [DbSchema(IsNullable = true, DefaultValue = "0m")]
        public decimal? UnitPrice { get; set; }

        [DbSchema(IsNullable = true, DefaultValue = "0")]
        public short? UnitsInStock { get; set; }

        [DbSchema(IsNullable = true, DefaultValue = "0")]
        public short? UnitsOnOrder { get; set; }

        [DbSchema(IsNullable = true, DefaultValue = "0")]
        public short? ReorderLevel { get; set; }

        [DbSchema(IsNullable = false, DefaultValue = "0")]
        public bool Discontinued { get; set; }

    }


    [Table("ProductsAboveAveragePrice")]
    public partial class ProductsAboveAveragePrice
    {
        [DbSchema(IsNullable = false, IsKey = true, MaxLength = 40)]
        public string ProductName { get; set; }

        [DbSchema(IsNullable = true)]
        public decimal? UnitPrice { get; set; }

    }


    [Table("ProductSalesfor1997")]
    public partial class ProductSalesfor1997
    {
        [DbSchema(IsNullable = false, IsKey = true, MaxLength = 15)]
        public string CategoryName { get; set; }

        [DbSchema(IsNullable = false, IsKey = true, MaxLength = 40)]
        public string ProductName { get; set; }

        [DbSchema(IsNullable = true)]
        public decimal? ProductSales { get; set; }

    }


    [Table("ProductsbyCategory")]
    public partial class ProductsbyCategory
    {
        [DbSchema(IsNullable = false, IsKey = true, MaxLength = 15)]
        public string CategoryName { get; set; }

        [DbSchema(IsNullable = false, IsKey = true, MaxLength = 40)]
        public string ProductName { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 20)]
        public string QuantityPerUnit { get; set; }

        [DbSchema(IsNullable = true)]
        public short? UnitsInStock { get; set; }

        [DbSchema(IsNullable = false, IsKey = true)]
        public bool Discontinued { get; set; }

    }


    [Table("Region")]
    public partial class Region
    {
        [DbSchema(IsNullable = false, IsKey = true)]
        public int RegionID { get; set; }

        [DbSchema(IsNullable = false, MaxLength = 50)]
        public string RegionDescription { get; set; }

    }


    [Table("SalesbyCategory")]
    public partial class SalesbyCategory
    {
        [DbSchema(IsNullable = false, IsKey = true)]
        public int CategoryID { get; set; }

        [DbSchema(IsNullable = false, IsKey = true, MaxLength = 15)]
        public string CategoryName { get; set; }

        [DbSchema(IsNullable = false, IsKey = true, MaxLength = 40)]
        public string ProductName { get; set; }

        [DbSchema(IsNullable = true)]
        public decimal? ProductSales { get; set; }

    }


    [Table("SalesTotalsbyAmount")]
    public partial class SalesTotalsbyAmount
    {
        [DbSchema(IsNullable = true)]
        public decimal? SaleAmount { get; set; }

        [DbSchema(IsNullable = false, IsKey = true)]
        public int OrderID { get; set; }

        [DbSchema(IsNullable = false, IsKey = true, MaxLength = 40)]
        public string CompanyName { get; set; }

        [DbSchema(IsNullable = true)]
        public DateTime? ShippedDate { get; set; }

    }


    [Table("Shippers")]
    public partial class Shippers
    {
        [DbSchema(IsNullable = false, IsKey = true, DatabaseGeneratedOption = Ionix.Data.StoreGeneratedPattern.Identity)]
        public int ShipperID { get; set; }

        [DbSchema(IsNullable = false, MaxLength = 40)]
        public string CompanyName { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 24)]
        public string Phone { get; set; }

    }


    [Table("SummaryofSalesbyQuarter")]
    public partial class SummaryofSalesbyQuarter
    {
        [DbSchema(IsNullable = true)]
        public DateTime? ShippedDate { get; set; }

        [DbSchema(IsNullable = false, IsKey = true)]
        public int OrderID { get; set; }

        [DbSchema(IsNullable = true)]
        public decimal? Subtotal { get; set; }

    }


    [Table("SummaryofSalesbyYear")]
    public partial class SummaryofSalesbyYear
    {
        [DbSchema(IsNullable = true)]
        public DateTime? ShippedDate { get; set; }

        [DbSchema(IsNullable = false, IsKey = true)]
        public int OrderID { get; set; }

        [DbSchema(IsNullable = true)]
        public decimal? Subtotal { get; set; }

    }


    [Table("Suppliers")]
    public partial class Suppliers
    {
        [DbSchema(IsNullable = false, IsKey = true, DatabaseGeneratedOption = Ionix.Data.StoreGeneratedPattern.Identity)]
        public int SupplierID { get; set; }

        [DbSchema(IsNullable = false, MaxLength = 40)]
        public string CompanyName { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 30)]
        public string ContactName { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 30)]
        public string ContactTitle { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 60)]
        public string Address { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 15)]
        public string City { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 15)]
        public string Region { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 10)]
        public string PostalCode { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 15)]
        public string Country { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 24)]
        public string Phone { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 24)]
        public string Fax { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 1073741823)]
        public string HomePage { get; set; }

    }


    [Table("Territories")]
    public partial class Territories
    {
        [DbSchema(IsNullable = false, IsKey = true, MaxLength = 20)]
        public string TerritoryID { get; set; }

        [DbSchema(IsNullable = false, MaxLength = 50)]
        public string TerritoryDescription { get; set; }

        [DbSchema(IsNullable = false)]
        public int RegionID { get; set; }

    }
}

