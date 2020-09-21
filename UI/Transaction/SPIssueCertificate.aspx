<%@ Page Title="" Language="C#" MasterPageFile="~/mp/site.master" AutoEventWireup="true"
    CodeBehind="SPIssueCertificate.aspx.cs" Inherits="SBM_WebUI.mp.SPIssueCertificate" %>

<%@ Register Src="~/UI/UC/UCUserDetails.ascx" TagName="UserDetails" TagPrefix="uc1" %>
<%@ Register Src="../UC/UCCustomerDetail_HSB.ascx" TagName="CustomerDetails" TagPrefix="uc2" %>
<%@ Register Src="../UC/UCPolicy.ascx" TagName="PolicyDetails" TagPrefix="uc3" %>
<%@ Register Src="../UC/UCSearchIssue.ascx" TagName="SIssue" TagPrefix="uc4" %>
<%@ Register Src="~/UI/UC/UCMessage.ascx" TagName="Error" TagPrefix="uc3" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <script src="../../Scripts/tabber.js" type="text/javascript"></script>

    <script language="javascript" type="text/javascript">
        function pageLoad(sender, args) {
            $('input[id*=Date]').each(function() { $(this).mask("99/99/9999"); });
        }

        function CheckNextData() {
            var reg = document.getElementById("<%=hdRegNo.ClientID %>");
            if (reg != null) {
                if (reg.value == "") {
                    MsgPopupReturnTrue('Info');
                }
            }
            return true;
        }

        function ShowPolicyDetail() {
            var reg = document.getElementById("<%=hdRegNo.ClientID %>");
            if (reg != null) {
                if (reg.value == "") {
                    MsgPopupReturnTrue('Info');
                } else {
                    PolicyDetailPopupReturnTrue();
                }
            }
            return true
        }
    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphDet" runat="server">
    <fieldset>
        <legend>Issue Details</legend>
        <asp:UpdatePanel runat="server" ID="UpdatePanel8">
            <ContentTemplate>
                <asp:HiddenField runat="server" ID="hdRegNo" />
                <asp:HiddenField runat="server" ID="hdIssueTransNo" />
                <asp:HiddenField runat="server" ID="hdChargeAmount" />
                <table width="100%" align="center" class="tableBody" border="0">
                    <tr>
                        <td align="right">
                            SP Type
                        </td>
                        <td>
                            <asp:DropDownList ID="ddlSpType" SkinID="ddlMedium" runat="server" AutoPostBack="True">
                            </asp:DropDownList>
                        </td>
                        <td align="right">
                            Branch Code
                        </td>
                        <td>
                            <asp:DropDownList ID="ddlBranch" SkinID="ddlMedium" runat="server">
                            </asp:DropDownList>
                        </td>
                        <td align="right">
                            Issue Date
                        </td>
                        <td align="left">
                            <div class="fieldLeft">
                                <asp:TextBox ID="txtIssueDate" Width="100px" CssClass="textInput" runat="server"
                                    onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            Reg No
                        </td>
                        <td>
                            <div class="fieldLeft">
                                <asp:TextBox ID="txtRegistrationNo" Width="140px" CssClass="textInput" runat="server"
                                    MaxLength="25" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                            </div>
                            <div class="errorIcon">
                                <asp:Button CssClass="ButtonAsh" runat="server" OnClick="btnRegSearch_Click" Text="Search"
                                    ID="bntRegSeach" OnClientClick="return openIssueSearchPopup('ctl00_cphDet_txtRegistrationNo')" /></div>
                        </td>
                        <td align="right">
                            Customer Type
                        </td>
                        <td>
                            <asp:DropDownList ID="ddlCustomerType" SkinID="ddlMedium" runat="server">
                            </asp:DropDownList>
                        </td>
                        <td align="right">
                            Applied Amount
                        </td>
                        <td>
                            <asp:TextBox ID="txtAppliedAmount" Width="100px" CssClass="textInput" runat="server"
                                onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
            <Triggers>
                <%--<asp:AsyncPostBackTrigger EventName="Click" ControlID="btnLeftShift" />
                <asp:AsyncPostBackTrigger EventName="Click" ControlID="btnRightShift" />--%>
                <%--<asp:AsyncPostBackTrigger EventName="Click" ControlID="bnnPrintLimit" />--%>
                <asp:AsyncPostBackTrigger EventName="Click" ControlID="bntRegSeach" />
                <asp:AsyncPostBackTrigger EventName="Click" ControlID="btnShowPolicy" />
            </Triggers>
        </asp:UpdatePanel>
    </fieldset>
        <br />
    <fieldset>
        <legend>Customer(s) Details</legend>
            <div style="height: 70%; width: 100%; overflow: auto;">
                <asp:UpdatePanel runat="server" ID="UpdatePanel7">
                    <ContentTemplate>
                        <asp:GridView Style="width: 98%; height: 100%" ID="gvCustomerDetail" runat="server"
                            AutoGenerateColumns="False" OnRowCommand="gvCustomerDetail_RowCommand" SkinID="SBMLGridGreen">
                            <Columns>
                                <asp:TemplateField HeaderText="Select" HeaderStyle-Width="5%">
                                    <ItemTemplate>
                                        <asp:Button CssClass="ButtonAsh" CommandName="Select" runat="server" ID="btnCusDetailSelect"
                                            Text="Select" OnClientClick="return CustomerDetailPopupReturnTrue()" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField DataField="bfCustomerID" HeaderText="Customer ID" HeaderStyle-HorizontalAlign="Left" />
                                <asp:BoundField DataField="bfCustomerName" HeaderText="Name" />
                                <asp:BoundField DataField="bfAddress" HeaderText="Address" />
                                <asp:BoundField DataField="bfDateOfBirth" HeaderText="Date Of Birth 1" HeaderStyle-HorizontalAlign="Left" />
                                <%--<asp:BoundField DataField="bfDateOfBirth2" HeaderText="Date Of Birth 2" />--%>
                                <asp:BoundField DataField="bfPhone" HeaderText="Phone" />
                                <%--<asp:BoundField DataField="bfNationality" HeaderText="Nationality" />--%>
                                <asp:BoundField DataField="bfPassportNo" HeaderText="Passport No" />
                                <asp:BoundField DataField="bfForeignAddress" HeaderText="Foreign Address" />
                            </Columns>
                            <EmptyDataTemplate>
                                No record found
                            </EmptyDataTemplate>
                            <AlternatingRowStyle CssClass="odd" />
                        </asp:GridView>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
        </fieldset>
        <%--<asp:BoundField DataField="bfDateOfBirth2" HeaderText="Date Of Birth 2" />--%><%--<asp:BoundField DataField="bfNationality" HeaderText="Nationality" />--%>
    <br />
    <fieldset>
        <legend>Nominee(s) Details</legend>
            <div style="height: 60%; width: 100%; overflow: auto;">
                <asp:UpdatePanel runat="server" ID="UpdatePanel6">
                    <ContentTemplate>
                        <asp:GridView Style="width: 98%; height: 100%" ID="gvNomDetail" runat="server" AutoGenerateColumns="False"
                            SkinID="SBMLGridGreen">
                            <Columns>
                                <asp:BoundField DataField="bfSlNo" HeaderText="SlNo" />
                                <asp:BoundField DataField="bfNomineeName" HeaderText="Nominee Name" />
                                <asp:BoundField DataField="bfRelation" HeaderText="Relation" />
                                <asp:BoundField DataField="bfNomineeShare" HeaderText="Share" />
                                <asp:BoundField DataField="bfAddress" HeaderText="Address" />
                                <asp:BoundField DataField="bfAmount" HeaderText="Amount" />
                            </Columns>
                            <EmptyDataTemplate>
                                No record found
                            </EmptyDataTemplate>
                            <AlternatingRowStyle CssClass="odd" />
                        </asp:GridView>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
        </fieldset>
        <%--<asp:BoundField DataField="bfNationality" HeaderText="Nationality" />--%><%-- End of Customer(s) Details Tab  --%><%-- Nominee(s) Details Tab  --%><%-- End of Nominee(s) Details Tab  --%>
    <br />
    <fieldset>
        <legend>Account holder details</legend>
        <asp:UpdatePanel runat="server" ID="UpdatePanel2">
            <ContentTemplate>
                <table width="98%" align="center" class="tableBody" border="0">
                    <tr>
                        <td>
                            Holder Name
                        </td>
                        <td colspan="2">
                            <asp:TextBox ID="txtBHDHolderName" Width="300px" CssClass="textInput" runat="server"
                                onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                        </td>
                        <td>
                            Relation
                        </td>
                        <td>
                            <asp:TextBox ID="txtBHDRelation" Width="150px" CssClass="textInput" runat="server"
                                onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                        </td>
                        <td>
                            <asp:Button CssClass="ButtonAsh" OnClick="btnShowPolicy_Click" runat="server" OnClientClick="return ShowPolicyDetail()"
                                Text="Show Policy" ID="btnShowPolicy" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Issue Name
                        </td>
                        <td colspan="2">
                            <asp:TextBox ID="txtIssueName" Width="415px" CssClass="textInput" runat="server"
                                ReadOnly="true" TextMode="MultiLine" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                        </td>
                        <td align="right">
                            Address
                        </td>
                        <td colspan="2">
                            <asp:TextBox ID="txtBHDAddress" Width="330px" CssClass="textInput" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
    </fieldset>
    <%-- Nominee(s) Wise Denomination Tab  --%>
    <table width="100%" align="center" class="tableBody" border="0">
        <tr>
            <td valign="top" width="60%">
                <asp:UpdatePanel runat="server" ID="UpdatePanel3">
                    <ContentTemplate>
                        <fieldset>
                            <legend>Certificate(s) Details</legend>
                            <table width="100%" align="center" class="tableBody" cellpadding="3" cellspacing="3"
                                border="0">
                                <tr>
                                    <td valign="top">
                                        <div style="height: 305px; width: 98%; overflow: auto;">
                                            <asp:GridView Style="width: 98%" ID="gvDenomDetail" runat="server" AutoGenerateColumns="False"
                                                SkinID="SBMLGridGreen" ShowHeader="true" OnRowDataBound="gvDenomDetail_RowDataBound">
                                                <Columns>
                                                    <asp:BoundField DataField="bfDenomination" HeaderText="Denomination" />
                                                    <asp:BoundField DataField="bfSeries" HeaderText="Series" />
                                                    <asp:BoundField DataField="bfSerialNo" HeaderText="Serial No" />
                                                    <asp:BoundField DataField="bfStatus" HeaderText="Status" />
                                                </Columns>
                                                <EmptyDataTemplate>
                                                    No record found
                                                </EmptyDataTemplate>
                                                <AlternatingRowStyle CssClass="odd" />
                                            </asp:GridView>
                                        </div>
                                    </td>
                                </tr>
                            </table>
                        </fieldset>
                    </ContentTemplate>
                </asp:UpdatePanel>
                <br />
            </td>
            <td valign="top" width="40%">
                <asp:UpdatePanel runat="server" ID="UpdatePanel4">
                    <ContentTemplate>
                        <fieldset>
                            <legend>Certificate Genration Details</legend>
                            <table width="98%" align="center" class="tableBody" cellpadding="3" cellspacing="3"
                                border="0">
                                <tr>
                                    <td align="right">
                                        Certificate Type
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="ddlCertificateType" SkinID="ddlLarge" Width="200px" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlCertificateType_SelectedIndexChanged">
                                            <asp:ListItem Selected="True" Value="0">Select Type</asp:ListItem>
                                            <asp:ListItem Value="1">Issuance Certificate</asp:ListItem>
                                            <asp:ListItem Value="2">Encashment Certificate</asp:ListItem>
                                            <asp:ListItem Value="3">Interest Payment Certificate</asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="right">
                                        Last Cert. Print Date
                                    </td>
                                    <td>
                                            <asp:TextBox Width="120px" ID="txtLastCertDate" CssClass="textInput" runat="server"
                                            onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)" Enabled="False"></asp:TextBox>

                                    </td>
                                </tr>
                                <tr>
                                    <td align="right">
                                        No of Cert. Printed Earlier
                                    </td>
                                    <td>
                                        <asp:TextBox Width="80px" ID="txtNoofCertPrint" CssClass="textInput" runat="server"
                                            onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)" Enabled="False"></asp:TextBox>
                                        <asp:TextBox Width="100px" ForeColor="Red" ID="txtDuplicate" CssClass="textInput" runat="server"
                                            onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)" Font-Bold="True" Enabled="False"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="right">
                                        Last Maker
                                    </td>
                                    <td>
                                        <asp:TextBox Width="200px" ID="txtLastMaker" CssClass="textInput" runat="server"
                                            onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)" Enabled="False"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="right">
                                        Customer Type
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="ddlCertCustomerType" SkinID="ddlMedium" Width="150px" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlCertCustomerType_SelectedIndexChanged">
                                            <asp:ListItem Value="REGULAR">REGULAR</asp:ListItem>
                                            <asp:ListItem Value="SELECT">SELECT</asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="right">
                                        Waive Charges/ Reprint
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="ddlWaiveCharges" SkinID="ddlMedium" Width="150px" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlWaiveCharges_SelectedIndexChanged">
                                            <asp:ListItem Selected="True" Value="0">No</asp:ListItem>
                                            <asp:ListItem Value="1">Yes</asp:ListItem>
                                            <asp:ListItem Value="2">RePrint</asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="right">
                                        Charge Amount
                                    </td>
                                    <td>
                                        <asp:TextBox Width="120px" ID="txtChargeAmount" CssClass="textInput" runat="server"
                                            onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)" Enabled="False"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="right">VAT Amount
                                    </td>
                                    <td>
                                        <asp:TextBox Width="120px" ID="txtVATAmount" CssClass="textInput" runat="server"
                                            onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)" Enabled="False"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="right">Total Amount
                                    </td>
                                    <td>
                                        <asp:TextBox Width="120px" ID="txtTotalAmount" CssClass="textInput" runat="server"
                                            onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)" Enabled="False"></asp:TextBox>
                                    </td>
                                </tr>
                            </table>
                        </fieldset>
                    </ContentTemplate>
                <Triggers>
                <asp:AsyncPostBackTrigger EventName="SelectedIndexChanged" ControlID="ddlCertificateType" />
                <asp:AsyncPostBackTrigger EventName="SelectedIndexChanged" ControlID="ddlCertCustomerType" />
                <asp:AsyncPostBackTrigger EventName="SelectedIndexChanged" ControlID="ddlWaiveCharges" />
            </Triggers>
                </asp:UpdatePanel>
            </td>
        </tr>
    </table>
    <%--<div class="tabbertab">
            <h2>
                Nominee(s) Wise Denomination</h2>
            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                <ContentTemplate>
                    <div style="width: 100%; overflow: auto;">
                        <asp:UpdatePanel runat="server" ID="UpdatePanel5">
                            <ContentTemplate>
                                <asp:GridView Style="width: 98%; height: 100%" ID="gvNomDemon" runat="server" AutoGenerateColumns="False"
                                    SkinID="SBMLGridGreen">
                                    <Columns>
                                        <asp:TemplateField HeaderText="Select">
                                            <ItemTemplate>
                                                <asp:Button CssClass="ButtonAsh" CommandName="Delete" runat="server" ID="btnSelect"
                                                    Text="Select" />
                                            </ItemTemplate>
                                            <HeaderStyle Width="5%" />
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="Val1" HeaderText="Nominee Name" />
                                        <asp:BoundField DataField="Val1" HeaderText="Denomination" />
                                        <asp:BoundField DataField="Val1" HeaderText="Quantity" />
                                        <asp:BoundField DataField="Val1" HeaderText="Amount" />
                                    </Columns>
                                    <EmptyDataTemplate>
                                        No record found
                                    </EmptyDataTemplate>
                                    <AlternatingRowStyle CssClass="odd" />
                                </asp:GridView>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                </ContentTemplate>
                <Triggers>
                </Triggers>
            </asp:UpdatePanel>
        </div>
        </div>--%>
    <asp:UpdatePanel runat="server" ID="upUserDet">
        <ContentTemplate>
            <uc1:UserDetails ID="ucUserDet" runat="server" />
        </ContentTemplate>
    </asp:UpdatePanel>
    <%-- End of Nominee(s) Wise Denomination Tab  --%>
    <br />
    <fieldset>
        <table width="98%" align="center" class="tableBody" border="0">
            <tr>
                <td align="right">
                    <asp:DropDownList ID="ddlTaxYear" SkinID="ddlMedium" runat="server" TabIndex="2" AutoPostBack="true">
                        <asp:ListItem>2013</asp:ListItem>
                        <asp:ListItem>2014</asp:ListItem>
                        <asp:ListItem>2015</asp:ListItem>
                        <asp:ListItem>2016</asp:ListItem>
                        <asp:ListItem>2017</asp:ListItem>
                        <asp:ListItem>2018</asp:ListItem>
                        <asp:ListItem>2019</asp:ListItem>
                        <asp:ListItem>2020</asp:ListItem>
                        <asp:ListItem>2021</asp:ListItem>
                        <asp:ListItem>2022</asp:ListItem>
                        <asp:ListItem>2023</asp:ListItem>
                        <asp:ListItem>2024</asp:ListItem>
                        <asp:ListItem>2025</asp:ListItem>
                    </asp:DropDownList>
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Print Certificate" ID="btnPrintCertificate" OnClick="btnPrintCertificate_Click" Width="175px" />
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Reset Data" ID="btnReset" OnClick="btnReset_Click" Width="175px" />
                    <%--<br />--%>
                </td>
                <%--Start to User Comments--%>
            </tr>
        </table>
    </fieldset>
    <uc2:CustomerDetails ID="CustomerDetail" runat="server" />
    <uc3:PolicyDetails ID="PD" runat="server" />
    <uc4:SIssue ID="SIssue" runat="server" Type="2" />
    <uc3:Error ID="ucMessage" runat="server" />
</asp:Content>
