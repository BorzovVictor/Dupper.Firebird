using System.Collections.Generic;

namespace Dupper.Firebird.FBSQLHelpers
{
    public partial class Fields
    {
        public string FIELD_NAME { get; set; }
        public string CS_TYPE { get; set; }
        public short? FIELD_LENGTH { get; set; }
        public int? FIELD_NOT_NULL { get; set; }
        public string FIELD_DEFAULT { get; set; }
        public string FIELD_DESCRIPTION { get; set; }
        public short PK_FIELD { get; set; }
    }

    public class Table
    {
        public string Name { get; set; }
        public IEnumerable<Fields> FieldList { get; set; }
    }

    public class StoredProcedure
    {
        public string NAME { get; set; }
        public short? INPUTS { get; set; }
        public short? OUTPUTS { get; set; }
        public string DESCRIPTION { get; set; }
        public string SOURCE { get; set; }
    }
}
