﻿using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxControlToolkit.HTMLEditor.ToolbarButton;
using Core;
using System;
using System.Collections.Generic;
using System.Text;
using Core.Doska;
using ImageButton = System.Web.UI.WebControls.ImageButton;

namespace Site.Modules.Bulletins
{
    public partial class Default : Core.SitePage
    {
        private string _moderator;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            //moderator.Visible = Permissions["Дошка оголошень"].Create;

            //var items = CampusClient.GetBulletinBoard(SessionId);

            //Render(items);

            _moderator = CampusClient.DeskIsModerator(SessionId);
            LoadBoard();
            foreach (var v in CampusClient.DeskGetAllowedProfiles())
            {
                drop1.Items.Add(v.Name);
            }

            foreach (var v in CampusClient.DeskGetFacultyTypesList())
            {
                drop2.Items.Add(v.Name);
            }

        }

        private void Render(IEnumerable<Campus.Common.BulletinBoard> items)
        {
            var sb = new StringBuilder();

            sb.AppendLine("<table class=\"table  table-condensed  table-hover\">");

            foreach (var item in items)
            {
                sb.AppendLine("<tr>");
                sb.AppendLine("<td>");
                sb.AppendFormat("<div class=\"date\">{0}</div>", item.DateCreate);
                sb.AppendFormat("<span class=\"poster\">{0}</span>", String.IsNullOrEmpty(item.CreatorUserFullName) ? "Анонім" : item.CreatorUserFullName);
                sb.AppendFormat("<h4 class=\"text-primary\">{0}</h4>", item.Subject);
                sb.AppendFormat("<p class=\"text-success\">{0}</p>", item.Text);
                sb.AppendLine("</td>");
                sb.AppendLine("</tr>");
            }

            sb.AppendLine("</table>");

            bulletins.InnerHtml = sb.ToString();
        }

        public void ChangeBulletin(TextBox b1, TextBox b2)
        {
            CampusClient.DeskAddBulletein(SessionId, b1.Text, b2.Text);
        }

        public void EditBulletin(Bulletin l)
        {
            ActualBulletinDiv.InnerHtml = "";
            var box1 = new TextBox();
            box1.Text = l.Subject;
            box1.Width = 500;

            var box2 = new TextBox();
            box2.Text = l.Text;
            box2.Width = 900;
            box2.TextMode = TextBoxMode.MultiLine;
            box2.Height = 300;

            var button1 = new Button();
            button1.Text = "Змінити";
            button1.Click += (sender, args) =>
            {
                CampusClient.DeskRemoveBulletin(l.Subject, l.Text);
                ChangeBulletin(box1, box2);
                LoadBoard();
            };

            var button2 = new Button();
            button2.Text = "Відмінити";
            button2.Click += (sender, args) =>
            {
                LoadBoard();
            };


            string header2 = "<table class=\"header-table\"><tr><td rowspan=\"2\"><div style=\"text-align: left;\">";

            string header3 = "</div></td>" +
                                "<td><div style=\"text-align: right;\">" +
                                l.CreationDate +
                                "(Дата створення)</div></td></tr>" +
                                "<tr>" +
                                "<td><div style=\"text-align: right;\">публікатор " +
                                l.CreatorName +
                                "</div></td></tr></table>";
            string txt1 = "<div class=\"panel panel-default\"> " +
                          "<div class=\"panel-heading\" data-toggle=\"collapse\" data-parent=\"#accordion\" data-target=\"#collapse" +
                          "\">" +
                          "<h4 class=\"panel-title\">";

            string txt4 = "</h4>" +
                          "</div>" +
                          "<div id=\"collapse" + "\" class=\"panel-collapse collapse in\">" +
                          "<div class=\"panel-body\">";

            string txt5 = "</div></div></div>";


            ActualBulletinDiv.Controls.Add(new LiteralControl(txt1));
            ActualBulletinDiv.Controls.Add(new LiteralControl(header2));
            ActualBulletinDiv.Controls.Add(box1);
            ActualBulletinDiv.Controls.Add(new LiteralControl(header3));
            ActualBulletinDiv.Controls.Add(new LiteralControl(txt4));
            ActualBulletinDiv.Controls.Add(box2);
            ActualBulletinDiv.Controls.Add(new LiteralControl(txt5));
            ActualBulletinDiv.Controls.Add(new LiteralControl(txt5));
            ActualBulletinDiv.Controls.Add(button1);
            ActualBulletinDiv.Controls.Add(button2);
        }

