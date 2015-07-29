namespace Secucard.Model.Loyalty
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    [DataContract]
    public class Program : SecuObject
    {
        [DataMember(Name = "cardGroup")]
        public CardGroup CardGroup { get; set; }

        [DataMember(Name = "conditions")]
        public List<Condition> conditions { get; set; }

        [DataMember(Name = "description")]
        public string Description { get; set; }

        public override string ServiceResourceName
        {
            get { return "loyalty.program"; }
        }
    }
}