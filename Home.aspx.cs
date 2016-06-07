using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using IncomeTaxCalc.Web.Models;
using System.Xml;
using System.Configuration;
using System.Data.SqlClient;
using System.Text;

namespace IncomeTaxCalc.Web
{
    public partial class Home : System.Web.UI.Page
    {
        PersonDetailModel personModel = new PersonDetailModel();
        IncomeTaxDetailModel incomeModel = new IncomeTaxDetailModel();

        string connectionString = ConfigurationManager.ConnectionStrings["IncomeTaxConnectionString"].ConnectionString;
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void ButtonUploadXml_Click(object sender, EventArgs e)
        {
            if (FileUploadITR.HasFile)
            {
                try
                {
                    string fileExtension = Path.GetExtension(FileUploadITR.PostedFile.FileName);

                    if (fileExtension == ".xml")
                    {
                        XmlDocument xmlDoc = new XmlDocument();
                        xmlDoc.Load(FileUploadITR.PostedFile.FileName);

                        // Read person deails
                        XmlNodeList personList = xmlDoc.DocumentElement.SelectNodes("/IncomeTax/PersonDetail");
                        
                        foreach (XmlNode personDetail in personList)
                        {
                            personModel.PANno = personDetail.SelectSingleNode("PANno").InnerText;
                            personModel.Name = personDetail.SelectSingleNode("Name").InnerText;
                            personModel.Address = personDetail.SelectSingleNode("Address").InnerText;
                            personModel.MobileNo = personDetail.SelectSingleNode("Mobileno").InnerText;                            
                        }

                        //Upload xml details to DB

                        bool inserted = UploadPersonDetails(personModel);

                        // Read Income details
                        XmlNodeList incomeList = xmlDoc.DocumentElement.SelectNodes("/IncomeTax/IncomeDetail");

                        foreach (XmlNode incomeDetail in incomeList)
                        {
                            if (personModel != null)
                            {
                                incomeModel.PANno = personModel.PANno;
                            }
                            
                            incomeModel.AssessmentYear = incomeDetail.SelectSingleNode("AssessmentYear").InnerText;
                            incomeModel.IncomeFromSalary = incomeDetail.SelectSingleNode("IncomeFromSalary").InnerText;
                            incomeModel.IncomeFromOtherSource = incomeDetail.SelectSingleNode("IncomeFromOtherSource").InnerText;
                            incomeModel._80C = incomeDetail.SelectSingleNode("_80C").InnerText;
                        }

                        bool insertedIncome = UploadIncomeDetails(incomeModel);
                    }
                }
                catch (Exception ex)
                {                    
                    throw;
                } 
            }
        }

        private bool UploadPersonDetails(PersonDetailModel personModel)
        {
            bool isInserted = false;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                StringBuilder commandText = new StringBuilder();
                
                commandText.AppendFormat(string.Format("Insert into PersonalDetail(PANno, Name, AddressInfo, MobileNo) Values ('{0}', '{1}', '{2}', '{3}')", personModel.PANno, personModel.Name, personModel.Address, personModel.MobileNo));
                
                SqlCommand command = new SqlCommand(commandText.ToString(), con);
                command.ExecuteNonQuery();          
                isInserted = true;
            }
            return isInserted;
        }

        private bool UploadIncomeDetails(IncomeTaxDetailModel incomeModel)
        {
            bool isInserted = false;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                StringBuilder commandText = new StringBuilder();

                commandText.AppendFormat(string.Format("Insert into IncomeDetail(PANno, IncomeFromSalary, IncomeFromOtherSource, AssessmentYear, [80C]) Values ('{0}', '{1}', '{2}', '{3}', '{4}')", personModel.PANno, incomeModel.IncomeFromSalary, incomeModel.IncomeFromOtherSource, incomeModel.AssessmentYear, incomeModel._80C));

                SqlCommand command = new SqlCommand(commandText.ToString(), con);
                command.ExecuteNonQuery();
                isInserted = true;
            }
            return isInserted;
        }

        protected void ButtonGetIncomeDeduction_Click(object sender, EventArgs e)
        {
            string panno = TextBoxPANno.Text;
            string assessmentYear = TextBoxAssessmentYear.Text;

            IncomeTaxDetailModel incomeDetail = GetIncomeDetail(panno, assessmentYear);

            LabelTotalIncome.Text =Math.Round((Convert.ToDecimal(incomeDetail.IncomeFromSalary) + Convert.ToDecimal(incomeDetail.IncomeFromOtherSource)), 0).ToString();
            LabelDeduction.Text = Math.Round(Convert.ToDecimal(incomeDetail._80C), 0).ToString();

            LabelTaxableIncome.Text = Math.Round((Convert.ToDecimal(LabelTotalIncome.Text) - Convert.ToDecimal(LabelDeduction.Text)), 0).ToString();
            long taxableIncome =Convert.ToInt64(Math.Round(Convert.ToDecimal(LabelTaxableIncome.Text), 0));
            if (taxableIncome <= 200000)
            {
                LabelIncomeTax.Text = taxableIncome.ToString();
                LabelEducationCess.Text = 0.ToString();
            }
            else if (taxableIncome > 200000 && taxableIncome <= 500000)
            {
                long taxable = taxableIncome - 200000;
                LabelIncomeTax.Text = (taxable * 0.1).ToString();
                LabelEducationCess.Text = (taxable * 0.02).ToString();
                LabelTotalTaxPayment.Text = (Convert.ToInt64(LabelIncomeTax.Text) + Convert.ToInt64(LabelEducationCess.Text)).ToString();
            }
            else if (taxableIncome > 500000 && taxableIncome <= 1000000)
            {
                long taxable = taxableIncome - 200000;
                LabelIncomeTax.Text = (taxable * 0.2).ToString();
                LabelEducationCess.Text = (taxable * 0.02).ToString();
                LabelTotalTaxPayment.Text = (Convert.ToInt64(LabelIncomeTax.Text) + Convert.ToInt64(LabelEducationCess.Text)).ToString();
            }
            else
            {
                long taxable = taxableIncome - 200000;
                LabelIncomeTax.Text = (taxable * 0.3).ToString();
                LabelEducationCess.Text = (taxable * 0.02).ToString();
                LabelTotalTaxPayment.Text = (Convert.ToInt64(LabelIncomeTax.Text) + Convert.ToInt64(LabelEducationCess.Text)).ToString();
            }
        }

        private IncomeTaxDetailModel GetIncomeDetail(string panno, string assessmentYear)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                StringBuilder commandText = new StringBuilder();

                commandText.AppendFormat(string.Format("Select * from IncomeDetail where PANno ='{0}' and AssessmentYear = '{1}'", panno, assessmentYear));

                SqlCommand command = new SqlCommand(commandText.ToString(), con);
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    // get the results of each column
                    incomeModel.IncomeFromSalary = reader["IncomeFromSalary"].ToString();
                    incomeModel.IncomeFromOtherSource = reader["IncomeFromOtherSource"].ToString();
                    incomeModel._80C = reader["80C"].ToString();   
                }
            }
            return incomeModel;
        }
    }
}