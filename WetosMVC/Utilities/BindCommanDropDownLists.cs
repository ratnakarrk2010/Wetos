using System;
using System.Linq;
using System.Web.UI.WebControls;
using System.Collections.Generic;

namespace WetosMVCMainApp.Utilities
{
    public class BindCommanDropDownLists
    {
        #region Bind Company,Branch,Department and Designation
        public void BindDropDown(DropDownList ddlCompanyName, DropDownList ddlBranchName, DropDownList ddlDepartmentName,DropDownList ddlDesignationName)
        {
            try
            {
                List<DataEntity.Company> lstCompany = new List<WETOS.DataEntity.Company>();
                DataEntity.Company AddBlankCompany = new WETOS.DataEntity.Company();

                AddBlankCompany.CompanyId = ClientConstants.Negation;
                AddBlankCompany.CompanyName = ClientConstants.SELECT;

                lstCompany.Add(AddBlankCompany);
                lstCompany.AddRange(WETOSSession.GetAllCompany);

                ddlCompanyName.DataValueField = ClientConstants.CompanyId;
                ddlCompanyName.DataTextField = ClientConstants.CompanyName;
                ddlCompanyName.DataSource = lstCompany;
                ddlCompanyName.DataBind();


                List<DataEntity.Branch> lstBranch = new List<WETOS.DataEntity.Branch>();
                DataEntity.Branch AddBlankBranch = new WETOS.DataEntity.Branch();

                AddBlankBranch.BranchId = ClientConstants.Negation;
                AddBlankBranch.BranchName = ClientConstants.SELECT;

                lstBranch.Add(AddBlankBranch);
                lstBranch.AddRange(WETOSSession.GetAllBranch);

                ddlBranchName.DataValueField = ClientConstants.BranchId;
                ddlBranchName.DataTextField = ClientConstants.BranchName;
                ddlBranchName.DataSource = lstBranch;
                ddlBranchName.DataBind();


                List<DataEntity.Department> lstDepartment = new List<WETOS.DataEntity.Department>();
                DataEntity.Department AddBlankDepartment = new WETOS.DataEntity.Department();

                AddBlankDepartment.DepartmentId = ClientConstants.Negation;
                AddBlankDepartment.DepartmentName = ClientConstants.SELECT;

                lstDepartment.Add(AddBlankDepartment);
                lstDepartment.AddRange(WETOSSession.GetAllDepartment);

                ddlDepartmentName.DataValueField = ClientConstants.DepartmentId;
                ddlDepartmentName.DataTextField = ClientConstants.DepartmentName;
                ddlDepartmentName.DataSource = lstDepartment;
                ddlDepartmentName.DataBind();


                List<DataEntity.Designation> lstDesignation = new List<WETOS.DataEntity.Designation>();
                DataEntity.Designation AddBlankDesignation = new WETOS.DataEntity.Designation();

                AddBlankDesignation.DesignationId = ClientConstants.Negation;
                AddBlankDesignation.DesignationName = ClientConstants.SELECT;

                lstDesignation.Add(AddBlankDesignation);
                lstDesignation.AddRange(WETOSSession.GetAllDesignation);

                ddlDesignationName.DataValueField = ClientConstants.DesignationId;
                ddlDesignationName.DataTextField = ClientConstants.DesignationName;
                ddlDesignationName.DataSource = lstDesignation;
                ddlDesignationName.DataBind();
            }
            catch
            {
                throw;
            }
            

        }


        public void BindDropDownTemp(DropDownList ddlCompanyName, DropDownList ddlBranchName, DropDownList ddlDepartmentName, DropDownList ddlDesignationName)
        {
            try
            {
                List<DataEntity.Company> lstCompany = new List<WETOS.DataEntity.Company>();
                DataEntity.Company AddBlankCompany = new WETOS.DataEntity.Company();

                AddBlankCompany.CompanyId = ClientConstants.Negation;
                AddBlankCompany.CompanyName = ClientConstants.SELECT;

                lstCompany.Add(AddBlankCompany);
                lstCompany.AddRange(WETOSSession.TempGetAllCompany);

                ddlCompanyName.DataValueField = ClientConstants.CompanyId;
                ddlCompanyName.DataTextField = ClientConstants.CompanyName;
                ddlCompanyName.DataSource = lstCompany;
                ddlCompanyName.DataBind();


                List<DataEntity.Branch> lstBranch = new List<WETOS.DataEntity.Branch>();
                DataEntity.Branch AddBlankBranch = new WETOS.DataEntity.Branch();

                AddBlankBranch.BranchId = ClientConstants.Negation;
                AddBlankBranch.BranchName = ClientConstants.SELECT;

                lstBranch.Add(AddBlankBranch);
                lstBranch.AddRange(WETOSSession.TempGetAllBranch);

                ddlBranchName.DataValueField = ClientConstants.BranchId;
                ddlBranchName.DataTextField = ClientConstants.BranchName;
                ddlBranchName.DataSource = lstBranch;
                ddlBranchName.DataBind();


                List<DataEntity.Department> lstDepartment = new List<WETOS.DataEntity.Department>();
                DataEntity.Department AddBlankDepartment = new WETOS.DataEntity.Department();

                AddBlankDepartment.DepartmentId = ClientConstants.Negation;
                AddBlankDepartment.DepartmentName = ClientConstants.SELECT;

                lstDepartment.Add(AddBlankDepartment);
                lstDepartment.AddRange(WETOSSession.TempGetAllDepartment);

                ddlDepartmentName.DataValueField = ClientConstants.DepartmentId;
                ddlDepartmentName.DataTextField = ClientConstants.DepartmentName;
                ddlDepartmentName.DataSource = lstDepartment;
                ddlDepartmentName.DataBind();


                List<DataEntity.Designation> lstDesignation = new List<WETOS.DataEntity.Designation>();
                DataEntity.Designation AddBlankDesignation = new WETOS.DataEntity.Designation();

                AddBlankDesignation.DesignationId = ClientConstants.Negation;
                AddBlankDesignation.DesignationName = ClientConstants.SELECT;

                lstDesignation.Add(AddBlankDesignation);
                lstDesignation.AddRange(WETOSSession.TempGetAllDesignation );

                ddlDesignationName.DataValueField = ClientConstants.DesignationId;
                ddlDesignationName.DataTextField = ClientConstants.DesignationName;
                ddlDesignationName.DataSource = lstDesignation;
                ddlDesignationName.DataBind();
            }
            catch
            {
                throw;
            }


        }
        #endregion        

        #region Bind Company,Branch and Department
        public void BindDropDown(DropDownList ddlCompanyName, DropDownList ddlBranchName, DropDownList ddlParentDepartment)
        {
            try
            {
                List<DataEntity.Company> lstCompany = new List<WETOS.DataEntity.Company>();
                DataEntity.Company AddBlankCompany = new WETOS.DataEntity.Company();

                AddBlankCompany.CompanyId = ClientConstants.Negation;
                AddBlankCompany.CompanyName = ClientConstants.SELECT;

                lstCompany.Add(AddBlankCompany);
                lstCompany.AddRange(WETOSSession.GetAllCompany);

                ddlCompanyName.DataValueField = ClientConstants.CompanyId;
                ddlCompanyName.DataTextField = ClientConstants.CompanyName;
                ddlCompanyName.DataSource = lstCompany;
                ddlCompanyName.DataBind();


                List<DataEntity.Branch> lstBranch = new List<WETOS.DataEntity.Branch>();
                DataEntity.Branch AddBlankBranch = new WETOS.DataEntity.Branch();

                AddBlankBranch.BranchId = ClientConstants.Negation;
                AddBlankBranch.BranchName = ClientConstants.SELECT;

                lstBranch.Add(AddBlankBranch);
                lstBranch.AddRange(WETOSSession.GetAllBranch);

                ddlBranchName.DataValueField = ClientConstants.BranchId;
                ddlBranchName.DataTextField = ClientConstants.BranchName;
                ddlBranchName.DataSource = lstBranch;
                ddlBranchName.DataBind();


                List<DataEntity.Department> lstDepartment = new List<WETOS.DataEntity.Department>();
                DataEntity.Department AddBlankDepartment = new WETOS.DataEntity.Department();

                AddBlankDepartment.DepartmentId = ClientConstants.Negation;
                AddBlankDepartment.DepartmentName = ClientConstants.SELECT;

                lstDepartment.Add(AddBlankDepartment);
                lstDepartment.AddRange(WETOSSession.GetAllDepartment);

                ddlParentDepartment.DataValueField = ClientConstants.DepartmentId;
                ddlParentDepartment.DataTextField = ClientConstants.DepartmentName;
                ddlParentDepartment.DataSource = lstDepartment;
                ddlParentDepartment.DataBind();
            }
            catch
            {
                throw;
            }
            


        }
        #endregion        

        #region Bind Company and Branch 
        public void BindDropDown(DropDownList ddlCompanyName, DropDownList ddlBranchName)
        {
            try
            {
                List<DataEntity.Company> lstCompany = new List<WETOS.DataEntity.Company>();
                DataEntity.Company AddBlankCompany = new WETOS.DataEntity.Company();

                AddBlankCompany.CompanyId = ClientConstants.Negation;
                AddBlankCompany.CompanyName = ClientConstants.SELECT;

                lstCompany.Add(AddBlankCompany);
                lstCompany.AddRange(WETOSSession.GetAllCompany);

                ddlCompanyName.DataValueField = ClientConstants.CompanyId;
                ddlCompanyName.DataTextField = ClientConstants.CompanyName;
                ddlCompanyName.DataSource = lstCompany;
                ddlCompanyName.DataBind();

                List<DataEntity.Branch> lstBranch = new List<WETOS.DataEntity.Branch>();
                DataEntity.Branch AddBlankBranch = new WETOS.DataEntity.Branch();

                AddBlankBranch.BranchId = ClientConstants.Negation;
                AddBlankBranch.BranchName = ClientConstants.SELECT;

                lstBranch.Add(AddBlankBranch);
                lstBranch.AddRange(WETOSSession.GetAllBranch);

                ddlBranchName.DataValueField = ClientConstants.BranchId;
                ddlBranchName.DataTextField = ClientConstants.BranchName;
                ddlBranchName.DataSource = lstBranch;
                ddlBranchName.DataBind();

            }
            catch
            {
                throw;
            }


        }
        #endregion        

        #region Bind Company 
        public void BindDropDownCompany(DropDownList ddlCompanyName)
        {
            try
            {
                List<DataEntity.Company> lstCompany = new List<WETOS.DataEntity.Company>();
                DataEntity.Company AddBlankCompany = new WETOS.DataEntity.Company();

                AddBlankCompany.CompanyId = ClientConstants.Negation;
                AddBlankCompany.CompanyName = ClientConstants.SELECT;

                lstCompany.Add(AddBlankCompany);
                lstCompany.AddRange(WETOSSession.GetAllCompany);

                ddlCompanyName.DataValueField = ClientConstants.CompanyId;
                ddlCompanyName.DataTextField = ClientConstants.CompanyName;
                ddlCompanyName.DataSource = lstCompany;
                ddlCompanyName.DataBind();
            }
            catch
            {
                throw;
            }


        }
        public void BindDropDownCompany(DropDownList ddlCompanyName, List<WETOS.DataEntity.Company> lstCompanyParam)
        {
            try
            {
                List<DataEntity.Company> lstCompany = new List<WETOS.DataEntity.Company>();
                DataEntity.Company AddBlankCompany = new WETOS.DataEntity.Company();

                AddBlankCompany.CompanyId = ClientConstants.Negation;
                AddBlankCompany.CompanyName = ClientConstants.SELECT;

                lstCompany.Add(AddBlankCompany);
                lstCompany.AddRange(lstCompanyParam);

                ddlCompanyName.DataValueField = ClientConstants.CompanyId;
                ddlCompanyName.DataTextField = ClientConstants.CompanyName;
                ddlCompanyName.DataSource = lstCompany;
                ddlCompanyName.DataBind();
            }
            catch
            {
                throw;
            }


        }
        #endregion        

