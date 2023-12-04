using App.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trivesta.Data.Interface;
using Trivesta.Model.ViewModel;

namespace Trivesta.Business
{
    public class GeneralBusiness
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly RoomBusiness _roomBusiness;
        public GeneralBusiness(IUnitOfWork unitOfWork, RoomBusiness roomBusiness)
        {
            _unitOfWork = unitOfWork;
            _roomBusiness = roomBusiness;
        }
        public async Task<HomeVM> GetHomeVM()
        {
            HomeVM homeVM = new HomeVM();
            RoomsVM roomsVM = new RoomsVM();
            roomsVM.DefaultRooms = await _unitOfWork.Rooms.DefaultRooms("Public");
            homeVM.RoomsVM = roomsVM;
            homeVM.PbKey = FlutterwaveServices.PbKey;

            return homeVM;
        }
    }
}
