﻿using CartrigeAltstar.Helpers;
using CartrigeAltstar.Model;
using System;
using System.Data.Entity;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Windows.Forms;

namespace CartrigeAltstar
{

    public partial class ListSubdivisionForm : Form
    {

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            PrintDepartment();
        }

        ContexAltstarContext db;
        public ResourceManager resourceManager;

        public ListSubdivisionForm(ResourceManager _resourceManager)
        {
            this.resourceManager = _resourceManager;
            InitializeComponent();
            db = new ContexAltstarContext();
            db.Subdivisions.Load();
            this.Text = resourceManager.GetString("ListOfDepartment");
        }

        public void PrintDepartment()
        {
            var dt = DateTime.Now;
            try
            {
                db.Subdivisions.Load();
                var data = db.Subdivisions.Local.ToBindingList();

                dataGridViewListSubdivision.DataSource = data;

                dataGridViewListSubdivision.Columns["Department"].HeaderText = resourceManager.GetString("Department");
                dataGridViewListSubdivision.Columns["Address"].HeaderText = resourceManager.GetString("Address");

                dataGridViewListSubdivision.ColumnHeadersDefaultCellStyle.Font = new Font(dataGridViewListSubdivision.Font, FontStyle.Bold);

                dataGridViewListSubdivision.Columns["Id"].Width = 30;
                dataGridViewListSubdivision.Columns["Department"].Width = 300;
                dataGridViewListSubdivision.Columns["Address"].Width = 350;

                dataGridViewListSubdivision.Columns["Printers"].Visible = false;
                dataGridViewListSubdivision.Columns["Compatibilities"].Visible = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        //Add
        private void btnAddDepartment_Click(object sender, EventArgs e)
        {

            AddUpdateSubdivision addCartrige = new AddUpdateSubdivision(resourceManager, null);
            DialogResult result = addCartrige.ShowDialog(this);
            if (result == DialogResult.Cancel)
                return;

            PrintDepartment();
        }

        //Update
        private void btnUpdateDepartment_Click(object sender, EventArgs e)
        {

            // update Cartrige
            if (dataGridViewListSubdivision.SelectedRows.Count > 0)
            {
                int index = dataGridViewListSubdivision.SelectedRows[0].Index;
                int id = 0;
                bool converted = int.TryParse(dataGridViewListSubdivision[0, index].Value.ToString(), out id);
                if (converted == false)
                    return;

                AddUpdateSubdivision updateCartrigeForm = new AddUpdateSubdivision(resourceManager, id);
                DialogResult result = updateCartrigeForm.ShowDialog(this);
                if (result == DialogResult.Cancel)
                    return;


                db = new ContexAltstarContext();
                PrintDepartment();
            }

        }

        //Delete
        private void btnDellDepartment_Click(object sender, EventArgs e)
        {

            // Delete Cartrige
            if (dataGridViewListSubdivision.SelectedRows.Count > 0)
            {

                try
                {
                    int department = dataGridViewListSubdivision.SelectedRows[0].Index;
                    int id = 0;
                    bool converted = int.TryParse(dataGridViewListSubdivision[0, department].Value.ToString(), out id);
                    if (converted == false)
                        return;

                    Subdivision depertmentDel = db.Subdivisions.Find(id);
                    //find ForeignKey Printer.SubdivisionId and set null
                    var printer = depertmentDel.Printers.FirstOrDefault(p=>p.SubdivisionId==id);
                    if (printer != null)
                        printer.SubdivisionId = null;

                    //find ForeignKey Printer.SubdivisionId and set null
                    var compatibilities = depertmentDel.Compatibilities.FirstOrDefault(c=>c.SubdivisionId == id);
                        if(compatibilities != null)
                            compatibilities.SubdivisionId = null;

                        db.SaveChanges();
                        db.Subdivisions.Remove(depertmentDel);

                }
                catch (Exception ex)
                {

                    MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                db.SaveChanges();
                MessageBox.Show(resourceManager.GetString("DepartmentWasRemoved"));
                PrintDepartment();
            }
        }

        //export
        private void btnExportExel_Click(object sender, EventArgs e) => ExelHelper.MyExportExel(dataGridViewListSubdivision, true, resourceManager.GetString("ListOfDepartment"));
        private void btnClosed_Click(object sender, EventArgs e) =>this.Close();
        
    }
}
