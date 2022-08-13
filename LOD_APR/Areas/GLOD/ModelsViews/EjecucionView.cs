using System;
using System.Collections.Generic;

namespace LOD_APR.Areas.GLOD.ModelsViews
{
    public class EjecucionView
    {
        public string ID { get; set; }
        public int Folio { get; set; }
        public int IdEnvio { get; set; }
        public Form Form { get; set; }
        public DateTime ExecutionTime { get; set; }
        public string UserName { get; set; }
        public ItemsData[] ItemsData { get; set; }
        public FormsData[] FormsData { get; set; }
    }
    public class Form
    {
        public string FormID { get; set; }
        public string FormName { get; set; }
    }
    public class Field
    {
        public string FieldID { get; set; }
        public string FieldName { get; set; }
        public string FieldValue { get; set; }
        public FieldType Type { get; set; }
        public Option[] Options { get; set; }
    }

    public class ItemsData
    {
        public string ItemID { get; set; }
        public string ItemName { get; set; }
        public Field[] Fields { get; set; }
    }
    public class FieldType
    {
        public int Type { get; set; }
        public string TypeName { get; set; }
        public int TypeWidth { get; set; }
    }
    public class Option
    {
        public string OptionID { get; set; }
        public string OptionName { get; set; }
        public bool Selected { get; set; }
    }
    public class FormsData
    {
        public string FormID { get; set; }
        public string FormName { get; set; }
        public List<string> Fields { get; set; }
        public List<string[]> Rows { get; set; }
    }
}