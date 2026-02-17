using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Text.RegularExpressions;

namespace lmHTMLGenerator
{
    public class TagReplacement
    {
        public string TagName = "";
        public string TagValue = "";
        public static TagReplacement CreateTag(string name, string value)
        {
            TagReplacement tmp = new TagReplacement();
            tmp.TagName = name;
            tmp.TagValue = value;
            return tmp;
        }
        private TagReplacement() { }
    }
    public class TagGroup
    {
        public string Name = "";
        private List<List<TagReplacement>> groupElements = new List<List<TagReplacement>>();
        public void AddGroupItem(List<TagReplacement> item_tags)
        {
            groupElements.Add(item_tags);
        }
        public int getCount()
        {
            return groupElements.Count;
        }
        public List<TagReplacement> GetGroupItem(int index)
        {
            return groupElements[index];
        }
    }
    public class TemplateGenerator
    {
        public string[] Elements = { "<!--[{0}]-->" //normal tags
                                       , "<!--[IF {0}]-->", "<!--[ELSE {0}]-->", "<!--[ENDIF {0}]-->" // conditional tags
                                       , "<!--[GROUP {0}]-->", "<!--[ENDGROUP {0}]-->" // group tags
                                       , "<!--[GRPELEM {0}]-->", "<!--[ENDGRPELEM {0}]-->" // group element tags
                                   };

        private Dictionary<string, bool> conditionals = new Dictionary<string, bool>();
        private List<TagReplacement> baseTags = new List<TagReplacement>();
        private Dictionary<string, TagGroup> tagGroups = new Dictionary<string, TagGroup>();
        public void AddTag(string name, string value)
        {
            baseTags.Add(TagReplacement.CreateTag(name, value));
        }
        public void AddRowsAsGroup(string groupName, DataTable dt)
        {
            if (!tagGroups.ContainsKey(groupName))
            {
                tagGroups[groupName] = new TagGroup();
            }
            TagGroup tg = tagGroups[groupName];
            foreach (DataRow dr in dt.Rows)
            {
                List<TagReplacement> tRow = new List<TagReplacement>();
                foreach (DataColumn dc in dt.Columns)
                {
                    tRow.Add(TagReplacement.CreateTag(dc.ColumnName.ToUpper(), dr[dc.ColumnName].ToString()));
                }
                tg.AddGroupItem(tRow);
            }
        }

