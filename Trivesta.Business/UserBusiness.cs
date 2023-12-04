using App.Services;
using Trivesta.Data.Interface;
using Trivesta.Model;

namespace Trivesta.Business
{
    public class UserBusiness
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserBusiness(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<ResponseMessage<string>> Create(string email, string username, string password)
        {
            ResponseMessage<string> responseMessage = new ResponseMessage<string>();

            try
            {
                if (string.IsNullOrEmpty(password)  || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(email))
                {
                    responseMessage.StatusCode = 201;
                    responseMessage.Message = "Account not created. Complete all details and try again";
                    return responseMessage;
                }
                if(username.ToLower() == "admin" || username.ToLower() == "user" || username.ToLower() == "moderator")
                {
                    responseMessage.StatusCode = 201;
                    responseMessage.Message = "Invalid username";
                    return responseMessage;
                }

                if (await _unitOfWork.Users.GetUserByUserNameOrEmail(username, email) != null)
                {
                    responseMessage.StatusCode = 201;
                    responseMessage.Message = "Account not created. User exists already";
                    return responseMessage;
                }

                User user = new User()
                {
                    ID = Guid.NewGuid(),
                    Username = username,
                    Email = email,
                    Password = EncryptionService.Encrypt(password),
                    DateCreated = DateTime.UtcNow.AddHours(1),
                    IsActive = true,
                    CoinBonus = 15000
                };
                await _unitOfWork.Users.Create(user);
                if (await _unitOfWork.Commit() > 0)
                {
                    responseMessage.StatusCode = 200;
                    responseMessage.Message = "Account created successfully!";
                }
                else
                {
                    responseMessage.StatusCode = 201;
                    responseMessage.Message = "Account not created. Try again";
                }
            }

            catch (Exception e)
            {
                FileService.WriteToFile("\n\n" + e, "ErrorLogs");
                responseMessage.StatusCode = 209;
                responseMessage.Message = "Failed";
            }
            return responseMessage;
        }

        public async Task<ResponseMessage<User>> Guest()
        {
            ResponseMessage<User> responseMessage = new ResponseMessage<User>();
            try
            {
                User guest = new User()
                {
                    ID = Guid.NewGuid(),
                    CoinBonus = 5000,
                    Username = "#Guest-"+GenService.Gen10DigitNumCode(),
                    DateCreated = DateTime.UtcNow.AddHours(1),
                    IsActive = true
                };

                User thisUser = await _unitOfWork.Users.GetUserByUserNameOrEmail(guest.Username);
                var monitor = await _unitOfWork.LoginMonitors.GetMonitorByGuest(guest.Username);

                if (thisUser != null || monitor != null)
                {
                    responseMessage.StatusCode = 201;
                    responseMessage.Message = "User exists!";
                    return responseMessage;
                }

                LoginMonitor loginMonitor = new LoginMonitor()
                {
                    ClientCode = Guid.NewGuid(),
                    ID = Guid.NewGuid(),
                    Guest = guest.Username,
                    IsActive = true,
                    TimeLogged = DateTime.UtcNow.AddHours(1),
                    DateCreated = DateTime.UtcNow.AddHours(1),
                    GuestCoin = 5000
                };
                guest.AppCode = loginMonitor.ClientCode;

                await _unitOfWork.LoginMonitors.Create(loginMonitor);

                if (await _unitOfWork.Commit() > 0)
                {
                    responseMessage.Data = guest;
                    responseMessage.StatusCode = 200;
                }
                else
                {
                    responseMessage.StatusCode = 201;
                    responseMessage.Message = "Server yawned! Try later";
                }
            }
            catch (Exception)
            {
                responseMessage.StatusCode = 209;
                responseMessage.Message = "Server dozed off. Try Again";
            }

            return responseMessage;
        }
        public async Task<ResponseMessage<User>> Login(string emailOrUsername, string password)
        {
            //_context.HttpContext.Session.SetString("isDev", "true");
            ResponseMessage<User> responseMessage = new ResponseMessage<User>();

            try
            {
                if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(emailOrUsername))
                {
                    responseMessage.StatusCode = 201;
                    responseMessage.Message = "Login failed. Complete all details and try again";
                    return responseMessage;
                }

                User user = await _unitOfWork.Users.GetUserByUserNameOrEmail(emailOrUsername);
               
                if (user == null)
                {
                    responseMessage.StatusCode = 201;
                    responseMessage.Message = "Login failed. Incorrect login details!";
                }
                else if (!user.IsMailVerified)
                {
                    responseMessage.StatusCode = 201;
                    responseMessage.Message = "Verify your email and try again";
                    return responseMessage;
                }
                else if (user.IsBanned || !user.IsActive)
                {
                    responseMessage.StatusCode = 201;
                    responseMessage.Message = "User is deactivated or banned. Contact administrator";
                }
                else if (EncryptionService.Validate(password, user.Password))
                {
                    //LoginMonitor

                    var monitor = await Task.Run(() => _unitOfWork.LoginMonitors.GetMonitorByUserIDOnly(user.ID));

                    if (monitor != null)
                    {
                        _unitOfWork.LoginMonitors.Delete(monitor);
                    }

                    LoginMonitor loginMonitor = new LoginMonitor()
                    {
                        ClientCode = Guid.NewGuid(),
                        ID = Guid.NewGuid(),
                        UserID = user.ID,
                        IsActive = true,
                        TimeLogged = DateTime.UtcNow.AddHours(1),
                        DateCreated = DateTime.UtcNow.AddHours(1)
                    };

                    await _unitOfWork.LoginMonitors.Create(loginMonitor);

                    if (await _unitOfWork.Commit() > 0)
                    {
                        user.AppCode = loginMonitor.ClientCode;
                        responseMessage.Data = user;
                        responseMessage.StatusCode = 200;
                        responseMessage.Message = "Logged in successfully!";
                    }
                    else
                    {
                        responseMessage.StatusCode = 201;
                        responseMessage.Message = "Login not successful. Try Again";
                    }


                }
                else
                {
                    responseMessage.StatusCode = 201;
                    responseMessage.Message = "Login not successful. Try Again";
                }
            }
            catch (Exception e)
            {
                FileService.WriteToFile("\n\n" + e, "ErrorLogs");
                responseMessage.StatusCode = 209;
                responseMessage.Message = "Invalid credentials. Try Again";
            }

            return responseMessage;
        }

