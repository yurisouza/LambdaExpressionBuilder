﻿using System;
using System.Reflection;

namespace ExpressionBuilder.Resources
{
    /// <summary>
    /// Provides information on the property to the Expression Builder.
    /// </summary>
    public class Property
    {
        /// <summary>
        /// Property identifier conventionalized by for the Expression Builder.
        /// </summary>
        public string Id { get; private set; }
        /// <summary>
        /// Property name obtained from a ResourceManager, or the property's original name (in the absence of a ResourceManager correspondent value).
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Property metadata.
        /// </summary>
        public PropertyInfo Info { get; private set; }
        /// <summary>
        /// Add <see cref="Common.Operation.EqualsAny"/> to operation list
        /// </summary>
        public bool MatchAny = false;

        internal Property(string id, string name, PropertyInfo info, bool matchAny)
        {
            Id = id;
            Name = name;
            Info = info;
            MatchAny = matchAny;
        }

        /// <summary>
        /// String representation of <see cref="Property" />.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0} ({1})", Name, Id);
        }
    }
}