        #region Bind Branch
        public void BindDropDownBranch(DropDownList ddlBranchName)
        {
            try
            {
                List<DataEntity.Branch> lstBranch = new List<WETOS.DataEntity.Branch>();
                DataEntity.Branch AddBlankBranch = new WETOS.DataEntity.Branch();
                DataEntity.Branch AddAllBranch = new WETOS.DataEntity.Branch();

                AddBlankBranch.BranchId = ClientConstants.Negation;
                AddBlankBranch.BranchName = ClientConstants.SELECT;
                lstBranch.Add(AddBlankBranch);
                //AddAllBranch.BranchId = ClientConstants.IntDefaultId;
                //AddAllBranch.BranchName = ClientConstants.SELECT;
                //lstBranch.Add(AddAllBranch);
                
                lstBranch.AddRange(WETOSSession.GetAllBranch); 
                ddlBranchName.DataValueField = ClientConstants.BranchId;
                ddlBranchName.DataTextField = ClientConstants.BranchName;
                ddlBranchName.DataSource = lstBranch;
                ddlBranchName.DataBind();

            }
            catch
            {
                throw;
            }


        }
        public void BindDropDownDivision(DropDownList ddlDivision)
        {
            List<DataEntity.Division> lstDivision = new List<WETOS.DataEntity.Division>();
            DataEntity.Division AddBlankDivision = new WETOS.DataEntity.Division();

            AddBlankDivision.DivisionId = ClientConstants.Negation;
            AddBlankDivision.DivisionName = ClientConstants.ALL;

            lstDivision.Add(AddBlankDivision);
           
            lstDivision.AddRange(WETOSSession.GetAllDivision);
            ddlDivision.DataValueField = ClientConstants.DivisionId;
            ddlDivision.DataTextField = ClientConstants.DivisionName;
            ddlDivision.DataSource = lstDivision;
            ddlDivision.DataBind();
        }

        public void BindDropDownDivisionnew(DropDownList ddlDivision)
        {
            List<DataEntity.Division> lstDivision = new List<WETOS.DataEntity.Division>();
            DataEntity.Division AddBlankDivision = new WETOS.DataEntity.Division();

            AddBlankDivision.DivisionId = ClientConstants.Negation;
            AddBlankDivision.DivisionName = ClientConstants.SELECT;

            lstDivision.Add(AddBlankDivision);

            lstDivision.AddRange(WETOSSession.GetAllDivision);
            ddlDivision.DataValueField = ClientConstants.DivisionId;
            ddlDivision.DataTextField = ClientConstants.DivisionName;
            ddlDivision.DataSource = lstDivision;
            ddlDivision.DataBind();
        }
        public void BindDropDownDivision(DropDownList ddlDivision, List<WETOS.DataEntity.Division> lsttempDivision)
        {
            List<DataEntity.Division> lstDivision = new List<WETOS.DataEntity.Division>();
            DataEntity.Division AddBlankDivision = new WETOS.DataEntity.Division();

            AddBlankDivision.DivisionId = ClientConstants.Negation;
            AddBlankDivision.DivisionName = ClientConstants.SELECT;

            lstDivision.Add(AddBlankDivision);

            lstDivision.AddRange(lsttempDivision);
            ddlDivision.DataValueField = ClientConstants.DivisionId;
            ddlDivision.DataTextField = ClientConstants.DivisionName;
            ddlDivision.DataSource = lstDivision;
            ddlDivision.DataBind();
        }
        public void BindDropDownBranch(DropDownList ddlBranchName,List<WETOS.DataEntity.Branch> lstBranchParam)
        {
            try
            {
                List<DataEntity.Branch> lstBranch = new List<WETOS.DataEntity.Branch>();
                DataEntity.Branch AddBlankBranch = new WETOS.DataEntity.Branch();

                AddBlankBranch.BranchId = ClientConstants.Negation;
                AddBlankBranch.BranchName = ClientConstants.SELECT;

                lstBranch.Add(AddBlankBranch);
                lstBranch.AddRange(lstBranchParam);

                ddlBranchName.DataValueField = ClientConstants.BranchId;
                ddlBranchName.DataTextField = ClientConstants.BranchName;
                ddlBranchName.DataSource = lstBranch;
                ddlBranchName.DataBind();

            }
            catch
            {
                throw;
            }
        }
        public void BindDropDownAllBranch(DropDownList ddlBranchName, List<WETOS.DataEntity.Branch> lstBranchParam)
        {
            try
            {
                List<DataEntity.Branch> lstBranch = new List<WETOS.DataEntity.Branch>();
                DataEntity.Branch AddBlankBranch = new WETOS.DataEntity.Branch();
                DataEntity.Branch AddAllBranch = new WETOS.DataEntity.Branch();

                AddBlankBranch.BranchId = ClientConstants.Negation;
                AddBlankBranch.BranchName = ClientConstants.SELECT;

                AddAllBranch.BranchId = ClientConstants.IntDefaultId;
                AddAllBranch.BranchName = ClientConstants.ALL;

                lstBranch.Add(AddBlankBranch);
                lstBranch.Add(AddAllBranch);
                lstBranch.AddRange(lstBranchParam);

                ddlBranchName.DataValueField = ClientConstants.BranchId;
                ddlBranchName.DataTextField = ClientConstants.BranchName;
                ddlBranchName.DataSource = lstBranch;
                ddlBranchName.DataBind();

            }
            catch
            {
                throw;
            }
        }
#endregion

        #region Bind Department
        public void BindDropDownDepartment(DropDownList ddlDepartmentName, List<WETOS.DataEntity.Department> lstDepartmentParam)
        {
            try
            {
                List<DataEntity.Department> lstDepartment = new List<WETOS.DataEntity.Department>();
                DataEntity.Department AddBlankDepartment = new WETOS.DataEntity.Department();

                AddBlankDepartment.DepartmentId = ClientConstants.Negation;
                AddBlankDepartment.DepartmentName = ClientConstants.SELECT;

                lstDepartment.Add(AddBlankDepartment);
                lstDepartment.AddRange(lstDepartmentParam);

                ddlDepartmentName.DataValueField = ClientConstants.DepartmentId;
                ddlDepartmentName.DataTextField = ClientConstants.DepartmentName;
                ddlDepartmentName.DataSource = lstDepartment;
                ddlDepartmentName.DataBind();

            }
            catch
            {
                throw;
            }
        }
        #endregion                    

        public void BindDropDownDesignation(DropDownList ddlDeisgnation, List<WETOS.DataEntity.Designation> lstAllDesignation)
        {
            try
            {
                List<DataEntity.Designation> lstDesignation = new List<WETOS.DataEntity.Designation>();
                DataEntity.Designation AddBlankDivision = new WETOS.DataEntity.Designation();

                AddBlankDivision.DesignationId = ClientConstants.Negation;
                AddBlankDivision.DesignationName = ClientConstants.SELECT;

                lstDesignation.Add(AddBlankDivision);
                lstDesignation.AddRange(lstAllDesignation);

                ddlDeisgnation.DataValueField = ClientConstants.DesignationId;
                ddlDeisgnation.DataTextField = ClientConstants.DesignationName;
                ddlDeisgnation.DataSource = lstDesignation;
                ddlDeisgnation.DataBind();

            }
            catch
            {
                throw;
            }
        }
        #region Bind Grade
        public void BindDropdownGrade(DropDownList ddlgrade,List<WETOS.DataEntity.Grade> lstGrade)
        {
            try
            {
                List<DataEntity.Grade> lstGradeName = new List<WETOS.DataEntity.Grade>();
                DataEntity.Grade AddBlankGrade = new WETOS.DataEntity.Grade();

                AddBlankGrade.GradeId = ClientConstants.Negation;
                AddBlankGrade.GradeName = ClientConstants.ALL;

                lstGradeName.Add(AddBlankGrade);
                lstGradeName.AddRange(lstGrade);

                ddlgrade.DataValueField = ClientConstants.GradeId;
                ddlgrade.DataTextField = ClientConstants.GradeName;
                ddlgrade.DataSource = lstGradeName;
                ddlgrade.DataBind();

            }
            catch
            {
                throw;
            }
        }

        #endregion
        public void BindDropdownGradeSingle(DropDownList ddlgrade, List<WETOS.DataEntity.Grade> lstGrade)
        {
            try
            {
                List<DataEntity.Grade> lstGradeName = new List<WETOS.DataEntity.Grade>();
                DataEntity.Grade AddBlankGrade = new WETOS.DataEntity.Grade();

                AddBlankGrade.GradeId = ClientConstants.Negation;
                AddBlankGrade.GradeName = ClientConstants.SELECT;

                lstGradeName.Add(AddBlankGrade);
                lstGradeName.AddRange(lstGrade);

                ddlgrade.DataValueField = ClientConstants.GradeId;
                ddlgrade.DataTextField = ClientConstants.GradeName;
                ddlgrade.DataSource = lstGradeName;
                ddlgrade.DataBind();

            }
            catch
            {
                throw;
            }
        }
        #region Bind Status Selection Of Application
        public void BindDropDown(DropDownList ddlStatusSelection, String strAccessRights, int BranchId, int CompanyId)
        {
            try
            {
                List<DataEntity.Status> lstStatus = new List<WETOS.DataEntity.Status>();

                //if (strAccessRights.Contains('5'))
                //{
                DataEntity.Status objPStatus = new WETOS.DataEntity.Status();
                objPStatus.StatusId = ClientConstants.ApplicationPendingStatus;
                objPStatus.StatusName = ClientConstants.ApplicationPendingName;
                lstStatus.Add(objPStatus);
                // }
                if (strAccessRights.Contains(ClientConstants.ApproveRights))
                {
                    DataEntity.Status objStatus = new WETOS.DataEntity.Status();
                    objStatus.StatusId = ClientConstants.ApplicationApproveStatus;
                    objStatus.StatusName = ClientConstants.ApplicationApproveName;
                    lstStatus.Add(objStatus);
                }
                if (strAccessRights.Contains(ClientConstants.SanctionRights))
                {
                    DataEntity.Status objStatus = new WETOS.DataEntity.Status();
                    objStatus.StatusId = ClientConstants.ApplicationSanctionStatus;
                    objStatus.StatusName = ClientConstants.ApplicationSanctionName;
                    lstStatus.Add(objStatus);
                    DataEntity.Status objcancelStatus = new WETOS.DataEntity.Status();
                    objcancelStatus.StatusId = ClientConstants.CancelApplicationStatus;
                    objcancelStatus.StatusName = ClientConstants.ApplicationCancel;
                    lstStatus.Add(objcancelStatus);
                }
                if (strAccessRights.Contains(ClientConstants.RejectRights))
                {
                    DataEntity.Status objStatus = new WETOS.DataEntity.Status();
                    objStatus.StatusId = ClientConstants.ApplicationRejectStatus;
                    objStatus.StatusName = ClientConstants.ApplicationRejectName;
                    lstStatus.Add(objStatus);
                }

                ddlStatusSelection.DataValueField = ClientConstants.StatusId;
                ddlStatusSelection.DataTextField = ClientConstants.StatusName;
                ddlStatusSelection.DataSource = lstStatus;
                ddlStatusSelection.DataBind();

            }
            catch
            {
                throw;
            }


        }
        #endregion        