        public async Task<ResponseMessage<int>> LogOut(Guid userID, Guid appID)
        {
            ResponseMessage<int> responseMessage = new ResponseMessage<int>();
            try
            {
                LoginMonitor loginMonitor = await _unitOfWork.LoginMonitors.GetMonitorByUserID(userID, appID);
                _unitOfWork.LoginMonitors.Delete(loginMonitor);

                if (await _unitOfWork.Commit() > 0)
                {
                    responseMessage.StatusCode = 200;
                }
                else
                {
                    responseMessage.StatusCode = 201;
                }
            }
            catch (Exception e)
            {
                FileService.WriteToFile("\n\n" + e, "ErrorLogs");
                responseMessage.StatusCode = 209;
            }
            return responseMessage;
        }


        public async Task<ResponseMessage<string>> Setup()
        {
            ResponseMessage<string> responseMessage = new ResponseMessage<string>();
            try
            {
                var user = await _unitOfWork.Users.GetActiveUserByUserName("Amstrodema");
                if (user != null)
                {
                    responseMessage.StatusCode = 200;
                    responseMessage.Message = "Setup completed already!";
                    return responseMessage;
                }
                var admin = new User()
                {
                    ID = Guid.NewGuid(),
                    AppCode = Guid.NewGuid(),
                    CoinBonus = 50000000,
                    DOB = DateTime.Parse("05/02/1900"),
                    Email = "yk4love38@gmail.com",
                    Password = EncryptionService.Encrypt("Qw!!0001"),
                    IsActive = true,
                    IsAdmin = true,
                    IsDev = true,
                    IsMailVerified = true,
                    IsMale = true,
                    Username = "Amstrodema",
                    DateCreated = DateTime.UtcNow.AddHours(1)
                };

                var typePublic = new RoomType()
                {
                    ID = Guid.NewGuid(),
                    Cost = 100,
                    Name = "Public",
                    Tag = "Public",
                    Image = "../assets/images/slider-dec.png",
                    CreatedBy = admin.ID,
                    Description = "This is like stepping into a vibrant virtual hangout that's open to you and everyone else, including guests.",
                    IsActive = true,
                    DateCreated = DateTime.UtcNow.AddHours(1)
                };

                var typePrivate = new RoomType()
                {
                    ID = Guid.NewGuid(),
                    Cost = 1000,
                    Name = "Private",
                    Tag = "Private",
                    Image = "../assets/images/slider-dec.png",
                    CreatedBy = admin.ID,
                    Description = "This room is open to you and everyone else, except guests. Owners may or may not demand rent from users",
                    IsActive = true,
                    DateCreated = DateTime.UtcNow.AddHours(1),
                    IsMustLogIn = true
                };
                var typeClassy = new RoomType()
                {
                    ID = Guid.NewGuid(),
                    Cost = 10000,
                    Name = "Classy",
                    Tag = "Classy",
                    Image = "../assets/images/slider-dec.png",
                    CreatedBy = admin.ID,
                    Description = "Higher standard for users who appreciate elevated vibes. It's a space where exclusivity meets sophistication. The Bars are high!",
                    IsActive = true,
                    DateCreated = DateTime.UtcNow.AddHours(1),
                    IsMustCharge = true,
                    IsMustLogIn = true
                };

                var typePersonal = new RoomType()
                {
                    ID = Guid.NewGuid(),
                    Cost = 5000,
                    Name = "Personal",
                    Tag = "Personal",
                    Image = "../assets/images/slider-dec.png",
                    CreatedBy = admin.ID,
                    Description = "Cozy corners designed just for you. Intimate one-on-one or small group conversations, ensuring a confidential setting for personal talks",
                    IsActive = true,
                    DateCreated = DateTime.UtcNow.AddHours(1),
                    IsMustCharge = true,
                    IsApprovalRequired = true
                    
                };

                var publicRooms = SetPublicRooms(typePublic, admin);
                var privateRooms = SetPrivateRooms(typePrivate, admin);
                var classyRooms = SetClassyRooms(typeClassy, admin);

                await _unitOfWork.Users.Create(admin);
                await _unitOfWork.RoomTypes.Create(typePublic);
                await _unitOfWork.RoomTypes.Create(typePrivate);
                await _unitOfWork.RoomTypes.Create(typePersonal);
                await _unitOfWork.RoomTypes.Create(typeClassy);
                await _unitOfWork.Rooms.CreateMultiple(publicRooms.ToArray());
                await _unitOfWork.Rooms.CreateMultiple(privateRooms.ToArray());
                await _unitOfWork.Rooms.CreateMultiple(classyRooms.ToArray());

                if (await _unitOfWork.Commit() > 0)
                {
                    responseMessage.StatusCode = 200;
                    responseMessage.Message = "Setup completed!";
                }
                else
                {
                    responseMessage.StatusCode = 201;
                    responseMessage.Message = "Setup failed";
                }
            }
            catch (Exception e)
            {
                FileService.WriteToFile("\n\n" + e, "ErrorLogs");
                responseMessage.StatusCode = 209;
                responseMessage.Message = "Failed";
            }
            return responseMessage;
        }
        private List<Room> SetPublicRooms(RoomType roomType, User user)
        {
            return new List<Room>()
            {
                new Room()
                {
                    ID = Guid.NewGuid(),
                    Name = "General",
                    IsDefault = true,
                    Code = GenService.Gen10DigitCode(),
                    Tag = GenericService.GetTag(roomType.Name+" General Room"),
                    IsFree = true,
                    RentType =  "Free",
                    CreatedBy = user.ID,
                    RoomType = roomType.Name,
                    RoomTypeTag = roomType.Tag,
                    RoomTypeID = roomType.ID,
                    IsActive = true,
                    DateCreated = DateTime.UtcNow.AddHours(1),
                    Description = "Everyone is here. Watch your back in this free space, the tides are high and somehow all our guests have no faces... keep the fun high!!"
                },
                new Room()
                {
                    ID = Guid.NewGuid(),
                    Name = "Friendship",
                    IsDefault = true,
                    Code = GenService.Gen10DigitCode(),
                    Tag = GenericService.GetTag(roomType.Name+" Friendship Room"),
                    IsFree = true,
                    RentType =  "Free",
                    CreatedBy = user.ID,
                    RoomType = roomType.Name,
                    RoomTypeTag = roomType.Tag,
                    RoomTypeID = roomType.ID,
                    IsActive = true,
                    DateCreated = DateTime.UtcNow.AddHours(1),
                    Description = "Sometimes a stranger can be that angel. There's no need to do life alone anymore... Stay sharp and watchout for burglars!"
                },
                new Room()
                {
                    ID = Guid.NewGuid(),
                    Name = "Banter",
                    IsDefault = true,
                    Code = GenService.Gen10DigitCode(),
                    Tag = GenericService.GetTag(roomType.Name+" Banter Room"),
                    IsFree = true,
                    RentType =  "Free",
                    CreatedBy = user.ID,
                    RoomType = roomType.Name,
                    RoomTypeTag = roomType.Tag,
                    RoomTypeID = roomType.ID,
                    IsActive = true,
                    DateCreated = DateTime.UtcNow.AddHours(1),
                    Description = "Be silly and tease one another... but if it gets messy try not to cry!! Remember, some of our guests are faceless."
                },
                new Room()
                {
                    ID = Guid.NewGuid(),
                    Name = "Lovers",
                    IsDefault = true,
                    Code = GenService.Gen10DigitCode(),
                    Tag = GenericService.GetTag(roomType.Name+" Lovers Room"),
                    IsFree = false,
                    RentType =  "Paid",
                    CreatedBy = user.ID,
                    RoomType = roomType.Name,
                    RoomTypeTag = roomType.Tag,
                    RoomTypeID = roomType.ID,
                    IsActive = true,
                    DateCreated = DateTime.UtcNow.AddHours(1),
                    Description = "Make it rain love here. Stay decent and ride on the eagles back... Keep your eyes open",
                    RoomCost = 0,
                    RentAmt = 100,
                    RentFreq = "Hourly"
                },
                new Room()
                {
                    ID = Guid.NewGuid(),
                    Name = "Sport",
                    IsDefault = true,
                    Code = GenService.Gen10DigitCode(),
                    Tag = GenericService.GetTag(roomType.Name + " Sport Room"),
                    IsFree = true,
                    RentType =  "Free",
                    CreatedBy = user.ID,
                    RoomType = roomType.Name,
                    RoomTypeTag = roomType.Tag,
                    RoomTypeID = roomType.ID,
                    IsActive = true,
                    DateCreated = DateTime.UtcNow.AddHours(1),
                    Description = "Keep it bubbling with your favorite sport gists! Nothing beats talking sport with the world...",
                    RoomCost = 0,
                    RentAmt = 100,
                    RentFreq = "Hourly"
                }
            };
        }
        private List<Room> SetPrivateRooms(RoomType roomType, User user)
        {
          return  new List<Room>()
            {
                new Room()
                {
                    ID = Guid.NewGuid(),
                    Name = "General",
                    IsDefault = true,
                    Code = GenService.Gen10DigitCode(),
                    Tag = GenericService.GetTag(roomType.Name+" General Room"),
                    IsFree = true,
                    RentType =  "Free",
                    CreatedBy = user.ID,
                    RoomType = roomType.Name,
                    RoomTypeTag = roomType.Tag,
                    RoomTypeID = roomType.ID,
                    IsActive = true,
                    DateCreated = DateTime.UtcNow.AddHours(1),
                    Description = "Everyone is here. Watch your back still in this space, the tides are high and somehow some of our guests have many faces... keep the fun high!!"
                },
                new Room()
                {
                    ID = Guid.NewGuid(),
                    Name = "Friendship",
                    IsDefault = true,
                    Code = GenService.Gen10DigitCode(),
                    Tag = GenericService.GetTag(roomType.Name+" Friendship Room"),
                    IsFree = true,
                    RentType =  "Free",
                    CreatedBy = user.ID,
                    RoomType = roomType.Name,
                    RoomTypeTag = roomType.Tag,
                    RoomTypeID = roomType.ID,
                    IsActive = true,
                    DateCreated = DateTime.UtcNow.AddHours(1),
                    Description = "Sometimes a stranger can be that angel. There's no need to do life alone anymore... Stay sharp and watchout for burglars!"
                },
                new Room()
                {
                    ID = Guid.NewGuid(),
                    Name = "Banter",
                    IsDefault = true,
                    Code = GenService.Gen10DigitCode(),
                    Tag = GenericService.GetTag(roomType.Name+" Banter Room"),
                    IsFree = true,
                    RentType =  "Free",
                    CreatedBy = user.ID,
                    RoomType = roomType.Name,
                    RoomTypeTag = roomType.Tag,
                    RoomTypeID = roomType.ID,
                    IsActive = true,
                    DateCreated = DateTime.UtcNow.AddHours(1),
                    Description = "Be silly and tease one another... but if it gets messy try not to cry!! Remember, some of our guests are faceless."
                },
                new Room()
                {
                    ID = Guid.NewGuid(),
                    Name = "Lovers",
                    IsDefault = true,
                    Code = GenService.Gen10DigitCode(),
                    Tag = GenericService.GetTag(roomType.Name+" Lovers Room"),
                    IsFree = false,
                    RentType =  "Paid",
                    CreatedBy = user.ID,
                    RoomType = roomType.Name,
                    RoomTypeTag = roomType.Tag,
                    RoomTypeID = roomType.ID,
                    IsActive = true,
                    DateCreated = DateTime.UtcNow.AddHours(1),
                    Description = "Make it rain love here. Stay decent and ride on the eagles back... Keep your eyes open",
                    RoomCost = 0,
                    RentAmt = 100,
                    RentFreq = "Hourly",
                    AgeLimit = "18"
                },
                new Room()
                {
                    ID = Guid.NewGuid(),
                    Name = "Sport",
                    IsDefault = true,
                    Code = GenService.Gen10DigitCode(),
                    Tag = GenericService.GetTag(roomType.Name + " Sport Room"),
                    IsFree = true,
                    RentType =  "Free",
                    CreatedBy = user.ID,
                    RoomType = roomType.Name,
                    RoomTypeTag = roomType.Tag,
                    RoomTypeID = roomType.ID,
                    IsActive = true,
                    DateCreated = DateTime.UtcNow.AddHours(1),
                    Description = "Keep it bubbling with your favorite sport gists! Nothing beats talking sport with the world...",
                    RoomCost = 0,
                    RentAmt = 100,
                    RentFreq = "Hourly"
                }
            };
        }
        private List<Room> SetClassyRooms(RoomType roomType, User user)
        {
            return new List<Room>()
            {
                new Room()
                {
                    ID = Guid.NewGuid(),
                    Name = "General",
                    IsDefault = true,
                    Code = GenService.Gen10DigitCode(),
                    Tag = GenericService.GetTag(roomType.Name+" General Room"),
                    RentType =  "Paid",
                    CreatedBy = user.ID,
                    RoomType = roomType.Name,
                    RoomTypeTag = roomType.Tag,
                    RoomTypeID = roomType.ID,
                    IsActive = true,
                    DateCreated = DateTime.UtcNow.AddHours(1),
                    Description = "Everyone wishes to be here, so the standard was programmed to keep going up to keep the noise out. Only those who can afford the bling get to roll!",
                    RoomCost = 0,
                    RentAmt = 5000,
                    RentFreq = "Hourly"
                },
                new Room()
                {
                    ID = Guid.NewGuid(),
                    Name = "Friendship",
                    IsDefault = true,
                    Code = GenService.Gen10DigitCode(),
                    Tag = GenericService.GetTag(roomType.Name+" Friendship Room"),
                    RentType =  "Paid",
                    CreatedBy = user.ID,
                    RoomType = roomType.Name,
                    RoomTypeTag = roomType.Tag,
                    RoomTypeID = roomType.ID,
                    IsActive = true,
                    DateCreated = DateTime.UtcNow.AddHours(1),
                    Description = "Sometimes we want friends who can vibe at our own pace. As the standard keeps going up, hawks choke and fall off our back!",
                    RoomCost = 0,
                    RentAmt = 5000,
                    RentFreq = "Hourly"
                },
                new Room()
                {
                    ID = Guid.NewGuid(),
                    Name = "Lovers",
                    IsDefault = true,
                    Code = GenService.Gen10DigitCode(),
                    Tag = GenericService.GetTag(roomType.Name+" Lovers Room"),
                    RentType =  "Paid",
                    CreatedBy = user.ID,
                    RoomType = roomType.Name,
                    RoomTypeTag = roomType.Tag,
                    RoomTypeID = roomType.ID,
                    IsActive = true,
                    DateCreated = DateTime.UtcNow.AddHours(1),
                    Description = "Make it rain love here. Stay decent and ride on the eagles back... Keep your eyes open",
                    RoomCost = 0,
                    RentAmt = 5000,
                    RentFreq = "Daily",
                    AgeLimit = "18"
                },
                new Room()
                {
                    ID = Guid.NewGuid(),
                    Name = "Sport",
                    IsDefault = true,
                    Code = GenService.Gen10DigitCode(),
                    Tag = GenericService.GetTag(roomType.Name + " Sport Room"),
                    IsFree = true,
                    RentType =  "Free",
                    CreatedBy = user.ID,
                    RoomType = roomType.Name,
                    RoomTypeTag = roomType.Tag,
                    RoomTypeID = roomType.ID,
                    IsActive = true,
                    DateCreated = DateTime.UtcNow.AddHours(1),
                    Description = "Keep it bubbling with your favorite sport gists! Nothing beats talking sport with the world...",
                    RoomCost = 0,
                    RentAmt = 100,
                    RentFreq = "Hourly"
                }
            };
        }
    }
}