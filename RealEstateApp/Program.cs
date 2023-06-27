using RealEstateApp.Model;
using System;
using System.Linq;

public class Program
{
    static void Main()
    {
        using (var context = new RealEstateDbContext())
        {
            //// Create
            Console.WriteLine("Inserting a new property");
            var property = new Property { Address = "123 Elm St", Status = "Available", OwnerID = 1 };
            context.Properties.Add(property);
            context.SaveChanges();

            // Read
            Console.WriteLine("Querying for a property");
            var prop = context.Properties
                .OrderBy(b => b.PropertyID)
                .First();
            // Display the property details
            Console.WriteLine($"Property ID: {prop.PropertyID}");
            Console.WriteLine($"Address: {prop.Address}");
            Console.WriteLine($"Status: {prop.Status}");
            Console.WriteLine($"Owner ID: {prop.OwnerID}");

            // Update
            Console.WriteLine("Updating the property and saving changes");
            prop.Status = "Not Available";
            context.Properties.Update(prop);
            context.SaveChanges();

            //// Delete
            //Console.WriteLine("Delete the property");
            //context.Properties.Remove(prop);
            //context.SaveChanges();

            // Delete
            int propertyId = 3; // ID of the property you want to delete

            // Fetch the property
            var propertyToDelete = context.Properties.Find(propertyId);

            if (propertyToDelete != null)
            {
                // Fetch the rental contracts associated with the property
                var rentalContracts = context.RentalContracts.Where(rc => rc.PropertyID == propertyId).ToList();

                // Delete the rental contracts and their associated maintenance requests
                foreach (var rc in rentalContracts)
                {
                    // Fetch the maintenance requests associated with the rental contract
                    var maintenanceRequests = context.MaintenanceRequests.Where(mr => mr.ContractID == rc.ContractID).ToList();

                    // Delete the maintenance requests
                    foreach (var mr in maintenanceRequests)
                    {
                        context.MaintenanceRequests.Remove(mr);
                    }

                    // Delete the rental contract
                    context.RentalContracts.Remove(rc);
                }

                // Save changes to delete rental contracts and maintenance requests
                context.SaveChanges();

                // Now, delete the property
                context.Properties.Remove(propertyToDelete);

                // Save changes to delete property
                context.SaveChanges();
            }

        }
    }
}