        #region Bind Branch On Company BasisTemp
        public void FilterBranchOnCompanyBasis(DropDownList ddlBranchName, DataEntity.Company ObjCompany)
        {
            List<DataEntity.Branch> lstBranch = new List<WETOS.DataEntity.Branch>();
            List<DataEntity.Branch> filterlstBranch = new List<WETOS.DataEntity.Branch>();
            DataEntity.Branch AddBlankBranch = new WETOS.DataEntity.Branch();

            AddBlankBranch.BranchId = ClientConstants.Negation;
            AddBlankBranch.BranchName = ClientConstants.SELECT;

            filterlstBranch.AddRange(WETOSSession.TempGetAllBranch);

            filterlstBranch = (from filter in filterlstBranch
                                 where filter.CompanyId == ObjCompany.CompanyId || filter.CompanyId ==0 
                                 select filter).ToList();

            lstBranch.Add(AddBlankBranch);
            lstBranch.AddRange(filterlstBranch);

            ddlBranchName.DataValueField = ClientConstants.BranchId;
            ddlBranchName.DataTextField = ClientConstants.BranchName;
            ddlBranchName.DataSource = lstBranch;
            ddlBranchName.DataBind();
            
        }
        public void FilterBranchOnCompanyBasisTemp(DropDownList ddlBranchName, DataEntity.Company ObjCompany)
        {
            List<DataEntity.Branch> lstBranch = new List<WETOS.DataEntity.Branch>();
            List<DataEntity.Branch> filterlstBranch = new List<WETOS.DataEntity.Branch>();
            DataEntity.Branch AddBlankBranch = new WETOS.DataEntity.Branch();

            AddBlankBranch.BranchId = ClientConstants.Negation;
            AddBlankBranch.BranchName = ClientConstants.SELECT;

            filterlstBranch.AddRange(WETOSSession.TempGetAllBranch);

            filterlstBranch = (from filter in filterlstBranch
                               where filter.CompanyId == ObjCompany.CompanyId
                               select filter).ToList();

            lstBranch.Add(AddBlankBranch);

            lstBranch.AddRange(filterlstBranch);
            

            ddlBranchName.DataValueField = ClientConstants.BranchId;
            ddlBranchName.DataTextField = ClientConstants.BranchName;
            ddlBranchName.DataSource = lstBranch;
            ddlBranchName.DataBind();

        }

        #endregion

        #region Bind Company On Location Basis
        public void FilterCompanyOnLocationBasis(DropDownList ddlBranchName, DataEntity.Location ObjLocation)
        {
            List<DataEntity.Company> lstBranch = new List<WETOS.DataEntity.Company>();
            List<DataEntity.Company> filterlstBranch = new List<WETOS.DataEntity.Company>();
            DataEntity.Company AddBlankBranch = new WETOS.DataEntity.Company();

            AddBlankBranch.CompanyId = ClientConstants.Negation;
            AddBlankBranch.CompanyName = ClientConstants.SELECT;

            filterlstBranch.AddRange(WETOSSession.TempGetAllCompany );

            filterlstBranch = (from filter in filterlstBranch
                               where filter.LocationId == ObjLocation.LocationId
                               select filter).ToList();

            lstBranch.Add(AddBlankBranch);
            lstBranch.AddRange(filterlstBranch);

            ddlBranchName.DataValueField = ClientConstants.CompanyId;
            ddlBranchName.DataTextField = ClientConstants.CompanyName;
            ddlBranchName.DataSource = lstBranch;
            ddlBranchName.DataBind();

        }
        #endregion

        #region Bind division on Branch Basis
        public void FilterDivisiontOnBranchBasis(DropDownList ddlDivision, DataEntity.Branch ObjBranch)
        {
            List<DataEntity.Division> lstDivision = new List<WETOS.DataEntity.Division>();
            DataEntity.Division AddBlankDivision = new WETOS.DataEntity.Division();

            AddBlankDivision.DivisionId = ClientConstants.Negation;
            AddBlankDivision.DivisionName = ClientConstants.ALL;

            lstDivision.Add(AddBlankDivision);
            lstDivision.AddRange(WETOSSession.GetAllDivision);

            lstDivision = (from filter in lstDivision
                             where filter.BranchId == ObjBranch.BranchId || filter.CompanyId == 0
                             select filter).ToList();

            ddlDivision.DataValueField = ClientConstants.DivisionId;
            ddlDivision.DataTextField = ClientConstants.DivisionName;
            ddlDivision.DataSource = lstDivision;
            ddlDivision.DataBind();
        }
         
        #endregion

        #region Bind Department on Branch Basis
        public void FilterDepartmentOnBranchBasis(DropDownList ddlDepartmentName, DataEntity.Branch ObjBranch)
        {
            List<DataEntity.Department> lstDepartment = new List<WETOS.DataEntity.Department>();
            DataEntity.Department AddBlankDepartment = new WETOS.DataEntity.Department();

            AddBlankDepartment.DepartmentId = ClientConstants.Negation;
            AddBlankDepartment.DepartmentName = ClientConstants.SELECT;

            lstDepartment.Add(AddBlankDepartment);
            lstDepartment.AddRange(WETOSSession.GetAllDepartment);

            lstDepartment = (from filter in lstDepartment
                             where filter.BranchId == ObjBranch.BranchId ||filter.CompanyId==0
                             select filter).ToList();

            ddlDepartmentName.DataValueField = ClientConstants.DepartmentId;
            ddlDepartmentName.DataTextField = ClientConstants.DepartmentName;
            ddlDepartmentName.DataSource = lstDepartment;
            ddlDepartmentName.DataBind();
        }
        public void FilterDepartmentOnBranchBasisTemp(DropDownList ddlDepartmentName, DataEntity.Branch ObjBranch)
        {
            List<DataEntity.Department> lstDepartment = new List<WETOS.DataEntity.Department>();
            DataEntity.Department AddBlankDepartment = new WETOS.DataEntity.Department();

            AddBlankDepartment.DepartmentId = ClientConstants.Negation;
            AddBlankDepartment.DepartmentName = ClientConstants.SELECT;

            lstDepartment.Add(AddBlankDepartment);
            lstDepartment.AddRange(WETOSSession.TempGetAllDepartment);

            lstDepartment = (from filter in lstDepartment
                             where filter.BranchId == ObjBranch.BranchId || filter.CompanyId == 0
                             select filter).ToList();

            ddlDepartmentName.DataValueField = ClientConstants.DepartmentId;
            ddlDepartmentName.DataTextField = ClientConstants.DepartmentName;
            ddlDepartmentName.DataSource = lstDepartment;
            ddlDepartmentName.DataBind();
        }

        public void FilterDepartmentOnDivisionBasisTemp(DropDownList ddlDepartmentName, DataEntity.Division ObjDivision)
        {
            List<DataEntity.Department> lstDepartment = new List<WETOS.DataEntity.Department>();
            DataEntity.Department AddBlankDepartment = new WETOS.DataEntity.Department();

            AddBlankDepartment.DepartmentId = ClientConstants.Negation;
            AddBlankDepartment.DepartmentName = ClientConstants.SELECT;

            lstDepartment.Add(AddBlankDepartment);
            lstDepartment.AddRange(WETOSSession.TempGetAllDepartment);

            lstDepartment = (from filter in lstDepartment
                             where (filter.DivisionID == ObjDivision.DivisionId && filter.BranchId==ObjDivision.BranchId) || filter.CompanyId == 0
                             select filter).ToList();

            ddlDepartmentName.DataValueField = ClientConstants.DepartmentId;
            ddlDepartmentName.DataTextField = ClientConstants.DepartmentName;
            ddlDepartmentName.DataSource = lstDepartment;
            ddlDepartmentName.DataBind();
        }


        public void FilterDepartmentOnDivisionBasis(DropDownList ddldepartmentName, DataEntity.Department ObjDepartment)
        {
            List<DataEntity.Department> lstDepartment = new List<WETOS.DataEntity.Department>();
            DataEntity.Department AddBlankDepartment = new WETOS.DataEntity.Department();

            //AddBlankDepartment.DepartmentId = ClientConstants.Negation;
            //AddBlankDepartment.DepartmentName = ClientConstants.SELECT;

            //lstDepartment.Add(AddBlankDepartment);
            lstDepartment.AddRange(WETOSSession.GetAllDepartment);

            lstDepartment = (from filter in lstDepartment
                             where (filter.DivisionID == ObjDepartment.DivisionID && filter.BranchId == ObjDepartment.BranchId) && filter.CompanyId == ObjDepartment.CompanyId
                             select filter).ToList();

            AddBlankDepartment.DepartmentId = ClientConstants.Negation;
            AddBlankDepartment.DepartmentName = ClientConstants.ALL;

            lstDepartment.Add(AddBlankDepartment);

            ddldepartmentName.DataValueField = ClientConstants.DepartmentId;
            ddldepartmentName.DataTextField = ClientConstants.DepartmentName;
            ddldepartmentName.DataSource = lstDepartment;
            ddldepartmentName.DataBind();
        }

        #endregion

        #region Bind Designation on Department Basis
        public void FilterDesignationOnDepartmentBasis(DropDownList ddlDesignationName, DataEntity.Department ObjDepartment)
        {
            List<DataEntity.Designation> lstDesignation = new List<WETOS.DataEntity.Designation>();
            DataEntity.Designation AddBlankDesignation = new WETOS.DataEntity.Designation();

            AddBlankDesignation.DesignationId = ClientConstants.Negation;
            AddBlankDesignation.DesignationName = ClientConstants.SELECT;

            lstDesignation.Add(AddBlankDesignation);
            lstDesignation.AddRange(WETOSSession.GetAllDesignation);

            lstDesignation = (from filter in lstDesignation
                              where filter.DepartmentId == ObjDepartment.DepartmentId ||filter.CompanyId==0
                              select filter).ToList();

            ddlDesignationName.DataValueField = ClientConstants.DesignationId;
            ddlDesignationName.DataTextField = ClientConstants.DesignationName;
            ddlDesignationName.DataSource = lstDesignation;
            ddlDesignationName.DataBind();
        }

        public void BindDropdownDesignation(DropDownList ddlDesignationName)
        {
            List<DataEntity.Designation> lstDesignation = new List<WETOS.DataEntity.Designation>();
            DataEntity.Designation AddBlankDesignation = new WETOS.DataEntity.Designation();

            AddBlankDesignation.DesignationId = ClientConstants.Negation;
            AddBlankDesignation.DesignationName = ClientConstants.SELECT;

            lstDesignation.Add(AddBlankDesignation);
            lstDesignation.AddRange(WETOSSession.GetAllDesignation);

           

            ddlDesignationName.DataValueField = ClientConstants.DesignationId;
            ddlDesignationName.DataTextField = ClientConstants.DesignationName;
            ddlDesignationName.DataSource = lstDesignation;
            ddlDesignationName.DataBind();
        }
        public void FilterDesignationOnDepartmentBasisTemp(DropDownList ddlDesignationName, DataEntity.Department ObjDepartment)
        {
            List<DataEntity.Designation> lstDesignation = new List<WETOS.DataEntity.Designation>();
            DataEntity.Designation AddBlankDesignation = new WETOS.DataEntity.Designation();

            AddBlankDesignation.DesignationId = ClientConstants.Negation;
            AddBlankDesignation.DesignationName = ClientConstants.SELECT;

            lstDesignation.Add(AddBlankDesignation);
            lstDesignation.AddRange(WETOSSession.TempGetAllDesignation);

            lstDesignation = (from filter in lstDesignation
                              where filter.DepartmentId == ObjDepartment.DepartmentId || filter.CompanyId == 0
                              select filter).ToList();

            ddlDesignationName.DataValueField = ClientConstants.DesignationId;
            ddlDesignationName.DataTextField = ClientConstants.DesignationName;
            ddlDesignationName.DataSource = lstDesignation;
            ddlDesignationName.DataBind();
        }

        #endregion

