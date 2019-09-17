using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Data.SqlClient;
using DayPilot.Web.Ui;
using System.Collections;
using System.Data.Services.Client;
using System.Net;
using System.Drawing;
using System.Web.Services;
using System.Web.Script.Services;

namespace Compendiums
{
    using System.IO;

    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["CompendiumsConnectionString1"].ConnectionString);
            conn.Open();
            SqlCommand AppList_cmd = new SqlCommand("SELECT * FROM AppLink", conn);
            SqlDataAdapter app_da = new SqlDataAdapter(AppList_cmd);
            DataTable app_dt = new DataTable();
            app_da.Fill(app_dt);
            for (int i = 0; i < app_dt.Rows.Count; i++)
            {
                HyperLink hyperlink = new HyperLink();
                hyperlink.Text = app_dt.Rows[i]["App"].ToString();
                hyperlink.NavigateUrl = app_dt.Rows[i]["Link"].ToString(); ;
                Panel1.Controls.Add(hyperlink);
            }
            if (!IsPostBack)
            {
                    
                SqlCommand permiss = new SqlCommand("SELECT COUNT(*) FROM Permission WHERE Account2000 = '" + Context.User.Identity.Name + "' AND AppName='Compendiums';", conn);
                int permiss_count = Convert.ToInt32(permiss.ExecuteScalar());
                if (permiss_count == 0)
                {
                    conn.Close(); //close the connection
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "redirect", "alert('Sorry! You do not have access to this application.'); window.location='../';", true);
                }
                else
                {
                    
                    SqlCommand comm = new SqlCommand("SELECT Permission FROM Permission WHERE Account2000 = '" + Context.User.Identity.Name + "' AND AppName='Compendiums';", conn);
                    String Permission = Convert.ToString(comm.ExecuteScalar());
                    SqlCommand DC_comm = new SqlCommand("SELECT (CASE WHEN x.OfficeID=0 then 'ALL' else y.Office end)'Office' FROM Permission x LEFT JOIN DC y ON y.OfficeID=x.OfficeID WHERE Account2000 = '" + Context.User.Identity.Name + "' AND AppName='Compendiums';", conn);
                    String DC = Convert.ToString(DC_comm.ExecuteScalar());
                    DateTime today = DateTime.Now;
                    SqlCommand commdate = new SqlCommand("UPDATE Permission SET LastLogin='" + today + "' WHERE Account2000 = '" + Context.User.Identity.Name + "' AND AppName='Compendiums';", conn);
                    commdate.ExecuteScalar();
                    Session["Permission"] = Permission;
                    Session["DC"] = DC;
                    if (Permission == "Basic")
                    {
                        Manage_Users.Visible = false;
                        txtMethod.Enabled = false;
                        txtVisual.Enabled = false;
                        txtTasting.Enabled = false;
                        txtOther.Enabled = false;
                        txtMethod.CssClass = "wh100 txtdisable input";
                        txtVisual.CssClass = "wh100 txtdisable input";
                        txtTasting.CssClass = "wh100 txtdisable input";
                        txtOther.CssClass = "wh100 txtdisable input";
                        btnUpdate.Visible = false;
                        //fuTop.Visible = false;
                        //fuSide.Visible = false;
                        //fuSmear.Visible = false;
                        //lbtnTop.Visible = false;
                        //lbtnSide.Visible = false;
                        lbtnSmear.Visible = false;
                    }else if(Permission == "Kitchen")
                    {
                        Manage_Users.Visible = false;
                    }
                    gvBom.DataBind();
                    ddlPortion.Text = "1";
                    ScriptManager.RegisterStartupScript(Page, this.GetType(), "Key", "<script>Disable(); </script>", false);
                }
                    
            }
            conn.Close();
        }
        protected void GetData(string item)
        {
            try
            {
                string sql = @"SELECT (x.[No_]+' — '+x.[Description])'Item',convert(decimal(8,2), x.[Quantity])'Quantity',x.[Unit of Measure Code]'UOM',
MAX(CASE WHEN x.[Unit of Measure Code]='LB' THEN x.[Quantity]*453.59
WHEN x.[Unit of Measure Code]='OZ' THEN x.[Quantity]*28.35
WHEN x.[Unit of Measure Code]='KG' THEN x.[Quantity]*1000
WHEN x.[Unit of Measure Code]='G' THEN x.[Quantity]
WHEN x.[Unit of Measure Code]='GL' AND y.UOM='TSP' THEN x.[Quantity]*768*y.Grams_UOM
WHEN x.[Unit of Measure Code]='LT' AND y.UOM='TSP' THEN x.[Quantity]*202.88*y.Grams_UOM
WHEN x.[Unit of Measure Code]='QT' AND y.UOM='TSP' THEN x.[Quantity]*192*y.Grams_UOM
WHEN x.[Unit of Measure Code]='CUP' AND y.UOM='TSP' THEN x.[Quantity]*48*y.Grams_UOM
WHEN x.[Unit of Measure Code]='FLOZ' AND y.UOM='TSP' THEN x.[Quantity]*6*y.Grams_UOM
WHEN x.[Unit of Measure Code]='TSP' AND y.UOM='TSP' THEN x.[Quantity]*y.Grams_UOM
WHEN x.[Unit of Measure Code]='PORTION' AND z.Code='G' THEN x.[Quantity]*z.[Qty_ per Unit of Measure]
WHEN x.[Unit of Measure Code]='CT' AND z.Code='OZ' THEN x.[Quantity]*z.[Qty_ per Unit of Measure]*28.35
ELSE -1 END)'G',
MAX(CASE WHEN x.[Unit of Measure Code]='LB' AND y.UOM='TSP' THEN (x.[Quantity]*453.59)/y.Grams_UOM
WHEN x.[Unit of Measure Code]='OZ' AND y.UOM='TSP' THEN (x.[Quantity]*28.35)/y.Grams_UOM
WHEN x.[Unit of Measure Code]='KG' AND y.UOM='TSP' THEN (x.[Quantity]*1000)/y.Grams_UOM
WHEN x.[Unit of Measure Code]='G' AND y.UOM='TSP' THEN x.[Quantity]/y.Grams_UOM
WHEN x.[Unit of Measure Code]='GL' THEN x.[Quantity]*768
WHEN x.[Unit of Measure Code]='LT' THEN x.[Quantity]*202.88
WHEN x.[Unit of Measure Code]='QT' THEN x.[Quantity]*192
WHEN x.[Unit of Measure Code]='CUP' THEN x.[Quantity]*48
WHEN x.[Unit of Measure Code]='FLOZ' THEN x.[Quantity]*6
WHEN x.[Unit of Measure Code]='TSP' THEN x.[Quantity]
WHEN x.[Unit of Measure Code]='PORTION' AND z.Code='G' AND y.UOM='TSP' THEN (x.[Quantity]*z.[Qty_ per Unit of Measure])/y.Grams_UOM
WHEN x.[Unit of Measure Code]='CT' AND z.Code='OZ' AND y.UOM='TSP' THEN (x.[Quantity]*z.[Qty_ per Unit of Measure]*28.35)/y.Grams_UOM
ELSE -1 END)'TSP',
(CASE WHEN x.[Unit of Measure Code]='LB' THEN 'LB'
WHEN x.[Unit of Measure Code]='OZ' THEN 'LB'
WHEN x.[Unit of Measure Code]='KG' THEN 'LB'
WHEN x.[Unit of Measure Code]='G' THEN 'LB'
WHEN x.[Unit of Measure Code]='GL' THEN 'QT'
WHEN x.[Unit of Measure Code]='LT' THEN 'QT'
WHEN x.[Unit of Measure Code]='QT' THEN 'QT'
WHEN x.[Unit of Measure Code]='CUP' THEN 'QT'
WHEN x.[Unit of Measure Code]='FLOZ' THEN 'QT'
WHEN x.[Unit of Measure Code]='TSP' THEN 'QT'
ELSE 'LB' END)'DDL'
FROM [SUNBASKET_1000_TEST].[dbo].[Receiving$Production BOM Line] x
LEFT JOIN [SUNBASKET_1000_TEST].[dbo].[Receiving$Item] y ON y.No_=x.No_
LEFT JOIN [SUNBASKET_1000_TEST].[dbo].[Receiving$Item Unit of Measure] z ON z.[Item No_]=x.No_
WHERE x.[Production BOM No_]='" + item + @"' AND (([Starting Date]<=GETDATE() AND [Ending Date]>=GETDATE()) OR ([Starting Date]<=GETDATE() AND [Ending Date]='1753-01-01') OR ([Starting Date]='1753-01-01' AND [Ending Date]>=GETDATE()))
GROUP BY x.No_,x.Description,x.Quantity,x.[Unit of Measure Code]";
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["CompendiumsConnectionString2"].ConnectionString);
                con.Open();
                SqlCommand cmd = new SqlCommand(sql, con);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                gvBom.DataSource = dt;
                gvBom.DataBind();
                hfBomCount.Value = dt.Rows.Count.ToString();
                sql = @"CREATE TABLE #Final (
                    Item varchar(255),
                    Allergen varchar(255)
                    )
                    CREATE TABLE #Final2 (
                    Item varchar(255),
                    Allergen varchar(255)
                    )
                    DECLARE @item VARCHAR(50)
                    DECLARE @allg VARCHAR(10)
                    DECLARE @prefix VARCHAR(255)

                    INSERT INTO #Final
                    SELECT 'test',[Allergen Code]
                    FROM [SUNBASKET_1000_TEST].[dbo].[Receiving$Item Allergen] 
                    WHERE [Item No_]='" + item + @"'

                    While (Select Count(*) From #Final) > 0
                    BEGIN
                    Select Top 1 @item = Item From #Final
                    Select Top 1 @allg = Allergen From #Final
                    IF (Select Count(*) From #Final WHERE Item=@item AND (Allergen LIKE LEFT(@allg,2)+'%' OR Allergen IS NULL)) = 1
                    BEGIN
                    INSERT INTO #Final2 Values(@item,(SELECT [Description] FROM [SUNBASKET_1000_TEST].[dbo].[Receiving$Allergen] WHERE [Code]=@allg))
                    DELETE FROM #Final WHERE Item=@item AND (Allergen=@allg OR Allergen IS NULL)
                    END
                    ELSE
                    BEGIN
                    DELETE FROM #Final WHERE Item=@item AND Allergen=LEFT(@allg,2)
                    Select Top 1 @prefix = SUBSTRING([Description],0, CHARINDEX('(',[Description])) From #Final LEFT JOIN [SUNBASKET_1000_TEST].[dbo].[Receiving$Allergen] ON Code=Allergen COLLATE DATABASE_DEFAULT WHERE Allergen LIKE LEFT(@allg,2)+'%'
                    INSERT INTO #Final2
                    SELECT @item,RTRIM(@prefix)+' ('+STUFF((SELECT ', ' + CAST((SELECT SUBSTRING([Description],CHARINDEX('(',[Description])+1, CHARINDEX(')',[Description],CHARINDEX('(',[Description])+1)-CHARINDEX('(',[Description])-1)) AS VARCHAR(80)) AS [text()] FROM [SUNBASKET_1000_TEST].[dbo].[Receiving$Allergen] LEFT JOIN #Final ON Allergen=Code COLLATE DATABASE_DEFAULT  WHERE Allergen LIKE LEFT(@allg,2)+'%' AND Item=@item FOR XML PATH('')), 1, 2, NULL)+')'
                    DELETE FROM #Final WHERE Item=@item AND Allergen LIKE LEFT(@allg,2)+'%'
                    END
                    END

                    SELECT 'Contains: ' + STUFF((SELECT ', ' + CAST(Allergen AS VARCHAR(80)) AS [text()] FROM #Final2 y WHERE y.Item=Item FOR XML PATH('')), 1, 2, NULL) 'Allergen'
                    FROM #Final2
                    GROUP BY Item
                    DROP TABLE #Final
                    DROP TABLE #Final2";
                cmd = new SqlCommand(sql, con);
                String allergen = Convert.ToString(cmd.ExecuteScalar());
                if (allergen != "")
                {
                    lblAllergen.Text = allergen;
                }
                else
                {
                    lblAllergen.Text = "None";
                }

                con = new SqlConnection(ConfigurationManager.ConnectionStrings["CompendiumsConnectionString1"].ConnectionString);
                con.Open();
                cmd = new SqlCommand("SELECT [PREP],[VISUAL],[TASTE],[OTHER] FROM [Operation].[dbo].[Compendiums] WHERE ITEM='"+item+"'", con);
                da = new SqlDataAdapter(cmd);
                dt = new DataTable();
                da.Fill(dt);
                if(dt.Rows.Count>0)
                {
                    txtMethod.Text = dt.Rows[0][0].ToString();
                    lblMethod.Text = dt.Rows[0][0].ToString();
                    txtVisual.Text = dt.Rows[0][1].ToString();
                    lblVisual.Text = dt.Rows[0][1].ToString();
                    txtTasting.Text = dt.Rows[0][2].ToString();
                    lblTasting.Text = dt.Rows[0][2].ToString();
                    txtOther.Text = dt.Rows[0][3].ToString();
                    lblOther.Text = dt.Rows[0][3].ToString();
                }
                else
                {
                    txtMethod.Text = "";
                    lblMethod.Text = "";
                    txtVisual.Text = "";
                    lblVisual.Text = "";
                    txtTasting.Text = "";
                    lblTasting.Text = "";
                    txtOther.Text = "";
                    lblOther.Text = "";
                }
                con.Close();
                Random rnd = new Random();
                int random = rnd.Next(10000, 99999);
                //if (File.Exists(Server.MapPath("photo/" + item + "-top.jpg")))
                //{
                //    imgTop.ImageUrl = "photo/" + item + "-top.jpg?" + random;
                //}
                //else
                //{
                //    imgTop.ImageUrl = "img/noimg.png";
                //}
                //if (File.Exists(Server.MapPath("photo/" + item + "-side.jpg")))
                //{
                //    imgSide.ImageUrl = "photo/" + item + "-side.jpg?" + random;
                //}
                //else
                //{
                //    imgSide.ImageUrl = "img/noimg.png";
                //}
                if (File.Exists(Server.MapPath("photo/" + item + "-smear.jpg")))
                {
                    imgSmear.ImageUrl = "photo/" + item + "-smear.jpg?" + random;
                }
                else
                {
                    imgSmear.ImageUrl = "img/noimg.png";
                }
            }
            catch (Exception ex)
            {
                string error = ex.Message.Replace('"', ' ').Replace("'", " ");
                ScriptManager.RegisterStartupScript(Page, this.GetType(), "Key", "<script>Disable(); </script>", false);
                Response.Write("<script>alert('" + error + "');</script>");
            }
            

        }
        protected void SearchItem(object sender, EventArgs e)
        {
            string no = txtItem.Text.Substring(0, 5);
            hfItemNo.Value = no;
            GetData(no);
            ScriptManager.RegisterStartupScript(Page, this.GetType(), "Key", "<script>Calculate(); </script>", false);
        }

        //protected void btnTop_Click(object sender, EventArgs e)
        //{
        //    Boolean fileOK = false;
        //    String path = Server.MapPath("~/photo/");
        //    if (fuTop.HasFile)
        //    {
        //        String fileExtension = System.IO.Path.GetExtension(fuTop.FileName).ToLower();

        //        if (fileExtension == ".gif" || fileExtension == ".png" || fileExtension == ".jpeg" || fileExtension == ".jpg")
        //        {
        //            fileOK = true;
        //        }

        //    }

        //    if (fileOK)
        //    {
        //        try
        //        {
        //            // Load the image.
        //            System.Drawing.Image image1 = System.Drawing.Image.FromStream(fuTop.FileContent);
        //            // Save the image in JPEG format.
        //            image1.Save(path + hfItemNo.Value + "-top.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
                    
        //            Response.Write("<script>alert('File uploaded!');</script>");
        //            GetData(hfItemNo.Value);
        //            ScriptManager.RegisterStartupScript(Page, this.GetType(), "Key", "<script>Calculate(); </script>", false);
        //        }
        //        catch (Exception ex)
        //        {
        //            Response.Write("<script>alert('File could not be uploaded.');</script>");
        //        }
        //    }
        //    else
        //    {
        //        Response.Write("<script>alert('Cannot accept files of this type.');</script>");
        //    }
        //    ScriptManager.RegisterStartupScript(Page, this.GetType(), "Key", "<script>Calculate(); </script>", false);

        //}

        //protected void btnSide_Click(object sender, EventArgs e)
        //{
        //    Boolean fileOK = false;
        //    String path = Server.MapPath("~/photo/");
        //    if (fuSide.HasFile)
        //    {
        //        String fileExtension = System.IO.Path.GetExtension(fuSide.FileName).ToLower();

        //        if (fileExtension == ".gif" || fileExtension == ".png" || fileExtension == ".jpeg" || fileExtension == ".jpg")
        //        {
        //            fileOK = true;
        //        }

        //    }

        //    if (fileOK)
        //    {
        //        try
        //        {
        //            // Load the image.
        //            System.Drawing.Image image1 = System.Drawing.Image.FromStream(fuSide.FileContent);
        //            // Save the image in JPEG format.
        //            image1.Save(path + hfItemNo.Value + "-side.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);

        //            Response.Write("<script>alert('File uploaded!');</script>");
        //            GetData(hfItemNo.Value);
        //            ScriptManager.RegisterStartupScript(Page, this.GetType(), "Key", "<script>Calculate(); </script>", false);
        //        }
        //        catch (Exception ex)
        //        {
        //            Response.Write("<script>alert('File could not be uploaded.');</script>");
        //        }
        //    }
        //    else
        //    {
        //        Response.Write("<script>alert('Cannot accept files of this type.');</script>");
        //    }
        //    ScriptManager.RegisterStartupScript(Page, this.GetType(), "Key", "<script>Calculate(); </script>", false);
        //}

        protected void btnSmear_Click(object sender, EventArgs e)
        {
            Boolean fileOK = false;
            String path = Server.MapPath("~/photo/");
            if (fuSmear.HasFile)
            {
                String fileExtension = System.IO.Path.GetExtension(fuSmear.FileName).ToLower();

                if (fileExtension == ".gif" || fileExtension == ".png" || fileExtension == ".jpeg" || fileExtension == ".jpg")
                {
                    fileOK = true;
                }

            }

            if (fileOK)
            {
                try
                {
                    // Load the image.
                    System.Drawing.Image image1 = System.Drawing.Image.FromStream(fuSmear.FileContent);
                    // Save the image in JPEG format.
                    image1.Save(path + hfItemNo.Value + "-smear.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);

                    Response.Write("<script>alert('File uploaded!');</script>");
                    GetData(hfItemNo.Value);
                    ScriptManager.RegisterStartupScript(Page, this.GetType(), "Key", "<script>Calculate(); </script>", false);
                }
                catch (Exception ex)
                {
                    Response.Write("<script>alert('File could not be uploaded.');</script>");
                }
            }
            else
            {
                Response.Write("<script>alert('Cannot accept files of this type.');</script>");
            }
            ScriptManager.RegisterStartupScript(Page, this.GetType(), "Key", "<script>Calculate(); </script>", false);
        }

        //TextBox Auto Complete
        [WebMethod]
        public static List<string> GetItems(string item)
        {
            List<string> itemsResult = new List<string>();
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["CompendiumsConnectionString2"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    //cmd.CommandText = "SELECT TOP (10) [Account]'Item' FROM [Operation].[dbo].[PrepTrackerPermission] WHERE Account LIKE '%'+@Search+'%'";
                    cmd.CommandText = "SELECT TOP(10) (CASE WHEN [No_ 2] != '' THEN No_+' - '+[No_ 2]+' - '+[Description] COLLATE DATABASE_DEFAULT ELSE No_+' - '+[Description] COLLATE DATABASE_DEFAULT END)'Item' FROM [SUNBASKET_1000_TEST].[dbo].[Receiving$Item] WHERE No_ LIKE '2%' AND Blocked=0 AND (No_ LIKE '%'+@Search+'%' OR LOWER([No_ 2]) LIKE '%'+@Search+'%' OR LOWER([Description]) LIKE '%'+@Search+'%')";
                    cmd.Connection = con;
                    con.Open();
                    cmd.Parameters.AddWithValue("@Search", item.ToLower());
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        itemsResult.Add(dr["Item"].ToString());
                    }
                    con.Close();
                    return itemsResult;
                }
            }
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtMethod.Text != lblMethod.Text || txtVisual.Text != lblVisual.Text || txtTasting.Text != lblTasting.Text || txtOther.Text != lblOther.Text)
                {
                    using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["CompendiumsConnectionString1"].ConnectionString))
                    {
                        using (SqlCommand cmd = new SqlCommand())
                        {
                            cmd.CommandText = "IF EXISTS (SELECT * FROM [Operation].[dbo].[Compendiums] WHERE ITEM=@item ) UPDATE [Operation].[dbo].[Compendiums] SET PREP=@method, VISUAL=@visual, TASTE=@taste, OTHER=@other WHERE ITEM=@item ELSE INSERT INTO [Operation].[dbo].[Compendiums] VALUES (@item,@method,@visual,@taste,@other)";
                            cmd.Connection = con;
                            con.Open();
                            cmd.Parameters.AddWithValue("@method", txtMethod.Text);
                            cmd.Parameters.AddWithValue("@visual", txtVisual.Text);
                            cmd.Parameters.AddWithValue("@taste", txtTasting.Text);
                            cmd.Parameters.AddWithValue("@other", txtOther.Text);
                            cmd.Parameters.AddWithValue("@item", hfItemNo.Value);
                            cmd.ExecuteNonQuery();
                            con.Close();
                        }
                    }
                    Response.Write("<script>alert('Update Successful!');</script>");
                    ScriptManager.RegisterStartupScript(Page, this.GetType(), "Key", "<script>Calculate(); </script>", false);
                }
            }
            catch(Exception ex)
            {
                string error = ex.Message.Replace('"', ' ').Replace("'", " ");
                Response.Write("<script>alert('" + error + "');</script>");
            }
        }
    }
}