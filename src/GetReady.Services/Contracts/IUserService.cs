namespace GetReady.Services.Contracts
{
    using Models.UserModels;

    public interface IUserService
    {
        UserWithTokenDto Register(UserRegisterDto data, bool admin = false);

        UserWithTokenDto Login(UserLoginDto data);
    }
}