        public void UpdateValue(string tagname, string value)
        {
            foreach (TagReplacement tr in baseTags)
            {
                if (tr.TagName.ToLower().CompareTo(tagname.ToLower()) == 0)
                {
                    tr.TagValue = value;
                    return;
                }
            }
            baseTags.Add(TagReplacement.CreateTag(tagname, value));
        }
        public string GetTagValue(string tagname)
        {
            foreach (TagReplacement tr in baseTags)
            {
                if (tr.TagName.ToLower().CompareTo(tagname.ToLower()) == 0)
                {
                    return tr.TagValue;
                }
            }
            return "";
        }
        public void SetConditional(string name, bool value)
        {
            conditionals[name] = value;
        }
        public string BuildOutput(string template_text)
        {
            string result = replaceConditionals(template_text);
            result = replaceGroupTags(result); // by replacing group tags first it allows us to use generic tags even in the groups
            result = replaceBaseTags(result);
            return result;
        }
        public void MakeNonEmptyConditional(string tagname)
        {
            foreach (TagReplacement tr in baseTags)
            {
                if (tr.TagName == tagname && !string.IsNullOrEmpty(tr.TagValue))
                {
                    SetConditional("SHOW" + tagname, true);
                    return;
                }
            }
            SetConditional("SHOW" + tagname, false);
        }
        private string replaceBaseTags(string template_text)
        {
            string result = template_text;
            foreach (TagReplacement tr in baseTags)
            {
                result = result.Replace(string.Format(Elements[0], tr.TagName), tr.TagValue);
            }
            return result;
        }
        private string replaceConditionals(string template_text)
        {
            string result = template_text;
            foreach (KeyValuePair<string, bool> kvp in conditionals)
            {
                string iftag = String.Format(Elements[1], kvp.Key);
                string elsetag = String.Format(Elements[2], kvp.Key);
                string endiftag = String.Format(Elements[3], kvp.Key);

                while (result.Contains(iftag))
                {
                    int ifpos = result.IndexOf(iftag);
                    if (ifpos > -1)
                    {
                        int elsepos = result.IndexOf(elsetag, ifpos);
                        int endifpos = result.IndexOf(endiftag, ifpos);
                        if (elsepos > endifpos) { elsepos = -1; } // if the else is outside of the if/endif block, it may belong to another set, ignore it
                        if (endifpos == -1) { throw new Exception("Malformed template, on if block: " + kvp.Key); } // if we dont find and end tag, error out, bad template
                        if (kvp.Value)
                        {
                            // we want to remove the if tag
                            // remove the else clause all the way to the end of the endif if present
                            // if no else clause, just remove end tag
                            if (elsepos == -1)
                            {
                                // easiest case, just remove the instances, endif first so we dont throw off position
                                result = result.Remove(endifpos, endiftag.Length);
                                result = result.Remove(ifpos, iftag.Length);
                            }
                            else
                            {
                                int tlength = (endifpos - elsepos) + endiftag.Length;
                                result = result.Remove(elsepos, tlength); // remove the else all the way thru the endif
                                result = result.Remove(ifpos, iftag.Length); // remove the tag
                            }
                        }
                        else
                        {
                            if (elsepos == -1)
                            {
                                int tlength = (endifpos - ifpos) + endiftag.Length;
                                // easiest case, just remove the instances, endif first so we dont throw off position
                                result = result.Remove(ifpos, tlength);
                            }
                            else
                            {
                                result = result.Remove(endifpos, endiftag.Length); // remove the end tag
                                int tlength = (elsepos - ifpos) + elsetag.Length;
                                result = result.Remove(ifpos, tlength); // and remove the if tag thru the else tag
                            }
                        }
                    }

                }
            }
            return result;
        }
        private string replaceGroupTags(string template_text)
        {
            string result = template_text;
            foreach (KeyValuePair<string, TagGroup> kvp in tagGroups)
            {
                string begingroup = String.Format(Elements[4], kvp.Key);
                string endgroup = String.Format(Elements[5], kvp.Key);
                while (result.Contains(begingroup))
                {
                    bool removegroup = false;
                    int bPos = result.IndexOf(begingroup);
                    int ePos = result.IndexOf(endgroup);
                    if (ePos > -1)
                    {
                        string wholegroup = result.Substring(bPos, (ePos - bPos) + endgroup.Length);
                        // lets count the elements
                        int elecount = 0;
                        for (int i = 1; i < 101; i++) // only 100 elements allowed max, but it will repeat as necessary
                        {
                            int ebpos = wholegroup.IndexOf(string.Format(Elements[6], i));
                            if (ebpos > -1)
                            {
                                int eepos = wholegroup.IndexOf(string.Format(Elements[7], i), ebpos);
                                if (eepos > -1)
                                {
                                    elecount++;
                                }
                                else
                                {
                                    removegroup = true;
                                    //throw new Exception("Malformed template, missing end element tag " + i.ToString() + " in group block: " + kvp.Key);
                                    break;
                                }
                            }
                            else if (elecount == 0)
                            {
                                removegroup = true;
                                break;
                                // malformed group, could just remove it
                                //throw new Exception("Malformed template, no elements in group block: " + kvp.Key);
                            }
                            else
                            {
                                break;
                            }
                        }
                        if (removegroup || elecount == 0)
                        {
                            result = result.Remove(bPos, (ePos - bPos) + endgroup.Length);
                        }
                        else
                        {
                            string totalresult = "";
                            // we have elements and didnt error, lets do the real replacement
                            int rows = kvp.Value.getCount() / elecount; // how many rows will we need
                            rows += (kvp.Value.getCount() % elecount > 0) ? 1 : 0;
                            int groupid = 1;
                            for (int cr = 0; cr < rows; cr++)
                            {
                                string elenow = wholegroup;
                                int curitem = (rows * elecount) - (cr * elecount);
                                int itemsinrow = Math.Min(elecount, curitem); // we are going to stop when we hit this item and remove elements after it if neccessary
                                for (int i = 1; i < elecount + 1; i++)
                                {
                                    string stag = string.Format(Elements[6], i);
                                    string etag = string.Format(Elements[7], i);
                                    int estart = wholegroup.IndexOf(stag); // already validated for proper form
                                    int eend = wholegroup.IndexOf(etag, estart);
                                    // grab the element out, fix it, then replace it, for all elements
                                    string fullelement = wholegroup.Substring(estart, ((eend + etag.Length) - estart)); // leave the tags out of it
                                    string element = wholegroup.Substring(estart + stag.Length, (eend - (estart + stag.Length))); // leave the tags out of it
                                    int whichitem = (cr * elecount) + (i - 1);
                                    if (whichitem > kvp.Value.getCount() - 1) // we dont need this element, trash it
                                    {
                                        elenow = elenow.Replace(fullelement, "");
                                    }
                                    else
                                    {

                                        List<TagReplacement> tgl = kvp.Value.GetGroupItem(whichitem);
                                        foreach (TagReplacement tri in tgl)
                                        {
                                            element = element.Replace(string.Format(Elements[0], tri.TagName), tri.TagValue);
                                            element = element.Replace(string.Format(Elements[0], "GROUPINDEX"), groupid.ToString());
                                        }
                                        groupid++;
                                        elenow = elenow.Replace(fullelement, element);
                                    }
                                }
                                // now remove the group tags
                                elenow = elenow.Replace(begingroup, "").Replace(endgroup, "");
                                // and add it to total result
                                totalresult += elenow;
                            }
                            result = result.Replace(wholegroup, totalresult);
                        }
                        // done with this group, lets replace the tag with the result...
                    }
                    else
                    {
                        throw new Exception("Malformed template, on group block: " + kvp.Key);
                    }
                }
            }
            return result;
        }
    }
}
