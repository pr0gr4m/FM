﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FileManagerClient
{
    public partial class TextViewer : Form
    {
        public TextViewer()
        {
            InitializeComponent();
        }

        public TextBox getTextBox()
        {
            return textBox1;
        }
    }
}
