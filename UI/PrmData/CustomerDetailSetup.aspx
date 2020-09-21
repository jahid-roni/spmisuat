<%@ Page Title="" Language="C#" MasterPageFile="~/mp/site.master" AutoEventWireup="true"
    CodeBehind="CustomerDetailSetup.aspx.cs" Inherits="CustomerDetailSetup" %>

<%@ Register Src="~/UI/UC/UCUserDetails.ascx" TagName="UserDetails" TagPrefix="uc1" %>
<%@ Register Src="~/UI/UC/UCMessage.ascx" TagName="Error" TagPrefix="uc3" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">

    <script language="javascript" type="text/javascript">
        function pageLoad(sender, args) {
            $('input[id*=Date]').each(function() { $(this).mask("99/99/9999"); });
            $('input[id*=txtCustomerID]').keypress(function(e) { return intNumber(e); });
            $('input[id*=txtPhone]').keypress(function(e) { return textPhoneFax(e); });
            $('input[id*=txtPhone2]').keypress(function(e) { return textPhoneFax(e); });
        }

        function RejectValidation() {
            var sErrorList = "";

            sErrorList += RequiredData('ctl00_cphDet_ucUserDet_txtCheckerComments', 'TextBox', "Checker Comments cannot be empty!");

            return OpenErrorPanel(sErrorList, 'Reject');
        }

        function Reset() {
            ResetData('<%=txtMasterNo.ClientID %>');
            ResetData('<%=txtCustomerID.ClientID %>');
            var obj = document.getElementById('<%=txtCustomerID.ClientID %>');
            if (obj != null) {
                obj.readOnly = false;
            }
            ResetData('<%=txtCustomerName.ClientID %>');
            ResetData('<%=txtAddress.ClientID %>');
            ResetData('<%=txtPhone.ClientID %>');
            ResetData('<%=txtNationality.ClientID %>');
            ResetData('<%=txtForignAddress.ClientID %>');
            ResetData('<%=txtDateofBirth.ClientID %>');
            ResetData('<%=ddlSex.ClientID %>');
            ResetData('<%=txtPassportNo.ClientID %>');
            ResetData('<%=txtIssueAt.ClientID %>');
            ResetData('<%=txtNationalID.ClientID %>');
            ResetData('<%=txtBirthCertNo.ClientID %>');
            ResetData('<%=txtEmail.ClientID %>');

            ResetData('<%=txtCustomerName2.ClientID %>');
            ResetData('<%=txtAddress2.ClientID %>');
            ResetData('<%=txtForignAddress2.ClientID %>');
            ResetData('<%=txtPhone2.ClientID %>');
            ResetData('<%=txtDateofBirth2.ClientID %>');
            ResetData('<%=ddlSex2.ClientID %>');
            ResetData('<%=txtNationality2.ClientID %>');
            ResetData('<%=txtPassportNo2.ClientID %>');
            ResetData('<%=txtIssueAt2.ClientID %>');
            ResetData('<%=txtNationalID2.ClientID %>');
            ResetData('<%=txtBirthCertNo2.ClientID %>');
            ResetData('<%=txtEmail2.ClientID %>');

            ResetData('ctl00_cphDet_ucUserDet_txtCheckerId');
            ResetData('ctl00_cphDet_ucUserDet_txtCheckerComments');
            ResetData('ctl00_cphDet_ucUserDet_txtMakeDate');
            ResetData('ctl00_cphDet_ucUserDet_txtCheckDate');

            var currentDt = new Date();
            var mm = currentDt.getMonth() + 1;
            mm = (mm < 10) ? '0' + mm : mm;
            var dd = currentDt.getDate();
            dd = (dd < 10) ? '0' + dd : dd;
            var yyyy = currentDt.getFullYear();
            var date = mm + '/' + dd + '/' + yyyy;

            var makeDate = document.getElementById('ctl00_cphDet_ucUserDet_txtMakeDate')
            makeDate.value = date;

            //ResetData('<%=hdCustomerID.ClientID %>');

            return false;
        }

        function SaveValidation() {

            var sErrorList = "";
            sErrorList += RequiredData('<%=txtCustomerName.ClientID %>', 'TextBox', "Customer Name cannot be empty!");
            sErrorList += RequiredData('<%=txtAddress.ClientID %>', 'TextBox', "Address cannot be empty!");
            sErrorList += RequiredData('<%=txtPhone.ClientID %>', 'TextBox', "Phone cannot be empty!");
            sErrorList += RequiredData('<%=txtNationality.ClientID %>', 'TextBox', "Nationality cannot be empty!");
            sErrorList += RequiredData('<%=txtForignAddress.ClientID %>', 'TextBox', "Forign Address cannot be empty!");
            sErrorList += RequiredData('<%=txtDateofBirth.ClientID %>', 'TextBox', "Date of Birth cannot be empty!");
            sErrorList += RequiredData('<%=txtIssueAt.ClientID %>', 'TextBox', "Issue At cannot be empty!");
            sErrorList += RequiredData('<%=txtEmail.ClientID %>', 'Email', "Email ID cannot be empty!");

            var oPassportNo = document.getElementById('<%=txtPassportNo.ClientID %>');
            var oBirthCertNo = document.getElementById('<%=txtBirthCertNo.ClientID %>');
            var oNationalID = document.getElementById('<%=txtNationalID.ClientID %>');
            if (oPassportNo != null && oBirthCertNo != null && oNationalID != null) {
                if (oPassportNo.value == "" && oBirthCertNo.value == "" && oNationalID.value == "") {
                    sErrorList += RequiredData('<%=txtPassportNo.ClientID %>', 'TextBox', "Passport No cannot be empty!");
                    sErrorList += RequiredData('<%=txtBirthCertNo.ClientID %>', 'TextBox', "Birth certificate No cannot be empty!");
                    sErrorList += RequiredData('<%=txtNationalID.ClientID %>', 'TextBox', "National ID cannot be empty!");
                }
            }
            return OpenErrorPanel(sErrorList, 'Save');
        }


        function Delete() {
            var divErrorPanel = document.getElementById('divErrorPanel');
            var hdCustomerID = document.getElementById('<%=hdCustomerID.ClientID %>');
            if (hdCustomerID.value != "") {
                divErrorPanel.style.display = "none";
                if (CheckForDelete('this customer detail')) {
                    MsgPopupReturnTrue('Delete');
                    return true;
                } else {
                    return false;
                }
            }
            else {
                // show error divErrorList                
                var errorList = document.getElementById('divErrorList');
                errorList.innerHTML = "<ul><li>Customer detail has not been selected</li></ul>";
                divErrorPanel.style.display = "block";
                window.scroll(0, 0);
                return false;
                // end of show error divErroList
            }
        }

        function LoadValidation(type) {
            var sErrorList = "";
            if (type == 'M') {
                sErrorList += RequiredData('<%=txtMasterNo.ClientID %>', 'TextBox', "Master ID cannot be empty!");
            }
            else {
                sErrorList += RequiredData('<%=txtCustomerID.ClientID %>', 'TextBox', "Customer ID cannot be empty!");
            }

            return OpenErrorPanel(sErrorList, '');
        }
    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphDet" runat="server">
    <%-- Error --%>
    <div id="divErrorPanel" style="display: none">
        <fieldset class="errorFieldset">
            <legend class="errorLegend">Sorry, there were problems with your input</legend>
            <div class="errorBox" id="divErrorList">
            </div>
        </fieldset>
        <br />
    </div>
    <%-- Error --%>
    <fieldset runat="server" id="fsList">
        <legend>Unapproved Customer Accounts</legend>
        <table width="100%" align="center" class="tableBody" border="0">
            <tr>
                <td>
                    <asp:UpdatePanel runat="server" ID="UpdatePanel1">
                        <ContentTemplate>
                            <asp:HiddenField runat="server" ID="hdCustomerID" />
                            <asp:GridView Width="98%" OnRowCommand="gvList_RowCommand" OnRowDataBound="gvList_RowDataBound"
                                ID="gvList" runat="server" AutoGenerateColumns="true" SkinID="SBMLGridGreen"
                                ShowHeader="true" AllowPaging="True" OnPageIndexChanging="gvList_PageIndexChanging"  >
                                <Columns>
                                    <asp:TemplateField HeaderText="Select" HeaderStyle-Width="5%">
                                        <ItemTemplate>
                                            <asp:Button CssClass="ButtonAsh" CommandName="Select" runat="server" ID="btnSelect"
                                                OnClientClick="CloseErrorPanel()" Text="Select" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                                <EmptyDataTemplate>
                                    No record found
                                </EmptyDataTemplate>
                                <AlternatingRowStyle CssClass="odd" />
                            </asp:GridView>
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger EventName="Click" ControlID="btnSave" />
                            <asp:AsyncPostBackTrigger EventName="Click" ControlID="btnReject" />
                            <asp:AsyncPostBackTrigger EventName="Click" ControlID="btnDelete" />
                            <asp:AsyncPostBackTrigger EventName="Click" ControlID="btnApprove" />
                            <asp:AsyncPostBackTrigger EventName="Click" ControlID="btnCustomerLoad" />
                            <asp:AsyncPostBackTrigger EventName="Click" ControlID="btnMasterLoad" />
                        </Triggers>
                    </asp:UpdatePanel>
                </td>
            </tr>
        </table>
    </fieldset>
    <br />
    <fieldset>
        <legend>Master Information</legend>
        <asp:UpdatePanel runat="server" ID="upData">
            <ContentTemplate>
                <table width="95%" align="center" class="tableBody" border="0">
                    <tr>
                        <td align="right">
                            Master No
                        </td>
                        <td>
                            <div class="fieldLeft">
                                <asp:TextBox ID="txtMasterNo" runat="server" MaxLength="7" CssClass="textInput" onblur="blurActiveInputWithObj(this)"
                                    onfocus="highlightActiveInputWithObj(this),CloseErrorPanel()" Width="120px"></asp:TextBox>
                            </div>
                            <div class="errorIcon">
                                *</div>
                            <div class="searchButton">
                                <asp:Button ID="btnMasterLoad" OnClientClick="return LoadValidation('M')" OnClick="btnMasterLoad_Click"
                                    runat="server" CssClass="ButtonAsh" Text="Search" />
                            </div>
                        </td>
                        <td>
                            <asp:CheckBox Text="Walk-in Customer" ID="chkWalkInCustomer" runat="server" />
                        </td>
                        <td>
                            <asp:Button CssClass="ButtonAsh" runat="server" Text="Update from EBBS" ID="Button1" />
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            Customer ID
                        </td>
                        <td colspan="3">
                            <div class="fieldLeft">
                                <asp:TextBox ID="txtCustomerID" runat="server" MaxLength="20" CssClass="textInput" onblur="blurActiveInputWithObj(this)"
                                    onfocus="highlightActiveInputWithObj(this)" Width="120px"></asp:TextBox>
                            </div>
                            <div class="errorIcon">
                                *</div>
                            <div class="searchButton">
                                <asp:Button ID="btnCustomerLoad" OnClientClick="return LoadValidation('C')" OnClick="btnCustomerLoad_Click"
                                    runat="server" CssClass="ButtonAsh" Text="Search" />
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            Customer Name
                        </td>
                        <td>
                            <div class="fieldLeft">
                                <asp:TextBox ID="txtCustomerName" runat="server" CssClass="textInput" onblur="blurActiveInputWithObj(this)"
                                    onfocus="highlightActiveInputWithObj(this)" Width="240px"></asp:TextBox>
                            </div>
                            <div class="errorIcon">
                                *</div>
                        </td>
                        <td align="right">
                            Customer Name 2
                        </td>
                        <td>
                            <asp:TextBox ID="txtCustomerName2" runat="server" CssClass="textInput" onblur="blurActiveInputWithObj(this)"
                                onfocus="highlightActiveInputWithObj(this)" Width="240px"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td align="right" valign="top">
                            Address
                        </td>
                        <td>
                            <div class="fieldLeft">
                                <asp:TextBox TextMode="MultiLine" Height="70px" Width="240px" ID="txtAddress" CssClass="textInput"
                                    runat="server" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox></div>
                            <div class="errorIcon">
                                *</div>
                        </td>
                        <td align="right" valign="top">
                            Address 2
                        </td>
                        <td>
                            <asp:TextBox TextMode="MultiLine" Height="70px" Width="240px" ID="txtAddress2" CssClass="textInput"
                                runat="server" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td align="right" valign="top">
                            Foreign Address
                        </td>
                        <td valign="top">
                            <div class="fieldLeft">
                                <asp:TextBox TextMode="MultiLine" Height="70px" Width="240px" ID="txtForignAddress"
                                    CssClass="textInput" runat="server" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox></div>
                            <div class="errorIcon">
                                *</div>
                        </td>
                        <td align="right" valign="top">
                            Foreign Address2
                        </td>
                        <td valign="top">
                            <div class="fieldLeft">
                                <asp:TextBox TextMode="MultiLine" Height="70px" Width="240px" ID="txtForignAddress2"
                                    CssClass="textInput" runat="server" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox></div>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            Phone
                        </td>
                        <td>
                            <div class="fieldLeft">
                                <asp:TextBox ID="txtPhone" runat="server" CssClass="textInput" onblur="blurActiveInputWithObj(this)"
                                    onfocus="highlightActiveInputWithObj(this)" Width="240px" MaxLength="50"></asp:TextBox>
                            </div>
                            <div class="errorIcon">
                                *</div>
                        </td>
                        <td align="right">
                            Phone2
                        </td>
                        <td>
                            <asp:TextBox ID="txtPhone2" runat="server" CssClass="textInput" onblur="blurActiveInputWithObj(this)"
                                onfocus="highlightActiveInputWithObj(this)" Width="240px"  MaxLength="50"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            Date of Birth
                        </td>
                        <td>
                            <div class="fieldLeft">
                                <asp:TextBox Width="80Px" ID="txtDateofBirth" CssClass="textInput" runat="server"
                                    onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)">
                                </asp:TextBox>
                            </div>
                            <div class="errorIcon">
                                *</div>
                        </td>
                        <td align="right">
                            Date of Birth 2
                        </td>
                        <td>
                            <asp:TextBox Width="80Px" ID="txtDateofBirth2" CssClass="textInput" runat="server"
                                onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)">
                            </asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            Gender
                        </td>
                        <td>
                            <div class="fieldLeft">
                                <asp:DropDownList ID="ddlSex" runat="server" SkinID="ddlSmall">
                                    <asp:ListItem Text="Male" Value="M"></asp:ListItem>
                                    <asp:ListItem Text="Female" Value="F"></asp:ListItem>
                                </asp:DropDownList>
                            </div>
                        </td>
                        <td align="right">
                            Gender 2
                        </td>
                        <td>
                            <div class="fieldLeft">
                                <asp:DropDownList ID="ddlSex2" runat="server" SkinID="ddlSmall">
                                    <asp:ListItem Text="Male" Value="M"></asp:ListItem>
                                    <asp:ListItem Text="Female" Value="F"></asp:ListItem>
                                </asp:DropDownList>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            Nationality
                        </td>
                        <td>
                            <div class="fieldLeft">
                                <asp:TextBox ID="txtNationality" runat="server" CssClass="textInput" onblur="blurActiveInputWithObj(this)"
                                    onfocus="highlightActiveInputWithObj(this)" Width="240px"></asp:TextBox>
                            </div>
                            <div class="errorIcon">
                                *</div>
                        </td>
                        <td align="right">
                            Nationality 2
                        </td>
                        <td>
                            <div class="fieldLeft">
                                <asp:TextBox ID="txtNationality2" runat="server" CssClass="textInput" onblur="blurActiveInputWithObj(this)"
                                    onfocus="highlightActiveInputWithObj(this)" Width="240px"></asp:TextBox>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            Issue At
                        </td>
                        <td>
                            <div class="fieldLeft">
                                <asp:TextBox ID="txtIssueAt" MaxLength="20" runat="server" CssClass="textInput" onblur="blurActiveInputWithObj(this)"
                                    onfocus="highlightActiveInputWithObj(this)" Width="240px"></asp:TextBox>
                            </div>
                            <div class="errorIcon">
                                *</div>
                        </td>
                        <td align="right">
                            Issue At 2
                        </td>
                        <td>
                            <div class="fieldLeft">
                                <asp:TextBox ID="txtIssueAt2" MaxLength="20" runat="server" CssClass="textInput"
                                    onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"
                                    Width="240px"></asp:TextBox>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            Passport No
                        </td>
                        <td valign="top">
                            <div class="fieldLeft">
                                <asp:TextBox ID="txtPassportNo" runat="server" CssClass="textInput" onblur="blurActiveInputWithObj(this)"
                                    onfocus="highlightActiveInputWithObj(this)" Width="240px"></asp:TextBox>
                            </div>
                            <div class="errorIcon">
                                *</div>
                        </td>
                        <td align="right">
                            Passport No 2
                        </td>
                        <td valign="top">
                            <div class="fieldLeft">
                                <asp:TextBox ID="txtPassportNo2" runat="server" CssClass="textInput" onblur="blurActiveInputWithObj(this)"
                                    onfocus="highlightActiveInputWithObj(this)" Width="240px"></asp:TextBox>
                            </div>
                        </td>
                    </tr>
                    
                    <tr>
                        <td align="right">
                            National ID
                        </td>
                        <td>
                            <div class="fieldLeft">
                                <asp:TextBox ID="txtNationalID" runat="server" CssClass="textInput" onblur="blurActiveInputWithObj(this)"
                                    onfocus="highlightActiveInputWithObj(this)" Width="240px"></asp:TextBox>
                            </div>
                            <div class="errorIcon">
                                *</div>
                        </td>
                        <td align="right">
                            National ID 2
                        </td>
                        <td>
                            <asp:TextBox ID="txtNationalID2" runat="server" CssClass="textInput" onblur="blurActiveInputWithObj(this)"
                                onfocus="highlightActiveInputWithObj(this)" Width="240px"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            Birth Registration No.
                        </td>
                        <td>
                            <div class="fieldLeft">
                                <asp:TextBox ID="txtBirthCertNo" MaxLength="45" runat="server" CssClass="textInput"
                                    onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"
                                    Width="240px"></asp:TextBox>
                            </div>
                            <div class="errorIcon">
                                *</div>
                        </td>
                        <td align="right">
                            Birth Registration No. 2
                        </td>
                        <td>
                            <asp:TextBox ID="txtBirthCertNo2" MaxLength="45" runat="server" CssClass="textInput"
                                onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"
                                Width="240px"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            Email ID
                        </td>
                        <td>
                            <div class="fieldLeft">
                                <asp:TextBox ID="txtEmail" MaxLength="45" runat="server" CssClass="textInput" onblur="blurActiveInputWithObj(this)"
                                    onfocus="highlightActiveInputWithObj(this)" Width="240px"></asp:TextBox>
                            </div>
                            <div class="errorIcon">
                                *</div>
                        </td>
                        <td align="right">
                            Email ID 2
                        </td>
                        <td>
                            <asp:TextBox ID="txtEmail2" MaxLength="45" runat="server" CssClass="textInput" onblur="blurActiveInputWithObj(this)"
                                onfocus="highlightActiveInputWithObj(this)" Width="240px"></asp:TextBox>
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger EventName="Click" ControlID="btnSave" />
                <asp:AsyncPostBackTrigger EventName="Click" ControlID="btnReject" />
                <asp:AsyncPostBackTrigger EventName="Click" ControlID="btnDelete" />
                <asp:AsyncPostBackTrigger EventName="Click" ControlID="btnApprove" />
                <asp:AsyncPostBackTrigger EventName="Click" ControlID="btnCustomerLoad" />
                <asp:AsyncPostBackTrigger EventName="Click" ControlID="btnMasterLoad" />
            </Triggers>
        </asp:UpdatePanel>
    </fieldset>
    <br />
    <%--Start to User Comments--%>
    <asp:UpdatePanel runat="server" ID="upUserDet">
        <ContentTemplate>
            <uc1:UserDetails ID="ucUserDet" runat="server" />
        </ContentTemplate>
    </asp:UpdatePanel>
    <%--End of User Comments--%>
    <br />
    <fieldset>
        <table width="95%" align="center" cellpadding="10" cellspacing="3" border="0">
            <tr>
                <td align="right">
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Back" ID="btnBack" OnClick="btnBack_Click" />
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Reject" ID="btnReject" OnClientClick="return RejectValidation()"
                        OnClick="btnReject_Click" />
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Approve" ID="btnApprove" OnClientClick=" MsgPopupReturnTrue('Approve') "
                        OnClick="btnApprove_Click" />
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Reset" ID="btnReset" OnClientClick="return Reset()" />
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Save" ID="btnSave" OnClientClick="return SaveValidation()"
                        OnClick="btnSave_Click" />
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Delete" ID="btnDelete" OnClientClick="return Delete()"
                        OnClick="btnDelete_Click" />
                </td>
            </tr>
        </table>
    </fieldset>
    <uc3:Error ID="ucMessage" runat="server" />
</asp:Content>
