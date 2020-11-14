using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace CCPP.Cryptocurrency.Common.BTC
{
    public class AddressDetail
    {
        [JsonProperty(PropertyName = "address")]
        public string Address { get; set; }

        [JsonProperty(PropertyName = "scriptPubKey")]
        public string ScriptPubKey { get; set; }

        [JsonProperty(PropertyName = "ismine")]
        public bool IsMine { get; set; }

        [JsonProperty(PropertyName = "iswatchonly")]
        public bool IsWatchOnly { get; set; }

        [JsonProperty(PropertyName = "isscript")]
        public bool IsScript { get; set; }

        [JsonProperty(PropertyName = "iswitness")]
        public bool IsWitness { get; set; }
    }
}