using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Weekday;

namespace InvoiceUpdate
{

    public class Invoice
    {
        private GarpGenericDB mIGK, mLR, mHIA;
        Dictionary<string, string> mInvoiceList = new Dictionary<string, string>();

        public Invoice()
        {
            mIGK = new GarpGenericDB("IGK", "SYS", "SYS");
            mLR = new GarpGenericDB("LR", "SYS", "SYS");
            mHIA = new GarpGenericDB("HIA", "SYS", "SYS");

        }

        public void Update()
        {
            int count = 0;
            mLR.find("ZZZ");
            

            while(!mLR.BOF && count < 200)
            {
                mInvoiceList.Add(mLR.getValue("REF").Trim(), mLR.getValue("FNR").Trim());
                mLR.prev();
                count++;
            }
             
            while(!mIGK.EOF)
            {
                if(mIGK.getValue("FAF").Equals("F"))
                {
                    if(mInvoiceList.ContainsKey(mIGK.getValue("TX1").Trim()))
                    {
                        if(updateInvoice(mIGK.getValue("ONR"), mInvoiceList[mIGK.getValue("TX1").Trim()])
                        {
                            mIGK.setValue("FAF", "5");
                        }
                    }
                }
            }
        }

        private bool updateInvoice(string onr, string garpInvoicNo)
        {

            try
            {
                if (!mHIA.find(onr))
                    mHIA.next();

                // Update all deliverynotes without invoicenumber
                while (mHIA.getValue("ONR").Equals(onr) && !mHIA.EOF)
                {
                    if (mHIA.getValue("FNR").Equals(""))
                    {
                        mHIA.setValue("FNR", garpInvoicNo);
                        mHIA.setValue("FAF", "5");
                        mHIA.post();

                        // If we find invoice, update it
                        if (mLR.find(mHIA.getApp.Bolag + garpInvoicNo))
                        {
                            // But only if no delivernote already is present
                            if (mLR.getValue("HNR").Equals(""))
                            {
                                mLR.setValue("HNR", mHIA.getValue("HNR"));
                                mLR.post();
                            }
                        }

                        return true;
                    }
                    mHIA.next();
                }
            }
            catch(Exception e)
            {
                return false;
            }

            return false;

        }

    }
}
