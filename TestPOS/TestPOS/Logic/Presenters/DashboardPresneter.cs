using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestPOS.DBContext.Models;
using TestPOS.Views.Interfaces;

namespace TestPOS.Logic.Presenters
{
    public class DashboardPresneter
    {
        #region Fields
        private readonly IDashboard repo;

        #endregion

        #region Constractors
        public DashboardPresneter(IDashboard Repo)
        {
            repo = Repo;
            RefreshData();
        }
        #endregion

        #region Functions
        public void RefreshData()
        {
            using (Model1 db = new Model1())
            {

                repo.DataSource = (from item in db.Products.DefaultIfEmpty()
                                           select new
                                           {
                                               ID_NO = item.ProductID,
                                               ProductName = item.ProductName,
                                               Supplier = item.Supplier.CompanyName,
                                               Category = item.Category.CategoryName,
                                               UnitePrice = item.UnitPrice,
                                           }).ToList();

                repo.DataSource_Suppliers = (from item in db.Suppliers.DefaultIfEmpty()
                                   select new
                                   {
                                       SupplierID = item.SupplierID,
                                       CompanyName = item.CompanyName,
                                       ContactName = item.ContactName,
                                       ContactTitle = item.ContactTitle,
                                       Country = item.Country,
                                   }).ToList();

            }
        }
        #endregion
    }
}
