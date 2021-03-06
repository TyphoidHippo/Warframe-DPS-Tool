﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WFLib;

namespace WarframeDPSTool
{
    class PercentBinding
    {
        private readonly TextBox _FormElement;
        private readonly Action _OnChange;
        public Percent Value
        {
            get
            {
                var s = this._FormElement.Text;
                if (string.IsNullOrWhiteSpace(s)) { return Percent.Zero; }

                double d = 0;
                double.TryParse(this._FormElement.Text, out d);
                return Percent.FromPercent0to100(d);
            }
            set { this._FormElement.Text = value.AsPercent0to100.ToString(); this._OnChange(); }
        }

        public PercentBinding(TextBox pFormElement, Action pOnChange)
        {
            this._FormElement = pFormElement;
            this._OnChange = pOnChange;
            //{ ((Form1)(pFormElement.FindForm())).mass; };
            this._FormElement.TextChanged += _FormElement_TextChanged;
        }

        private void _FormElement_TextChanged(object sender, EventArgs e)
        {
            this._OnChange();
        }

        public void Clear()
        {
            this._FormElement.Text = "";
        }
    }
}