        #region Bind DropDownList Employee Code and Employee Name
        #region Bind EmployeeCode and EmployeeName
        public void BindDropDownEmployeeCode(DropDownList ddlEmployeeName)
        {
            List<DataEntity.Employee> lstEmployee = new List<WETOS.DataEntity.Employee>();
            DataEntity.Employee AddBlankEmployee = new WETOS.DataEntity.Employee();

            AddBlankEmployee.EmployeeId = ClientConstants.Negation;
            AddBlankEmployee.EmployeeCode = ClientConstants.SELECT;

            lstEmployee.Add(AddBlankEmployee);
            lstEmployee.AddRange(WETOSSession.GetAllEmployee);
            ddlEmployeeName.DataValueField = ClientConstants.EmployeeId;
            ddlEmployeeName.DataTextField = ClientConstants.EmployeeCode;
            ddlEmployeeName.DataSource = lstEmployee;
            ddlEmployeeName.DataBind();
        }
        public void BindDropDownEmpCodeAndName(DropDownList ddlEmployeeName)
        {
            List<DataEntity.Employee> lstEmployee = new List<WETOS.DataEntity.Employee>();
            DataEntity.Employee AddBlankEmployee = new WETOS.DataEntity.Employee();

            AddBlankEmployee.EmployeeId = ClientConstants.Negation;
            AddBlankEmployee.EmpCodeAndName = ClientConstants.SELECT;

            lstEmployee.Add(AddBlankEmployee);
            lstEmployee.AddRange(WETOSSession.GetAllEmployee);

            ddlEmployeeName.DataValueField = ClientConstants.EmployeeId;
            ddlEmployeeName.DataTextField = ClientConstants.EmpCodeAndName;
            ddlEmployeeName.DataSource = lstEmployee;
            ddlEmployeeName.DataBind();
        }
        public void BindDropDownEmpCodeAndName(DropDownList ddlEmployeeName,List<WETOS.DataEntity.Employee> lstAllEmployee)
        {
            List<DataEntity.Employee> lstEmployee = new List<WETOS.DataEntity.Employee>();
            DataEntity.Employee AddBlankEmployee = new WETOS.DataEntity.Employee();

            AddBlankEmployee.EmployeeId = ClientConstants.Negation;
            AddBlankEmployee.EmpCodeAndName = ClientConstants.SELECT;

            lstEmployee.Add(AddBlankEmployee);
            lstEmployee.AddRange(lstAllEmployee);

            ddlEmployeeName.DataValueField = ClientConstants.EmployeeId;
            ddlEmployeeName.DataTextField = ClientConstants.EmpCodeAndName;
            ddlEmployeeName.DataSource = lstEmployee;
            ddlEmployeeName.DataBind();
        }

        public void BindDropDownEmployeeName(DropDownList ddlEmployeeName)
        {
            List<DataEntity.Employee> lstEmployee = new List<WETOS.DataEntity.Employee>();
            DataEntity.Employee AddBlankEmployee = new WETOS.DataEntity.Employee();

            AddBlankEmployee.EmployeeId = ClientConstants.Negation;
            AddBlankEmployee.FullNameWithEmployeeCode = ClientConstants.SELECT;

            lstEmployee.Add(AddBlankEmployee);
            lstEmployee.AddRange(WETOSSession.GetAllEmployee);

            ddlEmployeeName.DataValueField = ClientConstants.EmployeeId;
            ddlEmployeeName.DataTextField = ClientConstants.EmployeeName;
            ddlEmployeeName.DataSource = lstEmployee;
            ddlEmployeeName.DataBind();
        }

        public void BindDropDownEmployeeName(DropDownList ddlEmployeeName,List<WETOS.DataEntity.Employee> lstEmployeeParam)
        {
            List<DataEntity.Employee> lstEmployee = new List<WETOS.DataEntity.Employee>();
            DataEntity.Employee AddBlankEmployee = new WETOS.DataEntity.Employee();

            AddBlankEmployee.EmployeeId = ClientConstants.Negation;
            AddBlankEmployee.FullNameWithEmployeeCode = ClientConstants.SELECT;

            lstEmployee.Add(AddBlankEmployee);
            lstEmployee.AddRange(lstEmployeeParam);

            ddlEmployeeName.DataValueField = ClientConstants.EmployeeId;
            ddlEmployeeName.DataTextField = ClientConstants.EmployeeName;
            ddlEmployeeName.DataSource = lstEmployee;
            ddlEmployeeName.DataBind();
        }
        #endregion

        #region Bind EmployeeCode and EmployeeName on Company Basis
        public void FilterEmployeeCodeOnCompanyBasis(DropDownList ddlEmployeeName, DataEntity.Company ObjCompany)
        {
            List<DataEntity.Employee> lstEmployee = new List<WETOS.DataEntity.Employee>();
            List<DataEntity.Employee> filterlstEmployee = new List<WETOS.DataEntity.Employee>();
            DataEntity.Employee AddBlankEmployee = new WETOS.DataEntity.Employee();

            AddBlankEmployee.EmployeeId = ClientConstants.Negation;
            AddBlankEmployee.EmployeeCode = ClientConstants.SELECT;

            //lstEmployee.Add(AddBlankEmployee);
            filterlstEmployee.AddRange(WETOSSession.GetAllEmployee);

            filterlstEmployee = (from filter in filterlstEmployee
                                 where filter.CompanyId == ObjCompany.CompanyId
                                 select filter).ToList();

            lstEmployee.Add(AddBlankEmployee);
            lstEmployee.AddRange(filterlstEmployee);


            ddlEmployeeName.DataValueField = ClientConstants.EmployeeId;
            ddlEmployeeName.DataTextField = ClientConstants.EmployeeCode;
            ddlEmployeeName.DataSource = lstEmployee;
            ddlEmployeeName.DataBind();
        }

        public void FilterEmployeeNameOnCompanyBasis(DropDownList ddlEmployeeName, DataEntity.Company ObjCompany)
        {
            List<DataEntity.Employee> lstEmployee = new List<WETOS.DataEntity.Employee>();
            List<DataEntity.Employee> filterlstEmployee = new List<WETOS.DataEntity.Employee>();
            DataEntity.Employee AddBlankEmployee = new WETOS.DataEntity.Employee();

            AddBlankEmployee.EmployeeId = ClientConstants.Negation;
            AddBlankEmployee.FullNameWithEmployeeCode = ClientConstants.SELECT;

            //lstEmployee.Add(AddBlankEmployee);
            filterlstEmployee.AddRange(WETOSSession.GetAllEmployee);

            filterlstEmployee = (from filter in filterlstEmployee
                                 where filter.CompanyId == ObjCompany.CompanyId
                                 select filter).ToList();

            lstEmployee.Add(AddBlankEmployee);
            lstEmployee.AddRange(filterlstEmployee);


            ddlEmployeeName.DataValueField = ClientConstants.EmployeeId;
            ddlEmployeeName.DataTextField = ClientConstants.EmployeeName;
            ddlEmployeeName.DataSource = lstEmployee;
            ddlEmployeeName.DataBind();
        }
        #endregion

        #region Bind EmployeeCode and EmployeeName on Branch Basis
        public void FilterEmployeeCodeOnBranchBasis(DropDownList ddlEmployeeName, DataEntity.Branch ObjBranch)
        {
            List<DataEntity.Employee> lstEmployee = new List<WETOS.DataEntity.Employee>();
            List<DataEntity.Employee> filterlstEmployee = new List<WETOS.DataEntity.Employee>();
            DataEntity.Employee AddBlankEmployee = new WETOS.DataEntity.Employee();

            AddBlankEmployee.EmployeeId = ClientConstants.Negation;
            AddBlankEmployee.EmployeeCode = ClientConstants.SELECT;

            filterlstEmployee.AddRange(WETOSSession.GetAllEmployee);

            filterlstEmployee = (from filter in filterlstEmployee
                                 where filter.BranchId == ObjBranch.BranchId
                                 select filter).ToList();

            lstEmployee.Add(AddBlankEmployee);
            lstEmployee.AddRange(filterlstEmployee);

            ddlEmployeeName.DataValueField = ClientConstants.EmployeeId;
            ddlEmployeeName.DataTextField = ClientConstants.EmployeeCode;
            ddlEmployeeName.DataSource = lstEmployee;
            ddlEmployeeName.DataBind();
        }

        public void FilterEmployeeNameOnBranchBasis(DropDownList ddlEmployeeName, DataEntity.Branch ObjBranch)
        {
            List<DataEntity.Employee> lstEmployee = new List<WETOS.DataEntity.Employee>();
            List<DataEntity.Employee> filterlstEmployee = new List<WETOS.DataEntity.Employee>();
            DataEntity.Employee AddBlankEmployee = new WETOS.DataEntity.Employee();

            AddBlankEmployee.EmployeeId = ClientConstants.Negation;
            AddBlankEmployee.EmpCodeAndName = ClientConstants.SELECT;
            //AddBlankEmployee.FullNameWithEmployeeCode = ClientConstants.SELECT;

            filterlstEmployee.AddRange(WETOSSession.GetAllEmployee);

            filterlstEmployee = (from filter in filterlstEmployee
                                 where filter.BranchId == ObjBranch.BranchId
                                 select filter).ToList();

            lstEmployee.Add(AddBlankEmployee);
            lstEmployee.AddRange(filterlstEmployee);

            ddlEmployeeName.DataValueField = ClientConstants.EmployeeId;
            ddlEmployeeName.DataTextField = ClientConstants.EmpCodeAndName;
            ddlEmployeeName.DataSource = lstEmployee;
            ddlEmployeeName.DataBind();
        }
        #endregion

        #region Bind EmployeeCode and EmployeeName on Department Basis
        public void FilterEmployeeCodeOnDepartmentBasis(DropDownList ddlEmployeeName, DataEntity.Department ObjDepartment)
        {
            List<DataEntity.Employee> lstEmployee = new List<WETOS.DataEntity.Employee>();
            List<DataEntity.Employee> filterlstEmployee = new List<WETOS.DataEntity.Employee>();
            DataEntity.Employee AddBlankEmployee = new WETOS.DataEntity.Employee();

            AddBlankEmployee.EmployeeId = ClientConstants.Negation;
            AddBlankEmployee.EmployeeCode = ClientConstants.SELECT;

            filterlstEmployee.AddRange(WETOSSession.GetAllEmployee);

            filterlstEmployee = (from filter in filterlstEmployee
                                 where filter.DepartmentId == ObjDepartment.DepartmentId
                                 select filter).ToList();

            lstEmployee.Add(AddBlankEmployee);
            lstEmployee.AddRange(filterlstEmployee);

            ddlEmployeeName.DataValueField = ClientConstants.EmployeeId;
            ddlEmployeeName.DataTextField = ClientConstants.EmployeeCode;
            ddlEmployeeName.DataSource = lstEmployee;
            ddlEmployeeName.DataBind();
        }

        public void FilterEmployeeNameOnDepartmentBasis(DropDownList ddlEmployeeName, DataEntity.Department ObjDepartment)
        {
            List<DataEntity.Employee> lstEmployee = new List<WETOS.DataEntity.Employee>();
            List<DataEntity.Employee> filterlstEmployee = new List<WETOS.DataEntity.Employee>();
            DataEntity.Employee AddBlankEmployee = new WETOS.DataEntity.Employee();

            AddBlankEmployee.EmployeeId = ClientConstants.Negation;
            AddBlankEmployee.FullNameWithEmployeeCode = ClientConstants.SELECT;

            filterlstEmployee.AddRange(WETOSSession.GetAllEmployee);

            filterlstEmployee = (from filter in filterlstEmployee
                                 where filter.DepartmentId == ObjDepartment.DepartmentId
                                 select filter).ToList();

            lstEmployee.Add(AddBlankEmployee);
            lstEmployee.AddRange(filterlstEmployee);

            ddlEmployeeName.DataValueField = ClientConstants.EmployeeId;
            ddlEmployeeName.DataTextField = ClientConstants.EmployeeName;
            ddlEmployeeName.DataSource = lstEmployee;
            ddlEmployeeName.DataBind();
        }
        #endregion

