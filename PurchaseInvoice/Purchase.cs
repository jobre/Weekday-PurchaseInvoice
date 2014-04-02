using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using GemoForms;

namespace PurchaseInvoice
{
    public class Purchase : IDisposable
    {
        private GarpGenericDB mIGA, mHIA, mLR, mIGK;
        private Garp.Application mGarp;
        private Garp.Dataset dsIGR;
        private Garp.IComponent mInvoiceButton;
        private bool disposed = false;

        public Purchase()
        {
            mGarp = new Garp.Application();

            mIGA = new GarpGenericDB("IGA");
            mIGK = new GarpGenericDB("IGK");
            mHIA = new GarpGenericDB("HIA");
            mLR = new GarpGenericDB("LR");

            dsIGR = mGarp.Datasets.Item("igrMcDataSet");
            dsIGR.AfterScroll += new Garp.IDatasetEvents_AfterScrollEventHandler(dsIGR_AfterScroll);
            dsIGR.BeforePost += new Garp.IDatasetEvents_BeforePostEventHandler(dsIGR_BeforePost);
            mGarp.ButtonClick += new Garp.IGarpApplicationEvents_ButtonClickEventHandler(mGarp_ButtonClick);

            mInvoiceButton = mGarp.Components.AddButton("Invoice");
            mInvoiceButton.Top = 3;
            mInvoiceButton.Left = 550;
            mInvoiceButton.Text = "Fakturanr";
        }

        void mGarp_ButtonClick()
        {
            if (mGarp.Components.CurrentField == "Invoice")
            {
                showInvoiceForm(true);
            }
        }

        void dsIGR_AfterScroll()
        {
            if (shouldWeShowInvoiceForm())
                showInvoiceForm(false);

        }

        void dsIGR_BeforePost()
        {
        }

        private void showInvoiceForm(bool forceupdate)
        {
            InvoiceNumber frm = new InvoiceNumber();
            bool invoiceSet = false;

            frm.ShowDialog();

            mIGK.find(dsIGR.Fields.Item("ONR").Value.PadRight(6) + "  0");
            mIGK.next();

            while (mIGK.getValue("ONR").Equals(dsIGR.Fields.Item("ONR").Value.Trim()) && mIGK.getValue("RDC").Trim().Equals("0") && !mIGK.EOF)
            {
                if (mIGK.getValue("FAF").Equals("F"))
                {
                    mIGK.setValue("TX1", frm.InvoiceNo);
                    mIGK.post();
                    invoiceSet = true;
                    break;
                }
                else if (mIGK.getValue("FAF").Equals("5")) // Kan inte ändra fakturanummer om en Garp faktura redan är uppdaterad med denna faktura
                    invoiceSet = true;

                mIGK.next();
            }

            if(!invoiceSet)
            {
                mIGK.insert();
                mIGK.setValue("ONR", dsIGR.Fields.Item("ONR").Value);
                mIGK.setValue("RDC", "0");
                mIGK.setValue("SQC", "255");
                mIGK.setValue("OSE", "I");
                mIGK.setValue("FAF", "F");
                mIGK.setValue("TX1", frm.InvoiceNo);
                mIGK.post();
            }
        }

        private bool shouldWeShowInvoiceForm()
        {
            mHIA.index = 2;

            if(!mHIA.find(dsIGR.Fields.Item("ONR").Value.PadRight(6)))
                mHIA.next();

            // Check if it exist a deliverynote without invoicnumber
            while (mHIA.getValue("ONR").Equals(dsIGR.Fields.Item("ONR").Value) && !mHIA.EOF)
            {
                if (mHIA.getValue("FNR").Equals(""))
                    return true;
                    
                mHIA.next();
            }

            return false;
        }

        ~Purchase()
        {
            Dispose();
        }

        public virtual void Dispose()
        {
            if (!this.disposed)
            {
                try
                {
                    mIGA.Dispose();
                    mHIA.Dispose();
                    mLR.Dispose();
                    
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(mGarp);
                }
                finally
                {
                    this.disposed = true;
                    GC.SuppressFinalize(this);
                }
            }
        }
    }
}
