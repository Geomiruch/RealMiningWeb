
using System.Linq;
using User_Story.Models;

namespace User_Story.Data
{
    public class DBInitializer
    {
        public static void Initialize(Context context)
        {
            //context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            if (context.Roles.Any())
            {
                return;
            }


            //Ролі
            var roles = new Role[]
            {
                new Role{ Name = "Moderator"},
                new Role{ Name = "User"}
            };
            
            foreach (Role r in roles)
            {
                context.Roles.Add(r);
            }
            context.SaveChanges();

        }
    }
}
