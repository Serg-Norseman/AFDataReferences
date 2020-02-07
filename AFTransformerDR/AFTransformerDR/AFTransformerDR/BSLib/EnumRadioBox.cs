/*
 *  "BSLib".
 *  Copyright (C) 2011-2017 by Sergey V. Zhdanovskih.
 *
 *  This program is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace BSLib.WinForms.Controls
{
    public class EnumRadioBox : GroupBox, INotifyPropertyChanged
    {
        private List<RadioButton> fButtons;
        private Type fEnumType;
        private int fMaxTextWidth;
        private object fValue;

        public event PropertyChangedEventHandler PropertyChanged;

        [DefaultValue(0)]
        public object Value
        {
            get {
                return fValue;
            }
            set {
                for (int i = 0; i < fButtons.Count; i++) {
                    if (fButtons[i].Tag.Equals(value)) {
                        fButtons[i].Checked = true;
                        break;
                    }
                }

                fValue = value;

                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Value"));
            }
        }

        public Type EnumType
        {
            get { return fEnumType; }
            set {
                fMaxTextWidth = 0;
                this.Controls.Clear();
                if (fButtons == null)
                    fButtons = new List<RadioButton>();
                fButtons.Clear();
                Array strs = Enum.GetValues(value);
                for (int i = 0; i < strs.Length; i++) {
                    RadioButton obj = new RadioButton();
                    obj.Width = this.GetMaxTextWidth();
                    fButtons.Add(obj);
                    this.Controls.Add(obj);
                    obj.Text = strs.GetValue(i).GetDescription();
                    obj.Tag = strs.GetValue(i);
                    obj.Click += ButonClick;
                }
                OrderButtons();
                fEnumType = value;
            }
        }

        public EnumRadioBox()
        {
            DoubleBuffered = true;
            fMaxTextWidth = 0;
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            OrderButtons();
            base.OnSizeChanged(e);
        }

        private int GetMaxTextWidth()
        {
            if (fMaxTextWidth == 0) {
                float mx = 150;
                if (fEnumType != null) {
                    using (var gfx = CreateGraphics()) {
                        Array strs = Enum.GetValues(fEnumType);
                        for (int i = 0; i < strs.Length; i++) {
                            string str = strs.GetValue(i).GetDescription();
                            mx = Math.Max(mx, gfx.MeasureString(str, this.Font).Width);
                        }
                    }
                }
                fMaxTextWidth = (int)mx;
            }

            return fMaxTextWidth;
        }

        private void OrderButtons()
        {
            try {
                int x = 5;
                int y = 20;
                for (int i = 0; i < fButtons.Count; i++) {
                    RadioButton obj = fButtons[i];
                    obj.Top = y;
                    obj.Left = x;
                    y += 20;
                    if (y + 20 > this.Height) {
                        y = 20;
                        x += GetMaxTextWidth();
                    }
                }
            } catch (Exception ex) {
            }
        }

        private void ButonClick(object sender, EventArgs e)
        {
            this.Value = (sender as RadioButton).Tag;
        }
    }
}
