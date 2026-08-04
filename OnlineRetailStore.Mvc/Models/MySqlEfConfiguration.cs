using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;

namespace OnlineRetailStore.Mvc.Models
{
    // EF6 auto-discovers this by scanning the assembly that declares AppDbContext.
    public class MySqlEfConfiguration : DbConfiguration
    {
        public MySqlEfConfiguration()
        {
            SetManifestTokenResolver(new MariaDbManifestTokenResolver());
        }

        // XAMPP/MariaDB prefixes its real version with "5.5.5-" for legacy client
        // compatibility (e.g. "5.5.5-10.4.32-MariaDB"). MySql.Data.EntityFramework's
        // built-in version check only reads that leading "5.5" and rejects it as
        // "prior to 5.6", even though the server is a modern MariaDB. Skip that broken
        // check entirely and force the "5.7" schema, which MariaDB 10.4 is compatible with.
        private class MariaDbManifestTokenResolver : IManifestTokenResolver
        {
            public string ResolveManifestToken(DbConnection connection) => "5.7";
        }
    }
}
