using Simple.Data.Common.Enums;

namespace Simple.Data.Common.Options;

public class DbConnectionOptions
{
    public DataProvider DataProvider { get; set; }
    public string ConnectionString { get; set; }
    public string ConnectionName { get; set; }
}
