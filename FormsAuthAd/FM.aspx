<%@ Page Language="c#" AutoEventWireup="true" CodeBehind="FM.aspx.cs" Inherits="Compendiums.FM" %>

<%@ Import Namespace="System.Security.Principal" %>


<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1, shrink-to-fit=no" />

    <title>FM Compendiums</title>

    <!-- Bootstrap core CSS -->
    <link href="vendor/bootstrap/css/bootstrap.min.css" rel="stylesheet" />
    <link href="css/jquery-ui.css" rel="stylesheet" />
    <link rel="stylesheet" type="text/css" href="css/styles.css?v=1.5" />
    <script src="vendor/jquery/jquery-1.12.4.js"></script>
    <script src="vendor/jquery/jquery-ui.js"></script>

    <script type="text/javascript">
       $(document).ready(function(){
           $('.ddlUnit').change(function () {
               Calculate();
           });

           $('#gvBom_txtScale').keyup(function () {
               $('#hfScale').val($('#gvBom_txtScale').val());
               Calculate();
           });
           $('#txtItem').keyup(function () {
               Disable();
           });
           $("#txtItem").autocomplete({
                source: function(request, response) {  
                    $.ajax({  
                        type: "POST",  
                        contentType: "application/json; charset=utf-8",  
                        url: "FM.aspx/GetItems",  
                        data: "{'item':'" + $('#txtItem').val() + "'}",  
                        dataType: "json",  
                        success: function(data) {  
                            response(data.d);
                        },  
                        error: function(result) {  
                            alert("No Match");  
                        }  
                    });  
               },
               select: function (event, ui) {
                   
                   $("#txtItem").val(ui.item.label);
                   document.getElementById('anchorId').click();
                    return false
                },
                change: function (event, ui) {
                    if (!ui.item) {
                        $("#txtItem").val("");
                        $("#gvBom").toggle().hide();
                    }
                }
            });
        });
        function Disable() {
            $('.disable').attr("disabled", "disabled");
            $('.txtdisable').attr("disabled", "disabled");
            $('.txtdisable').text("");
            $('#btnUpdate').css({ 'background-color': '#ddd' });
            $('.ImageSize').attr('src','img/noimg.png');
        }
        function Calculate(){var count=parseInt($("#hfBomCount").val());if(count>0){$("#gvBom_txtScale").val($("#hfScale").val());for(var scale=parseFloat($("#hfScale").val()),portion=parseFloat($("#ddlPortion").val()),i=0;count>i;i++){var base=parseFloat($("#gvBom_hfQty_"+i).val());$("#gvBom_lblQty_"+i).text((base*portion).toFixed(2));var unit=$("#gvBom_ddlUnit_"+i).val();if("LB"==unit){var g=parseFloat($("#gvBom_hfG_"+i).val());g>0?$("#gvBom_lblScale_"+i).text((g*portion/453.59*scale).toFixed(2)):$("#gvBom_lblScale_"+i).text("Not able to Convert!")}else if("OZ"==unit){var g=parseFloat($("#gvBom_hfG_"+i).val());g>0?$("#gvBom_lblScale_"+i).text((g*portion/28.35*scale).toFixed(2)):$("#gvBom_lblScale_"+i).text("Not able to Convert!")}else if("KG"==unit){var g=parseFloat($("#gvBom_hfG_"+i).val());g>0?$("#gvBom_lblScale_"+i).text((g*portion/1e3*scale).toFixed(2)):$("#gvBom_lblScale_"+i).text("Not able to Convert!")}else if("G"==unit){var g=parseFloat($("#gvBom_hfG_"+i).val());g>0?$("#gvBom_lblScale_"+i).text((g*portion*scale).toFixed(2)):$("#gvBom_lblScale_"+i).text("Not able to Convert!")}else if("GL"==unit){var tsp=parseFloat($("#gvBom_hfTSP_"+i).val());tsp>0?$("#gvBom_lblScale_"+i).text((tsp*portion/768*scale).toFixed(2)):$("#gvBom_lblScale_"+i).text("Not able to Convert!")}else if("LT"==unit){var tsp=parseFloat($("#gvBom_hfTSP_"+i).val());tsp>0?$("#gvBom_lblScale_"+i).text((tsp*portion/202.88*scale).toFixed(2)):$("#gvBom_lblScale_"+i).text("Not able to Convert!")}else if("QT"==unit){var tsp=parseFloat($("#gvBom_hfTSP_"+i).val());tsp>0?$("#gvBom_lblScale_"+i).text((tsp*portion/192*scale).toFixed(2)):$("#gvBom_lblScale_"+i).text("Not able to Convert!")}else if("CUP"==unit){var tsp=parseFloat($("#gvBom_hfTSP_"+i).val());tsp>0?$("#gvBom_lblScale_"+i).text((tsp*portion/48*scale).toFixed(2)):$("#gvBom_lblScale_"+i).text("Not able to Convert!")}else if("FLOZ"==unit){var tsp=parseFloat($("#gvBom_hfTSP_"+i).val());tsp>0?$("#gvBom_lblScale_"+i).text((tsp*portion/6*scale).toFixed(2)):$("#gvBom_lblScale_"+i).text("Not able to Convert!")}else if("TBL"==unit){var tsp=parseFloat($("#gvBom_hfTSP_"+i).val());tsp>0?$("#gvBom_lblScale_"+i).text((tsp*portion/3*scale).toFixed(2)):$("#gvBom_lblScale_"+i).text("Not able to Convert!")}else if("TSP"==unit){var tsp=parseFloat($("#gvBom_hfTSP_"+i).val());tsp>0?$("#gvBom_lblScale_"+i).text((tsp*portion*scale).toFixed(2)):$("#gvBom_lblScale_"+i).text("Not able to Convert!")}}}}
    </script>
     