        #region Bind EmployeeCode and EmployeeName on Designation Basis
        public void FilterEmployeeCodeOnDesignationBasis(DropDownList ddlEmployeeName, DataEntity.Designation ObjDesignation)
        {
            List<DataEntity.Employee> lstEmployee = new List<WETOS.DataEntity.Employee>();
            List<DataEntity.Employee> filterlstEmployee = new List<WETOS.DataEntity.Employee>();
            DataEntity.Employee AddBlankEmployee = new WETOS.DataEntity.Employee();

            AddBlankEmployee.EmployeeId = ClientConstants.Negation;
            AddBlankEmployee.EmployeeCode = ClientConstants.SELECT;

            filterlstEmployee.AddRange(WETOSSession.GetAllEmployee);

            filterlstEmployee = (from filter in filterlstEmployee
                                 where filter.DesignationId == ObjDesignation.DesignationId
                                 select filter).ToList();

            lstEmployee.Add(AddBlankEmployee);
            lstEmployee.AddRange(filterlstEmployee);

            ddlEmployeeName.DataValueField = ClientConstants.EmployeeId;
            ddlEmployeeName.DataTextField = ClientConstants.EmployeeCode;
            ddlEmployeeName.DataSource = lstEmployee;
            ddlEmployeeName.DataBind();
        }

        public void FilterEmployeeNameOnDesignationBasis(DropDownList ddlEmployeeName, DataEntity.Designation ObjDesignation)
        {
            List<DataEntity.Employee> lstEmployee = new List<WETOS.DataEntity.Employee>();
            List<DataEntity.Employee> filterlstEmployee = new List<WETOS.DataEntity.Employee>();
            DataEntity.Employee AddBlankEmployee = new WETOS.DataEntity.Employee();

            AddBlankEmployee.EmployeeId = ClientConstants.Negation;
            AddBlankEmployee.FullNameWithEmployeeCode = ClientConstants.SELECT;

            filterlstEmployee.AddRange(WETOSSession.GetAllEmployee);

            filterlstEmployee = (from filter in filterlstEmployee
                                 where filter.DesignationId == ObjDesignation.DesignationId
                                 select filter).ToList();

            lstEmployee.Add(AddBlankEmployee);
            lstEmployee.AddRange(filterlstEmployee);

            ddlEmployeeName.DataValueField = ClientConstants.EmployeeId;
            ddlEmployeeName.DataTextField = ClientConstants.EmployeeName;
            ddlEmployeeName.DataSource = lstEmployee;
            ddlEmployeeName.DataBind();
        }
        #endregion
        #endregion        

        #region Bind DropDownList Employee Code and Employee Name Without Created Users

        #region Bind EmployeeCode and EmployeeName
        public void BindDropDownEmployeeCodeWithOutCreated(DropDownList ddlEmployeeName, List<DataEntity.User> listUser)
        {
            List<DataEntity.Employee> lstEmployee = new List<WETOS.DataEntity.Employee>();
            List<DataEntity.Employee> lstTempEmployee = new List<WETOS.DataEntity.Employee>();

            DataEntity.Employee AddBlankEmployee = new WETOS.DataEntity.Employee();

            AddBlankEmployee.EmployeeId = ClientConstants.Negation;
            AddBlankEmployee.EmployeeCode = ClientConstants.SELECT;


            lstTempEmployee = WETOSSession.GetAllEmployee;
            foreach (var item in listUser)
            {
                lstTempEmployee = (from chk in lstTempEmployee
                                   where chk.EmployeeId != item.EmployeeId
                                   select chk).ToList();
            }

            lstEmployee.Add(AddBlankEmployee);
            lstEmployee.AddRange(lstTempEmployee);

            ddlEmployeeName.DataValueField = ClientConstants.EmployeeId;
            ddlEmployeeName.DataTextField = ClientConstants.EmployeeCode;
            ddlEmployeeName.DataSource = lstEmployee;
            ddlEmployeeName.DataBind();
        }

        public void BindDropDownEmployeeCodeWithOutCreatedTemp(DropDownList ddlEmployeeName, List<DataEntity.User> listUser)
        {
            List<DataEntity.Employee> lstEmployee = new List<WETOS.DataEntity.Employee>();
            List<DataEntity.Employee> lstTempEmployee = new List<WETOS.DataEntity.Employee>();

            DataEntity.Employee AddBlankEmployee = new WETOS.DataEntity.Employee();

            AddBlankEmployee.EmployeeId = ClientConstants.Negation;
            AddBlankEmployee.EmployeeCode = ClientConstants.SELECT;


            lstTempEmployee = WETOSSession.TempGetAllEmployee;
            foreach (var item in listUser)
            {
                lstTempEmployee = (from chk in lstTempEmployee
                                   where chk.EmployeeId != item.EmployeeId
                                   select chk).ToList();
            }

            lstEmployee.Add(AddBlankEmployee);
            lstEmployee.AddRange(lstTempEmployee);

            ddlEmployeeName.DataValueField = ClientConstants.EmployeeId;
            ddlEmployeeName.DataTextField = ClientConstants.EmployeeCode;
            ddlEmployeeName.DataSource = lstEmployee;
            ddlEmployeeName.DataBind();
        }

        public void BindDropDownEmployeeNameWithOutCreated(DropDownList ddlEmployeeName, List<DataEntity.User> listUser)
        {
            List<DataEntity.Employee> lstEmployee = new List<WETOS.DataEntity.Employee>();
            List<DataEntity.Employee> lstTempEmployee = new List<WETOS.DataEntity.Employee>();
            DataEntity.Employee AddBlankEmployee = new WETOS.DataEntity.Employee();

            AddBlankEmployee.EmployeeId = ClientConstants.Negation;
            AddBlankEmployee.FullNameWithEmployeeCode = ClientConstants.SELECT;

            lstTempEmployee = WETOSSession.GetAllEmployee;
            foreach (var item in listUser)
            {
                lstTempEmployee = (from chk in lstTempEmployee
                                   where chk.EmployeeId != item.EmployeeId
                                   select chk).ToList();
            }

            lstEmployee.Add(AddBlankEmployee);
            lstEmployee.AddRange(lstTempEmployee);

            ddlEmployeeName.DataValueField = ClientConstants.EmployeeId;
            ddlEmployeeName.DataTextField = ClientConstants.EmployeeName;
            ddlEmployeeName.DataSource = lstEmployee;
            ddlEmployeeName.DataBind();
        }
        public void BindDropDownEmployeeNameWithOutCreatedTemp(DropDownList ddlEmployeeName, List<DataEntity.User> listUser)
        {
            List<DataEntity.Employee> lstEmployee = new List<WETOS.DataEntity.Employee>();
            List<DataEntity.Employee> lstTempEmployee = new List<WETOS.DataEntity.Employee>();
            DataEntity.Employee AddBlankEmployee = new WETOS.DataEntity.Employee();

            AddBlankEmployee.EmployeeId = ClientConstants.Negation;
            AddBlankEmployee.FullNameWithEmployeeCode = ClientConstants.SELECT;

            lstTempEmployee = WETOSSession.TempGetAllEmployee;
            foreach (var item in listUser)
            {
                lstTempEmployee = (from chk in lstTempEmployee
                                   where chk.EmployeeId != item.EmployeeId
                                   select chk).ToList();
            }

            lstEmployee.Add(AddBlankEmployee);
            lstEmployee.AddRange(lstTempEmployee);

            ddlEmployeeName.DataValueField = ClientConstants.EmployeeId;
            ddlEmployeeName.DataTextField = ClientConstants.EmployeeName;
            ddlEmployeeName.DataSource = lstEmployee;
            ddlEmployeeName.DataBind();
        }

        #endregion

        #region Bind EmployeeCode and EmployeeName on Company Basis
        public void FilterEmployeeCodeWithOutCreatedOnCompanyBasis(DropDownList ddlEmployeeName, DataEntity.Company ObjCompany, List<DataEntity.User> listUser)
        {
            List<DataEntity.Employee> lstEmployee = new List<WETOS.DataEntity.Employee>();
            List<DataEntity.Employee> filterlstEmployee = new List<WETOS.DataEntity.Employee>();
            List<DataEntity.Employee> lstTempEmployee = new List<WETOS.DataEntity.Employee>();
            DataEntity.Employee AddBlankEmployee = new WETOS.DataEntity.Employee();

            AddBlankEmployee.EmployeeId = ClientConstants.Negation;
            AddBlankEmployee.EmployeeCode = ClientConstants.SELECT;

            //lstEmployee.Add(AddBlankEmployee);
            filterlstEmployee.AddRange(WETOSSession.GetAllEmployee);

            filterlstEmployee = (from filter in filterlstEmployee
                                 where filter.CompanyId == ObjCompany.CompanyId
                                 select filter).ToList();

            lstTempEmployee = filterlstEmployee;
            foreach (var item in listUser)
            {
                lstTempEmployee = (from chk in lstTempEmployee
                                   where chk.EmployeeId != item.EmployeeId
                                   select chk).ToList();
            }

            lstEmployee.Add(AddBlankEmployee);
            lstEmployee.AddRange(lstTempEmployee);


            ddlEmployeeName.DataValueField = ClientConstants.EmployeeId;
            ddlEmployeeName.DataTextField = ClientConstants.EmployeeCode;
            ddlEmployeeName.DataSource = lstEmployee;
            ddlEmployeeName.DataBind();
        }
        public void FilterEmployeeCodeWithOutCreatedOnCompanyBasisTemp(DropDownList ddlEmployeeName, DataEntity.Company ObjCompany, List<DataEntity.User> listUser)
        {
            List<DataEntity.Employee> lstEmployee = new List<WETOS.DataEntity.Employee>();
            List<DataEntity.Employee> filterlstEmployee = new List<WETOS.DataEntity.Employee>();
            List<DataEntity.Employee> lstTempEmployee = new List<WETOS.DataEntity.Employee>();
            DataEntity.Employee AddBlankEmployee = new WETOS.DataEntity.Employee();

            AddBlankEmployee.EmployeeId = ClientConstants.Negation;
            AddBlankEmployee.EmployeeCode = ClientConstants.SELECT;

            //lstEmployee.Add(AddBlankEmployee);
            filterlstEmployee.AddRange(WETOSSession.TempGetAllEmployee);

            filterlstEmployee = (from filter in filterlstEmployee
                                 where filter.CompanyId == ObjCompany.CompanyId
                                 select filter).ToList();

            lstTempEmployee = filterlstEmployee;
            foreach (var item in listUser)
            {
                lstTempEmployee = (from chk in lstTempEmployee
                                   where chk.EmployeeId != item.EmployeeId
                                   select chk).ToList();
            }

            lstEmployee.Add(AddBlankEmployee);
            lstEmployee.AddRange(lstTempEmployee);


            ddlEmployeeName.DataValueField = ClientConstants.EmployeeId;
            ddlEmployeeName.DataTextField = ClientConstants.EmployeeCode;
            ddlEmployeeName.DataSource = lstEmployee;
            ddlEmployeeName.DataBind();
        }
        public void FilterEmployeeNameWithOutCreatedOnCompanyBasis(DropDownList ddlEmployeeName, DataEntity.Company ObjCompany, List<DataEntity.User> listUser)
        {
            List<DataEntity.Employee> lstEmployee = new List<WETOS.DataEntity.Employee>();
            List<DataEntity.Employee> filterlstEmployee = new List<WETOS.DataEntity.Employee>();
            List<DataEntity.Employee> lstTempEmployee = new List<WETOS.DataEntity.Employee>();
            DataEntity.Employee AddBlankEmployee = new WETOS.DataEntity.Employee();

            AddBlankEmployee.EmployeeId = ClientConstants.Negation;
            AddBlankEmployee.FullNameWithEmployeeCode = ClientConstants.SELECT;

            //lstEmployee.Add(AddBlankEmployee);
            filterlstEmployee.AddRange(WETOSSession.GetAllEmployee);

            filterlstEmployee = (from filter in filterlstEmployee
                                 where filter.CompanyId == ObjCompany.CompanyId
                                 select filter).ToList();

            lstTempEmployee = filterlstEmployee;
            foreach (var item in listUser)
            {
                lstTempEmployee = (from chk in lstTempEmployee
                                   where chk.EmployeeId != item.EmployeeId
                                   select chk).ToList();
            }

            lstEmployee.Add(AddBlankEmployee);
            lstEmployee.AddRange(lstTempEmployee);

            ddlEmployeeName.DataValueField = ClientConstants.EmployeeId;
            ddlEmployeeName.DataTextField = ClientConstants.EmployeeName;
            ddlEmployeeName.DataSource = lstEmployee;
            ddlEmployeeName.DataBind();
        }
        public void FilterEmployeeNameWithOutCreatedOnCompanyBasisTemp(DropDownList ddlEmployeeName, DataEntity.Company ObjCompany, List<DataEntity.User> listUser)
        {
            List<DataEntity.Employee> lstEmployee = new List<WETOS.DataEntity.Employee>();
            List<DataEntity.Employee> filterlstEmployee = new List<WETOS.DataEntity.Employee>();
            List<DataEntity.Employee> lstTempEmployee = new List<WETOS.DataEntity.Employee>();
            DataEntity.Employee AddBlankEmployee = new WETOS.DataEntity.Employee();

            AddBlankEmployee.EmployeeId = ClientConstants.Negation;
            AddBlankEmployee.FullNameWithEmployeeCode = ClientConstants.SELECT;

            //lstEmployee.Add(AddBlankEmployee);
            filterlstEmployee.AddRange(WETOSSession.TempGetAllEmployee);

            filterlstEmployee = (from filter in filterlstEmployee
                                 where filter.CompanyId == ObjCompany.CompanyId
                                 select filter).ToList();

            lstTempEmployee = filterlstEmployee;
            foreach (var item in listUser)
            {
                lstTempEmployee = (from chk in lstTempEmployee
                                   where chk.EmployeeId != item.EmployeeId
                                   select chk).ToList();
            }

            lstEmployee.Add(AddBlankEmployee);
            lstEmployee.AddRange(lstTempEmployee);

            ddlEmployeeName.DataValueField = ClientConstants.EmployeeId;
            ddlEmployeeName.DataTextField = ClientConstants.EmployeeName;
            ddlEmployeeName.DataSource = lstEmployee;
            ddlEmployeeName.DataBind();
        }
        #endregion