        void LoadBoard()
        {
            //ActualBulletinDiv.InnerHtml += add_buletin("s", "t");

            var list = CampusClient.DeskGetActualBulletins(SessionId);
            ActualBulletinDiv.InnerHtml = "";
            ActualBulletinDiv.Controls.Add(new LiteralControl("<div class=\"panel-group\" id=\"accordion\">"));
            int i = 0;
            foreach (var l in list.Reverse())
            {
                string s = l.Subject;
                string t = l.Text;
                var b1 = new ImageButton();
                b1.AlternateText = "Видалити";
                b1.Click += (source, args) =>
                {
                    CampusClient.DeskRemoveBulletin(s, t);
                    LoadBoard();
                };
                var b2 = new ImageButton();
                b2.AlternateText = "Редагувати";
                b2.Click += (source, args) =>
                {

                };
                ActualBulletinDiv.Controls.Add(b1);
                ActualBulletinDiv.Controls.Add(b2);
                string header = "<table class=\"header-table\"><tr><td rowspan=\"2\"><div style=\"text-align: left;\">" +
                                l.Subject +
                                "</div></td>" +
                                "<td><div style=\"text-align: right;\">" +
                                l.CreationDate +
                                "(Дата створення)</div></td></tr>" +
                                "<tr>" +
                                "<td><div style=\"text-align: right;\">публікатор " +
                                l.CreatorName +
                                "</div></td></tr></table>";
                string txt = "<div class=\"panel panel-default\"> " +
                                "<div class=\"panel-heading\" data-toggle=\"collapse\" data-parent=\"#accordion\" data-target=\"#collapse" + i + "\">" +
                                "<h4 class=\"panel-title\">" +
                                header +
                                "</h4>" +
                                "</div>" +
                                "<div id=\"collapse" + i + "\" class=\"panel-collapse collapse \">" +
                                "<div class=\"panel-body\">" +
                                l.Text +
                                "</div></div></div>";
                ActualBulletinDiv.Controls.Add(new LiteralControl(txt));

                i++;
            }
            ActualBulletinDiv.Controls.Add(new LiteralControl("</div>"));

            /*
            ActualBulletinDiv.InnerHtml += "<div class=\"panel-group\" id=\"accordion\">";
            var i = 0;
            foreach (var bul in CampusClient.DeskGetActualBulletins(CampusClient.SessionId))
            {
                string txt = "<div class=\"panel panel-default\"> " +
                             "<div class=\"panel-heading\" data-toggle=\"collapse\" data-parent=\"#accordion\" data-target=\"#collapse" + i + "\">" +
                             "<h4 class=\"panel-title\">" + 
                             bul.Subject + 
                             "</h4>" +
                             "</div>" +
                             "<div id=\"collapse" + i + "\" class=\"panel-collapse collapse in\">" +
                             "<div class=\"panel-body\">" +
                             bul.Text +
                             "</div></div></div>";
                ActualBulletinDiv.InnerHtml += txt;
                i++;
            }
            ActualBulletinDiv.InnerHtml += "</div>";
            */
        }

        protected void add_buletin(object sender, EventArgs e)
        {
            string res = "REQUEST RESULT: " +
                             ((CampusClient.DeskAddBulletein(SessionId, sub_text.Text, text_text.Text) == "0")
                                 ? "Success"
                                 : "Fail");
            LoadBoard();
        }


