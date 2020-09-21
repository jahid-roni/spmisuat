<%@ Page Title="" Language="C#" MasterPageFile="~/mp/site.master" AutoEventWireup="true"
    CodeBehind="JournalEntryApprove.aspx.cs" Inherits="SPMS_Web.mp.JournalEntryApprove" %>

<%@ Register Src="~/UI/UC/UCUserDetails.ascx" TagName="UserDetails" TagPrefix="uc1" %>
<%@ Register Src="~/UI/UC/UCMessage.ascx" TagName="Error" TagPrefix="uc2" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxtoolkit" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <script language="javascript" type="text/javascript">
        function pageLoad(sender, args) {
            $('input[id*=Date]').each(function() { $(this).mask("99/99/9999"); });
        }
        function ApproveValidate() {
            var sErrorList = "";
            var c = 0;
            $('#<%=gvAccRecon.ClientID %>').find("input:checkbox").each(function() {
                if(this.checked == true) c ++;
            });
            if (c <= 0) {
                sErrorList += "Please select journals";
            }
            // show error divErrorList
            var divErrorPanel = document.getElementById('divErrorPanel');
            if (sErrorList.length > 0) {
                var errorList = document.getElementById('divErrorList');
                errorList.innerHTML = "<ul>" + sErrorList + "</ul>";
                divErrorPanel.style.display = "block";

                window.scroll(0, 0);
                return false;
            }
            else {
                divErrorPanel.style.display = "none";
                //MsgPopupReturnTrue('Save');
                return true;
            }            
        }
    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphDet" runat="server">
    <%-- Error --%>
    <div id="divErrorPanel" style="display: none">
        <fieldset class="errorFieldset">
            <legend class="errorLegend">Mandatory Field List</legend>
            <div class="errorBox" id="divErrorList">
            </div>
        </fieldset>
        <br />
    </div>
    <%-- Error --%>
    <fieldset>
        <legend>Journal Approve</legend>
        <asp:UpdatePanel runat="server" ID="UpdatePanel3">
            <ContentTemplate>
                <table width="95%" align="center" cellpadding="3" cellspacing="3" border="0">
                    <tr>
                        <td width="20%" align="right">
                            SP Type
                        </td>
                        <td width="20%">
                            <asp:DropDownList runat="server" ID="ddlSPType" Width="140px">
                            </asp:DropDownList>
                        </td>
                        <td width="30%" align="right">
                            Transaction Date
                        </td>
                        <td width="30%">
                            <asp:TextBox runat="server" ID="txtTransDate" CssClass="textInput" Width="140px"
                                onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            Account No
                        </td>
                        <td>
                            <asp:DropDownList runat="server" ID="ddlAccountNo" Width="140px">
                            </asp:DropDownList>
                        </td>
                        <td align="right">
                            Maker
                        </td>
                        <td>
                            <asp:DropDownList runat="server" ID="ddlMaker" Width="140px" />
                        </td>
                    </tr>
                    <tr>
                        <td colspan="4" align="center">
                            <asp:Button runat="server" ID="btnSearch" Text="Search" CssClass="ButtonAsh" OnClick="btnSearch_Click" />
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
    </fieldset>
    <br />
    <div class="PageCaptionRight">
        Journal</div>
    <asp:UpdatePanel runat="server" ID="UpdatePanel5">
        <ContentTemplate>
            <div style="height: 200px; width: 100%; overflow: auto;">
                <asp:GridView Style="width: 98%" ID="gvAccRecon" runat="server" AutoGenerateColumns="False"
                    SkinID="SCBLGridGreen" ShowHeader="true">
                    <Columns>
                        <asp:TemplateField HeaderText="Select" HeaderStyle-Width="5%">
                            <ItemTemplate>
                                <asp:CheckBox runat="server" ID="chkSelected" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="SPTypeID" HeaderText="SP Type" />
                        <asp:BoundField DataField="AccountNo" HeaderText="Account No" />
                        <asp:BoundField DataField="Narration" HeaderText="Narration" />
                        <asp:BoundField DataField="CurrencyID" HeaderText="Currency" />
                        <asp:BoundField DataField="DrCr" HeaderText="DrCr" />
                        <asp:BoundField DataField="Amount" HeaderText="Amount" />
                        <asp:BoundField DataField="MakerID" HeaderText="Maker ID" />
                        <asp:BoundField DataField="MakeDate" HeaderText="Make Date" />
                    </Columns>
                </asp:GridView>
            </div>
            <div align="right" style="padding-top: 6px">
                <asp:Button runat="server" CssClass="ButtonAsh" ID="btnSelectAll" name="Select" Text="Select All"
                    OnClick="btnSelectAll_Click" />
                &nbsp;
                <asp:Button runat="server" CssClass="ButtonAsh" ID="btnDeselectAll" name="DeSelect"
                    Text="Deselect All" OnClick="btnDeselectAll_Click" />
                &nbsp;
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
    <br />
    <fieldset>
        <legend>Reconciliation Specification</legend>
        <asp:UpdatePanel runat="server" ID="UpdatePanel6">
            <ContentTemplate>
                <table width="95%" align="center" class="tableBody" cellpadding="3" cellspacing="3"
                    border="0">
                    <tr>
                        <td align="right" width="30%">
                            Total Debit Amount
                        </td>
                        <td width="20%">
                            <asp:TextBox runat="server" CssClass="textInput" Width="130px" ID="txtTotDrAmt" onblur="blurActiveInputWithObj(this)"
                                onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                        </td>
                        <td align="right" width="30%">
                            Total Credit Amount
                        </td>
                        <td width="20%">
                            <asp:TextBox runat="server" CssClass="textInput" Width="130px" ID="txtTotCrAmt" onblur="blurActiveInputWithObj(this)"
                                onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            Total No. of Debit Transaction
                        </td>
                        <td>
                            <asp:TextBox runat="server" CssClass="textInput" Width="130px" ID="txtNoOfDrTrans"
                                onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                        </td>
                        <td align="right">
                            Total No. of Credit Transaction
                        </td>
                        <td>
                            <asp:TextBox runat="server" CssClass="textInput" Width="130px" ID="txtNoOfCrTrans"
                                onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
    </fieldset>
    <br />
    <%--End of User Comments--%>
    <asp:UpdatePanel runat="server" ID="upUserDet">
        <ContentTemplate>
            <uc1:UserDetails ID="ucUserDet" runat="server" />
        </ContentTemplate>
    </asp:UpdatePanel>
    <%--End of User Comments--%>
    <fieldset>
        <asp:UpdatePanel runat="server" ID="UpdatePanel7">
            <ContentTemplate>
                <table width="95%" align="center" class="tableBody" cellpadding="10" cellspacing="3"
                    border="0">
                    <tr>
                        <td align="center">
                            <asp:Button CssClass="ButtonAsh" runat="server" Text="Reject" ID="btnReject" OnClick="btnReject_Click" />
                            <asp:Button CssClass="ButtonAsh" runat="server" Text="Approve" ID="btnApprove" OnClick="btnApprove_Click" OnClientClick="return ApproveValidate()" />
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
    </fieldset>
    <asp:UpdatePanel runat="server" ID="UpdatePanel1">
        <ContentTemplate>
            <uc2:Error ID="ucMessage" runat="server" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
