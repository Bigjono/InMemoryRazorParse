using System.Collections.ObjectModel;

namespace InMemoryRazorParsing
{
    public class CustomerContainer
    {

        public CustomerContainer()
        {
            Customers = new Collection<Customer>();
        }

        public string Name { get; set; }
        public bool IsEnabled { get; set; }
        public Collection<Customer> Customers { get; set; }
    }


    public class Customer
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Telephone { get; set; }

    }
}
