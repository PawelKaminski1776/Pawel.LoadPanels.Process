using NServiceBus;
using Pawel.LoadPanels.Contracts.ViewDtos;
using BowlingSys.Services.UserService;
using BowlingSys.Entities.UserDBEntities;
using System;

namespace BowlingSys.Handlers.Handlers
{
    public class MyMessageHandler : IHandleMessages<ViewDto>
    {
        private readonly ViewService _viewService;

        public MyMessageHandler(ViewService viewService)
        {
            _viewService = viewService;
        }

        public async Task Handle(ViewDto message, IMessageHandlerContext context)
        {
            try
            {

                var Result = await _viewService.CallGetAllPanelViews();
            

                await context.Reply(Result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred while processing the message: {ex.Message}");

                throw; 
            }
        }

       


    }
}
