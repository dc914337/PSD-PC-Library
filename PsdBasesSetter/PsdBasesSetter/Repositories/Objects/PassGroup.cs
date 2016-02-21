using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace PsdBasesSetter.Repositories.Objects
{
    [JsonObject]
    public class PassGroup : IEnumerable<PassItem>
    {
        [DataMember]
        public String UUID { get; set; }
        [DataMember]
        public String Name { get; set; }
        [DataMember]
        public String Notes { get; set; }

        [DataMember]
        public PasswordList Passwords { get; set; } = new PasswordList();
        [DataMember]
        public PassGroupsList PassGroups { get; set; } = new PassGroupsList();

        public IEnumerator<PassItem> GetEnumerator()
        {
            foreach (var pass in Passwords)
            {
                yield return pass.Value;
            }

            foreach (var subGroup in PassGroups)
            {
                foreach (var pass in subGroup.Passwords)
                {
                    yield return pass.Value;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            // uses the strongly typed IEnumerable<T> implementation
            return GetEnumerator();
        }

        public void AddPass(PassItem newPassword)
        {
            Passwords.AddPass(newPassword);
        }
    }
}
