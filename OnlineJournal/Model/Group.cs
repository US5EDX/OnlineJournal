namespace OnlineJournal.Model
{
    public class Group
    {
        public string Code { get; set; }
        public PairItem Curator { get; set; }

        public object GetGroupWithoutFullName()
        {
            return new { Code = this.Code, Curator = this.Curator.Value };
        }
    }
}
