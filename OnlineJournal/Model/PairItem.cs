using System;

namespace OnlineJournal.Model
{
    public class PairItem : IComparable<PairItem>, IEquatable<PairItem>
    {
        public string Value { get; set; }
        public string Display { get; set; }

        public int CompareTo(PairItem other)
        {
            if (other == null)
                return 1;

            int valueComparison = string.Compare(this.Value, other.Value, StringComparison.Ordinal);
            if (valueComparison != 0)
                return valueComparison;

            return string.Compare(this.Display, other.Display, StringComparison.Ordinal);
        }

        public bool Equals(PairItem other)
        {
            if (other == null)
                return false;

            return string.Equals(this.Value, other.Value, StringComparison.Ordinal) &&
                   string.Equals(this.Display, other.Display, StringComparison.Ordinal);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (obj is PairItem item)
                return Equals(item);

            return false;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + (Value != null ? Value.GetHashCode() : 0);
                hash = hash * 23 + (Display != null ? Display.GetHashCode() : 0);
                return hash;
            }
        }

        public static bool operator ==(PairItem left, PairItem right)
        {
            if (ReferenceEquals(left, null))
                return ReferenceEquals(right, null);

            return left.Equals(right);
        }

        public static bool operator !=(PairItem left, PairItem right)
        {
            return !(left == right);
        }

        public static bool operator <(PairItem left, PairItem right)
        {
            if (left == null)
                throw new ArgumentNullException(nameof(left));

            return left.CompareTo(right) < 0;
        }

        public static bool operator <=(PairItem left, PairItem right)
        {
            if (left == null)
                throw new ArgumentNullException(nameof(left));

            return left.CompareTo(right) <= 0;
        }

        public static bool operator >(PairItem left, PairItem right)
        {
            if (left == null)
                throw new ArgumentNullException(nameof(left));

            return left.CompareTo(right) > 0;
        }

        public static bool operator >=(PairItem left, PairItem right)
        {
            if (left == null)
                throw new ArgumentNullException(nameof(left));

            return left.CompareTo(right) >= 0;
        }
    }
}
