using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Npgsql;

namespace WinFormsApp1
{
    public partial class Form1 : Form
    {

        private string id = "";
        private int intRow = 0;
        public Form1()
        {
            InitializeComponent();
            ResetMe();
        }

        private void ResetMe()
        {

            this.id = string.Empty;

            firstNameTextBox.Text = "";
            lastNameTextBox.Text = "";

            if (genderComboBox.Items.Count > 0)
            {
                genderComboBox.SelectedIndex = 0;
            }

            updateButton.Text = "Update ()";
            deleteButton.Text = "Delete ()";
            keywordTextBox.Clear();

            if (keywordTextBox.CanSelect)
            {
                keywordTextBox.Select();
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            loadData("");
        }

        private void loadData(string keyword)
        {
           // CRUD.sql = "SELECT autoid, firstname, lastname, CONCAT(firstname, ' ',lastname) AS fullname, gender FROM  testDB" +
           //    "WHERE CONCAT(CAST(autoid as varchar), ' ', firstname, ' ', lastname) LIKE @keyword::varchar " +
           //     "OR TRIM(gender) LIKE @keyword::varchar ORDER BY autoid ASC";
           CRUD.sql = "SELECT autoid, firstname, lastname, CONCAT(firstname, ' ',lastname) AS fullname, gender, inserttime FROM  testdb;";
           

            string strKeyword = string.Format("%{0}%", keyword);

            CRUD.cmd = new NpgsqlCommand(CRUD.sql, CRUD.con);
            CRUD.cmd.Parameters.Clear();
            CRUD.cmd.Parameters.AddWithValue("keyword", strKeyword);

            DataTable dt = CRUD.PerformCRUD(CRUD.cmd);

            if (dt.Rows.Count > 0)
            {
                intRow=Convert.ToInt32(dt.Rows.Count.ToString());
            }
            else
            {
                intRow = 0;

            }
            toolStripStatusLabel1.Text = "Number of row(s): " + intRow.ToString();

            DataGridView dgv1 = dataGridView1;

            dgv1.MultiSelect = false;
            dgv1.AutoGenerateColumns = true;
            dgv1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            dgv1.DataSource = dt;

            dgv1.Columns[0].HeaderText = "ID";
            dgv1.Columns[1].HeaderText = "First Name";
            dgv1.Columns[2].HeaderText = "Last Name";
            dgv1.Columns[3].HeaderText = "Full Name";
            dgv1.Columns[4].HeaderText = "Gender";
            dgv1.Columns[5].HeaderText = "InsertTime";

            dgv1.Columns[0].Width = 85;
            dgv1.Columns[1].Width = 170;
            dgv1.Columns[2].Width = 170;
            dgv1.Columns[3].Width = 220;
            dgv1.Columns[4].Width = 100;
            dgv1.Columns[5].Width = 170;

        }

        private void execute(string mySQL, string param)
        {
            CRUD.cmd = new NpgsqlCommand(mySQL, CRUD.con);
            addParamerters(param);
            CRUD.PerformCRUD(CRUD.cmd);

        }

        private void addParamerters(string str)
        {
            CRUD.cmd.Parameters.Clear();
            CRUD.cmd.Parameters.AddWithValue("firstName", firstNameTextBox.Text.Trim());
            CRUD.cmd.Parameters.AddWithValue("lastName", lastNameTextBox.Text.Trim());
            CRUD.cmd.Parameters.AddWithValue("gender", genderComboBox.SelectedItem.ToString());
            CRUD.cmd.Parameters.AddWithValue("insertTime", DateTime.Now); //test and see


            if (str == "Update" || str == "Delete" && !string.IsNullOrEmpty(this.id))
            {
                CRUD.cmd.Parameters.AddWithValue("id", this.id);
            }
        }

        private void insertButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(firstNameTextBox.Text.Trim()) || string.IsNullOrEmpty(lastNameTextBox.Text.Trim()))
            {
                MessageBox.Show("Please input first name and last name.", "Insert Data",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            
            CRUD.sql = "Insert into testDB (firstName, lastName, gender ,inserttime) values ( @firstName, @lastName, @gender, now()); ";//NOW()::Date
            
            execute(CRUD.sql, "Insert");


            MessageBox.Show("dData has been saved.", "Insert Data",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            loadData("");

            ResetMe();


        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex != -1)
            {
                DataGridView dgv1 = dataGridView1;

                this.id = Convert.ToString(dgv1.CurrentRow.Cells[0].Value);
                updateButton.Text = "Update("+ this.id +")";
                deleteButton.Text = "Delete("+ this.id +")";

                firstNameTextBox.Text = Convert.ToString(dgv1.CurrentRow.Cells[1].Value);
                lastNameTextBox.Text = Convert.ToString(dgv1.CurrentRow.Cells[2].Value);

                genderComboBox.SelectedItem = Convert.ToString(dgv1.CurrentRow.Cells[4].Value);

                

            }
        }

        private void updateButton_Click(object sender, EventArgs e)
        {
            if (dataGridView1.Rows.Count == 0)
            {
                return;

            }

            if (string.IsNullOrEmpty(this.id))
            {
                MessageBox.Show("Please select an item from list", "Update Data",
                 MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (string.IsNullOrEmpty(firstNameTextBox.Text.Trim()) || string.IsNullOrEmpty(lastNameTextBox.Text.Trim()))
            {
                MessageBox.Show("Please input first name and last name.", "Insert Data",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            
            CRUD.sql = "Update testDB set firstName = @firstName, lastName= @lastName, gender=@gender , inserttime = now() where autoid = @id::integer; ";

            execute(CRUD.sql, "Update");


            MessageBox.Show("Data has been updated.", "Insert Data",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            loadData("");

            ResetMe();
        }



        private void deleteButton_Click(object sender, EventArgs e)
        {
            if (dataGridView1.Rows.Count == 0)
            {
                return;

            }

            if (string.IsNullOrEmpty(this.id))
            {
                MessageBox.Show("Please select an item from list", "Delete Data",
                 MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if(MessageBox.Show("Do you want to permanently delete the selected record?", "Delete Data",MessageBoxButtons.YesNo,MessageBoxIcon.Question,MessageBoxDefaultButton.Button1) == DialogResult.Yes)
            {
                CRUD.sql = "delete from testDB where autoid = @id::integer; ";

                execute(CRUD.sql, "Update");


                MessageBox.Show("Data has been deleted.", "Delete Data",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                loadData("");

                ResetMe();
            }


        }

        private void searchButton1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(keywordTextBox.Text.Trim()))
            {
                loadData("");
            }
            else
            {
                loadData(keywordTextBox.Text.Trim());
            }

            ResetMe();
        }
    }
}
