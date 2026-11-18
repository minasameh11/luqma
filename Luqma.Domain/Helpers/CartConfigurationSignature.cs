namespace Luqma.Domain.Helpers
{
    // Builds the key that identifies a cart line: the item option plus the selected addons
    public static class CartConfigurationSignature
    {
        public const string NoOption = "none";
        public const string Empty = NoOption + "|";

        public static string Build(Guid? menuItemOptionId, IEnumerable<Guid> addonIds)
        {
            var option = menuItemOptionId?.ToString() ?? NoOption;

            var addons = addonIds
                .Distinct()
                .Select(id => id.ToString())
                .OrderBy(id => id, StringComparer.Ordinal);

            return option + "|" + string.Join(",", addons);
        }
    }
}
