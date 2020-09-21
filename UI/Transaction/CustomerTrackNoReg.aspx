<%@ Page Title="" Language="C#" MasterPageFile="~/mp/site.master" AutoEventWireup="true"
    CodeBehind="CustomerTrackNoReg.aspx.cs" Inherits="SBM_WebUI.mp.CustomerTrackNoReg" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphDet" runat="server">
    <br />
    <fieldset>
        <legend>Tracking Details</legend>
        <table width="95%" align="center" cellpadding="3" cellspacing="3" border="0">
            <tr>
                <td width="15%" align="right">
                    Track No
                </td>
                <td width="20%">
                    <asp:TextBox runat="server" ID="txtSearch" CssClass="textInput" Width="140px" onblur="blurActiveInputWithObj(this)"
                        onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                </td>
                <td>
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Search" ID="btnSearch" OnClientClick="return SearchTrackNoPopup()" />
                </td>
                <td width="15%" align="right">
                    Tracking Date
                </td>
                <td width="20%">
                    <asp:TextBox runat="server" ID="TextBox5" CssClass="textInput" Width="140px" onblur="blurActiveInputWithObj(this)"
                        onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                </td>
            </tr>
        </table>
    </fieldset>
    <br />
    <fieldset>
        <legend>Registration Details</legend>
        <br />
        <div style="height: 200px; width: 95%; overflow: auto; border: solid 1px silver;">
            <table width="95%" align="center" border="0">
                <tr>
                    <td>
                        <asp:GridView Style="width: 98%" ID="gvRegDetails" runat="server" AutoGenerateColumns="False"
                            SkinID="SBMLGridGreen" ShowHeader="true">
                            <Columns>
                                <asp:TemplateField HeaderStyle-Width="5%">
                                    <ItemTemplate>
                                        <asp:CheckBox runat="server" ID="chkReg" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField DataField="Val1" HeaderText="Tracked" />
                                <asp:BoundField DataField="Val1" HeaderText="SP Type" />
                                <asp:BoundField DataField="Val1" HeaderText="Issue Name" />
                                <asp:BoundField DataField="Val1" HeaderText="Master No" />
                                <asp:BoundField DataField="Val1" HeaderText="Issue Date" />
                                <asp:BoundField DataField="Val1" HeaderText="Issue Amount" />
                            </Columns>
                        </asp:GridView>
                    </td>
                </tr>
            </table>
        </div>
        <table width="95%" align="center" cellpadding="10" cellspacing="3" border="0">
            <tr>
                <td align="right">
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Add" ID="btnAdd" OnClientClick="return SearchTrackPopup()" />
                    &nbsp;
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Remove" ID="btnRemove" />
                    &nbsp;
                </td>
            </tr>
        </table>
    </fieldset>
    <br />
    <fieldset>
        <table width="95%" align="center" border="0">
            <tr>
                <td align="left" width="40%">
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Reset" ID="btnReset" />
                    &nbsp;
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Save" ID="btnSave" />
                    &nbsp;
                </td>
                <td align="right" width="60%">
                    <asp:CheckBox runat="server" ID="chkIncludeFiscalYear" Text="Include Fiscal Year" />
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Print Limit" ID="btnPrintLimit" />
                </td>
            </tr>
        </table>
    </fieldset>
    <%--Track No Search Popup Start--%>
    <div>
        <div id="popTrackNoSearch" class="MDClass" runat="server">
            <table width="600" cellpadding="0" cellspacing="0" border="0">
                <tr>
                    <td>
                        <div class="MDHeader" style="width: 100%">
                            <div class="MDTitle">
                                Tranck No Search</div>
                            <div class="MDClose">
                                <a href="javascript:void(0);">
                                    <img border="0" alt="" src="../../Images/popupclose.gif" onclick="javascript:hideModal('ctl00_ASPxRoundPanel2_cphDet_popTrackNoSearch');" />
                                </a>
                            </div>
                        </div>
                    </td>
                </tr>
                <tr>
                    <td>
                        <div class="MDBody">
                            <%--<table width="400px">
                            <tr>
                                <td>
                                    <div class="row">
                                        <span class="labelLeft">Name:</span> <span class="valueField">
                                            <asp:TextBox MaxLength="13" CssClass="textInput" runat="server" ID="txtName"></asp:TextBox>
                                        </span>
                                    </div>
                                    <div id="divButton" class="row centerAlign">
                                        <asp:Button ID="btnAction" runat="server" Text="Action Partial" CssClass="ButtonGreen" />
                                    </div>
                                </td>
                            </tr>
                        </table>--%>
                            <fieldset>
                                <legend>Search Details</legend>
                                <table width="95%" align="center" class="tableBody" border="0">
                                    <tr>
                                        <td>
                                            Track No
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" ID="TextBox10" CssClass="textInput" Width="140px" onblur="blurActiveInputWithObj(this)"
                                                onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            Master No
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" ID="TextBox6" CssClass="textInput" Width="140px" onblur="blurActiveInputWithObj(this)"
                                                onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            Registration No
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" ID="TextBox7" CssClass="textInput" Width="140px" onblur="blurActiveInputWithObj(this)"
                                                onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            Issue Date
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" ID="TextBox8" CssClass="textInput" Width="140px" onblur="blurActiveInputWithObj(this)"
                                                onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            Issue Name
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" ID="TextBox9" CssClass="textInput" Width="400px" onblur="blurActiveInputWithObj(this)"
                                                onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            &nbsp;
                                        </td>
                                        <td>
                                            <asp:Button ID="Button5" CssClass="ButtonAsh" runat="server" Text="Search" />
                                        </td>
                                    </tr>
                                </table>
                            </fieldset>
                            <fieldset>
                                <table width="95%" align="center" class="tableBody" border="0">
                                    <tr>
                                        <td>
                                            <asp:GridView Style="width: 98%" ID="gvTrackSearch" runat="server" AutoGenerateColumns="False"
                                                SkinID="SBMLGridGreen" ShowHeader="true">
                                                <Columns>
                                                    <asp:TemplateField HeaderStyle-Width="5%">
                                                        <ItemTemplate>
                                                            <asp:Button ID="Button6" CssClass="ButtonAshSmall" runat="server" Text="Select" />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:BoundField DataField="Val1" HeaderText="Track No." />
                                                    <asp:BoundField DataField="Val1" HeaderText="Reg No." />
                                                    <asp:BoundField DataField="Val1" HeaderText="SP Type." />
                                                    <asp:BoundField DataField="Val1" HeaderText="Issue Name." />
                                                    <asp:BoundField DataField="Val1" HeaderText="Master No." />
                                                    <asp:BoundField DataField="Val1" HeaderText="Issue Date." />
                                                    <asp:BoundField DataField="Val1" HeaderText="Issue Amount." />
                                                </Columns>
                                            </asp:GridView>
                                        </td>
                                    </tr>
                                </table>
                            </fieldset>
                        </div>
                    </td>
                </tr>
            </table>
        </div>

        <script language="javascript" type="text/javascript">
            function SearchTrackNoPopup() {
                showModal("ctl00_ASPxRoundPanel2_cphDet_popTrackNoSearch");
                return false;
            }
        </script>

    </div>
    <%--Track Search Popup End-- OnClick="btnAction_OnClick --%>
    <%--Track Search Popup Start--%>
    <div>
        <div id="popTrackSearch" class="MDClass" runat="server">
            <table width="600" cellpadding="0" cellspacing="0" border="0">
                <tr>
                    <td>
                        <div class="MDHeader" style="width: 100%">
                            <div class="MDTitle">
                                Registration Search</div>
                            <div class="MDClose">
                                <a href="javascript:void(0);">
                                    <img border="0" alt="" src="../../Images/popupclose.gif" onclick="javascript:hideModal('ctl00_ASPxRoundPanel2_cphDet_popTrackSearch');" />
                                </a>
                            </div>
                        </div>
                    </td>
                </tr>
                <tr>
                    <td>
                        <div class="MDBody">
                            <%--<table width="400px">
                            <tr>
                                <td>
                                    <div class="row">
                                        <span class="labelLeft">Name:</span> <span class="valueField">
                                            <asp:TextBox MaxLength="13" CssClass="textInput" runat="server" ID="txtName"></asp:TextBox>
                                        </span>
                                    </div>
                                    <div id="divButton" class="row centerAlign">
                                        <asp:Button ID="btnAction" runat="server" Text="Action Partial" CssClass="ButtonGreen" />
                                    </div>
                                </td>
                            </tr>
                        </table>--%>
                            <fieldset>
                                <legend>Search Details</legend>
                                <table width="95%" align="center" class="tableBody" border="0">
                                    <tr>
                                        <td>
                                            Master No
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" ID="TextBox1" CssClass="textInput" Width="140px" onblur="blurActiveInputWithObj(this)"
                                                onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            Registration No
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" ID="TextBox2" CssClass="textInput" Width="140px" onblur="blurActiveInputWithObj(this)"
                                                onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            Issue Date
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" ID="TextBox3" CssClass="textInput" Width="140px" onblur="blurActiveInputWithObj(this)"
                                                onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            Issue Name
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" ID="TextBox4" CssClass="textInput" Width="400px" onblur="blurActiveInputWithObj(this)"
                                                onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            &nbsp;
                                        </td>
                                        <td>
                                            <asp:Button ID="Button1" CssClass="ButtonAsh" runat="server" Text="Search" />
                                        </td>
                                    </tr>
                                </table>
                            </fieldset>
                            <fieldset>
                                <table width="95%" align="center" class="tableBody" border="0">
                                    <tr>
                                        <td>
                                            <asp:Button ID="Button2" CssClass="ButtonAsh" runat="server" Text="Mark All" />
                                            <asp:Button ID="Button3" CssClass="ButtonAsh" runat="server" Text="Unmark All" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:GridView Style="width: 98%" ID="gvRegSearchDetails" runat="server" AutoGenerateColumns="False"
                                                SkinID="SBMLGridGreen" ShowHeader="true">
                                                <Columns>
                                                    <asp:TemplateField HeaderStyle-Width="5%">
                                                        <ItemTemplate>
                                                            <asp:CheckBox runat="server" ID="chkReg" />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:BoundField DataField="Val1" HeaderText="Reg No." />
                                                    <asp:BoundField DataField="Val1" HeaderText="SP Type." />
                                                    <asp:BoundField DataField="Val1" HeaderText="Issue Name." />
                                                    <asp:BoundField DataField="Val1" HeaderText="Master No." />
                                                    <asp:BoundField DataField="Val1" HeaderText="Issue Date." />
                                                    <asp:BoundField DataField="Val1" HeaderText="Issue Amount." />
                                                </Columns>
                                            </asp:GridView>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:Button ID="Button4" CssClass="ButtonAsh" runat="server" Text="Select" />
                                        </td>
                                    </tr>
                                </table>
                            </fieldset>
                        </div>
                    </td>
                </tr>
            </table>
        </div>

        <script language="javascript" type="text/javascript">
            function SearchTrackPopup() {
                showModal("ctl00_ASPxRoundPanel2_cphDet_popTrackSearch");
                return false;
            }
        </script>

    </div>
    <%--Track Search Popup End-- OnClick="btnAction_OnClick --%>
</asp:Content>
