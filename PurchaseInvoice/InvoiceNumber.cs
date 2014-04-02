using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace PurchaseInvoice
{
    public partial class InvoiceNumber : Form
    {
        public InvoiceNumber()
        {
            InitializeComponent();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!txtInvoiceNo.Text.Trim().Equals(""))
            {
                InvoiceNo = txtInvoiceNo.Text;
                this.Close();
            }
            else
            {
                InvoiceNo = "--";
                this.Close();
            }
        }

        public string InvoiceNo { get; set; }

        private void InvoiceNumber_Load(object sender, EventArgs e)
        {
            InvoiceNo = "";
        }
    }
}
