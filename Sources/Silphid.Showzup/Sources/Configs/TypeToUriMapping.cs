using System;
using System.Linq;
using Silphid.Extensions;
using UnityEngine;

namespace Silphid.Showzup
{
    [Serializable]
    public class TypeToUriMapping : Mapping
    {
        [SerializeField] private string _target;

        public Uri Target => new Uri(_target);

        public TypeToUriMapping(Type source, Uri target, VariantSet variants) : base(source, variants)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            _target = target.ToString();
        }

        public override bool Matches(string filter) =>
            base.Matches(filter) ||
            _target.CaseInsensitiveContains(filter);

        public override string ToString()
        {
            var variants = Variants != null
                ? Variants.Any() ? $" [{Variants}]" : ""
                : "null";
            
            return $"{Source?.Name.ToStringOrNullLiteral()} => " +
                   $"{_target.ToStringOrNullLiteral()}" +
                   $"{variants}";
        }

        public override bool IsValid =>
            base.IsValid &&
            _target != null;
    }
}