        //<div class="panel-group" id="accordion">
        //            <div class="panel panel-default">
        //            <div class="panel-heading" data-toggle="collapse" data-parent="#accordion" data-target="#collapseOne">
        //                <h4 class="panel-title">
        //                    Сообщение #1
        //                </h4>
        //            </div>
        //            <div id="collapseOne" class="panel-collapse collapse in">
        //                <div class="panel-body">
        //                    Про що сьогодні (не) домовилися Україна, Росія та Єврокомісія - людською мовою. Підсумки сьогоднішніх газових перемовин у Берліні за участю єврокомісара Гюнтера Етінгера, російського міністра енергетики Олександра Новака та його українського візаві Юрія Продана стала топ-новиною сьогоднішнього вечора. Втім, практично жодне зі ЗМІ, яке поспішило оприлюднити повідомлення цю тему (як правило - із сенсаційним заголовком), не змогло коректно передати зміст того, про що домовилися (точніше, не домовилися) три сторони сьогодні в Берліні.
        //                    Про що сьогодні (не) домовилися Україна, Росія та Єврокомісія - людською мовою. Підсумки сьогоднішніх газових перемовин у Берліні за участю єврокомісара Гюнтера Етінгера, російського міністра енергетики Олександра Новака та його українського візаві Юрія Продана стала топ-новиною сьогоднішнього вечора. Втім, практично жодне зі ЗМІ, яке поспішило оприлюднити повідомлення цю тему (як правило - із сенсаційним заголовком), не змогло коректно передати зміст того, про що домовилися (точніше, не домовилися) три сторони сьогодні в Берліні.
        //                    Про що сьогодні (не) домовилися Україна, Росія та Єврокомісія - людською мовою. Підсумки сьогоднішніх газових перемовин у Берліні за участю єврокомісара Гюнтера Етінгера, російського міністра енергетики Олександра Новака та його українського візаві Юрія Продана стала топ-новиною сьогоднішнього вечора. Втім, практично жодне зі ЗМІ, яке поспішило оприлюднити повідомлення цю тему (як правило - із сенсаційним заголовком), не змогло коректно передати зміст того, про що домовилися (точніше, не домовилися) три сторони сьогодні в Берліні.
        //                </div>
        //            </div>
        //            </div>
        //            <div class="panel panel-default">
        //            <div class="panel-heading" data-toggle="collapse" data-parent="#accordion" data-target="#collapseOne1">
        //                <h4 class="panel-title">
        //                    Сообщение #2
        //                </h4>
        //            </div>
        //            <div id="collapseOne1" class="panel-collapse collapse">
        //                <div class="panel-body">
        //                    Про що сьогодні (не) домовилися Україна, Росія та Єврокомісія - людською мовою. Підсумки сьогоднішніх газових перемовин у Берліні за участю єврокомісара Гюнтера Етінгера, російського міністра енергетики Олександра Новака та його українського візаві Юрія Продана стала топ-новиною сьогоднішнього вечора. Втім, практично жодне зі ЗМІ, яке поспішило оприлюднити повідомлення цю тему (як правило - із сенсаційним заголовком), не змогло коректно передати зміст того, про що домовилися (точніше, не домовилися) три сторони сьогодні в Берліні.
        //                    Про що сьогодні (не) домовилися Україна, Росія та Єврокомісія - людською мовою. Підсумки сьогоднішніх газових перемовин у Берліні за участю єврокомісара Гюнтера Етінгера, російського міністра енергетики Олександра Новака та його українського візаві Юрія Продана стала топ-новиною сьогоднішнього вечора. Втім, практично жодне зі ЗМІ, яке поспішило оприлюднити повідомлення цю тему (як правило - із сенсаційним заголовком), не змогло коректно передати зміст того, про що домовилися (точніше, не домовилися) три сторони сьогодні в Берліні.
        //                    Про що сьогодні (не) домовилися Україна, Росія та Єврокомісія - людською мовою. Підсумки сьогоднішніх газових перемовин у Берліні за участю єврокомісара Гюнтера Етінгера, російського міністра енергетики Олександра Новака та його українського візаві Юрія Продана стала топ-новиною сьогоднішнього вечора. Втім, практично жодне зі ЗМІ, яке поспішило оприлюднити повідомлення цю тему (як правило - із сенсаційним заголовком), не змогло коректно передати зміст того, про що домовилися (точніше, не домовилися) три сторони сьогодні в Берліні.
        //                </div>
        //            </div>
        //            </div>
        //            <div class="panel panel-default">
        //            <div class="panel-heading" data-toggle="collapse" data-parent="#accordion" data-target="#collapseOne2">
        //                <h4 class="panel-title">
        //                    Сообщение #3
        //                </h4>
        //            </div>
        //            <div id="collapseOne2" class="panel-collapse collapse">
        //                <div class="panel-body">
        //                    Про що сьогодні (не) домовилися Україна, Росія та Єврокомісія - людською мовою. Підсумки сьогоднішніх газових перемовин у Берліні за участю єврокомісара Гюнтера Етінгера, російського міністра енергетики Олександра Новака та його українського візаві Юрія Продана стала топ-новиною сьогоднішнього вечора. Втім, практично жодне зі ЗМІ, яке поспішило оприлюднити повідомлення цю тему (як правило - із сенсаційним заголовком), не змогло коректно передати зміст того, про що домовилися (точніше, не домовилися) три сторони сьогодні в Берліні.
        //                    Про що сьогодні (не) домовилися Україна, Росія та Єврокомісія - людською мовою. Підсумки сьогоднішніх газових перемовин у Берліні за участю єврокомісара Гюнтера Етінгера, російського міністра енергетики Олександра Новака та його українського візаві Юрія Продана стала топ-новиною сьогоднішнього вечора. Втім, практично жодне зі ЗМІ, яке поспішило оприлюднити повідомлення цю тему (як правило - із сенсаційним заголовком), не змогло коректно передати зміст того, про що домовилися (точніше, не домовилися) три сторони сьогодні в Берліні.
        //                    Про що сьогодні (не) домовилися Україна, Росія та Єврокомісія - людською мовою. Підсумки сьогоднішніх газових перемовин у Берліні за участю єврокомісара Гюнтера Етінгера, російського міністра енергетики Олександра Новака та його українського візаві Юрія Продана стала топ-новиною сьогоднішнього вечора. Втім, практично жодне зі ЗМІ, яке поспішило оприлюднити повідомлення цю тему (як правило - із сенсаційним заголовком), не змогло коректно передати зміст того, про що домовилися (точніше, не домовилися) три сторони сьогодні в Берліні.
        //                </div>
        //            </div>
        //            </div>
        //        </div>

