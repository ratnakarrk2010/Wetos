using System.Web.UI.WebControls;
using System.Collections.Generic;
using System.Linq;

namespace WetosMVCMainApp.Utilities
{
    public class BindControls
    {
        #region BindDropDown Roles
        public void BindDropDownRoles(DropDownList ddlRoleType)
        {
            List<WETOS.DataEntity.Roles> lstRoles = new List<WETOS.DataEntity.Roles>();
            DataEntity.Roles AddBlankRoles = new WETOS.DataEntity.Roles();

            AddBlankRoles.RoleId = ClientConstants.Negation;
            AddBlankRoles.RoleName = ClientConstants.SELECT;

            lstRoles.Add(AddBlankRoles);
            lstRoles.AddRange(WETOSSession.GetAllRoles);

            ddlRoleType.DataValueField = ClientConstants.RoleId;
            ddlRoleType.DataTextField = ClientConstants.RoleName;
            ddlRoleType.DataSource = lstRoles;
            ddlRoleType.DataBind();
        }
        #endregion

        #region BindListBox
        public void BindListBoxLocation(ListBox lbLocation)
        {
            List<WETOS.DataEntity.Location> lstLocation = new List<WETOS.DataEntity.Location>();
            DataEntity.Location AddBlankLocation = new WETOS.DataEntity.Location();

            lstLocation.AddRange(WETOSSession.GetAllLocation);

            lbLocation.DataValueField = ClientConstants.LocationId;
            lbLocation.DataTextField = ClientConstants.LocationName;
            lbLocation.DataSource = lstLocation;
            lbLocation.DataBind();
        }

        public void BindListBoxLocation(ListBox lbLocation,List<DataEntity.Location> lstAllLocations)
        {
            List<WETOS.DataEntity.Location> lstLocation = new List<WETOS.DataEntity.Location>();
            DataEntity.Location AddBlankLocation = new WETOS.DataEntity.Location();

            lstLocation.AddRange(lstAllLocations);

            lbLocation.DataValueField = ClientConstants.LocationId;
            lbLocation.DataTextField = ClientConstants.LocationName;
            lbLocation.DataSource = lstLocation;
            lbLocation.DataBind();
        }
       
        public void BindListBoxCompany(ListBox lbCompany)
        {
            List<DataEntity.Company> lstCompany = new List<WETOS.DataEntity.Company>();
            DataEntity.Company AddBlankCompany = new WETOS.DataEntity.Company();

            lstCompany.AddRange(WETOSSession.GetAllCompany);

            lbCompany.DataValueField = ClientConstants.CompanyId;
            lbCompany.DataTextField = ClientConstants.CompanyName;
            lbCompany.DataSource = lstCompany;
            lbCompany.DataBind();
        }

        public void BindListBoxCompany(ListBox lbCompany,List<DataEntity.Company> lstAllCompanies)
        {
            List<DataEntity.Company> lstCompany = new List<WETOS.DataEntity.Company>();
            DataEntity.Company AddBlankCompany = new WETOS.DataEntity.Company();

            lstCompany.AddRange(lstAllCompanies);
            lbCompany.DataValueField = ClientConstants.CompanyId;
            lbCompany.DataTextField = ClientConstants.CompanyName;
            lbCompany.DataSource = lstCompany;
            lbCompany.DataBind();
        }

        public void BindListBoxBranch(ListBox lbBranch)
        {
            List<DataEntity.Branch> lstBranch = new List<WETOS.DataEntity.Branch>();
            DataEntity.Branch AddBlankBranch = new WETOS.DataEntity.Branch();

            lstBranch.AddRange(WETOSSession.GetAllBranch);

            lbBranch.DataValueField = ClientConstants.BranchId;
            lbBranch.DataTextField = ClientConstants.BranchName;
            lbBranch.DataSource = lstBranch;
            lbBranch.DataBind();

        }

