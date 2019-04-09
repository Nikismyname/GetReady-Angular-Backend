namespace GetReady.Services.Contracts
{
    public interface ISeederService
    {
        void SeedGlobalRoot();
        void SeedAdmin();
        void Seed123();
    }
}
