<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCSearchClaim.ascx.cs"
    Inherits="SBM_WebUI.UI.UC.UCSearchClaim" %>
<div id="MDClaimSearch" class="MDClass" runat="server">
    <table width="600" cellpadding="0" cellspacing="0" border="0">
        <tr>
            <td>
                <div class="MDHeader" style="width: 100%">
                    <div class="MDTitle">
                        <asp:Label runat="server" ID="lblPageCaption"></asp:Label></div>
                    <div class="MDClose">
                        <a href="javascript:void(0);">
                            <img border="0" alt="" src="../../Images/popupclose.gif" onclick="javascript:hideModal('ctl00_cphDet_Claim_MDClaimSearch');" />
                        </a>
                    </div>
                </div>
            </td>
        </tr>
        <tr>
            <td>
                <div class="MDBody" style="height: 500px; overflow: auto;">
                    <%--<asp:UpdatePanel runat="server" ID="upSuccess">
                        <ContentTemplate>--%>
                    <fieldset>
                        <legend>Search Criteria</legend>
                        <table width="550px" align="center" class="tableBody" border="0" cellpadding="3">
                            <tr>
                                <td align="right">
                                    Reference No
                                </td>
                                <td>
                                    &nbsp;&nbsp;<asp:TextBox ID="txtRefNo" Width="140px" CssClass="textInput" runat="server"
                                        onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"
                                        MaxLength="20"></asp:TextBox>
                                </td>
                                <td colspan="2" align="right">
                                    <asp:CheckBox runat="server" Text="Last Statement" ID="chkLastStatement" onClick="CheckLastStatement()" />
                                </td>
                            </tr>
                            <tr>
                                <td align="right">
                                    SP Type
                                </td>
                                <td colspan="4">
                                    &nbsp;&nbsp;<asp:DropDownList ID="ddlSPType" SkinID="ddlMedium" runat="server">
                                    </asp:DropDownList>
                                </td>
                            </tr>
                        </table>
                        <fieldset>
                            <legend>
                                <asp:CheckBox runat="server" Text="Statement Date" ID="chkStatemenDt" onClick="StatementDate()" /></legend>
                            <table width="550px" align="center" class="tableBody" border="0" cellpadding="3">
                                <tr>
                                    <td align="right">
                                        From Date
                                    </td>
                                    <td>
                                        &nbsp;&nbsp;<asp:TextBox runat="server" ID="txtFromDate" Width="140px" CssClass="textInput"
                                            onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                                    </td>
                                    <td align="right">
                                        To Date
                                    </td>
                                    <td>
                                        <asp:TextBox runat="server" ID="txtToDate" Width="150px" CssClass="textInput" onblur="blurActiveInputWithObj(this)"
                                            onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                                    </td>
                                </tr>
                            </table>
                        </fieldset>
                    </fieldset>
                    <br />
                    <fieldset>
                        <table width="550" align="center" class="tableBody" border="0">
                            <tr>
                                <td align="center">
                                    <asp:Label runat="server" ID="lblProgress"></asp:Label>
                                </td>
                                <td align="right">
                                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Search" ID="btnSearch" OnClientClick="ShowProgressStatus('ctl00_cphDet_Claim_lblProgress')"
                                        OnClick="btnSearch_Click" />
                                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Reset" ID="btnReset" OnClick="btnReset_Click" />
                                </td>
                            </tr>
                        </table>
                    </fieldset>
                    <br />
                    <fieldset runat="server" id="fsList">
                        <legend>Reference Details List</legend>
                        <table width="100%" align="center" class="tableBody" border="0">
                            <tr>
                                <td>
                                    <asp:UpdatePanel runat="server" ID="upGv">
                                        <ContentTemplate>
                                            <asp:GridView Width="98%" AutoGenerateColumns="true" ID="gvSearchList" runat="server"
                                                OnRowCommand="gvSearchList_RowCommand" SkinID="SBMLGridGreen" ShowHeader="true"
                                                OnRowDataBound="gvSearchList_RowDataBound" AllowPaging="true" OnPageIndexChanging="gvSearchList_PageIndexChanging">
                                                <Columns>
                                                    <asp:TemplateField HeaderText="Select" HeaderStyle-Width="5%">
                                                        <ItemTemplate>
                                                            <asp:Button CssClass="ButtonAsh" CommandName="Select" runat="server" ID="btnSelect"
                                                                OnClientClick="hideModal('ctl00_cphDet_Claim_MDClaimSearch')" Text="Select" />
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
                                            <asp:AsyncPostBackTrigger EventName="Click" ControlID="btnSearch" />
                                            <asp:AsyncPostBackTrigger EventName="Click" ControlID="btnReset" />
                                        </Triggers>
                                    </asp:UpdatePanel>
                                </td>
                            </tr>
                        </table>
                    </fieldset>
                    <%--                        </ContentTemplate>
                    </asp:UpdatePanel>--%>
                </div>
            </td>
        </tr>
    </table>