        #region Bind EmployeeCode and EmployeeName on Branch Basis
        public void FilterEmployeeCodeWithOutCreatedOnBranchBasis(DropDownList ddlEmployeeName, DataEntity.Branch ObjBranch, List<DataEntity.User> listUser)
        {
            List<DataEntity.Employee> lstEmployee = new List<WETOS.DataEntity.Employee>();
            List<DataEntity.Employee> filterlstEmployee = new List<WETOS.DataEntity.Employee>();
            List<DataEntity.Employee> lstTempEmployee = new List<WETOS.DataEntity.Employee>();
            DataEntity.Employee AddBlankEmployee = new WETOS.DataEntity.Employee();

            AddBlankEmployee.EmployeeId = ClientConstants.Negation;
            AddBlankEmployee.EmployeeCode = ClientConstants.SELECT;

            filterlstEmployee.AddRange(WETOSSession.GetAllEmployee);

            filterlstEmployee = (from filter in filterlstEmployee
                                 where filter.BranchId == ObjBranch.BranchId
                                 select filter).ToList();

            lstTempEmployee = filterlstEmployee;
            foreach (var item in listUser)
            {
                lstTempEmployee = (from chk in lstTempEmployee
                                   where chk.EmployeeId != item.EmployeeId
                                   select chk).ToList();
            }

            lstEmployee.Add(AddBlankEmployee);
            lstEmployee.AddRange(lstTempEmployee);

            ddlEmployeeName.DataValueField = ClientConstants.EmployeeId;
            ddlEmployeeName.DataTextField = ClientConstants.EmployeeCode;
            ddlEmployeeName.DataSource = lstEmployee;
            ddlEmployeeName.DataBind();
        }
        public void FilterEmployeeCodeWithOutCreatedOnBranchBasisTemp(DropDownList ddlEmployeeName, DataEntity.Branch ObjBranch, List<DataEntity.User> listUser)
        {
            List<DataEntity.Employee> lstEmployee = new List<WETOS.DataEntity.Employee>();
            List<DataEntity.Employee> filterlstEmployee = new List<WETOS.DataEntity.Employee>();
            List<DataEntity.Employee> lstTempEmployee = new List<WETOS.DataEntity.Employee>();
            DataEntity.Employee AddBlankEmployee = new WETOS.DataEntity.Employee();

            AddBlankEmployee.EmployeeId = ClientConstants.Negation;
            AddBlankEmployee.EmployeeCode = ClientConstants.SELECT;

            filterlstEmployee.AddRange(WETOSSession.TempGetAllEmployee);

            filterlstEmployee = (from filter in filterlstEmployee
                                 where filter.BranchId == ObjBranch.BranchId
                                 select filter).ToList();

            lstTempEmployee = filterlstEmployee;
            foreach (var item in listUser)
            {
                lstTempEmployee = (from chk in lstTempEmployee
                                   where chk.EmployeeId != item.EmployeeId
                                   select chk).ToList();
            }

            lstEmployee.Add(AddBlankEmployee);
            lstEmployee.AddRange(lstTempEmployee);

            ddlEmployeeName.DataValueField = ClientConstants.EmployeeId;
            ddlEmployeeName.DataTextField = ClientConstants.EmployeeCode;
            ddlEmployeeName.DataSource = lstEmployee;
            ddlEmployeeName.DataBind();
        }
        public void FilterEmployeeCodeWithOutCreatedOnDivisionBasisTemp(DropDownList ddlEmployeeName, DataEntity.Division ObjDivision, List<DataEntity.User> listUser)
        {
            List<DataEntity.Employee> lstEmployee = new List<WETOS.DataEntity.Employee>();
            List<DataEntity.Employee> filterlstEmployee = new List<WETOS.DataEntity.Employee>();
            List<DataEntity.Employee> lstTempEmployee = new List<WETOS.DataEntity.Employee>();
            DataEntity.Employee AddBlankEmployee = new WETOS.DataEntity.Employee();

            AddBlankEmployee.EmployeeId = ClientConstants.Negation;
            AddBlankEmployee.EmployeeCode = ClientConstants.SELECT;

            filterlstEmployee.AddRange(WETOSSession.TempGetAllEmployee);

            filterlstEmployee = (from filter in filterlstEmployee
                                 where  filter.DivisionId == ObjDivision.DivisionId
                                 select filter).ToList();

            lstTempEmployee = filterlstEmployee;
            foreach (var item in listUser)
            {
                lstTempEmployee = (from chk in lstTempEmployee
                                   where chk.EmployeeId != item.EmployeeId
                                   select chk).ToList();
            }

            lstEmployee.Add(AddBlankEmployee);
            lstEmployee.AddRange(lstTempEmployee);

            ddlEmployeeName.DataValueField = ClientConstants.EmployeeId;
            ddlEmployeeName.DataTextField = ClientConstants.EmployeeCode;
            ddlEmployeeName.DataSource = lstEmployee;
            ddlEmployeeName.DataBind();
        }
        public void FilterEmployeeNameWithOutCreatedOnBranchBasis(DropDownList ddlEmployeeName, DataEntity.Branch ObjBranch, List<DataEntity.User> listUser)
        {
            List<DataEntity.Employee> lstEmployee = new List<WETOS.DataEntity.Employee>();
            List<DataEntity.Employee> filterlstEmployee = new List<WETOS.DataEntity.Employee>();
            List<DataEntity.Employee> lstTempEmployee = new List<WETOS.DataEntity.Employee>();
            DataEntity.Employee AddBlankEmployee = new WETOS.DataEntity.Employee();

            AddBlankEmployee.EmployeeId = ClientConstants.Negation;
            AddBlankEmployee.FullNameWithEmployeeCode = ClientConstants.SELECT;

            filterlstEmployee.AddRange(WETOSSession.GetAllEmployee);

            filterlstEmployee = (from filter in filterlstEmployee
                                 where filter.BranchId == ObjBranch.BranchId
                                 select filter).ToList();

            lstTempEmployee = filterlstEmployee;
            foreach (var item in listUser)
            {
                lstTempEmployee = (from chk in lstTempEmployee
                                   where chk.EmployeeId != item.EmployeeId
                                   select chk).ToList();
            }

            lstEmployee.Add(AddBlankEmployee);
            lstEmployee.AddRange(lstTempEmployee);

            ddlEmployeeName.DataValueField = ClientConstants.EmployeeId;
            ddlEmployeeName.DataTextField = ClientConstants.EmployeeName;
            ddlEmployeeName.DataSource = lstEmployee;
            ddlEmployeeName.DataBind();
        }
        public void FilterEmployeeNameWithOutCreatedOnBranchBasisTemp(DropDownList ddlEmployeeName, DataEntity.Branch ObjBranch, List<DataEntity.User> listUser)
        {
            List<DataEntity.Employee> lstEmployee = new List<WETOS.DataEntity.Employee>();
            List<DataEntity.Employee> filterlstEmployee = new List<WETOS.DataEntity.Employee>();
            List<DataEntity.Employee> lstTempEmployee = new List<WETOS.DataEntity.Employee>();
            DataEntity.Employee AddBlankEmployee = new WETOS.DataEntity.Employee();

            AddBlankEmployee.EmployeeId = ClientConstants.Negation;
            AddBlankEmployee.FullNameWithEmployeeCode = ClientConstants.SELECT;

            filterlstEmployee.AddRange(WETOSSession.TempGetAllEmployee);

            filterlstEmployee = (from filter in filterlstEmployee
                                 where filter.BranchId == ObjBranch.BranchId
                                 select filter).ToList();

            lstTempEmployee = filterlstEmployee;
            foreach (var item in listUser)
            {
                lstTempEmployee = (from chk in lstTempEmployee
                                   where chk.EmployeeId != item.EmployeeId
                                   select chk).ToList();
            }

            lstEmployee.Add(AddBlankEmployee);
            lstEmployee.AddRange(lstTempEmployee);

            ddlEmployeeName.DataValueField = ClientConstants.EmployeeId;
            ddlEmployeeName.DataTextField = ClientConstants.EmployeeName;
            ddlEmployeeName.DataSource = lstEmployee;
            ddlEmployeeName.DataBind();
        }
        public void FilterEmployeeNameWithOutCreatedOnDivisionBasisTemp(DropDownList ddlEmployeeName, DataEntity.Division ObjDivision, List<DataEntity.User> listUser)
        {
            List<DataEntity.Employee> lstEmployee = new List<WETOS.DataEntity.Employee>();
            List<DataEntity.Employee> filterlstEmployee = new List<WETOS.DataEntity.Employee>();
            List<DataEntity.Employee> lstTempEmployee = new List<WETOS.DataEntity.Employee>();
            DataEntity.Employee AddBlankEmployee = new WETOS.DataEntity.Employee();

            AddBlankEmployee.EmployeeId = ClientConstants.Negation;
            AddBlankEmployee.FullNameWithEmployeeCode = ClientConstants.SELECT;

            filterlstEmployee.AddRange(WETOSSession.TempGetAllEmployee);

            filterlstEmployee = (from filter in filterlstEmployee
                                 where  filter.DivisionId == ObjDivision.DivisionId
                                 select filter).ToList();

            lstTempEmployee = filterlstEmployee;
            foreach (var item in listUser)
            {
                lstTempEmployee = (from chk in lstTempEmployee
                                   where chk.EmployeeId != item.EmployeeId
                                   select chk).ToList();
            }

            lstEmployee.Add(AddBlankEmployee);
            lstEmployee.AddRange(lstTempEmployee);

            ddlEmployeeName.DataValueField = ClientConstants.EmployeeId;
            ddlEmployeeName.DataTextField = ClientConstants.EmployeeName;
            ddlEmployeeName.DataSource = lstEmployee;
            ddlEmployeeName.DataBind();
        }
        #endregion

