using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using TCS.WebUI.Extensions;
using TCS.WebUI.Helpers;
using TCS.WebUI.ViewComponents;

namespace TCS.WebUI.Models
{
    public static class NavigationModel
    {
        private const string Underscore = "_";
        private const string Dash = "-";
        private const string Space = " ";
        private static readonly string Empty = string.Empty;
        public static readonly string Void = "javascript:void(0);";

        public static SmartNavigation Seed => BuildNavigation();
        public static SmartNavigation Full => BuildNavigation(seedOnly: false);

        private static SmartNavigation BuildNavigation(bool seedOnly = true)
        {
            var jsonText = File.ReadAllText("nav.json");
            SmartNavigation test = NavMenu.NavMenuLoad();
            var navigation = NavigationBuilder.FromJson(jsonText);
            var menu = FillProperties(test.Lists, seedOnly);

            return new SmartNavigation(menu);
        }

        private static List<ListItem> FillProperties(IEnumerable<ListItem> items, bool seedOnly, ListItem parent = null)
        {
            var result = new List<ListItem>();

            foreach (var item in items)
            {
                item.Text ??= item.Title;
                item.Tags = string.Concat(parent?.Tags, Space, item.Title.ToLower()).Trim();

                var parentRoute = (Path.GetFileNameWithoutExtension(parent?.Text ?? Empty)?.Replace(Space, Underscore) ?? Empty).ToLower();
                var sanitizedHref = parent == null ? item.Href?.Replace(Dash, Empty) : item.Href?.Replace(parentRoute, parentRoute.Replace(Underscore, Empty)).Replace(Dash, Empty);
                var route = Path.GetFileNameWithoutExtension(sanitizedHref ?? Empty)?.Split(Underscore) ?? Array.Empty<string>();

#if DEBUG
                item.Route = route.Length > 1 ? $"/{route.First()}/{string.Join(Empty, route.Skip(1))}" : item.Href;
#else
                item.Route = route.Length > 1 ? $"/tcs/{route.First()}/{string.Join(Empty, route.Skip(1))}" : item.Href;
#endif

                item.I18n = parent == null
                    ? $"nav.{item.Title.ToLower().Replace(Space, Underscore)}"
                    : $"{parent.I18n}_{item.Title.ToLower().Replace(Space, Underscore)}";
                item.Type = parent == null ? item.Href == null ? ItemType.Category : ItemType.Single : item.Items.Any() ? ItemType.Parent : ItemType.Child;
                item.Items = FillProperties(item.Items, seedOnly, item);

                if (item.Href.IsVoid() && item.Items.Any())
                    item.Type = ItemType.Sibling;

                if (!seedOnly || item.ShowOnSeed)
                {
                    if (item.Roles != null)
                    {
                        foreach (string rol in item.Roles)
                        {
                            IEnumerable<Claim> roleToCheck = NavigationViewComponent.claims;
                            if (roleToCheck.Any(x=>x.Value == rol))
                            {
                                result.Add(item);
                                break;
                            }
                        }
                    }
                    else
                    {
                        result.Add(item);
                    }
                }
            }

            return result;
        }
    }
}