</div>

<script language="javascript" type="text/javascript">

    var currentDt = new Date();
    var mm = currentDt.getMonth() + 1;
    mm = (mm < 10) ? '0' + mm : mm;
    var dd = currentDt.getDate();
    dd = (dd < 10) ? '0' + dd : dd;
    var yyyy = currentDt.getFullYear();

    var date = dd + '/' + mm + '/' + yyyy;
    function CheckLastStatement(obj) {
        var chkStatemenDt = document.getElementById('<%=chkStatemenDt.ClientID %>');
        var txtRefNo = document.getElementById('<%=txtRefNo.ClientID %>');
        var chkStatemenDt = document.getElementById('<%=chkStatemenDt.ClientID %>');
        var txtFromDate = document.getElementById('<%=txtFromDate.ClientID %>');
        var txtToDate = document.getElementById('<%=txtToDate.ClientID %>');

        txtToDate.value = date;
        txtFromDate.value = date;

        if (obj.checked) {
            chkStatemenDt.checked = false;
            txtRefNo.value = "";
            txtRefNo.disabled = true;
            txtFromDate.disabled = true;
            txtToDate.disabled = true;
            txtFromDate.readOnly = true;
            txtToDate.readOnly = true;
        } else {
            txtRefNo.value = "";
            chkStatemenDt.checked = true;
            txtRefNo.disabled = false;
            txtFromDate.disabled = false;
            txtToDate.disabled = false;
            txtFromDate.readOnly = false;
            txtToDate.readOnly = false;
        }
    }

    function StatementDate(obj) {
        var chkLastStatement = document.getElementById('<%=chkLastStatement.ClientID %>');
        var txtRefNo = document.getElementById('<%=txtRefNo.ClientID %>');
        var chkStatemenDt = document.getElementById('<%=chkStatemenDt.ClientID %>');
        var txtFromDate = document.getElementById('<%=txtFromDate.ClientID %>');
        var txtToDate = document.getElementById('<%=txtToDate.ClientID %>');
        txtToDate.value = date;
        txtFromDate.value = date;

        if (obj.checked) {
            chkLastStatement.checked = false;
            txtFromDate.disabled = false;
            txtToDate.disabled = false;
            txtFromDate.readOnly = false;
            txtToDate.readOnly = false;
        } else {
            chkLastStatement.checked = true;
            txtFromDate.disabled = true;
            txtToDate.disabled = true;
            txtFromDate.readOnly = true;
            txtToDate.readOnly = true;
        }
    }

    function ClaimSearchPopupHide() {
        hideModal('ctl00_cphDet_Claim_MDClaimSearch');
        return false;
    }
    function ClaimSearchPopupHide() {
        hideModal('ctl00_cphDet_Claim_MDClaimSearch');
        return true;
    }

    function ClaimSearchReturnTrue() {
        $("#<%=gvSearchList.ClientID %>").remove();
        showModal("ctl00_cphDet_Claim_MDClaimSearch");
        return false;
    }
</script>