        /*
        protected void get_profile_type(object sender, EventArgs e)
        {
            output_box2.Text = CampusClient.DeskGetAllowedProfiles().ToStringList(info => info.Name);
        }


        protected void get_group_list_click(object sender, EventArgs e)
        {
            output_box4.Text = CampusClient.DeskGetGroupTypesList(10193).ToStringList(info => info.Name + "\n" + info.CreationYear);
        }

        protected void get_actual_click(object sender, EventArgs e)
        {
            output_box1.Text =
                CampusClient.DeskGetActualBulletins(SessionId)
                    .ToStringList(info => "Sub: " + info.Subject + "\n" + "Text: " + info.Text);
        }

        protected void get_faculty_list_click(object sender, EventArgs e)
        {
            output_box3.Text = CampusClient.DeskGetFacultyTypesList().ToStringList(info => info.Name);
        }

        protected void add_buletin_click(object sender, EventArgs e)
        {
            input_box.Text = "REQUEST RESULT: " +
                             ((CampusClient.DeskAddBulletein(SessionId, "test sub", input_box.Text) == "0")
                                 ? "Success"
                                 : "Fail");
        }


        protected void get_moderator(object sender, EventArgs e)
        {
            output_box6.Text = CampusClient.DeskIsModerator(SessionId).ToString();
        }


        /*
        protected void get_group_list_click(object sender, EventArgs e)
        {
            var result = CampusClient.MakeRequest<List<SimpleInfo>>(CampusClient.BuildUrl("bulletinboard", "deskgetprofiletypeslist"));
            output_box.Text = ToStringList(result, a => "Id: " + a.Id.ToString() + " Name: " + a.Name);
        }

        protected void get_actual_click(object sender, EventArgs e)
        {
            var result = CampusClient.MakeRequest<List<Bulletin>>(CampusClient.BuildUrl("bulletinboard", "deskgetactualbulletins", new { CampusClient.SessionId }));
            output_box.Text = ToStringList(result, a => "Author: " + a.CreatorName + " Description: " + a.Text);
        }

        protected void get_faculty_list_click(object sender, EventArgs e)
        {
            var result = CampusClient.MakeRequest<List<SimpleInfo>>(CampusClient.BuildUrl("bulletinboard", "deskgetfacultytypeslist"));
            output_box.Text = ToStringList(result, a => "Id: " + a.Id + " Name: " + a.Name);
        }

        protected void add_buletin_click(object sender, EventArgs e)
        {
            var bulletin = new Bulletin();
            bulletin.Text = input_box.Text;
            var args = String.Format("sessionid={0}bulletin={1}", CampusClient.SessionId, Newtonsoft.Json.JsonConvert.SerializeObject(bulletin));
            var url = CampusClient.BuildUrl("bulletinboard", "deskaddbulletei");
            var result = CampusClient.Request(url, "POST", args);
            output_box.Text = result;
        }

        
         */
    }
}