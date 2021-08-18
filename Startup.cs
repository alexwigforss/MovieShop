using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using MovieShop.Models;
using Owin;

[assembly: OwinStartupAttribute(typeof(MovieShop.Startup))]
namespace MovieShop
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            CreateRolesandUsers();
        }
        private void CreateRolesandUsers()
        {
            ApplicationDbContext db = new ApplicationDbContext();
            var rolemanager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));//dels with class of odentity role
            var usermanager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));

            if(!rolemanager.RoleExists("Admin"))
            {
                //First we create admin role
                var role = new IdentityRole();
                role.Name = "Admin";
                rolemanager.Create(role);
            }
            if (usermanager.FindByEmail("alex@movieshop.com") == null)
            {
                // CreateRolesandUsers SuperUser for site
                var user = new ApplicationUser();
                user.UserName = "Alex";
                user.Email = "alex@movieshop.com";
                string pswrd = "qwerty123"; //weakest password ever
                var chkres = usermanager.Create(user, pswrd);

                // On Success Assign Admin To Above User
                if(chkres.Succeeded)
                {
                    usermanager.AddToRole(user.Id,"Admin");
                }
            }

            // Adding Manager Role
            if (!rolemanager.RoleExists("Manager"))
            {
                //First we create admin role
                var role = new IdentityRole();
                role.Name = "Manager";
                rolemanager.Create(role);
            }
        }
    }
}
