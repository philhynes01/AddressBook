using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Xml;

namespace AddressBook
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

        }
        //Create the people List
        List<Person> people = new List<Person>();

        private void Form1_Load(object sender, EventArgs e)
        {
            //Checks to see if the file and directory exists and creates the xml file
            string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            if (!Directory.Exists(path + "\\Address Book"))
                Directory.CreateDirectory(path + "\\Address Book");
            if (!File.Exists(path + "\\Address Book\\settings.xml"))
            {
                //Writes the start element
                XmlTextWriter xW = new XmlTextWriter(path + "\\Address Book\\settings.xml", Encoding.UTF8);
                xW.WriteStartElement("People");
                xW.WriteEndElement();
                xW.Close();
            }
            //Loads the contents of the xml file into the program
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(path + "\\Address Book\\settings.xml");
            foreach (XmlNode xNode in xDoc.SelectNodes("People/Person"))
            {
                Person p = new Person();
                p.Name = xNode.SelectSingleNode("Name").InnerText;
                p.Email = xNode.SelectSingleNode("Email").InnerText;
                p.StreetAddress = xNode.SelectSingleNode("Address").InnerText;
                p.AdditionalNotes = xNode.SelectSingleNode("Notes").InnerText;
                p.Birthday = DateTime.FromFileTime(Convert.ToInt64(xNode.SelectSingleNode("Birthday").InnerText));
                people.Add(p);
                listView1.Items.Add(p.Name);
            }
        }
        //Clear the text boxes
        void ClearFields()
        {
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
            dateTimePicker1.Value = DateTime.Now;
        }
        //Add a person to the list
        private void button2_Click(object sender, EventArgs e)
        {
            Person p = new Person();
            p.Name = textBox1.Text;
            p.StreetAddress = textBox3.Text;
            p.Email = textBox2.Text;
            p.Birthday = dateTimePicker1.Value;
            p.AdditionalNotes = textBox4.Text;
            people.Add(p);
            listView1.Items.Add(p.Name);
            ClearFields();
            notifyIcon1.ShowBalloonTip(1000, "Contact Added!", p.Name + " has been added to the contact list!", ToolTipIcon.Info);
        }
        //Populates the text boxes when a user selected a person from the list control
        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (listView1.SelectedItems.Count == 0) return;
            textBox1.Text = people[listView1.SelectedItems[0].Index].Name;
            textBox2.Text = people[listView1.SelectedItems[0].Index].Email;
            textBox3.Text = people[listView1.SelectedItems[0].Index].StreetAddress;
            textBox4.Text = people[listView1.SelectedItems[0].Index].AdditionalNotes;
            dateTimePicker1.Value = people[listView1.SelectedItems[0].Index].Birthday;
        }
        //Removes the selected person from the list and clears the text boxes
        private void button3_Click(object sender, EventArgs e)
        {
            notifyIcon1.ShowBalloonTip(1000, "Contact Removed!", "The contact " + people[listView1.SelectedItems[0].Index].Name + " has been removed", ToolTipIcon.Info);
            DialogResult result = MessageBox.Show("Are you sure you want to remove " + people[listView1.SelectedItems[0].Index].Name + "?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
            if (result == DialogResult.Yes)
            {
                Remove();
                ClearFields();
                
            }
        }
        //Method to remove people from the list
        void Remove()
        {
            try
            {
                listView1.Items.Remove(listView1.SelectedItems[0]);
                people.RemoveAt(listView1.SelectedItems[0].Index);
            }
            catch { }
        }

        //Person Class
        class Person
        {
            public string Name { get; set; }
            public string Email { get; set; }
            public string StreetAddress { get; set; }
            public string AdditionalNotes { get; set; }
            public DateTime Birthday { get; set; }

        }

        //Removes the selected person from the list and clears the text boxes 
        private void removeToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure you want to remove " + people[listView1.SelectedItems[0].Index].Name + "?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
            if (result == DialogResult.Yes)
            {
                Remove();
                ClearFields();
            }
        }

        //Saves any changes
        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Save Changes?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
            if (result == DialogResult.Yes)
            {
                try
                {
                    people[listView1.SelectedItems[0].Index].Name = textBox1.Text;
                    people[listView1.SelectedItems[0].Index].Email = textBox2.Text;
                    people[listView1.SelectedItems[0].Index].StreetAddress = textBox3.Text;
                    people[listView1.SelectedItems[0].Index].AdditionalNotes = textBox4.Text;
                    people[listView1.SelectedItems[0].Index].Birthday = dateTimePicker1.Value;
                    listView1.SelectedItems[0].Text = textBox1.Text;
                    ClearFields();
                }
                catch { }
            }
        }

        //Writes to the xml file
        void WriteToXml()
        {
            XmlDocument xDoc = new XmlDocument();
            string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            xDoc.Load(path + "\\Address Book\\settings.xml");

            XmlNode xNode = xDoc.SelectSingleNode("People");
            xNode.RemoveAll();

            foreach (Person p in people)
            {
                XmlNode xTop = xDoc.CreateElement("Person");
                XmlNode xName = xDoc.CreateElement("Name");
                XmlNode xEmail = xDoc.CreateElement("Email");
                XmlNode xAddress = xDoc.CreateElement("Address");
                XmlNode xNotes = xDoc.CreateElement("Notes");
                XmlNode xBirthday = xDoc.CreateElement("Birthday");

                xName.InnerText = p.Name;
                xEmail.InnerText = p.Email;
                xAddress.InnerText = p.StreetAddress;
                xNotes.InnerText = p.AdditionalNotes;
                xBirthday.InnerText = p.Birthday.ToFileTime().ToString();

                xTop.AppendChild(xName);
                xTop.AppendChild(xEmail);
                xTop.AppendChild(xAddress);
                xTop.AppendChild(xNotes);
                xTop.AppendChild(xBirthday);

                xDoc.DocumentElement.AppendChild(xTop);


            }
            xDoc.Save(path + "\\Address Book\\settings.xml");


        }

        //Clears the xml file
        void ClearData()
        {
            try
            {
                XmlDocument xDoc = new XmlDocument();
                string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                xDoc.Load(path + "\\Address Book\\settings.xml");

                XmlNode xNode = xDoc.SelectSingleNode("People");
                xNode.RemoveAll();

                System.IO.File.Delete(path + "\\Address Book\\settings.xml");
            }
            catch { }
        }


        bool ClearAll = false;

        //Depending on the bool ClearAll either writes the data to the xml or deletes it
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure you Exit?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
            if (result == DialogResult.Yes)
            {
                if (ClearAll == false)
                {
                    WriteToXml();
                }
                else
                {
                    ClearData();
                }
            }
            else
            {
                e.Cancel = true;
                this.Activate();
            }
            
        }

        //Closes the app
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        //Removes all entries from the app and xml file
        private void button4_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure you want to remove all entries?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
            if (result == DialogResult.Yes)
            {
                ClearAll = true;
                ClearFields();
                ClearData();
                listView1.Clear();
            }
            
        }
    }
}