        #region Bind EmployeeCode and EmployeeName on Department Basis
        public void FilterEmployeeCodeWithOutCreatedOnDepartmentBasisTemp(DropDownList ddlEmployeeName, DataEntity.Department ObjDepartment, List<DataEntity.User> listUser)
        {
            List<DataEntity.Employee> lstEmployee = new List<WETOS.DataEntity.Employee>();
            List<DataEntity.Employee> filterlstEmployee = new List<WETOS.DataEntity.Employee>();
            List<DataEntity.Employee> lstTempEmployee = new List<WETOS.DataEntity.Employee>();
            DataEntity.Employee AddBlankEmployee = new WETOS.DataEntity.Employee();

            AddBlankEmployee.EmployeeId = ClientConstants.Negation;
            AddBlankEmployee.EmployeeCode = ClientConstants.SELECT;

            filterlstEmployee.AddRange(WETOSSession.TempGetAllEmployee);

            filterlstEmployee = (from filter in filterlstEmployee
                                 where filter.DepartmentId == ObjDepartment.DepartmentId
                                 select filter).ToList();

            lstTempEmployee = filterlstEmployee;
            foreach (var item in listUser)
            {
                lstTempEmployee = (from chk in lstTempEmployee
                                   where chk.EmployeeId != item.EmployeeId
                                   select chk).ToList();
            }

            lstEmployee.Add(AddBlankEmployee);
            lstEmployee.AddRange(lstTempEmployee);

            ddlEmployeeName.DataValueField = ClientConstants.EmployeeId;
            ddlEmployeeName.DataTextField = ClientConstants.EmployeeCode;
            ddlEmployeeName.DataSource = lstEmployee;
            ddlEmployeeName.DataBind();
        }
        public void FilterEmployeeCodeWithOutCreatedOnDepartmentBasis(DropDownList ddlEmployeeName, DataEntity.Department ObjDepartment, List<DataEntity.User> listUser)
        {
            List<DataEntity.Employee> lstEmployee = new List<WETOS.DataEntity.Employee>();
            List<DataEntity.Employee> filterlstEmployee = new List<WETOS.DataEntity.Employee>();
            List<DataEntity.Employee> lstTempEmployee = new List<WETOS.DataEntity.Employee>();
            DataEntity.Employee AddBlankEmployee = new WETOS.DataEntity.Employee();

            AddBlankEmployee.EmployeeId = ClientConstants.Negation;
            AddBlankEmployee.EmployeeCode = ClientConstants.SELECT;

            filterlstEmployee.AddRange(WETOSSession.GetAllEmployee);

            filterlstEmployee = (from filter in filterlstEmployee
                                 where filter.DepartmentId == ObjDepartment.DepartmentId
                                 select filter).ToList();

            lstTempEmployee = filterlstEmployee;
            foreach (var item in listUser)
            {
                lstTempEmployee = (from chk in lstTempEmployee
                                   where chk.EmployeeId != item.EmployeeId
                                   select chk).ToList();
            }

            lstEmployee.Add(AddBlankEmployee);
            lstEmployee.AddRange(lstTempEmployee);

            ddlEmployeeName.DataValueField = ClientConstants.EmployeeId;
            ddlEmployeeName.DataTextField = ClientConstants.EmployeeCode;
            ddlEmployeeName.DataSource = lstEmployee;
            ddlEmployeeName.DataBind();
        }
        public void FilterEmployeeNameWithOutCreatedOnDepartmentBasisTemp(DropDownList ddlEmployeeName, DataEntity.Department ObjDepartment, List<DataEntity.User> listUser)
        {
            List<DataEntity.Employee> lstEmployee = new List<WETOS.DataEntity.Employee>();
            List<DataEntity.Employee> filterlstEmployee = new List<WETOS.DataEntity.Employee>();
            List<DataEntity.Employee> lstTempEmployee = new List<WETOS.DataEntity.Employee>();
            DataEntity.Employee AddBlankEmployee = new WETOS.DataEntity.Employee();

            AddBlankEmployee.EmployeeId = ClientConstants.Negation;
            AddBlankEmployee.FullNameWithEmployeeCode = ClientConstants.SELECT;

            filterlstEmployee.AddRange(WETOSSession.TempGetAllEmployee);

            filterlstEmployee = (from filter in filterlstEmployee
                                 where filter.DepartmentId == ObjDepartment.DepartmentId
                                 select filter).ToList();

            lstTempEmployee = filterlstEmployee;
            foreach (var item in listUser)
            {
                lstTempEmployee = (from chk in lstTempEmployee
                                   where chk.EmployeeId != item.EmployeeId
                                   select chk).ToList();
            }

            lstEmployee.Add(AddBlankEmployee);
            lstEmployee.AddRange(lstTempEmployee);

            ddlEmployeeName.DataValueField = ClientConstants.EmployeeId;
            ddlEmployeeName.DataTextField = ClientConstants.EmployeeName;
            ddlEmployeeName.DataSource = lstEmployee;
            ddlEmployeeName.DataBind();
        }
        public void FilterEmployeeNameWithOutCreatedOnDepartmentBasis(DropDownList ddlEmployeeName, DataEntity.Department ObjDepartment, List<DataEntity.User> listUser)
        {
            List<DataEntity.Employee> lstEmployee = new List<WETOS.DataEntity.Employee>();
            List<DataEntity.Employee> filterlstEmployee = new List<WETOS.DataEntity.Employee>();
            List<DataEntity.Employee> lstTempEmployee = new List<WETOS.DataEntity.Employee>();
            DataEntity.Employee AddBlankEmployee = new WETOS.DataEntity.Employee();

            AddBlankEmployee.EmployeeId = ClientConstants.Negation;
            AddBlankEmployee.FullNameWithEmployeeCode = ClientConstants.SELECT;

            filterlstEmployee.AddRange(WETOSSession.GetAllEmployee);

            filterlstEmployee = (from filter in filterlstEmployee
                                 where filter.DepartmentId == ObjDepartment.DepartmentId
                                 select filter).ToList();

            lstTempEmployee = filterlstEmployee;
            foreach (var item in listUser)
            {
                lstTempEmployee = (from chk in lstTempEmployee
                                   where chk.EmployeeId != item.EmployeeId
                                   select chk).ToList();
            }

            lstEmployee.Add(AddBlankEmployee);
            lstEmployee.AddRange(lstTempEmployee);

            ddlEmployeeName.DataValueField = ClientConstants.EmployeeId;
            ddlEmployeeName.DataTextField = ClientConstants.EmployeeName;
            ddlEmployeeName.DataSource = lstEmployee;
            ddlEmployeeName.DataBind();
        }
        #endregion

        #region Bind EmployeeCode and EmployeeName on Designation Basis
        public void FilterEmployeeCodeWithOutCreatedOnDesignationBasisTemp(DropDownList ddlEmployeeName, DataEntity.Designation ObjDesignation, List<DataEntity.User> listUser)
        {
            List<DataEntity.Employee> lstEmployee = new List<WETOS.DataEntity.Employee>();
            List<DataEntity.Employee> filterlstEmployee = new List<WETOS.DataEntity.Employee>();
            List<DataEntity.Employee> lstTempEmployee = new List<WETOS.DataEntity.Employee>();
            DataEntity.Employee AddBlankEmployee = new WETOS.DataEntity.Employee();

            AddBlankEmployee.EmployeeId = ClientConstants.Negation;
            AddBlankEmployee.EmployeeCode = ClientConstants.SELECT;

            filterlstEmployee.AddRange(WETOSSession.TempGetAllEmployee);

            filterlstEmployee = (from filter in filterlstEmployee
                                 where filter.DesignationId == ObjDesignation.DesignationId
                                 select filter).ToList();

            lstTempEmployee = filterlstEmployee;
            foreach (var item in listUser)
            {
                lstTempEmployee = (from chk in lstTempEmployee
                                   where chk.EmployeeId != item.EmployeeId
                                   select chk).ToList();
            }

            lstEmployee.Add(AddBlankEmployee);
            lstEmployee.AddRange(lstTempEmployee);

            ddlEmployeeName.DataValueField = ClientConstants.EmployeeId;
            ddlEmployeeName.DataTextField = ClientConstants.EmployeeCode;
            ddlEmployeeName.DataSource = lstEmployee;
            ddlEmployeeName.DataBind();
        }
        public void FilterEmployeeCodeWithOutCreatedOnDesignationBasis(DropDownList ddlEmployeeName, DataEntity.Designation ObjDesignation, List<DataEntity.User> listUser)
        {
            List<DataEntity.Employee> lstEmployee = new List<WETOS.DataEntity.Employee>();
            List<DataEntity.Employee> filterlstEmployee = new List<WETOS.DataEntity.Employee>();
            List<DataEntity.Employee> lstTempEmployee = new List<WETOS.DataEntity.Employee>();
            DataEntity.Employee AddBlankEmployee = new WETOS.DataEntity.Employee();

            AddBlankEmployee.EmployeeId = ClientConstants.Negation;
            AddBlankEmployee.EmployeeCode = ClientConstants.SELECT;

            filterlstEmployee.AddRange(WETOSSession.GetAllEmployee);

            filterlstEmployee = (from filter in filterlstEmployee
                                 where filter.DesignationId == ObjDesignation.DesignationId
                                 select filter).ToList();

            lstTempEmployee = filterlstEmployee;
            foreach (var item in listUser)
            {
                lstTempEmployee = (from chk in lstTempEmployee
                                   where chk.EmployeeId != item.EmployeeId
                                   select chk).ToList();
            }

            lstEmployee.Add(AddBlankEmployee);
            lstEmployee.AddRange(lstTempEmployee);

            ddlEmployeeName.DataValueField = ClientConstants.EmployeeId;
            ddlEmployeeName.DataTextField = ClientConstants.EmployeeCode;
            ddlEmployeeName.DataSource = lstEmployee;
            ddlEmployeeName.DataBind();
        }
        public void FilterEmployeeNameWithOutCreatedOnDesignationBasis(DropDownList ddlEmployeeName, DataEntity.Designation ObjDesignation, List<DataEntity.User> listUser)
        {
            List<DataEntity.Employee> lstEmployee = new List<WETOS.DataEntity.Employee>();
            List<DataEntity.Employee> filterlstEmployee = new List<WETOS.DataEntity.Employee>();
            List<DataEntity.Employee> lstTempEmployee = new List<WETOS.DataEntity.Employee>();
            DataEntity.Employee AddBlankEmployee = new WETOS.DataEntity.Employee();

            AddBlankEmployee.EmployeeId = ClientConstants.Negation;
            AddBlankEmployee.FullNameWithEmployeeCode = ClientConstants.SELECT;

            filterlstEmployee.AddRange(WETOSSession.GetAllEmployee);

            filterlstEmployee = (from filter in filterlstEmployee
                                 where filter.DesignationId == ObjDesignation.DesignationId
                                 select filter).ToList();

            lstTempEmployee = filterlstEmployee;
            foreach (var item in listUser)
            {
                lstTempEmployee = (from chk in lstTempEmployee
                                   where chk.EmployeeId != item.EmployeeId
                                   select chk).ToList();
            }

            lstEmployee.Add(AddBlankEmployee);
            lstEmployee.AddRange(lstTempEmployee);

            ddlEmployeeName.DataValueField = ClientConstants.EmployeeId;
            ddlEmployeeName.DataTextField = ClientConstants.EmployeeName;
            ddlEmployeeName.DataSource = lstEmployee;
            ddlEmployeeName.DataBind();
        }
        public void FilterEmployeeNameWithOutCreatedOnDesignationBasisTemp(DropDownList ddlEmployeeName, DataEntity.Designation ObjDesignation, List<DataEntity.User> listUser)
        {
            List<DataEntity.Employee> lstEmployee = new List<WETOS.DataEntity.Employee>();
            List<DataEntity.Employee> filterlstEmployee = new List<WETOS.DataEntity.Employee>();
            List<DataEntity.Employee> lstTempEmployee = new List<WETOS.DataEntity.Employee>();
            DataEntity.Employee AddBlankEmployee = new WETOS.DataEntity.Employee();

            AddBlankEmployee.EmployeeId = ClientConstants.Negation;
            AddBlankEmployee.FullNameWithEmployeeCode = ClientConstants.SELECT;

            filterlstEmployee.AddRange(WETOSSession.TempGetAllEmployee);

            filterlstEmployee = (from filter in filterlstEmployee
                                 where filter.DesignationId == ObjDesignation.DesignationId
                                 select filter).ToList();

            lstTempEmployee = filterlstEmployee;
            foreach (var item in listUser)
            {
                lstTempEmployee = (from chk in lstTempEmployee
                                   where chk.EmployeeId != item.EmployeeId
                                   select chk).ToList();
            }

            lstEmployee.Add(AddBlankEmployee);
            lstEmployee.AddRange(lstTempEmployee);

            ddlEmployeeName.DataValueField = ClientConstants.EmployeeId;
            ddlEmployeeName.DataTextField = ClientConstants.EmployeeName;
            ddlEmployeeName.DataSource = lstEmployee;
            ddlEmployeeName.DataBind();
        }
        #endregion

