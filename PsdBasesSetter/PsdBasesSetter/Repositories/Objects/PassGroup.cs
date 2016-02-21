using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PsdBasesSetter.Repositories.Objects
{
    public class PassGroup : IEnumerable<PassItem>
    {
        public String UUID { get; set; }
        public String Name { get; set; }
        public String Notes { get; set; }

        public PasswordList Passwords { get; set; }
        public PassGroupsList PassGroups { get; set; }

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
