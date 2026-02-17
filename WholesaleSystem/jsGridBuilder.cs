using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LMWholesale
{
	public class jsGridBuilder
	{
		public string Height { get; set; }
		public string Width { get; set; }
		public bool Filtering { get; set; }
		public bool Editing { get; set; }
		public bool Sorting { get; set; }
		public bool Paging { get; set; }
		public bool PageLoading { get; set; }
		public bool AutoLoad { get; set; }
		public int PageSize { get; set; }
		public int PageButtonCount { get; set; }
		public int PageIndex { get; set; }
		public string PagerFormat { get; set; }
		public string MethodURL { get; set; }
		public string FieldList { get; set; }
		public string HTMLElement { get; set; }
		public List<string> NotSortableFields { get; set; }
		public List<string> FilterableFields { get; set; }
		public string OnRowSelectFunction { get; set; }
		public string OnClearRowSelectFunction { get; set; }
		public string OnDoubleClickFunction { get; set; }
		public string LoadMessage { get; set; }
		public string ExtraFunctionality { get; set; }
        public string PagerManipulation { get; set; }
		public string jsGridIdentifier { get; set; }
        public Dictionary<string, string> ExtraParameters { get; set; } = new Dictionary<string, string>();

		public jsGridBuilder()
		{
			NotSortableFields = new List<string>();
			FilterableFields = new List<string>();
			Height = Width = "auto";
			Filtering = Sorting = Paging = AutoLoad = PageLoading = true;
			PageSize = 25;
			PageButtonCount = 5;
			PageIndex = 1;
			PagerFormat = "Pages: {first} {prev} {pages} {next} {last} &nbsp;&nbsp;&nbsp; {pageIndex} of {pageCount} &nbsp;&nbsp; Total Items: {itemCount}";
			MethodURL = FieldList = "";
			HTMLElement = "jsGrid";
			LoadMessage = "Loading data. Please, wait...";
			jsGridIdentifier = "0";
        }

		public string RenderGrid(bool jsDebug = false)
		{
			string rowSelect = null;
			string rowDblClick = null;
			string onClear = null;
			string top = "$(function(){";
			string bottom = null;
			string extraFunctionality = null;

			if (!string.IsNullOrEmpty(OnRowSelectFunction))
			{
				rowSelect = $"{OnRowSelectFunction}(item);";
			}

			if (!string.IsNullOrEmpty(OnDoubleClickFunction))
			{
				rowDblClick = $"rowDoubleClick:function(args){{{OnDoubleClickFunction}}},";
			}

			if (!string.IsNullOrEmpty(OnClearRowSelectFunction))
			{
				onClear = $"{OnClearRowSelectFunction}();";
			}

			if (!string.IsNullOrEmpty(ExtraFunctionality))
			{
				extraFunctionality = $"{ExtraFunctionality}";
			}

			string[] AcceptedParams = new string[4] { "kListing", "kDealer", "kWholesaleAuctionName", "isSimple" };
			string json = $@"""{{'filter':'"" + JSON.stringify(filter) + ""'REPLACE_ME}}""";
			if (ExtraParameters.Count != 0)
			{
				List<string> remove = new List<string>();
				ExtraParameters.ForEach(param => { if (!AcceptedParams.Contains(param.Key)) remove.Add(param.Key); });
				remove.ForEach(r => ExtraParameters.Remove(r));

				json = json.Replace("REPLACE_ME", $@",'ExtraParams':'""+JSON.stringify({Util.serializer.Serialize(ExtraParameters)})+""'");
			}
			else
				json = json.Replace("REPLACE_ME", "");

			string jsGrid = $@"
			<script type=""text/javascript"">{top}
				$('#{HTMLElement}').jsGrid({{
					height: '{Height}',
					width: '{Width}',
					{FlagValue("filtering", Filtering)},
					{FlagValue("editing", Editing)},
					{FlagValue("sorting", Sorting)},
					{FlagValue("paging", Paging)},
					{FlagValue("pageLoading", PageLoading)},
					{FlagValue("autoload", AutoLoad)},
					pageSize: {PageSize},
					buttonCount: {PageButtonCount},
					pageIndex: {PageIndex},
					pagerFormat: ""{PagerFormat}"",
                    loadMessage: ""{LoadMessage}"",
					onDataLoaded: function(){{
						if (document.getElementsByClassName('jsgrid-grid-body')[{jsGridIdentifier}].scrollHeight > document.getElementsByClassName('jsgrid-grid-body')[{jsGridIdentifier}].clientHeight)
							document.getElementsByClassName('jsgrid-grid-header')[{jsGridIdentifier}].style.paddingRight = '10px';
						else
							document.getElementsByClassName('jsgrid-grid-header')[{jsGridIdentifier}].style.paddingRight = '0';
					}},
					onRefreshed: function(){{
						$('.jsgrid-pager').contents().filter(function(){{
							return this.nodeType == 3;
						}}).each(function(){{
							this.textContent = this.textContent.replace('{{itemsCount}}', $('#{HTMLElement}').jsGrid('_itemsCount'));
						}});

						var pagerItem = document.getElementById('{HTMLElement}').children[2].children[0];

						if (document.getElementsByClassName(""jsgrid-pager-page"").length < 2 )
						{{
							pagerItem.parentElement.style.display = ""block"";
							pagerItem.innerHTML = ""<span style='font-weight:bold;'>Total Items: "" + ($('#{HTMLElement}').data('JSGrid').data.length ?? 0) + ""</span>"";

							if ('{HTMLElement}' == 'vehicleManagementJSGrid') {{
								pagerItem.innerHTML += ""&nbsp;&nbsp;Items Per Page:&nbsp;&nbsp;<input type='text' id='pageSize' value='""+ $('#{HTMLElement}').jsGrid(""option"", ""pageSize"") +""' style='width:30px;height:18px;text-align:-webkit-center'>"";
								pagerItem.innerHTML += ""&nbsp;&nbsp;<button id='pageSizeBtn' onclick='javascript:SetPageSize();return false;' style='width:35px;height:18px;text-align:-webkit-center;margin:0;padding:0;color:white;background-color:#005589;font-size:10px;'>Set</button>""
							}}
						}}
						else
						{{
							if (screen.width < 768) {{
								document.getElementsByClassName('jsgrid-pager')[0].style.display = 'block';
							}} else {{
								document.getElementsByClassName('jsgrid-pager')[0].style.display = 'flex';
							}}
						}}
						{extraFunctionality}
					}},
					rowClick:function(args){{
						var item = $(args.event.target).closest('tr');
						if(this._clicked_row != null) {{
							this._clicked_row.removeClass('jsgrid-clicked-row');
						}}
						this._clicked_row = item;
						item.addClass('jsgrid-clicked-row');
						{rowSelect}
					}},
					{rowDblClick}
					controller: {{
						loadData: function(filter)
						{{
							var startIndex = (filter.pageIndex - 1) * filter.pageSize;
							var d1 = $.Deferred();

							$.ajax({{
								type: ""POST"",
								url: ""{MethodURL}"",
								data: {json},
								contentType: ""application/json; charset=utf-8"",
								dataType: ""json"",
								error: function (XMLHttpRequest, textStatus, errorThrown) {{
									alert(XMLHttpRequest.responseJSON.Message);
								}}
								}}).done(function(response) {{
									var tmp = response.d;
									{onClear}
									if(tmp.indexOf('ZeroSuccess')>0){{return;alert('fail');}};
									var tmp2 = tmp.substring(0, tmp.indexOf(""|""));
									var tmp3 = JSON.parse(tmp.substring(tmp.indexOf(""|"") + 1));
									var feeds = {{
										data: tmp3,
										itemsCount: tmp2 * 1
									}};
									d1.resolve(feeds);
							}});
							return d1.promise();
						}},
					}},
					fields: [{FieldList}]
				}});
			}});
			{bottom}
			</script>";

			return jsGrid;
		}

		public void SetFieldListFromGridDef(string griddef, string KeyField, bool KeyFieldHidden = true)
		{
			StringBuilder sb = new StringBuilder("{ name: \"" + KeyField + "\", visible: " + ((KeyFieldHidden) ? "false" : "true") + "},\r\n");
			string[] oldcols = griddef.Split("|".ToCharArray());
			foreach (string oldcol in oldcols)
			{
				sb.Append(buildFieldDef(oldcol.Split(":".ToCharArray())));
			}
			FieldList = sb.ToString();
			if (FieldList.EndsWith(",") || FieldList.EndsWith("|"))
			{
				FieldList = FieldList.Substring(0, FieldList.Length - 1);
			}
		}

		protected string buildFieldDef(string[] parts)
		{
			int partsLength = parts.Length;
			if (partsLength == 1)
				return "";

			string name = parts[1];
			string label = parts[2];
			if (label.Length == 0)
            {
                label = name;
            }
			string width = parts[3];
            if (width.Length == 0)
            {
                width = "50px";
            }
			else
            {
				if (width.IndexOf("%") >= 0)
					width = "'" + width + "'";
            }
			string sortStrategy = "text";
			string sorter = "'text'";
			if (partsLength >= 5) {
				if (parts[4].Length != 0 && parts[4] != "")
				{
					sortStrategy = $"{parts[4]}";
					sorter = "'number'";
				}
			}

			string align = "";
            string visible = "true";
			if (parts[0].CompareTo(">") == 0)
			{
				align = "right";
			}
			else if (parts[0].CompareTo("<") == 0)
			{
				align = "left";
			}
			else if (parts[0].CompareTo("!") == 0)
            {
                visible = "false";
            }
			else
			{
				align = "center";
			}

			// sorting is fixed right now since its not in grid def but it should be doable in the grid //sorting:{FieldSortable(name)}
			//return string.Format("{{ name: '{0}', visible: {1}, title: '{2}', width: {3}, sorting: {4}, filtering: {5}, type: \"text\"{6} }},", name, visible, label, width, FieldSortable(name), FieldFilterable(name), align);
			string returnString = $"{{{{name:'{name}',visible:{visible},title:'{label}',width:{width},filtering:{FieldFilterable(name)},type:'{sortStrategy}',sorting:{FieldUnSortable(name)},sorter:{sorter},align:'{align}'}}}},";
			return string.Format(returnString);
        }

		protected string FieldUnSortable(string fieldname)
		{
			return (NotSortableFields.Contains(fieldname) ? "false" : "true");
		}
		protected string FieldFilterable(string fieldname)
		{
			return (FilterableFields.Contains(fieldname.ToLower()) ? "true" : "false");
		}

		protected string FlagValue(string name, bool flag)
		{
			return string.Format("{0}: {1}", name, (flag) ? "true" : "false");
		}
	}
}