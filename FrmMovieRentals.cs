/*
 * Kaylee Nevin
 * CSC 236 - 470
 * Project: Movie Rental 5.0
 * Description: Movie Rental Revision 5
 * Date: 5/6/2016
*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO; //StreamReader

namespace MovieRentals
{
    //I chose a struct because I initially thought that a struct would be more efficient 
    //for this particular program. Classes seemed a little too powerful and unnecessary. 
    //However, because I chose a struct, changing the availability status of the movie 
    //became more difficult. I had to create new objects with a different status and 
    //remove the old object. This would be a very poor way to write an improveable or 
    //expandable program. If I were required to be able to change the status back - I would
    //choose Classes rather than a struct. 

    struct MoviesForRent
    {
        public string category;
        public string title;
        public string movieType;
        public string movieCost;
        public string availability; 
    }

    public partial class FrmMovieRentals : Form
    {
        private List<MoviesForRent> movieInventory = new List<MoviesForRent>(); 

        public FrmMovieRentals()
        {
            InitializeComponent();
        }

        //Method to create the struct object list
        private void ReadInventory()
        {
            StreamReader reader;
            string newLine;

            try
            {
                MoviesForRent item = new MoviesForRent();
                reader = File.OpenText("Movies.dat");
                char[] delim = { ',' };

                while (!reader.EndOfStream)
                {
                    newLine = reader.ReadLine();
                    string[] tokens = newLine.Split(delim); 

                    //struct object properties
                    item.category = tokens[0];
                    item.title = tokens[1];
                    item.movieType = tokens[2]; 
                    item.movieCost = tokens[3];
                    item.availability = tokens[4];

                    movieInventory.Add(item); 
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            categoryBxLoad(); 
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            AutoValidate = AutoValidate.Disable; 
            this.Close();
        }

        private void btnCancelItem_Click(object sender, EventArgs e)
        {
            //GUI resets
            gboxMemNum.BackColor = default(Color);
            rBtnBluRay.BackColor = default(Color);
            rBtnDVD.BackColor = default(Color);

            //Combo Box resets
            cbBoxTitle.ResetText();
            cbBoxCategory.ResetText();
            rBtnUserControl.Checked = true;

            //Remove Item from shopping cart
            if (lsBoxShoppingCart.SelectedIndex > -1)
                lsBoxShoppingCart.Items.RemoveAt(lsBoxShoppingCart.SelectedIndex);
            else
                MessageBox.Show("Please select item to cancel.");
        }

        private void ckBxMember_CheckedChanged(object sender, EventArgs e)
        {
            gboxMemNum.Visible = true;

            if (ckBxMember.Checked)
            {
                ckBxMember.Enabled = false; 
            }
        }

        private void btnAddtoCart_Click(object sender, EventArgs e)
        {
            //Declarations
            string errorMessage;
            string memNum = txtMemNum.Text;
            string memName = txtMemName.Text; 
            string category = (string)cbBoxCategory.SelectedItem;
            string title = (string)cbBoxTitle.SelectedItem;
            string movieType = getMovieType();
            string movieCost = getRentalCost();
            string availability = getAvailability(); 
            string item = category + ","  + title + ","
                + movieType + "," + movieCost;

            try
            {
                if (ckBxMember.Checked)
                {
                    //After checkbox is checked, validation of textboxes 
                    //takes place
                    ValidateMemberNumber(memNum, out errorMessage);
                    //After validated, txt box no longer accessible
                    txtMemNum.Enabled = false;
                    //Validate member name
                    ValidateMemberName(memName, out errorMessage);
                    //member name no longer accessible
                    txtMemName.Enabled = false;
                }

                //User must have category selected
                if (cbBoxCategory.SelectedIndex > -1)
                {
                    //User must have title selected
                    if (cbBoxTitle.SelectedIndex > -1)
                    {
                        //Exceptions for user errors
                        if (rBtnUserControl.Checked)
                            throw new Exception("You must choose either BluRay or DVD.");
                        else if (availability != "Available")
                            throw new Exception("This title is currently unavailable.");
                        else
                            lsBoxShoppingCart.Items.Add(item);

                        //Prepares program for next entry
                        cbBoxCategory.ResetText();
                        cbBoxTitle.ResetText();
                        rBtnUserControl.Checked = true;
                        rBtnBluRay.BackColor = default(Color);
                        rBtnDVD.BackColor = default(Color);
                        gboxMemNum.BackColor = default(Color);
                    }
                    else
                    {
                        MessageBox.Show("Please choose a title.");
                        cbBoxTitle.BackColor = Color.Aqua;
                    }
                }
                else
                {
                    MessageBox.Show("Please choose a Category.");
                    cbBoxCategory.BackColor = Color.Aqua; 
                }
            }
            catch (Exception ex)
            {
                //Throws error message and highlights radio buttons
                MessageBox.Show(ex.Message);
                rBtnBluRay.BackColor = Color.Aqua;
                rBtnDVD.BackColor = Color.Aqua;
            }
        }

        private void btnCheckOut_Click(object sender, EventArgs e)
        {
            //Declarations
            double discountRate = 0.10;
            double subTotal = 0.0;
            double grandTotal = 0.0;
            double discount = 0.0; 
            string item;
            string title; 
            int countDVD = 0;
            int countBRay = 0;

            txtInvoiceSum.AppendText("Customer Invoice: \n");

            //lop to count the number of DVDs vs Blu-Rays
            for (int i = 0; i < lsBoxShoppingCart.Items.Count; i++)
            {
                lsBoxShoppingCart.SetSelected(i, true);
                item = lsBoxShoppingCart.SelectedItem.ToString();
                if (item.Contains("DVD"))
                    countDVD++;
                else if (item.Contains("Blu-Ray"))
                    countBRay++;
            }

            txtInvoiceSum.AppendText("DVD: " + " (" + countDVD + ") " + "\n");

            //loop to add each DVD title 
            for (int i = 0; i < lsBoxShoppingCart.Items.Count; i++)
            {
                lsBoxShoppingCart.SetSelected(i, true);
                item = lsBoxShoppingCart.SelectedItem.ToString();
                if (item.Contains("DVD"))
                {
                    title = item.Split(',')[1];
                    txtInvoiceSum.AppendText(title + "\n");
                }
            }

            txtInvoiceSum.AppendText("--------------------\n");
            txtInvoiceSum.AppendText("Blu-Ray: " + " (" + countBRay + ") " + "\n");

            //loop to add each of the Blu-Ray titles to the invoice
            for (int i = 0; i < lsBoxShoppingCart.Items.Count; i++)
            {
                lsBoxShoppingCart.SetSelected(i, true);
                item = lsBoxShoppingCart.SelectedItem.ToString();
                if (item.Contains("Blu-Ray"))
                {
                    title = item.Split(',')[1];
                    txtInvoiceSum.AppendText(title + "\n");
                }
            }

            //conditional statement gives the appropriate discount to members
            if (ckBxMember.Checked)
            {
                subTotal = (4.50 * countDVD) + (5.00 * countBRay);
                discount = subTotal * discountRate;
                grandTotal = subTotal - discount;
                txtInvoiceSum.AppendText("--------------------\n");
                txtInvoiceSum.AppendText("SubTotal: " + subTotal.ToString("f2") + "\n");
                txtInvoiceSum.AppendText("Discount: " + discount.ToString("f2") + "\n");
                txtInvoiceSum.AppendText("--------------------\n");
                txtInvoiceSum.AppendText("Grand Total: " + grandTotal.ToString("f2") + "\n");
            }
            //no discount for non-members
            else
            {
                subTotal = (4.50 * countDVD) + (5.00 * countBRay);
                grandTotal = subTotal - discount;
                txtInvoiceSum.AppendText("--------------------\n");
                txtInvoiceSum.AppendText("SubTotal: " + subTotal.ToString("f2") + "\n");
                txtInvoiceSum.AppendText("Discount: " + discount.ToString("f2") + "\n");
                txtInvoiceSum.AppendText("--------------------\n");
                txtInvoiceSum.AppendText("Grand Total: " + grandTotal.ToString("f2") + "\n");
            }

            //Calls my method to change the status of availability w/in the struct
            setAvailability();

            //Resets adjustments for boxes, etc. 
            ckBxMember.Checked = false;
            gboxMemNum.Visible = false;
            txtMemNum.Text = "";
            txtMemName.Text = ""; 
            cbBoxTitle.Items.Clear(); 
            cbBoxCategory.Items.Clear();
            cbBoxCategory.Enabled = false;
            cbBoxTitle.Enabled = false; 
            lsBoxShoppingCart.Visible = false; 
            txtInvoiceSum.Visible = true;
        }

        private void cbBoxCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            //reset GUI default color
            cbBoxCategory.BackColor = default(Color);

            cbBoxTitle.Items.Clear();

            //Puts titles into titlebox, available for selection
            //based upon category chosen
            titleBxLoad(); 
        }

        private void FrmMovieRentals_Load(object sender, EventArgs e)
        {
            //loads movie inventory as soon as the program form loads
            ReadInventory(); 
        }

        private void cbBoxTitle_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Resets GUI default color upon selection
            cbBoxTitle.BackColor = default(Color);
        }

        private void btnNewOrder_Click(object sender, EventArgs e)
        {
            //Reset check box
            ckBxMember.Enabled = true;
            ckBxMember.Checked = false;
            //Reset radio buttons
            rBtnUserControl.Checked = true;
            //turn Member credential box back on 
            gboxMemNum.Visible = false;
            //turn on cb boxes
            cbBoxCategory.Enabled = true;
            cbBoxTitle.Enabled = true; 
            //load the category box w/ content again
            categoryBxLoad(); 
            //reset text member credential text boxes
            txtMemNum.Text = "";
            txtMemNum.Enabled = true; 
            txtMemName.Text = "";
            txtMemName.Enabled = true; 
            //reset shopping cart and invoice summary
            lsBoxShoppingCart.Visible = true;
            lsBoxShoppingCart.Text = ""; 
            txtInvoiceSum.Visible = false;
            txtInvoiceSum.Text = "";
            //Turn on error messages
            memNameError.SetError(txtMemName, "Input Format: fmlastName@student.cccs.edu."); 
            memNumError.SetError(txtMemNum, "Input Format: S########.");

            //Resets GUI default color upon selection
            cbBoxTitle.BackColor = default(Color);
            cbBoxCategory.BackColor = default(Color);

            //Clear all items from lsBox
            lsBoxShoppingCart.Items.Clear(); 
        }

        private void txtMemNum_Validated(object sender, EventArgs e)
        {
            //Turn cbBoxes back on 
            cbBoxCategory.Enabled = true;
            cbBoxTitle.Enabled = true;
            //Reset background color on txt box
            txtMemNum.BackColor = default(Color); 
            //turn off error icon 
            memNumError.SetError(txtMemNum, "");
        }

        private void txtMemNum_Validating(object sender, CancelEventArgs e)
        {
            string errorMessage;
            if (!ValidateMemberNumber(txtMemNum.Text, out errorMessage))
            {
                //Cancel the event and select the text to be corrected 
                //by the user. 
                e.Cancel = true;
                txtMemNum.BackColor = Color.Aqua;
                cbBoxCategory.Enabled = false;
                cbBoxTitle.Enabled = false;
                //Set the ErrorProvider error w/ the text to display
                memNumError.SetError(txtMemNum, errorMessage);
            }
        }

        private void txtMemName_Validated(object sender, EventArgs e)
        {
            //Turn cbBoxes back on 
            cbBoxCategory.Enabled = true;
            cbBoxTitle.Enabled = true;
            //Reset Background color
            txtMemName.BackColor = default(Color);
            //Get rid of error icon 
            memNameError.SetError(txtMemName, "");
        }

        private void txtMemName_Validating(object sender, CancelEventArgs e)
        {
            string errorMessage;
            if (!ValidateMemberName(txtMemName.Text, out errorMessage))
            {
                //Cancel the event and select the text to be corrected 
                //by the user. 
                e.Cancel = true;
                txtMemName.BackColor = Color.Aqua;
                cbBoxCategory.Enabled = false;
                cbBoxTitle.Enabled = false; 

                //Set the ErrorProvider error w/ the text to display
                memNameError.SetError(txtMemName, errorMessage);
            }
        }

        //Methods
        private void categoryBxLoad()
        {
            foreach(MoviesForRent item in movieInventory) 
            {
                //Prevents duplicates
                if (!cbBoxCategory.Items.Contains(item.category))
                    cbBoxCategory.Items.Add(item.category);
            }
        }

        //this method loads the title drop down box
        private void titleBxLoad()
        {
            string selectedCat;

            foreach (MoviesForRent item in movieInventory)
            {
                selectedCat = (string)cbBoxCategory.SelectedItem;
                //Prevents duplicates
                if (item.category == selectedCat && 
                        !cbBoxTitle.Items.Contains(item.title))
                    cbBoxTitle.Items.Add(item.title);
            }
        }

        //this method merely returns the movie type DVD/Blu-Ray
        private string getMovieType()
        {
            string selectedCat = (string)cbBoxCategory.SelectedItem;
            string selectedTitle = (string)cbBoxTitle.SelectedItem; 
            string movieType = " "; 

            foreach (MoviesForRent item in movieInventory)
            {
                if (item.category == selectedCat && item.title == selectedTitle
                    && rBtnDVD.Checked && item.movieType == "DVD")
                    movieType = item.movieType; 
                else if (item.category == selectedCat && item.title == selectedTitle
                    && rBtnBluRay.Checked && item.movieType == "Blu-Ray")
                    movieType = item.movieType;
            }
            return movieType; 
        }

        //method returns the price of the movie rental
        private string getRentalCost()
        {
            string selectedCat = (string)cbBoxCategory.SelectedItem;
            string selectedTitle = (string)cbBoxTitle.SelectedItem;
            string cost = " ";

            foreach (MoviesForRent item in movieInventory)
            {
                if (item.category == selectedCat && item.title == selectedTitle
                    && rBtnDVD.Checked && item.movieType == "DVD")
                    cost = item.movieCost;
                else if (item.category == selectedCat && item.title == selectedTitle
                    && rBtnBluRay.Checked && item.movieType == "Blu-Ray")
                    cost = item.movieCost;
            }
            return cost;
        }

        //method returns the status of rental availability
        public string getAvailability()
        {
            string selectedCat = (string)cbBoxCategory.SelectedItem;
            string selectedTitle = (string)cbBoxTitle.SelectedItem;
            string availability = " ";

            foreach (MoviesForRent item in movieInventory)
            {
                if (item.category == selectedCat && item.title == selectedTitle
                    && rBtnDVD.Checked && item.movieType == "DVD")
                    availability = item.availability;
                else if (item.category == selectedCat && item.title == selectedTitle
                    && rBtnBluRay.Checked && item.movieType == "Blu-Ray")
                    availability = item.availability;
            }
            return availability;
        }

        //method changes the status of availability
        public void setAvailability()
        {
            MoviesForRent item = new MoviesForRent();
            string shoppingCartItem;
            string availChange = "CheckedOut";
            char[] delim = { ',' };

            for (int i = 0; i < lsBoxShoppingCart.Items.Count; i++)
            {
                lsBoxShoppingCart.SetSelected(i, true);
                shoppingCartItem = lsBoxShoppingCart.SelectedItem.ToString();
                string[] tokens = shoppingCartItem.Split(delim);
                item.category = tokens[0];
                item.title = tokens[1];
                item.movieType = tokens[2];
                item.movieCost = tokens[3];

                for (int j = 0; j < movieInventory.Count; j++)
                {
                    if (movieInventory.Contains(item))
                        movieInventory.RemoveAt(j);
                }

                item.availability = availChange;

                movieInventory.Add(item); 
            }
        }

        private bool ValidateMemberNumber(string number, out string errorMessage)
        {
            bool valid = true;

            if (!number.StartsWith("S"))
                valid = false;
            else if (number.Contains(" "))
                valid = false;
            else if (number.Equals(""))
                valid = false;
            else if (number.Count() < 9)
                valid = false; 

            for (int i = 1; i < number.Count(); i++)
            {
                if (!Char.IsDigit(number[i]))
                    valid = false;
            }

            errorMessage = "Input Format: S########.";
            return valid;
        }

        private bool ValidateMemberName(string name, out string errorMessage)
        {
            bool valid = true;
            int index = name.IndexOf("@");
            string subStrEmail = "@student.cccs.edu";
            string vldSubStrEmail = "";

            if (name.Equals("")) 
                valid = false;
            else if (!name.Contains("@"))
                valid = false;
            else if (name.StartsWith("@"))
                valid = false;
            else if (name.Contains(" "))
                valid = false; 
            else
                vldSubStrEmail = name.Substring(index);

            if (subStrEmail != vldSubStrEmail)
                valid = false;

            errorMessage = "Input Format: fmlastName@student.cccs.edu.";
            return valid;
        }

        //This code works on the first click of the X but it requires 
        //that you override WndProc, found the suggestion on StackOverflow
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x10) // The upper right "X" was clicked
            {
                AutoValidate = AutoValidate.Disable; 
            }
            base.WndProc(ref m);
        }
    }
}

