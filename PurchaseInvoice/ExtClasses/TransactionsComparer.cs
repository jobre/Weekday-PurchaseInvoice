using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace GemoForms
{
  class TransactionsComparer : IComparer
  {
    private int col;
    public TransactionsComparer()
    {
      col = 0;
    }
    public TransactionsComparer(int column)
    {
      col = column;
    }
    public int Compare(object x, object y)
    {
      int returnVal = -1;
      try
      {
        returnVal = String.Compare(((frmTransactions.Transaction)x).Date, ((frmTransactions.Transaction)y).Date);
        //returnVal *= -1; // Descending sort
      }
      catch 
      {
      }
      return returnVal;
    }
  }
}
