﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PsdBasesSetter.Repositories.Objects;

namespace PSD
{
    public partial class EditPasswordForm : Form
    {
        private PassItem _passItem;
        public Boolean Confirmed { get; private set; } = false;

        public EditPasswordForm(PassItem pass)
        {
            InitializeComponent();
            if (pass == null)
                pass = new PassItem();
            _passItem = pass;
        }

        private void EditPassword_Load(object sender, EventArgs e)
        {
            txtPass.UseSystemPasswordChar = false;
            FillFields();
        }

        private void FillFields()
        {
            txtTitle.Text = _passItem.Title;
            txtLogin.Text = _passItem.Login;
            cbxEnterWithLogin.Checked = _passItem.EnterWithLogin;
            txtPass.Text = Encoding.ASCII.GetString(_passItem.Pass);
            rtxtDescription.Text = _passItem.Description;
        }

        private void FillPass()
        {
            _passItem.Title = txtTitle.Text;
            _passItem.Login = txtLogin.Text;
            _passItem.EnterWithLogin = cbxEnterWithLogin.Checked;
            _passItem.Pass = Encoding.ASCII.GetBytes(txtPass.Text);
            _passItem.Description = rtxtDescription.Text;
        }

        private void EditPasswordForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            FillPass();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            Confirmed = true;
            this.Close();
        }

        private void btnDiscard_Click(object sender, EventArgs e)
        {
            Confirmed = false;
            this.Close();
        }

        private void cbxShowPassword_CheckedChanged(object sender, EventArgs e)
        {
            txtPass.UseSystemPasswordChar = cbxShowPassword.Checked;
        }

        private void btnGenPass_Click(object sender, EventArgs e)
        {
            string randomPass = new PassGenerator().GenerateStringPassword(16);
            txtPass.Text = randomPass;
        }
    }
}
