<%@ Page Title="" Language="C#" MasterPageFile="~/mp/site.master" Theme="Blue" AutoEventWireup="true"
    CodeBehind="DownloadEBBSCustomer.aspx.cs" Inherits="SBM_WebUI.mp.DownloadEBBSCustomer" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphDet" runat="server">
    <fieldset>
        <legend>Imported File Details</legend>
        <table>
            <tr>
                <td>
                    <asp:UpdatePanel ID="UpdatePanel2" runat="server" RenderMode="Inline">
                        <ContentTemplate>
                            <asp:GridView ID="grdFileProcessHistory" SkinID="SBMLGridGreen" runat="server" Width="100%" OnRowCommand="grdFileProcessHistory_RowCommand"
                                PageSize="25">
                                <Columns>
                                    <asp:ButtonField Text="Delete" ButtonType="Button"></asp:ButtonField>
                                </Columns>
                            </asp:GridView>
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="btnStart" EventName="Click"></asp:AsyncPostBackTrigger>
                        </Triggers>
                    </asp:UpdatePanel>
                </td>
            </tr>
        </table>
    </fieldset>
    <fieldset>
        <legend>File Import Details</legend>
        <table>
            <tr>
                <td style="width: 129px; height: 6px;" align="left">
                    <asp:Label ID="Label3" runat="server" Text="Import Type:"></asp:Label>
                </td>
                <td style="width: 100px; height: 6px;">
                    <asp:DropDownList ID="ddlFileType" runat="server" Width="362px" AutoPostBack="True"
                        OnSelectedIndexChanged="ddlFileType_SelectedIndexChanged">
                        <asp:ListItem>Select</asp:ListItem>
                        <asp:ListItem Value="C:\Files\SPMS\CSDT\">Customer Details</asp:ListItem>
                        <asp:ListItem Value="C:\Files\SPMS\ACDT\">Account Details</asp:ListItem>
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td style="width: 129px">
                </td>
                <td style="width: 100px">
                </td>
            </tr>
            <tr>
                <td align="right" colspan="2" style="height: 29px; text-align: left">
                    <asp:UpdatePanel ID="UpdatePanel3" runat="server" RenderMode="Inline">
                        <ContentTemplate>
                            <table>
                                <tbody>
                                    <tr>
                                        <td style="width: 129px" align="left">
                                            <asp:Label ID="Label1" runat="server" Text="File Path:"></asp:Label>
                                        </td>
                                        <td style="width: 100px">
                                            <asp:TextBox ID="txtFilePath" runat="server" Width="572px"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="width: 129px" align="left">
                                            <asp:Label ID="Label4" runat="server" Text="Import Date:"></asp:Label>
                                        </td>
                                        <td style="width: 100px">
                                            <asp:TextBox ID="txtImportDate" runat="server" Width="215px" ReadOnly="True"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="width: 129px" align="left">
                                            <asp:Label ID="Label2" runat="server" Text="File Creation Date:"></asp:Label>
                                        </td>
                                        <td style="width: 100px">
                                            <asp:TextBox ID="txtFileCreationDate" runat="server" Width="215px" 
                                                ReadOnly="True"></asp:TextBox>
                                        </td>
                                    </tr>
                                </tbody>
                            </table>
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="ddlFileType" EventName="SelectedIndexChanged">
                            </asp:AsyncPostBackTrigger>
                        </Triggers>
                    </asp:UpdatePanel>
                </td>
            </tr>
            <tr>
                <td>
                </td>
                <td style="width: 242px; height: 29px" align="left">
                    <asp:UpdateProgress ID="UpdateProgress1" runat="server">
                        <ProgressTemplate>
                            <div id="pgStatus">
                                <asp:Label ID="Label55" runat="server" Text="Import Progressing . . ."></asp:Label>&nbsp;
                                <img src="../../Images/loading.gif" style="width: 28px; height: 21px" /></div>
                        </ProgressTemplate>
                    </asp:UpdateProgress>
                </td>
            </tr>
            <tr>
                <td>
                </td>
                <td colspan="1" style="height: 29px">
                    <asp:Button ID="btnStart" runat="server" Text="Start" Width="80px" OnClick="btnStart_Click" />&nbsp;
                    <asp:Button ID="btnClose" runat="server" Text="Close" Width="80px" Visible="False" />&nbsp;
                    <asp:Button ID="btnHome" runat="server" Text="Home" Width="80px" OnClick="btnHome_Click"
                        Visible="False" />
                </td>
            </tr>
            <tr>
                <td>
                </td>
                <td colspan="1" style="height: 30px">
                    <asp:UpdatePanel ID="UpdatePanel1" runat="server" RenderMode="Inline">
                        <ContentTemplate>
                            <br />
                            <table>
                                <tbody>
                                    <tr>
                                        <td style="background-color: #00ccff" colspan="1">
                                        </td>
                                        <td style="background-color: #00ccff" colspan="2">
                                            <asp:Label ID="lblMessage" runat="server"></asp:Label>
                                        </td>
                                        <td style="width: 338px; background-color: #00ccff" colspan="1">
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="width: 184px; background-color: #00ccff">
                                        </td>
                                        <td style="width: 219px; background-color: #00ccff">
                                            <asp:Label ID="Label90" runat="server" Text="No of Row Processed"></asp:Label>
                                        </td>
                                        <td style="width: 46px; height: 26px; background-color: #00ccff">
                                            <asp:TextBox ID="txtProcess" runat="server" Width="143px" ReadOnly="True"></asp:TextBox>
                                        </td>
                                        <td style="width: 338px; height: 26px; background-color: #00ccff">
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="width: 184px; background-color: #00ccff">
                                        </td>
                                        <td style="width: 219px; background-color: #00ccff">
                                            <asp:Label ID="Label6" runat="server" Text="No of Row Imported"></asp:Label>
                                        </td>
                                        <td style="width: 46px; height: 26px; background-color: #00ccff">
                                            <asp:TextBox ID="txtImported" runat="server" Width="143px" ReadOnly="True"></asp:TextBox>
                                        </td>
                                        <td style="width: 338px; height: 26px; background-color: #00ccff">
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="width: 184px; background-color: #00ccff">
                                        </td>
                                        <td style="width: 219px; background-color: #00ccff">
                                            <asp:Label ID="Label7" runat="server" Text="No of Row Uncompleted"></asp:Label>
                                        </td>
                                        <td style="width: 46px; height: 26px; background-color: #00ccff">
                                            <asp:TextBox ID="txtUncompleted" runat="server" Width="143px" ReadOnly="True"></asp:TextBox>
                                        </td>
                                        <td style="width: 338px; height: 26px; background-color: #00ccff">
                                        </td>
                                    </tr>
                                </tbody>
                            </table>
                            <asp:GridView ID="grdBDDB1" runat="server" Width="650px" PageSize="25" OnPageIndexChanging="grdBDDB1_PageIndexChanging"
                                AutoGenerateColumns="False">
                            </asp:GridView>
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="btnStart" EventName="Click"></asp:AsyncPostBackTrigger>
                            <asp:AsyncPostBackTrigger ControlID="grdFileProcessHistory" EventName="RowCommand">
                            </asp:AsyncPostBackTrigger>
                        </Triggers>
                    </asp:UpdatePanel>
                    &nbsp;
                </td>
            </tr>
        </table>
    </fieldset>
    <fieldset>
    </fieldset>
</asp:Content>
