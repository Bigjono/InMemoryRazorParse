using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Linq;

namespace InMemoryRazorParsing.Model
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

    public class Testimonial
    {

        public string Title { get; set; }
        public string Link { get; set; }
        public string Description { get; set; }
        public string PubDate { get; set; }
        public string AuthorName { get; set; }
        public string Stars { get; set; }
        public string City { get; set; }
        public string Location { get; set; }

    }
}
