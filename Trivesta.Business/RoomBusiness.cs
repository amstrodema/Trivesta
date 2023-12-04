using App.Services;
using Azure;
using Microsoft.AspNetCore.Http;
using System.Threading;
using Trivesta.Data.Interface;
using Trivesta.Model;
using Trivesta.Model.ViewModel;

namespace Trivesta.Business
{
    public class RoomBusiness
    {
        private readonly IUnitOfWork _unitOfWork;

        public RoomBusiness(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Room> Get(string roomCodeTagOrID) => await _unitOfWork.Rooms.GetByRoomCodeTagOrID(roomCodeTagOrID);
        public async Task<RoomType> GetType(string tag) => await _unitOfWork.RoomTypes.GetByTag(tag);
        public async Task<IEnumerable<Room>> Get() => await _unitOfWork.Rooms.GetAll();
        public async Task<Room> RandomRoom()
        {
            var rooms = await _unitOfWork.Rooms.GetAll(); 
            rooms = GenericService.Shuffle(rooms);

            return rooms[0];
        }
        public async Task<RoomsVM> GetRoomVM(string t)
        {
            RoomsVM roomsVM = new RoomsVM();
            roomsVM.Room = await Get(t);
            roomsVM.RoomType = await GetType(roomsVM.Room.RoomTypeTag);

            return roomsVM;
        }
        public async Task<RoomsVM> GetRoomsVM()
        {
            RoomsVM roomsVM = new RoomsVM();
            roomsVM.Rooms = await _unitOfWork.Rooms.NotDefaultRooms();
            roomsVM.DefaultRooms = await _unitOfWork.Rooms.DefaultRooms("Public");
            return roomsVM;
        }
        public async Task<RoomsVM> GetRoomsVM(string roomTypeTag)
        {
            RoomsVM roomsVM = new RoomsVM();
            roomsVM.Rooms = await _unitOfWork.Rooms.NotDefaultRooms(roomTypeTag);
            roomsVM.DefaultRooms = await _unitOfWork.Rooms.DefaultRooms(roomTypeTag);
            return roomsVM;
        }
        public async Task<ResponseMessage<RoomType>> Create(RoomsVM roomsVM, User user, IFormFile image)
        {
            ResponseMessage<RoomType> responseMessage = new ResponseMessage<RoomType>();
            try
            {
                Room room = roomsVM.Room;
                RoomType roomType = await GetType(room.RoomTypeTag);
                responseMessage.Data = roomType;
                var allTransactions = await _unitOfWork.CoinTransactions.GetByUserID(user.ID);
                var bal = allTransactions.Where(p => p.IsCredit).Sum(o => o.Amount) - allTransactions.Where(p => !p.IsCredit).Sum(o => o.Amount);
                decimal bill = 0;

                if (string.IsNullOrEmpty(room.RentFreq))
                {
                    room.RentFreq = string.Empty;
                }

                if (string.IsNullOrEmpty(room.Name) || string.IsNullOrEmpty(room.Description) || image == null)
                {
                    responseMessage.StatusCode = 201;
                    responseMessage.Message = "Complete all details!";
                    return responseMessage;
                }

                if (roomType.IsMustCharge)
                    bill = roomType.Cost;
                else if (!room.IsFree)
                    bill = room.RoomCost;

                bal = bal - bill;
                room.Image = await ImageService.SaveImageInFolder(image, room.ID.ToString(), "Room");

                if (bal < 0)
                {
                    user.CoinBonus -= bal;
                    if (user.CoinBonus < 0)
                    {
 responseMessage.StatusCode = 201;
                    responseMessage.Message = "Insufficient coins";
                    return responseMessage;
                    }
                    _unitOfWork.Users.Update(user);
                }

                if (bill != 0 && room.RentAmt > (bill * 10 / 100))
                {
                    responseMessage.StatusCode = 201;
                    responseMessage.Message = "Rent higher than threshold!";
                    return responseMessage;
                }

                room.ID = Guid.NewGuid();
                room.DateCreated = DateTime.UtcNow.AddHours(1);
                room.CreatedBy = user.ID;
                room.IsActive = true;

                do
                {
                    room.Code = GenService.Gen10DigitCode();
                } while (await _unitOfWork.Rooms.GetByRoomCode(room.Code) != null);

                if (string.IsNullOrEmpty(user.Email))
                {
                    room.IsFake = true;
                }
                if (!user.IsAdmin)
                {
                    room.IsDefault = false;
                }

                if (room.IsDefault)
                    room.Tag = GenericService.GetTag(roomType.Name + " " + room.Name);
                else
                    room.Tag = GenericService.GetTag(room.Name + " " + room.Code);

                var coinTrx = new CoinTransaction()
                {
                    ID = Guid.NewGuid(),
                    Amount = bill,
                    IsCredit = false,
                    DateCreated = DateTime.UtcNow.AddHours(1),
                    UserID = user.ID
                };

                await _unitOfWork.Rooms.Create(room);

                if (coinTrx.Amount > 0)
                {
                    await _unitOfWork.CoinTransactions.Create(coinTrx);
                }


                if (await _unitOfWork.Commit() > 0)
                {
                    responseMessage.StatusCode = 200;
                    responseMessage.Message = "Room created!";
                }
                else
                {
                    responseMessage.StatusCode = 201;
                    responseMessage.Message = "Room failed";
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

        public async Task<ResponseMessage<RoomsVM>> ChatRoomsVM(string roomCode, User thisUser)
        {
            ResponseMessage<RoomsVM> responseMessage = new ResponseMessage<RoomsVM>();
            try
            {
                RoomsVM roomsVM = new RoomsVM();
                var room = await Get(roomCode);
                if (room == null || room.IsBarred || !room.IsActive )
                {
                    responseMessage.StatusCode = 201;
                    responseMessage.Message = "Room not found or inactive";
                    return responseMessage;
                }

                var roomType = await GetType(room.RoomTypeTag);
                var monitor = await _unitOfWork.LoginMonitors.GetMonitorByUserIDOrGuest(thisUser.ID, thisUser.AppCode, thisUser.Username);
                var checkUser = await _unitOfWork.RoomMembers.GetByUserID(thisUser.ID);
                //if (thisUser.IsMailVerified)
                //{
                //    thisUser = await _unitOfWork
                //}

                if (monitor == null)
                {
                    responseMessage.StatusCode = 201;
                    responseMessage.Message = "Session expired!";
                    return responseMessage;
                }

                if (roomType.IsApprovalRequired)
                {
                   var response = await HandlePersonalGroup(thisUser, room, roomType, checkUser, monitor);
                    if (response.StatusCode != 200)
                    {
                        responseMessage.StatusCode = response.StatusCode;
                        responseMessage.Message = response.Message;
                        return responseMessage;
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(room.AgeLimit) || !string.IsNullOrEmpty(room.GenderLimit) || !string.IsNullOrEmpty(room.RelationshipLimit) || !string.IsNullOrEmpty(room.ReligiousLimit))
                    {
                        if (!thisUser.IsAdmin && !thisUser.IsDev && (thisUser.DOB == default || string.IsNullOrEmpty(thisUser.Email)))
                        {
                            responseMessage.StatusCode = 201;
                            responseMessage.Message = "Complete profile required";
                            return responseMessage;
                        }
                    }
                    if (room.RentAmt > 0)
                    {
                        var res = await ChargeForRooms(room, thisUser, monitor);
                        if (!res)
                        {
                            responseMessage.StatusCode = 201;
                            responseMessage.Message = "Insufficient coins";
                            return responseMessage;
                        }

                        if (await _unitOfWork.Commit() < 1)
                        {
                            responseMessage.StatusCode = 201;
                            responseMessage.Message = "Access denied!";
                            return responseMessage;
                        }
                    }
                }

                //if (thisUser == null && roomType.IsMustCharge)
                //{
                //    responseMessage.StatusCode = 201;
                //    responseMessage.Message = "Login is required";
                //    return responseMessage;
                //}

                roomsVM.Creator = await _unitOfWork.Users.Find(room.CreatedBy);
                roomsVM.Room = room;
                responseMessage.StatusCode = 200;
                responseMessage.Data = roomsVM;

            }
            catch (Exception e)
            {
                FileService.WriteToFile("\n\n" + e, "ErrorLogs");
                responseMessage.Data = new RoomsVM();
                responseMessage.StatusCode = 209;
                responseMessage.Message = "Failed";
            }

            return responseMessage;
        }
        private async Task<ResponseMessage<string>> HandlePersonalGroup( User thisUser, Room room, RoomType roomType, RoomMember checkUser, LoginMonitor monitor)
        {
            ResponseMessage<string> responseMessage = new ResponseMessage<string>();
            try
            {
                if (checkUser == null)
                {
                    if (room.RentAmt > 0)
                    {
                        var res = await ChargeForRooms(room, thisUser, monitor);
                        if (!res)
                        {
                            responseMessage.StatusCode = 201;
                            responseMessage.Message = "Insufficient coins";
                            return responseMessage;
                        }
                    }

                    RoomMember roomMember = new RoomMember()
                    {
                        ID = Guid.NewGuid(),
                        DateCreated = DateTime.UtcNow.AddHours(1),
                        Duration = roomType.DecayGraceInMinutes,
                        RoomID = room.ID,
                        UserID = thisUser.ID
                    };
                    await _unitOfWork.RoomMembers.Create(roomMember);

                    if (await _unitOfWork.Commit() < 1)
                    {
                        throw new Exception();
                    }
                    responseMessage.StatusCode = 201;
                    responseMessage.Message = "Request submitted";
                }
                else if (!checkUser.IsApproved)
                {
                    responseMessage.StatusCode = 201;
                    responseMessage.Message = "Pending approval";
                }
                else
                {
                    responseMessage.StatusCode = 200;
                }
            }
            catch (Exception)
            {
                responseMessage.StatusCode = 209;
                responseMessage.Message = "Failed Request";
                return responseMessage;
            }

            return responseMessage;
        }
   
        private async Task<bool> ChargeForRooms(Room room, User thisUser, LoginMonitor monitor)
        {
            var allTransactions = await _unitOfWork.CoinTransactions.GetByUserID(thisUser.ID);
            var bal = allTransactions.Where(p => p.IsCredit).Sum(o => o.Amount) - allTransactions.Where(p => !p.IsCredit).Sum(o => o.Amount);
            var balHolder = bal;
            bal = bal - room.RentAmt;

            if (bal < 0)
            {
                if (thisUser.IsMailVerified)
                {
                    thisUser.CoinBonus += bal;
                    _unitOfWork.Users.Update(thisUser);
                }
                else
                {
                    thisUser.CoinBonus = monitor.GuestCoin += bal;
                    _unitOfWork.LoginMonitors.Update(monitor);
                }

                if (thisUser.CoinBonus < 0)
                {
                    return false;
                }
              
            }

            if (thisUser.IsMailVerified && balHolder > 0 && balHolder != bal)
            {
                var coinTrx = new CoinTransaction()
                {
                    ID = Guid.NewGuid(),
                    Amount = bal <= 0 ? balHolder : balHolder - bal,
                    IsCredit = false,
                    DateCreated = DateTime.UtcNow.AddHours(1),
                    UserID = thisUser.ID
                };

                await _unitOfWork.CoinTransactions.Create(coinTrx);
            }

            return true;
        }
    }
}