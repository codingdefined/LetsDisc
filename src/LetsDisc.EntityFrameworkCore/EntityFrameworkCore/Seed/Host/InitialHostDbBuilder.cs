namespace LetsDisc.EntityFrameworkCore.Seed.Host
{
    public class InitialHostDbBuilder
    {
        private readonly LetsDiscDbContext _context;

        public InitialHostDbBuilder(LetsDiscDbContext context)
        {
            _context = context;
        }

        public void Create()
        {
            new DefaultEditionCreator(_context).Create();
            new DefaultLanguagesCreator(_context).Create();
            new HostRoleAndUserCreator(_context).Create();
            new DefaultSettingsCreator(_context).Create();
            new DefaultPostTypesCreator(_context).Create();
            new DefaultVoteTypesCreator(_context).Create();
            _context.SaveChanges();
        }
    }
}
