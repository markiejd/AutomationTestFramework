

using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using Core.Logging;
using Generic.Steps;

namespace Generic.Elements.Steps.TSQL.Code
{
    public class DescTableKeyStorage
    {
        public static List<DescTableKey>? DescTableKeys { get; set; } = new List<DescTableKey>();

        public static List<DescTableKey>? GetDescTableKeyStore()
        {
            // Retrieve the stored DescTableKeys from a static variable or a database, etc.
            // For simplicity, we will just return the current instance's DescTableKeys.
            return DescTableKeys;
        }

        public static bool SetDescTableKeyStore(List<DescTableKey> descTableKeys)
        {
            if (descTableKeys == null || descTableKeys.Count == 0)
            {
                return false;
            }
            try
            {
                DescTableKeys = descTableKeys;
                return true;
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                Console.WriteLine($"Error setting DescTableKeyStore: {ex.Message}");
                return false;
            }
        }

    }

    public class DescTableKey
    {
        public string KeyName { get; set; } = string.Empty;
        public string ColumnName { get; set; } = string.Empty;
        public string KeyType { get; set; } = string.Empty;

        // using newtonsoft.json convert json to object
        public static List<DescTableKey> DescTableKeyFromJson(string json)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<List<DescTableKey>>(json) ?? new List<DescTableKey>();
        }
    }

    public class DescTableStorage
    {
        public static List<DescTable>? DescTables { get; set; } = new List<DescTable>();

        public static List<DescTable>? GetDescTableStore()
        {
            // Retrieve the stored DescTables from a static variable or a database, etc.
            // For simplicity, we will just return the current instance's DescTables.
            return DescTables;
        }

        public static bool SetDescTableStore(List<DescTable> descTables)
        {
            if (descTables == null || descTables.Count == 0)
            {
                return false;
            }
            try
            {
                DescTables = descTables;
                return true;
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                Console.WriteLine($"Error setting DescTableStore: {ex.Message}");
                return false;
            }
        }
    }

    public class DescTable
    {
        public string COLUMN_NAME { get; set; } = string.Empty;
        public string DATA_TYPE { get; set; } = string.Empty;
        public string IS_NULLABLE { get; set; } = string.Empty;
        public int? CHARACTER_MAXIMUM_LENGTH { get; set; } = null;

        // using newtonsoft.json convert json to object
        public static List<DescTable> DescTableFromJson(string json)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<List<DescTable>>(json) ?? new List<DescTable>();
        }

    }

}