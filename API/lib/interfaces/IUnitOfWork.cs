

using API.DTO;
using API.LIB.HELPERS;
using API.Models;
using CloudinaryDotNet.Actions;

namespace API.LIB.INTERFACES;


public interface IUnitOfWork{

    IUserRepository UserRepository { get; }
    IMessageRepository MessageRepository { get; }
    ILikesRepository LikesRepository { get; }

    Task<bool> Complete();
    bool HasChanges(); //ENTITY FRAMEOWKR TRACKING CHANGES IN TRANSACITONS
}