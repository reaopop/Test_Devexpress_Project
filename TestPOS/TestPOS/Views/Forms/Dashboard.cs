using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using DevExpress.XtraEditors;
using TestPOS.DBContext.Models;
using TestPOS.Logic.Presenters;
using TestPOS.Views.Interfaces;

namespace TestPOS.Views.Forms
{
    public partial class Dashboard : DevExpress.XtraEditors.XtraForm,IDashboard
    {

        #region Properties
        public object DataSource { get => gridControl1.DataSource; set => gridControl1.DataSource = value; }
        public object DataSource_Suppliers { get => gridControl_Suppliers.DataSource; set => gridControl_Suppliers.DataSource = value; }

        DashboardPresneter dashboardPresneter;

        #endregion
        public Dashboard()
        {
            InitializeComponent();
            dashboardPresneter = new DashboardPresneter(this);

        }


        private void tileBar_SelectedItemChanged(object sender, TileItemEventArgs e)
        {
            navigationFrame.SelectedPageIndex = tileBarGroupTables.Items.IndexOf(e.Item);
        }
    }
}