        #endregion

        #region Bind EmployeeGroup DropDown
        public void BindEmployeeGrpDropdownList(DropDownList ddlEmpGrpName)
        {
            List<DataEntity.EmployeeGroup> lstEmployeeGroup = new List<WETOS.DataEntity.EmployeeGroup>();
            DataEntity.EmployeeGroup AddBlankEmployeeGroup = new WETOS.DataEntity.EmployeeGroup();

            AddBlankEmployeeGroup.EmployeeGroupId = ClientConstants.Negation;
            AddBlankEmployeeGroup.EmployeeGroupName = ClientConstants.SELECT;

            lstEmployeeGroup.Add(AddBlankEmployeeGroup);
            lstEmployeeGroup.AddRange(WETOSSession.GetAllGroup);

            ddlEmpGrpName.DataValueField = ClientConstants.EmployeeGroupId;
            ddlEmpGrpName.DataTextField = ClientConstants.EmployeeGroupName;
            ddlEmpGrpName.DataSource = lstEmployeeGroup;
            ddlEmpGrpName.DataBind();
        }
        public void BindEmployeeGrpDropdownListOnSelectedBranch(DropDownList ddlEmpGrpName,DataEntity.Branch objBranch)
        {
            
            List<DataEntity.EmployeeGroup> lstEmployeeGroup = new List<WETOS.DataEntity.EmployeeGroup>();
            List<DataEntity.EmployeeGroup> lstTempFilterList = new List<WETOS.DataEntity.EmployeeGroup>();
            DataEntity.EmployeeGroup AddBlankEmployeeGroup = new WETOS.DataEntity.EmployeeGroup();

            AddBlankEmployeeGroup.EmployeeGroupId = ClientConstants.Negation;
            AddBlankEmployeeGroup.EmployeeGroupName = ClientConstants.ALL;


            lstTempFilterList.AddRange(WETOSSession.GetAllGroup);
            //lstTempFilterList = (from chk in lstTempFilterList
            //                  where chk.BranchId==objBranch.BranchId
            //                  select chk).ToList();
            lstEmployeeGroup.Add(AddBlankEmployeeGroup);
            lstEmployeeGroup.AddRange(lstTempFilterList);
            ddlEmpGrpName.DataValueField = ClientConstants.EmployeeGroupId;
            ddlEmpGrpName.DataTextField = ClientConstants.EmployeeGroupName;
            ddlEmpGrpName.DataSource = lstEmployeeGroup;
            ddlEmpGrpName.DataBind();
        }
        #endregion end BindDropDown

        #region Bind Shift DropDownList
        public void BindShiftDropDown(DropDownList ddlShiftCode)
        {
            List<DataEntity.Shift> lstShift = new List<WETOS.DataEntity.Shift>();
            DataEntity.Shift AddBlankShift = new WETOS.DataEntity.Shift();

            AddBlankShift.ShiftId = ClientConstants.SELECT;
            AddBlankShift.ShiftName = ClientConstants.SELECT;

            lstShift.Add(AddBlankShift);
            lstShift.AddRange(WETOSSession.TempGetAllShifts);

            ddlShiftCode.DataValueField = ClientConstants.ShiftId;
            ddlShiftCode.DataTextField = ClientConstants.ShiftId;
            ddlShiftCode.DataSource = lstShift;
            ddlShiftCode.DataBind();
        }

        public void BindShiftDropDownForShiftSchedule(DropDownList ddlShiftCode,List<DataEntity.Shift> Shift)
        {
            ddlShiftCode.Items.Clear();
            List<DataEntity.Shift> lstShift = new List<WETOS.DataEntity.Shift>();
            DataEntity.Shift AddBlankShift = new WETOS.DataEntity.Shift();

            AddBlankShift.ShiftId = ClientConstants.BLANKVAL;
            AddBlankShift.ShiftName = ClientConstants.BLANK ;

            lstShift.Add(AddBlankShift);
            lstShift.AddRange(Shift);

            ddlShiftCode.DataValueField = ClientConstants.ShiftId;
            ddlShiftCode.DataTextField = ClientConstants.ShiftId;
            ddlShiftCode.DataSource = lstShift;
            ddlShiftCode.DataBind();
        }

        public void BindShiftDropDownForCondone(DropDownList ddlShiftCode, List<DataEntity.Shift> Shift)
        {
            ddlShiftCode.Items.Clear();
            List<DataEntity.Shift> lstShift = new List<WETOS.DataEntity.Shift>();
            DataEntity.Shift AddBlankShift = new WETOS.DataEntity.Shift();

            AddBlankShift.ShiftId = ClientConstants.SELECT;
            AddBlankShift.ShiftName = ClientConstants.SELECT;

            lstShift.Add(AddBlankShift);
            lstShift.AddRange(Shift);

            ddlShiftCode.DataValueField = ClientConstants.ShiftId;
            ddlShiftCode.DataTextField = ClientConstants.ShiftId;
            ddlShiftCode.DataSource = lstShift;
            ddlShiftCode.DataBind();
        }

        public void BindShiftDropDownOnBranch(DropDownList ddlShiftCode, DataEntity.Branch ObjBranch)
        {
            List<DataEntity.Shift> lstShift = new List<WETOS.DataEntity.Shift>();
            List<DataEntity.Shift> filterlstShift = new List<WETOS.DataEntity.Shift>();

            DataEntity.Shift AddBlankShift = new WETOS.DataEntity.Shift();

            AddBlankShift.ShiftId = ClientConstants.SELECT;
            AddBlankShift.ShiftId = ClientConstants.SELECT;

            filterlstShift.AddRange(WETOSSession.TempGetAllShifts);

            filterlstShift = (from filter in filterlstShift
                              where filter.BranchId == ObjBranch.BranchId
                              select filter).ToList();

            lstShift.Add(AddBlankShift);
            lstShift.AddRange(filterlstShift);

            ddlShiftCode.DataValueField = ClientConstants.ShiftId;
            ddlShiftCode.DataTextField = ClientConstants.ShiftId;
            ddlShiftCode.DataSource = lstShift;
            ddlShiftCode.DataBind();
        }
        #endregion end BindDropDown

        #region Bind Religion Drop Down
        public void BindReligionDropDown(DropDownList ddlReligion)
        {
            List<DataEntity.Religion> lstReligion = new List<WETOS.DataEntity.Religion>();
            DataEntity.Religion AddBlankReligion = new WETOS.DataEntity.Religion();

            AddBlankReligion.ReligionId = ClientConstants.Negation;
            AddBlankReligion.ReligionName = ClientConstants.SELECT;

            lstReligion.Add(AddBlankReligion);
            lstReligion.AddRange(WETOSSession.GetAllReligion);

            ddlReligion.DataValueField = ClientConstants.ReligionId;
            ddlReligion.DataTextField = ClientConstants.ReligionName;
            ddlReligion.DataSource = lstReligion;
            ddlReligion.DataBind();
        }
        #endregion

        #region Bind Financial Year Drop Down
        public void BindFinancialYearDropDown(DropDownList ddlFinancialYear,List<DataEntity.FinancialYear> financialYear)
        {
            List<DataEntity.FinancialYear> lstFinancialYear = new List<WETOS.DataEntity.FinancialYear>();
            DataEntity.FinancialYear AddBlankFinancialYear = new WETOS.DataEntity.FinancialYear();

            AddBlankFinancialYear.FinancialId = ClientConstants.Negation;
            AddBlankFinancialYear.FinancialName = ClientConstants.SELECT;

            lstFinancialYear.Add(AddBlankFinancialYear);
            lstFinancialYear.AddRange(financialYear);

            ddlFinancialYear.DataValueField = ClientConstants.FinancialId;
            ddlFinancialYear.DataTextField = ClientConstants.FinancialName;
            ddlFinancialYear.DataSource = lstFinancialYear;
            ddlFinancialYear.DataBind();
        }
        #endregion

        #region Bind Shift Rotations
        public void BindDropDownShiftRotation(DropDownList ddlShiftRotation)
        {
            try
            {
                List<DataEntity.ShiftRotation> lstShiftRotation = new List<WETOS.DataEntity.ShiftRotation>();
                DataEntity.ShiftRotation AddBlankShiftRotation = new WETOS.DataEntity.ShiftRotation();

                AddBlankShiftRotation.RotationId = ClientConstants.Negation;
                AddBlankShiftRotation.RotationName = ClientConstants.SELECT;

                lstShiftRotation.Add(AddBlankShiftRotation);
                lstShiftRotation.AddRange(WETOSSession.GetAllShiftsRotations);

                ddlShiftRotation.DataValueField = ClientConstants.RotationId;
                ddlShiftRotation.DataTextField = ClientConstants.RotationName;

                ddlShiftRotation.DataSource = lstShiftRotation;
                ddlShiftRotation.DataBind();
            }
            catch
            {
                throw;
            }


        }
        #endregion

        #region Bind Shift Rotations On Branch Basis

        public void FilterShiftRotationOnBranch(DropDownList ddlShiftRotation, DataEntity.Branch ObjBranch)
        {
            List<DataEntity.ShiftRotation> lstShiftRotation = new List<WETOS.DataEntity.ShiftRotation>();
            List<DataEntity.ShiftRotation> filterlstShiftRotation = new List<WETOS.DataEntity.ShiftRotation>();

            DataEntity.ShiftRotation AddBlankShiftRotation = new WETOS.DataEntity.ShiftRotation();

            AddBlankShiftRotation.RotationId = ClientConstants.Negation;
            AddBlankShiftRotation.RotationName = ClientConstants.SELECT;

            filterlstShiftRotation.AddRange(WETOSSession.GetAllShiftsRotations);

            filterlstShiftRotation = (from filter in filterlstShiftRotation
                                      where filter.BrachId == ObjBranch.BranchId
                                      select filter).ToList();

            lstShiftRotation.Add(AddBlankShiftRotation);
            lstShiftRotation.AddRange(filterlstShiftRotation);

            ddlShiftRotation.DataValueField = ClientConstants.RotationId;
            ddlShiftRotation.DataTextField = ClientConstants.RotationName;
            ddlShiftRotation.DataSource = lstShiftRotation;
            ddlShiftRotation.DataBind();

        }

        #endregion
    }
}
