using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using Core.Logging;
using Newtonsoft.Json;

namespace Generic.Elements.Steps.TSQL.Code
{

    // Storage class for Data Dictionary created from Excel file
    public class DataDictionaryStorage
    {

        public static List<DataDictionaryRow>? DataDictionary { get; set; } = new List<DataDictionaryRow>();

        public static bool SetDataDictionaryStore(List<DataDictionaryRow> dataDictionary)
        {
            if (dataDictionary == null || dataDictionary.Count == 0)
            {
                return false;
            }
            try
            {
                DataDictionary = dataDictionary;
                return true;
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                Console.WriteLine($"Error setting DataDictionaryStore: {ex.Message}");
                return false;
            }
        }

        public static List<DataDictionaryRow>? GetDataDictionaryStore()
        {
            // Retrieve the stored DataDictionary from a static variable or a database, etc.
            // For simplicity, we will just return the current instance's DataDictionary.
            return DataDictionary;
        }



    }

    public class DataDictionaryRow
    {
        [JsonProperty("Conceptual Entity")]
        public string? ConceptualEntity { get; set; }

        [JsonProperty("Attribute Name")]
        public string? AttributeName { get; set; }

        [JsonProperty("Schema")]
        public string? Schema { get; set; }

        [JsonProperty("Physical Column Name")]
        public string? PhysicalColumnName { get; set; }

        [JsonProperty("Physical Table Name")]
        public string? PhysicalTableName { get; set; }

        [JsonProperty("Data Type")]
        public string? DataType { get; set; }

        [JsonProperty("Nullable")]
        public bool? Nullable { get; set; }

        [JsonProperty("Primary Key")]
        public bool? PrimaryKey { get; set; }

        [JsonProperty("Business Key Group")]
        public string? BusinessKeyGroup { get; set; }

        [JsonProperty("Unique Key Group")]
        public string? UniqueKeyGroup { get; set; }

        [JsonProperty("Foreign Key Group")]
        public string? ForeignKeyGroup { get; set; }

        [JsonProperty("Foreign Key Target Table")]
        public string? ForeignKeyTargetTable { get; set; }

        [JsonProperty("Foreign Key Target Column")]
        public string? ForeignKeyTargetColumn { get; set; }

        [JsonProperty("Description")]
        public string? Description { get; set; }

        [JsonProperty("Allowed Values")]
        public string? AllowedValues { get; set; }

        [JsonProperty("Is Mandatory")]
        public bool? IsMandatory { get; set; }

        [JsonProperty("Source")]
        public string? Source { get; set; }

        [JsonProperty("Source Field")]
        public string? SourceField { get; set; }

        [JsonProperty("Transformation Logic")]
        public string? TransformationLogic { get; set; }

        [JsonProperty("Example Value")]
        public string? ExampleValue { get; set; }

        [JsonProperty("Source System")]
        public string? SourceSystem { get; set; }

        [JsonProperty("Mapping Notes")]
        public string? MappingNotes { get; set; }

        [JsonProperty("Data Sensitivity")]
        public string? DataSensitivity { get; set; }

        [JsonProperty("Business Owner")]
        public string? BusinessOwner { get; set; }

        [JsonProperty("Data Steward")]
        public string? DataSteward { get; set; }

        [JsonProperty("Validation Rules")]
        public string? ValidationRules { get; set; }

        [JsonProperty("Lineage Diagram Link")]
        public string? LineageDiagramLink { get; set; }

        [JsonProperty("Requirements Traceability")]
        public string? RequirementsTraceability { get; set; }


        // using newtonsoft.json convert json to object
        public static List<DataDictionaryRow> DataDictionaryFromJson(string jsonText)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<List<DataDictionaryRow>>(jsonText) ?? new List<DataDictionaryRow>();
        }

        // pass in a string which is json from reading the excel file and convert it to a list of DataDictionaryRow
        

    }

}