        public void BindListBoxBranch(ListBox lbBranch,List<DataEntity.Branch> lstAllBranches)
        {
            List<DataEntity.Branch> lstBranch = new List<WETOS.DataEntity.Branch>();
            DataEntity.Branch AddBlankBranch = new WETOS.DataEntity.Branch();
             lstBranch.AddRange(lstAllBranches);
             lbBranch.DataValueField = ClientConstants.BranchId;
            lbBranch.DataTextField = ClientConstants.BranchName;
            lbBranch.DataSource = lstBranch;
            lbBranch.DataBind();

        }
        public void BindCheckListBoxBranch(CheckBoxList lbBranch, List<DataEntity.Branch> lstAllBranches)
        {
            List<DataEntity.Branch> lstBranch = new List<WETOS.DataEntity.Branch>();
            DataEntity.Branch AddBlankBranch = new WETOS.DataEntity.Branch();
            lstBranch.AddRange(lstAllBranches);
            lbBranch.DataValueField = ClientConstants.BranchId;
            lbBranch.DataTextField = ClientConstants.BranchName;
            lbBranch.DataSource = lstBranch;
            lbBranch.DataBind();

        }
        public void BindListBoxDepartment(ListBox lbDepartment)
        {
            List<DataEntity.Department> lstDepartment = new List<WETOS.DataEntity.Department>();
            DataEntity.Department AddBlankDepartment = new WETOS.DataEntity.Department();

           
            lstDepartment.AddRange(WETOSSession.GetAllDepartment);

            lbDepartment.DataValueField = ClientConstants.DepartmentId;
            lbDepartment.DataTextField = ClientConstants.DepartmentName;
            lbDepartment.DataSource = lstDepartment;
            lbDepartment.DataBind(); 
        }
        public void BindListBoxDepartment(ListBox lbDepartment,List<DataEntity.Department> lstAllDepts)
        {
            List<DataEntity.Department> lstDepartment = new List<WETOS.DataEntity.Department>();
            DataEntity.Department AddBlankDepartment = new WETOS.DataEntity.Department();
             lstDepartment.AddRange(lstAllDepts);
             lbDepartment.DataValueField = ClientConstants.DepartmentId;
            lbDepartment.DataTextField = ClientConstants.DepartmentName;
            lbDepartment.DataSource = lstDepartment;
            lbDepartment.DataBind();
        }
        public void BindCheckListBoxDepartment(CheckBoxList lbDepartment, List<DataEntity.Department> lstAllDepts)
        {
            List<DataEntity.Department> lstDepartment = new List<WETOS.DataEntity.Department>();
            DataEntity.Department AddBlankDepartment = new WETOS.DataEntity.Department();
            lstDepartment.AddRange(lstAllDepts);
            lbDepartment.DataValueField = ClientConstants.DepartmentId;
            lbDepartment.DataTextField = ClientConstants.DepartmentName;
            lbDepartment.DataSource = lstDepartment;
            lbDepartment.DataBind();
        }
        public void BindListBoxEmployee(ListBox lstEmployee, List<DataEntity.Employee> lstAllEmployee)
        {
            List<DataEntity.Employee> lstFilteredEmployee = new List<WETOS.DataEntity.Employee>();
        
            lstFilteredEmployee.AddRange(lstAllEmployee);
            lstEmployee.DataValueField = ClientConstants.EmployeeCode;
            lstEmployee.DataTextField = ClientConstants.EmpCodeAndName;
            lstEmployee.DataSource = lstAllEmployee;
            lstEmployee.DataBind();
        }
        #endregion

       
        
        
        public void BindListBoxCenter(ListBox lbCenter, List<DataEntity.Division> lstAllCenter)
        {
            List<DataEntity.Division> lstCenter = new List<WETOS.DataEntity.Division>();
            lstCenter.AddRange(lstAllCenter);
            lbCenter.Items.Clear();
            lbCenter.DataValueField = ClientConstants.DivisionId;
            lbCenter.DataTextField = ClientConstants.DivisionName;
            lbCenter.DataSource = lstCenter;
            lbCenter.DataBind();
        }

        public void BindListBoxEmployeeType(ListBox lbEmployeeType, List<DataEntity.EmployeeType> lstAllEmployeeType)
        {
            List<DataEntity.EmployeeType> lstEmployeeType = new List<WETOS.DataEntity.EmployeeType>();
            lstEmployeeType.AddRange(lstAllEmployeeType);
            lbEmployeeType.Items.Clear();
            lbEmployeeType.DataValueField = ClientConstants.EmployeeTypeId;
            lbEmployeeType.DataTextField = ClientConstants.EmployeeTypeName;
            lbEmployeeType.DataSource = lstEmployeeType;
            lbEmployeeType.DataBind();
        }