</head>
<body>
    <!-- Navigation -->
    <form id="Form1" runat="server">
        <nav class="navbar navbar-expand-lg navbar-dark bg-dark fixed-top">
            <div class="container">
                <div class="dropdown">
                <a class="navbar-brand" href="../">
                    <img src="img/logo.png" width="200" alt="">
                </a>
                <div class="dropdown-content">
                    <asp:Panel ID="Panel1" runat="server"></asp:Panel>
                </div>
            </div>
                <button class="navbar-toggler" type="button" data-toggle="collapse" data-target="#navbarResponsive" aria-controls="navbarResponsive" aria-expanded="false" aria-label="Toggle navigation">
                    <span class="navbar-toggler-icon"></span>
                </button>
                <div class="collapse navbar-collapse" id="navbarResponsive">
                    <ul class="navbar-nav ml-auto">
                        <li class="nav-item">
                            <asp:LinkButton ID="Compendiums_R" class="nav-link" runat="server" href="Default.aspx">Compendiums</asp:LinkButton>
                        </li>
                        <li class="nav-item active">
                            <asp:LinkButton ID="FMlb" class="nav-link" runat="server" href="FM.aspx">FM Compendiums<span class="sr-only">(current)</span></asp:LinkButton>
                        </li>
                        <li class="nav-item">
                            <asp:LinkButton ID="Manage_Users" class="manuser-link" runat="server" href="User.aspx">Manage Users</asp:LinkButton>
                        </li>
                        <li class="nav-item">
                            <asp:LinkButton ID="LinkButton2" class="signout-link" OnClick="Logout_Click" runat="server">Sign out</asp:LinkButton>
                        </li>
                    </ul>
                </div>
            </div>
        </nav>

        <!-- Page Content -->
        
        <div class="container containertop">
            
            <a id="anchorId" runat="server" onclick="return true" onserverclick="SearchItem"></a> 
            <br />

            <table class="main-table">
                <tbody style="vertical-align: top;">
                <tr >
                    <td id="col1" class="header notes-item">ITEM</td>
                    <td id="col2" class="header notes-item">One Portion</td>
                    <td id="col3"></td>
                    <td id="col4"></td>
                    <td id="col5"></td>
                    <td id="col6"></td>
                    <td id="col7"  rowspan="6">
                        <table class="notes-table">
                            <tr>
                                <td class="tabledivider header notes-item">Allergens</td>
                            </tr>
                            <tr>
                                <td class="tabledivider notes-item"><asp:Label ID="lblAllergen" CssClass="txtdisable input" runat="server" Text="None"></asp:Label></td>
                            </tr>
                            <tr>
                                <td class="tabledivider"></td>
                            </tr>
                            <tr>
                                <td class="tabledivider header notes-item">Method</td>
                            </tr>
                            <tr>
                                <td class="notes-item"><asp:TextBox ID="txtMethod" class="wh100 txtdisable input" TextMode="MultiLine" runat="server"></asp:TextBox>
                                    <asp:Label ID="lblMethod" CssClass="txtdisable input" runat="server" Visible="false"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td class="tabledivider"></td>
                            </tr>
                            <tr>
                                <td class="tabledivider header notes-item">Visual Notes</td>
                            </tr>
                            <tr>
                                <td class="tableTextbox notes-item"><asp:TextBox ID="txtVisual" class="wh100 txtdisable input" TextMode="MultiLine" runat="server"></asp:TextBox>
                                    <asp:Label ID="lblVisual" CssClass="txtdisable input" runat="server" Visible="false"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td class="tabledivider"></td>
                            </tr>
                            <tr>
                                <td class="tabledivider header notes-item">Tasting Notes</td>
                            </tr>
                            <tr>
                                <td class="tableTextbox notes-item"><asp:TextBox ID="txtTasting" class="wh100 txtdisable input" TextMode="MultiLine" runat="server"></asp:TextBox>
                                    <asp:Label ID="lblTasting" CssClass="txtdisable input" runat="server" Visible="false"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td class="tabledivider"></td>
                            </tr>
                            <tr>
                                <td class="tabledivider header notes-item">Prep Notes/ Shelf Life</td>
                            </tr>
                            <tr>
                                <td class="tableTextbox notes-item"><asp:TextBox ID="txtOther" class="wh100 txtdisable input" TextMode="MultiLine" runat="server"></asp:TextBox>
                                    <asp:Label ID="lblOther" CssClass="txtdisable input" runat="server" Visible="false"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td class="zerodivider"><asp:Button ID="btnUpdate" CssClass="update disable" runat="server" Text="Update" OnClick="btnUpdate_Click" /></td>
                            </tr>
                        </table>
                    </td> <!--Allergen?-->
                </tr>
                <tr id="row2">
                    <td class="notes-item"><asp:TextBox ID="txtItem" class="wh100 input" onFocus="this.select()" runat="server"></asp:TextBox><asp:HiddenField ID="hfItemNo" runat="server" />
                    </td>
                    <td class="notes-item"><asp:DropDownList ID="ddlPortion" class="wh100 ddlUnit input" runat="server">
                        <asp:ListItem Value="0.083">1 tsp</asp:ListItem>
                        <asp:ListItem Value="0.125">0.5 TBL</asp:ListItem>
                        <asp:ListItem Value="0.25">1 TBL</asp:ListItem>
                        <asp:ListItem Value="0.5">2 TBL</asp:ListItem>
                        <asp:ListItem Value="1">0.25 C</asp:ListItem>
                        <asp:ListItem Value="2">0.5 C</asp:ListItem>
                        <asp:ListItem Value="3">0.75 C</asp:ListItem>
                        <asp:ListItem Value="4">1 C</asp:ListItem>
                        <asp:ListItem Value="5">1.25 C</asp:ListItem>
                        <asp:ListItem Value="6">1.5 C</asp:ListItem>
                        <asp:ListItem Value="7">1.75 C</asp:ListItem>
                        <asp:ListItem Value="8">2 C</asp:ListItem>
                    </asp:DropDownList>
                        <asp:HiddenField ID="hfScale" Value="500" runat="server" />
                    </td>
                    <td></td>
                    <td style="text-align: right;" class=""></td>
                    <td></td>
                    <td></td>
                </tr>
                <tr id="row3">
                    <td></td>
                    <td></td>
                    <td></td>
                    <td></td>
                    <td></td>
                    <td></td>
                </tr>
                <tr id="row4" >
                    <td class="highlightCell" colspan="5">
                        <asp:HiddenField ID="hfBomCount" Value="0" runat="server" />
                        <asp:GridView ID="gvBom" Style="font-size: 13px" runat="server" AutoGenerateColumns="False" Width="100%" ShowFooter="false" BorderColor="#DEDFDE" BorderStyle="None" BorderWidth="1px" CellPadding="4" EmptyDataText="Please select an item.">
                            <Columns>
                                <asp:TemplateField HeaderText="Quantity">
                                    <ItemTemplate>
                                        <asp:Label runat="server" ID="lblQty"></asp:Label>
                                        <asp:Label runat="server" Text='<%# Bind("UOM") %>' ID="lblUOM"></asp:Label>
                                        <asp:HiddenField ID="hfQty" Value='<%# Bind("Quantity") %>' runat="server" />
                                    </ItemTemplate>
                                    <HeaderStyle Width="80px"></HeaderStyle>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Component">
                                    <ItemTemplate>
                                        <asp:Label runat="server" Text='<%# Bind("Item") %>' ID="lblItem"></asp:Label>
                                        <asp:HiddenField ID="hfG" Value='<%# Bind("G") %>' runat="server" />
                                        <asp:HiddenField ID="hfTSP" Value='<%# Bind("TSP") %>' runat="server" />
                                    </ItemTemplate>
                                    <HeaderStyle ></HeaderStyle>
                                </asp:TemplateField>
                                <asp:TemplateField ItemStyle-HorizontalAlign="Right" HeaderStyle-HorizontalAlign="Right">
                                    <HeaderTemplate>
                                        <asp:Label runat="server" Text="Scale to: " ID="lblScaleto"></asp:Label>
                                        <asp:TextBox ID="txtScale" CssClass="input" Width="60px" onFocus="this.select()" runat="server" autocomplete="off"></asp:TextBox>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:Label runat="server" ID="lblScale"></asp:Label>
                                        <asp:DropDownList ID="ddlUnit" CssClass="ddlUnit" Text='<%# Bind("DDL") %>' runat="server">
                                            <asp:ListItem>LB</asp:ListItem>
                                            <asp:ListItem>OZ</asp:ListItem>
                                            <asp:ListItem>KG</asp:ListItem>
                                            <asp:ListItem>G</asp:ListItem>
                                            <asp:ListItem>GL</asp:ListItem>
                                            <asp:ListItem>LT</asp:ListItem>
                                            <asp:ListItem>QT</asp:ListItem>
                                            <asp:ListItem>CUP</asp:ListItem>
                                            <asp:ListItem>FLOZ</asp:ListItem>
                                            <asp:ListItem>TBL</asp:ListItem>
                                            <asp:ListItem>TSP</asp:ListItem>
                                        </asp:DropDownList>
                                    </ItemTemplate>
                                    <HeaderStyle Width="125px"></HeaderStyle>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </td>
                    <td></td>
                </tr>
                <tr id="row5">
                    <td></td>
                    <td></td>
                    <td></td>
                    <td></td>
                    <td></td>
                    <td></td>
                </tr>
                <tr id="row6">
                    <td colspan="5">
                        <table class="notes-table">
                            <tr class="tabledivider">
                                <td class="header notes-item photowidth">Photo - Top</td>
                                <td class="photodivider"></td>
                                <td class="header notes-item photowidth">Photo - Side</td>
                                <td class="photodivider"></td>
                                <td class="header notes-item photowidth">Photo - Smear</td>
                            </tr>
                            <tr>
                                <td class="notes-item center"><asp:Image ID="imgTop" CssClass="ImageSize" runat="server" /></td>
                                <td></td>
                                <td class="notes-item center"><asp:Image ID="imgSide" CssClass="ImageSize" runat="server" /></td>
                                <td></td>
                                <td class="notes-item center"><asp:Image ID="imgSmear" CssClass="ImageSize" runat="server" /></td>
                            </tr>
                            <tr class="zerodivider">
                                <td class="notes-item">
                                    <table class="notes-table"><tr><td><asp:FileUpload ID="fuTop" CssClass="FileUpload disable" runat="server" /></td><td class="photodivider"><asp:Button ID="lbtnTop" CssClass="upload disable" OnClick="btnTop_Click" runat="server"></asp:Button></td></tr></table>
                                    </td>
                                <td></td>
                                <td class="notes-item">
                                    <table class="notes-table"><tr><td><asp:FileUpload ID="fuSide" CssClass="FileUpload disable" runat="server" /></td><td class="photodivider"><asp:Button ID="lbtnSide" CssClass="upload disable" OnClick="btnSide_Click" runat="server"></asp:Button></td></tr></table>
                              </td>
                                <td></td>
                                <td class="notes-item">
                                    <table class="notes-table"><tr><td><asp:FileUpload ID="fuSmear" CssClass="FileUpload disable" runat="server" /></td><td class="photodivider"><asp:Button ID="lbtnSmear" CssClass="upload disable" OnClick="btnSmear_Click" runat="server"></asp:Button></td></tr></table>
                              </td>
                            </tr>
                        </table>
                    </td>
                    <td></td>
                </tr>
            </table>

        </div>
        
    </form>
    <!-- Bootstrap core JavaScript -->
    <script src="vendor/bootstrap/js/bootstrap.bundle.min.js"></script>



</body>
</html>
<script runat="server">
    void Logout_Click(Object sender, EventArgs e)
    {
        FormsAuthentication.SignOut();
        FormsAuthentication.RedirectToLoginPage();
    }
</script>