        public void BindListBoxEmployeeType(ListBox lbShift, List<DataEntity.Shift> lstAllShift)
        {
            List<DataEntity.Shift> lstShift = new List<WETOS.DataEntity.Shift>();
            lstShift.AddRange(lstAllShift);
            lbShift.Items.Clear();
            lbShift.DataValueField = ClientConstants.ShiftId;
            lbShift.DataTextField = ClientConstants.ShiftName;
            lbShift.DataSource = lstShift;
            lbShift.DataBind();
        }
        public void BindListBoxEmployeeGroup(ListBox lbEmployeeGroup, List<DataEntity.EmployeeGroup> lstAllEmployeeGroup)
        {
            List<DataEntity.EmployeeGroup> lstEmployeeGroup = new List<WETOS.DataEntity.EmployeeGroup>();
            lstEmployeeGroup.AddRange(lstAllEmployeeGroup);
            lbEmployeeGroup.Items.Clear();
            lbEmployeeGroup.DataValueField = ClientConstants.EmployeeGroupId;
            lbEmployeeGroup.DataTextField = ClientConstants.EmployeeGroupName;
            lbEmployeeGroup.DataSource = lstEmployeeGroup;
            lbEmployeeGroup.DataBind();
        }
        public void BindCheckListBoxEmployeeGroup(CheckBoxList lbEmployeeGroup, List<DataEntity.EmployeeGroup> lstAllEmployeeGroup)
        {
            List<DataEntity.EmployeeGroup> lstEmployeeGroup = new List<WETOS.DataEntity.EmployeeGroup>();
            lstEmployeeGroup.AddRange(lstAllEmployeeGroup);
            lbEmployeeGroup.Items.Clear();
            lbEmployeeGroup.DataValueField = ClientConstants.EmployeeGroupId;
            lbEmployeeGroup.DataTextField = ClientConstants.EmployeeGroupName;
            lbEmployeeGroup.DataSource = lstEmployeeGroup;
            lbEmployeeGroup.DataBind();
        }
        public void BindListBoxDivision(ListBox lbDivision, List<DataEntity.Division> lstAllDivision)
        {
            List<DataEntity.Division> lstDivision = new List<WETOS.DataEntity.Division>();
            lstDivision.AddRange(lstAllDivision);
            lbDivision.Items.Clear();
            lbDivision.DataValueField = ClientConstants.DesignationId;
            lbDivision.DataTextField = ClientConstants.DesignationName;
            lbDivision.DataSource = lstDivision;
            lbDivision.DataBind();
        }
        public void BindCheckListBoxDivision(CheckBoxList lbDivision, List<DataEntity.Division> lstAllDivision)
        {
            List<DataEntity.Division> lstDivision = new List<WETOS.DataEntity.Division>();
            lstDivision.AddRange(lstAllDivision);
            lbDivision.Items.Clear();
            lbDivision.DataValueField = ClientConstants.DivisionId;
            lbDivision.DataTextField = ClientConstants.DivisionName;
            lbDivision.DataSource = lstDivision;
            lbDivision.DataBind();
        }
        public void BindCheckListBoxDesignation(CheckBoxList lbDesignation, List<DataEntity.Designation> lstAllDesiognation)
        {
            List<DataEntity.Designation> lstDesignation = new List<WETOS.DataEntity.Designation>();
            lstDesignation.AddRange(lstAllDesiognation);
            lbDesignation.Items.Clear();
            lbDesignation.DataValueField = ClientConstants.DesignationId;
            lbDesignation.DataTextField = ClientConstants.DesignationName;
            lbDesignation.DataSource = lstDesignation;
            lbDesignation.DataBind();
        }
        public void BindCheckListBoxGrade(CheckBoxList lbGrade, List<DataEntity.Grade> lstAllGrade)
        {
            List<DataEntity.Grade> lstGrade = new List<WETOS.DataEntity.Grade>();
            lstGrade.AddRange(lstAllGrade);
            lbGrade.Items.Clear();
            lbGrade.DataValueField = ClientConstants.GradeId;
            lbGrade.DataTextField = ClientConstants.GradeName;
            lbGrade.DataSource = lstGrade;
            lbGrade.DataBind();
        }
        public void BindListBoxFullEmployeeName(ListBox lstEmployee, List<DataEntity.Employee> lstAllEmployee)
        {
            List<DataEntity.Employee> lstFilteredEmployee = new List<WETOS.DataEntity.Employee>();
            // DataEntity.Employee AddBlankDepartment = new WETOS.DataEntity.Employee();
            lstFilteredEmployee.AddRange(lstAllEmployee);
            lstEmployee.DataValueField = ClientConstants.EmployeeCode;
            lstEmployee.DataTextField = ClientConstants.EmployeeName;
            lstEmployee.DataSource = lstAllEmployee;
            lstEmployee.DataBind();
        }

        

        #region BindCheckBoxList
        public void BindCheckBoxListRights(CheckBoxList checkboxList)
        {
            List<DataEntity.Rights> lstRights = new List<WETOS.DataEntity.Rights>();
            DataEntity.Rights AddBlankRights = new WETOS.DataEntity.Rights();

            lstRights.AddRange(WETOSSession.GetAllRights);


            checkboxList.DataValueField = ClientConstants.RightsId;
            checkboxList.DataTextField = ClientConstants.RightsName;
            checkboxList.DataSource = lstRights;
            checkboxList.DataBind();
        }

        #endregion
    }
}
